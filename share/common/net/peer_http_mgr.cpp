#include "stdafx.h"
#include "peer_http_mgr.h"
#include <net\peer_http.h>

peer_http_mgr::peer_http_mgr()
{
}

peer_http_mgr::~peer_http_mgr()
{
}

void peer_http_mgr::postTask(boost::shared_ptr<peer_http> task)
{
	m_complate_queue.push(task);
}
void peer_http_mgr::update(double elapsed)
{
	boost::shared_ptr<peer_http> task;
	while (!m_complate_queue.empty())
	{		
		if(m_complate_queue.pop(task))
			task->on_complete();
	}
}