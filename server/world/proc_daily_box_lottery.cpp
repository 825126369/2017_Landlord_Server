#include "stdafx.h"
#include "proc_daily_box_lottery.h"
#include "game_player.h"
#include "daily_box_lottery_mgr.h"

using namespace boost;

void initDailyBoxLotteryPacket()
{
	packetc2w_req_lottery_box_factory::regedit_factory();
	packetw2c_req_lottery_box_result_factory::regedit_factory();
	packetc2w_thankyou_exchange_ticket_factory::regedit_factory();
	packetw2c_thankyou_exchange_ticket_result_factory::regedit_factory();
}

// «Î«Û±¶œ‰≥ÈΩ±
bool packetc2w_req_lottery_box_factory::packet_process(shared_ptr<world_peer> peer, 
														boost::shared_ptr<game_player> player, 
														shared_ptr<packetc2w_req_lottery_box> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_lottery_box_result, e_mst_w2c_req_lottery_box_result);
	int result = 0;
	auto mgr = player->get_sys<DailyBoxLotteryMgr>();
	if(msg->useticket())
	{
		result = mgr->doBoxLotteryWithTicket(msg->boxindex());
	}
	else
	{
		result = mgr->doBoxLottery(msg->boxindex());
	}
	sendmsg->set_result(result);

	if(result == msg_type_def::e_rmt_success)
	{
		sendmsg->set_useticket(msg->useticket());
		stBoxLotteryResult * pRes = mgr->getBoxLotteryResult();
		sendmsg->set_rewardtype(pRes->m_resultType);
	}
	
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// –ª–ª≤Œ”Î∂“ªª¿Ò»Ø
bool packetc2w_thankyou_exchange_ticket_factory::packet_process(shared_ptr<world_peer> peer, 
																boost::shared_ptr<game_player> player, 
																shared_ptr<packetc2w_thankyou_exchange_ticket> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_thankyou_exchange_ticket_result, e_mst_w2c_thankyou_exchange_ticket_result);
	auto mgr = player->get_sys<DailyBoxLotteryMgr>();
	int result = mgr->exchangeTicket();
	sendmsg->set_result(result);

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
