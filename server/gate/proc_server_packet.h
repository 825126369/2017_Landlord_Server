#pragma once
#include <net\packet_manager.h>
#include <server_protocol.pb.h>
#include "server_peer.h"

using namespace server_protocols;


//monitor -> gate
PACKET_REGEDIT_RECV_LOG(server_peer, packet_server_register_result);
PACKET_REGEDIT_RECV(server_peer, packet_other_server_connect);
PACKET_REGEDIT_RECV(server_peer, packet_other_server_disconnect);
PACKET_REGEDIT_RECV(server_peer, packet_updata_servers_info);

//gate -> monitor
PACKET_REGEDIT_SEND(packet_heartbeat);
PACKET_REGEDIT_SEND(packet_updata_self_info);
PACKET_REGEDIT_SEND(packet_server_register);


//all <-> gate
PACKET_REGEDIT_RECV(server_peer, packet_transmit_msg);
PACKET_REGEDIT_RECV(server_peer, packet_broadcast_msg);
PACKET_REGEDIT_RECV(server_peer, packet_regedit_route_msg);
PACKET_REGEDIT_RECV(server_peer, packet_broadcast_msg2);
PACKET_REGEDIT_RECV(server_peer, packet_get_ip);
PACKET_REGEDIT_SEND(packet_get_ip_result);

//all <-> gate
PACKET_REGEDIT_SEND(packet_server_connect);
PACKET_REGEDIT_RECV_LOG(server_peer, packet_server_connect_result);

PACKET_REGEDIT_RECV_LOG(server_peer, packet_player_connect);
PACKET_REGEDIT_RECV_LOG(server_peer, packet_player_disconnect);

PACKET_REGEDIT_SEND(packet_gate_setlogic_ok);

PACKET_REGEDIT_RECV_LOG(server_peer, packet_clear_session);
