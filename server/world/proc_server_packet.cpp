#include "stdafx.h"
#include "proc_server_packet.h"
#include "server_peer.h"
#include "backstage_manager.h"
#include <server_base.pb.h>
#include <enable_json_map.h>
#include <time_helper.h>
#include "timer_manager.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "game_db_log.h"

using namespace boost;

void initServerPacket()
{
	packet_updata_servers_info_factory::regedit_factory();
	packet_server_register_result_factory::regedit_factory();
	packet_other_server_disconnect_factory::regedit_factory();
	packet_other_server_connect_factory::regedit_factory();

	packet_heartbeat_factory::regedit_factory();
	packet_updata_self_info_factory::regedit_factory();
	packet_server_register_factory::regedit_factory();
	packet_get_ip_result_factory::regedit_factory();
	packet_get_ip_factory::regedit_factory();
}

// 更新服务器信息
bool packet_updata_servers_info_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_updata_servers_info> msg)
{	
	__ENTER_FUNCTION_CHECK;

	SINFO_MAP& simap = backstage_manager::instance().SInfoMap;

	auto silist = msg->sinfos();
	for (int i = 0;i<silist.size();i++)
	{
		auto sii = silist.Get(i);
		auto fit = simap.find(sii.server_port());
		if(fit != simap.end())
		{
			if(sii.has_attributes())
				fit->second->mutable_attributes()->CopyFrom(sii.attributes());
		}
		else
		{
			server_info_define sid = make_shared<server_protocols::server_info>();			
			sid->CopyFrom(sii);
			simap.insert(std::make_pair(sid->server_port(), sid));
		}		
	}
	//backstage_manager::instance().check_logic();
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//收到 服务器 注册结果
bool packet_server_register_result_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_server_register_result> msg)
{	
	__ENTER_FUNCTION_CHECK;
	time_helper::instance().set_base_time(msg->server_time());
	peer->set_remote_id(peer->get_remote_port());	

	//同步时间之后才能执行定时器初始化
	timer_manager::instance().init_timer();

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//这里这么做的原因是：及时告知其他服务器，我断开连接了
bool packet_other_server_disconnect_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_other_server_disconnect> msg)
{	
	__ENTER_FUNCTION_CHECK;

	SINFO_MAP& simap = backstage_manager::instance().SInfoMap;
	auto fit = simap.find(msg->server_id());
	if(fit != simap.end())
		simap.erase(fit);
	else
	{
		SLOG_ERROR << "other_server_disconnect error id"<< msg->server_id();
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//这里这么做的原因是：及时告知其他服务器，我连接了
bool packet_other_server_connect_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_other_server_connect> msg)
{	
	__ENTER_FUNCTION_CHECK;

	SINFO_MAP& simap = backstage_manager::instance().SInfoMap;

	auto sii = msg->sinfo();
	auto fit = simap.find(sii.server_port());
	if(fit == simap.end())
	{
		server_info_define sid = make_shared<server_protocols::server_info>();			
		sid->CopyFrom(sii);
		simap.insert(std::make_pair(sid->server_port(), sid));
	}	
	else
	{
		SLOG_ERROR << "other_server_connect error id"<< sii.server_port() << " type:"<<sii.server_type();
		return false;
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


bool packet_get_ip_result_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packet_get_ip_result> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	auto player = game_player_mgr::instance().find_player(msg->sessionid());
	if(player)
	{
		player->LastIP->set_string(msg->ip());
		player->LastPort->set_value(msg->port());
		db_log::instance().player_login_log(player->PlayerId->get_value(), msg->ip(), msg->port(), player->MachineCode, player->MachineType);
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}