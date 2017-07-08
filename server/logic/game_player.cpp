
#include "stdafx.h"
#include "game_player.h"
#include <net/packet_manager.h>
#include <enable_crypto.h>
#include <enable_json_map.h>
#include <enable_random.h>
#include "game_player_mgr.h"
#include <boost/regex.hpp>
#include "time_helper.h"
#include <task_manager.h>
#include "game_check.h"
#include "logic_peer.h"
#include <i_game_phandler.h>
#include "backstage_manager.h"
#include "proc_world2logic_protocol.h"
#include "game_manager.h"
#include "i_game_engine.h"
#include "M_MultiLanguageCFG.h"
#include "M_GameCFG.h"
#include "M_BaseInfoCFG.h"
#include <boost/format.hpp>

using namespace boost;

enable_obj_pool_init(game_player, boost::null_mutex);

game_player::game_player()
	:m_sessionid(0)
	,m_state(e_ps_none)
	,PlayerID(0)
	,Gold(0)
	,VIPLevel(0)
	,PhotoFrame(0)
	,m_check_kick(0)
	,Ticket(0)
	,Sex(0)
	,ExperienceVIP(0)
	,CreateTime(0)
	,IsRobot(false)
	,m_life(0)
	,m_check_life(0)
	,MonthCard(0)
	,Lucky(0)
	,TempIncome(0)
	,TotalIncome(0)
	,Privilege(0)
{
	init_sys();
}

game_player::~game_player()
{

}

void game_player::heartbeat( double elapsed )
{
	if(m_state == e_ps_disconnect)
	{
		m_check_kick+= elapsed;
		if(m_check_kick > 300)//掉线300秒踢出游戏
		{
			game_player_mgr::instance().del_player(PlayerID);
			m_check_kick = 0;
		}
	}

	//许珂2017-7-7 号 修正 机器人的生命，以及限制金币,这种做法没有任何意义，可以完全交给逻辑服务器去处理
	/*if(IsRobot)
	{
		m_check_life += elapsed;
		if(m_life > 0 && m_check_life > m_life)
		{
			game_manager::instance().release_robot(PlayerID);	
			m_check_life = 0;
		}	

		//机器人金币限制
		static int maxgold = M_BaseInfoCFG::GetSingleton()->GetData("MaxGold")->mValue;
		if(Gold >= maxgold)
		{
			game_manager::instance().release_robot(PlayerID);
		}		
	}	*/
}

