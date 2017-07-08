#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>

class world_peer;
namespace google
{
	namespace protobuf
	{
		class Message;
	};
};

class servers_manager:
	public enable_object_manager<world_peer, boost::shared_mutex>,
	public enable_singleton<servers_manager>,
	public server_manager_handler
{
public:
	servers_manager(void);
	virtual ~servers_manager(void);

	bool regedit_server(boost::shared_ptr<world_peer> peer);
	boost::shared_ptr<world_peer> find_server(uint16_t serverid);
	bool remove_server(boost::shared_ptr<world_peer> peer);

	void heartbeat( double elapsed );


	template<class T>
	void broadcast_msg(T msg, uint16_t except_id = -1)
	{
		broadcast_msg(msg->packet_id(), msg, except_id);
	};

	template<class T>
	void broadcast_msg2(T msg, uint16_t game_sid = 0)
	{
		broadcast_msg2(msg->packet_id(), msg, game_sid);
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
	virtual bool add_obj(int obj_id, boost::shared_ptr<world_peer> obj);


private:

	void broadcast_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint16_t except_id = -1);
	void broadcast_msg2(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint16_t game_sid = -1);

	int send_msg_to_client(uint32_t sessionid, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);
	int send_msg_to_client(std::list<uint32_t>& sessionids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);

	ENABLE_MAP<uint16_t, boost::shared_ptr<world_peer>> servers_map;
};

