#pragma once
#include <net/peer_tcp.h>
#include <server_manager_handler.h>
#include <list>
#include <enable_object_pool.h>

class world_peer:
	public peer_tcp
	,public enable_obj_pool<world_peer>
{
public:
	world_peer();
	virtual ~world_peer(void);

	virtual uint16_t get_type();

	virtual void on_peer_event(e_peer_event eps);

	void heartbeat( double elapsed );

	void set_mgr_handler(server_manager_handler* _handler);

	//only gatepeer
	template<class T>
	void send_msg_to_client(uint32_t sessionid, T msg)
	{
		send_msg_to_client(sessionid, msg->packet_id(), msg);
	};
	int send_msg_to_client(uint32_t sessionid, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);
	template<class T>
	void send_msg_to_client(const std::list<uint32_t>& sessionids, T msg)
	{
		send_msg_to_client(sessionids, msg->packet_id(), msg);
	};
	int send_msg_to_client(const std::list<uint32_t>& sessionids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);

private:
	server_manager_handler* mgr_handler;
};

