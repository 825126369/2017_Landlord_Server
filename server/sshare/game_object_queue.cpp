#include "stdafx.h"
#include "game_object_queue.h"
#include "db_base.h"
#include <com_log.h>

game_object_queue::game_object_queue()
{

}

game_object_queue::~game_object_queue()
{

}

bool game_object_queue::db_pop()
{
	if(!is_load() || !pop_obj())
		return false;
	
//	auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$pop" << BSON(get_cells_name()<<BSON("key"<<-1))));
	auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$pop" << BSON(get_cells_name() <<-1)));
	if(!ret.empty())
	{
		SLOG_ERROR << "game_object_queue::$pop error:"<<ret;
		return false;
	}
	
	return true;
}

bool game_object_queue::db_push(GObjPtr obj)
{
	if(!is_load()|| obj == nullptr)
		return false;
	
	auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$push"<<BSON(get_cells_name()<< obj->to_bson())));
	if(!ret.empty())
	{
		SLOG_ERROR << "game_object_container::$push id:"<<obj->get_id() <<" error:"<<ret;
		return false;
	}	

	return true;
}

bool game_object_queue::db_push(game_object* obj)
{
	if(!is_load()|| obj == nullptr)
		return false;
	
	auto ret =  get_db()->update(get_container_name(), get_id_finder(), BSON("$push"<<BSON(get_cells_name()<< obj->to_bson())));
	if(!ret.empty())
	{
		SLOG_ERROR << "game_object_container::$push id:"<<obj->get_id() <<" error:"<<ret;
		return false;
	}	

	return true;
}