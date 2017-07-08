#include "stdafx.h"
#include "interrupt_service.h"

#include <boost/bind.hpp>
#include <stdexcept>
#ifdef WIN32
#include <windows.h>
#else
#include <pthread.h>
#include <signal.h>
#include <unistd.h>
#endif

#ifdef WIN32
boost::function0<void> before_interrupt_quit_func;
BOOL WINAPI console_ctrl_handler(DWORD ctrl_type)
{
	switch (ctrl_type)
	{
	case CTRL_C_EVENT:
	case CTRL_BREAK_EVENT:
	case CTRL_CLOSE_EVENT:
	case CTRL_SHUTDOWN_EVENT:
		before_interrupt_quit_func();
		return TRUE;
	default:
		return FALSE;
	}
}
void interrupt_service::signalHandler()
{
	if (checkFirstClose())
	{
		handler->beforeInterruptQuit();
		if (!grace_quit_flag)
		{
			exit(0);
		}           
	}               
}
#else
void interrupt_service::detectSignalThread()
{
	// Wait for signal indicating time to shut down.
	sigset_t wait_mask;
	sigemptyset(&wait_mask);
	sigaddset(&wait_mask, SIGINT);
	sigaddset(&wait_mask, SIGQUIT);
	sigaddset(&wait_mask, SIGTERM);
	int sig = 0;
	sigwait(&wait_mask, &sig);
	if (checkFirstClose())
	{
		handler->beforeInterruptQuit();
		if (!grace_quit_flag)
		{
			exit(0);
		}
	}
}
#endif

interrupt_service::interrupt_service(interrupt_service_handler* handler_)
	: handler(handler_)
	, grace_quit_flag(true)
	, single_close_flag(false)
{
	static int INTERRUPT_SERVICE_INSTANCE_TIMES = 0;
	if ((INTERRUPT_SERVICE_INSTANCE_TIMES++) != 0)
	{
		throw std::runtime_error("misuse: interrupt_service should only be instance once!");
	}
#ifdef WIN32
	grp.create_thread(boost::bind(&interrupt_service_handler::run, handler));
	before_interrupt_quit_func = boost::bind(&interrupt_service::signalHandler, this);
	SetConsoleCtrlHandler(console_ctrl_handler, TRUE);
#else
	// Block all signals for background thread.
	sigset_t new_mask;
	sigfillset(&new_mask);   
	pthread_sigmask(SIG_BLOCK, &new_mask, NULL);
	grp.create_thread(boost::bind(&interrupt_service_handler::run, handler));
#endif
}


interrupt_service::~interrupt_service()
{
	handler = nullptr;
}

void interrupt_service::join(bool grace_quit_flag_)
{
	grace_quit_flag = grace_quit_flag_;
#ifndef WIN32
	sigset_t wait_mask;
	sigemptyset(&wait_mask);
	sigaddset(&wait_mask, SIGINT);
	sigaddset(&wait_mask, SIGQUIT);
	sigaddset(&wait_mask, SIGTERM);
	pthread_sigmask(SIG_BLOCK, &wait_mask, 0);
	boost::thread detect_signal_thread(boost::bind(&interrupt_service::detectSignalThread, this));
	sigset_t new_mask;
	sigfillset(&new_mask);   
	pthread_sigmask(SIG_BLOCK, &new_mask, NULL);
#endif
	grp.join_all();
	if (checkFirstClose())
	{
		handler->beforeNormalQuit();           
	}
}

bool interrupt_service::checkFirstClose()
{
	boost::mutex::scoped_lock lock(single_close_mtx);
	if (!single_close_flag)
	{
		single_close_flag = true;
		return true;       
	}   
	else
	{
		return false;
	}
}