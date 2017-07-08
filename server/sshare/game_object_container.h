#pragma once
#include "game_object_map.h"

class db_base;
class game_object_container:
	public game_object_map
{	
public:
	game_object_container();
	virtual ~game_object_container();
	
	virtual const std::string& get_container_name()=0;	//±íÃû		
	virtual bool is_load()= 0;		
	virtual uint32_t get_index_id() = 0;
	virtual const std::string& get_index_name() = 0;
	virtual const mongo::BSONObj& get_id_finder() = 0;
	virtual db_base* get_db() = 0;

	//game_object_hanlder
	virtual bool db_add(GObjPtr obj);
	virtual bool db_update(GObjPtr obj);
	virtual bool db_del(GObjPtr obj);

	virtual bool db_add(game_object* obj);
	virtual bool db_update(game_object* obj);
	virtual bool db_del(game_object* obj);

	void reset_all();

	virtual void set_update(bool bupdate = true);
private:
	bool m_has_update;
};
