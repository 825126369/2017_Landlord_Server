#include "stdafx.h"
#include "game_player.h"
#include "check_account_task.h"
#include <net/packet_manager.h>
#include "world_server.h"
#include <enable_crypto.h>
#include <enable_json_map.h>
#include <enable_random.h>
#include <boost/lexical_cast.hpp>
#include "proc_c2w_lobby_protocol.h"
#include "proc_logic2world_protocol.h"
#include "proc_shop.h"
#include "proc_world_packet.h"
#include "servers_manager.h"
#include "backstage_manager.h"
#include "game_player_mgr.h"
#include "time_helper.h"
#include "M_RechangeCFG.h"
#include "game_db.h"
#include "M_BaseInfoCFG.h"
#include "game_sys_recharge.h"
#include "name_valid.h"
#include "msg_type_def.pb.h"
#include "gift_def.h"
#include "M_GiftCFG.h"
#include "M_ItemCFG.h"
#include "bag_mgr.h"
#include "mail_sys.h"
#include "global_sys_mgr.h"
#include "M_MultiLanguageCFG.h"
#include "id_generator_sys.h"
#include "pump_type.pb.h"
#include "pump_sys.h"
#include "gm_sys.h"
#include "operation_activity_sys.h"
#include "operation_activity_type.h"
#include <boost/algorithm/string.hpp>
#include "game_check.h"
#include <boost/regex.hpp>
#include "chat_sys.h"
#include "robots_sys.h"
#include "game_quest_mgr.h"
#include "game_quest.h"
#include "game_db_log.h"
#include "M_RobotNameCFG.h"

//enable_obj_pool_init(game_player, boost::null_mutex);

static const std::string AES_KEY = "&@*(#kas9081fajk";
static const std::string RAND_KEY = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

using namespace boost;

static void getNewAccountMail(std::string& title, std::string& sender, std::string& content)
{
	// 新建账号发封邮件
	const M_MultiLanguageCFGData* data = M_MultiLanguageCFG::GetSingleton()->GetData("Mail_Title");
	if(data)
	{
		title = data->mName;
	}

	data = M_MultiLanguageCFG::GetSingleton()->GetData("Mail_From");
	if(data)
	{
		sender = data->mName;
	}

	data = M_MultiLanguageCFG::GetSingleton()->GetData("Mail_Text");
	if(data)
	{
		content = data->mName;
	}
}

game_player::game_player()
	:m_sessionid(0)
	,m_logicid(0)
	,m_state(e_ps_none)
	,m_gameid(0)
	,m_check_logout(0)
	,LastGameChatTime(0)
	,m_check_save(0)
	,islogin_success(false)
	,m_login_time(0)
	,m_channel(0)
	,m_robotVipTime(0)
{
	init_sys();
	init_game_object();
}

game_player::~game_player()
{
	SLOG_CRITICAL << "销毁玩家" << std::endl;
	on_logout();
	sys_exit();
}


void game_player::heartbeat( double elapsed )
{
	if(m_state == e_ps_disconnect)
	{
		m_check_logout+=elapsed;
		if(m_check_logout > 400)//秒
		{
			game_player_mgr::instance().set_del_player(shared_from_this());
			m_check_logout = 0;
		}
	}
	else
	{
		m_check_save+=elapsed;
		if(m_check_save > 1)//秒
		{
			store_game_object();
			m_check_save = 0;
		}
	}

	sys_update(elapsed);

	heartbeatRobotVip(elapsed);

}

void game_player::on_logout()
{	
	auto peer =get_gate();
	if(peer)
	{
		auto msg =  PACKET_CREATE(packet_clear_session, e_mst_clear_session);		
		msg->set_sessionid(m_sessionid);
		peer->send_msg(msg);
	}		

	time_t curTime = time_helper::instance().get_cur_time();
	if (m_login_time != 0)
	{
		time_t online_time = curTime - m_login_time;
		if (online_time > 0)
		{
			OnlineTime->add_value(online_time);
		}
	}

	LogoutTime->set_value(curTime);
	store_game_object();
	

	if(IsRobot->get_value())
	{
		GLOBAL_SYS(RobotsSys)->release_robot(PlayerId->get_value(), get_viplvl(), Gold->get_value());
	}
}

void game_player::player_logout()
{
	m_state = e_ps_disconnect;
	m_check_logout = 0;
	if(check_logic())
	{
		auto msg =  PACKET_CREATE(packet_player_disconnect, e_mst_player_disconnect);		
		msg->set_sessionid(m_sessionid);
		auto peer = get_logic();
		peer->send_msg(msg);
	}
	else
	{
		game_player_mgr::instance().set_del_player(shared_from_this());
	}
}

void game_player::player_login(uint32_t sessionid, const std::string& account, const std::string& token, const std::string& platform, const std::string& login_platform)
{
	m_sessionid = sessionid;
	Account->set_string(account, false);
	PlayerId->set_value(0, false);

	m_token = token;
	Platform->set_string(platform);
	LoginPlatform->set_string(login_platform);
	reset_gatepeer();
	start_check();
}

