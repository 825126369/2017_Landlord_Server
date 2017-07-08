#pragma once
#include "game_sys_def.h"
#include "mail_def.h"
#include "enable_hashmap.h"

struct stNoticeInfo;
struct stServiceInfo;

// 公告
class NoticeSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_notice);

	virtual void init_sys_object();

	virtual void sys_update(double delta);

	/*
			获取公告列表
			minT	找时间 > minT的公告
			Last	返回最后一个公告的发送时间
			返回值参见 e_msg_result_def定义
	*/
	int getNotice(std::vector<stNoticeInfo>& res, time_t minT, time_t* Last = NULL);

	// 返回客服信息
	bool getServiceInfo(const std::string& plat, std::vector<stServiceInfo>* & info);
private:
	void _parseServiceInfo(std::string& plat, std::string& info);
private:
	time_t m_lastRemove;

	// plat->info
	ENABLE_MAP<std::string, std::vector<stServiceInfo> > m_serviceInfo;
	double m_elapsedTime;
};

struct stNoticeInfo
{
	time_t m_genTime;
	std::string m_title;
	std::string m_content;
	int m_order;
};

struct stServiceInfo
{
	int m_infoType;
	std::string m_key;
	std::string m_value;
};

