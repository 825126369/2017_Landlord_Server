#include "stdafx.h"
#include "game_object_container.h"
#include "db_base.h"
#include <com_log.h>

game_object_container::game_object_container()
	:m_has_update(false)
{

}

game_object_container::~game_object_container()
{

}


bool game_object_container::db_update(GObjPtr obj)
{
	if(!is_load() || obj == nullptr)
		return false;
	//先逻辑判断 减少数据库操作 
	auto fb = BSON(get_index_name() << get_index_id() << get_cells_name()+"."+get_id_name() << obj->get_id());
	//if( get_db()->get_count(get_container_name(), fb) > 0)
	{
		auto ret =  get_db()->update(get_container_name(), fb, BSON("$set" << BSON(get_cells_name()+".$" << obj->to_bson(true))));
		if(!ret.empty())
		{
			SLOG_ERROR << "game_object_container::$set id:"<<obj->get_id() << " error:"<<ret;
			return false;
		}
	}

	obj->set_update(false);
	return true;
}

bool game_object_container::db_add(GObjPtr obj)
{
	if(!is_load()|| obj == nullptr)
		return false;

	//先逻辑判断 减少数据库操作 
	//auto fb = BSON("player_id" << get_playerid() << get_cells_name()+"."+get_id_name() << obj->get_id());
	//if( get_db()->get_count(get_container_name(), fb) == 0)
	{
		auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$push"<<BSON(get_cells_name()<< obj->to_bson(true))));
		if(!ret.empty())
		{
			SLOG_ERROR << "game_object_container::$push id:"<<obj->get_id() <<" error:"<<ret;
			return false;
		}
	}

	return true;
}

bool game_object_container::db_del(GObjPtr obj)
{
	if(!is_load()|| obj == nullptr)
		return false;

	auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$pull" << BSON( get_cells_name() << BSON( get_id_name() << obj->get_id()))));
	if(!ret.empty())
	{
		SLOG_ERROR << "game_object_container::$pull id:"<<obj->get_id()<<" error:"<< ret;
		return false;
	}
	return true;
}


bool game_object_container::db_update(game_object* obj)
{
	if(!is_load()|| obj == nullptr)
		return false;
	//先逻辑判断 减少数据库操作 
	auto fb = BSON(get_index_name() << get_index_id() << get_cells_name()+"."+get_id_name() << obj->get_id());
	//if( get_db()->get_count(get_container_name(), fb) > 0)
	{
		auto ret =  get_db()->update(get_container_name(), fb, BSON("$set" << BSON(get_cells_name()+".$" << obj->to_bson(true))));
		if(!ret.empty())
		{
			SLOG_ERROR << "game_object_container::$set id:"<<obj->get_id() << " error:"<<ret;
			return false;
		}
	}
	obj->set_update(false);
	return true;
}

bool game_object_container::db_add(game_object* obj)
{
	if(!is_load()|| obj == nullptr)
		return false;

	//先逻辑判断 减少数据库操作 
	//auto fb = BSON("player_id" << get_playerid() << get_cells_name()+"."+get_id_name() << obj->get_id());
	//if( get_db()->get_count(get_container_name(), fb) == 0)
	{
		auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$push"<<BSON(get_cells_name()<< obj->to_bson(true))));
		if(!ret.empty())
		{
			SLOG_ERROR << "game_object_container::$push id:"<<obj->get_id() <<" error:"<<ret;
			return false;
		}
	}

	return true;
}

bool game_object_container::db_del(game_object* obj)
{
	if(!is_load()|| obj == nullptr)
		return false;

	auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$pull" << BSON( get_cells_name() << BSON( get_id_name() << obj->get_id()))));
	if(!ret.empty())
	{
		SLOG_ERROR << "game_object_container::$pull id:"<<obj->get_id()<<" error:"<< ret;
		return false;
	}
	return true;
}

void game_object_container::reset_all()
{
	if(!is_load() ||!m_has_update)
		return;	

	mongo::BSONObjBuilder ba;
	ba.appendArray(get_cells_name(), to_bsonarr(true));

	auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$set" << ba.done()));
	if(!ret.empty())
	{
		SLOG_ERROR << "game_object_container::reset_all error:"<< ret;
	}
}


void game_object_container::set_update(bool bupdate)
{
	m_has_update = bupdate;
}