void game_player::player_relogin(const std::string& token)
{
	m_token = token;
	reset_gatepeer();
	start_check(true);
}

void game_player::start_check(bool isrelogin)
{
	auto task = boost::make_shared<check_account_task>(world_server::instance().get_io_service());
	task->set_client(shared_from_this(), isrelogin);
	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");

	boost::format fmt = boost::format("/AccCheck.aspx?acc=%1%&platform=%2%&serverId=%3%") 
		% Account->get_string() % LoginPlatform->get_string() % world_server::instance().get_groupid();
	mr.spath = fmt.str();

	task->post_request(mr);
	m_state = e_ps_checking;
}


uint32_t game_player::get_sessionid()
{
	return m_sessionid;
}

void game_player::set_sessionid(uint32_t sessionid)
{
	m_sessionid = sessionid;
}

e_player_state game_player::get_state()
{
	return m_state;
}

bool game_player::check_token(const std::string& account, const std::string& token, const std::string& sign)
{
	std::string tsign = account + token + AES_KEY;
	return (enable_crypto_helper::CalMD5(reinterpret_cast<const uint8_t*>(tsign.c_str()), tsign.length()) == sign);
}

#include "xk_random.h"
bool game_player::http_run(const std::string& respose)
{
	__ENTER_FUNCTION_CHECK;
	//放到同步线程里 
	std::string resposeInfo = enable_crypto_helper::Base64Decode(respose);
	ENABLE_MAP<std::string, std::string> mData;
	enable_json_helper::str_to_json_map(resposeInfo, mData);
	auto it = mData.find("result");
	if (it != mData.end())
	{
		std::string& result = it->second;
		if (result == "true")
		{
			it = mData.find("data");
			if (it != mData.end())
			{
				std::vector<std::string> mData2;
				split(mData2, it->second, boost::is_any_of("_"), token_compress_on);

				if(mData2.size() == 2)
				{
					int tseed = boost::lexical_cast<int>(mData2[0]);
					std::string& tkey = mData2[1];
					xk_Random rd(tseed);
					std::string randkey = rd.rand_str(RAND_KEY, 16);
					std::string token = enable_crypto_helper::AESDecryptString(m_token, randkey);
					if(token == tkey)//校验成功
					{
						return true;
					}
					else
					{
						enable_random<boost::rand48> rd(tseed);
						std::string randkey = rd.rand_str(RAND_KEY, 16);
						std::string token = enable_crypto_helper::AESDecryptString(m_token, randkey);
						if (token == tkey)//校验成功
						{
							return true;
						}
					}
				}					
			}
		}
	}

	return false;
	__LEAVE_FUNCTION_CHECK;
	return EX_CHECK;
}

//在主线程中执行
void game_player::http_check(bool success, const std::string& respose, bool isrelogin)
{
	__ENTER_FUNCTION;

	mongo::BSONObj obj = db_player::instance().findone(DB_PLAYER_INFO, BSON("account"<< Account->get_string()));

	auto retinfo = msg_type_def::e_rmt_fail;

	bool block = false;
	if(obj.hasField("delete"))
	{
		block = obj.getBoolField("delete");
	}

	if(!block && obj.hasField("KickEndTime"))
	{
		time_t kt = obj.getField("KickEndTime").Date().toTimeT();
		block = (time_helper::instance().get_cur_time() < kt);
		retinfo = msg_type_def::e_rmt_player_prohibit;
	}

	if(!block && success)
	{

		if (!isrelogin)
		{
			m_login_time = time_helper::instance().get_cur_time();

			CheckMap->get_Tmap<check_map>()->attach(this);

			//验证成功后获取一个逻辑服务器
			if(!loadPlayer(obj))
			{
				create_player();
				sys_init();

				get_sys<BagMgr>()->doActivity();
			}
			else
			{
				sys_load();
			}

			m_giftStatPtr->attachPlayer(PlayerId->get_value());

			if(_checkReflush())
				resetPlayerInfo();

			if(OldAcc->get_string().empty())
				OldAcc->set_string(Account->get_string());
		}

		if(ChannelID->get_value() <=0)
			ChannelID->set_value(m_channel);

		if(is_gaming())
		{
			on_joingame();
		}

		GLOBAL_SYS(GmSys)->onPlayerLogin(this, true);
		stActivityEvent evt;
		evt.m_activityType = activity_type_login_at_day;
		GLOBAL_SYS(OperationActivitySys)->onPlayerEvent(this, &evt);

		game_player_mgr::instance().addPlayerById(shared_from_this());
		auto msg = PACKET_CREATE(packetw2c_player_connect_result, e_mst_w2c_player_connect_result);
		msg->set_result(msg_type_def::e_rmt_success);		
		static std::string CUR_VER = world_server::instance().get_cfg().get<std::string>("ver");
		msg->set_ver(CUR_VER);
		msg->set_gaming(get_gameid());
		msg->set_servertime(time_helper::instance().get_cur_time());
		send_msg_to_client( msg);
		m_state = e_ps_playing;		
		islogin_success = true;

		//登陆成功获取IP后才能记录登陆		
		auto peer2 =get_gate();
		if(peer2)
		{
			auto msg2 = PACKET_CREATE(packet_get_ip, e_mst_get_ip);
			msg2->set_sessionid(get_sessionid());
			peer2->send_msg(msg2);
		}
		return;	
	}

	m_state = e_ps_disconnect;
	auto msg = PACKET_CREATE(packetw2c_player_connect_result, e_mst_w2c_player_connect_result);
	msg->set_result(retinfo);	
	servers_manager::instance().send_msg_to_client(m_sessionid, msg);	
	game_player_mgr::instance().set_del_player(shared_from_this());

	__LEAVE_FUNCTION;
}

