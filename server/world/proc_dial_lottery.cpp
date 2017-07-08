#include "stdafx.h"
#include "proc_dial_lottery.h"
#include "game_player.h"
#include "dial_lottery_mgr.h"
#include "time_helper.h"

using namespace boost;

void initDialLotteryPacket()
{
	packetc2w_req_dial_lottery_factory::regedit_factory();
	packetw2c_req_dial_lottery_result_factory::regedit_factory();
	packetc2w_req_dial_lottery_flag_factory::regedit_factory();
	packetw2c_req_dial_lottery_flag_result_factory::regedit_factory();
	packetc2w_req_month_card_reward_factory::regedit_factory();
	packetw2c_req_month_card_reward_result_factory::regedit_factory();
}

// 请求转盘抽奖
bool packetc2w_req_dial_lottery_factory::packet_process(shared_ptr<world_peer> peer, 
														boost::shared_ptr<game_player> player, 
														shared_ptr<packetc2w_req_dial_lottery> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_dial_lottery_result, e_mst_w2c_req_dial_lottery_result);
	auto mgr = player->get_sys<DialLotteryMgr>();
	int result = mgr->doSign(time_helper::instance().get_cur_time());
	if(result == msg_type_def::e_rmt_success)
	{
		stSignResult *pResult = mgr->getSignResult();
		sendmsg->set_num(pResult->m_dialNum);
		sendmsg->set_hasmonthcardreward(pResult->m_hasMonthCardReward);
	}
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求转盘标志
bool packetc2w_req_dial_lottery_flag_factory::packet_process(shared_ptr<world_peer> peer, 
															 boost::shared_ptr<game_player> player, 
															 shared_ptr<packetc2w_req_dial_lottery_flag> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_dial_lottery_flag_result, e_mst_w2c_req_dial_lottery_flag_result);
	auto mgr = player->get_sys<DialLotteryMgr>();
	bool res = mgr->isLotteryToday();
	sendmsg->set_islotterytoday(res);

	boost::gregorian::date curDate = time_helper::instance().get_cur_date();
	int count = mgr->getHasSignCount(curDate);
	sendmsg->set_hassigncount(count);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 领取月卡奖励
bool packetc2w_req_month_card_reward_factory::packet_process(shared_ptr<world_peer> peer, 
															 boost::shared_ptr<game_player> player, 
															 shared_ptr<packetc2w_req_month_card_reward> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_month_card_reward_result, e_mst_w2c_req_month_card_reward_result);
	auto mgr = player->get_sys<DialLotteryMgr>();
	int result = mgr->receiveMonthCardReward(time_helper::instance().get_cur_time());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
