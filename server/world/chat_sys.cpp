#include "stdafx.h"
#include "chat_sys.h"
#include "msg_type_def.pb.h"
#include "game_player.h"
#include "proc_chat.h"
#include "servers_manager.h"
#include "M_BaseInfoCFG.h"
#include "M_MultiLanguageCFG.h"
#include "time_helper.h"
#include "pump_type.pb.h"
#include "game_sys_recharge.h"
#include "game_db.h"
#include "game_quest_mgr.h"
#include "game_quest.h"
#include "enable_random.h"
#include "M_RobotNameCFG.h"

static std::string OPERATION_NOTIFY = "optionNotify";
static std::string OPERATION_SPEAKER = "operationSpeaker";

ChatSys::ChatSys()
{
	m_time = 0.0;
	m_interval = 1;
	static const M_BaseInfoCFGData* data = M_BaseInfoCFG::GetSingleton()->GetData("worldChatInterval");
	if(data)
	{
		m_interval = data->mValue;
	}
}

void ChatSys::init_sys_object()
{
	m_robotNotice.reset();
}

bool ChatSys::sys_load()
{
	mongo::BSONObj cond = BSON("playerId" << mongo::GT << 0);

	std::vector<mongo::BSONObj> vec;
	db_player::instance().find(vec, DB_SPEAKER, cond);

	if(!vec.empty())
	{
		for(int i = 0; i < (int)vec.size(); i++)
		{
			mongo::BSONObj& obj = vec[i];

			std::string id = obj.getField("_id").OID().toString();
			std::string content = obj.getStringField("content");
			int remainCount = obj.getIntField("remainCount");
			int vipLevel = obj.getIntField("vipLevel");
			std::string nickName = obj.getStringField("nickName");
			int playerId = obj.getIntField("playerId");
			bool hasMonthCard = obj.getBoolField("hasMonthCard");

			addContinuousSendSpeakerFromDb(playerId,vipLevel, nickName, content, remainCount, id, hasMonthCard);
		}
	}

	return true;
}

void ChatSys::sys_update(double delta)
{
	// 运营从后台发送的通告
	m_time += delta;
	if(m_time > 30.0)
	{
		time_t curT = time_helper::instance().get_cur_time();
		mongo::BSONObj rcond = BSON("sendTime" << mongo::LT << mongo::Date_t(curT * 1000));

		static mongo::BSONObj retField = BSON("content" << 1 << "repCount" << 1 << "interval" << 1 << "_id" << 1);

		std::vector<mongo::BSONObj> vec;
		db_player::instance().find(vec, OPERATION_SPEAKER, rcond, &retField);

		for(int i = 0; i < (int)vec.size(); i++)
		{
			mongo::BSONObj& obj = vec[i];
			std::string content = obj.getStringField("content");
			int repCount = obj.getIntField("repCount");
			int interval = obj.getIntField("interval");

			std::string id = obj.getField("_id").OID().toString();

			sysNotify(content, msg_type_def::NotifyTypeSys, repCount, interval);

			mongo::OID oid;
			oid.init(id);
			db_player::instance().remove(OPERATION_SPEAKER, BSON("_id" << oid));
		}

		m_time = 0.0;
	}

	static int interval = M_BaseInfoCFG::GetSingleton()->GetData("continuousSendSpeakerInterval")->mValue;

	if(!m_speaker.empty())
	{
		time_t curTime = time_helper::instance().get_cur_time();
		for(auto it = m_speaker.begin(); it != m_speaker.end(); )
		{
			if(curTime < it->second.m_sendTime)
			{
				++it;
				continue;
			}

			notify(it->second.m_nickName,
				it->second.m_content, 
				msg_type_def::NotifyTypePlayerSpeaker, 
				it->second.m_playerId, 
				it->second.m_vipLevel,
				it->second.m_hasMonthCard);

			it->second.m_remainCount--;

			if(it->second.m_remainCount <= 0)
			{
				mongo::OID oid;
				oid.init(it->second.m_id);
				db_player::instance().remove(DB_SPEAKER, BSON("_id" << oid));

				it = m_speaker.erase(it);
			}
			else
			{
				it->second.m_sendTime = curTime + interval;

				++it;
			}
		}
	}

	m_robotNotice.update(delta, this);
}

