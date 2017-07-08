#include "stdafx.h"
#include "proc_player_property.h"
#include "game_player.h"
#include "dial_lottery_mgr.h"
#include "msg_type_def.pb.h"
#include "M_BaseInfoCFG.h"
#include "game_db.h"
#include "bag_mgr.h"
#include "bag_def.h"
#include "pump_type.h"
#include "pump_sys.h"
#include "global_sys_mgr.h"
#include "pump_type.pb.h"
#include "player_log_mgr.h"
#include "M_BaseInfoCFG.h"
#include "time_helper.h"
#include "game_quest_mgr.h"
#include "game_quest.h"

using namespace boost;

void initPlayerPropertyPacket()
{
	packetc2w_update_playerhead_factory::regedit_factory();
	packetw2c_update_playerhead_result_factory::regedit_factory();

	packetc2w_update_nickname_factory::regedit_factory();
	packetw2c_update_nickname_result_factory::regedit_factory();

	packetc2w_update_sex_factory::regedit_factory();
	packetw2c_update_sex_result_factory::regedit_factory();

	packetc2w_update_signature_factory::regedit_factory();
	packetw2c_update_signature_result_factory::regedit_factory();

	packetc2w_change_photo_frame_factory::regedit_factory();
	packetw2c_change_photo_frame_result_factory::regedit_factory();

	packetc2w_req_game_stat_factory::regedit_factory();
	packetw2c_fishlord_stat_result_factory::regedit_factory();
	packetw2c_dice_stat_result_factory::regedit_factory();
	packetw2c_crocodile_stat_result_factory::regedit_factory();

	packetc2w_req_self_record_factory::regedit_factory();
	packetw2c_req_self_record_result_factory::regedit_factory();

	packetc2w_finish_one_new_guild_factory::regedit_factory();
	packetw2c_finish_one_new_guild_result_factory::regedit_factory();

	packetc2w_req_send_gift_log_factory::regedit_factory();
	packetw2c_req_send_gift_log_result_factory::regedit_factory();

	packetc2w_req_safebox_log_factory::regedit_factory();
	packetw2c_req_safebox_log_result_factory::regedit_factory();

	packetc2w_inform_playerhead_factory::regedit_factory();
	packetw2c_inform_playerhead_result_factory::regedit_factory();
}

// 修改头像
bool packetc2w_update_playerhead_factory::packet_process(shared_ptr<world_peer> peer, 
														boost::shared_ptr<game_player> player, 
														shared_ptr<packetc2w_update_playerhead> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_update_playerhead_result, e_mst_w2c_update_playerhead_result);
	time_t curT = time_helper::instance().get_cur_time();
	if(curT < player->UpLoadCustomHeadFreezeDeadTime->get_value())
	{
		sendmsg->set_result(msg_type_def::e_rmt_custom_head_freezing);
	}
	else if(player->IconCustom->get_string() != msg->headstr())
	{
		static int updateicon = M_BaseInfoCFG::GetSingleton()->GetData("UpdateIcon")->mValue;
		if(player->UpdateIconCount->get_value() > 1 && player->Ticket->get_value() < updateicon)
		{
			sendmsg->set_result(msg_type_def::e_rmt_ticket_not_enough);
		}
		else
		{
			player->IconCustom->set_string(msg->headstr());
			player->UpdateIconCount->add_value(1);
			if(player->UpdateIconCount->get_value() > 1)
				player->addItem(msg_type_def::e_itd_ticket, -updateicon, PropertyReasonType::type_reason_update_icon);
			//player->store_game_object();

			player->change_property(msg_type_def::e_itd_iconcustom);
			GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_upload_head_icon);

			sendmsg->set_headstr(player->IconCustom->get_string());
			sendmsg->set_result(msg_type_def::e_rmt_success);

			player->get_sys<game_quest_mgr>()->change_quest(e_qt_head_icon);
		}		
	}
	else
	{
		sendmsg->set_headstr(player->IconCustom->get_string());
		sendmsg->set_result(msg_type_def::e_rmt_success);
	}
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 修改昵称
bool packetc2w_update_nickname_factory::packet_process(shared_ptr<world_peer> peer, 
														 boost::shared_ptr<game_player> player, 
														 shared_ptr<packetc2w_update_nickname> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_update_nickname_result, e_mst_w2c_update_nickname_result);
	int result = player->updateNickname(msg->nickname());
	if(result == msg_type_def::e_rmt_success)
	{
		sendmsg->set_nickname(msg->nickname());
		player->change_property(msg_type_def::e_itd_nickname);
		GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_nickname_modify);

		player->get_sys<game_quest_mgr>()->change_quest(e_qt_update_name);
	}
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 修改性别
bool packetc2w_update_sex_factory::packet_process(shared_ptr<world_peer> peer, 
												  boost::shared_ptr<game_player> player, 
												  shared_ptr<packetc2w_update_sex> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_update_sex_result, e_mst_w2c_update_sex_result);
	int sexValue = msg->sex();
	if(sexValue < msg_type_def::sex_boy || sexValue >= msg_type_def::sex_max)
	{
		sexValue = msg_type_def::sex_boy;
	}

	if(sexValue != player->Sex->get_value())
	{
		player->Sex->set_value(sexValue);
		//player->store_game_object();
		player->change_property(msg_type_def::e_itd_sex);
		GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_sex_modify);
	}
	sendmsg->set_sex(player->Sex->get_value());
	sendmsg->set_result(msg_type_def::e_rmt_success);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 修改签名
