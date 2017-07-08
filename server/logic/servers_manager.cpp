#include "stdafx.h"
#include "servers_manager.h"
#include "logic_peer.h"
#include "logic_server.h"

using namespace boost;

servers_manager::servers_manager(void)
{
};
servers_manager::~servers_manager(void)
{
	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->set_mgr_handler(nullptr);
		++it;
	}
};

bool servers_manager::regedit_server(boost::shared_ptr<logic_peer> peer)
{
	auto ret = find_objr(peer->get_id());
	if(!ret) return false;

	auto it = servers_map.find(peer->get_remote_id());
	if(it == servers_map.end())
	{
		auto ret = servers_map.insert(std::make_pair(peer->get_remote_id(), peer));
		return ret.second;
	}

	return false;
}

bool servers_manager::remove_server(boost::shared_ptr<logic_peer> peer)
{
	auto it = servers_map.find(peer->get_remote_id());
	if(it != servers_map.end())
	{
		servers_map.erase(it);	
	}

	return remove_obj(peer->get_id());
}

boost::shared_ptr<logic_peer> servers_manager::find_server(uint16_t serverid)
{
	auto it = servers_map.find(serverid);
	if(it != servers_map.end())
		return it->second;

	return nullptr;
}

void servers_manager::heartbeat( double elapsed )
{
	__ENTER_FUNCTION;

	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->heartbeat( elapsed );
		++it;
	}

	exec_event();

	__LEAVE_FUNCTION
}

void servers_manager::broadcast_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint16_t except_id)
{
	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		if(it->second->get_id() != except_id)
		{
			it->second->send_msg(packet_id, msg);
		}
		
		++it;
	}
}

void servers_manager::exec_event()
{
	auto squeue = get_event_list();

	peer_event pe;
	while (!squeue->empty())
	{		
		if(!squeue->pop(pe))
			break;

		auto peer = find_objr(pe.peer_id);
		if(!peer)continue;

		switch (pe.ep_event)
		{
		case e_pe_disconnected:
			{
				SLOG_CRITICAL << "server_disconnect id:"<<peer->get_id();
				if(remove_server(peer))
					logic_server::instance().push_id(peer->get_id());
			}
			break;
		case e_pe_recved:break;
		default:
			SLOG_CRITICAL << "server_event untreated id:"<<peer->get_id() << " type:"<<pe.ep_event;
			break;
		}
	}

}

bool servers_manager::add_obj(int obj_id, boost::shared_ptr<logic_peer> obj)
{
	bool ret = enable_object_manager::add_obj(obj_id, obj);
	if(ret)
		obj->set_mgr_handler(this);
	return ret;
}

#include "proc_logic_packet.h"
#include <enable_hashmap.h>
int servers_manager::send_msg_to_client(uint32_t sessionid, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg)
{
	uint16_t gateid = session_helper::get_gateid(sessionid);
	auto peer = find_server(gateid);
	if(peer)
	{
		auto sendmsg = PACKET_CREATE(packet_transmit_msg, e_mst_transmit_msg);
		sendmsg->set_sessionid(sessionid);
		auto packet = sendmsg->mutable_msgpak();
		packet->set_msgid(packet_id);
		msg->SerializeToString(packet->mutable_msginfo());

		return peer->send_msg( sendmsg);
	}
	return -1;	
}

int servers_manager::send_msg_to_client(std::list<uint32_t>& sessionids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg)
{
	ENABLE_MAP<uint16_t, std::list<uint32_t>> gatelist;	

	for (auto it = sessionids.begin();it != sessionids.end(); ++it)
	{
		uint16_t gateid = session_helper::get_gateid(*it);
		auto fit = gatelist.find(gateid);
		if(fit != gatelist.end())
		{
			fit->second.push_back(*it);
		}
		else
		{
			std::list<uint32_t> tlist;
			tlist.push_back(*it);
			gatelist.insert(std::make_pair(gateid, tlist));
		}		
	}

	for (auto it = gatelist.begin(); it != gatelist.end(); ++it)
	{
		auto peer = find_server(it->first);
		if(peer)
		{
			auto sendmsg = PACKET_CREATE(packet_broadcast_msg, e_mst_broadcast_msg);
			std::list<uint32_t>& tlist = it->second;
			sendmsg->mutable_sessionids()->Reserve(tlist.size());
			for (auto tit = tlist.begin(); tit != tlist.end(); ++tit)
			{
				sendmsg->add_sessionids(*tit);
			}			

			auto packet = sendmsg->mutable_msgpak();
			packet->set_msgid(packet_id);
			msg->SerializeToString(packet->mutable_msginfo());
			peer->send_msg( sendmsg);
		}
	}

	return 0;
}