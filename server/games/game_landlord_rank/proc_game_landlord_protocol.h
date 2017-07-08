#pragma once
#include <net\packet_manager.h>
#include <game_landlord_rank_protocol.pb.h>
#include <net\peer_tcp.h>

class i_game_player;
using namespace game_landlord_rank_protocol;

void init_proc_landlord_protocol();

//托管
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_Hang, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_Hang_result);

//换桌
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_ChangeTable, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_ChangeTable_result);

//系统聊天
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_SystemTalk, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_notice_SystemTalk);

//进入战场
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_enter_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_enter_room_result);

PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_start_match, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_start_match_result);

//离开战场
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_leave_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_leave_room_result);

//断线重连 场景信息
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_get_room_scene_info, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_get_room_scene_info_result);

//检测游戏状态		//进游戏之后判断是否在斗地主
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_check_state, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_check_state_result);

//请求出牌
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_playhand, i_game_player);

//抢地主
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_rob_landlord, i_game_player);

//通知开始游戏
PACKET_REGEDIT_SEND(packetl2c_notice_startgame);
//通知 某某人 出牌信息
PACKET_REGEDIT_SEND(packetl2c_notice_who_playhand);
//通知 其他人出牌信息
PACKET_REGEDIT_SEND(packetl2c_notice_playhand);
//通知 是否你要抢地主
PACKET_REGEDIT_SEND(packetl2c_notice_rob_landlord);
//通知 胜负
PACKET_REGEDIT_SEND(packetl2c_notice_winlose);

PACKET_REGEDIT_SEND(packetl2c_notice_rob_landlord_result);

