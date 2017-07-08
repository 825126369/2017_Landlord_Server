#include "stdafx.h"
#include "proc_friend.h"
#include "game_player.h"
#include "friend_mgr.h"
#include "msg_type_def.pb.h"
#include "friend_def.h"
#include "game_player_mgr.h"
#include "proc_logic2world_friend.h"
#include "game_sys_recharge.h"
using namespace boost;

void initFriendPacket()
{
	packetc2w_add_friend_factory::regedit_factory();
	packetw2c_add_friend_result_factory::regedit_factory();
	packetc2w_remove_friend_factory::regedit_factory();
	packetw2c_remove_friend_result_factory::regedit_factory();
	packetc2w_req_friend_list_factory::regedit_factory();
	packetw2c_req_friend_list_result_factory::regedit_factory();
	packetc2w_search_friend_factory::regedit_factory();
	packetw2c_search_friend_result_factory::regedit_factory();
	packetc2w_enter_friend_room_factory::regedit_factory();
	packetw2c_enter_friend_room_result_factory::regedit_factory();
	packetc2w_get_friend_gameid_factory::regedit_factory();
	packetw2c_get_friend_gameid_result_factory::regedit_factory();
}

// 添加好友
bool packetc2w_add_friend_factory::packet_process(shared_ptr<world_peer> peer, 
												  boost::shared_ptr<game_player> player, 
												  shared_ptr<packetc2w_add_friend> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_add_friend_result, e_mst_w2c_add_friend_result);
	auto mgr = player->get_sys<FriendMgr>();
	int result = mgr->addFriend(msg->friendid());
	if(result == msg_type_def::e_rmt_success)
	{
		stFriendInfo info;
		bool res = mgr->getFriendInfo(msg->friendid(), info);
		if(res)
		{
			auto pInfo = sendmsg->mutable_info();
			pInfo->set_friendid(info.m_friendId);
			pInfo->set_sex(info.m_sex);
			pInfo->set_online(info.m_isOnLine);
			pInfo->set_viplevel(info.m_vipLevel);
			pInfo->set_nickname(info.m_nickName);
			pInfo->set_iconcustom(info.m_iconCustom);
			pInfo->set_photoframeid(info.m_photoFrameId);
			pInfo->set_selfsignature(info.m_selfSignature);

			if(!info.m_giftList.empty())
			{
				auto pGift = pInfo->mutable_giftlist();
				pGift->Reserve(info.m_giftList.size());
				for(auto it = info.m_giftList.begin(); it != info.m_giftList.end(); ++it)
				{
					auto pItem = pGift->Add();
					pItem->set_giftid(it->m_giftId);
					pItem->set_count(it->m_count);
				}
			}
			pInfo->set_giftcoincount(info.m_sendGiftCoinCount);
			pInfo->set_gold(info.m_gold);
			pInfo->set_fishlevel(info.m_fishLevel);
		}
	}

	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 移除好友
bool packetc2w_remove_friend_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_remove_friend> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_remove_friend_result, e_mst_w2c_remove_friend_result);
	int result = player->get_sys<FriendMgr>()->removeFriend(msg->friendid());
	sendmsg->set_friendid(msg->friendid());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