void game_player::load_robot(int playerid)
{
	mongo::BSONObj obj = db_player::instance().findone(DB_PLAYER_INFO, BSON("player_id"<< playerid));
	if(!loadPlayer(obj))
	{
		create_player(true);		
	}

	sys_load();

	m_giftStatPtr->attachPlayer(PlayerId->get_value());

	if(_checkReflush())
		resetPlayerInfo();

	game_player_mgr::instance().addPlayerById(shared_from_this());	

	m_state = e_ps_playing;	
	islogin_success = true;
}

int game_player::change_gold(GOLD_TYPE dif_gold, bool needsend, bool check, int reason)
{
	if(dif_gold == 0)
		return msg_type_def::e_rmt_success;

	GOLD_TYPE t = Gold->get_value();
	GOLD_TYPE old_value = t;
	if(dif_gold > 0 && t > MAX_MONEY - dif_gold)
	{
		if(check)
		{
			return msg_type_def::e_rmt_beyond_limit;
		}
		t = MAX_MONEY;
	}
	else
	{
		t += dif_gold;
	}
	if(t <= 0)
	{
		t = 0;
	}
	Gold->set_value(t);
	player_gold_log(old_value, t, reason);

	if(Gold->get_value() > MaxGold->get_value())
	{
		MaxGold->set_value(Gold->get_value());
	}

	if(needsend)
	{
		auto peer = get_logic();
		if(peer == nullptr)
			return msg_type_def::e_rmt_success;

		auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
		sendmsg->set_playerid(PlayerId->get_value());	
		auto cinfo = sendmsg->mutable_change_info();
		cinfo->set_gold(dif_gold);
		peer->send_msg(sendmsg);
	}	

	return msg_type_def::e_rmt_success;
}

void game_player::player_gold_log(GOLD_TYPE old_value, GOLD_TYPE new_value, int32_t reason)
{
	if (IsRobot->get_value())
	{
		return;
	}

	db_log::instance().player_gold_log(PlayerId->get_value(), old_value, new_value, reason);
}

void game_player::change_ticket(int dif_ticket, bool needsend)
{
	if(dif_ticket == 0)
		return;

	int t = Ticket->get_value();
	if(dif_ticket > 0 && t > MAX_TICKET - dif_ticket)
	{
		t = MAX_TICKET;
	}
	else
	{
		t += dif_ticket;
	}
	if(t <= 0)
	{
		t = 0;
	}
	Ticket->set_value(t);

	if(Ticket->get_value() > MaxTicket->get_value())
	{
		MaxTicket->set_value(Ticket->get_value());
	}

	if(needsend)
	{
		auto peer = get_logic();
		if(peer == nullptr)
			return;

		auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
		sendmsg->set_playerid(PlayerId->get_value());	
		auto cinfo = sendmsg->mutable_change_info();
		cinfo->set_ticket(dif_ticket);
		peer->send_msg(sendmsg);
	}	
}

void game_player::change_chip(int dif_chip)
{
	if(dif_chip == 0)
		return;
	
	int t = Chip->get_value();
	if(dif_chip > 0 && t > MAX_TICKET - dif_chip)
	{
		t = MAX_TICKET;
	}
	else
	{
		t += dif_chip;
	}
	if(t <= 0)
	{
		t = 0;
	}
	Chip->set_value(t);
}

void game_player::change_lucky(int dif_lucky)
{
	Lucky->add_value(dif_lucky);

	auto peer = get_logic();
	if(peer == nullptr)
		return;

	auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
	sendmsg->set_playerid(PlayerId->get_value());	
	auto cinfo = sendmsg->mutable_change_info_ex();
	cinfo->set_lucky(dif_lucky);
	peer->send_msg(sendmsg);
}

