#include "stdafx.h"
#include "proc_logic_packet.h"
#include "logic_peer.h"
#include "backstage_manager.h"
#include "servers_manager.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "logic_server.h"
#include "game_manager.h"

using namespace boost;

void init_logic_protocol()
{
	//proc_init_logic_packet
	packet_transmit_msg_factory::regedit_factory();
	packet_broadcast_msg_factory::regedit_factory();
	packet_regedit_route_msg_factory::regedit_factory();

	packet_server_connect_factory::regedit_factory();
	packet_server_connect_result_factory::regedit_factory();
	packet_gate_setlogic_ok_factory::regedit_factory();
	packet_player_disconnect_factory::regedit_factory();

	//特殊协议
	packet_g2c_send_msglist_factory::regedit_factory();
}

//收到其他服务器连接自己
bool packet_server_connect_factory::packet_process(shared_ptr<logic_peer> peer, shared_ptr<packet_server_connect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	peer->set_remote_id(msg->server_id());
	peer->set_remote_type(msg->server_type());

	if(servers_manager::instance().regedit_server(peer))
	{
		auto sendmsg = PACKET_CREATE(packet_server_connect_result, e_mst_server_connect_result);
		sendmsg->set_server_type(server_protocols::e_st_login);
		peer->send_msg(sendmsg);

		SLOG_CRITICAL << "packet_server_connect ok id:"<<msg->server_id() <<" type:"<<msg->server_type() << " ip:"<< peer->get_remote_ip() <<" port:"<<peer->get_remote_port();

		if(msg->server_type() == e_st_gate)
		{
			auto sendmsg = PACKET_CREATE(packet_regedit_route_msg, e_mst_regedit_route_msg);
			auto msgmap = packet_manager::instance().get_map();
			for (auto it = msgmap.begin(); it != msgmap.end(); ++it)
			{
				if(it->second->is_from_gate())
				{
					sendmsg->add_msgids(it->first);
				}
			}
			SLOG_CRITICAL << "注册路由信息（注册协议信息）:  " << peer->get_remote_type() << " | " << sendmsg->msgids().size()<<" | "<<msgmap.size()<< std::endl;
			peer->send_msg( sendmsg);
		}
	}
	else
	{
		SLOG_CRITICAL << "packet_server_connect fail id:"<<msg->server_id() <<" type:"<<msg->server_type() << " ip:"<< peer->get_remote_ip() <<" port:"<<peer->get_remote_port();
		return false;
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//主动连接其他服务器，返回结果
bool packet_server_connect_result_factory::packet_process(shared_ptr<logic_peer> peer, shared_ptr<packet_server_connect_result> msg)
{	
	__ENTER_FUNCTION_CHECK;
	peer->set_remote_id(peer->get_remote_port());
	if(peer->get_remote_type() == server_protocols::e_st_world)
	{
		backstage_manager::instance().set_world_state();
		logic_server::instance().init_game_engine();
	}

	SLOG_CRITICAL << "packet_server_connect_result ok id:"<<peer->get_remote_id() <<" type:"<<peer->get_remote_type() << " ip:"<< peer->get_remote_ip() <<" port:"<<peer->get_remote_port();

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


//////////////////////////////////////////////////////////////////////////
bool packet_transmit_msg_factory::packet_process(shared_ptr<logic_peer> peer, shared_ptr<packet_transmit_msg> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	auto msg2 = PACKET_CREATE(google::protobuf::Message, msg->msgpak().msgid());
	bool bret = false;
	if(msg2)
	{
		msg2->ParseFromString(msg->msgpak().msginfo());

		auto factory = packet_manager::instance().get_factroy(msg->msgpak().msgid());

		if(factory->use_sessionid())
			bret = factory->packet_process(peer, msg->sessionid(), msg2);
		else
		{
			auto player = game_player_mgr::instance().find_player(msg->sessionid());
			if(player != nullptr && player->get_state() == e_ps_playing)
			{
				bret = factory->packet_process(peer, player, msg2);
			}
		}
	}
	else
	{
		auto gpk = game_manager::instance().get_packet_mgr()->create(msg->msgpak().msgid());
		msg2 = CONVERT_POINT(google::protobuf::Message, gpk);
		if(msg2)
		{
			msg2->ParseFromString(msg->msgpak().msginfo());

			auto factory = game_manager::instance().get_packet_mgr()->get_factroy(msg->msgpak().msgid());			
			auto player = game_player_mgr::instance().find_player(msg->sessionid());
			if(factory!=nullptr&& player != nullptr && player->get_state() == e_ps_playing && player->get_handler() != nullptr)
			{
				bret = factory->packet_process(peer, player, msg2);
			}			
		}
	}

	if(!bret)
	{
		//SLOG_CRITICAL << "packet_transmit_msg error id:"<< msg->msgpak().msgid() << " serverid"<< peer->get_remote_id()
		//	<< " sessionid:"<<msg->msgpak().msgid();
		//协议处理错误就踢玩家掉线
		auto sendmsg = PACKET_CREATE(packet_player_disconnect, e_mst_player_disconnect);
		sendmsg->set_sessionid(msg->sessionid());
		peer->send_msg(sendmsg);
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

#include "game_player.h"
#include "game_player_mgr.h"
bool packet_gate_setlogic_ok_factory::packet_process(shared_ptr<logic_peer> peer, shared_ptr<packet_gate_setlogic_ok> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto player = game_player_mgr::instance().find_player(msg->sessionid());
	if(player != nullptr)
		player->GatePeer = boost::weak_ptr<logic_peer>(peer);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

bool packet_player_disconnect_factory::packet_process(shared_ptr<logic_peer> peer, shared_ptr<packet_player_disconnect> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto player = game_player_mgr::instance().find_player(msg->sessionid());
	if(player != nullptr)
	{
		player->set_state(e_ps_disconnect);
		//game_player_mgr::instance().remove_session(msg->sessionid());
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}