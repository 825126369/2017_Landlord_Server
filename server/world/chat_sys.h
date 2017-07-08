#pragma once
#include "game_sys_def.h"
class ChatSys;

// 连续发送小喇叭
struct stContinuousSendSpeaker
{
	std::string m_id;

	int m_playerId;
	int m_vipLevel;
	std::string m_nickName;
	std::string m_content;
	int m_remainCount;	// 剩余次数
	bool m_hasMonthCard;
	time_t m_sendTime;	// 发送的时间点

	stContinuousSendSpeaker()
	{
		m_sendTime = 0;
		m_hasMonthCard = false;
	}
};

struct stRobotNotice
{
	double m_robotNoticeTime;	// 机器通告时间
	double m_interval;
	void reset();
	void update(double delta, ChatSys* sys);	// 每隔一个随机时间（120秒到300秒）就随机挑选一条发送一次通告
};

// 聊天系统
class ChatSys : public game_sys_base
{
public:
	ChatSys();
	MAKE_SYS_TYPE(e_game_sys_type::e_gst_chat);
	virtual void init_sys_object();
	//加载数据
	virtual bool sys_load();
	virtual void sys_update(double delta);
	virtual void sys_exit();

	//游戏内聊天talker		说话者content		说话内容	audio_len	声音长度audio_time	录音时长
	int gameChat( game_player* talker, const std::string& content, int audio_len , int audio_time );
	// 玩家进入游戏
	int playerEnterGame(game_player* talker);
	//通告
	int notify(const std::string& nickName, const std::string& content, int notifyType, int playerId, int vipLevel,bool hasMonthCard, int repCount = 1, int repInterval = 0);
	//由系统发出的通告
	int sysNotify(const std::string& content, int notifyType, int repCount = 1, int repInterval = 0);
	//由玩家发起的全服公告，需要消耗礼券
	int playerNotify(game_player* talker, const std::string& content);
	// 连续小喇叭
	int playerContinuousSendSpeaker(game_player* talker, const std::string& content, int sendCount);
	//从数据库加载连续小喇叭的发送信息
	int addContinuousSendSpeakerFromDb(int playerId, int vipLevel, std::string& nickName, const std::string& content, int sendCount, const std::string& id, bool hasMonthCard);
	stContinuousSendSpeaker* getSpeaker(int playerId);
private:
	int _buildSpeaker(stContinuousSendSpeaker& speaker, mongo::BSONObj& bsonResult);
	
	void _operationNotify(int notifyType, const std::string& content);
private:
	double m_time;
	time_t m_interval;
	std::map<int, stContinuousSendSpeaker> m_speaker;
	stRobotNotice m_robotNotice;
};

