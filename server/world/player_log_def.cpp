#include "stdafx.h"
#include "player_log_def.h"
#include "game_player.h"
#include "game_db.h"

SendGiftLogInfo::SendGiftLogInfo()
{
	init_game_object();
}

void SendGiftLogInfo::init_game_object()
{
	m_sendTime = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "sendTime"));

	m_friendId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "friendId"));

	m_giftId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "giftId"));

	m_count = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(GOLD_OBJ_TYPE, "count"));

	m_sendgold = CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "sendgold"));

	m_mailId = regedit_strfield("mailId");

	m_friendNickName = regedit_strfield("friendNickName");
}

//enable_obj_pool_init(PrivateChatArray, boost::null_mutex);

const std::string& SendGiftLogInfoArray::get_cells_name()
{
	static std::string cellsname = "sendGiftLog";
	return cellsname;
}

const std::string& SendGiftLogInfoArray::get_id_name()
{
	static std::string idname = "friendId";
	return idname;
}

GObjPtr SendGiftLogInfoArray::create_game_object(uint32_t object_id)
{
	auto p = SendGiftLogInfo::malloc();
	p->m_friendId->set_value(object_id);
	return p;
}

const std::string& SendGiftLogInfoArray::get_container_name()
{
	return DB_PLAYER_INFO;
}

bool SendGiftLogInfoArray::is_load()
{
	return true;
}

uint32_t SendGiftLogInfoArray::get_index_id()
{
	return 0;
}

const mongo::BSONObj& SendGiftLogInfoArray::get_id_finder()
{
	return m_idFinder;
}

db_base* SendGiftLogInfoArray::get_db()
{
	return &db_player::instance();
}

void SendGiftLogInfoArray::setPlayer(int playerId) 
{
	m_idFinder = BSON("player_id" << playerId);	
}

//////////////////////////////////////////////////////////////////////////

SendMailLogInfo::SendMailLogInfo()
{
	init_game_object();
}

void SendMailLogInfo::init_game_object()
{
	m_sendTime = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "sendTime"));

	m_friendId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "friendId"));

	m_content = regedit_strfield("content");

	m_title = regedit_strfield("title");
	
	m_mailId = regedit_strfield("mailId");

	m_friendNickName = regedit_strfield("friendNickName");
}

//enable_obj_pool_init(PrivateChatArray, boost::null_mutex);

const std::string& SendMailLogInfoArray::get_cells_name()
{
	static std::string cellsname = "sendMailLog";
	return cellsname;
}

const std::string& SendMailLogInfoArray::get_id_name()
{
	static std::string idname = "friendId";
	return idname;
}

GObjPtr SendMailLogInfoArray::create_game_object(uint32_t object_id)
{
	auto p = SendMailLogInfo::malloc();
	p->m_friendId->set_value(object_id);
	return p;
}

const std::string& SendMailLogInfoArray::get_container_name()
{
	return DB_PLAYER_INFO;
}

bool SendMailLogInfoArray::is_load()
{
	return true;
}

uint32_t SendMailLogInfoArray::get_index_id()
{
	return 0;
}

const mongo::BSONObj& SendMailLogInfoArray::get_id_finder()
{
	return m_idFinder;
}

db_base* SendMailLogInfoArray::get_db()
{
	return &db_player::instance();
}

void SendMailLogInfoArray::setPlayer(int playerId) 
{
	m_idFinder = BSON("player_id" << playerId);	
}


SafeBoxLogInfo::SafeBoxLogInfo()
{
	init_game_object();
}

void SafeBoxLogInfo::init_game_object()
{
	m_time = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "time"));

	m_gold = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(GOLD_OBJ_TYPE, "gold"));

	m_player_gold = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(GOLD_OBJ_TYPE, "playergold"));
}


const std::string& SafeBoxLogInfoArray::get_cells_name()
{
	static std::string cellsname = "safeBoxLog";
	return cellsname;
}

const std::string& SafeBoxLogInfoArray::get_id_name()
{
	static std::string idname = "player_id";
	return idname;
}

GObjPtr SafeBoxLogInfoArray::create_game_object(uint32_t object_id)
{
	auto p = SafeBoxLogInfo::malloc();
	return p;
}

const std::string& SafeBoxLogInfoArray::get_container_name()
{
	return DB_PLAYER_INFO;
}

bool SafeBoxLogInfoArray::is_load()
{
	return m_playerId > 0;
}

uint32_t SafeBoxLogInfoArray::get_index_id()
{
	return m_playerId;
}

const mongo::BSONObj& SafeBoxLogInfoArray::get_id_finder()
{
	return m_idFinder;
}

db_base* SafeBoxLogInfoArray::get_db()
{
	return &db_player::instance();
}

void SafeBoxLogInfoArray::setPlayer(int playerId) 
{
	m_playerId = playerId;
	m_idFinder = BSON("player_id" << playerId);	
}
