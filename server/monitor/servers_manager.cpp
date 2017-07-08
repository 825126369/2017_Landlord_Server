#include "stdafx.h"
#include "servers_manager.h"
#include "monitor_peer.h"
#include "monitor_server.h"
#include <enable_processinfo.h>
#include <net/packet_manager.h>
#include <server_base.pb.h>
#include <server_protocol.pb.h>

using namespace boost;
using namespace server_protocols;

servers_manager::servers_manager(void)
	:m_checktime(0)
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

bool servers_manager::regedit_server(boost::shared_ptr<monitor_peer> peer, boost::shared_ptr<server_info> sinfo)
{
	auto ret = find_objr(peer->get_id());
	if(!ret) return false;
	
	for (auto it = m_infomap.begin();it != m_infomap.end();++it)
	{
		if(it->second->server_port() == sinfo->server_port())
			return false;
	}

	m_infomap.insert(std::make_pair(peer->get_id(), sinfo));	
	return true;
}

bool servers_manager::remove_server(uint16_t peer_id)
{
	bool ret = remove_obj(peer_id);
	auto it = m_infomap.find(peer_id);
	if(it != m_infomap.end())
		m_infomap.erase(it);

	return ret;
}

boost::shared_ptr<server_info> servers_manager::get_server_info(uint16_t peer_id)
{
	auto it = m_infomap.find(peer_id);
	if(it != m_infomap.end())
		return it->second;
	return nullptr;
}

void servers_manager::heartbeat(double elapsed)
{
	__ENTER_FUNCTION;

	if(m_checktime>UPDATE_TIME)//更新服务器信息列表到其他服务器
	{
		if (!m_infomap.empty())
		{
			auto sendmsg = PACKET_CREATE(packet_updata_servers_info, e_mst_updata_servers_info);
			sendmsg->mutable_sinfos()->Reserve(m_infomap.size());

			for (auto iit = m_infomap.begin(); iit != m_infomap.end(); iit++)
			{
				server_info* pinfo = sendmsg->add_sinfos();
				pinfo->CopyFrom(*(iit->second));
			}
			broadcast_msg(sendmsg);
		}
		m_checktime = 0;
	}
	else
	{
		m_checktime += elapsed;
	}

	for(auto it = obj_map.begin(); it != obj_map.end();it++)
	{
		it->second->heartbeat( elapsed );
	}

	exec_event();

	__LEAVE_FUNCTION
}

void servers_manager::broadcast_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint16_t except_id)
{
	for (auto it = obj_map.begin(); it != obj_map.end(); it++)
	{
		if (it->second->get_id() != except_id)
		{
			it->second->send_msg(packet_id, msg);
		}
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
				if(remove_server(peer->get_id()))
					monitor_server::instance().push_id(peer->get_id());

				auto sendmsg = PACKET_CREATE(packet_other_server_disconnect,e_mst_other_server_disconnect);
				__ASSERT((sendmsg != nullptr), "packet_other_server_disconnect is null!");
				sendmsg->set_server_id(peer->get_remote_id());				
				broadcast_msg(sendmsg);

			}
			break;
		case e_pe_recved:
			peer->reset_time();
			break;
		default://连接管理应该没有其他事件处理
			SLOG_CRITICAL << "server_event untreated id:"<<peer->get_id() << " type:"<<pe.ep_event;
			break;
		}
	}

}

bool servers_manager::add_obj(int obj_id, boost::shared_ptr<monitor_peer> obj)
{
	bool ret = enable_object_manager::add_obj(obj_id, obj);
	if(ret)
		obj->set_mgr_handler(this);
	return ret;
}
		

boost::shared_ptr<monitor_peer> servers_manager::get_server_bytype(uint16_t servertype)
{
	for (auto it = obj_map.begin(); it != obj_map.end(); ++it)
	{
		if(it->second->get_remote_type() == servertype)
		{
			return it->second;
		}
	}
	return monitor_peer::EmptyPtr;
}
