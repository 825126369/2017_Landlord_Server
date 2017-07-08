#pragma once
#include <net\packet_manager.h>
#include <game_happysupremacy_protocol.pb.h>
#include <net\peer_tcp.h>

class i_game_player;
using namespace game_happysupremacy_protocols;

void init_proc_happysupremacy_protocol();

//获得房间信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_get_room_info, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_get_room_info_result);

//进入桌子即进入"初级场"
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_enter_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_enter_room_result);

//获得桌子内场景信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_get_room_scene_info, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_get_room_scene_info_result);

//离开桌子即离开"初级场"
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_leave_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_leave_room_result);

//下注
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_add_bet, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_add_bet_result);

//续压
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_repeat_bet, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_repeat_bet_result);

//清零
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_clear_bet, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_clear_bet_result);

//上庄
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_ask_for_banker, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_ask_for_banker_result);

//下庄
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_leave_banker, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_leave_banker_result);

//抢庄
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_ask_for_first_banker, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_ask_for_first_banker_result);

//请求玩家列表
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_ask_for_player_list, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_ask_for_player_list_result);

//请求上庄列表
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_ask_for_banker_list, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_ask_for_banker_list_result);

//请求牌路
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_ask_for_history_list, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_ask_for_history_list_result);

//通知开始押注
PACKET_REGEDIT_SEND(packetl2c_bc_begin_bet);

//通知开奖
PACKET_REGEDIT_SEND(packetl2c_bc_begin_award);

//通知所以玩家下注信息
PACKET_REGEDIT_SEND(packetl2c_bc_total_bet_info);

//通知抢庄信息
PACKET_REGEDIT_SEND(packetl2c_bc_rob_banker_info);

//通知更换庄家
PACKET_REGEDIT_SEND(packetl2c_bc_change_banker);

//检测协议 玩家是否在桌子上
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_check_state, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_check_state_result);

//广播收到礼物
PACKET_REGEDIT_SEND(packetl2c_bc_accept_gift);

//GM
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_gm, i_game_player);

/*//-----------------------------控制客户端-----------------------------------------
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp,packetc2l_gm_talk_server, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_gm_talk_server);

PACKET_REGEDIT_RECVGATE_LOG(peer_tcp,packetc2l_gm_change_result, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_gm_change_result);

PACKET_REGEDIT_SEND(packetl2c_notice_gm_all_bet_info);


//-----------------------------------------
PACKET_REGEDIT_SEND(packetl2c_notice_gm_stock_info);*/