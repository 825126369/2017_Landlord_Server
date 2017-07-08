#include "stdafx.h"
#include "task_manager.h"

task_handler::task_handler()
	:m_sucess(false)
{

}

task_handler::~task_handler()
{

}

void task_handler::run()
{
	m_sucess = true;
}

void task_handler::complete()
{

}

bool task_handler::is_sucess()
{
	return m_sucess;
}

//////////////////////////////////////////////////////////////////////////

task_manager::task_manager()
	:m_threadpool(boost::thread::hardware_concurrency())
{
}

task_manager::~task_manager()
{
	m_threadpool.clear();
	m_complate_queue.clear();
}

void task_manager::postTask(boost::shared_ptr<task_handler> task, bool isUseThread)
{
	if(isUseThread)
	{
		m_threadpool.schedule(std::bind(&task_manager::runTask, this, task));
	}
	else
	{
		task->run();
		task->complete();
	}
}

void task_manager::update(double elapsed)
{
	boost::shared_ptr<task_handler> task;
	while (!m_complate_queue.empty())
	{		
		if(m_complate_queue.pop(task))
			task->complete();
	}
}

void task_manager::runTask(boost::shared_ptr<task_handler> task)
{
	task->run();
	m_complate_queue.push(task);
}