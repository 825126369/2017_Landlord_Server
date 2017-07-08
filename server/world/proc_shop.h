#pragma once
#include <net\packet_manager.h>
#include "client2world_shop.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initShopPacket();

// 购买商品
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_buy_commodity, game_player);
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_ask_recharge_history, game_player);
// 购买商品结果
PACKET_REGEDIT_SEND(packetw2c_buy_commodity_result);
PACKET_REGEDIT_SEND(packetw2c_open_first_gift);
PACKET_REGEDIT_SEND(packetw2c_recharge_history_result);

// 验证苹果订单
PACKET_REGEDIT_RECVGATE(world_peer, packetw2c_check_apple_order_form, game_player);
