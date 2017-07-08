#include "stdafx.h"
#include "proc_logic2world_protocol.h"
#include "game_engine_mgr.h"
#include "proc_c2w_lobby_protocol.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "pump_sys.h"
#include "chat_sys.h"
#include "global_sys_mgr.h"
#include "robots_sys.h"
#include "game_quest_mgr.h"
#include "star_lottery_mgr.h"
#include "M_BaseInfoCFG.h"

using namespace boost;

void initLogic2WorldPacket()
{
	packetl2w_game_ready_factory::regedit_factory();
	packetl2w_player_login_result_factory::regedit_factory();
	packetl2w_player_logout_result_factory::regedit_factory();
	packetw2l_change_player_property_factory::regedit_factory();

	packetw2l_player_login_factory::regedit_factory();
	packetw2l_player_logout_factory::regedit_factory();
	packetl2w_player_property_stat_factory::regedit_factory();

	packetw2l_accept_gift_factory::regedit_factory();
	packetl2w_game_broadcast_factory::regedit_factory();

	packetl2w_player_quest_change_factory::regedit_factory();
	packetl2w_player_star_change_factory::regedit_factory();
}

//游戏准备好了
bool packetl2w_game_ready_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packetl2w_game_ready> msg)
{	
	__ENTER_FUNCTION_CHECK;

	game_info gi;
	gi.GameID = msg->game_id();
	gi.GameVer = msg->game_ver();
	gi.GamePlayerCount = 0;
	gi.GameServerID = peer->get_remote_id();
	
	if(!game_engine_mgr::instance().add_game_info(gi))
	{
		SLOG_ERROR << "regedit game error  gameid:"<<gi.GameID <<" serverid:"<<gi.GameServerID;
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//进入游戏返回
bool packetl2w_player_login_result_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packetl2w_player_login_result> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto player = game_player_mgr::instance().findPlayerById(msg->playerid());
	if(player)
	{
		auto sendmsg = PACKET_CREATE(packetw2c_enter_game_result, e_mst_w2c_enter_game_result);
		if(msg->result() == msg_type_def::e_rmt_success)
		{
			player->on_joingame(false);
			sendmsg->set_result(msg_type_def::e_rmt_success);

			if(!player->IsRobot->get_value())
				GLOBAL_SYS(PumpSys)->enterGame(player->get_gameid(), player->PlayerId->get_value());
		}
		else
		{
			player->resetGameIdServerId();
			if(player->IsRobot->get_value())
				player->player_logout();
		}
		player->send_msg_to_client(sendmsg);
		
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


//退出游戏返回
bool packetl2w_player_logout_result_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packetl2w_player_logout_result> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto player = game_player_mgr::instance().findPlayerById(msg->playerid());
	if(player)
	{
		player->leave_game();

		if(player->IsRobot->get_value())
		{
			//机器人完全退出游戏			
			player->player_logout();			
		}
		else
		{
			auto sendmsg = PACKET_CREATE(packetw2c_leave_game_result, e_mst_w2c_leave_game_result);
			player->send_msg_to_client( sendmsg);	
		}
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//属性改变
bool packetw2l_change_player_property_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packetw2l_change_player_property> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto player = game_player_mgr::instance().findPlayerById(msg->playerid());
	if(player)
	{
		if(msg->has_change_info())
		{
			auto cinfo =msg->change_info();
			if(cinfo.has_gold())
			{
				player->change_gold(cinfo.gold(), false, false, 105);
				player->check_firstgift(cinfo.gold());
				//player->store_game_object();
			}
			if(cinfo.has_ticket())
			{
				player->change_ticket(cinfo.ticket(), false);
				//player->store_game_object();
			}
		}
		
		if(msg->has_change_info_ex())
		{
			auto cinfoex =msg->change_info_ex();
			if(cinfoex.has_lucky())
			{
				player->Lucky->set_value(cinfoex.lucky());			
				player->TempIncome->set_value(cinfoex.temp_income());			
				player->TotalIncome->set_value(cinfoex.total_income());
			}
		}
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

bool packetl2w_player_property_stat_factory::packet_process(shared_ptr<world_peer> peer, 
															shared_ptr<packetl2w_player_property_stat> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto player = game_player_mgr::instance().findPlayerById(msg->playerid());
	if(player)
	{
		GLOBAL_SYS(PumpSys)->moneyTotalLog(player.get(), msg->gameid(), msg->attrtype(), msg->addvalue(), msg->reason());
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//广播游戏消息
bool packetl2w_game_broadcast_factory::packet_process(shared_ptr<world_peer> peer, 
															shared_ptr<packetl2w_game_broadcast> msg)
{	
	__ENTER_FUNCTION_CHECK;

	GLOBAL_SYS(ChatSys)->sysNotify(msg->game_msg(), msg_type_def::NotifyTypeWinningPrize);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//玩家任务成就改变
bool packetl2w_player_quest_change_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packetl2w_player_quest_change> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto player = game_player_mgr::instance().findPlayerById(msg->player_id());
	if(player)
	{
		player->get_sys<game_quest_mgr>()->change_quest(msg->quest_id(), msg->q_count(), msg->ex_param());
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//星星属性改变
bool packetl2w_player_star_change_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packetl2w_player_star_change> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto player = game_player_mgr::instance().findPlayerById(msg->player_id());
	if(player)
	{
		auto mgr = player->get_sys<StarLotteryMgr>();
		if(msg->addaward()>0)
			mgr->CurAward->add_value(msg->addaward());

		static int StarMax = M_BaseInfoCFG::GetSingleton()->GetData("StarMax")->mValue;
		if(msg->addstar() > 0 && mgr->CurStar->get_value() < StarMax)
		{
			mgr->CurStar->add_value(msg->addstar());
			if(mgr->CurStar->get_value() > StarMax)
				mgr->CurStar->set_value(StarMax);
		}

		//player->store_game_object();
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
