#include "stdafx.h"
#include "proc_star_lottery.h"
#include "game_player.h"
#include "star_lottery_mgr.h"
#include "time_helper.h"

using namespace boost;

void initStarLotteryPacket()
{
	packetc2w_star_lottery_info_factory::regedit_factory();
	packetw2c_star_lottery_info_result_factory::regedit_factory();
	packetc2w_req_star_lottery_factory::regedit_factory();
	packetc2w_req_star_lottery_result_factory::regedit_factory();
}

// 请求转盘抽奖
bool packetc2w_star_lottery_info_factory::packet_process(shared_ptr<world_peer> peer, 
														boost::shared_ptr<game_player> player, 
														shared_ptr<packetc2w_star_lottery_info> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_star_lottery_info_result, e_mst_w2c_star_lottery_info_result);
	auto mgr = player->get_sys<StarLotteryMgr>();
	sendmsg->set_award(mgr->CurAward->get_value());
	sendmsg->set_star(mgr->CurStar->get_value());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求转盘标志
bool packetc2w_req_star_lottery_factory::packet_process(shared_ptr<world_peer> peer, 
															 boost::shared_ptr<game_player> player, 
															 shared_ptr<packetc2w_req_star_lottery> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetc2w_req_star_lottery_result, e_mst_c2w_req_star_lottery_result);
	auto mgr = player->get_sys<StarLotteryMgr>();
	
	stStarResult sr;
	if(mgr->Lottery(sr))
	{
		sendmsg->set_result(msg_type_def::e_rmt_success);
		sendmsg->set_award(sr.award);
		sendmsg->set_itemtype(sr.itemtype);
		sendmsg->set_itemcount(sr.itemcount);
	}
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
