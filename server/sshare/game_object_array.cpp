#include "stdafx.h"
#include <mongo/db/jsobj.h>
#include "game_object_array.h"
#include <com_log.h>

game_object_array::game_object_array()
{

}
game_object_array::~game_object_array()
{
	clear_obj();
}

bool game_object_array::put_obj(GObjPtr object)
{
	object->set_hanlder(this);
	m_cells.push_back(object);
	
	return true;
}

bool game_object_array::del_obj_by_index(uint32_t index)
{
	if(index < m_cells.size())
	{
		m_cells.erase(m_cells.begin() + index);
		return true;
	}
	return false;
}

bool game_object_array::pop_obj()
{
	if(m_cells.empty())
		return false;

	m_cells.erase(m_cells.begin());
	return true;
}

GObjPtr game_object_array::get_obj(uint32_t index)
{
	if(index<0 || index>=m_cells.size())
		return nullptr;

	return m_cells[index];	
}


GObjPtr game_object_array::find_obj(uint32_t object_id)
{
	for (auto it = m_cells.begin();it != m_cells.end(); ++it)
	{
		if((*it)->get_id() == object_id)
			return *it;
	}

	return nullptr;	
}

//////////////////////////////////////////////////////////////////////////
bool game_object_array::from_bson(mongo::BSONObj& mb)
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
			SLOG_ERROR << get_cells_name() << "game_object_array::create_game_object objid:"<<objid;
			continue;
		}
		obj->from_bson(tb);
		obj->set_hanlder(this);
		m_cells.push_back(obj);
	}
	
	return true;
}

mongo::BSONObj game_object_array::to_bsonarr(bool to_all)
{
	mongo::BSONArrayBuilder ba;

	for (auto it = m_cells.begin(); it != m_cells.end(); ++it)
	{		
		if(!to_all)
			to_all = update_all();
		ba.append((*it)->to_bson(to_all));
	}

	return ba.obj();
}

void game_object_array::clear_obj()
{
	m_cells.clear();
}