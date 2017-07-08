#pragma once
#include <net\packet_manager.h>
#include "client2world_star_lottery.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initStarLotteryPacket();

// 请求转盘抽奖
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_star_lottery_info, game_player);
// 请求转盘抽奖结果
PACKET_REGEDIT_SEND(packetw2c_star_lottery_info_result);

// 请求转盘标志
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_star_lottery, game_player);
// 请求抽奖结果
PACKET_REGEDIT_SEND(packetc2w_req_star_lottery_result);


