#include "stdafx.h"
#include "gate_server.h"
#include "proc_server_packet.h"
#include "proc_gate_packet.h"

void gate_server::init_packet()
{
	__ENTER_FUNCTION;
	//proc_init_server_packet
	packet_server_register_result_factory::regedit_factory();
	packet_other_server_connect_factory::regedit_factory();
	packet_other_server_disconnect_factory::regedit_factory();
	packet_updata_servers_info_factory::regedit_factory();

	packet_server_register_factory::regedit_factory();
	packet_heartbeat_factory::regedit_factory();
	packet_updata_self_info_factory::regedit_factory();


	//proc_init_gate_packet
	packet_transmit_msg_factory::regedit_factory();
	packet_broadcast_msg_factory::regedit_factory();
	packet_broadcast_msg2_factory::regedit_factory();
	packet_regedit_route_msg_factory::regedit_factory();

	packet_server_connect_factory::regedit_factory();
	packet_server_connect_result_factory::regedit_factory();

	packet_player_disconnect_factory::regedit_factory();
	packet_player_connect_factory::regedit_factory();

	packet_gate_setlogic_ok_factory::regedit_factory();
	packet_clear_session_factory::regedit_factory();

	//init client2gate
	packetc2g_heartbeat_factory::regedit_factory();
	packetc2g_select_gate_factory::regedit_factory();
	packetg2c_select_gate_result_factory::regedit_factory();

	packet_get_ip_factory::regedit_factory();
	packet_get_ip_result_factory::regedit_factory();

	__LEAVE_FUNCTION
}