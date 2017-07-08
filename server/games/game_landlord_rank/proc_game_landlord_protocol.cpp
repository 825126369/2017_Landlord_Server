#include "stdafx.h"
#include "proc_game_landlord_protocol.h"
#include <i_game_player.h>
#include "game_engine.h"
#include "logic_player.h"
using namespace boost;

void init_proc_landlord_protocol()
{
	packetc2l_Hang_factory::regedit_factory();
	packetl2c_Hang_result_factory::regedit_factory();
	packetc2l_ChangeTable_factory::regedit_factory();
	packetl2c_ChangeTable_result_factory::regedit_factory();
	packetc2l_SystemTalk_factory::regedit_factory();
	packetl2c_notice_SystemTalk_factory::regedit_factory();

	packetc2l_enter_room_factory::regedit_factory();
	packetl2c_enter_room_result_factory::regedit_factory();

	packetc2l_start_match_factory::regedit_factory();
	packetl2c_start_match_result_factory::regedit_factory();

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

//换桌
bool packetc2l_ChangeTable_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_ChangeTable> msg)
{
	__ENTER_FUNCTION_CHECK;


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

//进入斗地主房间
bool packetc2l_enter_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player,shared_ptr<packetc2l_enter_room> msg)
{	
	__ENTER_FUNCTION_CHECK;
	int ret = game_engine::instance().get_lobby().enter_room(player->get_playerid(), msg->room_id());

	auto sendmsg = PACKET_CREATE(packetl2c_enter_room_result, e_mst_l2c_enter_room);
	sendmsg->set_result((e_server_error_code)ret);
	if (ret == e_server_error_code::e_error_code_success)
	{
		auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	    int roomId=lcplayer->get_room()->get_room_id();
		sendmsg->set_room_id(roomId);
	}
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//匹配
bool packetc2l_start_match_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_start_match> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto lcplayer = CONVERT_POINT(logic_player, player->get_handler());
	e_server_error_code result= lcplayer->start_match();

	auto sendmsg = PACKET_CREATE(packetl2c_start_match_result, e_mst_l2c_start_match_result);
	sendmsg->set_result(result);
	if (result == e_server_error_code::e_error_code_success)
	{
		sendmsg->set_wait_time(lcplayer->get_wait_time());
	}
	lcplayer->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
};

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
	sendmsg->set_is_intable(lcplayer->is_inRoom());
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

