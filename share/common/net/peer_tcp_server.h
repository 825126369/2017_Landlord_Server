#pragma once
#include <boost/asio.hpp>
#include <enable_smart_ptr.h>
#include <boost/function.hpp>
#include <boost/atomic.hpp>

class peer_tcp;
class peer_tcp_server 
{
public :
	peer_tcp_server(boost::asio::io_service& ioservice, uint16_t port, boost::function0<void> callback) ;

	virtual ~peer_tcp_server () ;
	
public :	
	// accept new connection
	bool accept_v4 (boost::shared_ptr<peer_tcp> peer, bool force = false);
	bool accept_v6 (boost::shared_ptr<peer_tcp> peer, bool force = false);

	void close();

	bool check_accept_v4(bool force = false);
	bool check_accept_v6(bool force = false);
private:
	void handle_accept(const boost::system::error_code& error, boost::shared_ptr<peer_tcp> peer );
	void handle_accept2(const boost::system::error_code& error, boost::shared_ptr<peer_tcp> peer );
	
	//boost::asio::io_service& m_ioservice;
	boost::function0<void> m_func;

	boost::atomic_int_fast32_t m_v4flag;
	boost::atomic_int_fast32_t m_v6flag;

protected:
	boost::scoped_ptr<boost::asio::ip::tcp::acceptor> m_acceptor;
	boost::scoped_ptr<boost::asio::ip::tcp::acceptor> m_acceptor2;
	virtual void init_ip(boost::asio::io_service& ioservice, uint16_t port) = 0;
};

//////////////////////////////////////////////////////////////////////////
//ipv4
class peer_v4_server :public peer_tcp_server
{
public:
	peer_v4_server(boost::asio::io_service& ioservice, uint16_t port, boost::function0<void> callback) ;
	virtual ~peer_v4_server () ;

protected:
	virtual void init_ip(boost::asio::io_service& ioservice, uint16_t port);
};

//////////////////////////////////////////////////////////////////////////
//ipv6
class peer_v6_server:public peer_tcp_server
{
public:
	peer_v6_server(boost::asio::io_service& ioservice, uint16_t port, boost::function0<void> callback) ;
	virtual ~peer_v6_server () ;

protected:
	virtual void init_ip(boost::asio::io_service& ioservice, uint16_t port);
};

//////////////////////////////////////////////////////////////////////////
//ipv4 +ipv6
class peer_v4v6_server:public peer_tcp_server
{
public:
	peer_v4v6_server(boost::asio::io_service& ioservice, uint16_t port, boost::function0<void> callback) ;
	virtual ~peer_v4v6_server () ;

protected:
	virtual void init_ip(boost::asio::io_service& ioservice, uint16_t port);
};