bool packetc2w_update_signature_factory::packet_process(shared_ptr<world_peer> peer, 
														boost::shared_ptr<game_player> player, 
														shared_ptr<packetc2w_update_signature> msg)
{	
	__ENTER_FUNCTION_CHECK;

	static const M_BaseInfoCFGData* data = M_BaseInfoCFG::GetSingleton()->GetData("signatureMaxLength");
	int maxLength = 180;
	if(data)
	{
		maxLength = data->mValue * 3;
	}

	int result = msg_type_def::e_rmt_success;
	auto sendmsg = PACKET_CREATE(packetw2c_update_signature_result, e_mst_w2c_update_signature_result);
	if(msg->signature().length() < maxLength)
	{
		player->SelfSignature->set_string(msg->signature());
		sendmsg->set_signature(msg->signature());
		//player->store_game_object();
		GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_self_signature_modify);
	}
	else
	{
		result = msg_type_def::e_rmt_length_beyond_range;
	}
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 改变相框
bool packetc2w_change_photo_frame_factory::packet_process(shared_ptr<world_peer> peer, 
														  boost::shared_ptr<game_player> player, 
														  shared_ptr<packetc2w_change_photo_frame> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_change_photo_frame_result, e_mst_w2c_change_photo_frame_result);
	int result = msg_type_def::e_rmt_success;
	if(msg->photoframeid() == 0)
	{
		player->PhotoFrameId->set_value(0);
	}
	else
	{
		int count = player->get_sys<BagMgr>()->getItemCount(msg->photoframeid());
		if(count == 0)
		{
			result = msg_type_def::e_rmt_not_find_item;
		}
		else
		{
			player->PhotoFrameId->set_value(msg->photoframeid());
		}
	}

	if(result == msg_type_def::e_rmt_success)
	{
		//player->store_game_object();
		player->change_property(msg_type_def::e_itd_photoframe);

		sendmsg->set_photoframeid(msg->photoframeid());
	}

	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 获取战绩统计
