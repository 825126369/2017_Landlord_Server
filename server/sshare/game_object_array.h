#pragma once
#include <enable_smart_ptr.h>
#include <vector>
#include "game_object.h"
#include "game_object_handler.h"

class game_object_array
	:public game_object_handler
{
public:
	typedef std::vector<GObjPtr> cell_vec;
protected:
	cell_vec m_cells;
public:
	typedef cell_vec::iterator cell_it;
	
public:
	game_object_array();
	virtual ~game_object_array();

	virtual const std::string& get_cells_name()=0;		//数组名
	virtual const std::string& get_id_name()=0;		//array id 名
	virtual GObjPtr create_game_object(uint32_t object_id) = 0;	//通过id创建数组对象

	virtual bool update_all(){return false;}//如果是第2层以上的关系 必须重载为true

	uint32_t get_obj_count(){return m_cells.size();}
	bool put_obj(GObjPtr object);
	bool del_obj_by_index(uint32_t index);
	GObjPtr get_obj(uint32_t index);

	template<class T>
	boost::shared_ptr<T> get_Tobj(uint32_t index)
	{
		return CONVERT_POINT(T, get_obj(index));
	}

	template<class T>
	boost::shared_ptr<T> find_Tobj(uint32_t object_id)
	{
		return CONVERT_POINT(T, find_obj(object_id));
	}

	GObjPtr find_obj(uint32_t object_id);

	void clear_obj();
	bool pop_obj();

	cell_it begin(){return m_cells.begin();}
	cell_it end(){return m_cells.end();}

	mongo::BSONObj to_bsonarr(bool to_all = false);
	bool from_bson(mongo::BSONObj& mb);

	cell_vec& getAll(){ return m_cells; }
};
