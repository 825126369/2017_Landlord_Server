#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

class game_player;

class check_account_task : public peer_http
{
public:
	check_account_task(boost::asio::io_service& io_service);
	virtual ~check_account_task();

	void set_client(boost::shared_ptr<game_player> client, bool isrelogin = false);

	virtual void on_complete();

	
protected:
	virtual void http_response(bool result, const std::string& response);

private:
	bool m_result;
	bool m_relogin;
	std::string m_retinfo;
	boost::weak_ptr<game_player> m_client;
};