#pragma once
#include <net/peer_handler.h>
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <enable_smart_ptr.h>
#include <net/msg_queue.h>
#include <boost/enable_shared_from_this.hpp>
#include <boost/atomic.hpp>
#include <net/net_buffer.h>

//class send_buffer;
//class recv_buffer;
class peer_tcp_server;
class packet_head_s;

class peer_tcp: public boost::enable_shared_from_this<peer_tcp>
{
public:
	friend peer_tcp_server;

	peer_tcp();
	virtual ~peer_tcp();

	uint16_t get_id(){return m_id;};

	virtual uint16_t get_type() = 0;
private:
	boost::asio::ip::tcp::socket& socket();
	void handle_accept(const boost::system::error_code& error);
	void handle_connect(const boost::system::error_code& error);
	void handle_write(const boost::system::error_code& error, size_t bytes_transferred);
	void handle_read(const boost::system::error_code& error, size_t bytes_transferred);

	void poet_recv();
	void post_send();
	void close() ;
public :
	void init_peer(boost::asio::io_service& ioservice, uint16_t _id, bool _encrypt = false);
	// close connection	
	void discannect();
	void set_check_time();

	// try connect to remote host
	bool connect (const char* host, uint16_t port) ;
	bool connect (const std::string& host, uint16_t port) ;
	bool connect (const std::string& str_url) ;
	bool connect (boost::asio::ip::tcp::endpoint& server_endpoint);
	bool resolve_hostname(const std::string& hostname, uint16_t port, std::vector<boost::asio::ip::tcp::endpoint>& endpoints);

	// close previous connection and connect to another socket
	bool reconnect (const char* host, uint32_t port) ;
	bool reconnect ();

	template<class T>
	int send_msg(T msg)
	{
		return send_msg(msg->packet_id(), msg) ;
	};	

	// send data to peer
	int send_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg) ;

	int send_msg(uint16_t packet_id, const std::string& msgbuff) ;

	e_peer_state get_state();

	uint16_t get_remote_port ();
	std::string get_remote_ip ();
	uint16_t get_remote_type();
	void set_remote_type(uint16_t _type);
	uint16_t get_remote_id();
	void set_remote_id(uint16_t _id);

	virtual void init_buffer();
	int packet_service(int process_count = -1);

	typedef boost::function<bool(boost::shared_ptr<peer_tcp> peer, uint32_t msgid, const std::string& msgbuff)> GATE_ROUTE_HANDLER;
	void set_route_handler(GATE_ROUTE_HANDLER handler);
protected:	
	void timeout();	
	void clear_buffer();
	bool connect () ;
	virtual void on_peer_event(e_peer_event eps) = 0;
	void PrintBytes(char* msg, int length);

	//非线程安全 仅供用做数据统计
	virtual void on_recv_data(uint32_t len){};	
	virtual void on_send_data(uint32_t len){};	
	
	send_buffer m_outbuf;
	recv_buffer m_inbuf;

	boost::asio::io_service& get_service();
private:	

	enable_safe_queue<msgbuf> m_sendqueue;
	enable_safe_queue<msgbuf> m_recvqueue;
	boost::atomic_bool m_bSending;

	boost::scoped_ptr<boost::asio::ip::tcp::socket> m_socket;
	uint16_t m_id;
	boost::asio::ip::tcp::endpoint m_endpoint;
	
	boost::scoped_ptr<boost::asio::deadline_timer> m_deadtime;
	bool		m_bread;
	bool		m_bcheck_time;
	boost::scoped_ptr<packet_head_s> m_bufhead_s;
	boost::scoped_ptr<packet_head_s> m_bufhead_r;
	bool		m_need_route;
	GATE_ROUTE_HANDLER m_func;
	e_peer_state m_state;
	uint16_t remoto_type;
	uint16_t remoto_id;
};
