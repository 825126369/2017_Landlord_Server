#include "stdafx.h"
#include "proc_exchange.h"
#include "game_player.h"
#include "exchange_sys.h"
#include "global_sys_mgr.h"

using namespace boost;

void initExchangePacket()
{
	packetc2w_exchange_factory::regedit_factory();
	packetw2c_exchange_result_factory::regedit_factory();
	packetc2w_get_exchange_state_factory::regedit_factory();
	packetw2c_get_exchange_state_result_factory::regedit_factory();
	packetc2w_shopping_factory::regedit_factory();
	packetw2c_shopping_result_factory::regedit_factory();
}

// ÇëÇó¶Ò»»
bool packetc2w_exchange_factory::packet_process(shared_ptr<world_peer> peer, 
												boost::shared_ptr<game_player> player, 
												shared_ptr<packetc2w_exchange> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	auto sendmsg = PACKET_CREATE(packetw2c_exchange_result, e_mst_w2c_exchange_result);
	int result = GLOBAL_SYS(ExchangeSys)->exchange(player.get(), msg->exchangeid(), msg->phone());
	sendmsg->set_exchangeid(msg->exchangeid());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// È¡µÃ¶Ò»»×´Ì¬
bool packetc2w_get_exchange_state_factory::packet_process(shared_ptr<world_peer> peer, 
														  boost::shared_ptr<game_player> player, 
														  shared_ptr<packetc2w_get_exchange_state> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_get_exchange_state_result, e_mst_w2c_get_exchange_state_result);
	std::vector<stExchangeInfo> infoList;
	GLOBAL_SYS(ExchangeSys)->getExchangeList(player.get(), infoList);
	if(!infoList.empty())
	{
		auto pInfoList = sendmsg->mutable_infolist();
		pInfoList->Reserve(infoList.size());
		for(auto it = infoList.begin(); it != infoList.end(); ++it)
		{
			auto pInfo = pInfoList->Add();
			pInfo->set_time(it->m_genTime);
			pInfo->set_chgid(it->m_chgId);
			pInfo->set_isreceive(it->m_isReceive);
		}
	}
	
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


// ¹ºÎï
#include "game_sys_recharge.h"
bool packetc2w_shopping_factory::packet_process(shared_ptr<world_peer> peer, 
												boost::shared_ptr<game_player> player, 
												shared_ptr<packetc2w_shopping> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_shopping_result, e_mst_w2c_shopping_result);
	sendmsg->set_shopid(msg->shopid());

	auto rsys = player->get_sys<game_sys_recharge>();
	sendmsg->set_result(rsys->shopping(msg->shopid()));

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;

}