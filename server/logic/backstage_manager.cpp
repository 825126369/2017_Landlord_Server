#include "stdafx.h"
#include "backstage_manager.h"
#include "server_peer.h"
#include <server_base.pb.h>
#include "logic_server.h"
#include <net/packet_manager.h>
#include "game_player_mgr.h"

using namespace boost;


backstage_manager::backstage_manager(void)
{
	is_vaild =false;
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

boost::shared_ptr<server_peer> backstage_manager::get_world()
{
	if(is_vaild)
		return world_ptr;

	return nullptr;
}

void backstage_manager::set_world_state(bool vaild)
{
	is_vaild = vaild;
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

				if(peer->get_remote_type() == (uint16_t)server_protocols::e_st_monitor)
					peer->regedit_to_monitor();
				else if(peer->get_remote_type() == (uint16_t)server_protocols::e_st_world)
					peer->regedit_to_world();
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
				if(peer->get_remote_type() == (uint16_t)server_protocols::e_st_world)
				{
					set_world_state(false);
					//world断开需要做退出等逻辑处理
					if(!game_player_mgr::instance().is_closing())
						game_player_mgr::instance().serverdown();
				}
				if(peer->get_remote_type() == server_protocols::e_st_monitor)
				{//如果是monitor关闭 进入关服模式
					game_player_mgr::instance().serverdown(true);
				}
			}
			break;
		case e_pe_recved:break;
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

void backstage_manager::connect_world()
{
	if (world_ptr != nullptr && world_ptr->get_state() != e_ps_disconnected)
	{
		return;
	}

	for (auto it = SInfoMap.begin(); it != SInfoMap.end(); ++it)
	{
		if (it->second->server_type() == server_protocols::e_st_world)
		{
			if (world_ptr == nullptr)
			{
				auto peer = logic_server::instance().create_peer(server_protocols::e_st_world);
				world_ptr = CONVERT_POINT(server_peer, peer);
			}
			world_ptr->connect(it->second->server_ip(), it->second->server_port());
			break;
		}
	}
}