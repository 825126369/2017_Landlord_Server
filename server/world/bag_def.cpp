#include "stdafx.h"
#include "bag_def.h"
#include "game_db.h"

//enable_obj_pool_init(GameItem, boost::null_mutex);
GameItem::GameItem()
{
	init_game_object();
}

GameItem::~GameItem()
{
}

void GameItem::init_game_object()
{
	m_itemId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "itemId"));
	m_itemCount = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "count"));
}

uint32_t GameItem::get_id()
{
	return m_itemId->get_value();
}

//////////////////////////////////////////////////////////////////////////

BagMap::BagMap()
{
	m_playerId = 0;
}

const std::string& BagMap::get_cells_name()
{
	static std::string cellsname = "items";
	return cellsname;
}

const std::string& BagMap::get_index_name()
{
	static std::string name = "player_id";
	return name;
}

uint32_t BagMap::get_index_id()
{
	return m_playerId;
}

GObjPtr BagMap::create_game_object(uint32_t object_id)
{
	auto tmp_item = GameItem::malloc();
	tmp_item->m_itemId->set_value(object_id);
	return tmp_item;
}

const std::string& BagMap::get_container_name()
{
	return DB_PLAYER_BAG;
}

bool BagMap::is_load()
{
	return m_playerId > 0;
}

db_base* BagMap::get_db()
{
	return &db_player::instance();
}

const mongo::BSONObj& BagMap::get_id_finder()
{
	return m_idFinder;
}

const std::string& BagMap::get_id_name()
{
	static std::string idname = "itemId";
	return idname;
}

void BagMap::setPlayerId(int playerId)
{
	m_playerId = playerId;
	m_idFinder = BSON("player_id" << playerId);	
}