void ChatSys::sys_exit()
{
	for(auto it = m_speaker.begin(); it != m_speaker.end(); ++it)
	{
		mongo::OID oid;
		oid.init(it->second.m_id);
		db_player::instance().update(DB_SPEAKER, BSON("_id" << oid), BSON("$set" << BSON("remainCount" << it->second.m_remainCount)));
	}
}

int ChatSys::gameChat( game_player* talker, const std::string& content, int audio_len , int audio_time  )
{
	if(talker == nullptr)
		return msg_type_def::e_rmt_unknow;

	if(!talker->is_gaming())
		return msg_type_def::e_rmt_unknow;

	time_t curTime = time_helper::instance().get_cur_time();
	time_t delta = curTime - talker->LastGameChatTime;
	if(delta < m_interval)
	{
		auto sendmsg = PACKET_CREATE(packetw2c_chat_result, e_mst_w2c_chat_result);
		sendmsg->set_result(msg_type_def::e_rmt_chat_too_often);
		talker->send_msg_to_client(sendmsg);
	}
	else
	{
		talker->LastGameChatTime = curTime;
		auto sendmsg = PACKET_CREATE(packetw2c_chat_result, e_mst_w2c_chat_result);
		sendmsg->set_content(content);
		sendmsg->set_talkernickname(talker->NickName->get_string());
		sendmsg->set_talkerid(talker->PlayerId->get_value());
		sendmsg->set_result(msg_type_def::e_rmt_success);
		sendmsg->set_audio_len(audio_len);
		sendmsg->set_audio_time(audio_time);
		bool hasMonthCard = talker->get_sys<game_sys_recharge>()->getMonthCardRemainSecondTime(curTime) > 0;
		sendmsg->set_hasmonthcard(hasMonthCard);

		int vipLevel = talker->get_viplvl();
		sendmsg->set_talkerviplevel(vipLevel);
		servers_manager::instance().broadcast_msg2(sendmsg, talker->get_logicid());		
	}
	return 0;
}

int ChatSys::playerEnterGame(game_player *talker)
{
	if(talker == nullptr)
		return msg_type_def::e_rmt_unknow;
	
	if(!talker->is_gaming())
		return msg_type_def::e_rmt_unknow;

	static const int vipLimit = M_BaseInfoCFG::GetSingleton()->GetData("notifyEnterGameVipLevel")->mValue;
	int vipLevel = talker->get_viplvl();
	if(vipLevel < vipLimit)
		return msg_type_def::e_rmt_vip_under;

	auto sendmsg = PACKET_CREATE(packetw2c_chat_result, e_mst_w2c_chat_result);
	sendmsg->set_talkernickname(talker->NickName->get_string());
	sendmsg->set_talkerid(talker->PlayerId->get_value());
	sendmsg->set_result(msg_type_def::e_rmt_success);
	sendmsg->set_talkerviplevel(vipLevel);
	sendmsg->set_chattype(1);

	servers_manager::instance().broadcast_msg2(sendmsg, talker->get_logicid());	

	return msg_type_def::e_rmt_success;
}

int ChatSys::notify(const std::string& nickName, const std::string& content, int notifyType, 
					int playerId, int vipLevel, bool hasMonthCard,
					int repCount, int repInterval)
{
	auto sendmsg = PACKET_CREATE(packetw2c_notify, e_mst_w2c_notify);
	sendmsg->set_talkernickname(nickName);
	sendmsg->set_content(content);
	sendmsg->set_notifytype(notifyType);
	sendmsg->set_playerid(playerId);
	sendmsg->set_talkerviplevel(vipLevel);
	sendmsg->set_hasmonthcard(hasMonthCard);
	if(repCount > 1)
	{
		sendmsg->set_repcount(repCount);
		sendmsg->set_interval(repInterval);
	}
	servers_manager::instance().broadcast_msg2(sendmsg);  
	return msg_type_def::e_rmt_success;
}

