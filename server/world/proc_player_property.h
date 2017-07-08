#pragma once
#include <net\packet_manager.h>
#include "client2world_player_property.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initPlayerPropertyPacket();

// 修改头像
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_update_playerhead, game_player);
// 修改头像结果
PACKET_REGEDIT_SEND(packetw2c_update_playerhead_result);

// 修改昵称
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_update_nickname, game_player);
// 修改昵称结果
PACKET_REGEDIT_SEND(packetw2c_update_nickname_result);

// 修改性别
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_update_sex, game_player);
// 修改性别结果
PACKET_REGEDIT_SEND(packetw2c_update_sex_result);

// 修改签名
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_update_signature, game_player);
// 修改签名结果
PACKET_REGEDIT_SEND(packetw2c_update_signature_result);

// 改变相框
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_change_photo_frame, game_player);
// 改变相框结果
PACKET_REGEDIT_SEND(packetw2c_change_photo_frame_result);

// 获取战绩统计
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_game_stat, game_player);
// 捕鱼的战绩统计结果
PACKET_REGEDIT_SEND(packetw2c_fishlord_stat_result);
// 骰宝的战绩统计结果
PACKET_REGEDIT_SEND(packetw2c_dice_stat_result);
// 鳄鱼的战绩统计结果
PACKET_REGEDIT_SEND(packetw2c_crocodile_stat_result);

// 个人记录
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_self_record, game_player);
// 个人记录结果
PACKET_REGEDIT_SEND(packetw2c_req_self_record_result);

// 完成某个新手引导
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_finish_one_new_guild, game_player);
// 完成某个新手引导结果
PACKET_REGEDIT_SEND(packetw2c_finish_one_new_guild_result);

// 请求赠送日志
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_send_gift_log, game_player);
// 请求赠送日志结果
PACKET_REGEDIT_SEND(packetw2c_req_send_gift_log_result);

// 请求保险箱日志
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_safebox_log, game_player);
// 请求保险箱日志结果
PACKET_REGEDIT_SEND(packetw2c_req_safebox_log_result);

// 举报头像
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_inform_playerhead, game_player);
// 举报头像结果
PACKET_REGEDIT_SEND(packetw2c_inform_playerhead_result);


