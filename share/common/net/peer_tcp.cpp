#include "stdafx.h"
#include "peer_tcp.h"
#include "net_buffer.h"
#include "com_log.h"
#include "packet_head.h"
#include <boost/bind.hpp>
#include <google/protobuf/message.h>
#include "packet_manager.h"
#include <boost/timer.hpp>


using namespace boost;
using namespace boost::asio::ip;

//-----------------------------------------------
peer_tcp::peer_tcp()
{	
	m_state = e_ps_disconnected;
	m_id = 0;
	m_bSending = false;
	m_bread = false;
	m_bcheck_time = false;
	m_need_route = false;
	remoto_type = 0;
	remoto_id = 0;
	
	clear_buffer();
}

void peer_tcp::init_peer(boost::asio::io_service& ioservice, uint16_t _id, bool _encrypt)
{
	m_socket.reset(new boost::asio::ip::tcp::socket(ioservice));
	m_deadtime.reset(new boost::asio::deadline_timer(ioservice));
	m_id = _id;
	if(_encrypt)
	{
		m_bufhead_s.reset(new packet_head_c);
		m_bufhead_r.reset(new packet_head_c);
	}
	else
	{
		m_bufhead_s.reset(new packet_head_s);
		m_bufhead_r.reset(new packet_head_s);
		init_buffer();
	}
}

void peer_tcp::init_buffer()
{
	m_outbuf.reset_size(1024*128, 1024*1024);
	m_inbuf.reset_size(1024*128, 1024*1024);
}

void peer_tcp::clear_buffer()
{
	m_outbuf.clearup();
	m_inbuf.clearup();
	m_sendqueue.clear();
	m_recvqueue.clear();
	m_bSending = false;
}

tcp::socket& peer_tcp::socket()
{
	return *m_socket;
}

peer_tcp::~peer_tcp()
{
	if (m_state == e_ps_disconnected)
		return;

	if (m_deadtime)
		m_deadtime->cancel();

	__ENTER_FUNCTION
		if (m_socket->is_open())
		{
			system::error_code ec;
			m_socket->shutdown(tcp::socket::shutdown_send, ec);
			m_socket->close(ec);
		}

	m_state = e_ps_disconnected;

	__LEAVE_FUNCTION

		clear_buffer();

}

void peer_tcp::handle_accept(const system::error_code& error) //如果自身是服务器 就用这个
{
	__ENTER_FUNCTION
		if(error==0)
		{
			clear_buffer();
			//实现对每个客户端的数据处理
			if(m_bcheck_time)
			{
				m_bread = false;
				m_deadtime->expires_from_now(posix_time::seconds(3));
				m_deadtime->async_wait(boost::bind(&peer_tcp::timeout, shared_from_this()));
			}
			m_endpoint = m_socket->remote_endpoint();
			m_state = e_ps_connected;
			on_peer_event(e_peer_event::e_pe_accepted);
			poet_recv();
		}
		else
		{
			SLOG_WARNING << format("sessionid: %1% msg: %2%")%get_id()%error.message();
			m_state = e_ps_disconnected;
			on_peer_event(e_peer_event::e_pe_acceptfail);		
			//close();
		}

		__LEAVE_FUNCTION
}

void peer_tcp::handle_connect(const system::error_code& error) //如果自身是客户端 就用这个
{
	__ENTER_FUNCTION
		if(!error)
		{
			clear_buffer();
			m_state = e_ps_connected;
			on_peer_event(e_peer_event::e_pe_connected);
			poet_recv();
		}
		else
		{
            SLOG_WARNING << format("sessionid: %1% error:%2% msg: %3%")%get_id()%error.value()%error.message();
			system::error_code ec;
            m_socket->cancel(ec);
            if(m_socket->is_open())
            {
                m_socket->shutdown(tcp::socket::shutdown_send, ec);
                m_socket->close(ec);
            }
            
			m_state = e_ps_disconnected;
			on_peer_event(e_peer_event::e_pe_connectfail);
        }
	__LEAVE_FUNCTION
}

// 发送数据给客户端，不需要路由转发
int peer_tcp::send_msg(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg) 
{
	if(m_state != e_ps_connected)
		return -1;

	__ENTER_FUNCTION	
	msgbuf mb;
	mb.packet_id = packet_id;
	mb.msg = msg;	
	mb.need_route = false;
	m_sendqueue.safe_push(mb);		
	
	return m_sendqueue.size();

	__LEAVE_FUNCTION

		return -1;
}