bool packetc2w_req_game_stat_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_req_game_stat> msg)
{	
	__ENTER_FUNCTION_CHECK;

	switch(msg->gameid())
	{
	case 1:   // 捕鱼
		{
			auto sendmsg = PACKET_CREATE(packetw2c_fishlord_stat_result, e_mst_w2c_fishlord_stat_result);
			mongo::BSONObj cond = BSON("player_id" << player->PlayerId->get_value());
			mongo::BSONObj obj = db_game::instance().findone(DB_FISHLORD, cond);
			if(!obj.isEmpty()) 
			{
				if(obj.hasField("maxWinCoin"))
				{
					sendmsg->set_maxcoin(obj.getIntField("maxWinCoin"));
				}
				//sendmsg->set_maxticket(obj.getIntField("maxWinTicket"));
				if(obj.hasField("hitBlackDagonCount"))
				{
					sendmsg->set_hitblackdagoncount(obj.getIntField("hitBlackDagonCount"));
				}
				if(obj.hasField("hitBlueDagonCount"))
				{
					sendmsg->set_hitbluedagoncount(obj.getIntField("hitBlueDagonCount"));
				}
				if(obj.hasField("hitGoldDagonBombCount"))
				{
					sendmsg->set_hitgolddagonbombcount(obj.getIntField("hitGoldDagonBombCount"));
				}
			}
			peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		}
		break;
	case 2:   // 鳄鱼
		{
			auto sendmsg = PACKET_CREATE(packetw2c_crocodile_stat_result, e_mst_w2c_crocodile_stat_result);
			mongo::BSONObj cond = BSON("player_id" << player->PlayerId->get_value());
			mongo::BSONObj obj = db_game::instance().findone(DB_CROCODILE, cond);
			if(!obj.isEmpty()) 
			{
				if(obj.hasField("OnceWinMaxGold"))
				{
					sendmsg->set_maxcoin(obj.getIntField("OnceWinMaxGold"));
				}
				//sendmsg->set_maxticket(0);
				if(obj.hasField("HandselTime"))
				{
					sendmsg->set_hitwinningscount(obj.getIntField("HandselTime"));
				}
				if(obj.hasField("SpotlightTime"))
				{
					sendmsg->set_hitspotlightcount(obj.getIntField("SpotlightTime"));
				}
				if(obj.hasField("AllprizesTime"))
				{
					sendmsg->set_hiteveryonehasawardcount(obj.getIntField("AllprizesTime"));
				}
			}
			peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		}
		break;
	case 3:   // 骰宝
		{
			auto sendmsg = PACKET_CREATE(packetw2c_dice_stat_result, e_mst_w2c_dice_stat_result);
			mongo::BSONObj cond = BSON("player_id" << player->PlayerId->get_value());
			mongo::BSONObj obj = db_game::instance().findone(DB_DICE, cond);
			if(!obj.isEmpty()) 
			{
				if(obj.hasField("max_gold"))
				{
					sendmsg->set_maxcoin(obj.getIntField("max_gold"));
				}
				if(obj.hasField("max_coupon"))
				{
					sendmsg->set_maxticket(obj.getIntField("max_coupon"));
				}
				if(obj.hasField("max_leopard"))
				{
					sendmsg->set_hitleopardcount(obj.getIntField("max_leopard"));
				}
				if(obj.hasField("max_wins"))
				{
					sendmsg->set_maxsuccessioncount(obj.getIntField("max_wins"));
				}
			}
			peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		}
		break;
	default:
		{
			auto sendmsg = PACKET_CREATE(packetw2c_fishlord_stat_result, e_mst_w2c_fishlord_stat_result);
			peer->send_msg_to_client(player->get_sessionid(), sendmsg);
			SLOG_ERROR << boost::format("无法识别游戏[id = %1%]") % msg->gameid();
		}
		break;
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 个人记录
bool packetc2w_req_self_record_factory::packet_process(shared_ptr<world_peer> peer, 
													   boost::shared_ptr<game_player> player, 
													   shared_ptr<packetc2w_req_self_record> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_self_record_result, e_mst_w2c_req_self_record_result);
	sendmsg->set_maxcoin(player->MaxGold->get_value());
	sendmsg->set_maxticket(player->MaxTicket->get_value());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 完成某个新手引导
bool packetc2w_finish_one_new_guild_factory::packet_process(shared_ptr<world_peer> peer, 
															boost::shared_ptr<game_player> player, 
															shared_ptr<packetc2w_finish_one_new_guild> msg)
{	
	__ENTER_FUNCTION_CHECK;

	/*auto sendmsg = PACKET_CREATE(packetw2c_finish_one_new_guild_result, e_mst_w2c_finish_one_new_guild_result);
	player->NewGuildHasFinishStep->add_value(1);
	player->store_game_object();
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	*/
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求保险日志
bool packetc2w_req_safebox_log_factory::packet_process(shared_ptr<world_peer> peer, 
														 boost::shared_ptr<game_player> player, 
														 shared_ptr<packetc2w_req_safebox_log> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_safebox_log_result, e_mst_w2c_req_safebox_log_result);
	auto mgr = player->get_sys<PlayerLogMgr>();
	mgr->getSafeBoxLog(msg->lasttime(), sendmsg);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求赠送日志
bool packetc2w_req_send_gift_log_factory::packet_process(shared_ptr<world_peer> peer, 
														 boost::shared_ptr<game_player> player, 
														 shared_ptr<packetc2w_req_send_gift_log> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_send_gift_log_result, e_mst_w2c_req_send_gift_log_result);
	auto mgr = player->get_sys<PlayerLogMgr>();
	std::deque<stSendGiftLogInfo> logList;
	time_t Last = 0;
	mgr->getSendGiftLog(msg->lasttime(), Last, logList);
	if(!logList.empty())
	{
		auto pLogList = sendmsg->mutable_loglist();
		pLogList->Reserve(logList.size());
		for(auto it = logList.begin(); it != logList.end(); ++it)
		{
			auto pLog = pLogList->Add();
			pLog->set_sendtime(it->m_sendTime);
			pLog->set_firendid(it->m_friendId);
			pLog->set_friendnickname(it->m_friendNickName);
			pLog->set_giftid(it->m_giftId);
			pLog->set_count(it->m_count);
			pLog->set_sendgold(it->m_sendgold);
			pLog->set_mailid(it->m_mailid);
		}
	}
	sendmsg->set_lasttime(Last);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 举报头像
bool packetc2w_inform_playerhead_factory::packet_process(shared_ptr<world_peer> peer, 
													   boost::shared_ptr<game_player> player, 
													   shared_ptr<packetc2w_inform_playerhead> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_inform_playerhead_result, e_mst_w2c_inform_playerhead_result);
	
	mongo::BSONObjBuilder builder;
	// 举报人
	builder.append("informerId", player->PlayerId->get_value());
	// 被举报人ID
	builder.append("destPlayerId", msg->informdstplayerid());
	// 举报时间
	builder.appendTimeT("time", time_helper::instance().get_cur_time());
	const std::string& res = db_player::instance().insert(DB_INFORM_HEAD, builder.done());
	int result = res.empty() ? msg_type_def::e_rmt_success : msg_type_def::e_rmt_fail;

	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}