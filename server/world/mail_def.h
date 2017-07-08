#pragma once
#include "gift_def.h"

struct stMail
{
	// 发送者
	int32_t m_senderId;
	// 邮件ID
	std::string m_mailId;
	// 发送时间
	time_t m_time;
	// 标题
	std::string m_title;
	// 发送者
	std::string m_sender;
	// 内容
	std::string m_content;
	// 是否已领取道具
	bool m_isRecvive;
	// 道具列表
	std::vector<stGift> m_items;
};
