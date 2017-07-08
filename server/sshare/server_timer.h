#pragma once
#include <boost/asio.hpp>
#include <enable_smart_ptr.h>
#include <boost/function.hpp>
#include <boost/enable_shared_from_this.hpp>

class server_timer
	:public boost::enable_shared_from_this<server_timer>
{
public:
	server_timer(boost::asio::io_service& ioservice);
	~server_timer();

	//s √Î
	void init(boost::function0<void> func, int s);
private:
	void on_tick();
	boost::function0<void> m_func;
	boost::asio::deadline_timer m_deadtime;
};