//玩家需要同步到游戏的属性
void game_player::change_property(int type)
{
	if(!is_gaming())
		return;

	auto peer = get_logic();
	if(peer == nullptr)
		return;

	auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
	sendmsg->set_playerid(PlayerId->get_value());	
	auto cinfo = sendmsg->mutable_change_info();

	switch (type)
	{
	case msg_type_def::e_itd_photoframe:
		cinfo->set_curphotoframeid(PhotoFrameId->get_value());
		break;
	case msg_type_def::e_itd_sex:
		cinfo->set_sex(Sex->get_value());
		break;	
	case msg_type_def::e_itd_nickname:
		cinfo->set_nickname(NickName->get_string());
		break;	
	case msg_type_def::e_itd_iconcustom:
		cinfo->set_icon_custom(IconCustom->get_string());
		break;
	default:
		return;
	}
	peer->send_msg(sendmsg);
}

void game_player::change_vip(int vip)
{
	auto peer = get_logic();
	if(peer == nullptr)
		return;

	auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
	sendmsg->set_playerid(PlayerId->get_value());	
	auto cinfo = sendmsg->mutable_change_info();
	cinfo->set_viplvl(vip);
	peer->send_msg(sendmsg);
}

const mongo::BSONObj& game_player::get_id_finder()
{
	return m_id_finder;
}

bool game_player::load_player()
{
	mongo::BSONObj b = db_player::instance().findone(DB_PLAYER_INFO, BSON("account"<< Account->get_string()));
	if(b.isEmpty())
		return false;

	bool ret = from_bson(b);
	if(ret)
	{
		m_id_finder = BSON("player_id" <<  PlayerId->get_value() );
		if (FirstGiftTime->get_value() == 0)
		{
			FirstGiftTime->set_value(time_helper::instance().get_cur_time());
		}
	}

	return ret;
}

bool game_player::loadPlayer(mongo::BSONObj& b)
{
	if(b.isEmpty())
		return false;

	bool ret = from_bson(b);
	if(ret)
	{
		m_id_finder = BSON("player_id" <<  PlayerId->get_value() );
		if (FirstGiftTime->get_value() == 0)
		{
			FirstGiftTime->set_value(time_helper::instance().get_cur_time());
		}

		player_gold_log(Gold->get_value(), Gold->get_value(), gold_log_type_load);
	}

	return ret;
}

void game_player::heartbeatRobotVip(double elapsed)
{
	if(!IsRobot->get_value())
	{
		return;
	}

	m_robotVipTime += elapsed;
	if( m_robotVipTime >= 60 )
	{
		m_robotVipTime = 0;

		GOLD_TYPE nowGold = get_gold();

		int vipLevel = 0;
		if (nowGold <= 5000000)
		{
			//vip1
			vipLevel = 1;
		}
		else if(nowGold > 5000000 && nowGold <= 10000000)
		{
			//vip2
			vipLevel = 2;
		}
		else if(nowGold > 10000000 && nowGold <= 20000000)
		{
			//vip3
			vipLevel = 3;
		}
		else if(nowGold > 20000000 && nowGold <= 30000000)
		{
			//vip4
			vipLevel = 4;
		}
		else if(nowGold > 30000000 && nowGold <= 40000000)
		{
			//vip5
			vipLevel = 5;
		}
		else
		{
			//vip6 or //vip7
			vipLevel = global_random::instance().rand_int(6, 7);
		}

		if (get_viplvl() < vipLevel)
		{
			change_vip(vipLevel);
		}

	}


}

void game_player::init_game_object()
{
	Account = regedit_strfield("account");	
	NickName = regedit_strfield("nickname");
	Platform = regedit_strfield("platform");
	LoginPlatform = regedit_strfield("loginplatform");
	PlayerId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "player_id"));
	Gold = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(GOLD_OBJ_TYPE, "gold"));
	Ticket = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "ticket"));
	Chip = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "chip"));

	OnlineTime = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "online_time"));
	LogoutTime = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "logout_time"));
	IconCustom = regedit_strfield("icon_custom");
	UpLoadCustomHeadFreezeDeadTime = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "upLoadCustomHeadFreezeDeadTime"));

	Sex = CONVERT_POINT(Tfield<int8_t>, regedit_tfield(e_got_int8, "sex"));
	Sex->set_value(msg_type_def::sex_boy);
	UpdateIconCount = CONVERT_POINT(Tfield<int16_t>, regedit_tfield(e_got_int16, "UpdateIconCount"));

	SelfSignature = regedit_strfield("selfSignature");
	m_giftStat = regedit_mapfield("gifts", GiftMap::malloc());
	m_giftStatPtr = m_giftStat->get_Tmap<GiftMap>();

	MaxGold = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(GOLD_OBJ_TYPE, "maxGold"));
	MaxTicket = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "maxTicket"));

	LastCheckTime = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "lastcheck_time"));
	OnlineRewardCount = CONVERT_POINT(Tfield<int16_t>, regedit_tfield(e_got_int16, "OnlineRewardCount"));

	PhotoFrameId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "PhotoFrameId"));

	BindPhone = regedit_strfield("bindPhone");
	BindCount = CONVERT_POINT(Tfield<int8_t>, regedit_tfield(e_got_int8, "bindCount"));

	NewGuildHasFinishStep = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "newGuildHasFinishStep"));

	SendGiftCoinCount = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(GOLD_OBJ_TYPE, "sendGiftCoinCount"));

	FetchSafeBoxSecurityCodeCount = CONVERT_POINT(Tfield<int8_t>, regedit_tfield(e_got_int8, "fetchSafeBoxSecurityCodeCount"));

	UpdateNickCount = CONVERT_POINT(Tfield<int8_t>, regedit_tfield(e_got_int8, "updateNickCount"));

	ExperienceVIP = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "ExperienceVIP"));

	CreateTime= CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "create_time"));
	IsRobot = CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "is_robot"));
	FirstGiftTime = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "FirstGiftTime"));
	WinCount = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "WinCount"));

	ChannelID = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "ChannelID"));
	OldAcc = regedit_strfield("OldAcc");	

	CheckMap = regedit_mapfield("checks", check_map::malloc());

	KickEndTime= CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "KickEndTime"));

	PlayerType = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "PlayerType"));

	Lucky = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "Lucky"));
	TempIncome = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "TempIncome"));
	TotalIncome = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "TotalIncome"));

	LastIP = regedit_strfield("LastIP");
	LastPort = CONVERT_POINT(Tfield<int16_t>, regedit_tfield(e_got_int16, "LastPort"));
	Privilege = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "Privilege"));
}

