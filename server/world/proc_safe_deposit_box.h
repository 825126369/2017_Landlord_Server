#pragma once
#include <net\packet_manager.h>
#include "client2world_safe_deposit_box.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initSafeDepositBoxPacket();

// 设置保险箱密码
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_set_password, game_player);
// 设置保险箱密码结果
PACKET_REGEDIT_SEND(packetw2c_set_password_result);

// 修改保险箱密码
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_modify_password, game_player);
// 修改保险箱密码结果
PACKET_REGEDIT_SEND(packetw2c_modify_password_result);

// 重置保险箱密码
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_reset_password, game_player);
// 重置保险箱密码结果
PACKET_REGEDIT_SEND(packetw2c_reset_password_result);

// 重置保险箱密码
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_check_password, game_player);
// 重置保险箱密码结果
PACKET_REGEDIT_SEND(packetw2c_check_password_result);

// 存入金币
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_deposit_gold, game_player);
// 存入金币结果
PACKET_REGEDIT_SEND(packetw2c_deposit_gold_result);

// 取出金币
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_draw_gold, game_player);
// 取出金币结果
PACKET_REGEDIT_SEND(packetw2c_draw_gold_result);

// 获取保险箱的验证码
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_get_safe_box_security_code, game_player);
// 获取保险箱的验证码结果
PACKET_REGEDIT_SEND(packetw2c_get_safe_box_security_code_result);
