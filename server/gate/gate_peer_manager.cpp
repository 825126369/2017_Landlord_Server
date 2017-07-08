#include "stdafx.h"
#include "gate_peer_manager.h"
#include "gate_peer.h"
#include "clients_manager.h"
#include "gate_server.h"

gate_peer_manager::~gate_peer_manager()
{
	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->set_mgr_handler(nullptr);
		++it;
	}
}

void gate_peer_manager::heartbeat(double elapsed)
{
	__ENTER_FUNCTION

		for (auto it = obj_map.begin(); it != obj_map.end(); it++)
		{
			it->second->heartbeat(elapsed);
		}

	exec_event();
	__LEAVE_FUNCTION
}

void gate_peer_manager::exec_event()
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
				SLOG_CRITICAL << "client_accepted id:"<<peer->get_id();

				remove_obj(peer->get_id());
				clients_manager::instance().regedit_client(peer);
			}
			break;
		case e_pe_acceptfail:
			{
				SLOG_CRITICAL << "client_acceptfail id:"<<peer->get_id();
				if (remove_obj(peer->get_id()))
				{
					gate_server::instance().push_id(peer->get_id());
				}
			}
			break;
		case e_pe_recved:
			break;
		default://连接管理应该没有其他事件处理
			SLOG_CRITICAL << "server_event untreated id:"<<peer->get_id() << " type:"<<pe.ep_event;
			break;
		}
	}

}

bool gate_peer_manager::add_obj(int obj_id, boost::shared_ptr<gate_peer> obj)
{
	bool ret = enable_object_manager::add_obj(obj_id, obj);
	if(ret)
	{
		obj->set_mgr_handler(this);
		obj->set_route_handler(boost::bind(&clients_manager::route_handler, &clients_manager::instance(), _1,_2,_3));
	}
	return ret;
}