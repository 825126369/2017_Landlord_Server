#include "stdafx.h"
#include "backstage_manager.h"
#include "server_peer.h"
#include <server_base.pb.h>
#include <boost/lexical_cast.hpp>
#include "game_player_mgr.h"
using namespace boost;


backstage_manager::backstage_manager(void)
{
	//m_cur_logicid(0)
};
backstage_manager::~backstage_manager(void)
{
	for (auto it = obj_map.begin(); it != obj_map.end(); it++)
	{
		it->second->set_mgr_handler(nullptr);
	}
};

void backstage_manager::heartbeat(double elapsed)
{
	__ENTER_FUNCTION;

	for (auto it = obj_map.begin(); it != obj_map.end(); it++)
	{
		it->second->heartbeat(elapsed);
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
				if (peer->get_remote_type() == server_protocols::e_st_monitor)
				{
					peer->regedit_to_monitor();//发送注册协议
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

				if(peer->get_remote_type() == server_protocols::e_st_monitor)
				{
					game_player_mgr::instance().set_close_state();//如果是monitor关闭 进入关服模式
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

/*void backstage_manager::check_logic()
{
	int temp = 0;
	for(auto it = SInfoMap.begin();it != SInfoMap.end();it++)
	{
		server_info_define& sid = it->second;
		if (sid->server_type() == server_protocols::e_st_logic)
		{
			int count = sid->mutable_attributes()->client_count();
			if(count <= temp)
			{
				temp = count;
				m_cur_logicid = sid->server_port();//port = serverid;
			}			
		}
	}
}

uint16_t backstage_manager::get_logicid()
{
	return m_cur_logicid;
}*/