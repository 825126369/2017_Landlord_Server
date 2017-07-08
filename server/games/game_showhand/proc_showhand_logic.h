#pragma once
#include <net\packet_manager.h>
#include <game_showhand_protocol.pb.h>
#include <net\peer_tcp.h>

class i_game_player;
using namespace game_showhand_protocols;

void init_proc_showhand_logic();

//请求房间列表信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_get_room_info, i_game_player);
//进入房间返回列表信息
PACKET_REGEDIT_SEND(packetl2c_get_room_info_result);

//进入游戏房间请求信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_enter_game_room, i_game_player);
//进入游戏房间响应信息
PACKET_REGEDIT_SEND(packetl2c_enter_game_room_result);

//退出游戏房间请求信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_quit_game_room, i_game_player);
//退出游戏房间响应信息
PACKET_REGEDIT_SEND(packetl2c_quit_game_room_result);

//进入桌子（包括快速自动选桌加入）请求信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_choose_desk, i_game_player);
//玩家选择一个桌子，响应信息
PACKET_REGEDIT_SEND(packetl2c_choose_desk_result);

//桌子请求 场景信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_get_table_scene_info, i_game_player);
//响应场景信息
PACKET_REGEDIT_SEND( packetl2c_get_table_scene_result);

//离开桌子到房间请求信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_quit_desk, i_game_player);
//离开桌子到房间响应信息
PACKET_REGEDIT_SEND(packet_l2c_quit_desk_result);

//玩家进入准备状态请求信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_player_ready, i_game_player);
//玩家请求准备状态 响应信息
PACKET_REGEDIT_SEND(packetl2c_player_ready_result);

//玩家看底牌请求信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_view_card, i_game_player);
//玩家看底响应信息
PACKET_REGEDIT_SEND(packetl2c_view_card_result);

//玩家下注请求信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_bet_info, i_game_player);
//玩家看底响应信息
PACKET_REGEDIT_SEND(packetl2c_bet_info_result);

//检查房间状态
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp,packetc2l_check_state,i_game_player);
//检查房间状态返回
PACKET_REGEDIT_SEND(packetl2c_check_state_result);

//推送 玩家加入桌子列表信息
PACKET_REGEDIT_SEND(packetl2c_notice_join_table);
//推送 玩家离开桌子列表信息
PACKET_REGEDIT_SEND(packetl2c_notice_leave_table);
//推送 玩家离开桌子列表信息
PACKET_REGEDIT_SEND(packetl2c_notice_table_player_state);

//推送 开始游戏信息
PACKET_REGEDIT_SEND(packetl2c_notice_start_game_message);
//推送 谁下注信息
PACKET_REGEDIT_SEND(packetl2c_notice_bet);
//推送 发牌信息
PACKET_REGEDIT_SEND(packetl2c_notice_sendcard_message);
//推送 玩家开奖
PACKET_REGEDIT_SEND(packetl2c_notice_award_message);

//推送 GM 所有玩家的信息
PACKET_REGEDIT_SEND(packetl2c_notice_gm_all_card_info);
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp,packetc2l_gm_change_result,i_game_player);
PACKET_REGEDIT_SEND(packetl2c_gm_change_result);

PACKET_REGEDIT_SEND(packetl2c_notice_gm_stock_info);

PACKET_REGEDIT_SEND(packetl2c_notice_gm_luck_info); 


