#include "stdafx.h"
#include "proc_showhand_logic.h"
#include "game_engine.h"
#include <i_game_player.h>
#include "logic_lobby.h"
#include "logic_table.h"
#include "logic_player.h"
#include "logic_room.h"
#include "logic_table.h"
#include "msg_type_def.pb.h"
#include "cardmanager.h"

SHOWHAND_SPACE_USING
using namespace boost;

void init_proc_showhand_logic() {

	//请求房间列表信息
	packetc2l_get_room_info_factory::regedit_factory();
	//返回房间列表信息
	packetl2c_get_room_info_result_factory::regedit_factory();

    //请求进入游戏房间
    packetc2l_enter_game_room_factory::regedit_factory();
    //选择游戏房间响应信息
    packetl2c_enter_game_room_result_factory::regedit_factory();

    //退出游戏房间请求信息
    packetc2l_quit_game_room_factory::regedit_factory();
    //退出游戏房间，返回游戏主页响应信息
    packetl2c_quit_game_room_result_factory::regedit_factory();

    //玩家选择一个桌子（包括快速自动选桌加入）请求信息
    packetc2l_choose_desk_factory::regedit_factory();
    //玩家选择一个桌子（包括快速选桌加入）响应信息
    packetl2c_choose_desk_result_factory::regedit_factory();

	//桌子场景
	packetc2l_get_table_scene_info_factory::regedit_factory();
    //桌子场景响应信息
    packetl2c_get_table_scene_result_factory::regedit_factory();

    //离开桌子到房间请求信息
    packetc2l_quit_desk_factory::regedit_factory();
    //离开桌子到房间响应信息
    packet_l2c_quit_desk_result_factory::regedit_factory();

    //玩家进入准备状态请求信息
    packetc2l_player_ready_factory::regedit_factory();
    //玩家请求准备状态 响应信息
    packetl2c_player_ready_result_factory::regedit_factory();

    //玩家看底牌请求信息
    packetc2l_view_card_factory::regedit_factory();
    //玩家看底响应信息
    packetl2c_view_card_result_factory::regedit_factory();

    //玩家下注 请求信息
    packetc2l_bet_info_factory::regedit_factory();
	packetl2c_bet_info_result_factory::regedit_factory();

	//检查状态
	packetc2l_check_state_factory::regedit_factory();
	packetl2c_check_state_result_factory::regedit_factory();

	//通知 玩家加入桌子信息
	packetl2c_notice_join_table_factory::regedit_factory();
	//通知 玩家离开桌子信息
	packetl2c_notice_leave_table_factory::regedit_factory();
	//通知房间里玩家 桌子里玩家状态
	packetl2c_notice_table_player_state_factory::regedit_factory();

	//通知 开始游戏
	packetl2c_notice_start_game_message_factory::regedit_factory();
	//通知 玩家下注
	packetl2c_notice_bet_factory::regedit_factory();
	//通知 发牌
	packetl2c_notice_sendcard_message_factory::regedit_factory();
	//通知 奖励
	packetl2c_notice_award_message_factory::regedit_factory();

	packetl2c_notice_gm_all_card_info_factory::regedit_factory();
	packetc2l_gm_change_result_factory::regedit_factory();
	packetl2c_notice_gm_stock_info_factory::regedit_factory();
	packetl2c_notice_gm_luck_info_factory::regedit_factory();
}

//请求房间信息
bool packetc2l_get_room_info_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_get_room_info> msg) 
{
    __ENTER_FUNCTION_CHECK;

    auto sendmsg = PACKET_CREATE(packetl2c_get_room_info_result, e_mst_l2c_get_room_info_result);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());

    LROOM_MAP rooms = game_engine::instance().get_lobby().get_rooms();
	sendmsg->mutable_room_ids()->Reserve(rooms.size());

    for (LROOM_MAP::iterator it = rooms.begin(); it != rooms.end(); ++it) 
	{
		sendmsg->add_room_ids(it->first);
    }
    lcplayer->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//进入房间