// 发送数据给服务器，有的需要路由转发
int peer_tcp::send_msg(uint16_t packet_id, const std::string& msgbuff) 
{
	if(m_state != e_ps_connected)
		return -1;

	__ENTER_FUNCTION	
	msgbuf mb;
	mb.packet_id = packet_id;
	mb.need_route = true;
	mb.msgbuff = msgbuff;		
	m_sendqueue.safe_push(mb);		

	return m_sendqueue.size();

	__LEAVE_FUNCTION

		return -1;
}

void peer_tcp::post_send()
{
	if(m_state != e_ps_connected)
		return;

	__ENTER_FUNCTION		

		m_bSending = true;

		msgbuf mb; 
		while (m_sendqueue.safe_pop(mb))
		{			
			if(mb.need_route && m_need_route)
			{
				int len = mb.msgbuff.size();				
				int oldlen = m_outbuf.get_size();	
				m_bufhead_s->init(mb.packet_id, len);

				if(!m_outbuf.malloc_buf(PACKET_HEAD_SIZE + len))//
				{
					SLOG_ERROR << format( "sessionid: %1% m_outbuf is full")%get_id();
					break;
				}

				char* pbuf = m_outbuf.get_sendbuf() + oldlen;
				m_bufhead_s->to_array(pbuf);

				std::memcpy(pbuf+PACKET_HEAD_SIZE, mb.msgbuff.c_str(), len);
				m_bufhead_s->buffer_decryption(pbuf+PACKET_HEAD_SIZE, len);
			}
			else
			{
				int len = mb.msg->ByteSize();
				int oldlen = m_outbuf.get_size();	
				m_bufhead_s->init(mb.packet_id, len);

				if(!m_outbuf.malloc_buf(PACKET_HEAD_SIZE + len))
				{
					SLOG_ERROR << format( "sessionid: %1% m_outbuf is full")%get_id();
					break;
				}

				char* pbuf = m_outbuf.get_sendbuf() + oldlen;
				m_bufhead_s->to_array(pbuf);
				if(!mb.msg->SerializeToArray(pbuf+PACKET_HEAD_SIZE, len))
				{
					SLOG_ERROR << format( "sessionid: %1% Serialize error")%get_id();
					return;
				}
				m_bufhead_s->buffer_decryption(pbuf+PACKET_HEAD_SIZE, len);
			}				
		}	

		if(m_outbuf.get_size() <=0)
		{
			m_bSending = false;
			return;
		}

		asio::async_write(*m_socket, asio::buffer(m_outbuf.get_sendbuf(), m_outbuf.get_sendlen()), 
			boost::bind(&peer_tcp::handle_write, shared_from_this(), asio::placeholders::error, asio::placeholders::bytes_transferred));

		__LEAVE_FUNCTION
}
void peer_tcp::poet_recv()
{	
	__ENTER_FUNCTION
		if(!m_inbuf.malloc_buf())
		{
			SLOG_ERROR << format( "sessionid: %1% m_inbuf is full")%get_id();
			close();
			return;

		}
		m_socket->async_read_some(asio::buffer(m_inbuf.get_recvbuf(),m_inbuf.get_recvlen()),
			boost::bind(&peer_tcp::handle_read, shared_from_this(),
			asio::placeholders::error, asio::placeholders::bytes_transferred));

		return;
		__LEAVE_FUNCTION
			close();
}


void peer_tcp::handle_write(const system::error_code& error, size_t bytes_transferred)
{
	__ENTER_FUNCTION
		if(!m_socket->is_open())
			return;

	if(m_state == e_ps_disconnected)
		return;

	if(error.value() != 0)
	{
		SLOG_INFO << format( "sessionid: %1% msg: %2%")%get_id()%error.message();
	}

	if(!error && bytes_transferred>0)
	{		
		m_outbuf.release_buf(bytes_transferred);			
		on_send_data(bytes_transferred);
		post_send();
	}
	return;
	__LEAVE_FUNCTION

}

void  peer_tcp::timeout()
{
	if(!m_bread)
	{
		close();
	}
	system::error_code ec;
	m_deadtime->cancel(ec);
}

void peer_tcp::PrintBytes(char* msg,int length) 
{
	std::cout <<"头部字节：";
	for (int i = 0; i < length; i++) 
	{
		if (i != 11)
		{
			std::cout<<(int)msg[i]<<" ";
		}
		else
		{
			std::cout<<(int)msg[i]<<" | ";
		}
	}
	std::cout<< std::endl;
}

