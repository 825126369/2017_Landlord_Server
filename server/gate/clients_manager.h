#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>
#include <enable_hashmap.h>

class gate_peer;
namespace google
{
	namespace protobuf
	{
		class Message;
	};
};

class clients_manager:
	public enable_object_manager<gate_peer, boost::shared_mutex>,
	public enable_singleton<clients_manager>,
	public server_manager_handler
{
public:
	clients_manager(void);
	virtual ~clients_manager(void);

	bool regedit_client(boost::shared_ptr<gate_peer> peer);

	bool remove_client(uint16_t peer_id);
	void serverdown_client(uint16_t logicid = 0);

	void heartbeat( double elapsed );

	template<class T>
	void broadcast_msg(T msg, const std::vector<uint16_t>& peerid_list)
	{
		broadcast_msg(msg->packet_id(), msg, peerid_list);
	};	

	virtual void exec_event();
	virtual bool add_obj(int obj_id, boost::shared_ptr<gate_peer> obj);

	void updateinfo(double elapsed );

	//bool regedit_msg(uint32_t msgid, uint16_t servertype);
	//uint16_t route_msg(uint32_t msgid);
	bool route_handler(boost::shared_ptr<peer_tcp> peer, uint32_t msgid, const std::string& msgbuff);
private:
	//ENABLE_MAP<uint32_t, uint16_t> m_msg_route;
	void broadcast_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, const std::vector<uint16_t>& peerid_list);
	int m_oldcount;
	double m_updatetime;
};

