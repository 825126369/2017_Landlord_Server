#pragma once
#include <net\packet_manager.h>
#include <server_protocol.pb.h>
#include "server_peer.h"

using namespace server_protocols;

void initServerPacket();

//monitor -> world
PACKET_REGEDIT_RECV_LOG(server_peer, packet_server_register_result);
PACKET_REGEDIT_RECV(server_peer, packet_other_server_connect);
PACKET_REGEDIT_RECV(server_peer, packet_other_server_disconnect);
PACKET_REGEDIT_RECV(server_peer, packet_updata_servers_info);
PACKET_REGEDIT_RECV(server_peer, packet_get_ip_result);
PACKET_REGEDIT_SEND(packet_get_ip);

//world -> monitor
PACKET_REGEDIT_SEND(packet_heartbeat);
PACKET_REGEDIT_SEND(packet_updata_self_info);
PACKET_REGEDIT_SEND(packet_server_register);




