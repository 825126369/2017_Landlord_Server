#include "stdafx.h"
#include "th_pay_check.h"
#include "world_server.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include <enable_crypto.h>
#include <boost/algorithm/string.hpp>
#include <enable_json_map.h>
#include <boost/lexical_cast.hpp>
#include "payment_task.h"
#include "proc_logic2world_protocol.h"
#include "game_sys_recharge.h"
#include "payment_fix.h"

using namespace boost;

th_pay_check::th_pay_check(boost::asio::io_service& io_service)
	:peer_http(io_service)
	,m_result(false)
{

}

th_pay_check::~th_pay_check()
{

}

void th_pay_check::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;
	use_synchronization();	
}

void th_pay_check::init_task(boost::shared_ptr<game_player> client)
{
	m_client = boost::weak_ptr<game_player>(client);
	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");	
	auto fmt = format("/th_pay_check.aspx?account=%1%") 
		% client->Account->get_string();

	mr.spath = fmt.str();
	post_request(mr);
}

void th_pay_check::on_complete()
{
	__ENTER_FUNCTION;

	if(m_result)
	{		
		std::string resposeInfo = enable_crypto_helper::Base64Decode(m_retinfo);	
		SLOG_INFO << "th_pay_check info:"<<resposeInfo;

		ENABLE_MAP<std::string, std::string> mData;
		enable_json_helper::str_to_json_map(resposeInfo, mData);
		auto it = mData.find("result");
		if (it != mData.end())
		{
			std::string& result = it->second;
			if (result == "true")
			{
				it = mData.find("data");
				if (it != mData.end())//处理多个订单
				{							
					std::vector<std::string> vec;
					split(vec, it->second, algorithm::is_space(), token_compress_on);

					auto cl = m_client.lock();
					for (int i = 0;i<vec.size();i++)
					{
						std::vector<std::string> vs;
						split(vs, vec[i], boost::is_any_of("_"), token_compress_on);

						if(vs.size() < 3)
						{
							std::string tmp;
							for (int j = 0; j<vs.size(); j++)
							{
								tmp += vs[j] + " ";
							}

							SLOG_ERROR << "th_pay_check error:" << tmp;
							continue;
						}

						int currencytype = lexical_cast<int>(vs[1]);						
						int currencycount = lexical_cast<int>(vs[2]);

						cl->get_sys<game_sys_recharge>()->payment_once(vs[0], currencytype, currencycount);
					}

					return;
				}

				SLOG_ERROR << "th_pay_check:" << resposeInfo;
			}
		}		
		
	}
	else
	{
		SLOG_ERROR << "th_pay_check false";
	}

	__LEAVE_FUNCTION;
}

