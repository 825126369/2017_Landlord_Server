#pragma once
#include <net\packet_manager.h>
#include "client2world_daily_box_lottery.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initDailyBoxLotteryPacket();

// «Î«Û±¶œ‰≥ÈΩ±
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_lottery_box, game_player);
// «Î«Û±¶œ‰≥ÈΩ±Ω·π˚
PACKET_REGEDIT_SEND(packetw2c_req_lottery_box_result);

// –ª–ª≤Œ”Î∂“ªª¿Ò»Ø
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_thankyou_exchange_ticket, game_player);
// –ª–ª≤Œ”Î∂“ªª¿Ò»ØΩ·π˚
PACKET_REGEDIT_SEND(packetw2c_thankyou_exchange_ticket_result);


