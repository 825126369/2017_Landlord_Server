#pragma once
#include <net\packet_manager.h>
#include <server_protocol.pb.h>
#include "world_peer.h"

using namespace server_protocols;

void initWorldPacket();

//all <-> gate
PACKET_REGEDIT_RECV(world_peer, packet_transmit_msg);
PACKET_REGEDIT_SEND(packet_broadcast_msg);
PACKET_REGEDIT_SEND(packet_regedit_route_msg);
PACKET_REGEDIT_SEND(packet_broadcast_msg2);

PACKET_REGEDIT_SEND(packet_clear_session);


//all <-> world
PACKET_REGEDIT_RECV_LOG(world_peer, packet_server_connect);
PACKET_REGEDIT_SEND(packet_server_connect_result);

//player
PACKET_REGEDIT_RECV(world_peer, packet_player_disconnect);
PACKET_REGEDIT_SEND(packet_player_connect);

//monitor->world
PACKET_REGEDIT_RECV_LOG(world_peer, packet_http_command);