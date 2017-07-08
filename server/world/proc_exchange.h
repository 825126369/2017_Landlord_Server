#pragma once
#include <net\packet_manager.h>
#include "client2world_exchange.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initExchangePacket();

// 请求兑换
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_exchange, game_player);
// 请求兑换结果
PACKET_REGEDIT_SEND(packetw2c_exchange_result);

// 取得兑换状态
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_get_exchange_state, game_player);
// 取得兑换状态结果
PACKET_REGEDIT_SEND(packetw2c_get_exchange_state_result);


// 购物
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_shopping, game_player);
// 购物结果
PACKET_REGEDIT_SEND(packetw2c_shopping_result);
