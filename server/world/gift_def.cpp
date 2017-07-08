#include "stdafx.h"
#include "gift_def.h"
#include "game_db.h"

GiftInfo::GiftInfo()
{
	init_game_object();
}

void GiftInfo::init_game_object()
{
	m_giftId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "giftId"));
	m_count = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "count"));
}

uint32_t GiftInfo::get_id()
{ 
	return m_giftId->get_value();
}

//////////////////////////////////////////////////////////////////////////

const std::string& GiftMap::get_cells_name()
{
	static std::string cellsname = "gifts";
	return cellsname;
}

const std::string& GiftMap::get_index_name()
{
	static std::string idname = "player_id";
	return idname;
}

uint32_t GiftMap::get_index_id()
{
	return m_playerId;
}

GObjPtr GiftMap::create_game_object(uint32_t object_id)
{
	auto sp = GiftInfo::malloc();
	sp->m_giftId->set_value(object_id);
	return sp;
}

const std::string& GiftMap::get_container_name()
{
	return DB_PLAYER_INFO;
}

bool GiftMap::is_load()
{
	return true;
}

db_base* GiftMap::get_db()
{
	return &db_player::instance();
}

const mongo::BSONObj& GiftMap::get_id_finder()
{
	return m_idFinder;
}

const std::string& GiftMap::get_id_name()
{
	static std::string idname = "giftId";
	return idname;
}

void GiftMap::attachPlayer(int playerId) 
{
	m_playerId = playerId;
	m_idFinder = BSON("player_id" << playerId);	
}



