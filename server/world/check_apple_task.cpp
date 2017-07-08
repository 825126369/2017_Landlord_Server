

#include "stdafx.h"
#include "check_apple_task.h"
#include <enable_json_map.h>
#include "game_player.h"
#include "world_server.h"
#include "proc_c2w_lobby_protocol.h"
#include "game_sys_recharge.h"
#include <boost/algorithm/string.hpp>
#include <enable_crypto.h>

using namespace boost;

check_apple_task::check_apple_task( boost::asio::io_service& io_service )
	:peer_http(io_service)
{

}

check_apple_task::~check_apple_task()
{

}

void check_apple_task::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;
	use_synchronization();	
}

void check_apple_task::init_task(boost::shared_ptr<game_player> client, const std::string&  str)
{
	m_client = boost::weak_ptr<game_player>(client);

	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");
	mr.method = "POST";
	auto fmt = format("/CheckApple.aspx?account=%1%&platform=%2%&playerid=%3%") 
		% client->Account->get_string()
		% client->Platform->get_string()
		% client->PlayerId->get_value();

	header hd1;
	hd1.name = "Content-Length";
	hd1.value = lexical_cast<std::string>(str.length());
	mr.headers.push_back(hd1);
	header hd2;
	hd2.name = "Content-Type";
	hd2.value = "application/octet-stream";
	mr.headers.push_back(hd2);

	mr.content = str;
	mr.spath = fmt.str();
	
	post_request(mr);
}

void check_apple_task::on_complete()
{
	auto cl = m_client.lock();
	if(!cl)
	{			
		return;
	}

	__ENTER_FUNCTION;
		
	if(m_result)
	{
		std::string resposeInfo = enable_crypto_helper::Base64Decode(m_retinfo);
		ENABLE_MAP<std::string, std::string> mData;
		enable_json_helper::str_to_json_map(resposeInfo, mData);
		auto it = mData.find("result");
		if (it != mData.end())
		{
			std::string& result = it->second;
			if (result == "true")
			{
				it = mData.find("data");
				if (it != mData.end())//∂©µ•¥¶¿Ì
				{			
					std::vector<std::string> vec;
					split(vec, it->second, algorithm::is_space(), token_compress_on);
					for (int i = 0;i<vec.size();i++)
					{
						int payid = lexical_cast<int>(vec[i]);
						cl->get_sys<game_sys_recharge>()->payment_once(payid);
					}
					return;
				}
			}
			else
			{				
				it = mData.find("error");
				if (it != mData.end())
				{
					SLOG_ERROR << "payment_task:" << it->second;
				}						
			}
		}
		else
		{
			SLOG_ERROR << "payment_task:" << resposeInfo;
		}
	}

	auto sendmsg = PACKET_CREATE(packetw2c_ask_check_payment_result, e_mst_w2c_ask_check_payment_result);
	sendmsg->set_result(false);
	cl->send_msg_to_client(sendmsg);	

	__LEAVE_FUNCTION;
}
