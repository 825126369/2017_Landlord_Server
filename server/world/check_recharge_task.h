#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

class game_player;

class check_recharge_task : public peer_http
{
public:
	check_recharge_task(boost::asio::io_service& io_service);
	virtual ~check_recharge_task();

	void init_task(boost::shared_ptr<game_player> client);

	virtual void on_complete();
protected:
	virtual void http_response(bool result, const std::string& response);

private:
	bool m_result;
	std::string m_retinfo;
	boost::weak_ptr<game_player> m_client;
};