#include "stdafx.h"

#include <boost/bind.hpp>

#include "peer_tcp.h"
#include "peer_tcp_server.h"

#include "com_log.h"

using namespace boost;
using namespace boost::asio::ip;

peer_tcp_server::peer_tcp_server (asio::io_service& ioservice, uint16_t port,boost::function0<void> callback) 
{
	__ENTER_FUNCTION
		m_func = callback;

	__LEAVE_FUNCTION
}

peer_tcp_server::~peer_tcp_server () 
{

}

bool peer_tcp_server::accept_v4 ( boost::shared_ptr<peer_tcp> peer, bool force) 
{	
	__ENTER_FUNCTION
		if(!m_acceptor)
			return false;

	if(!force  && m_v4flag==0)
		return false;

	peer->m_state = e_ps_accepting;
	m_acceptor->async_accept(peer->socket(),boost::bind(&peer_tcp_server::handle_accept, this, asio::placeholders::error, peer));

	if(m_v4flag>0)
		m_v4flag--;

	return true;

	__LEAVE_FUNCTION
		peer->m_state = e_ps_disconnected;
	return false ;
}

bool peer_tcp_server::accept_v6 ( boost::shared_ptr<peer_tcp> peer, bool force) 
{	
	__ENTER_FUNCTION
		if(!m_acceptor2)
			return false;

	if(!force  && m_v6flag==0)
		return false;

	peer->m_state = e_ps_accepting;
	m_acceptor2->async_accept(peer->socket(),boost::bind(&peer_tcp_server::handle_accept2, this, asio::placeholders::error, peer));

	if(m_v6flag>0)
		m_v6flag--;

	return true;

	__LEAVE_FUNCTION
		peer->m_state = e_ps_disconnected;
	return false ;
}

bool peer_tcp_server::check_accept_v4(bool force)
{
	if(!m_acceptor ||!m_acceptor->is_open())
		return false;

	if(!force  && m_v4flag==0)
		return false;

	return true;
}

bool peer_tcp_server::check_accept_v6(bool force)
{
	if(!m_acceptor2 ||!m_acceptor2->is_open())
		return false;

	if(!force  && m_v6flag==0)
		return false;

	return true;
}

void peer_tcp_server::handle_accept(const boost::system::error_code& error, boost::shared_ptr<peer_tcp> peer)
{
	__ENTER_FUNCTION

		if (m_acceptor &&!m_acceptor->is_open()) 
			return;  
		

	peer->handle_accept(error);

	m_v4flag++;
	m_func();
	__LEAVE_FUNCTION
}

void peer_tcp_server::handle_accept2(const boost::system::error_code& error, boost::shared_ptr<peer_tcp> peer)
{
	__ENTER_FUNCTION

		if (m_acceptor2 &&!m_acceptor2->is_open()) 
			return;  

	peer->handle_accept(error);
	m_v6flag++;
	m_func();
	__LEAVE_FUNCTION
}




void peer_tcp_server::close()
{
	__ENTER_FUNCTION
	if(m_acceptor)
		m_acceptor->close();

	if(m_acceptor2)
		m_acceptor2->close();
	__LEAVE_FUNCTION
}

//////////////////////////////////////////////////////////////////////////
peer_v4_server::peer_v4_server(boost::asio::io_service& ioservice, uint16_t port, boost::function0<void> callback)
	:peer_tcp_server(ioservice, port, callback)
{
	init_ip(ioservice, port);
}

peer_v4_server::~peer_v4_server()
{

}

void peer_v4_server::init_ip(boost::asio::io_service& ioservice, uint16_t port)
{
	tcp::endpoint endpoint;
	endpoint.address(boost::asio::ip::address_v4::any());
	endpoint.port(port);
	m_acceptor.reset(new tcp::acceptor(ioservice, endpoint));
}

//////////////////////////////////////////////////////////////////////////
peer_v6_server::peer_v6_server(boost::asio::io_service& ioservice, uint16_t port, boost::function0<void> callback)
	:peer_tcp_server(ioservice, port, callback)
{
	init_ip(ioservice, port);
}

peer_v6_server::~peer_v6_server()
{

}

void peer_v6_server::init_ip(boost::asio::io_service& ioservice, uint16_t port)
{
	tcp::endpoint endpoint;
	endpoint.address(boost::asio::ip::address_v6::any());
	endpoint.port(port);
	m_acceptor2.reset(new tcp::acceptor(ioservice, endpoint));
}

//////////////////////////////////////////////////////////////////////////
peer_v4v6_server::peer_v4v6_server(boost::asio::io_service& ioservice, uint16_t port, boost::function0<void> callback)
	:peer_tcp_server(ioservice, port, callback)
{
	init_ip(ioservice, port);
}

peer_v4v6_server::~peer_v4v6_server()
{

}

void peer_v4v6_server::init_ip(boost::asio::io_service& ioservice, uint16_t port)
{
	tcp::endpoint endpoint;
	endpoint.address(boost::asio::ip::address_v4::any());
	endpoint.port(port);
	m_acceptor.reset(new tcp::acceptor(ioservice, endpoint));

	tcp::endpoint endpoint2;
	endpoint2.address(boost::asio::ip::address_v6::any());
	endpoint2.port(port);
	m_acceptor2.reset(new tcp::acceptor(ioservice, endpoint2));
}
