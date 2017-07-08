#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

class game_player;

class payment_fix : public peer_http
{
public:
	payment_fix(boost::asio::io_service& io_service);
	virtual ~payment_fix();

	void init_task(const std::string& orderid, const std::string& platform);

	virtual void on_complete();
protected:
	virtual void http_response(bool result, const std::string& response);

private:
	bool m_result;
	std::string m_retinfo;
};