void peer_tcp::handle_read(const system::error_code& error, size_t bytes_transferred)
{
	__ENTER_FUNCTION
		if (!m_socket->is_open())
			return;

	if (m_state == e_ps_disconnected)
		return;

	if (error.value() != 0)
	{
		SLOG_INFO << format("sessionid: %1% msg: %2%") % get_id() % error.message();
	}

	if (!error && bytes_transferred > 0)
	{
		if (!m_bread) m_bread = true;

		m_inbuf.fix_offset(bytes_transferred);

		while (m_inbuf.get_offset() >= PACKET_HEAD_SIZE)//buff长度大于包头长度时
		{
			m_bufhead_r->parse_from(m_inbuf.get_head());
			if (!m_bufhead_r->check_head())
			{
				SLOG_ERROR << format("sessionid: %1% packet_head error") % get_id();
				close();
				return;
			}

			if (m_inbuf.get_offset() < PACKET_HEAD_SIZE + m_bufhead_r->get_size())//bytes_transferred size 小于实际数据值
				break;

			m_bufhead_r->buffer_decryption(m_inbuf.get_head() + PACKET_HEAD_SIZE, m_bufhead_r->get_size());
			msgbuf mb;
			auto msg = PACKET_CREATE(google::protobuf::Message, m_bufhead_r->get_id());
			if (msg == nullptr || !msg->ParseFromArray(m_inbuf.get_head() + PACKET_HEAD_SIZE, m_bufhead_r->get_size()))
			{
				if (!m_need_route)
				{
					SLOG_ERROR << format("sessionid: %1%  ParseFromArray error id: %2%") % get_id() % m_bufhead_r->get_id();
					close();
					return;
				}

				mb.need_route = m_need_route;
				mb.msgbuff = std::string(m_inbuf.get_head() + PACKET_HEAD_SIZE, m_bufhead_r->get_size());
			}
			else
			{
				mb.msg = msg;
			}

			mb.packet_id = m_bufhead_r->get_id();
			m_recvqueue.safe_push(mb);

			m_inbuf.release_buf(PACKET_HEAD_SIZE + m_bufhead_r->get_size());
		}

		on_recv_data(bytes_transferred);
		on_peer_event(e_peer_event::e_pe_recved);
		poet_recv();
	}
	else
	{
		close();
	}
	return;
	__LEAVE_FUNCTION
		close();

}

// close previous connection and connect to another server socket
bool peer_tcp::reconnect ( const char* host , uint32_t port )
{
	__ENTER_FUNCTION

		// delete old socket impl object
		close();

	return connect(host, port);

	__LEAVE_FUNCTION

		return false ;
}

bool peer_tcp::reconnect()
{
	__ENTER_FUNCTION
		close();
	m_state = e_ps_connecting;
	m_socket->async_connect(m_endpoint,boost::bind(&peer_tcp::handle_connect, shared_from_this(), asio::placeholders::error));
	return true;
	__LEAVE_FUNCTION
		m_state = e_ps_disconnected;
	return false;
}

void peer_tcp::close()
{
	if (m_state == e_ps_disconnected)
		return;

	__ENTER_FUNCTION
		if (m_socket->is_open())
		{
			system::error_code ec;
			m_socket->shutdown(tcp::socket::shutdown_send, ec);
			m_socket->close(ec);
		}

	m_state = e_ps_disconnected;
	on_peer_event(e_peer_event::e_pe_disconnected);

	__LEAVE_FUNCTION
}

void peer_tcp::discannect()
{
	if (m_state == e_ps_disconnected || m_state == e_ps_disconnecting)
	{
		return;
	}

	__ENTER_FUNCTION

		m_state = e_ps_disconnecting;
	m_socket->get_io_service().post(boost::bind(&peer_tcp::close, shared_from_this()));
	__LEAVE_FUNCTION

}

void peer_tcp::set_check_time()
{
	m_bcheck_time =true;
}

bool peer_tcp::connect (const char* host, uint16_t port) 
{ 
	if(m_state != e_ps_disconnected)
		return false;

	__ENTER_FUNCTION

		m_endpoint.address(asio::ip::address::from_string(host));
	m_endpoint.port(port);
	return connect();

	__LEAVE_FUNCTION
		m_state = e_ps_disconnected;
	return false;
}

bool peer_tcp::connect (const std::string& host, uint16_t port)
{

	if(m_state != e_ps_disconnected)
		return false;

	__ENTER_FUNCTION

		m_endpoint.address(asio::ip::address::from_string(host));
	m_endpoint.port(port);
	return connect();

	__LEAVE_FUNCTION
		m_state = e_ps_disconnected;
	return false;
}

