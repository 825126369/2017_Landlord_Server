#include "stdafx.h"
#include "proc_game_landlord_protocol.h"
#include <i_game_player.h>
#include "game_engine.h"
#include "logic_player.h"
using namespace boost;

void init_proc_landlord_protocol()
{
	packetc2l_create_room_factory::regedit_factory();
	packetl2c_create_room_result_factory::regedit_factory();

	packetc2l_Hang_factory::regedit_factory();
	packetl2c_Hang_result_factory::regedit_factory();

	packetc2l_add_robot_factory::regedit_factory();
	packetl2c_add_robot_result_factory::regedit_factory();

	packetc2l_kich_player_factory::regedit_factory();
	packetl2c_kich_player_result_factory::regedit_factory();

	packetc2l_join_player_list_factory::regedit_factory();
	packetl2c_join_player_list_result_factory::regedit_factory();

	packetc2l_join_room_factory::regedit_factory();
	packetl2c_join_room_result_factory::regedit_factory();

	packetc2l_or_agree_join_room_factory::regedit_factory();
	packetl2c_or_agree_join_room_result_factory::regedit_factory();

	packetc2l_or_prepare_factory::regedit_factory();

	packetc2l_roomlist_factory::regedit_factory();
	packetl2c_roomlist_result_factory::regedit_factory();

	packetc2l_SystemTalk_factory::regedit_factory();
	packetl2c_notice_SystemTalk_factory::regedit_factory();

	packetc2l_enter_room_factory::regedit_factory();
	packetl2c_enter_room_result_factory::regedit_factory();

	packetc2l_leave_room_factory::regedit_factory();
	packetl2c_leave_room_result_factory::regedit_factory();

	packetc2l_get_room_scene_info_factory::regedit_factory();
	packetl2c_get_room_scene_info_result_factory::regedit_factory();

	packetc2l_check_state_factory::regedit_factory();
	packetl2c_check_state_result_factory::regedit_factory();

	packetc2l_playhand_factory::regedit_factory();

	packetc2l_rob_landlord_factory::regedit_factory();

	packetl2c_notice_startgame_factory::regedit_factory();
	packetl2c_notice_who_playhand_factory::regedit_factory();
	packetl2c_notice_playhand_factory::regedit_factory();
	packetl2c_notice_rob_landlord_factory::regedit_factory();
	packetl2c_notice_winlose_factory::regedit_factory();
	packetl2c_notice_rob_landlord_result_factory::regedit_factory();
	packetl2c_notice_room_enter_leave_info_factory::regedit_factory();
	packetl2c_notice_room_prepare_info_factory::regedit_factory();
	packetl2c_notice_room_change_info_factory::regedit_factory();
	packetl2c_notice_invite_room_list_factory::regedit_factory();
	packetl2c_notice_who_is_roomcreator_factory::regedit_factory();

}

