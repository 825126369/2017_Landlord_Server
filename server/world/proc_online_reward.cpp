#include "stdafx.h"
#include "proc_online_reward.h"
#include "game_player.h"
#include "online_reward_mgr.h"
#include "pump_type.h"
#include "pump_sys.h"
#include "global_sys_mgr.h"
#include "game_sys_recharge.h"

using namespace boost;

void initOnlineRewardPacket()
{
	packetc2w_receive_online_reward_factory::regedit_factory();
	packetw2c_receive_online_reward_result_factory::regedit_factory();

	packetc2w_receive_recharge_reward_factory::regedit_factory();
	packetw2c_receive_recharge_reward_result_factory::regedit_factory();

	packetc2w_req_online_reward_info_factory::regedit_factory();
	packetw2c_req_online_reward_info_result_factory::regedit_factory();
}

// 领取在线奖励
bool packetc2w_receive_online_reward_factory::packet_process(shared_ptr<world_peer> peer, 
											boost::shared_ptr<game_player> player, 
											shared_ptr<packetc2w_receive_online_reward> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	auto sendmsg = PACKET_CREATE(packetw2c_receive_online_reward_result, e_mst_w2c_receive_online_reward_result);
	auto mgr = player->get_sys<OnlineRewardMgr>();
	int result = mgr->receiveReward();
	sendmsg->set_result(result);
	sendmsg->set_rewardid(mgr->getRewardId());
	if(result != msg_type_def::e_rmt_success)
	{
		//sendmsg->set_remaintime(mgr->getRemainTime());
	}
	else
	{
		GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_online_reward);
	}
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 领取充值奖励
bool packetc2w_receive_recharge_reward_factory::packet_process(shared_ptr<world_peer> peer, 
															   boost::shared_ptr<game_player> player, 
															   shared_ptr<packetc2w_receive_recharge_reward> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_receive_recharge_reward_result, e_mst_w2c_receive_recharge_reward_result);
	auto mgr = player->get_sys<game_sys_recharge>();
	int result = mgr->recvRechargeReward();
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求在线奖励的领取信息
bool packetc2w_req_online_reward_info_factory::packet_process(shared_ptr<world_peer> peer, 
															  boost::shared_ptr<game_player> player, 
															  shared_ptr<packetc2w_req_online_reward_info> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_online_reward_info_result, e_mst_w2c_req_online_reward_info_result);
	auto mgr = player->get_sys<OnlineRewardMgr>();
	int count = mgr->getRecvCount();
	auto idList = sendmsg->mutable_recvidlist();

	for(int i = 0; i < count; i++)
	{
		OnlineRewardItem *pItem = mgr->getOnlineRewardItem(i);
		if(pItem)
		{
			if(pItem->m_isReceive->get_value())
			{
				auto th = idList->Add();
				*th = pItem->m_id->get_value();
			}
		}
	}

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