bool packetc2w_req_friend_list_factory::packet_process(shared_ptr<world_peer> peer, 
												  boost::shared_ptr<game_player> player, 
												  shared_ptr<packetc2w_req_friend_list> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_friend_list_result, e_mst_w2c_req_friend_list_result);
	std::vector<stFriendInfo> friendList;
	player->get_sys<FriendMgr>()->getFriendList(friendList);
	auto pList = sendmsg->mutable_friendlist();
	pList->Reserve(friendList.size());

	for(auto it = friendList.begin(); it != friendList.end(); ++it)
	{
		auto pInfo = pList->Add();
		pInfo->set_friendid(it->m_friendId);
		pInfo->set_sex(it->m_sex);
		pInfo->set_online(it->m_isOnLine);
		pInfo->set_viplevel(it->m_vipLevel);
		pInfo->set_nickname(it->m_nickName);
		pInfo->set_iconcustom(it->m_iconCustom);

		pInfo->set_photoframeid(it->m_photoFrameId);
		pInfo->set_selfsignature(it->m_selfSignature);

		if(!it->m_giftList.empty())
		{
			auto pGift = pInfo->mutable_giftlist();
			pGift->Reserve(it->m_giftList.size());
			for(auto git = it->m_giftList.begin(); git != it->m_giftList.end(); ++git)
			{
				auto pItem = pGift->Add();
				pItem->set_giftid(git->m_giftId);
				pItem->set_count(git->m_count);
			}
		}
		pInfo->set_giftcoincount(it->m_sendGiftCoinCount);
		pInfo->set_gold(it->m_gold);
		pInfo->set_fishlevel(it->m_fishLevel);
	}

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 搜索好友
bool packetc2w_search_friend_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_search_friend> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_search_friend_result, e_mst_w2c_search_friend_result);
	stFriendInfo info;
	int result = player->get_sys<FriendMgr>()->searchPlayer(msg->playerid(), info);
	if(result == msg_type_def::e_rmt_success)
	{
		auto ptr = sendmsg->mutable_info();
		ptr->set_friendid(info.m_friendId);
		ptr->set_sex(info.m_sex);
		ptr->set_online(info.m_isOnLine);
		ptr->set_viplevel(info.m_vipLevel);
		ptr->set_nickname(info.m_nickName);
		ptr->set_iconcustom(info.m_iconCustom);
		ptr->set_photoframeid(info.m_photoFrameId);
		ptr->set_selfsignature(info.m_selfSignature);

		if(!info.m_giftList.empty())
		{
			auto pGift = ptr->mutable_giftlist();
			pGift->Reserve(info.m_giftList.size());
			for(auto git = info.m_giftList.begin(); git != info.m_giftList.end(); ++git)
			{
				auto pItem = pGift->Add();
				pItem->set_giftid(git->m_giftId);
				pItem->set_count(git->m_count);
			}
		}
		ptr->set_giftcoincount(info.m_sendGiftCoinCount);
		ptr->set_gold(info.m_gold);
		ptr->set_fishlevel(info.m_fishLevel);
	}
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 进入好友房间
bool packetc2w_enter_friend_room_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_enter_friend_room> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_enter_friend_room_result, e_mst_w2c_enter_friend_room_result);
	bool hasFriend = player->get_sys<FriendMgr>()->hasFriend(msg->friendid());

	if(!hasFriend) // 不存在该好友
	{
		sendmsg->set_result(msg_type_def::e_rmt_unknow);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	if(player->is_gaming())
	{
		sendmsg->set_result(msg_type_def::e_rmt_unknow);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	auto pFriend = game_player_mgr::instance().findPlayerById(msg->friendid());
	if(pFriend == nullptr) // 好友离线
	{
		sendmsg->set_result(msg_type_def::e_rmt_friend_offline);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	if(!pFriend->is_gaming())
	{
		sendmsg->set_result(msg_type_def::e_rmt_not_in_game);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	if(pFriend->check_logic())
	{
		player->setGameIdServerId(pFriend->get_gameid(), pFriend->get_logicid());
		auto peerLogic = pFriend->get_logic();
		auto toLogicMsg = PACKET_CREATE(packetw2l_enter_friend_room, e_mst_w2l_enter_friend_room);
		toLogicMsg->set_playerid(player->PlayerId->get_value());
		toLogicMsg->set_sessionid(player->get_sessionid());
		toLogicMsg->set_friendid(msg->friendid());
		toLogicMsg->set_gameid(pFriend->get_gameid());

		auto ainfo = toLogicMsg->mutable_account_info();
		ainfo->set_aid(player->PlayerId->get_value());
		ainfo->set_gold(player->Gold->get_value());
		ainfo->set_nickname(player->NickName->get_string());
		//ainfo->set_icon_id(player->IconId->get_value());
		ainfo->set_sex(player->Sex->get_value());
		ainfo->set_curphotoframeid(player->PhotoFrameId->get_value());
		ainfo->set_icon_custom(player->IconCustom->get_string());
		ainfo->set_ticket(player->Ticket->get_value());		
		ainfo->set_viplvl(player->get_viplvl());
		ainfo->set_experience_vip(player->ExperienceVIP->get_value());
		ainfo->set_create_time(player->CreateTime->get_value());
		ainfo->set_monthcard_time(player->get_sys<game_sys_recharge>()->VipCardEndTime->get_value());
		ainfo->set_privilege(player->Privilege->get_value());

		auto ainfoex = toLogicMsg->mutable_account_info_ex();
		ainfoex->set_lucky(player->Lucky->get_value());
		ainfoex->set_temp_income(player->TempIncome->get_value());
		ainfoex->set_total_income(player->TotalIncome->get_value());

		peerLogic->send_msg(toLogicMsg);
	}
	else
	{
		sendmsg->set_result(msg_type_def::e_rmt_unknow);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


// 进入好友房间
bool packetc2w_get_friend_gameid_factory::packet_process(shared_ptr<world_peer> peer, 
														 boost::shared_ptr<game_player> player, 
														 shared_ptr<packetc2w_get_friend_gameid> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_get_friend_gameid_result, e_mst_w2c_get_friend_gameid_result);
	bool hasFriend = player->get_sys<FriendMgr>()->hasFriend(msg->friendid());

	if(!hasFriend) // 不存在该好友
	{
		sendmsg->set_result(msg_type_def::e_rmt_unknow);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	if(player->is_gaming())
	{
		sendmsg->set_result(msg_type_def::e_rmt_unknow);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	auto pFriend = game_player_mgr::instance().findPlayerById(msg->friendid());
	if(pFriend == nullptr) // 好友离线
	{
		sendmsg->set_result(msg_type_def::e_rmt_friend_offline);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	if(!pFriend->is_gaming())
	{
		sendmsg->set_result(msg_type_def::e_rmt_not_in_game);
		peer->send_msg_to_client(player->get_sessionid(), sendmsg);
		return true;
	}

	sendmsg->set_result(msg_type_def::e_rmt_success);
	sendmsg->set_gameid(pFriend->get_gameid());
	sendmsg->set_friendid(msg->friendid());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}