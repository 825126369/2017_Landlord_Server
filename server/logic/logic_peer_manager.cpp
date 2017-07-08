#include "stdafx.h"
#include "logic_peer_manager.h"
#include "logic_peer.h"
#include "servers_manager.h"
#include "logic_server.h"

logic_peer_manager::~logic_peer_manager()
{
	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->set_mgr_handler(nullptr);
		++it;
	}
}

void logic_peer_manager::heartbeat( double elapsed )
{
	__ENTER_FUNCTION

	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->heartbeat( elapsed );
		++it;
	}

	exec_event();
	__LEAVE_FUNCTION
}

void logic_peer_manager::exec_event()
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

				remove_obj(peer->get_id());
				servers_manager::instance().add_obj(peer->get_id(), peer);
			}
			break;
		case e_pe_acceptfail:
			{
				SLOG_CRITICAL << "server_acceptfail id:"<<peer->get_id();

				if(remove_obj(peer->get_id()))
					logic_server::instance().push_id(peer->get_id());
			}
			break;
		case e_pe_recved:break;
		default://连接管理应该没有其他事件处理
			SLOG_CRITICAL << "server_event untreated id:"<<peer->get_id() << " type:"<<pe.ep_event;
			break;
		}
	}

}

bool logic_peer_manager::add_obj(int obj_id, boost::shared_ptr<logic_peer> obj)
{
	bool ret = enable_object_manager::add_obj(obj_id, obj);
	if(ret)
		obj->set_mgr_handler(this);
	return ret;
}