#include "stdafx.h"
#include "monitor_peer_manager.h"
#include "monitor_peer.h"
#include "servers_manager.h"
#include "monitor_server.h"

monitor_peer_manager::~monitor_peer_manager(void)
{
	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->set_mgr_handler(nullptr);
		++it;
	}
}

void monitor_peer_manager::heartbeat(double elapsed)
{
	__ENTER_FUNCTION
		for (auto it = obj_map.begin(); it != obj_map.end(); it++)
		{
			it->second->heartbeat(elapsed);
		}
		exec_event();
	__LEAVE_FUNCTION
}

void monitor_peer_manager::exec_event()
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
		case e_pe_accepted:
			{
				SLOG_CRITICAL << "server_accepted id:"<<peer->get_id();
				peer->reset_time();
				remove_obj(peer->get_id());
				servers_manager::instance().add_obj(peer->get_id(), peer);
			}
			break;
		case e_pe_acceptfail:
			{
				SLOG_CRITICAL << "server_acceptfail id:"<<peer->get_id();
				if (remove_obj(peer->get_id()))
				{
					monitor_server::instance().push_id(peer->get_id());//ID分配器回收服务器 ID
				}
			}
			break;
		case e_pe_only_remove:
			{
				remove_obj(peer->get_id());
			}
			break;
		case e_pe_recved:
			break;
		default:
			SLOG_CRITICAL << "server_event untreated id:"<<peer->get_id() << " type:"<<pe.ep_event;
			break;
		}
	}

}

bool monitor_peer_manager::add_obj(int obj_id, boost::shared_ptr<monitor_peer> obj)
{
	bool ret = enable_object_manager::add_obj(obj_id, obj);
	if(ret)
		obj->set_mgr_handler(this);
	return ret;
}