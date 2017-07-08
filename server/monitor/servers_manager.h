#pragma once
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>

class monitor_peer;
namespace server_protocols
{
	class server_info;
};

namespace google
{
	namespace protobuf
	{
		class Message;
	};
};

class servers_manager:
	public enable_object_manager<monitor_peer, boost::shared_mutex>,
	public enable_singleton<servers_manager>,
	public server_manager_handler
{
public:
	servers_manager(void);
	virtual ~servers_manager(void);

	bool regedit_server(boost::shared_ptr<monitor_peer> peer, boost::shared_ptr<server_protocols::server_info> sinfo);

	bool remove_server(uint16_t peer_id);

	boost::shared_ptr<server_protocols::server_info> get_server_info(uint16_t peer_id);

	boost::shared_ptr<monitor_peer> get_server_bytype(uint16_t servertype);

	void heartbeat( double elapsed );

	template<class T>
	void broadcast_msg(T msg, uint16_t except_id = -1)
	{
		broadcast_msg(msg->packet_id(), msg, except_id);
	};
	

	virtual void exec_event();
	virtual bool add_obj(int obj_id, boost::shared_ptr<monitor_peer> obj);
private:

	void broadcast_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint16_t except_id = -1);

	ENABLE_MAP<uint16_t, boost::shared_ptr<server_protocols::server_info>> m_infomap;

	double m_checktime;
	static const int UPDATE_TIME = 3;//s
};