void game_player::leave_game()
{
	auto eng = game_manager::instance().get_game_engine();
	if(eng != nullptr)
	{
		eng->player_leave_game(PlayerID);
	}	

	auto peer = backstage_manager::instance().get_world();
	if(peer)
	{
		auto sendmsg = PACKET_CREATE(packetl2w_player_logout_result, e_mst_l2w_player_logout_result);	
		sendmsg->set_playerid(PlayerID);
		peer->send_msg(sendmsg);
	}	
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
void game_player::set_state(e_player_state eps)
{
	m_state = eps;

	if(get_handler())
	{
		m_check_kick = 0;
		get_handler()->on_change_state();
	}
}

void game_player::world_attribute_change(int atype, int v)
{
	switch (atype)
	{
	case msg_type_def::e_itd_gold:
		Gold += v;
		break;
	case msg_type_def::e_itd_vip:
		VIPLevel = v;
		break;
	case msg_type_def::e_itd_photoframe:
		PhotoFrame = v;
		break;
	case msg_type_def::e_itd_ticket :
		Ticket += v;
		break;
	case  msg_type_def::e_itd_sex :
		Sex = v;
		break;
	case msg_type_def::e_itd_monthcard:
		MonthCard = v;
		break;
	case msg_type_def::e_itd_firstgift:
		IsBuyFirstGift = v;
		break;
	case msg_type_def::e_itd_lucky:		
		Lucky = v;
		break;
	case msg_type_def::e_itd_privilege:		
		Privilege = v;
		break;
	}

	get_handler()->on_attribute_change(atype, v);
}

void game_player::world_attribute64_change(int atype, int64_t v)
{
	switch (atype)
	{
		case msg_type_def::e_itd_gold:
			Gold += v;
			break;
	}

	get_handler()->on_attribute64_change(atype, v);
}


//获取玩家id
uint32_t game_player::get_playerid()
{
	return PlayerID;
}

//获取玩家当前金币
int game_player::get_attribute(int atype)
{
	switch (atype)
	{
	case msg_type_def::e_itd_vip:
		return VIPLevel;
	case msg_type_def::e_itd_photoframe:
		return PhotoFrame;
	case msg_type_def::e_itd_ticket:
		return Ticket;
	case msg_type_def::e_itd_sex:
		return Sex;
	case msg_type_def::e_itd_monthcard:
		return MonthCard;	
	case msg_type_def::e_itd_lucky:
		return Lucky;		
	case msg_type_def::e_itd_privilege:
		return Privilege;		
	default:
		return 0;
	}	
}

GOLD_TYPE game_player::get_attribute64(int atype)
{
	switch (atype)
	{
	case msg_type_def::e_itd_gold:
		return Gold;
	case msg_type_def::e_itd_tempincome:
		return TempIncome;
	case msg_type_def::e_itd_totalincome:
		return TotalIncome;	
	default:
		return 0;
	}	
}

bool game_player::check_monthcard()
{
	auto tvip = time_helper::convert_to_date(MonthCard);
	auto tnow = time_helper::instance().get_cur_date();
	if(tvip > tnow)
		return true;

	return false;
}

const std::string& game_player::get_nickname()
{
	return NickName;
}

const std::string& game_player::get_icon_custom()
{
	return IconCustom;
}

bool game_player::is_ExperienceVIP()
{
	auto tvip = time_helper::convert_to_date(ExperienceVIP);
	auto tnow = time_helper::instance().get_cur_date();
	if(tvip > tnow)
		return true;

	return false;
}

bool game_player::is_BuyFirstGift()
{
	return IsBuyFirstGift;
}

//修改玩家金币(不要频繁调用)
bool game_player::change_gold(GOLD_TYPE cgold)
{
	if(-cgold > Gold)
		return false;

	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return false;

	if(Gold > MAX_MONEY - cgold)
		cgold = MAX_MONEY - Gold;

	Gold += cgold;

	auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
	sendmsg->set_playerid(get_playerid());	
	auto cinfo = sendmsg->mutable_change_info();
	cinfo->set_gold(cgold);
	peer->send_msg(sendmsg);

	return true;
}

bool game_player::change_ticket(int cticket)
{
	if(-cticket > Ticket)
		return false;

	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return false;

	if(Ticket > MAX_MONEY - cticket)
		cticket = MAX_MONEY - Gold;

	Ticket += cticket;

	auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
	sendmsg->set_playerid(get_playerid());	
	auto cinfo = sendmsg->mutable_change_info();
	cinfo->set_ticket(cticket);
	peer->send_msg(sendmsg);

	return true;
}

//修改幸运
bool game_player::change_lucky(int clucky, int64_t tempincome, int64_t totalincome)
{
	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return false;

	Lucky = clucky;
	TempIncome = tempincome;
	TotalIncome = totalincome;

	auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
	sendmsg->set_playerid(get_playerid());	
	auto cinfo = sendmsg->mutable_change_info_ex();
	cinfo->set_lucky(Lucky);
	cinfo->set_temp_income(TempIncome);
	cinfo->set_total_income(TotalIncome);
	peer->send_msg(sendmsg);

	return true;
}


//改变星星属性
bool game_player::add_starinfo(int addaward, int addstar)
{
	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return false;

	auto sendmsg = PACKET_CREATE(packetl2w_player_star_change, e_mst_l2w_player_star_change);
	sendmsg->set_player_id(get_playerid());	
	sendmsg->set_addaward(addaward);
	sendmsg->set_addstar(addstar);
	peer->send_msg(sendmsg);

	return true;
}

//发送协议到客户端
int game_player::send_msg_to_client(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg)
{
	if(GatePeer.expired())
		return -1;
	
	auto peer = GatePeer.lock();
	return peer->send_msg_to_client(m_sessionid, packet_id, msg);
}

void game_player::player_property_log(int ptype, int changecount, int reason, const std::string& param)
{
	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return;

	auto sendmsg = PACKET_CREATE(packetl2w_player_property_stat, e_mst_l2w_player_property_stat);
	sendmsg->set_playerid(get_playerid());	
	sendmsg->set_attrtype((msg_type_def::e_item_type_def)ptype);
	sendmsg->set_addvalue(changecount);
	sendmsg->set_gameid(game_manager::instance().get_gameid());
	sendmsg->set_reason((PropertyReasonType)reason);
	sendmsg->set_param(param);
	peer->send_msg(sendmsg);
}

void game_player::game_broadcast(const std::string& roomname, int infotype, const std::string& strinfo, int money, int moneytype)
{
	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return;

	static const std::string& gmsg = M_MultiLanguageCFG::GetSingleton()->GetData("GameBroadcast")->mName;
	static const std::string& gname = M_GameCFG::GetSingleton()->GetData(game_manager::instance().get_gameid())->mGameName;

	format fmt(gmsg.c_str());

	std::string mtstr = "MoneyType"+boost::lexical_cast<std::string>(moneytype);
	const M_MultiLanguageCFGData* mtp = M_MultiLanguageCFG::GetSingleton()->GetData(mtstr);
	if(mtp == nullptr)
		return;

	std::string htstr = "HitType"+boost::lexical_cast<std::string>(infotype);
	const M_MultiLanguageCFGData* htp = M_MultiLanguageCFG::GetSingleton()->GetData(htstr);
	if(htp == nullptr)
		return;
	
	fmt%NickName%gname%roomname%(money/10000)%mtp->mName%htp->mName%strinfo;

	auto sendmsg = PACKET_CREATE(packetl2w_game_broadcast, e_mst_l2w_game_broadcast);
	sendmsg->set_game_msg(fmt.str());
	peer->send_msg(sendmsg);
}

bool game_player::is_protect()
{
	if(CreateTime<=0)
		return false;

	auto nowt = time_helper::instance().get_cur_ptime();
	auto ct = time_helper::convert_to_ptime(CreateTime);	

	static int ptotect_time = M_BaseInfoCFG::GetSingleton()->GetData("ptotectTime")->mValue;

	if(nowt < ct + posix_time::seconds(ptotect_time))//当前时间小于保护结束时间
		return true;

	return false;
}

bool game_player::is_robot()
{
	return IsRobot;
}

const std::string* game_player::getLan(const std::string& lanKey)
{
	const M_MultiLanguageCFGData *pData = M_MultiLanguageCFG::GetSingleton()->GetData(lanKey);
	if(pData)
		return &pData->mName;
	
	return nullptr;
}

void game_player::gameBroadcast(const std::string& msg)
{
	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return;

	auto sendmsg = PACKET_CREATE(packetl2w_game_broadcast, e_mst_l2w_game_broadcast);
	sendmsg->set_game_msg(msg);
	peer->send_msg(sendmsg);
}

void game_player::reset_robot_life()
{
	if(!IsRobot)
		return;

	static int minLifeTime = M_BaseInfoCFG::GetSingleton()->GetData("MinLifeTime")->mValue;
	static int maxLifeTime = M_BaseInfoCFG::GetSingleton()->GetData("MaxLifeTime")->mValue;
	m_life = global_random::instance().rand_int(minLifeTime, maxLifeTime);

	game_manager::instance().response_robot(PlayerID);
}

void game_player::quest_change(int questid, int count, int param)
{
	auto peer = backstage_manager::instance().get_world();
	if(peer == nullptr)
		return;

	auto sendmsg = PACKET_CREATE(packetl2w_player_quest_change, e_mst_l2w_player_quest_change);
	sendmsg->set_player_id(PlayerID);
	sendmsg->set_quest_id(questid);
	sendmsg->set_q_count(count);
	sendmsg->set_ex_param(param);
	peer->send_msg(sendmsg);
}