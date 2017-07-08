#include "stdafx.h"
#include "logic_peer.h"
#include <server_base.pb.h>
#include <enable_processinfo.h>
#include "servers_manager.h"
#include <net/packet_manager.h>
#include <server_protocol.pb.h>

using namespace server_protocols;

logic_peer::logic_peer():
	mgr_handler(nullptr)
{
}


logic_peer::~logic_peer(void)
{
	mgr_handler = nullptr;
}

uint16_t logic_peer::get_type()
{
	return (uint16_t)server_protocols::e_st_logic;
}

void logic_peer::on_peer_event(e_peer_event eps)
{
	__ENTER_FUNCTION

		if(mgr_handler)
			mgr_handler->push_event(get_id(), eps);

	__LEAVE_FUNCTION
}

using namespace server_protocols;

void logic_peer::heartbeat( double elapsed )
{
	__ENTER_FUNCTION

		int packet_id = packet_service();
	if(packet_id != 0)
	{
		SLOG_ERROR << "monitor_peer packet_service error id:"<<get_id() << " packetid:"<<packet_id;
				
		return;
	}

	__LEAVE_FUNCTION
}

void logic_peer::set_mgr_handler(server_manager_handler* _handler)
{
	mgr_handler = _handler;
}

int logic_peer::send_msg_to_client(uint32_t sessionid, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg)
{
	auto sendmsg = PACKET_CREATE(packet_transmit_msg, e_mst_transmit_msg);
	sendmsg->set_sessionid(sessionid);
	auto packet = sendmsg->mutable_msgpak();
	packet->set_msgid(packet_id);
	msg->SerializeToString(packet->mutable_msginfo());

	return send_msg(sendmsg);
}

int logic_peer::send_msg_to_client(const std::list<uint32_t>& sessionids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg)
{
	auto sendmsg = PACKET_CREATE(packet_broadcast_msg, e_mst_broadcast_msg);

	for (auto it = sessionids.begin();it != sessionids.end(); ++it)
	{
		sendmsg->add_sessionids(*it);
	}

	auto packet = sendmsg->mutable_msgpak();
	packet->set_msgid(packet_id);
	msg->SerializeToString(packet->mutable_msginfo());

	return send_msg(sendmsg);
}