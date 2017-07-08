#include "stdafx.h"
#include "proc_rank.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "game_rank_sys.h"

using namespace boost;

void initRankPacket()
{
	packetc2w_req_coin_rank_factory::regedit_factory();
	packetw2c_req_coin_rank_result_factory::regedit_factory();
	packetc2w_req_recharge_rank_factory::regedit_factory();
	packetw2c_req_recharge_rank_result_factory::regedit_factory();

	packetc2w_req_coin_growth_factory::regedit_factory();
	packetw2c_req_coin_growth_result_factory::regedit_factory();
}

// 请求金币排行
bool packetc2w_req_coin_rank_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_req_coin_rank> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_coin_rank_result, e_mst_w2c_req_coin_rank_result);
	std::vector<stRankInfo> rankList;
	int selfRank = 0;
	GLOBAL_SYS(GameRankSys)->getCoinRankList(player.get(), rankList, selfRank);
	auto ptr = sendmsg->mutable_ranklist();
	ptr->Reserve(rankList.size());
	for(auto it = rankList.begin(); it != rankList.end(); ++it)
	{
		auto item = ptr->Add();
		item->set_playerid(it->m_playerId);
		item->set_nickname(it->m_nickName);
		item->set_gold(it->m_gold);
		item->set_viplevel(it->m_vipLevel);
	}
	sendmsg->set_selfrank(selfRank);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

bool packetc2w_req_recharge_rank_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_req_recharge_rank> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_recharge_rank_result, e_mst_w2c_req_recharge_rank_result);
	sendmsg->set_is_yesterday(msg->is_yesterday());

	std::vector<stRankInfo> rankList;
	int selfRank = 0;
	int selfrmb = 0;
	GLOBAL_SYS(GameRankSys)->getRechargeRankList(player.get(), rankList, selfRank, selfrmb, msg->is_yesterday());
	auto ptr = sendmsg->mutable_ranklist();
	ptr->Reserve(rankList.size());
	for(auto it = rankList.begin(); it != rankList.end(); ++it)
	{
		auto item = ptr->Add();
		item->set_playerid(it->m_playerId);
		item->set_nickname(it->m_nickName);
		item->set_rmb(it->m_gold);
		item->set_viplevel(it->m_vipLevel);
	}
	sendmsg->set_selfrank(selfRank);
	sendmsg->set_selfrmb(selfrmb);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求金币增长排行
bool packetc2w_req_coin_growth_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_req_coin_growth> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_coin_growth_result, e_mst_w2c_req_coin_growth_result);
	std::vector<stRankInfo> rankList;
	int selfRank = 0;
	GOLD_TYPE selfGold = 0;
	GLOBAL_SYS(GameRankSys)->getCoinGrowthList(player.get(), rankList, selfRank, selfGold);
	auto ptr = sendmsg->mutable_ranklist();
	ptr->Reserve(rankList.size());
	for(auto it = rankList.begin(); it != rankList.end(); ++it)
	{
		auto item = ptr->Add();
		item->set_playerid(it->m_playerId);
		item->set_nickname(it->m_nickName);
		item->set_gold(it->m_gold);
		item->set_viplevel(it->m_vipLevel);
	}
	sendmsg->set_selfrank(selfRank);
	sendmsg->set_selfgold(selfGold);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
