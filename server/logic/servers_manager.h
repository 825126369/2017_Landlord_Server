#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>

class logic_peer;
namespace google
{
	namespace protobuf
	{
		class Message;
	};
};

class servers_manager:
	public enable_object_manager<logic_peer, boost::shared_mutex>,
	public enable_singleton<servers_manager>,
	public server_manager_handler
{
public:
	servers_manager(void);
	virtual ~servers_manager(void);

	bool regedit_server(boost::shared_ptr<logic_peer> peer);
	boost::shared_ptr<logic_peer> find_server(uint16_t serverid);
	bool remove_server(boost::shared_ptr<logic_peer> peer);

	void heartbeat( double elapsed );

	template<class T>
	void broadcast_msg(T msg, uint16_t except_id = -1)
	{
		broadcast_msg(msg->packet_id(), msg, except_id);
	};

	template<class T>
	int send_msg_to_client(uint32_t sessionid, T msg)
	{
		return send_msg_to_client(sessionid, msg->packet_id(), msg);
	};

	template<class T>
	int send_msg_to_client(std::list<uint32_t>& sessionids, T msg)
	{
		return send_msg_to_client(sessionids, msg->packet_id(), msg);
	}

	virtual void exec_event();
	virtual bool add_obj(int obj_id, boost::shared_ptr<logic_peer> obj);

	int send_msg_to_client(std::list<uint32_t>& sessionids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);
private:
	void broadcast_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint16_t except_id = -1);

	int send_msg_to_client(uint32_t sessionid, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);
	
	ENABLE_MAP<uint16_t, boost::shared_ptr<logic_peer>> servers_map;	
};

