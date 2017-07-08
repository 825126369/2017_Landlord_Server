#pragma once
#include "game_object_queue.h"
class game_player;

struct SendGiftLogInfo : public game_object, public enable_obj_pool<SendGiftLogInfo>
{
	SendGiftLogInfo();

	// 注册属性
	virtual void init_game_object();

	// 赠送时间
	Tfield<time_t>::TFieldPtr m_sendTime;

	// 好友id
	Tfield<int32_t>::TFieldPtr m_friendId;

	// 礼物ID
	Tfield<int32_t>::TFieldPtr m_giftId;

	// 数量
	Tfield<GOLD_TYPE>::TFieldPtr m_count;

	//
	Tfield<bool>::TFieldPtr m_sendgold;

	// 邮件ID
	GStringFieldPtr m_mailId;

	// 好友昵称
	GStringFieldPtr m_friendNickName;
};

class SendGiftLogInfoArray : public game_object_queue , public enable_obj_pool<SendGiftLogInfoArray>
{
public:
	virtual const std::string& get_cells_name();

	virtual const std::string& get_id_name();

	virtual GObjPtr create_game_object(uint32_t object_id);

	virtual const std::string& get_container_name();	

	virtual bool is_load();		

	virtual uint32_t get_index_id();

	virtual const mongo::BSONObj& get_id_finder();

	virtual db_base* get_db();

	void setPlayer(int playerId);
public:
	mongo::BSONObj m_idFinder;
	int m_cellIndex;
};

//////////////////////////////////////////////////////////////////////////

struct SendMailLogInfo : public game_object, public enable_obj_pool<SendMailLogInfo>
{
	SendMailLogInfo();

	// 注册属性
	virtual void init_game_object();

	// 发送时间
	Tfield<time_t>::TFieldPtr m_sendTime;

	// 好友id
	Tfield<int32_t>::TFieldPtr m_friendId;
	// 好友昵称
	GStringFieldPtr m_friendNickName;
	
	// 发送内容
	GStringFieldPtr m_content;

	// 邮件标题
	GStringFieldPtr m_title;

	// 邮件ID
	GStringFieldPtr m_mailId;
};

class SendMailLogInfoArray : public game_object_queue , public enable_obj_pool<SendMailLogInfoArray>
{
public:
	virtual const std::string& get_cells_name();

	virtual const std::string& get_id_name();

	virtual GObjPtr create_game_object(uint32_t object_id);

	virtual const std::string& get_container_name();	

	virtual bool is_load();		

	virtual uint32_t get_index_id();

	virtual const mongo::BSONObj& get_id_finder();

	virtual db_base* get_db();

	void setPlayer(int playerId);
public:
	mongo::BSONObj m_idFinder;
	int m_cellIndex;
};


struct SafeBoxLogInfo : public game_object, public enable_obj_pool<SafeBoxLogInfo>
{
	SafeBoxLogInfo();

	// 注册属性
	virtual void init_game_object();

	// 时间
	Tfield<time_t>::TFieldPtr m_time;

	// 金币
	Tfield<GOLD_TYPE>::TFieldPtr m_gold;

	Tfield<GOLD_TYPE>::TFieldPtr m_player_gold;
};

class SafeBoxLogInfoArray : public game_object_queue , public enable_obj_pool<SafeBoxLogInfoArray>
{
public:
	virtual const std::string& get_cells_name();

	virtual const std::string& get_id_name();

	virtual GObjPtr create_game_object(uint32_t object_id);

	virtual const std::string& get_container_name();	

	virtual bool is_load();		

	virtual uint32_t get_index_id();

	virtual const mongo::BSONObj& get_id_finder();

	virtual db_base* get_db();

	void setPlayer(int playerId);
public:
	mongo::BSONObj m_idFinder;
	int m_playerId;
};