int ChatSys::sysNotify(const std::string& content, int notifyType, int repCount, int repInterval)
{
	static const M_MultiLanguageCFGData* pSystem = M_MultiLanguageCFG::GetSingleton()->GetData("System");
	return notify(pSystem->mName, content, notifyType, 0, 0, false, repCount, repInterval);
}

int ChatSys::playerNotify(game_player* talker, const std::string& content)
{
	if(talker == nullptr)
		return msg_type_def::e_rmt_unknow;

	int costTicket = 5;
	static const M_BaseInfoCFGData* data = M_BaseInfoCFG::GetSingleton()->GetData("all_notice_player");
	if(data)
	{
		costTicket = data->mValue;
	}

	static const int free_level = M_BaseInfoCFG::GetSingleton()->GetData("FreeSpeakerLevel")->mValue;
	if (talker->get_viplvl() < free_level)
	{
		if(talker->Ticket->get_value() < costTicket)
			return msg_type_def::e_rmt_ticket_not_enough;

		talker->addItem(msg_type_def::e_itd_ticket, -costTicket, type_reason_player_notify);
	}

	bool hasMonthCard = talker->get_sys<game_sys_recharge>()->getMonthCardRemainSecondTime() > 0;

	notify(talker->NickName->get_string(), 
		content, 
		msg_type_def::NotifyTypePlayerSpeaker, 
		talker->PlayerId->get_value(),
		talker->get_viplvl(),
		hasMonthCard);

	talker->get_sys<game_quest_mgr>()->change_quest(e_qt_loudspeaker);

	return msg_type_def::e_rmt_success;
}

void ChatSys::_operationNotify(int notifyType, const std::string& content)
{
	static const M_MultiLanguageCFGData* pSystem = M_MultiLanguageCFG::GetSingleton()->GetData("System");

	switch(notifyType)
	{
	case 0:	   // 普通公告
		{
			sysNotify(content, msg_type_def::NotifyTypeSys);
		}
		break;
	case 1:    // 维护公告
		{
			
		}
		break;
	}
}

int ChatSys::playerContinuousSendSpeaker(game_player* talker, const std::string& content, int sendCount)
{
	if(talker == nullptr)
		return msg_type_def::e_rmt_unknow;

	if(sendCount <= 0)
		return msg_type_def::e_rmt_unknow;

	// 发小喇叭的VIP限制
	static const int vipLimit = M_BaseInfoCFG::GetSingleton()->GetData("continuousVipCount")->mValue;
	auto vipMgr = talker->get_sys<game_sys_recharge>();
	if(vipMgr->VipLevel->get_value() < vipLimit)
		return msg_type_def::e_rmt_vip_under;

	static const int MAX_COUNT = M_BaseInfoCFG::GetSingleton()->GetData("continuousSpeakerMaxCount")->mValue;
	if(sendCount > MAX_COUNT)
		return msg_type_def::e_rmt_speaker_beyond_max_count;

	auto ptr = getSpeaker(talker->PlayerId->get_value());
	if(ptr)
		return msg_type_def::e_rmt_last_speaker_not_finish;

	int costTicket = 0;
	static const M_BaseInfoCFGData* data = M_BaseInfoCFG::GetSingleton()->GetData("continuousSendSpeakerCost");
	if(data)
	{
		costTicket = data->mValue * sendCount;
	}
	else
	{
		costTicket = 25 * sendCount;
	}

	static const int free_level = M_BaseInfoCFG::GetSingleton()->GetData("FreeSpeakerLevel")->mValue;
	if (talker->get_viplvl() < free_level)
	{
		if(talker->Ticket->get_value() < costTicket)
			return msg_type_def::e_rmt_ticket_not_enough;

		talker->addItem(msg_type_def::e_itd_ticket, -costTicket, type_reason_continuous_send_speaker);
	}

	stContinuousSendSpeaker speaker;
	speaker.m_playerId = talker->PlayerId->get_value();
	speaker.m_vipLevel = talker->get_viplvl();
	speaker.m_nickName = talker->NickName->get_string();
	speaker.m_content = content;
	speaker.m_remainCount = sendCount;
	speaker.m_hasMonthCard = vipMgr->getMonthCardRemainSecondTime() > 0;

	mongo::BSONObj obj;
	_buildSpeaker(speaker, obj);
	const std::string& res = db_player::instance().insert(DB_SPEAKER, obj);

	m_speaker[speaker.m_playerId] = speaker;

	talker->get_sys<game_quest_mgr>()->change_quest(e_qt_loudspeaker, sendCount);

	return msg_type_def::e_rmt_success;
}

