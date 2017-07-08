#pragma once
#include "game_sys_def.h"

struct stSendGiftLogInfo;
struct stSendMailLogInfo;
struct stPlayerInfo;

namespace client2world_protocols 
{
	class packetw2c_req_safebox_log_result;
}

// 玩家日志
class PlayerLogMgr : public enable_obj_pool<PlayerLogMgr>, public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_player_log);

	MAKE_GET_OWNER(game_player)

	virtual void init_sys_object();

	virtual bool sys_load();

	/*
			返回赠送礼物发送日志
	*/
	int getSendGiftLog(time_t minT, time_t& Last, std::deque<stSendGiftLogInfo>& res);

	/*
			返回邮件发送日志
	*/
	int getSendMailLog(time_t minT, time_t& Last, std::deque<stSendMailLogInfo>& res);

	/*
			返回保险箱日志
	*/
	int getSafeBoxLog(time_t start_time, boost::shared_ptr<client2world_protocols::packetw2c_req_safebox_log_result>& msg);

	/*
			添加赠送记录
	*/
	int addSendGiftLog(int playerId, int giftId, GOLD_TYPE giftCount, const std::string& mailId, bool send, int maxLogCount);
	/*
			添加保险箱记录
	*/
	int addSafeBoxLog(GOLD_TYPE gold, GOLD_TYPE playerGold, int maxLogCount);

	/*
			添加发送邮件记录
	*/
	int addSendMailLog(int friendId, const std::string& title, const std::string& content, const std::string& mailId, int maxLogCount);

	/*
			删除邮件日志
	*/
	int delSendMailLog(const std::string& mailId);
private:
	bool _getPlayerInfo(int playerId, stPlayerInfo& info);
private:
	// 赠送列表
	GArrayFieldPtr m_sendList;

	// 邮件列表
	GArrayFieldPtr m_mailList;
	//保险箱记录
	GArrayFieldPtr m_safeBoxList;
};

struct stSendGiftLogInfo
{
	time_t m_sendTime;
	int32_t m_friendId;
	std::string m_friendNickName;
	int32_t m_giftId;
	int32_t m_count;
	bool m_sendgold;
	std::string m_mailid;
};

struct stSendMailLogInfo
{
	time_t m_sendTime;
	int32_t m_friendId;
	std::string m_title;
	std::string m_content;
	std::string m_mailId;
	std::string m_friendNickName;
};