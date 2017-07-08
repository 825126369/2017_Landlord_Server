#include "stdafx.h"
#include "check_account_task.h"
#include "game_player.h"

check_account_task::check_account_task(boost::asio::io_service& io_service)
	:peer_http(io_service)
	,m_result(false)
	,m_relogin(false)
{

}

check_account_task::~check_account_task()
{

}

void check_account_task::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;

	use_synchronization();	
}

void check_account_task::set_client(boost::shared_ptr<game_player> client, bool isrelogin)
{
	m_client = boost::weak_ptr<game_player>(client);
	m_relogin = isrelogin;
}

void check_account_task::on_complete()
{
	auto cl = m_client.lock();
	if(!cl)
	{			
		return;
	}

	if(m_result)
	{
		m_result = cl->http_run(m_retinfo);
		if(!m_result)
			m_retinfo = "check_account_task error";
	}
	cl->http_check(m_result, m_retinfo, m_relogin);
}