bool packetc2l_enter_game_room_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_enter_game_room> msg) 
{
    __ENTER_FUNCTION_CHECK;
    auto sendmsg = PACKET_CREATE(packetl2c_enter_game_room_result, e_mst_l2c_enter_game_room_result);
    msg_type_def::e_msg_result_def result = msg_type_def::e_rmt_fail;

    int roomid = msg->room_id();
    auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if (!lcplayer->is_inRoom())
	{
		result = (msg_type_def::e_msg_result_def)lcplayer->enter_room(roomid);		
	}

	if(result == msg_type_def::e_rmt_success || lcplayer->is_inRoom())
	{
		lcplayer->get_room()->copy_tablelist(sendmsg->mutable_table_list());
		result = msg_type_def::e_rmt_success;
		sendmsg->set_room_id(lcplayer->get_room()->get_room_id());
	}

    sendmsg->set_result(result);
    player->send_msg_to_client(sendmsg);
    
	__LEAVE_FUNCTION_CHECK
    return !EX_CHECK;
}

//离开房间
bool packetc2l_quit_game_room_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_quit_game_room> msg) 
{
    __ENTER_FUNCTION_CHECK;
    
    auto sendmsg = PACKET_CREATE(packetl2c_quit_game_room_result, e_mst_l2c_quit_game_room_result);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if(lcplayer!=nullptr)
	{
		int result= lcplayer->leave_room();
		if(result == 1)
		{
			 LROOM_MAP rooms = game_engine::instance().get_lobby().get_rooms();
			sendmsg->mutable_room_ids()->Reserve(rooms.size());

			for (LROOM_MAP::iterator it = rooms.begin(); it != rooms.end(); ++it) 
			{
				sendmsg->add_room_ids(it->first);
			}
		}

		sendmsg->set_result((msg_type_def::e_msg_result_def)result);
	}else
	{
		sendmsg->set_result(msg_type_def::e_msg_result_def::e_rmt_fail);
	}

    lcplayer->send_msg_to_client(sendmsg);
	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//进入桌子
bool packetc2l_choose_desk_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_choose_desk> msg) 
{
    __ENTER_FUNCTION_CHECK;

 	auto sendmsg = PACKET_CREATE(packetl2c_choose_desk_result, e_mst_l2c_choose_desk_result);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());

    msg_type_def::e_msg_result_def result = msg_type_def::e_rmt_fail; 
	int table_id=-1;


	if(lcplayer->is_inTable())
	{
		result = msg_type_def::e_rmt_success;
	}
	else if(msg->table_id()==0 && lcplayer->is_inRoom() )//快速选桌
	{		
		table_id=lcplayer->get_room()->match_auto_table(lcplayer->get_last_select_table_id());
		lcplayer->set_last_select_table_id(table_id);
	}else//手动选桌
	{
		table_id=msg->table_id();
	}

	if(table_id>0 && lcplayer->is_inRoom())
	{
		result = (msg_type_def::e_msg_result_def)lcplayer->enter_table(table_id);

		//std::cout<<"进入桌子："<<lcplayer->get_pid()<<std::endl;
	}


	if(result == msg_type_def::e_rmt_success)
	{
		lcplayer->get_table()->copy_table_info(sendmsg->mutable_table_info());
		sendmsg->set_room_id(lcplayer->get_room()->get_room_id());
	}

	sendmsg->set_result(result);
    player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//离开桌子
bool packetc2l_quit_desk_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_quit_desk> msg) {

    __ENTER_FUNCTION_CHECK;

    auto sendmsg = PACKET_CREATE(packet_l2c_quit_desk_result, e_mst_l2c_quit_desk_result);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	msg_type_def::e_msg_result_def result = msg_type_def::e_rmt_fail;
	if(lcplayer!=nullptr && lcplayer->is_inRoom() && lcplayer->is_inTable())
	{
		result=(msg_type_def::e_msg_result_def)lcplayer->leave_table();
		if(lcplayer->get_room()!=nullptr)
		{
			sendmsg->set_room_id(lcplayer->get_room()->get_room_id());
		}
		//std::cout<<"离开桌子："<<lcplayer->get_pid()<<std::endl;
	}else
	{
		 result = msg_type_def::e_rmt_fail;
	}
	lcplayer->get_room()->copy_tablelist(sendmsg->mutable_table_list());
	sendmsg->set_result(result);
	player->send_msg_to_client(sendmsg);

	__LEAVE_FUNCTION_CHECK
    return !EX_CHECK;
}

