#pragma once
//跨平台控制台退出处理
#include <boost/thread.hpp>

class interrupt_service_handler
{
public:
	virtual ~interrupt_service_handler(){}

	virtual void run(){};
	virtual void beforeInterruptQuit(){};
	virtual void beforeNormalQuit(){};
};

class interrupt_service
{
public:
	interrupt_service(interrupt_service_handler* handler_);
	virtual ~interrupt_service();
	void join(bool grace_quit_flag_ = false);   
private:   
	boost::thread_group grp;
	interrupt_service_handler* handler;
	bool grace_quit_flag;
	boost::mutex single_close_mtx;
	bool single_close_flag;
	bool checkFirstClose();
#ifdef WIN32
	void signalHandler();
#else
	void detectSignalThread();
#endif
};