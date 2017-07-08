#pragma once
#include <net/peer_tcp.h>
#include <server_manager_handler.h>
#include <enable_object_pool.h>

class server_peer:
	public peer_tcp
		,public enable_obj_pool<server_peer>
{
public:
	server_peer();
	virtual ~server_peer(void);

	virtual uint16_t get_type();

	virtual void on_peer_event(e_peer_event eps);

	void heartbeat( double elapsed );

	void set_mgr_handler(server_manager_handler* _handler);

	void check_state( double elapsed );

	void regedit_to_monitor();
	void regedit_to_server();
private:
	server_manager_handler* mgr_handler;
	double m_checktime;
};

