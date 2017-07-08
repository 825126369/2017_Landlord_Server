#include "stdafx.h"
#include "world_server.h"
#include "world_peer.h"
#include "world_peer_manager.h"
#include "servers_manager.h"
#include "backstage_manager.h"
#include "server_peer.h"
#include <server_base.pb.h>
#include <boost/timer.hpp>
#include <net/peer_http_mgr.h>
#include "game_player_mgr.h"
#include "global_sys_mgr.h"
#include "game_db_log.h"
using namespace boost;

world_server::world_server(void)
{
}


world_server::~world_server(void)
{
	
}

bool world_server::DumpCallback(const wchar_t* dump_path,
						  const wchar_t* minidump_id,
						  void* context,
						  EXCEPTION_POINTERS* exinfo,
						  MDRawAssertionInfo* assertion,
						  bool succeeded)
{
	//异常特殊处理 可以上传到服务器
	return true;
}

bool world_server::on_init()
{
	init_config();
	init_packet();
	init_db();
	connect_monitor();

	return true;
}

void world_server::run()
{
	__ENTER_FUNCTION
		timer t;
	 double elapsed=0;

	//主循环
	while (is_runing())
	{
		t.restart();
		run_timer();
		world_peer_manager::instance().heartbeat( elapsed );
		servers_manager::instance().heartbeat( elapsed );
		backstage_manager::instance().heartbeat( elapsed );

		peer_http_mgr::instance().update( elapsed );

		game_player_mgr::instance().heartbeat(elapsed);
		global_sys_mgr::instance().sys_update(elapsed);

		elapsed = t.elapsed();
		if(elapsed < 1)
		{
			this_thread::sleep(posix_time::milliseconds(1));
			elapsed = t.elapsed();
		}		
		else 
		{
			SLOG_CRITICAL<< "server_run longtime:"<<elapsed;
		}
	}

	__LEAVE_FUNCTION
}

void world_server::on_exit()
{
	global_sys_mgr::instance().sys_exit();
	world_peer_manager::instance().clear();
	servers_manager::instance().clear();
	backstage_manager::instance().clear();
	game_player_mgr::instance().clear();
	db_log::instance().close();
}

shared_ptr<peer_tcp> world_server::create_peer()
{
	__ENTER_FUNCTION
	lock_guard<boost::mutex> gurad(m_createlock);	
	shared_ptr<world_peer> peer = world_peer::malloc();
	peer->init_peer(m_io_service, generate_id());
	world_peer_manager::instance().add_obj(peer->get_id(), peer);

	return peer;
	__LEAVE_FUNCTION
		return nullptr;
}

boost::shared_ptr<peer_tcp> world_server::create_peer(uint16_t remote_type)
{
	__ENTER_FUNCTION
		lock_guard<boost::mutex> gurad(m_createlock);

	shared_ptr<server_peer> peer = server_peer::malloc();
	peer->init_peer(m_io_service, generate_id());
	peer->set_remote_type(remote_type);
	backstage_manager::instance().add_obj(peer->get_id(), peer);

	return peer;
	__LEAVE_FUNCTION
		return nullptr;
}

void world_server::connect_monitor()
{
	__ENTER_FUNCTION
	if(xml_cfg.check("monitor_ip") && xml_cfg.check("monitor_port"))
	{
		auto peer = create_peer(server_protocols::e_st_monitor);	
		std::string tarip = xml_cfg.get_ex<std::string>("monitor_ip", std::string("localhost"));
		uint16_t tarport = xml_cfg.get<uint16_t>("monitor_port");
		peer->connect(tarip, tarport);
	}
	__LEAVE_FUNCTION
}

#include "game_db.h"
#include "global_sys_mgr.h"
void world_server::init_db()
{
	if(xml_cfg.check("db_crypto"))
	{
		if(xml_cfg.get<int>("db_crypto") == 1)
		{
			db_player::instance().set_userpwd(xml_cfg.get<std::string>("db_user"), xml_cfg.get<std::string>("db_pwd"));
			db_log::instance().set_userpwd(xml_cfg.get<std::string>("db_user"), xml_cfg.get<std::string>("db_pwd"));
			db_game::instance().set_userpwd(xml_cfg.get<std::string>("db_user"), xml_cfg.get<std::string>("db_pwd"));
		}
		else
		{
			db_player::instance().set_userpwd(xml_cfg.get<std::string>("db_user"), xml_cfg.get<std::string>("db_pwd"), false);
			db_log::instance().set_userpwd(xml_cfg.get<std::string>("db_user"), xml_cfg.get<std::string>("db_pwd"), false);
			db_game::instance().set_userpwd(xml_cfg.get<std::string>("db_user"), xml_cfg.get<std::string>("db_pwd"), false);
		}
	}

	if(xml_cfg.check("playerdb_url") && xml_cfg.check("playerdb_name"))
	{
		db_player::instance().init_db(xml_cfg.get<std::string>("playerdb_url"), xml_cfg.get<std::string>("playerdb_name"));
	}

	if(xml_cfg.check("logdb_url") && xml_cfg.check("logdb_name"))
	{
		db_log::instance().init_db(xml_cfg.get<std::string>("logdb_url"), xml_cfg.get<std::string>("logdb_name"));
	}
	if(xml_cfg.check("gamedb_url") && xml_cfg.check("gamedb_name"))
	{
		db_game::instance().init_db(xml_cfg.get<std::string>("gamedb_url"), xml_cfg.get<std::string>("gamedb_name"));
	}

	//db初始化之后才能加载系统
	global_sys_mgr::instance().sys_load();
}