void game_player::to_bson_ex(mongo::BSONObjBuilder& ba)
{
	ba.appendBool("delete", false);
}

bool game_player::store_game_object(bool to_all)
{
	if(!has_update())
		return true;

	if(!islogin_success)
		return true;

	auto err = db_player::instance().update(DB_PLAYER_INFO, get_id_finder(), BSON("$set"<<to_bson(to_all)));
	if(!err.empty())
	{
		SLOG_ERROR << "game_player::store_game_object :" <<err;
		return false;
	}
	return true;
}

void game_player::create_player(bool isRobot)
{
	PlayerId->set_value(game_player_mgr::instance().generic_playerid());	
	CreateTime->set_value(time_helper::instance().get_cur_time());
	Account->set_update();
	OldAcc->set_string(Account->get_string());
	NickName->set_string(_newAccountName());
	LastCheckTime->set_value(time_helper::instance().get_cur_time());
	FirstGiftTime->set_value(CreateTime->get_value());

	int index = -1;
	if (Sex->get_value() == 1)
	{
		index = global_random::instance().rand_int(1, 18);
	}
	else
	{
		index = global_random::instance().rand_int(1, 17);
	}
	PhotoFrameId->set_value(index);

	if(isRobot)
	{
		IsRobot->set_value(true);
		Platform->set_string("default");
		LoginPlatform->set_string("default");

		//随机账号
		std::string acc = "robot_";		
		acc += boost::lexical_cast<std::string>(PlayerId->get_value());
		Account->set_string(acc);

		int nsex = global_random::instance().rand_int(1,2);
		if(nsex==2)//性别女
		{
			Sex->set_value(2);
		}
		else if(nsex ==1)
		{
			Sex->set_value(1);
		}

		int index = -1;
		if (Sex->get_value() == 1)
		{
			index=global_random::instance().rand_int(1, 18);
		}
		else
		{
			index = global_random::instance().rand_int(1, 17);
		}
		PhotoFrameId->set_value(index);
		UpdateIconCount->add_value(1);

		//昵称
		int maxcount= M_RobotNameCFG::GetSingleton()->GetCount()-1;
		index = global_random::instance().rand_int(0, maxcount);
		std::string tempname = M_RobotNameCFG::GetSingleton()->GetData(index)->mNickName;
		NickName->set_string(tempname);
		UpdateNickCount->add_value(1);		
	}

	static int defaultvip = M_BaseInfoCFG::GetSingleton()->GetData("defaultvip")->mValue;
	get_sys<game_sys_recharge>()->VipLevel->set_value(defaultvip);

	auto err = db_player::instance().update(DB_PLAYER_INFO, BSON("account"<<Account->get_string()<<"platform"<<LoginPlatform->get_string()), BSON("$set"<<to_bson(true, true)));
	if(!err.empty())
	{
		SLOG_ERROR << "game_player::create_player :" <<err;
	}

	m_id_finder = BSON("player_id" << PlayerId->get_value() );	

	std::string title, sender, content;
	getNewAccountMail(title, sender, content);
	GLOBAL_SYS(MailSys)->sendMail(title, sender, content, 0, PlayerId->get_value(), 7);

	store_game_object(true);
}

bool game_player::is_gaming()
{
	return m_gameid>0;
}
uint16_t game_player::get_gameid()
{
	return m_gameid;
}

static mongo::BSONObj SeachField = BSON("player_id" << 1);

