#pragma once
#include <net\packet_manager.h>
#include <server_protocol.pb.h>
#include "logic_peer.h"

using namespace server_protocols;

void init_logic_protocol();

//all <-> gate
PACKET_REGEDIT_RECV_LOG(logic_peer, packet_transmit_msg);
PACKET_REGEDIT_SEND(packet_broadcast_msg);
PACKET_REGEDIT_SEND(packet_regedit_route_msg);

//all <-> logic
PACKET_REGEDIT_RECV_LOG(logic_peer, packet_server_connect);
PACKET_REGEDIT_RECV_LOG(logic_peer, packet_server_connect_result);

PACKET_REGEDIT_RECV_LOG(logic_peer, packet_player_disconnect);

PACKET_REGEDIT_RECV_LOG(logic_peer, packet_gate_setlogic_ok);


//gate->client 
#include <client2gate_protocol.pb.h>
using namespace client2gate_protocols;
PACKET_REGEDIT_SEND(packet_g2c_send_msglist);