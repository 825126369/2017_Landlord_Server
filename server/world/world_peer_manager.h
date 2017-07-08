#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>

class world_peer;
class world_peer_manager:
	public enable_object_manager<world_peer, boost::mutex>,
	public enable_singleton<world_peer_manager>,
	public server_manager_handler
{
public:
	world_peer_manager(void){};
	virtual ~world_peer_manager(void);

	void heartbeat( double elapsed );

	virtual void exec_event();
	virtual bool add_obj(int obj_id, boost::shared_ptr<world_peer> obj);

};

