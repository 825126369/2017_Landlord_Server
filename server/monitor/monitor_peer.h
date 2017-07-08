#pragma once
#include <net/peer_tcp.h>
#include <server_manager_handler.h>
#include <enable_object_pool.h>

class monitor_peer:
	public peer_tcp
	,public enable_obj_pool<monitor_peer>
{
public:
	monitor_peer();
	virtual ~monitor_peer(void);

	virtual uint16_t get_type();

	virtual void on_peer_event(e_peer_event eps) override;

	void heartbeat( double elapsed );

	void reset_time();

	virtual void on_recv_data(uint32_t len);	
	virtual void on_send_data(uint32_t len);	

	void set_mgr_handler(server_manager_handler* _handler);
private:
	// 任何服务器超过30秒，没响应，向页面发数据报警
	void _onServerExceptionProcess();
private:
	server_manager_handler* mgr_handler;
	double m_check_time;

	bool check_timeout();
};

