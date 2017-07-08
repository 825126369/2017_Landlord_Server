#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>

class logic_peer;
class logic_peer_manager:
	public enable_object_manager<logic_peer, boost::mutex>,
	public enable_singleton<logic_peer_manager>,
	public server_manager_handler
{
public:
	logic_peer_manager(void){};
	~logic_peer_manager(void);

	void heartbeat( double elapsed );

	virtual void exec_event();
	virtual bool add_obj(int obj_id, boost::shared_ptr<logic_peer> obj);

};

