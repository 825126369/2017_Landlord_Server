#include "stdafx.h"
#include "payment_fix.h"
#include "world_server.h"

using namespace boost;

payment_fix::payment_fix(boost::asio::io_service& io_service)
	:peer_http(io_service)
{

}
payment_fix::~payment_fix()
{

}

void payment_fix::init_task(const std::string& orderid, const std::string& platform)
{
	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");	
	auto fmt = format("/FixRecharge.aspx?orderid=%1%&platform=%2%")
		% orderid
		% platform;

	mr.spath = fmt.str();
	post_request(mr);
}


void payment_fix::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;
	use_synchronization();	
}


void payment_fix::on_complete()
{
	__ENTER_FUNCTION;

	if(!m_result)
	{
		SLOG_ERROR << "payment_fix fail:" <<m_retinfo;
	}

	__LEAVE_FUNCTION;
}

