#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

class game_player;

class th_pay_check : public peer_http
{
public:
	th_pay_check(boost::asio::io_service& io_service);
	virtual ~th_pay_check();

	void init_task(boost::shared_ptr<game_player> client);

	virtual void on_complete();
protected:
	virtual void http_response(bool result, const std::string& response);

private:
	bool m_result;
	std::string m_retinfo;
	boost::weak_ptr<game_player> m_client;
};