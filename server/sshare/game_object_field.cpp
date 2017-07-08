#include "stdafx.h"
#include "game_object_field.h"
#include "game_object.h"

//enable_obj_pool_init(Tfield<int8_t>, boost::null_mutex);
//enable_obj_pool_init(Tfield<int16_t>, boost::null_mutex);
//enable_obj_pool_init(Tfield<int32_t>, boost::null_mutex);
//enable_obj_pool_init(Tfield<int64_t>, boost::null_mutex);
//enable_obj_pool_init(Tfield<bool>, boost::null_mutex);
//enable_obj_pool_init(Tfield<double>, boost::null_mutex);
//enable_obj_pool_init(Tfield<time_t>, boost::null_mutex);
//
//enable_obj_pool_init(field_string, boost::null_mutex);
//enable_obj_pool_init(field_array, boost::null_mutex);
//enable_obj_pool_init(field_oid, boost::null_mutex);

//////////////////////////////////////////////////////////////////////////
field_base::field_base()
	:m_fieldtype(e_got_none)
	,m_update(false)
	,m_owner(nullptr)
{

}
field_base::~field_base()
{
	m_func.clear();
}

bool field_base::init_field(e_game_object_type et, const std::string& fieldname, game_object* gowner)
{
	m_fieldname = fieldname;
	m_fieldtype = et;
	m_owner = gowner;
	return true;
}

e_game_object_type field_base::get_fieldtype()
{
	return m_fieldtype;
}
const std::string& field_base::get_fieldname()
{
	return m_fieldname;
}

bool field_base::is_update()
{
	return m_update;
}
void field_base::set_update(bool update)
{
	m_update = update;

	if(update && !m_func.empty())
		m_func();

	if(m_owner != nullptr && update)
		m_owner->set_update();
}

void field_base::set_fieldhandler(boost::function0<void> callback)
{
	m_func = callback;
}

//////////////////////////////////////////////////////////////////////////
field_string::field_string()
{

}
field_string::~field_string()
{
	m_v.clear();
}

bool field_string::init_strfield(const std::string& fieldname, game_object* gowner)
{
	field_base::init_field(e_got_string, fieldname, gowner);
	return true;
}

void field_string::set_string(const std::string& v, bool isupdate)
{
	m_v = v;
	set_update(isupdate);
}

const std::string& field_string::get_string()
{
	return m_v;
}

//////////////////////////////////////////////////////////////////////////
field_intlist::field_intlist()
{

}
field_intlist::~field_intlist()
{

}

bool field_intlist::init_intlistfield(const std::string& fieldname, game_object* gowner)
{
	field_base::init_field(e_got_intlist, fieldname, gowner);
	return true;
}

std::vector<int>* field_intlist::get_intlist()
{
	return &m_v;
}

bool field_intlist::check(int v)
{
	for (int i=0;i<count();i++)
	{
		if(m_v[i] == v)
			return true;
	}
	return false;
}

int field_intlist::get(int index)
{
	if(index>=0 && index<count())
		return m_v[index];
	return 0;
}

void field_intlist::put(int v)
{
	m_v.push_back(v);
	set_update();
}

int field_intlist::count()
{
	return m_v.size();
}
void field_intlist::clear()
{
	m_v.clear();
	set_update();
}

//////////////////////////////////////////////////////////////////////////
field_oid::field_oid()
{

}
field_oid::~field_oid()
{

}

bool field_oid::init_oidfield(const std::string& fieldname, game_object* gowner)
{
	field_base::init_field(e_got_oid, fieldname, gowner);
	return true;
}

void field_oid::gen_oid()
{
	m_v.init();
	set_update();
}

void field_oid::set_oid(const mongo::OID& v)
{
	m_v = v;
	set_update();
}

const mongo::OID& field_oid::get_oid()
{
	return m_v;
}

//////////////////////////////////////////////////////////////////////////
field_array::field_array()
{

}
field_array::~field_array()
{
	m_v.reset();
}
bool field_array::init_arrayfield(const std::string& fieldname, GArrayObjPtr default_v, game_object* gowner)
{
	if(default_v == nullptr)
		return false;

	field_base::init_field(e_got_array, fieldname, gowner);
	m_v = default_v;
	return true;
}

void field_array::set_array()
{
	//数组设置只是标示
	set_update();
}

GArrayObjPtr field_array::get_array()
{
	return m_v;
}

//////////////////////////////////////////////////////////////////////////
field_map::field_map()
{

}
field_map::~field_map()
{
	m_v.reset();
}
bool field_map::init_mapfield(const std::string& fieldname, GMapObjPtr default_v, game_object* gowner)
{
	if(default_v == nullptr)
		return false;

	field_base::init_field(e_got_map, fieldname, gowner);
	m_v = default_v;
	return true;
}

void field_map::set_map()
{
	//数组设置只是标示
	set_update();
}

GMapObjPtr field_map::get_map()
{
	return m_v;
}

//////////////////////////////////////////////////////////////////////////
field_obj::field_obj()
{

}
field_obj::~field_obj()
{
	m_v.reset();
}
bool field_obj::init_objfield(const std::string& fieldname, GObjPtr default_v, game_object* gowner)
{
	if(default_v == nullptr)
		return false;

	field_base::init_field(e_got_object, fieldname, gowner);
	m_v = default_v;
	return true;
}

void field_obj::set_obj( GObjPtr v)//非管理obj 需要设置标识才能增量更新
{
	m_v = v;
	set_update();
}
GObjPtr field_obj::get_obj()
{
	return m_v;
}