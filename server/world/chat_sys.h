#pragma once
#include "game_sys_def.h"
class ChatSys;

// ��������С����
struct stContinuousSendSpeaker
{
	std::string m_id;

	int m_playerId;
	int m_vipLevel;
	std::string m_nickName;
	std::string m_content;
	int m_remainCount;	// ʣ�����
	bool m_hasMonthCard;
	time_t m_sendTime;	// ���͵�ʱ���

	stContinuousSendSpeaker()
	{
		m_sendTime = 0;
		m_hasMonthCard = false;
	}
};

struct stRobotNotice
{
	double m_robotNoticeTime;	// ����ͨ��ʱ��
	double m_interval;
	void reset();
	void update(double delta, ChatSys* sys);	// ÿ��һ�����ʱ�䣨120�뵽300�룩�������ѡһ������һ��ͨ��
};

// ����ϵͳ
class ChatSys : public game_sys_base
{
public:
	ChatSys();
	MAKE_SYS_TYPE(e_game_sys_type::e_gst_chat);
	virtual void init_sys_object();
	//��������
	virtual bool sys_load();
	virtual void sys_update(double delta);
	virtual void sys_exit();

	//��Ϸ������talker		˵����content		˵������	audio_len	��������audio_time	¼��ʱ��
	int gameChat( game_player* talker, const std::string& content, int audio_len , int audio_time );
	// ��ҽ�����Ϸ
	int playerEnterGame(game_player* talker);
	//ͨ��
	int notify(const std::string& nickName, const std::string& content, int notifyType, int playerId, int vipLevel,bool hasMonthCard, int repCount = 1, int repInterval = 0);
	//��ϵͳ������ͨ��
	int sysNotify(const std::string& content, int notifyType, int repCount = 1, int repInterval = 0);
	//����ҷ����ȫ�����棬��Ҫ������ȯ
	int playerNotify(game_player* talker, const std::string& content);
	// ����С����
	int playerContinuousSendSpeaker(game_player* talker, const std::string& content, int sendCount);
	//�����ݿ��������С���ȵķ�����Ϣ
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