int ChatSys::addContinuousSendSpeakerFromDb(int playerId, 
											int vipLevel,
											std::string& nickName, 
											const std::string& content, 
											int sendCount,
											const std::string& id, 
											bool hasMonthCard)
{
	stContinuousSendSpeaker speaker;
	speaker.m_id = id;
	speaker.m_playerId = playerId;
	speaker.m_vipLevel = vipLevel;
	speaker.m_nickName = nickName;
	speaker.m_content = content;
	speaker.m_hasMonthCard = hasMonthCard;
	speaker.m_remainCount = sendCount;

	m_speaker[playerId] = speaker;

	return msg_type_def::e_rmt_success;
}

stContinuousSendSpeaker* ChatSys::getSpeaker(int playerId)
{
	auto it = m_speaker.find(playerId);
	if(it == m_speaker.end())
		return nullptr;

	return &it->second;
}

int ChatSys::_buildSpeaker(stContinuousSendSpeaker& speaker, mongo::BSONObj& bsonResult)
{
	mongo::OID oid;
	oid.init();

	mongo::BSONObjBuilder builder;
	speaker.m_id = oid.toString();
	builder.appendOID("_id", &oid);

	// 玩家id
	builder.append("playerId", speaker.m_playerId);
	builder.append("vipLevel", speaker.m_vipLevel);
	builder.append("nickName", speaker.m_nickName);
	builder.append("content", speaker.m_content);
	builder.append("hasMonthCard", speaker.m_hasMonthCard);

	builder.append("remainCount", speaker.m_remainCount);

	bsonResult = builder.done().copy();
	return msg_type_def::e_rmt_success;
}

// 机器人通告
void stRobotNotice::reset()
{
	static const M_BaseInfoCFGData* data1 = M_BaseInfoCFG::GetSingleton()->GetData("RobotNoticeMinTime");
	static const M_BaseInfoCFGData* data2 = M_BaseInfoCFG::GetSingleton()->GetData("RobotNoticeMaxTime");

	m_interval = global_random::instance().rand_int(data1->mValue, data2->mValue);
	m_robotNoticeTime = 0;
}

void stRobotNotice::update(double delta, ChatSys* sys)
{
	m_robotNoticeTime += delta;
	if(m_robotNoticeTime > m_interval)
	{
		int maxCount = M_RobotNameCFG::GetSingleton()->GetCount() - 1;
		int index = global_random::instance().rand_int(0, maxCount);
		const M_RobotNameCFGData* pRobot = M_RobotNameCFG::GetSingleton()->GetData(index);
		if(pRobot)
		{
			char buf[16] = {0};
			int r = global_random::instance().rand_int(1, 7);
			sprintf_s(buf, 16, "RobotNotice%02d", r);

			const M_MultiLanguageCFGData *pNotice = M_MultiLanguageCFG::GetSingleton()->GetData(buf);
			if(pNotice)
			{
				boost::format fmt = boost::format(pNotice->mName) % pRobot->mNickName;
				sys->sysNotify(fmt.str(), msg_type_def::NotifyTypeRobot);
			}
		}

		reset();
	}
}