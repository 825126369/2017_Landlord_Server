#pragma once
#include <net\packet_manager.h>
#include "gate_peer.h"

#include <client2gate_protocol.pb.h>

using namespace client2gate_protocols;

PACKET_REGEDIT_RECV(gate_peer, packetc2g_heartbeat);

PACKET_REGEDIT_RECV_LOG(gate_peer, packetc2g_select_gate);
PACKET_REGEDIT_SEND(packetg2c_select_gate_result);
