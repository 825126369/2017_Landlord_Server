#include "stdafx.h"
#include "backstage_manager.h"
#include "server_peer.h"
#include <server_base.pb.h>
#include "gate_server.h"
#include <net/packet_manager.h>
#include "clients_manager.h"
using namespace boost;


backstage_manager::backstage_manager(void)
{
};
backstage_manager::~backstage_manager(void)
{
	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->set_mgr_handler(nullptr);
		++it;
	}
};

bool backstage_manager::regedit_server(boost::shared_ptr<server_peer> peer)
{
	auto ret = find_objr(peer->get_id());
	if (!ret)
	{
		SLOG_CRITICAL << "注册服务器失败：" << peer->get_remote_type() << std::endl;
		return false;
	}
	auto retb = servers_map.insert(std::make_pair(peer->get_remote_id(), peer));

	//一组服务器 最多只有一个login和一个world

	if(peer->get_remote_type() == server_protocols::e_st_world)
		world_ptr = peer;

	return retb.second;
}

bool backstage_manager::remove_server(boost::shared_ptr<server_peer> peer)
{
	auto it = servers_map.find(peer->get_remote_id());
	if(it != servers_map.end())
	{
		servers_map.erase(it);
	}

	return remove_obj(peer->get_id());
}


boost::shared_ptr<server_peer> backstage_manager::get_server_byid(uint16_t serverid)
{
	auto it = servers_map.find(serverid);
	if(it != servers_map.end())
		return it->second;

	return nullptr;
}

boost::shared_ptr<server_peer> backstage_manager::get_server_bytype(uint16_t servertype)
{
	for (auto it = servers_map.begin(); it != servers_map.end(); ++it)
	{
		if(it->second->get_remote_type() == servertype)
		{
			return it->second;
		}
	}
	return nullptr;
}

void backstage_manager::heartbeat( double elapsed )
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

void backstage_manager::exec_event()
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
		case e_pe_connected:
			{
				SLOG_CRITICAL << "server_connected id:"<<peer->get_id();
				if (peer->get_remote_type() == (uint16_t)server_protocols::e_st_monitor)
				{
					peer->regedit_to_monitor();//向Monitor 注册服务器信息
				}
				else
				{
					peer->regedit_to_server();//注册到其他服务器
				}
			}
			break;
		case e_pe_connectfail:
			{
				SLOG_CRITICAL << "server_connectfail id:"<<peer->get_id();
			}
			break;
		case e_pe_disconnected: 
			{
				SLOG_CRITICAL << "server_disconnected id:"<<peer->get_id();
				if (remove_server(peer))
				{
					gate_server::instance().push_id(peer->get_id());//回收ID
				}

				if(peer->get_remote_type() == server_protocols::e_st_logic)
				{
					clients_manager::instance().serverdown_client(peer->get_remote_id());//
				}
				else if(peer->get_remote_type() == server_protocols::e_st_world)
				{
					clients_manager::instance().serverdown_client();
				}
				else if(peer->get_remote_type() == server_protocols::e_st_monitor)
				{	
					gate_server::instance().close();
				}
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

bool backstage_manager::add_obj(int obj_id, boost::shared_ptr<server_peer> obj)
{
	bool ret = enable_object_manager::add_obj(obj_id, obj);
	if(ret)
		obj->set_mgr_handler(this);
	return ret;
}

void backstage_manager::check_servers()
{
	for (auto it = SInfoMap.begin(); it != SInfoMap.end(); ++it)
	{
		bool needconnect = false;
		server_info_define& sid = it->second;

		if (sid->server_type() == server_protocols::e_st_world)
		{			
			if(world_ptr == nullptr ||world_ptr->get_state() == e_ps_disconnected)
			{
				needconnect = true;
			}
		}
		else if(sid->server_type() == server_protocols::e_st_logic)
		{
			auto peer = get_server_byid(sid->server_port());
			if(peer == nullptr || peer->get_state() == e_ps_disconnected)
			{
				needconnect = true;
			}
		}

		if(needconnect)//这样的话，如果是自己（或不符合的服务器），跳过，就不创建句柄
		{
			auto peer = gate_server::instance().create_peer(sid->server_type());
			peer->set_remote_id(sid->server_port());
			peer->set_remote_type(sid->server_type());
			peer->connect(sid->server_ip(), sid->server_port());//gate 连接 世界服务器，或 逻辑服务器
			regedit_server(CONVERT_POINT(server_peer, peer));

			if (sid->server_type() == server_protocols::e_st_world)
			{
				world_ptr = CONVERT_POINT(server_peer, peer);
			}
		}
	}
}
