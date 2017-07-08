#include "stdafx.h"
#include "proc_game_happysupremacy_protocol.h"
#include <i_game_player.h>
#include "game_engine.h"
#include "logic_room.h"
#include "logic_player.h"

using namespace boost;

void init_proc_happysupremacy_protocol()
{
	packetc2l_get_room_info_factory::regedit_factory();
	packetl2c_get_room_info_result_factory::regedit_factory();
	packetc2l_enter_room_factory::regedit_factory();
	packetl2c_enter_room_result_factory::regedit_factory();
	packetc2l_get_room_scene_info_factory::regedit_factory();
	packetl2c_get_room_scene_info_result_factory::regedit_factory();
	packetc2l_leave_room_factory::regedit_factory();
	packetl2c_leave_room_result_factory::regedit_factory();
	packetc2l_add_bet_factory::regedit_factory();
	packetl2c_add_bet_result_factory::regedit_factory();
	packetc2l_repeat_bet_factory::regedit_factory();
	packetl2c_repeat_bet_result_factory::regedit_factory();
	packetc2l_clear_bet_factory::regedit_factory();
	packetl2c_clear_bet_result_factory::regedit_factory();
	packetc2l_ask_for_banker_factory::regedit_factory();
	packetl2c_ask_for_banker_result_factory::regedit_factory();
	packetc2l_leave_banker_factory::regedit_factory();
	packetl2c_leave_banker_result_factory::regedit_factory();
	packetc2l_ask_for_first_banker_factory::regedit_factory();
	packetl2c_ask_for_first_banker_result_factory::regedit_factory();
	packetc2l_ask_for_player_list_factory::regedit_factory();
	packetl2c_ask_for_player_list_result_factory::regedit_factory();
	packetc2l_ask_for_banker_list_factory::regedit_factory();
	packetl2c_ask_for_banker_list_result_factory::regedit_factory();
	packetc2l_ask_for_history_list_factory::regedit_factory();
	packetl2c_ask_for_history_list_result_factory::regedit_factory();

	packetl2c_bc_begin_bet_factory::regedit_factory();
	packetl2c_bc_begin_award_factory::regedit_factory();
	packetl2c_bc_total_bet_info_factory::regedit_factory();
	packetl2c_bc_rob_banker_info_factory::regedit_factory();
	packetl2c_bc_change_banker_factory::regedit_factory();

	packetc2l_check_state_factory::regedit_factory();
	packetl2c_check_state_result_factory::regedit_factory();

	packetl2c_bc_accept_gift_factory::regedit_factory();

	packetc2l_gm_factory::regedit_factory();

	/*packetc2l_gm_talk_server_factory::regedit_factory();
	packetl2c_gm_talk_server_factory::regedit_factory();
	packetc2l_gm_change_result_factory::regedit_factory();
	packetl2c_gm_change_result_factory::regedit_factory();
	packetl2c_notice_gm_all_bet_info_factory::regedit_factory();

	packetl2c_notice_gm_stock_info_factory::regedit_factory();*/
}

//获得房间信息(给客户单传递一个列表（房间ID列表，策划配置的）)
bool packetc2l_get_room_info_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
												 shared_ptr<packetc2l_get_room_info> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_get_room_info_result, e_mst_l2c_get_room_info_result);

	auto room_list = game_engine::instance().get_lobby().get_rooms();
	sendmsg->mutable_room_list()->Reserve(room_list.size());
	for (auto it = room_list.begin(); it != room_list.end(); ++it)
	{
		auto r = sendmsg->add_room_list();
		r->set_roomid(it->second->get_room_id());
	}
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//进入房间，获取场景信息
bool packetc2l_enter_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
													 shared_ptr<packetc2l_enter_room> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_enter_room_result, e_mst_l2c_enter_room_result);
	
	int ret = game_engine::instance().get_lobby().enter_room(player->get_playerid(), msg->roomid());
	sendmsg->set_result((msg_type_def::e_msg_result_def)ret);
	player->send_msg_to_client(sendmsg);
	
	//成功进入房间，发送场景信息
	if (ret == msg_type_def::e_rmt_success)
	{
		auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
		auto sendmsg = lcplayer->get_room()->get_room_scene_info();
		player->send_msg_to_client(sendmsg);	
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//断线重连，或者场景数据不匹配时, 请求场景信息
bool packetc2l_get_room_scene_info_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_get_room_scene_info> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if(lcplayer!=nullptr && lcplayer->get_room())
	{
		auto sendmsg = lcplayer->get_room()->get_room_scene_info();
		player->send_msg_to_client(sendmsg);	
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//离开房间
bool packetc2l_leave_room_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
												  shared_ptr<packetc2l_leave_room> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	auto sendmsg = PACKET_CREATE(packetl2c_leave_room_result, e_mst_l2c_leave_room_result);
	if (!lcplayer->get_is_banker())
	{
		lcplayer->leave_room();
		sendmsg->set_result(msg_type_def::e_rmt_success);
		sendmsg->set_player_gold(lcplayer->get_gold());
	}
	else
	{
		sendmsg->set_result(msg_type_def::e_rmt_can_not_leave);
	}
	

	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//下注
bool packetc2l_add_bet_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
												   shared_ptr<packetc2l_add_bet> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_add_bet_result, e_mst_l2c_add_bet);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	logic_room* room = lcplayer->get_room();
	if (room != nullptr)
	{
		if (room->get_room_state() == e_game_state::e_state_game_bet)	//在可押注的时间
		{
			msg_type_def::e_msg_result_def temp_result = lcplayer->add_bet(msg->betinfo().type(),msg->betinfo().bet_count());
			sendmsg->set_result(temp_result);
			if (temp_result == msg_type_def::e_rmt_success)
			{
				sendmsg->mutable_betinfo()->set_type(msg->betinfo().type());
				sendmsg->mutable_betinfo()->set_bet_count(msg->betinfo().bet_count());
			}
		}
		else
		{
			sendmsg->set_result(msg_type_def::e_rmt_no_can_bet);
		}
	}
	else
	{
		sendmsg->set_result(msg_type_def::e_rmt_no_can_bet);
	}
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//续压
bool packetc2l_repeat_bet_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
											   shared_ptr<packetc2l_repeat_bet> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_repeat_bet_result, e_mst_l2c_repeat_bet);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	logic_room* room = lcplayer->get_room();
	if (room != nullptr)
	{
		if (room->get_room_state() == e_game_state::e_state_game_bet)	//在可押注的时间
		{
			sendmsg->set_result(lcplayer->repeat_bet());
		}
		else
		{
			sendmsg->set_result(msg_type_def::e_rmt_no_can_bet);
		}
	}
	else
		sendmsg->set_result(msg_type_def::e_rmt_no_can_bet);


	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//清零
