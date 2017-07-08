#include "stdafx.h"
#include "proc_world2logic_friend.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "game_manager.h"
#include <i_game_engine.h>
#include "proc_world2logic_protocol.h"

using namespace boost;

void initWorld2LogicFriend()
{
	packetl2w_enter_friend_room_result_factory::regedit_factory();
	packetw2l_enter_friend_room_factory::regedit_factory();
	packetl2w_request_robot_factory::regedit_factory();
}

// 进入好友房间
bool packetw2l_enter_friend_room_factory::packet_process(shared_ptr<server_peer> peer, 
														 shared_ptr<packetw2l_enter_friend_room> msg)
{	
	__ENTER_FUNCTION_CHECK;

	
	auto player = game_player_mgr::instance().find_playerbyid(msg->playerid());
	
	auto sendmsg = PACKET_CREATE(packetl2w_enter_friend_room_result, e_mst_l2w_enter_friend_room_result);
	sendmsg->set_playerid(msg->playerid());
	sendmsg->set_gameid(msg->gameid());
	auto eng = game_manager::instance().get_game_engine();
	if(!player && eng != nullptr)
	{	
		player = game_player::malloc();
		auto ainfo = msg->account_info();
		player->PlayerID = ainfo.aid();
		player->Gold = ainfo.gold();
		player->Ticket = ainfo.ticket();
		player->NickName = ainfo.nickname();
		player->VIPLevel = ainfo.viplvl();
		player->PhotoFrame = ainfo.curphotoframeid();
		player->Sex = ainfo.sex();
		player->IconCustom = ainfo.icon_custom();
		player->ExperienceVIP = ainfo.experience_vip();
		player->set_sessionid(msg->sessionid());
		player->CreateTime = ainfo.create_time();
		player->Privilege = ainfo.privilege();

		auto ainfoex = msg->account_info_ex();
		player->Lucky = ainfoex.lucky();
		player->TempIncome = ainfoex.temp_income();
		player->TotalIncome = ainfoex.total_income();

		int ret = eng->player_join_friend_game(player, msg->friendid());
		sendmsg->set_result((msg_type_def::e_msg_result_def)ret);
		if(sendmsg->result() == msg_type_def::e_rmt_success)
		{
			player->set_state(e_ps_playing);
			game_player_mgr::instance().add_player(player);							
		}			
	}

	peer->send_msg(sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
