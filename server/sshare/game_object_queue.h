#pragma once
#include "game_object_array.h"

class db_base;
class game_object_queue:
	public game_object_array
{	
public:
	game_object_queue();
	virtual ~game_object_queue();
	
	virtual const std::string& get_container_name()=0;	//±íÃû		
	virtual bool is_load()= 0;		
	virtual uint32_t get_index_id() = 0;
	virtual const mongo::BSONObj& get_id_finder() = 0;
	virtual db_base* get_db() = 0;


	//game_object_hanlder
	virtual bool db_pop();
	virtual bool db_push(GObjPtr obj);
	virtual bool db_push(game_object* obj);
};
