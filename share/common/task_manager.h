#pragma once
#include <string>
//#include <boost/detail/winapi/thread_pool.hpp>
#include <boost/threadpool.hpp>
#include <enable_queue.h>
#include <enable_singleton.h>
#include <enable_smart_ptr.h>

class task_handler
{
public:
	task_handler();
	virtual ~task_handler();
	virtual void run();
	virtual void complete();

	bool is_sucess();
protected:
	bool m_sucess;
};


class task_manager:
	public enable_singleton<task_manager>
{
public:
	task_manager();
	virtual ~task_manager();
	// isUseThread是否使用多线程，默认为不使用
	void postTask(boost::shared_ptr<task_handler> task, bool isUseThread = false);
	void update(double elapsed);

protected:

	void runTask(boost::shared_ptr<task_handler> task);
private:
	fast_safe_queue<boost::shared_ptr<task_handler>> m_complate_queue;
	boost::threadpool::pool m_threadpool;
};

