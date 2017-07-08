#pragma once
#include <enable_queue.h>
#include <enable_singleton.h>
#include <enable_smart_ptr.h>

class peer_http;
class peer_http_mgr:
	public enable_singleton<peer_http_mgr>
{
public:
	peer_http_mgr();
	virtual ~peer_http_mgr();

	void postTask(boost::shared_ptr<peer_http> task);
	void update(double elapsed);

private:
	fast_safe_queue<boost::shared_ptr<peer_http>> m_complate_queue;
};

