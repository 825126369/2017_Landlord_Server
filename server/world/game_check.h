#pragma once
#include "game_def.h"
#include "game_object_container.h"


class game_check
	:public game_object
	,public enable_obj_pool<game_check>
{
public:
	game_check();
	virtual ~game_check();

	virtual void init_game_object();//注册属性
	virtual uint32_t get_id();

	Tfield<int32_t>::TFieldPtr CheckID;
	Tfield<int16_t>::TFieldPtr CheckValue;

};

//////////////////////////////////////////////////////////////////////////
class check_map
	:public game_object_container
	,public enable_obj_pool<check_map>
{
public:
	check_map();
	virtual ~check_map();

	virtual const std::string& get_cells_name();		//数组名
	virtual const std::string& get_id_name();		//map key 名
	virtual GObjPtr create_game_object(uint32_t object_id);
	virtual const std::string& get_container_name();	//表名		
	virtual bool is_load();		
	virtual uint32_t get_index_id();
	virtual const std::string& get_index_name();
	virtual db_base* get_db();
	virtual const mongo::BSONObj& get_id_finder();

	void attach(game_player* player);
	int16_t get_check(uint32_t checkid);
	bool add_check(uint32_t checkid, int16_t v = 1);

private:
	game_player* m_player;
};

//////////////////////////////////////////////////////////////////////////

class check_array
	:public game_object_array
	,public enable_obj_pool<check_array>
{
public:
	check_array();
	virtual ~check_array();

	virtual const std::string& get_cells_name();		//数组名
	virtual const std::string& get_id_name();		//map key 名
	virtual GObjPtr create_game_object(uint32_t object_id);

	void init_array(std::string cellname);
	int16_t get_check(uint32_t checkid);
	bool add_check(uint32_t checkid, int16_t v = 1);
private:
	std::string m_cellname;
};