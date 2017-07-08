#include "stdafx.h"
#include "server_timer.h"

using namespace boost;

server_timer::server_timer(boost::asio::io_service& ioservice)
	:m_deadtime(ioservice)
{
}

server_timer::~server_timer()
{
	m_deadtime.cancel();
}

void server_timer::init(boost::function0<void> func, int s)
{
	m_func = func;
	m_deadtime.expires_from_now(posix_time::seconds(s));
	m_deadtime.async_wait(boost::bind(&server_timer::on_tick, shared_from_this()));	
}

void server_timer::on_tick()
{
	if(!m_func.empty())
		m_func();

	m_deadtime.cancel();
}