#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>

class monitor_peer;
class monitor_peer_manager:
	public enable_object_manager<monitor_peer, boost::mutex>,
	public enable_singleton<monitor_peer_manager>,
	public server_manager_handler
{
public:
	monitor_peer_manager(void){};
	virtual ~monitor_peer_manager(void);

	void heartbeat( double elapsed );

	virtual void exec_event();

	virtual bool add_obj(int obj_id, boost::shared_ptr<monitor_peer> obj) override;
};

