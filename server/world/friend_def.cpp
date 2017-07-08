#include "stdafx.h"
#include "friend_def.h"
#include "game_db.h"

FriendItem::FriendItem()
{
	init_game_object();
}

void FriendItem::init_game_object()
{
	m_friendId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "friendId"));
}

uint32_t FriendItem::get_id()
{
	return m_friendId->get_value();
}

//////////////////////////////////////////////////////////////////////////

FriendMap::FriendMap()
{
	m_playerId = 0;
}

const std::string& FriendMap::get_cells_name()
{
	static std::string cellsname = "friends";
	return cellsname;
}

const std::string& FriendMap::get_index_name()
{
	static std::string name = "player_id";
	return name;
}

uint32_t FriendMap::get_index_id()
{
	return m_playerId;
}

GObjPtr FriendMap::create_game_object(uint32_t object_id)
{
	auto finfo = FriendItem::malloc();
	finfo->m_friendId->set_value(object_id);
	return finfo;
}

const std::string& FriendMap::get_container_name()
{
	return DB_PLAYER_INFO;
}

bool FriendMap::is_load()
{
	return m_playerId > 0;
}

db_base* FriendMap::get_db()
{
	return &db_player::instance();
}

const mongo::BSONObj& FriendMap::get_id_finder()
{
	return m_idFinder;
}

const std::string& FriendMap::get_id_name()
{
	static std::string idname = "friendId";
	return idname;
}

void FriendMap::setPlayerId(int playerId)
{
	m_playerId = playerId;
	m_idFinder = BSON("player_id" << playerId);	
}
