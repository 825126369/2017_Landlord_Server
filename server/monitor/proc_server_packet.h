#pragma once
#include <net\packet_manager.h>
#include <server_protocol.pb.h>
#include "monitor_peer.h"

using namespace server_protocols;

//all -> monitor
PACKET_REGEDIT_RECV_LOG(monitor_peer, packet_server_register);
PACKET_REGEDIT_RECV_LOG(monitor_peer, packet_heartbeat);
PACKET_REGEDIT_RECV_LOG(monitor_peer, packet_updata_self_info);

//monitor -> all
PACKET_REGEDIT_SEND(packet_server_register_result);
PACKET_REGEDIT_SEND(packet_other_server_connect);
PACKET_REGEDIT_SEND(packet_other_server_disconnect);
PACKET_REGEDIT_SEND(packet_updata_servers_info);
PACKET_REGEDIT_SEND(packet_http_command);