int game_player::updateNickname(const std::string& newname)
{
	if(newname == NickName->get_string())
		return msg_type_def::e_rmt_success;

	static int modifyCost = M_BaseInfoCFG::GetSingleton()->GetData("modifyNicknameCost")->mValue;
	if(UpdateNickCount->get_value() > 0)
	{
		if(Ticket->get_value() < modifyCost)
			return msg_type_def::e_rmt_ticket_not_enough;
	}

	static const int maxLength = M_BaseInfoCFG::GetSingleton()->GetData("nickNameMaxLength")->mValue * 3;
	if(newname.empty() || newname.length() > maxLength)
		return msg_type_def::e_rmt_error_nickname;

	int retCode = NameValid::isValid(newname, msg_type_def::e_rmt_error_nickname);
	if(retCode != msg_type_def::e_rmt_success)
		return retCode;

	mongo::BSONObj b = db_player::instance().findone(DB_PLAYER_INFO, BSON("nickname" << newname), &SeachField);
	if(!b.isEmpty())
		return msg_type_def::e_rmt_same_nickname;

	if(UpdateNickCount->get_value() > 0)
	{
		addItem(msg_type_def::e_itd_ticket, -modifyCost, type_reason_modify_nickname);
	}
	else
	{
		UpdateNickCount->add_value(1);

		if(!IsRobot->get_value())
		{
			GLOBAL_SYS(PumpSys)->moneyTotalLog(this, 0, msg_type_def::e_itd_ticket, type_reason_modify_nickname, "free");
		}
	}

	NickName->set_string(newname);
	return msg_type_def::e_rmt_success;
}

int game_player::addGift(int giftId, GOLD_TYPE count, const std::string& param)
{
	const M_GiftCFGData* data = M_GiftCFG::GetSingleton()->GetData(giftId);
	if(data == nullptr)
		return msg_type_def::e_rmt_unknow;

	int64_t total_gold = data->mCoin * count;
	int64_t CounterFee = 0;
	
	if (is_goldshop() == false)
	{
		//手续费
		CounterFee = total_gold * data->mPercent;
		total_gold -= CounterFee;
	}
	

	std::vector<std::string> strs;
	split(strs, param, boost::is_any_of(":"), token_compress_on);

	boost::format fmt = boost::format("giftId:%1%,fromPlayerId:%2%,count:%3%") % giftId % strs[0] % count;

	int retCode = change_gold(total_gold, true, true, type_reason_accept_gift);
	if(retCode != msg_type_def::e_rmt_success)
		return retCode;

	GLOBAL_SYS(PumpSys)->moneyTotalLog(this, 0, msg_type_def::e_itd_gold, total_gold, type_reason_accept_gift, fmt.str());

	if (giftId != 30107)
	{
		auto p = m_giftStatPtr->find_Tobj<GiftInfo>(giftId);
		if(p)
		{
			p->m_count->add_value(count);
			m_giftStatPtr->db_update(p);
		}
		else
		{
			p = GiftInfo::malloc();
			p->m_giftId->set_value(giftId);
			p->m_count->set_value(count);
			m_giftStatPtr->put_obj(p);
			m_giftStatPtr->db_add(p);
		}
	}
	else
	{
		GLOBAL_SYS(PumpSys)->pumpSendGold2(strs[1], this, total_gold, CounterFee);
	}

	return msg_type_def::e_rmt_success;
}

int game_player::addItem(int itemId, GOLD_TYPE count, int reason, const std::string& param)
{
	if(count == 0)
		return msg_type_def::e_rmt_success;

	auto pItem = M_ItemCFG::GetSingleton()->GetData(itemId);
	if(pItem == nullptr)
		return msg_type_def::e_rmt_unknow;

	switch (pItem->mItemCategory)
	{
	case msg_type_def::e_itd_gold:   // 增加金币
		{
			change_gold(count, true, false, reason);
			GLOBAL_SYS(PumpSys)->moneyTotalLog(this, 0, msg_type_def::e_itd_gold, count, reason, param);
		}
		break;
	case msg_type_def::e_itd_ticket: // 增加礼券
		{
			change_ticket(count);
			GLOBAL_SYS(PumpSys)->moneyTotalLog(this, 0, msg_type_def::e_itd_ticket, count, reason, param);
		}
		break;
	case msg_type_def::e_itd_gift:  // 增加礼物
		{
			return addGift(itemId, count, param);
		}
		break;
	case msg_type_def::e_itd_chip:
		{
			change_chip(count);
			GLOBAL_SYS(PumpSys)->moneyTotalLog(this, 0, msg_type_def::e_itd_chip, count, reason, param);
		}
		break;
	case msg_type_def::e_itd_vip_experience_card: // vip体验卡
		{
			addExperienceVIP(count);
		}
		break;
	default: // 默认往背包里面加
		{
			get_sys<BagMgr>()->addItem(itemId, count);
			get_sys<game_quest_mgr>()->change_quest(e_qt_photo_frame);
		}
		break;
	}

	return msg_type_def::e_rmt_success;
}

