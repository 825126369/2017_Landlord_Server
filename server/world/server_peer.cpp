#include "stdafx.h"
#include "server_peer.h"
#include <server_base.pb.h>
#include <enable_processinfo.h>
#include "servers_manager.h"
#include <net/packet_manager.h>
#include <server_protocol.pb.h>
#include "world_server.h"
using namespace server_protocols;

server_peer::server_peer():
	m_checktime(0)
{
}


server_peer::~server_peer(void)
{
	mgr_handler = nullptr;
}

uint16_t server_peer::get_type()
{
	return (uint16_t)server_protocols::e_st_world;
}

void server_peer::on_peer_event(e_peer_event eps)
{
	__ENTER_FUNCTION

		if(mgr_handler)
			mgr_handler->push_event(get_id(), eps);

	__LEAVE_FUNCTION
}

using namespace server_protocols;

void server_peer::heartbeat( double elapsed )
{
	__ENTER_FUNCTION

		int packet_id = packet_service();
	if(packet_id != 0)
	{		
		SLOG_ERROR << "monitor_peer packet_service error id:"<<get_id() << " packetid:"<<packet_id <<" remote_id"<< get_remote_id()<< " remote_type:"<<get_remote_type();			
	}

	check_state(elapsed);
	__LEAVE_FUNCTION
}

void server_peer::set_mgr_handler(server_manager_handler* _handler)
{
	mgr_handler = _handler;
}

static const int CHECK_TIME = 10;

void server_peer::check_state(double elapsed)
{
	__ENTER_FUNCTION
		m_checktime += elapsed;
	if (m_checktime > CHECK_TIME)
	{
		e_peer_state eps = get_state();
		if (eps != e_ps_connected && eps != e_ps_connecting)
		{
			SLOG_CRITICAL << "server_reconnect id:" << get_id() << " remote_id:" << get_remote_id() << " remote_type:" << get_remote_type();
			reconnect();//重连
		}
		else if (get_remote_type() == (uint16_t)server_protocols::e_st_monitor)
		{
			auto sendmsg = PACKET_CREATE(packet_heartbeat, e_mst_heartbeat);
			send_msg(sendmsg);//向monitor发送心跳
		}
		m_checktime = 0;
	}
	__LEAVE_FUNCTION
}

void server_peer::regedit_to_monitor()
{
	auto sendmsg = PACKET_CREATE(packet_server_register, e_mst_server_register);
	sendmsg->set_server_type(server_protocols::e_server_type::e_st_world);
	sendmsg->set_server_port(world_server::instance().get_serverid());
	send_msg(sendmsg);
}