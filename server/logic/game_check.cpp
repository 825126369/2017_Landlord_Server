#include "stdafx.h"
#include "game_check.h"
#include "game_db.h"
#include "game_player.h"

enable_obj_pool_init(game_check, boost::null_mutex);
enable_obj_pool_init(check_map, boost::null_mutex);

game_check::game_check()
{
	init_game_object();
}

game_check::~game_check()
{
}

void game_check::init_game_object()
{
	CheckID = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "check_id"));
	CheckValue = CONVERT_POINT(Tfield<int16_t>, regedit_tfield(e_got_int16, "check_value"));
}

uint32_t game_check::get_id()
{
	return CheckID->get_value();
}

//////////////////////////////////////////////////////////////////////////
check_map::check_map()
{

}
check_map::~check_map()
{

}
const std::string& check_map::get_cells_name()		//数组名
{
	static std::string cellsname = "checks";
	return cellsname;
}
const std::string& check_map::get_id_name()		//array id 名
{
	static std::string idname = "check_id";
	return idname;
}

GObjPtr check_map::create_game_object(uint32_t object_id)
{
	auto sp = game_check::malloc();
	sp->CheckID->set_value(object_id);
	return sp;
}

const std::string& check_map::get_container_name()	//表名		
{
	return DB_PLAYER_INFO;
}
bool check_map::is_load()
{
	return m_player != nullptr;
}
uint32_t check_map::get_index_id()
{
	return m_player->PlayerId->get_value();
}

const std::string& check_map::get_index_name()
{
	return DB_PLAYER_INDEX;
}
db_base* check_map::get_db()
{
	return &db_player::instance();
}
const mongo::BSONObj& check_map::get_id_finder()
{
	return m_player->get_id_finder();
}

void check_map::attach(game_player* player)
{
	m_player = player;
}

int16_t check_map::get_check(uint32_t checkid)
{
	auto it = find_obj(checkid);
	if(it != nullptr)
	{
		auto ch = CONVERT_POINT(game_check , it);
		return ch->CheckValue->get_value();
	}
	
	return 0;
}

bool check_map::add_check(uint32_t checkid, int16_t v)
{
	auto it = find_obj(checkid);
	if(it != nullptr)
	{
		auto ch = CONVERT_POINT(game_check , it);
		ch->CheckValue->add_value(v);
		return db_update(ch);
	}
	else
	{
		auto ch = game_check::malloc();
		ch->CheckID->set_value(checkid);
		ch->CheckValue->add_value(v);
		if(put_obj(ch))
			return db_add(ch);
	}

	return true;
}

//////////////////////////////////////////////////////////////////////////
check_array::check_array()
{
	m_cellname = "check_list";
}
check_array::~check_array()
{

}
const std::string& check_array::get_cells_name()		//数组名
{
	return m_cellname;
}
const std::string& check_array::get_id_name()		//array id 名
{
	static std::string idname = "check_id";
	return idname;
}

GObjPtr check_array::create_game_object(uint32_t object_id)
{
	auto sp = game_check::malloc();
	sp->CheckID->set_value(object_id);
	return sp;
}

void check_array::init_array(std::string cellname)
{
	m_cellname = cellname;
}
int16_t check_array::get_check(uint32_t checkid)
{
	for (auto it =begin(); it != end(); ++it)
	{
		if((*it)->get_id() == checkid)
		{	 	
			auto ch = CONVERT_POINT(game_check , (*it));
			return ch->CheckValue->get_value();
		}
	}

	return 0;
}
bool check_array::add_check(uint32_t checkid, int16_t v )
{
	bool badd = false;
	for (auto it =begin(); it != end(); ++it)
	{
		if((*it)->get_id() == checkid)
		{	 	
			auto ch = CONVERT_POINT(game_check , (*it));
			ch->CheckValue->add_value(v);
			badd = true;
		}
	}

	if(!badd)
	{
		auto sp = game_check::malloc();
		sp->CheckID->set_value(checkid);
		sp->CheckValue->add_value(v);
		put_obj(sp);	
		badd = true;
	}
	return badd;
}
