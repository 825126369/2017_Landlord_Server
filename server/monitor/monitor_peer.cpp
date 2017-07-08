#include "stdafx.h"
#include "monitor_peer.h"
#include <server_base.pb.h>
#include <enable_processinfo.h>
#include "servers_manager.h"
#include <net/packet_manager.h>
#include <server_protocol.pb.h>
#include "monitor_server.h"
#include <boost/lexical_cast.hpp>
#include "exception_warn_task.h"

using namespace server_protocols;

monitor_peer::monitor_peer():
	mgr_handler(nullptr)
{
	reset_time();
}

monitor_peer::~monitor_peer(void)
{
	mgr_handler = nullptr;
}

uint16_t monitor_peer::get_type()
{
	return (uint16_t)server_protocols::e_st_monitor;
}

void monitor_peer::on_peer_event(e_peer_event eps)
{
	__ENTER_FUNCTION

		if(mgr_handler)
			mgr_handler->push_event(get_id(), eps);

	__LEAVE_FUNCTION
}

using namespace server_protocols;

void monitor_peer::heartbeat( double elapsed )
{
	__ENTER_FUNCTION

	int packet_id = packet_service();
	if(packet_id != 0)
	{		
		SLOG_ERROR << "monitor_peer packet_service error id:"<<get_id() << " packetid:"<<packet_id;			
	}

	if(check_timeout())
	{
		auto sinfo = servers_manager::instance().get_server_info(get_id());
		if(sinfo != nullptr)
		{
			SLOG_CRITICAL << "monitor_peer timeout id:"<<get_id() << " server_type:"<<sinfo->server_type()<< " server_ip:"<< sinfo->server_ip()<< " server_port:"<< sinfo->server_port();
		}
		else
		{
			SLOG_CRITICAL << "monitor_peer timeout id:"<<get_id() << " can't find server_info!";
		}

		_onServerExceptionProcess();
		reset_time();//每间隔检查1次 不用一直报错
	}

	__LEAVE_FUNCTION
}

void monitor_peer::reset_time()
{
	m_check_time = enable_processinfo::get_tick_count();
}

static const int TIME_OUT_COUNT = 30000;//30s 检查一次心跳
bool monitor_peer::check_timeout()
{
	if(get_state() != e_ps_connected)
		return false;
	
	double el = enable_processinfo::get_tick_count() - m_check_time;
	/*if (get_remote_type() == server_protocols::e_server_type::e_st_world)
	{
		std::cout << "检查时间：" << el << std::endl;
	}*/
	return el > TIME_OUT_COUNT;		
}

void monitor_peer::set_mgr_handler(server_manager_handler* _handler)
{
	mgr_handler = _handler;
}

void monitor_peer::on_recv_data(uint32_t len)
{
	
}
void monitor_peer::on_send_data(uint32_t len)
{
	
}

void monitor_peer::_onServerExceptionProcess()
{
	try
	{
		auto task = boost::make_shared<ExceptionWarnTask>(monitor_server::instance().get_io_service());
		msg_request mr;
		// 报警页面
		mr.uri = monitor_server::instance().get_cfg().get<std::string>("warnAspx");
		std::string serverId = boost::lexical_cast<std::string>(monitor_server::instance().get_groupid());
		mr.spath = "/ServerCrashWarn.aspx?serverId=" + serverId;
		task->post_request(mr);
	}
	catch(std::exception* ex)
	{
		SLOG_ERROR<< boost::format("向宕机报警页面发送数据时发生异常: %1%") % ex->what();
	}
}
