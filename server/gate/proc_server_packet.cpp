#include "stdafx.h"
#include "proc_server_packet.h"
#include "server_peer.h"
#include "backstage_manager.h"
#include <server_base.pb.h>
#include <enable_json_map.h>
#include "clients_manager.h"
#include "gate_peer.h"
#include <time_helper.h>
#include "gate_server.h"

using namespace boost;
//收到 Monitor 3s所有的服务器信息
bool packet_updata_servers_info_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_updata_servers_info> msg)
{
	__ENTER_FUNCTION_CHECK;
	//SLOG_CRITICAL << "更新服务器信息";
	SINFO_MAP& simap = backstage_manager::instance().SInfoMap;
	auto silist = msg->sinfos();
	for (int i = 0; i < silist.size(); i++)
	{
		auto sii = silist.Get(i);
		auto fit = simap.find(sii.server_port());
		if (fit != simap.end())
		{
			if (sii.has_attributes())
			{
				fit->second->mutable_attributes()->CopyFrom(sii.attributes());
			}
		}
		else
		{
			server_info_define sid = make_shared<server_protocols::server_info>();
			sid->CopyFrom(sii);
			simap.insert(std::make_pair(sid->server_port(), sid));
		}
	}

	backstage_manager::instance().check_servers();

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//收到 注册Monitor 服务，返回结果
bool packet_server_register_result_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_server_register_result> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//std::cout << "注册服务器返回结果" << peer->get_remote_type()<<std::endl;
	time_helper::instance().set_base_time(msg->server_time());
	peer->set_remote_id(peer->get_remote_port());	
	backstage_manager::instance().regedit_server(peer);
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//收到 其他服务器断开连接
bool packet_other_server_disconnect_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_other_server_disconnect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//std::cout << "其他服务器断开" << std::endl;
	SINFO_MAP& simap = backstage_manager::instance().SInfoMap;
	auto fit = simap.find(msg->server_id());
	if(fit != simap.end())
		simap.erase(fit);

	SLOG_CRITICAL << "other_server_disconnect id:"<< msg->server_id();

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//说到 其他服务器连接
bool packet_other_server_connect_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_other_server_connect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//std::cout << "其他服务器连接" <<peer->get_remote_type()<< std::endl;
	SINFO_MAP& simap = backstage_manager::instance().SInfoMap;

	auto sii = msg->sinfo();
	auto fit = simap.find(sii.server_port());
	if(fit == simap.end())
	{
		server_info_define sid = make_shared<server_protocols::server_info>();			
		sid->CopyFrom(sii);
		simap.insert(std::make_pair(sid->server_port(), sid));
	}
	backstage_manager::instance().check_servers();
	SLOG_CRITICAL << "other_server_connect id:"<< sii.server_port() << " type:"<<sii.server_type();	

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//连接其他服务器，返回结果（除了Monitor）
bool packet_server_connect_result_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_server_connect_result> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//SLOG_CRITICAL << "服务器连接结果"<<msg->server_type()<< std::endl;
	peer->set_remote_id(peer->get_remote_port());
	SLOG_CRITICAL << "packet_server_connect_result ok id:"<<peer->get_remote_id() <<" type:"<<peer->get_remote_type() << " ip:"<< peer->get_remote_ip() <<" port:"<<peer->get_remote_port();

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

//收到其他服务器发给客户端的消息 路由转发协议
bool packet_transmit_msg_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_transmit_msg> msg)
{	
	__ENTER_FUNCTION_CHECK;	
	//std::cout << "转发协议" << std::endl;
	auto client = clients_manager::instance().find_objr(session_helper::get_peerid(msg->sessionid()));
	if(client)
	{
		client->send_msg(msg->msgpak().msgid(), msg->msgpak().msginfo());
		return true;
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//收到广播信息
bool packet_broadcast_msg_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_broadcast_msg> msg)
{
	__ENTER_FUNCTION_CHECK;
	//std::cout << "广播信息" << msg->packet_id() << " | " << msg->msgpak().msgid() << std::endl;

	for (int i = 0; i < msg->sessionids_size(); i++)
	{
		auto client = clients_manager::instance().find_objr(session_helper::get_peerid(msg->sessionids(i)));
		if (client)
		{
			client->send_msg(msg->msgpak().msgid(), msg->msgpak().msginfo());			
		}
	}
	return true;


	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//聊天消息
bool packet_broadcast_msg2_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_broadcast_msg2> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//std::cout << "广播聊天信息" <<msg->packet_id() << " | " << msg->msgpak().msgid() << std::endl;
	auto cmap = clients_manager::instance().get_map();
	for (auto it = cmap.begin(); it != cmap.end(); ++it)
	{
		if(msg->game_sid() == 0 || msg->game_sid()==it->second->logic_id)
		{
			it->second->send_msg(msg->msgpak().msgid(), msg->msgpak().msginfo());	
		}
	}	

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


//玩家连接
bool packet_player_connect_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_player_connect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//SLOG_CRITICAL<< "玩家连接" << std::endl;
	auto client = clients_manager::instance().find_objr(session_helper::get_peerid(msg->sessionid()));
	if(client)
	{
		client->logic_id = msg->logicid();	
		
		auto serverpeer = backstage_manager::instance().get_server_byid(msg->logicid());
		if(serverpeer != nullptr)
		{
			auto msg2 = PACKET_CREATE(packet_gate_setlogic_ok, e_mst_gate_setlogic_ok);
			msg2->set_sessionid(msg->sessionid());
			serverpeer->send_msg(msg2);
		}
	}	

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//注册 路由信息
bool packet_regedit_route_msg_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_regedit_route_msg> msg)
{	
	__ENTER_FUNCTION_CHECK;
	/*for (auto it = msg->msgids().begin(); it != msg->msgids().end(); ++it)
	{
		clients_manager::instance().regedit_msg((*it), peer->get_remote_type());
	}*/
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

bool packet_player_disconnect_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_player_disconnect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//SLOG_CRITICAL << "玩家断开连接" << std::endl;
	auto client = clients_manager::instance().find_objr(session_helper::get_peerid(msg->sessionid()));
	if(client != nullptr)
	{
		client->logic_id = 0;
		client->discannect();
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

bool packet_clear_session_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_clear_session> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//SLOG_CRITICAL << "清除会话信息" << std::endl;
	uint16_t peerid = session_helper::get_peerid(msg->sessionid());
	auto client = clients_manager::instance().find_objr(peerid);
	if(client != nullptr)
	{
		SLOG_ERROR << "clear sessionid peerid:"<<peerid<<" sessionid:"<<msg->sessionid();
		client->IsValid = false;
		client->discannect();
	}
	else
	{
		gate_server::instance().push_id(peerid);
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

bool packet_get_ip_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_get_ip> msg)
{	
	__ENTER_FUNCTION_CHECK;
	SLOG_CRITICAL << "获取IP信息" << std::endl;
	uint16_t peerid = session_helper::get_peerid(msg->sessionid());
	auto client = clients_manager::instance().find_objr(peerid);
	if(client != nullptr)
	{
		auto sendmsg = PACKET_CREATE(packet_get_ip_result, e_mst_get_ip_result);	
		sendmsg->set_sessionid(msg->sessionid());
		sendmsg->set_ip(client->get_remote_ip());
		sendmsg->set_port(client->get_remote_port());
		peer->send_msg(sendmsg);
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}