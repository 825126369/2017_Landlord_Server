#pragma once
#include <net\packet_manager.h>
#include "client2world_dial_lottery.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initDialLotteryPacket();

// 请求转盘抽奖
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_dial_lottery, game_player);
// 请求转盘抽奖结果
PACKET_REGEDIT_SEND(packetw2c_req_dial_lottery_result);

// 请求转盘标志
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_dial_lottery_flag, game_player);
// 请求转盘标志结果
PACKET_REGEDIT_SEND(packetw2c_req_dial_lottery_flag_result);

// 领取月卡奖励
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_month_card_reward, game_player);
// 领取月卡奖励结果
PACKET_REGEDIT_SEND(packetw2c_req_month_card_reward_result);

