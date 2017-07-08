#include "stdafx.h"
#include "logic_server.h"
#include "logic_peer.h"
#include "logic_peer_manager.h"
#include "servers_manager.h"
#include "backstage_manager.h"
#include "server_peer.h"
#include <server_base.pb.h>
#include <boost/timer.hpp>
#include "game_player_mgr.h"
#include <task_manager.h>
#include "global_sys_mgr.h"
#include "game_manager.h"
#include "i_game_engine.h"

using namespace boost;

logic_server::logic_server(void)
	:m_engine(nullptr)
{
}


logic_server::~logic_server(void)
{
	
}

bool logic_server::DumpCallback(const wchar_t* dump_path,
						  const wchar_t* minidump_id,
						  void* context,
						  EXCEPTION_POINTERS* exinfo,
						  MDRawAssertionInfo* assertion,
						  bool succeeded)
{
	//异常特殊处理 可以上传到服务器

	return true;
}

bool logic_server::on_init()
{
	//初始化
	init_packet();
	init_config();
	init_db();

	connect_monitor();

	return true;
}



void logic_server::run()
{
	__ENTER_FUNCTION
		timer t;
	double elapsed=0;

	//主循环
	while (is_runing())
	{
		t.restart();
		run_timer();

		logic_peer_manager::instance().heartbeat( elapsed );
		servers_manager::instance().heartbeat( elapsed );
		backstage_manager::instance().heartbeat( elapsed );
		game_player_mgr::instance().heartbeat( elapsed );
		task_manager::instance().update(elapsed);
		game_manager::instance().heartbeat(elapsed);

		if(m_engine != nullptr)
			m_engine->heartbeat(elapsed);

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

void logic_server::on_exit()
{
	global_sys_mgr::instance().sys_exit();

	if(m_engine != nullptr)
	{
		m_engine->exit_engine();
		m_engine = nullptr;
	}

	//退出处理 可做数据保存 dump之后也会走这里
	logic_peer_manager::instance().clear();
	servers_manager::instance().clear();
	backstage_manager::instance().clear();


}

shared_ptr<peer_tcp> logic_server::create_peer()
{
	__ENTER_FUNCTION
	lock_guard<boost::mutex> gurad(m_createlock);	
	shared_ptr<logic_peer> peer = logic_peer::malloc();//make_shared<logic_peer>(m_io_service, generate_id());
	peer->init_peer(m_io_service, generate_id());
	logic_peer_manager::instance().add_obj(peer->get_id(), peer);

	return peer;
	__LEAVE_FUNCTION
		return nullptr;
}

boost::shared_ptr<peer_tcp> logic_server::create_peer(uint16_t remote_type)
{
	__ENTER_FUNCTION
		lock_guard<boost::mutex> gurad(m_createlock);

	shared_ptr<server_peer> peer = server_peer::malloc();//make_shared<server_peer>(m_io_service, generate_id());
	peer->init_peer(m_io_service, generate_id());
	peer->set_remote_type(remote_type);
	backstage_manager::instance().add_obj(peer->get_id(), peer);

	return peer;
	__LEAVE_FUNCTION
		return nullptr;
}

void logic_server::connect_monitor()
{
	__ENTER_FUNCTION
	if(xml_cfg.check("monitor_ip") &&
		xml_cfg.check("monitor_port"))
	{
		auto peer = create_peer(server_protocols::e_st_monitor);
		
		std::string tarip = xml_cfg.get_ex<std::string>("monitor_ip", std::string("localhost"));
		uint16_t tarport = xml_cfg.get<uint16_t>("monitor_port");
		peer->connect(tarip.c_str(), tarport);
	}
	__LEAVE_FUNCTION
}

bool logic_server::init_db()
{
	global_sys_mgr::instance().sys_load();

	return true;
}



void logic_server::init_game_engine()
{
	if(game_manager::instance().open())
	{
		m_engine = game_manager::instance().get_game_engine();
		if(m_engine != nullptr)
		{
			if(!m_engine->init_engine(get_cfg()))
			{
				m_engine->exit_engine();
				m_engine = nullptr;
			}
		}
	}
}