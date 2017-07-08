#include "stdafx.h"
#include <mongo/db/jsobj.h>
#include "game_object_map.h"
#include <com_log.h>

game_object_map::game_object_map()
{

}
game_object_map::~game_object_map()
{
	clear_obj();
}

bool game_object_map::put_obj(GObjPtr object)
{
	auto it = m_cells.find(object->get_id());
	if (it != m_cells.end())
		return false;

	object->set_hanlder(this);
	m_cells.insert(std::make_pair(object->get_id(), object));
	
	return true;
}

GObjPtr game_object_map::find_obj(uint32_t object_id)
{
	auto it = m_cells.find(object_id);
	if (it != m_cells.end())
		return it->second;

	return nullptr;	
}

bool game_object_map::del_obj(uint32_t object_id)
{
	auto it = m_cells.find(object_id);
	if (it == m_cells.end())
		return false;
	
	m_cells.erase(it);
	return true;
}


bool game_object_map::have_obj(uint32_t object_id)
{
	auto it = m_cells.find(object_id);
	if (it != m_cells.end())
		return true;
	return false;	
}

//////////////////////////////////////////////////////////////////////////
bool game_object_map::from_bson(mongo::BSONObj& mb)
{
	if(!mb.hasField(get_cells_name()))
		return false;

	m_cells.clear();
	auto vec = mb.getField(get_cells_name()).Array();
	for (auto it = vec.begin(); it != vec.end(); ++ it)
	{
		mongo::BSONObj& tb = it->Obj();
		uint32_t objid = tb.getIntField(get_id_name());
		auto obj = create_game_object(objid);
		if(obj == nullptr)
		{
			SLOG_ERROR << get_cells_name() << "game_object_map::create_game_object objid:"<<objid;
			continue;
		}
		obj->from_bson(tb);
		obj->set_hanlder(this);
		m_cells.insert(std::make_pair(obj->get_id(), obj));
	}
	
	return true;
}

mongo::BSONObj game_object_map::to_bsonarr(bool to_all)
{
	mongo::BSONArrayBuilder ba;

	for (auto it = m_cells.begin(); it != m_cells.end(); ++it)
	{	
		if(!to_all)
			to_all = update_all();
		ba.append(it->second->to_bson(to_all));
	}

	return ba.obj();
}

void game_object_map::clear_obj()
{
	m_cells.clear();
}
