#include "stdafx.h"
#include "payment_task.h"
#include "world_server.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include <task_manager.h>
#include <enable_crypto.h>
#include <boost/algorithm/string.hpp>
#include <enable_json_map.h>
#include <boost/lexical_cast.hpp>
#include "proc_c2w_lobby_protocol.h"
#include "world_peer.h"
#include "game_sys_recharge.h"
#include "payment_fix.h"

using namespace boost;

payment_task::payment_task(boost::asio::io_service& io_service)
	:peer_http(io_service)
	,m_check_count(0)
	,m_payment_lottery(false)
{

}
payment_task::~payment_task()
{

}

void payment_task::init_task(const std::string& orderid, boost::shared_ptr<game_player> player, bool payment_lottery)
{
	m_payment_lottery = payment_lottery;
	m_orderid = orderid;
	m_client = boost::weak_ptr<game_player>(player);

	m_req.uri = world_server::instance().get_cfg().get<std::string>("http_check");
	//auto fmt = format("%1%?account=%2%&orderid=%3%&platform=%4%")%"/PaymentOnce.aspx"%player->Account->get_string()%m_orderid%player->Platform->get_string();
	auto fmt = format("/PaymentOnce.aspx?account=%1%&orderid=%2%&platform=%3%&playerid=%4%")
		% player->Account->get_string()
		% m_orderid
		% player->Platform->get_string()
		% player->PlayerId->get_value();

	m_req.spath = fmt.str();
	post_request(m_req);
}


void payment_task::http_response(bool result, const std::string& response)
{
	m_result = result;
	m_retinfo = response;
	use_synchronization();	
}


void payment_task::on_complete()
{
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
				if (it != mData.end())//订单处理
				{			
					std::vector<std::string> vs;
					split(vs, it->second, algorithm::is_space(), token_compress_on);

					int payid = lexical_cast<int>(vs[0]);
					int rmb = lexical_cast<int>(vs[1]);
					int custom = 0;
					if (vs.size() >= 4)
					{
						custom = lexical_cast<int>(vs[3]);
					}

					auto cl = m_client.lock();
					if(!cl)
					{			
						SLOG_ERROR << "payment_task player error, orderid:"<<m_orderid;
						//特殊处理
						it = mData.find("platform");
						if (it != mData.end() && vs.size()>2)
						{
							auto pfix = boost::make_shared<payment_fix>(world_server::instance().get_io_service());
							pfix->init_task(vs[2],  it->second);
						}
						return;
					}

					if(custom == 0)
						cl->get_sys<game_sys_recharge>()->payment_once(payid, rmb, false, m_payment_lottery);
					else
						cl->get_sys<game_sys_recharge>()->payment_once(vs[2], custom, payid, rmb);

					return;
				}
			}
			else if(m_check_count == 2)//只有第1次提示信息
			{				
				auto cl = m_client.lock();
				if(!cl)
				{			
					SLOG_ERROR << "payment_task player error, orderid:"<<m_orderid;
					return;
				}

				auto sendmsg = PACKET_CREATE(packetw2c_ask_check_payment_result, e_mst_w2c_ask_check_payment_result);
				sendmsg->set_result(false);
				cl->send_msg_to_client(sendmsg);							
			}
		}
		else
		{
			SLOG_ERROR << "payment_task:" << resposeInfo;
		}

		if(m_check_count < 10)
		{
			m_check_count++;				
			m_retinfo.clear();			
			auto task = CONVERT_POINT(payment_task, shared_from_this());
			world_server::instance().add_server_timer(bind(&payment_task::check_more_time, task), 3*m_check_count);
		}				
	}
	else
	{
		SLOG_ERROR << "payment_task false";
	}

	__LEAVE_FUNCTION;
}

void  payment_task::check_more_time()
{
	post_request(m_req);
}


