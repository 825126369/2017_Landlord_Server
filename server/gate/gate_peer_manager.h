#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>

class gate_peer;
class gate_peer_manager:
	public enable_object_manager<gate_peer, boost::mutex>,
	public enable_singleton<gate_peer_manager>,
	public server_manager_handler
{
public:
	gate_peer_manager(void){};
	virtual ~gate_peer_manager(void);

	void heartbeat( double elapsed );

	virtual void exec_event();
	virtual bool add_obj(int obj_id, boost::shared_ptr<gate_peer> obj);

};

