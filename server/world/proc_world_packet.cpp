#include "stdafx.h"
#include "proc_world_packet.h"
#include "world_peer.h"
#include "servers_manager.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "command_mgr.h"

using namespace boost;

void initWorldPacket()
{
	packet_server_connect_factory::regedit_factory();
	packet_transmit_msg_factory::regedit_factory();
	packet_player_disconnect_factory::regedit_factory();

	packet_broadcast_msg_factory::regedit_factory();
	packet_regedit_route_msg_factory::regedit_factory();
	packet_broadcast_msg2_factory::regedit_factory();
	packet_clear_session_factory::regedit_factory();
	packet_server_connect_result_factory::regedit_factory();
	packet_player_connect_factory::regedit_factory();
	packet_http_command_factory::regedit_factory();
}

//收到 其他服务器连接
bool packet_server_connect_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packet_server_connect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	peer->set_remote_id(msg->server_id());
	peer->set_remote_type(msg->server_type());
	if(servers_manager::instance().regedit_server(peer))
	{
		auto sendmsg = PACKET_CREATE(packet_server_connect_result, e_mst_server_connect_result);
		sendmsg->set_server_type(server_protocols::e_st_world);
		peer->send_msg( sendmsg);

		SLOG_CRITICAL << "packet_server_connect ok!  id:"<<msg->server_id() <<" type:"<<msg->server_type() << " ip:"<< peer->get_remote_ip() <<" port:"<<peer->get_remote_port();
		/*if(msg->server_type() == e_st_gate)//一旦连上网关服务器，则把需要路由的协议发送给网关服务器
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
			peer->send_msg( sendmsg);
		}*/
	}
	else
	{
		SLOG_CRITICAL << "packet_server_connect fail id:"<<msg->server_id() <<" type:"<<msg->server_type() << " ip:"<< peer->get_remote_ip() <<" port:"<<peer->get_remote_port();
		return false;
	}
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//收到Gate 转发的客户端协议
bool packet_transmit_msg_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packet_transmit_msg> msg)
{
	__ENTER_FUNCTION_CHECK;

	auto msg2 = PACKET_CREATE(google::protobuf::Message, msg->msgpak().msgid());
	bool bret = false;
	if (msg2)
	{
		msg2->ParseFromString(msg->msgpak().msginfo());
		auto factory = packet_manager::instance().get_factroy(msg->msgpak().msgid());

		if (factory->use_sessionid())
		{
			bret = factory->packet_process(peer, msg->sessionid(), msg2);
		}
		else
		{
			auto player = game_player_mgr::instance().find_player(msg->sessionid());
			if (player != nullptr)
			{
				bret = factory->packet_process(peer, player, msg2);
			}
		}
	}

	if (!bret)
	{
		SLOG_CRITICAL << "packet_transmit_msg error id:" << msg->msgpak().msgid() << " serverid" << peer->get_remote_id() << " sessionid:" << msg->msgpak().msgid();
		auto sendmsg = PACKET_CREATE(packet_player_disconnect, e_mst_player_disconnect);
		sendmsg->set_sessionid(msg->sessionid());
		peer->send_msg(sendmsg);
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//gate 通知 world 玩家断开连接
bool packet_player_disconnect_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packet_player_disconnect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	auto p = game_player_mgr::instance().find_player(msg->sessionid());
	if(p != nullptr)
	{		
		if(p->get_sessionid() == msg->sessionid())
		{
			p->player_logout();//这里只做了标记 并未移除数据
		}		
		game_player_mgr::instance().remove_session(msg->sessionid());
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//monitor->world （这个有用吗？）
bool packet_http_command_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packet_http_command> msg)
{	
	__ENTER_FUNCTION_CHECK;
	//这里做命令处理
	for(int i = 0; i < msg->cmdstr_size(); i++)
	{
		auto httpCmd = msg->mutable_cmdstr(i);
		command_mgr::instance().parse_cmd(*httpCmd);
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