void game_player::resetPlayerInfo()
{
	OnlineRewardCount->set_value(0);
	BindCount->set_value(0);
	SendGiftCoinCount->set_value(0);
	FetchSafeBoxSecurityCodeCount->set_value(0);
	sys_time_update();

	LastCheckTime->set_value(time_helper::instance().get_cur_time());
}

void game_player::setFirstGift()
{
	auto peer = get_logic();
	if(peer != nullptr)
	{
		auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
		sendmsg->set_playerid(PlayerId->get_value());	
		auto cinfo = sendmsg->mutable_change_info();
		cinfo->set_isbuyfirstgift(true);
		peer->send_msg(sendmsg);
	}		
}

bool game_player::join_game(uint16_t gameid, uint16_t serverid)
{
	bool res = setGameIdServerId(gameid, serverid);
	if(!res)
		return false;

	if(check_logic())
	{
		auto peer = get_logic();
		auto msg = PACKET_CREATE(packetw2l_player_login, e_mst_w2l_player_login);
		auto ainfo = msg->mutable_account_info();
		msg->set_sessionid(m_sessionid);;
		ainfo->set_aid(PlayerId->get_value());
		ainfo->set_gold(Gold->get_value());
		ainfo->set_nickname(NickName->get_string());		
		ainfo->set_viplvl(get_viplvl());
		ainfo->set_sex(Sex->get_value());
		ainfo->set_curphotoframeid(PhotoFrameId->get_value());
		ainfo->set_icon_custom(IconCustom->get_string());
		ainfo->set_ticket(Ticket->get_value());
		ainfo->set_experience_vip(ExperienceVIP->get_value());
		ainfo->set_create_time(CreateTime->get_value());				
		ainfo->set_monthcard_time(get_sys<game_sys_recharge>()->VipCardEndTime->get_value());
		ainfo->set_isbuyfirstgift(get_sys<game_sys_recharge>()->isBuyItem(17));
		ainfo->set_privilege(Privilege->get_value());

		auto ainfoex = msg->mutable_account_info_ex();
		ainfoex->set_is_robot(IsRobot->get_value());
		ainfoex->set_lucky(Lucky->get_value());
		ainfoex->set_temp_income(TempIncome->get_value());
		ainfoex->set_total_income(TotalIncome->get_value());

		peer->send_msg( msg);
		return true;
	}
	return false;
}

bool game_player::setGameIdServerId(uint16_t gameId, uint16_t serverId)
{
	if(m_logicid != 0)
		return false;//已经在游戏中	

	m_logicid = serverId;
	m_gameid = gameId;

	game_player_mgr::instance().onEnterGame(get_gameid());

	reset_logicpeer();
	return true;
}

bool game_player::resetGameIdServerId()
{
	if(m_logicid == 0)
		return false;//不在游戏

	game_player_mgr::instance().onExitGame(get_gameid());

	m_logicid = 0;
	m_gameid = 0;
	reset_logicpeer();	
	return true;
}

void game_player::on_joingame(bool blogin)
{
	if(blogin)
	{
		auto peer = get_logic();
		if(peer)
		{
			auto msg = PACKET_CREATE(packetw2l_player_login, e_mst_w2l_player_login);
			auto ainfo = msg->mutable_account_info();
			ainfo->set_aid(PlayerId->get_value());
			msg->set_sessionid(m_sessionid);
			peer->send_msg( msg);
		}
	}


	auto peer2 = get_gate();
	if(peer2)
	{
		auto msg =  PACKET_CREATE(packet_player_connect, e_mst_player_connect);
		msg->set_logicid(m_logicid);
		msg->set_sessionid(m_sessionid);
		peer2->send_msg( msg);
	}	

	GLOBAL_SYS(ChatSys)->playerEnterGame(this);
}


void game_player::leave_game()
{
	if(!resetGameIdServerId())
		return;
	
	auto peer2 = get_gate();
	if(peer2)
	{
		auto msg =  PACKET_CREATE(packet_player_connect, e_mst_player_connect);
		msg->set_logicid(m_logicid);
		msg->set_sessionid(m_sessionid);
		peer2->send_msg(msg);
	}
}

uint16_t game_player::get_logicid()
{
	return m_logicid;
}

bool game_player::check_logic()
{
	return !m_logicpeer.expired();
}
void game_player::clear_logic( uint16_t lid)
{
	if(lid == 0 || lid == m_logicid)
	{
		m_logicid = 0;
		m_logicpeer.reset();
	}	
}

void game_player::reset_logicpeer()
{
	m_logicpeer.reset();

	if(m_logicid <=0)		
		return;	

	auto peer = servers_manager::instance().find_server(m_logicid);
	if(peer)
		m_logicpeer = boost::weak_ptr<world_peer>(peer);
}

