#pragma once
#include <net\packet_manager.h>
#include <client2world_player_quest.pb.h>
#include "world_peer.h"

class game_player;
using namespace client2world_protocols;

void initQuestPacket();

//client <-> world
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_get_questlist, game_player);
PACKET_REGEDIT_SEND(packetw2c_get_questlist_result);

PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_receive_questreward, game_player);
PACKET_REGEDIT_SEND(packetw2c_receive_questreward_result);

PACKET_REGEDIT_SEND(packetw2c_change_quest);