//创建房间
bool packetc2l_create_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_create_room> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());

	if (msg->action() == 1)
	{
		auto result = lcplayer->get_lobby()->CreateRoom(lcplayer, msg.get());
		if (result == e_server_error_code::e_error_code_success)
		{
			auto sendmsg = PACKET_CREATE(packetl2c_enter_room_result, e_mst_l2c_enter_room);//创建成功后，就返回进入房间协议
			sendmsg->set_result(result);
			if (result == e_server_error_code::e_error_code_success)
			{
				auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
				auto m_room = lcplayer->get_table();
				sendmsg->mutable_roominfo()->set_room_id(m_room->get_table_id());
				sendmsg->mutable_roominfo()->set_password(m_room->get_roomPassword());
				sendmsg->mutable_roominfo()->set_roomname(m_room->get_roomName());
				sendmsg->mutable_roominfo()->set_room_creator(m_room->get_room_creator());

				std::map<int32_t, LPlayerPtr>& playerMap = m_room->get_playerMap();
				for (auto& var : playerMap)
				{
					auto playerInfo = sendmsg->add_otherplayerinfos();
					playerInfo->set_player_nickname(var.second->get_nickname());
					playerInfo->set_deskid(var.second->get_deskId());
					playerInfo->set_player_id(var.second->get_pid());
					playerInfo->set_player_head_frame(var.second->get_head_frame_id());
					playerInfo->set_player_gold(var.second->get_gold());
					playerInfo->set_player_sex(var.second->get_player_sex());
					playerInfo->set_player_vip_lv(var.second->get_viplvl());
					playerInfo->set_orprepare(var.second->get_orPrepare());
				}
				sendmsg->set_deskid(lcplayer->get_deskId());
			}
			player->send_msg_to_client(sendmsg);
		}

		auto sendmsg = PACKET_CREATE(packetl2c_create_room_result, e_mst_l2c_create_room);//创建成功后，就返回进入房间协议
		sendmsg->set_result(result);
		sendmsg->set_action(msg->action());
		player->send_msg_to_client(sendmsg);

	}
	else
	{
		e_server_error_code result = lcplayer->get_table()->Set_Room_Info(msg->roomname(), msg->password(), msg->orshuffle());

		auto sendmsg = PACKET_CREATE(packetl2c_create_room_result, e_mst_l2c_create_room);//创建成功后，就返回进入房间协议
		sendmsg->set_result(result);
		sendmsg->set_action(msg->action());
		player->send_msg_to_client(sendmsg);

	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//托管
bool packetc2l_Hang_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_Hang> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code result = lcplayer->SetOrHang(msg->orhang());

	auto sendmsg = PACKET_CREATE(packetl2c_Hang_result, e_mst_l2c_Hang);
	sendmsg->set_result(result);
	if (result == e_server_error_code::e_error_code_success)
	{
		sendmsg->set_orhang(msg->orhang());
	}
	lcplayer->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//聊天
bool packetc2l_SystemTalk_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_SystemTalk> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//添加机器人
bool  packetc2l_add_robot_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_add_robot> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code result=lcplayer->add_rob_player(msg->room_deskid());

	auto sendmsg = PACKET_CREATE(packetl2c_add_robot_result, e_mst_l2c_add_robot);
	sendmsg->set_result(result);

	lcplayer->send_msg_to_client(sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//踢出房间
bool  packetc2l_kich_player_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_kich_player> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code result=lcplayer->kich_player(msg->room_deskid());

	auto sendmsg = PACKET_CREATE(packetl2c_kich_player_result, e_mst_l2c_kich_player);
	sendmsg->set_result(result);
	lcplayer->send_msg_to_client(sendmsg);
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//邀请玩家列表
bool  packetc2l_join_player_list_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_join_player_list> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());

	auto sendmsg = PACKET_CREATE(packetl2c_join_player_list_result,e_mst_l2c_join_player_list_result);
	lcplayer->get_invite_player_list(sendmsg.get());
	lcplayer->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//邀请玩家加入房间
bool  packetc2l_join_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_join_room> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	auto result=lcplayer->invite_player(msg->playerid());

	auto sendmsg = PACKET_CREATE(packetl2c_join_room_result, e_mst_l2c_join_room_result);
	sendmsg->set_result(result);

	lcplayer->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//是否同意邀请
bool  packetc2l_or_agree_join_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_or_agree_join_room> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	auto result=lcplayer->or_agree_join_room(msg->joinid(),msg->oragree());
	if (result == e_server_error_code::e_error_code_success && msg->oragree())
	{
		auto sendmsg = PACKET_CREATE(packetl2c_enter_room_result, e_mst_l2c_enter_room);
		sendmsg->set_result(result);
		auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
		auto m_room = lcplayer->get_table();
		sendmsg->mutable_roominfo()->set_room_id(m_room->get_table_id());
		sendmsg->mutable_roominfo()->set_password(m_room->get_roomPassword());
		sendmsg->mutable_roominfo()->set_roomname(m_room->get_roomName());
		sendmsg->mutable_roominfo()->set_room_creator(m_room->get_room_creator());

		std::map<int32_t, LPlayerPtr>& playerMap = m_room->get_playerMap();
		for (auto& var : playerMap)
		{
			auto playerInfo = sendmsg->add_otherplayerinfos();
			playerInfo->set_player_nickname(var.second->get_nickname());
			playerInfo->set_deskid(var.second->get_deskId());
			playerInfo->set_player_id(var.second->get_pid());
			playerInfo->set_player_head_frame(var.second->get_head_frame_id());
			playerInfo->set_player_gold(var.second->get_gold());
			playerInfo->set_player_sex(var.second->get_player_sex());
			playerInfo->set_player_vip_lv(var.second->get_viplvl());
			playerInfo->set_orprepare(var.second->get_orPrepare());
		}
		sendmsg->set_deskid(lcplayer->get_deskId());
		player->send_msg_to_client(sendmsg);
	}

	auto sendmsg = PACKET_CREATE(packetl2c_or_agree_join_room_result, e_mst_l2c_or_agree_join_room_result);
	sendmsg->set_result(result);
	lcplayer->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//是否准备
bool  packetc2l_or_prepare_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_or_prepare> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	lcplayer->or_prepare(msg->orprepare());
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//获取房间列表
bool packetc2l_roomlist_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_roomlist> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());

	auto sendmsg = PACKET_CREATE(packetl2c_roomlist_result, e_mst_l2c_roomlist);
	lcplayer->get_lobby()->get_all_room_list(sendmsg.get());

	lcplayer->send_msg_to_client(sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


//进入斗地主房间
bool packetc2l_enter_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_enter_room> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code reuslt = e_server_error_code::e_error_code_failed;
	if (msg->room_id() > 0)
	{
		reuslt = lcplayer->enter_room(msg->room_id(),msg->password());
	}
	else
	{
		reuslt = lcplayer->start_match();
	}

	auto sendmsg = PACKET_CREATE(packetl2c_enter_room_result, e_mst_l2c_enter_room);
	sendmsg->set_result(reuslt);
	if (reuslt == e_server_error_code::e_error_code_success)
	{
		auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
		auto m_room = lcplayer->get_table();
		sendmsg->mutable_roominfo()->set_room_id(m_room->get_table_id());
		sendmsg->mutable_roominfo()->set_password(m_room->get_roomPassword());
		sendmsg->mutable_roominfo()->set_roomname(m_room->get_roomName());
		sendmsg->mutable_roominfo()->set_room_creator(m_room->get_room_creator());

		std::map<int32_t, LPlayerPtr>& playerMap = m_room->get_playerMap();
		for (auto& var : playerMap)
		{
			auto playerInfo = sendmsg->add_otherplayerinfos();
			playerInfo->set_player_nickname(var.second->get_nickname());
			playerInfo->set_deskid(var.second->get_deskId());
			playerInfo->set_player_id(var.second->get_pid());
			playerInfo->set_player_head_frame(var.second->get_head_frame_id());
			playerInfo->set_player_gold(var.second->get_gold());
			playerInfo->set_player_sex(var.second->get_player_sex());
			playerInfo->set_player_vip_lv(var.second->get_viplvl());
			playerInfo->set_orprepare(var.second->get_orPrepare());
		}
		sendmsg->set_deskid(lcplayer->get_deskId());
	}
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//离开斗地主房间
bool packetc2l_leave_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_leave_room> msg)
{
	__ENTER_FUNCTION_CHECK;

	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code result=lcplayer->leave_room();

	auto sendmsg = PACKET_CREATE(packetl2c_leave_room_result, e_mst_l2c_leave_room);
	sendmsg->set_result(result);
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//断线重练 获取场景信息
bool packetc2l_get_room_scene_info_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_get_room_scene_info> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	lcplayer->do_protobuf_send_SceneInfo();
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//检测游戏状态,进入游戏之后判断是否在桌子中
bool packetc2l_check_state_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_check_state> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	auto sendmsg = PACKET_CREATE(packetl2c_check_state_result, e_mst_l2c_check_state);
	sendmsg->set_is_intable(lcplayer->is_inTable() && lcplayer->get_table()->orInGame());
	player->send_msg_to_client(sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//出牌
bool packetc2l_playhand_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_playhand> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code result=lcplayer->playhand(msg->cards());
	if (result != e_server_error_code::e_error_code_success)
	{
		auto sendmsg = PACKET_CREATE(packetl2c_notice_playhand, e_mst_l2c_notice_playhand);
		sendmsg->set_result(result);
		player->send_msg_to_client(sendmsg);
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//抢地主
bool packetc2l_rob_landlord_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_rob_landlord> msg)
{
	__ENTER_FUNCTION_CHECK;

	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code result=lcplayer->robLandlord(msg->or_rob());

	if (result != e_server_error_code::e_error_code_success)
	{
		auto sendmsg = PACKET_CREATE(packetl2c_notice_rob_landlord_result, e_mst_l2c_notice_rob_landlord_result);
		sendmsg->set_result(result);
		player->send_msg_to_client(sendmsg);
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;

}

