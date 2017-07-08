#include "stdafx.h"
#include "check_recharge_task.h"
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

check_recharge_task::check_recharge_task(boost::asio::io_service& io_service)
	:peer_http(io_service)
	,m_result(false)
{

}

check_recharge_task::~check_recharge_task()
{

}

void check_recharge_task::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;
	use_synchronization();	
}

void check_recharge_task::init_task(boost::shared_ptr<game_player> client)
{
	m_client = boost::weak_ptr<game_player>(client);
	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");
	//auto fmt = format("%1%?account=%2%&platform=%3%")%"/CheckRecharge.aspx"%client->Account->get_string()%client->Platform->get_string();
	auto fmt = format("/CheckRecharge.aspx?account=%1%&platform=%2%&playerid=%3%") 
		% client->Account->get_string()
		% client->Platform->get_string()
		% client->PlayerId->get_value();

	mr.spath = fmt.str();
	post_request(mr);
}

void check_recharge_task::on_complete()
{
	__ENTER_FUNCTION;

	if(m_result)
	{		
		std::string resposeInfo = enable_crypto_helper::Base64Decode(m_retinfo);	
		SLOG_INFO << "check_recharge_task info:"<<resposeInfo;

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
					std::string pla = "";
					bool noplayer = false;
					if(!cl)
					{			
						SLOG_ERROR << "check_recharge_task player error";
						//特殊处理
						it = mData.find("platform");
						if (it != mData.end())
						{							
							pla = it->second;
						}	
						noplayer = true;
					}

					for (int i = 0;i<vec.size();i++)
					{
						std::vector<std::string> vs;
						split(vs, vec[i], boost::is_any_of(":"), token_compress_on);

						if(vs.size() < 2)
						{
							std::string tmp;
							for (int j = 0; j<vs.size(); j++)
							{
								tmp += vs[j] + " ";
							}

							SLOG_ERROR << "check_recharge_task error:" << tmp;
							continue;
						}

						int payid = lexical_cast<int>(vs[0]);						
						int rmb = lexical_cast<int>(vs[1]);
						int custom = lexical_cast<int>(vs[3]);

						if(noplayer)
						{
							if(vs.size()>2)
							{
								auto pfix = boost::make_shared<payment_fix>(world_server::instance().get_io_service());
								pfix->init_task(vs[2], pla);
							}
							continue;
						}

						if(custom == 0)
							cl->get_sys<game_sys_recharge>()->payment_once(payid, rmb);
						else
							cl->get_sys<game_sys_recharge>()->payment_once(vs[2], custom, payid, rmb);
					}

					return;
				}

				SLOG_ERROR << "check_recharge_task:" << resposeInfo;
			}
		}		
		
	}
	else
	{
		SLOG_ERROR << "check_recharge_task false";
	}

	__LEAVE_FUNCTION;
}

