#pragma once
#include "game_object_def.h"
#include <enable_object_pool.h>
#include <boost/function.hpp>

class field_base
{
public:
	field_base();
	virtual ~field_base();

	e_game_object_type get_fieldtype();
	const std::string& get_fieldname();
	bool is_update();
	void set_update(bool update = true);

	void set_fieldhandler(boost::function0<void> callback);//更新字段时的回调
	virtual bool init_field(e_game_object_type et, const std::string& fieldname, game_object* gowner);
private:
	e_game_object_type m_fieldtype;
	std::string m_fieldname;
	bool m_update;
	game_object* m_owner;
protected:
	boost::function0<void> m_func;
};

//////////////////////////////////////////////////////////////////////////
template<typename T>
class Tfield
	:public field_base
	,public enable_obj_pool<Tfield<T>>
{
public:
	Tfield(){m_v = 0;};
	virtual ~Tfield(){};

	typedef boost::shared_ptr<Tfield<T>> TFieldPtr;

	virtual bool init_field(e_game_object_type et, const std::string& fieldname, game_object* gowner)
	{
		if(et<=e_got_none || et>=e_got_string)
			return false;

		field_base::init_field(et, fieldname, gowner);
		return true;
	}

	T get_value()
	{
		return m_v;
	}
	void set_value(T v, bool needupdate = true)
	{
		m_v = v;
		if(needupdate)
			set_update();
	}
	void add_value(T v)
	{
		m_v += v;
		set_update();
	}
private:
	T m_v;
};

//////////////////////////////////////////////////////////////////////////
class field_string
	:public field_base
	,public enable_obj_pool<field_string>
{
public:
	field_string();
	virtual ~field_string();

	bool init_strfield(const std::string& fieldname, game_object* gowner);
	void set_string(const std::string& v, bool isupdate = true);
	const std::string& get_string();

private:
	std::string m_v;
};

//////////////////////////////////////////////////////////////////////////
#include <mongo/db/jsobj.h>
class field_oid
	:public field_base
	,public enable_obj_pool<field_oid>
{
public:
	field_oid();
	virtual ~field_oid();

	bool init_oidfield(const std::string& fieldname, game_object* gowner);
	void gen_oid();
	void set_oid(const mongo::OID& v);
	const mongo::OID& get_oid();

private:
	mongo::OID m_v;
};


//////////////////////////////////////////////////////////////////////////
class field_intlist
	:public field_base
	,public enable_obj_pool<field_intlist>
{
public:
	field_intlist();
	virtual ~field_intlist();

	bool init_intlistfield(const std::string& fieldname, game_object* gowner);
	std::vector<int>* get_intlist();

	bool check(int v);
	int get(int index);
	void put(int v);
	int count();
	void clear();
private:
	std::vector<int> m_v;
};

//////////////////////////////////////////////////////////////////////////
class field_array
	:public field_base
	,public enable_obj_pool<field_array>
{
public:
	field_array();
	virtual ~field_array();
	bool init_arrayfield(const std::string& fieldname, GArrayObjPtr default_v, game_object* gowner);

	void set_array();//非管理array 需要设置标识才能增量更新
	GArrayObjPtr get_array();

	template<typename T>
	boost::shared_ptr<T> get_Tarray()
	{
		return CONVERT_POINT(T, m_v);
	}

private:
	GArrayObjPtr m_v;
};

//////////////////////////////////////////////////////////////////////////
class field_map
	:public field_base
	,public enable_obj_pool<field_map>
{
public:
	field_map();
	virtual ~field_map();
	bool init_mapfield(const std::string& fieldname, GMapObjPtr default_v, game_object* gowner);

	void set_map();//非管理map 需要设置标识才能增量更新
	GMapObjPtr get_map();

	template<typename T>
	boost::shared_ptr<T> get_Tmap()
	{
		return CONVERT_POINT(T, m_v);
	}
private:
	GMapObjPtr m_v;
};

//////////////////////////////////////////////////////////////////////////
class field_obj
	:public field_base
	,public enable_obj_pool<field_obj>
{
public:
	field_obj();
	virtual ~field_obj();
	bool init_objfield(const std::string& fieldname, GObjPtr default_v, game_object* gowner);

	void set_obj( GObjPtr v);//非管理obj 需要设置标识才能增量更新
	GObjPtr get_obj();

	template<typename T>
	boost::shared_ptr<T> get_Tobj()
	{
		return CONVERT_POINT(T, m_v);
	}
private:
	GObjPtr m_v;
};