#include "stdafx.h"
#include "friend_mgr.h"
#include "enable_object_pool.h"
#include "game_player.h"
#include "msg_type_def.pb.h"
#include "friend_def.h"
#include "M_BaseInfoCFG.h"
#include "game_player_mgr.h"
#include "game_db.h"
#include "game_sys_recharge.h"
#include "gift_def.h"

void FriendMgr::init_sys_object()
{
	m_friends = get_game_player()->regedit_mapfield("friends", FriendMap::malloc());
	m_friendsPtr = m_friends->get_Tmap<FriendMap>();
}

bool FriendMgr::sys_load()
{
	m_friendsPtr->setPlayerId(get_game_player()->PlayerId->get_value());
	return true;
}

static mongo::BSONObj g_retHasPlayerField = BSON("player_id" << 1);

int FriendMgr::addFriend(int friendId)
{
	auto pFriend = m_friendsPtr->find_Tobj<FriendItem>(friendId);
	if(pFriend)
		return msg_type_def::e_rmt_exists_friend;

	if(friendId == get_game_player()->PlayerId->get_value())
		return msg_type_def::e_rmt_cannot_add_self;

	int maxCount = 50;
	static const M_BaseInfoCFGData* data = M_BaseInfoCFG::GetSingleton()->GetData("friendLimit");
	if(data)
	{
		maxCount = data->mValue;
	}
	
	if((int)m_friendsPtr->get_obj_count() >= maxCount)
		return msg_type_def::e_rmt_friend_full;

	auto target = game_player_mgr::instance().findPlayerById(friendId);
	if(target == nullptr)
	{
		 mongo::BSONObj cond = BSON("player_id" << friendId);
		 mongo::BSONObj obj = db_player::instance().findone(DB_PLAYER_INFO, cond, &g_retHasPlayerField);
		 if(obj.isEmpty())
			 return msg_type_def::e_rmt_player_not_exists;
	}

	auto pInfo = FriendItem::malloc();
	pInfo->m_friendId->set_value(friendId);
	m_friendsPtr->put_obj(pInfo);
	m_friendsPtr->db_add(pInfo);
	return msg_type_def::e_rmt_success;
}

int FriendMgr::removeFriend(int friendId)
{
	auto pFriend = m_friendsPtr->find_Tobj<FriendItem>(friendId);
	if(!pFriend)
		return msg_type_def::e_rmt_player_not_exists;

	m_friendsPtr->del_obj(friendId);
	m_friendsPtr->db_del(pFriend);
	return msg_type_def::e_rmt_success;
}

void FriendMgr::getFriendList(std::vector<stFriendInfo>& friendList)
{
	for(auto it = m_friendsPtr->begin(); it != m_friendsPtr->end(); ++it)
	{
		auto ptr = CONVERT_POINT(FriendItem, it->second);
		stFriendInfo info;
		bool res = _getFriendInfo(ptr->m_friendId->get_value(), info);
		if(res)
		{
			friendList.push_back(info);
		}
	}
}

bool FriendMgr::getFriendInfo(int friendId, stFriendInfo& info)
{
	auto pFriend = m_friendsPtr->find_Tobj<FriendItem>(friendId);
	if(!pFriend)
		false;

	return _getFriendInfo(friendId, info);
}

bool FriendMgr::hasFriend(int friendId)
{
	auto pFriend = m_friendsPtr->find_Tobj<FriendItem>(friendId);
	return pFriend != nullptr;
}

int FriendMgr::searchPlayer(int playerId, stFriendInfo& info)
{
	bool res = _getFriendInfo(playerId, info);
	if(!res)
		return msg_type_def::e_rmt_player_not_exists;

	return msg_type_def::e_rmt_success;
}

static mongo::BSONObj g_retField = BSON("sex" << 1 
										<< "VipLevel" << 1 
										<< "icon_custom" << 1 
										<< "nickname" << 1 
										<< "PhotoFrameId" << 1 
										<< "selfSignature" << 1 
										<< "gifts" << 1 
										<< "sendGiftCoinCount" << 1
										<< "gold" << 1);

// 经典捕鱼返回等级字段
static mongo::BSONObj g_retFishField = BSON("Level" << 1);

bool FriendMgr::_getFriendInfo(int friendId, stFriendInfo& info)
{
	bool online = false;
	auto pFriend = game_player_mgr::instance().findPlayerById(friendId);
	if(pFriend == nullptr)
	{
		mongo::BSONObj cond = BSON("player_id" << friendId);
		mongo::BSONObj obj = db_player::instance().findone(DB_PLAYER_INFO, cond, &g_retField);
		if(obj.isEmpty())
			return false;

		info.m_sex = obj.getIntField("sex");
		info.m_vipLevel = obj.getIntField("VipLevel");
		info.m_nickName = obj.getStringField("nickname");
		info.m_iconCustom = obj.getStringField("icon_custom");
		info.m_photoFrameId = obj.getIntField("PhotoFrameId");
		info.m_selfSignature = obj.getStringField("selfSignature");

		if(obj.hasField("gifts"))
		{
			auto vec = obj.getField("gifts").Array();
			for (auto it = vec.begin(); it != vec.end(); ++it)
			{
				mongo::BSONObj& tb = it->Obj();
				int giftId = tb.getIntField("giftId");
				int count = tb.getIntField("count");
				info.m_giftList.push_back(stGift(giftId, count));
			}
		}
		
		info.m_sendGiftCoinCount = obj.getField("sendGiftCoinCount").Long();
		info.m_gold = obj.getField("gold").Long();
	}
	else
	{
		online = true;
		info.m_sex = pFriend->Sex->get_value();
		info.m_vipLevel = pFriend->get_viplvl();
		info.m_nickName = pFriend->NickName->get_string();
		info.m_iconCustom = pFriend->IconCustom->get_string();
		info.m_photoFrameId = pFriend->PhotoFrameId->get_value();
		info.m_selfSignature = pFriend->SelfSignature->get_string();

		for(auto git = pFriend->m_giftStatPtr->begin(); git != pFriend->m_giftStatPtr->end(); ++git)
		{
			auto ptr = CONVERT_POINT(GiftInfo, git->second);
			info.m_giftList.push_back(stGift(ptr->m_giftId->get_value(), ptr->m_count->get_value()));
		}

		info.m_sendGiftCoinCount = pFriend->SendGiftCoinCount->get_value();
		info.m_gold = pFriend->Gold->get_value();
	}

	info.m_friendId = friendId;
	info.m_isOnLine = online;

	mongo::BSONObj condFriend = BSON("player_id" << friendId);
	mongo::BSONObj fishFriend = db_game::instance().findone(DB_FISHLORD, condFriend, &g_retFishField);
	if(fishFriend.isEmpty()) 
	{
		info.m_fishLevel = 0;
	}
	else
	{
		info.m_fishLevel = fishFriend.getIntField("Level");
	}

	return true;
}