//玩家准备
bool packetc2l_player_ready_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_player_ready> msg)
{
    __ENTER_FUNCTION_CHECK;
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if(lcplayer==nullptr)
	{
		//std::cout<<"Prepare..."<<std::endl;
		return true;
	}	
	lcplayer->prepare_game();
	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//看看底牌
bool packetc2l_view_card_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player,shared_ptr<packetc2l_view_card> msg) 
{
   	__ENTER_FUNCTION_CHECK;
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());

	lcplayer->seecard();

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//下注
bool packetc2l_bet_info_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_bet_info> msg) 
{
   	__ENTER_FUNCTION_CHECK

	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());

	auto result=lcplayer->add_bet(msg->bet_info());
	auto sendmsg = PACKET_CREATE(packetl2c_bet_info_result,e_mst_l2c_bet_result);
	if(sendmsg!=nullptr)
	{
		sendmsg->set_result((msg_type_def::e_msg_result_def)result);
		lcplayer->send_msg_to_client(sendmsg);
	}else
	{
		SLOG_CRITICAL<<"下注返回消息是空的"<<std::endl;
	}

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//玩家状态
bool packetc2l_check_state_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_check_state> msg)
{
	 __ENTER_FUNCTION_CHECK
	auto sendmsg = PACKET_CREATE(packetl2c_check_state_result, e_mst_l2c_check_state_result);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if (lcplayer != nullptr)
	{
		sendmsg->set_state(lcplayer->get_player_world_state());
	}
	else
	{
		sendmsg->set_state(false);
	}

	player->send_msg_to_client(sendmsg);
	__LEAVE_FUNCTION_CHECK

	return !EX_CHECK;
}


//请求桌子场景信息
bool packetc2l_get_table_scene_info_factory::packet_process(shared_ptr<peer_tcp> peer,shared_ptr<i_game_player> player,shared_ptr<packetc2l_get_table_scene_info> msg)
{
	 __ENTER_FUNCTION_CHECK
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	if (lcplayer != nullptr && lcplayer->is_inTable())
	{
		lcplayer->get_table()->do_protobuf_get_table_scene_info(lcplayer.get());
	}
	else
	{
		SLOG_CRITICAL<<"玩家不在桌子里，无法获取场景信息"<<std::endl;
	}
	__LEAVE_FUNCTION_CHECK

	return !EX_CHECK;
}

bool packetc2l_gm_change_result_factory::packet_process(shared_ptr<peer_tcp> peer, shared_ptr<i_game_player> player, 
													shared_ptr<packetc2l_gm_change_result> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetl2c_gm_change_result,e_mst_l2c_gm_change_result);
	auto lcplayer =  CONVERT_POINT(logic_player, player->get_handler());
	msg_type_def::e_msg_result_def result=msg_type_def::e_msg_result_def::e_rmt_fail;
	if (lcplayer != nullptr && lcplayer->get_room()!=nullptr && lcplayer->get_table()!=nullptr)
	{
		if(lcplayer->get_table()->get_player_left()->is_GM_CONTROL()!=lcplayer->get_table()->get_player_right()->is_GM_CONTROL())
		{
			int gm_command=0;
			if(msg->command()==1)
			{
				gm_command=lcplayer->get_table()->get_player_left()->get_pid()==lcplayer->get_pid()?1:2;
			}
			lcplayer->get_table()->set_GM_CONTROL_COMMAND(gm_command);
			result=msg_type_def::e_rmt_success;
		}
	}
	sendmsg->set_result(result);
	lcplayer->send_msg_to_client(sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}