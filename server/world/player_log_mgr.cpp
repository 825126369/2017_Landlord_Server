#include "stdafx.h"
#include "player_log_mgr.h"
#include "game_player.h"
#include "msg_type_def.pb.h"
#include "time_helper.h"
#include "player_log_def.h"
#include "game_player_mgr.h"
#include "game_db.h"
#include <client2world_player_property.pb.h>

struct stPlayerInfo
{
	std::string m_nickName;
};

void PlayerLogMgr::init_sys_object()
{
	auto player = get_game_player();
	m_sendList = player->regedit_arrfield("sendGiftLog", SendGiftLogInfoArray::malloc());
	m_mailList = player->regedit_arrfield("sendMailLog", SendMailLogInfoArray::malloc());
	m_safeBoxList = player->regedit_arrfield("safeBoxLog", SafeBoxLogInfoArray::malloc());
}

bool PlayerLogMgr::sys_load()
{
	auto player = get_game_player();
	m_sendList->get_Tarray<SendGiftLogInfoArray>()->setPlayer(player->PlayerId->get_value());
	m_mailList->get_Tarray<SendMailLogInfoArray>()->setPlayer(player->PlayerId->get_value());
	m_safeBoxList->get_Tarray<SafeBoxLogInfoArray>()->setPlayer(player->PlayerId->get_value());
	return true;
}

int PlayerLogMgr::getSendGiftLog(time_t minT, time_t& Last, std::deque<stSendGiftLogInfo>& res)
{
	time_t maxt = minT;

	std::vector<GObjPtr>& pArray = m_sendList->get_Tarray<SendGiftLogInfoArray>()->getAll();

	stSendGiftLogInfo info;
	for(auto it = pArray.rbegin(); it != pArray.rend(); ++it)
	{
		auto p = CONVERT_POINT(SendGiftLogInfo, *it);

		if(p->m_sendTime->get_value() > minT)
		{
			info.m_sendTime = p->m_sendTime->get_value();
			info.m_friendId = p->m_friendId->get_value();
			info.m_friendNickName = p->m_friendNickName->get_string();

			info.m_giftId = p->m_giftId->get_value();
			info.m_count = p->m_count->get_value();
			info.m_sendgold = p->m_sendgold->get_value();
			info.m_mailid = p->m_mailId->get_string();

			res.push_front(info);

			if(p->m_sendTime->get_value() > maxt)
			{
				maxt = p->m_sendTime->get_value();
			}
		}
		else
		{
			break;
		}
	}
	
	Last = maxt;

	return msg_type_def::e_rmt_success;
}

int PlayerLogMgr::getSafeBoxLog(time_t start_time, boost::shared_ptr<client2world_protocols::packetw2c_req_safebox_log_result>& msg)
{
	time_t maxt = start_time;

	std::vector<GObjPtr>& pArray = m_safeBoxList->get_Tarray<SafeBoxLogInfoArray>()->getAll();
	for(auto it = pArray.rbegin(); it != pArray.rend(); ++it)
	{
		auto p = CONVERT_POINT(SafeBoxLogInfo, *it);

		if(p->m_time->get_value() > start_time)
		{
			auto log = msg->add_loglist();
			log->set_time(p->m_time->get_value());
			log->set_gold(p->m_gold->get_value());
			log->set_player_gold(p->m_player_gold->get_value());
			if(p->m_time->get_value() > maxt)
			{
				maxt = p->m_time->get_value();
			}
		}
	}
	msg->set_lasttime(maxt);
	return msg_type_def::e_rmt_success;
}

int PlayerLogMgr::getSendMailLog(time_t minT, time_t& Last, std::deque<stSendMailLogInfo>& res)
{
	time_t maxt = minT;

	std::vector<GObjPtr>& pArray = m_mailList->get_Tarray<SendMailLogInfoArray>()->getAll();

	stSendMailLogInfo info;
	for(auto it = pArray.rbegin(); it != pArray.rend(); ++it)
	{
		auto p = CONVERT_POINT(SendMailLogInfo, *it);

		if(p->m_sendTime->get_value() > minT)
		{
			info.m_sendTime = p->m_sendTime->get_value();
			info.m_friendId = p->m_friendId->get_value();
			info.m_title = p->m_title->get_string();
			info.m_content = p->m_content->get_string();
			info.m_friendNickName = p->m_friendNickName->get_string();
			info.m_mailId = p->m_mailId->get_string();

			res.push_front(info);

			if(p->m_sendTime->get_value() > maxt)
			{
				maxt = p->m_sendTime->get_value();
			}
		}
		else
		{
			break;
		}
	}

	Last = maxt;

	return msg_type_def::e_rmt_success;
}

