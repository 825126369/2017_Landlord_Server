#pragma once
#include <net\packet_manager.h>
#include "client2world_mail.pb.h"
#include "world_peer.h"

class game_player;
using namespace client2world_protocols;

void initMailPacket();

// 获取邮件列表请求
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_get_mails, game_player);
// 获取邮件列表回应
PACKET_REGEDIT_SEND(packetw2c_get_mails_result);

// 领取邮件中的礼物请求
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_recv_mail_gifts, game_player);
// 领取邮件中的礼物请求结果
PACKET_REGEDIT_SEND(packetw2c_recv_mail_gifts_result);

// 发送邮件
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_send_mail, game_player);
// 发送邮件结果
PACKET_REGEDIT_SEND(packetw2c_send_mail_result);

// 删除某个邮件
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_remove_mail, game_player);
// 删除某个邮件结果
PACKET_REGEDIT_SEND(packetw2c_remove_mail_result);

// 收到礼物通知
PACKET_REGEDIT_SEND(packetw2c_accept_gift_notify);

// 请求发送邮件日志
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_send_mail_log, game_player);
// 请求发送邮件日志结果
PACKET_REGEDIT_SEND(packetw2c_req_send_mail_log_result);

// 删除某个邮件日志
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_remove_mail_log, game_player);
// 删除某个邮件日志结果
PACKET_REGEDIT_SEND(packetw2c_remove_mail_log_result);
