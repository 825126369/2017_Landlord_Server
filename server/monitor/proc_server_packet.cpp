#include "stdafx.h"
#include "proc_server_packet.h"
#include <server_base.pb.h>
#include "monitor_server.h"
#include "monitor_peer.h"
#include "monitor_peer_manager.h"
#include "servers_manager.h"
#include <time_helper.h>

using namespace boost;

//收到 其他服务器的信息
bool packet_server_register_factory::packet_process(shared_ptr<monitor_peer> peer, shared_ptr<packet_server_register> msg)
{
	__ENTER_FUNCTION_CHECK;
	auto sendmsg = PACKET_CREATE(packet_server_register_result, e_mst_server_register_result);

	peer->set_remote_id(msg->server_port());//端口为id
	peer->set_remote_type(msg->server_type());
	shared_ptr<server_info> sinfo = make_shared<server_info>();

	sinfo->set_server_port(msg->server_port());
	sinfo->set_server_type(msg->server_type());
	sinfo->set_server_ip(peer->get_remote_ip());
	if (msg->has_attributes())
	{
		sinfo->mutable_attributes()->CopyFrom(msg->attributes());
	}

	monitor_peer_manager::instance().push_event(peer->get_id(), e_pe_only_remove);//服务器信息移除,转移

	if (servers_manager::instance().regedit_server(peer, sinfo))//注册完成后，通知其他服务器，你已经向Monitor注册成功
	{
		auto broadmsg = PACKET_CREATE(packet_other_server_connect, e_mst_other_server_connect);
		server_info* pinfo = broadmsg->mutable_sinfo();
		pinfo->CopyFrom(*sinfo);
		servers_manager::instance().broadcast_msg(broadmsg, peer->get_id());
	}
	else
	{
		SLOG_ERROR << "server_regedit error id:" << peer->get_id() << " type:" << sinfo->server_type() << " ip:" << sinfo->server_ip() << " port:" << sinfo->server_port();
		return false;
	}
	sendmsg->set_server_time(time_helper::instance().get_cur_time());
	sendmsg->set_group_id(monitor_server::instance().get_groupid());
	peer->send_msg(sendmsg);
	SLOG_CRITICAL << "server_regedit ok id:" << peer->get_id() << " type:" << sinfo->server_type() << " ip:" << sinfo->server_ip() << " port:" << sinfo->server_port();
	return true;

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//收到 某个服务器 数据有变动
bool packet_updata_self_info_factory::packet_process(shared_ptr<monitor_peer> peer, shared_ptr<packet_updata_self_info> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sinfo = servers_manager::instance().get_server_info(peer->get_id());
	if(sinfo != nullptr)
	{
		sinfo->mutable_attributes()->CopyFrom(msg->attributes());
	}

	return true;
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


//心跳
bool packet_heartbeat_factory::packet_process(shared_ptr<monitor_peer> peer, shared_ptr<packet_heartbeat> msg)
{	
	__ENTER_FUNCTION_CHECK;

	peer->reset_time();	
	return true;
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
