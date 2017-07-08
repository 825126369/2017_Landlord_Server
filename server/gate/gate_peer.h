#pragma once
#include <net/peer_tcp.h>
#include <server_manager_handler.h>
#include <enable_object_pool.h>

class gate_peer:
	public peer_tcp
			,public enable_obj_pool<gate_peer>
{
public:
	gate_peer();
	virtual ~gate_peer(void);

	virtual uint16_t get_type();

	virtual void on_peer_event(e_peer_event eps);

	void heartbeat( double elapsed );

	void set_mgr_handler(server_manager_handler* _handler);

	uint16_t logic_id;

	//接收到协议就重置时间
	void reset_checktime();

	bool is_ok();
	void set_ok();

	bool IsValid;
private:
	server_manager_handler* mgr_handler;
	double m_checktime;
	bool m_isok;
};