bool peer_tcp::connect(const std::string& str_url) 
{
	if(m_state != e_ps_disconnected)
		return false;

	__ENTER_FUNCTION
		tcp::resolver resolver(m_socket->get_io_service());
	tcp::resolver::query query(str_url, "http");
	tcp::resolver::iterator iterator = resolver.resolve(query);
	m_endpoint = (*iterator).endpoint();
	return connect();
	__LEAVE_FUNCTION
		m_state = e_ps_disconnected;
	return false;
}

bool peer_tcp::connect (boost::asio::ip::tcp::endpoint& server_endpoint)
{
	if(m_state != e_ps_disconnected)
		return false;

	__ENTER_FUNCTION
	m_endpoint = server_endpoint;
	return connect();
	__LEAVE_FUNCTION
		m_state = e_ps_disconnected;
	return false;
}

bool peer_tcp::connect () 
{
	__ENTER_FUNCTION
	m_state = e_ps_connecting;

	boost::system::error_code err;
	std::string ip_str = m_endpoint.address().to_string(err);
	SLOG_INFO << format("connect ip: %1%")%ip_str;

	m_socket->async_connect(m_endpoint,
		boost::bind(&peer_tcp::handle_connect, shared_from_this(), asio::placeholders::error)
		);
	return true;
	__LEAVE_FUNCTION
		m_state = e_ps_disconnected;
	return false;
}

bool peer_tcp::resolve_hostname(const std::string& hostname, uint16_t port, std::vector<boost::asio::ip::tcp::endpoint>& endpoints)
{
	endpoints.clear();

	tcp::resolver resolver(m_socket->get_io_service());
	tcp::resolver::query query(hostname, boost::lexical_cast<std::string>(port));
	boost::system::error_code err;
	tcp::resolver::iterator iterator = resolver.resolve(query, err);
	tcp::resolver::iterator end;
	for(; iterator!=end; iterator++)  
	{  
		endpoints.push_back((*iterator).endpoint());
	}
	
	return !err;
}

uint16_t peer_tcp::get_remote_port () 
{ 
	__ENTER_FUNCTION
		return m_endpoint.port();

	__LEAVE_FUNCTION

		return 0;
}

std::string peer_tcp::get_remote_ip () 
{ 
	__ENTER_FUNCTION

		return m_endpoint.address().to_string();

	__LEAVE_FUNCTION
		return "";
}

uint16_t peer_tcp::get_remote_type()
{
	return remoto_type;
};
void peer_tcp::set_remote_type(uint16_t _type)
{
	remoto_type = _type;
}
uint16_t peer_tcp::get_remote_id()
{
	return remoto_id;
};
void peer_tcp::set_remote_id(uint16_t _id)
{
	remoto_id = _id;
}

int peer_tcp::packet_service(int process_count)
{
	__ENTER_FUNCTION

	msgbuf mb;
	timer tm;
	int i =0;
	while(m_recvqueue.safe_pop(mb))
	{
		if(process_count>0 && i>=process_count)
			break;

		tm.restart();

		if (m_need_route &&mb.need_route)
		{
			if(m_func.empty() || !m_func(shared_from_this(), mb.packet_id, mb.msgbuff))//服务器路由功能，在这里执行路由功能
			{
				return mb.packet_id;
			}
		}
		else
		{
			auto factory = packet_manager::instance().get_factroy(mb.packet_id);
			if(factory == nullptr ||!factory->packet_process(shared_from_this(), mb.msg))
			{			
				return mb.packet_id;
			}
		}		

		if(tm.elapsed() >0.05)
		{
			SLOG_WARNING << format("sessionid: %1% packet_process id: %2% time: %3%")%get_id()%m_bufhead_r->get_id()%tm.elapsed();
		}
		i++;
	}

	if(!m_bSending && !m_sendqueue.empty())
	{
		m_bSending = true;
		m_socket->get_io_service().post(boost::bind(&peer_tcp::post_send, shared_from_this())); 
	}

	return 0;
	__LEAVE_FUNCTION
	return -1;
}

e_peer_state peer_tcp::get_state()
{
	return m_state;
}

void peer_tcp::set_route_handler(GATE_ROUTE_HANDLER handler)
{
	if(!handler.empty())
	{
		m_func = handler;
		m_need_route = true;
	}	
}

boost::asio::io_service& peer_tcp::get_service()
{
	return m_socket->get_io_service();
}