bool packetc2l_clear_bet_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_clear_bet> msg)
{	
	/*__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_clear_bet_result, e_mst_l2c_clear_bet);

	sendmsg->set_result(msg_type_def::e_rmt_no_can_bet); //禁用清零

	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;*/
	return true;
}

//上庄
bool packetc2l_ask_for_banker_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
												 shared_ptr<packetc2l_ask_for_banker> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_ask_for_banker_result, e_mst_l2c_ask_for_banker);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	logic_room* room = lcplayer->get_room();
	if (room != nullptr)
	{
		sendmsg->set_result(room->add_banker_list(lcplayer->get_pid()));
	}
	else
		sendmsg->set_result(msg_type_def::e_rmt_fail);

	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//下庄
bool packetc2l_leave_banker_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
													  shared_ptr<packetc2l_leave_banker> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_leave_banker_result, e_mst_l2c_leave_banker);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	sendmsg->set_result(lcplayer->leave_banker());

	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//抢庄
bool packetc2l_ask_for_first_banker_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
												 shared_ptr<packetc2l_ask_for_first_banker> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_ask_for_first_banker_result, e_mst_l2c_ask_first_for_banker);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	logic_room* room = lcplayer->get_room();
	if (room != nullptr)
	{
		sendmsg->set_result(room->set_rob_banker(lcplayer->get_pid()));
	}
	else
		sendmsg->set_result(msg_type_def::e_rmt_fail);

	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求玩家列表
bool packetc2l_ask_for_player_list_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
															shared_ptr<packetc2l_ask_for_player_list> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_ask_for_player_list_result, e_mst_l2c_ask_player_list);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	logic_room* room = lcplayer->get_room();
	if (room != nullptr)
	{
		sendmsg = room->get_room_player_list();
	}
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求上庄列表
bool packetc2l_ask_for_banker_list_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
															shared_ptr<packetc2l_ask_for_banker_list> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_ask_for_banker_list_result, e_mst_l2c_ask_banker_list);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	logic_room* room = lcplayer->get_room();
	if (room != nullptr)
	{
		sendmsg = room->get_room_banker_list();
	}
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求牌路
bool packetc2l_ask_for_history_list_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
															shared_ptr<packetc2l_ask_for_history_list> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_ask_for_history_list_result, e_mst_l2c_ask_history_list);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	logic_room* room = lcplayer->get_room();
	if (room != nullptr)
	{
		sendmsg = room->get_room_history_list();
	}
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//询问玩家是否在房间里(检测状态)
bool packetc2l_check_state_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, shared_ptr<packetc2l_check_state> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_check_state_result, e_mst_l2c_check_state_result);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if (lcplayer != nullptr)
	{
		sendmsg->set_is_intable(lcplayer->get_room()!=nullptr);
	}
	else
		sendmsg->set_is_intable(false);

	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//GM
bool packetc2l_gm_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
													shared_ptr<packetc2l_gm> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if (lcplayer != nullptr)
		lcplayer->get_room()->set_gm(msg->gm_max());

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}