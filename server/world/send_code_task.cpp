#include "stdafx.h"
#include "send_code_task.h"

SendCode::SendCode(boost::asio::io_service& io_service)
	:peer_http(io_service)
	,m_result(false)
{

}

void SendCode::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;
}

void SendCode::on_complete()
{
}