int game_player::send_msg_to_client(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg)
{
	if(m_gatepeer.expired())
		return -1;

	auto peer = m_gatepeer.lock();
	if(peer)
	{
		auto sendmsg = PACKET_CREATE(packet_transmit_msg, e_mst_transmit_msg);
		sendmsg->set_sessionid(m_sessionid);
		auto packet = sendmsg->mutable_msgpak();
		packet->set_msgid(packet_id);
		msg->SerializeToString(packet->mutable_msginfo());

		return peer->send_msg( sendmsg);
	}
	return -1;	
}

void game_player::reset_gatepeer()
{
	m_gatepeer.reset();

	if(m_sessionid <=0)		
		return;	

	auto peer = servers_manager::instance().find_server(session_helper::get_gateid(m_sessionid));
	if (peer)
	{
		m_gatepeer = boost::weak_ptr<world_peer>(peer);
	}
}
boost::shared_ptr<world_peer> game_player::get_logic()
{
	return m_logicpeer.lock();
}
boost::shared_ptr<world_peer> game_player::get_gate()
{
	return m_gatepeer.lock();
}

bool game_player::_checkReflush()
{
	auto pt = time_helper::convert_to_date(LastCheckTime->get_value());
	if(pt >= time_helper::instance().get_cur_date())
		return false;

	return true;
}

std::string game_player::_newAccountName()
{
	auto idSys = GLOBAL_SYS(IdGeneratorSys);
	int curId = idSys->getCurId(IdTypeNewAccountId);
	static const M_MultiLanguageCFGData* data = M_MultiLanguageCFG::GetSingleton()->GetData("NewAccountName");
	if(!data)
		return boost::lexical_cast<std::string>(PlayerId->get_value());

	char buf[32] = {0};
	sprintf_s(buf, data->mName.c_str(), curId);

	int i = 0;
	mongo::BSONObj b = db_player::instance().findone(DB_PLAYER_INFO, BSON("nickname" << buf), &SeachField);
	while(!b.isEmpty() && i < 10000)
	{
		curId = idSys->getCurId(IdTypeNewAccountId);
		sprintf_s(buf, data->mName.c_str(), curId);
		b = db_player::instance().findone(DB_PLAYER_INFO, BSON("nickname" << buf), &SeachField);
		i++;
	}

	if(b.isEmpty())
		return buf;

	return boost::lexical_cast<std::string>(PlayerId->get_value());
}


bool game_player::addExperienceVIP(int day)
{
	auto tvip = time_helper::convert_to_date(ExperienceVIP->get_value());
	auto tnow = time_helper::instance().get_cur_date();
	if(tvip < tnow) // 
	{
		auto tend = tnow + gregorian::days(day);
		ExperienceVIP->set_value(time_helper::convert_from_date(tend));
	}
	else
	{
		auto tend = day * 86400;
		ExperienceVIP->add_value(tend);
	}

	auto peer = get_logic();
	if(peer != nullptr)
	{
		auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
		sendmsg->set_playerid(PlayerId->get_value());	
		auto cinfo = sendmsg->mutable_change_info();
		cinfo->set_experience_vip(ExperienceVIP->get_value());
		peer->send_msg(sendmsg);
	}		

	return true;
}

int game_player::get_viplvl()
{
	return get_sys<game_sys_recharge>()->VipLevel->get_value();
}

#include "game_check_def.h"
bool game_player::first_login()
{
	int islogin = CheckMap->get_Tmap<check_map>()->get_check(check_first_login);
	if(islogin<=0)
	{	
		CheckMap->get_Tmap<check_map>()->add_check(check_first_login);
		static int base_gold = M_BaseInfoCFG::GetSingleton()->GetData("base_gold")->mValue;
		static int base_ticket = M_BaseInfoCFG::GetSingleton()->GetData("base_ticket")->mValue;
		static int base_chip = M_BaseInfoCFG::GetSingleton()->GetData("base_chip")->mValue;

		addItem(msg_type_def::e_itd_gold, base_gold, type_reason_new_player);		
		addItem(msg_type_def::e_itd_ticket, base_ticket, type_reason_new_player);		
		addItem(msg_type_def::e_itd_chip, base_chip, type_reason_new_player);
		return true;
	}

	return false;
}

void game_player::check_firstgift(int32_t gold)
{
	if (IsRobot->get_value())
	{
		return;
	}
	if (time_helper::instance().get_cur_time() - FirstGiftTime->get_value() >= 86400)
	{
		return;
	}
	if (get_sys<game_sys_recharge>()->isBuyItem(17))
	{
		return;
	}
	if (get_gameid() <= 1)
	{
		return;
	}
	if (gold > 0)
	{
		WinCount->add_value(1);
		if (WinCount->get_value() == 1)
		{
			auto sendmsg = PACKET_CREATE(packetw2c_open_first_gift, e_mst_w2c_open_first_gift);
			send_msg_to_client(sendmsg);	
		}
	}
}

bool game_player::is_goldshop()
{
	return PlayerType->get_value() == 3;
}

GOLD_TYPE game_player::get_gold()
{
	return Gold->get_value();
}
