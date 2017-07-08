#include "stdafx.h"
#include "clients_manager.h"
#include "gate_peer.h"
#include "gate_server.h"
#include "proc_server_packet.h"
#include <enable_json_map.h>
#include <boost/lexical_cast.hpp>
#include "backstage_manager.h"
#include <client2logic_msg_type.pb.h>
#include <client2world_msg_type.pb.h>
#include "proc_server_packet.h"

using namespace boost;

clients_manager::clients_manager(void)
	:m_updatetime(0),m_oldcount(0)
{
};
clients_manager::~clients_manager(void)
{
	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->set_mgr_handler(nullptr);
		++it;
	}
};

bool clients_manager::regedit_client(boost::shared_ptr<gate_peer> peer)
{
	return add_obj(peer->get_id(), peer);
}

bool clients_manager::remove_client(uint16_t peer_id)
{
	return remove_obj(peer_id);
}

void clients_manager::heartbeat( double elapsed )
{
	__ENTER_FUNCTION;

	auto it = obj_map.begin();
	while (it != obj_map.end())
	{
		it->second->heartbeat( elapsed );
		++it;
	}

	exec_event();
	updateinfo(elapsed);

	__LEAVE_FUNCTION
}

void clients_manager::broadcast_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, const std::vector<uint16_t>& peerid_list)
{	
	for (int i = 0 ; i<peerid_list.size();i++)
	{
		auto it = obj_map.find(peerid_list[i]);
		if(it != obj_map.end())
		{
			it->second->send_msg(packet_id, msg);
		}
	}
}

void clients_manager::exec_event()
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
				SLOG_CRITICAL << "client_disconnect id:"<<peer->get_id();
				remove_client(peer->get_id());
				if(!peer->IsValid)
					gate_server::instance().push_id(peer->get_id());

				//玩家断开连接需要通知world
				auto sendmsg = PACKET_CREATE(packet_player_disconnect, e_mst_player_disconnect);
				sendmsg->set_sessionid(session_helper::get_sessionid(gate_server::instance().get_serverid(), peer->get_id()));
				if (backstage_manager::instance().world_ptr != nullptr)
				{
					backstage_manager::instance().world_ptr->send_msg(sendmsg);
				}
			}
			break;
		case e_pe_recved:
			break;
		default:
			SLOG_CRITICAL << "client_event untreated id:"<<peer->get_id() << " type:"<<pe.ep_event;
			break;
		}
	}

}

bool clients_manager::add_obj(int obj_id, boost::shared_ptr<gate_peer> obj)
{
	bool ret = enable_object_manager::add_obj(obj_id, obj);
	if(ret)
		obj->set_mgr_handler(this);
	return ret;
}



void clients_manager::updateinfo(double elapsed )
{	
	m_updatetime+=elapsed;
	if(m_updatetime > 10 && m_oldcount!= get_count())
	{
		m_oldcount = get_count();
		auto peer = backstage_manager::instance().get_server_bytype(e_st_monitor);
		if(peer)
		{
			auto msg = PACKET_CREATE(packet_updata_self_info, e_mst_updata_self_info);
			msg->mutable_attributes()->set_client_count(m_oldcount);
			peer->send_msg( msg);
		}
		m_updatetime = 0;
	}
	
}

//xk 修改：这里是多余的东西，所以全部注销
/*bool clients_manager::regedit_msg(uint32_t msgid, uint16_t servertype)
{
	auto it=  m_msg_route.find(msgid);
	if (it != m_msg_route.end())
		SLOG_CRITICAL<<"Regedit"<<msgid<<": is Repeated !!!"<<std::endl;
		return false;

	m_msg_route.insert(std::make_pair(msgid, servertype));
	return true;
}

uint16_t clients_manager::route_msg(uint32_t msgid)
{
	auto it=  m_msg_route.find(msgid);
	if(it == m_msg_route.end())
		return 0;

	return it->second;
	return 0;
}
*/

//接受到的客户端信息路由转发到逻辑服务器或世界服务器
bool clients_manager::route_handler(boost::shared_ptr<peer_tcp> peer, uint32_t msgid, const std::string& msgbuff)
{
	auto client = CONVERT_POINT(gate_peer ,peer);
	if(!client->is_ok())
	{
		SLOG_ERROR << "route_handler not_ok error packetid:"<<msgid<<" peerid:"<<peer->get_id();
		return false;
	}

	auto msg2 = PACKET_CREATE(packet_transmit_msg, e_mst_transmit_msg);
	msg2->set_sessionid(session_helper::get_sessionid(gate_server::instance().get_serverid(), peer->get_id()));
	auto pak = msg2->mutable_msgpak();
	pak->set_msgid(msgid);
	pak->set_msginfo(msgbuff);

	boost::shared_ptr<peer_tcp> serverpeer;	
	//接收分发处理
	if(msgid >client2logic_protocols::e_mst_start_c2l && msgid<client2logic_protocols::e_mst_start_l2c)//logic
	{		
		serverpeer =  backstage_manager::instance().get_server_byid(client->logic_id);		
	}
	else if(msgid >client2world_protocols::e_mst_start_c2w && msgid<client2world_protocols::e_mst_start_w2c)//world
	{
		serverpeer = backstage_manager::instance().world_ptr;
		client->IsValid =true;
	}
	else//gate其他协议不转发
	{
		SLOG_ERROR << "route_handler type error packetid:"<<msgid<<" peerid:"<<peer->get_id();
		return false;
	}

	if(serverpeer)
	{
		client->reset_checktime();//重置 检查时间
		serverpeer->send_msg(msg2);
	}
	else
	{
		SLOG_ERROR << "route_handler  serverpeer is null!  packetid:"<<msgid <<" peerid:"<<peer->get_id();
		return false;
	}

	return true;
}

void clients_manager::serverdown_client(uint16_t logicid)
{

	for (auto it = obj_map.begin();it != obj_map.end();++it)
	{
		if( logicid==0 ||it->second->logic_id == logicid )
			it->second->discannect();		
	}
}