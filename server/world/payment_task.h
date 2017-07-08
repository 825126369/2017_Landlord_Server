#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

class game_player;

class payment_task : public peer_http
{
public:
	payment_task(boost::asio::io_service& io_service);
	virtual ~payment_task();

	void init_task(const std::string& orderid, boost::shared_ptr<game_player> client, bool payment_lottery = false);

	virtual void on_complete();
protected:
	virtual void http_response(bool result, const std::string& response);

private:
	void check_more_time();

	bool m_result;
	std::string m_retinfo;
	boost::weak_ptr<game_player> m_client;
	std::string m_orderid;
	uint16_t m_check_count;	
	msg_request m_req;
	bool m_payment_lottery;
};