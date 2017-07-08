#include "stdafx.h"
#include "proc_logic2world_friend.h"
#include "proc_friend.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "pump_sys.h"

using namespace boost;

void initLogic2WorldFriend()
{
	packetl2w_enter_friend_room_result_factory::regedit_factory();
	packetw2l_enter_friend_room_factory::regedit_factory();
}

// 进入好友房间结果
bool packetl2w_enter_friend_room_result_factory::packet_process(shared_ptr<world_peer> peer, 
																shared_ptr<packetl2w_enter_friend_room_result> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_enter_friend_room_result, e_mst_w2c_enter_friend_room_result);
	auto player = game_player_mgr::instance().findPlayerById(msg->playerid());
	if(player)
	{
		sendmsg->set_result(msg->result());
		if(msg->result() == msg_type_def::e_rmt_success)
		{
			player->on_joingame(false);
			sendmsg->set_gameid(msg->gameid());
			sendmsg->set_result(msg_type_def::e_rmt_success);
			
			if(!player->IsRobot->get_value())
				GLOBAL_SYS(PumpSys)->enterGame(player->get_gameid(), player->PlayerId->get_value());		
		}
		else
		{
			player->resetGameIdServerId();
		}
	}
	else
	{
		sendmsg->set_result(msg_type_def::e_rmt_unknow);
	}
	player->send_msg_to_client(sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
