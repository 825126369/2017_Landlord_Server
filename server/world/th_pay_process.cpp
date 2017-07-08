#include "stdafx.h"
#include "th_pay_process.h"
#include "world_server.h"

using namespace boost;

th_pay_process::th_pay_process(boost::asio::io_service& io_service)
	:peer_http(io_service)
{

}
th_pay_process::~th_pay_process()
{

}

void th_pay_process::init_task(const std::string& orderid, const std::string& account)
{
	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");	
	auto fmt = format("/th_pay_process.aspx?orderid=%1%&account=%2%")
		% orderid
		% account;

	mr.spath = fmt.str();
	post_request(mr);
}


void th_pay_process::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;
	use_synchronization();	
}


void th_pay_process::on_complete()
{
	__ENTER_FUNCTION;

	if(!m_result)
	{
		SLOG_ERROR << "th_pay_process fail:" <<m_retinfo;
	}

	__LEAVE_FUNCTION;
}

