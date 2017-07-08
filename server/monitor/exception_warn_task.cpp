#include "stdafx.h"
#include "exception_warn_task.h"

ExceptionWarnTask::ExceptionWarnTask(boost::asio::io_service& io_service)
	:peer_http(io_service)
{

}

ExceptionWarnTask::~ExceptionWarnTask()
{

}

void ExceptionWarnTask::http_response(bool result, const std::string& response)
{

}

void ExceptionWarnTask::on_complete()
{
}