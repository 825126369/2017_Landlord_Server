#pragma once
#include <net\packet_manager.h>
#include <game_landlord_net_human_protocol.pb.h>
#include <net\peer_tcp.h>

class i_game_player;
using namespace game_landlord_net_human_protocol;

void init_proc_landlord_protocol();

//创建房间/房间设置
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_create_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_create_room_result);

//托管
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_Hang, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_Hang_result);

//请求添加机器人
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_add_robot, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_add_robot_result);

//踢出房间
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_kich_player, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_kich_player_result);

//邀请玩家列表
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_join_player_list, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_join_player_list_result);

//邀请玩家
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_join_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_join_room_result);

//是否同意邀请
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_or_agree_join_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_or_agree_join_room_result);

//准备
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_or_prepare, i_game_player);

//系统聊天
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_SystemTalk, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_notice_SystemTalk);

//获取房间列表返回
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_roomlist, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_roomlist_result);

//进入房间
PACKET_REGEDIT_RECVGATE_LOG(peer_tcp, packetc2l_enter_room, i_game_player);
PACKET_REGEDIT_SEND(packetl2c_enter_room_result);

//离开房间
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
//通知抢地主
PACKET_REGEDIT_SEND(packetl2c_notice_rob_landlord_result);

//通知房间内 某玩家进出情况
PACKET_REGEDIT_SEND(packetl2c_notice_room_enter_leave_info);
//通知房间内 某玩家准备信息
PACKET_REGEDIT_SEND(packetl2c_notice_room_prepare_info);
//通知 房间外 房间信息
PACKET_REGEDIT_SEND(packetl2c_notice_room_change_info);

//通知 邀请加入房间信息列表
PACKET_REGEDIT_SEND(packetl2c_notice_invite_room_list);

//通知 房主改变信息
PACKET_REGEDIT_SEND(packetl2c_notice_who_is_roomcreator);




