#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

// 异常警告处理
class ExceptionWarnTask : public peer_http
{
public:
	ExceptionWarnTask(boost::asio::io_service& io_service);
	virtual ~ExceptionWarnTask();

	virtual void on_complete();
protected:
	virtual void http_response(bool result, const std::string& response);
};