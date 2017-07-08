#include "stdafx.h"
#include "gate_peer.h"
#include <server_base.pb.h>
#include <enable_processinfo.h>
#include "clients_manager.h"
#include <net/packet_manager.h>
#include "gate_server.h"

using namespace server_protocols;

gate_peer::gate_peer():
mgr_handler(nullptr)
	,m_checktime(0)
	,m_isok(false)
	,logic_id(0)
	,IsValid(false)
{
	set_check_time();
}


gate_peer::~gate_peer(void)
{
	mgr_handler = nullptr;
}

uint16_t gate_peer::get_type()
{
	return (uint16_t)server_protocols::e_st_gate;
}

void gate_peer::on_peer_event(e_peer_event eps)
{
	__ENTER_FUNCTION

		if(mgr_handler)
			mgr_handler->push_event(get_id(), eps);

	__LEAVE_FUNCTION
}

void gate_peer::heartbeat( double elapsed )
{
	__ENTER_FUNCTION

	int packet_id = packet_service(16);//每帧执行16个包
	if(packet_id != 0)
	{
		SLOG_ERROR << "gate_peer packet_service error id:"<<get_id() << " packetid:"<<packet_id;
		discannect();//只有gate协议错误了才断开连接
		return;
	}

	if(get_state() == e_ps_accepting)
		return;

	m_checktime += elapsed;

	static bool IsDebug = gate_server::instance().get_cfg().get<bool>("debug");

	if(!IsDebug &&m_checktime > 60)//秒
	{
		SLOG_ERROR << "gate_peer check timeout id:"<<get_id()<<std::endl;
		if (!IsValid)
		{
			SLOG_ERROR << "please send info to logic or world,or disconnect:" << get_id()<<std::endl;
		}
		discannect();//超时没有消息断开
		m_checktime = 0;
		return;
	}

	__LEAVE_FUNCTION
}

void gate_peer::set_mgr_handler(server_manager_handler* _handler)
{
	mgr_handler = _handler;
}

void gate_peer::reset_checktime()
{
	if(IsValid)
		m_checktime = 0;
}

bool gate_peer::is_ok()
{
	return m_isok;
}
void gate_peer::set_ok()
{
	m_isok = true;
}