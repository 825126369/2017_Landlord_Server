#pragma once
#include "game_sys_def.h"
#include "mail_def.h"

class game_player;

// 邮件系统
class MailSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_mail);

	/*
			给玩家发送邮件
			title   邮件标题
			sender  发件人名字
			content 文本内容
			senderId	发送者ID
			durationDay		邮件有效天数, 0表示没有时间限制
			items   若邮件含道具信息，则传入，否则置空。
			mailId 返回一个邮件ID
			返回值参见 e_msg_result_def定义
	*/
	int sendMail(const std::string& title, const std::string& sender, const std::string& content, int senderId, int toPlayerId, int durationDay, std::vector<stGift>* items = NULL, std::string *mailId = NULL
		,int needCoin = 0);
	
	/*
			领取邮件中所含礼物
			player 玩家
			mailId 邮件ID
			resItems   若需要返回所领取的道具，则传入这个参数
			返回值参见 e_msg_result_def定义
	*/
	int receiveGift(game_player* player, const std::string& mailId, std::vector<stGift>* resGifts = NULL);
	
	/*
			删除某个邮件
			返回值参见 e_msg_result_def定义
	*/
	int removeMail(game_player* player, const std::string& mailId);
	
	/*
			获取玩家邮件
			t   找时间 > t的邮件
			Last 返回最后一封邮件的发送时间
			res 返回邮件结果
			返回值参见 e_msg_result_def定义
	*/
	int getMail(game_player* player, std::vector<stMail>& res, time_t t, time_t* Last = NULL);

	/*
			构建一份邮件
			由mailResult返回结果
	*/
	int buildMail(const std::string& title, const std::string& sender, const std::string& content, int senderId, int toPlayerId, int durationDay, mongo::BSONObj& mailResult, 
		std::vector<stGift>* items = NULL, std::string *mailId = NULL,int needCoin = 0);

	/*
			批量发送邮件
	*/
	int sendMail(std::vector<mongo::BSONObj>& mailList);

	/*
			发送礼物
			player		发送者
			返回值参见 e_msg_result_def定义
	*/
	int sendGift(game_player* player, const std::string& title, int toPlayerId, int durationDay, std::vector<stGift>& gifts, std::string *mailId = NULL);
};