int PlayerLogMgr::addSendGiftLog(int playerId, int giftId, GOLD_TYPE giftCount, const std::string& mailId, bool send, int maxLogCount)
{
	stPlayerInfo playerInfo;
	bool res = _getPlayerInfo(playerId, playerInfo);
	if(!res)
		return msg_type_def::e_rmt_unknow;

	auto logInfo = SendGiftLogInfo::malloc();
	logInfo->m_sendTime->set_value(time_helper::instance().get_cur_time());
	logInfo->m_friendId->set_value(playerId);
	logInfo->m_friendNickName->set_string(playerInfo.m_nickName);
	logInfo->m_giftId->set_value(giftId);
	logInfo->m_count->set_value(giftCount);
	logInfo->m_mailId->set_string(mailId);
	logInfo->m_sendgold->set_value(send);

	auto pArray = m_sendList->get_Tarray<SendGiftLogInfoArray>();
	if(pArray->get_obj_count() >= maxLogCount)
	{
		pArray->db_pop();
	}

	pArray->put_obj(logInfo);
	pArray->db_push(logInfo);

	return msg_type_def::e_rmt_success;
}

int PlayerLogMgr::addSafeBoxLog(GOLD_TYPE gold, GOLD_TYPE playerGold, int maxLogCount)
{
	auto logInfo = SafeBoxLogInfo::malloc();
	logInfo->m_time->set_value(time_helper::instance().get_cur_time());
	logInfo->m_gold->set_value(gold);
	logInfo->m_player_gold->set_value(playerGold);

	auto pArray = m_safeBoxList->get_Tarray<SafeBoxLogInfoArray>();
	if(pArray->get_obj_count() >= maxLogCount)
	{
		pArray->db_pop();
	}

	pArray->put_obj(logInfo);
	pArray->db_push(logInfo);

	return msg_type_def::e_rmt_success;
}

int PlayerLogMgr::addSendMailLog(int friendId, 
								 const std::string& title,
								 const std::string& content, 
								 const std::string& mailId,
								 int maxLogCount)
{
	stPlayerInfo playerInfo;
	bool res = _getPlayerInfo(friendId, playerInfo);
	if(!res)
		return msg_type_def::e_rmt_unknow;

	auto logInfo = SendMailLogInfo::malloc();
	logInfo->m_sendTime->set_value(time_helper::instance().get_cur_time());
	logInfo->m_friendId->set_value(friendId);
	logInfo->m_title->set_string(title);
	logInfo->m_content->set_string(content);
	logInfo->m_mailId->set_string(mailId);
	logInfo->m_friendNickName->set_string(playerInfo.m_nickName);

	auto pArray = m_mailList->get_Tarray<SendMailLogInfoArray>();
	if(pArray->get_obj_count() >= maxLogCount)
	{
		pArray->db_pop();
	}

	pArray->put_obj(logInfo);
	pArray->db_push(logInfo);

	return msg_type_def::e_rmt_success;
}

int PlayerLogMgr::delSendMailLog(const std::string& mailId)
{
	auto pArray = m_mailList->get_Tarray<SendMailLogInfoArray>();
	int count = pArray->get_obj_count();
	for(int i = 0; i < count; i++)
	{
		auto ptr = pArray->get_Tobj<SendMailLogInfo>(i);
		if(ptr->m_mailId->get_string() == mailId)
		{
			pArray->del_obj_by_index(i);
			m_mailList->set_update();
			//get_game_player()->store_game_object();
			return msg_type_def::e_rmt_success;
		}
	}
	return msg_type_def::e_rmt_unknow;
}

static mongo::BSONObj g_retField = BSON("nickname" << 1);

bool PlayerLogMgr::_getPlayerInfo(int playerId, stPlayerInfo& info)
{
	auto pTarget = game_player_mgr::instance().findPlayerById(playerId);
	if(pTarget == nullptr)
	{
		mongo::BSONObj cond = BSON("player_id" << playerId);
		mongo::BSONObj obj = db_player::instance().findone(DB_PLAYER_INFO, cond, &g_retField);
		if(obj.isEmpty())
			return false;

		info.m_nickName = obj.getStringField("nickname");
	}
	else
	{
		info.m_nickName = pTarget->NickName->get_string();
	}

	return true;
}
