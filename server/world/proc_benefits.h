#pragma once

#include <net\packet_manager.h>
#include <client2world_protocol.pb.h>
#include <client2world_benefits.pb.h>
#include <game_player.h>
#include "world_peer.h"

using namespace client2world_protocols;

void initBenefitsPacket();

PACKET_REGEDIT_RECVGATE(world_peer, packet_c2w_benefits, game_player);

PACKET_REGEDIT_SEND(packet_w2c_benefits_result);