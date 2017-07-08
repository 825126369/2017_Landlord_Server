#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

class game_player;

class th_pay_process : public peer_http
{
public:
	th_pay_process(boost::asio::io_service& io_service);
	virtual ~th_pay_process();

	void init_task(const std::string& orderid, const std::string& account);

	virtual void on_complete();
protected:
	virtual void http_response(bool result, const std::string& response);

private:
	bool m_result;
	std::string m_retinfo;
};