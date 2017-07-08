#include "stdafx.h"
#include "monitor_server.h"
#include "proc_server_packet.h"

void monitor_server::init_packet()
{
	__ENTER_FUNCTION;
	packet_server_register_factory::regedit_factory();
	packet_heartbeat_factory::regedit_factory();
	packet_updata_self_info_factory::regedit_factory();

	packet_server_register_result_factory::regedit_factory();
	packet_other_server_connect_factory::regedit_factory();
	packet_other_server_disconnect_factory::regedit_factory();
	packet_updata_servers_info_factory::regedit_factory();
	packet_http_command_factory::regedit_factory();

	__LEAVE_FUNCTION
}