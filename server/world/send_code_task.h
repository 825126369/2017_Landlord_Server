#pragma once
#include <net/peer_http.h>
#include <boost/function.hpp>

class game_player;

class SendCode : public peer_http
{
public:
	SendCode(boost::asio::io_service& io_service);
	virtual void on_complete();
protected:
	virtual void http_response(bool result, const std::string& response);
private:
	bool m_result;
	std::string m_retinfo;
};