#pragma once
#include <net\packet_manager.h>
#include "client2world_online_reward.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initOnlineRewardPacket();

// 领取在线奖励
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_receive_online_reward, game_player);
// 领取在线奖励结果
PACKET_REGEDIT_SEND(packetw2c_receive_online_reward_result);

// 领取充值奖励
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_receive_recharge_reward, game_player);
// 领取充值奖励结果
PACKET_REGEDIT_SEND(packetw2c_receive_recharge_reward_result);

// 请求在线奖励的领取信息
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_online_reward_info, game_player);
// 请求在线奖励的领取信息结果
PACKET_REGEDIT_SEND(packetw2c_req_online_reward_info_result);

