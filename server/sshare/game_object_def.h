#pragma once
#include <boost/smart_ptr.hpp>
#include <game_macro.h>

#include <string>

enum e_game_object_type
{
	e_got_none = 0,
	e_got_int8,
	e_got_int16,
	e_got_int32,
	e_got_int64,
	e_got_bool,
	e_got_double,
	e_got_date,			//time_t

	e_got_string,
	e_got_array,
	e_got_map,

	e_got_object,		//gameobj

	e_got_oid,			//oid
	e_got_intlist,		//std::list<int>
};

namespace mongo
{
	class BSONObj;
	class BSONObjBuilder;
};

class game_object;
class game_object_array;
class game_object_map;
class field_base;
class field_string;
class field_array;
class field_map;
class field_obj;
class field_oid;
class field_intlist;

typedef boost::shared_ptr<game_object> GObjPtr;
typedef boost::shared_ptr<game_object_array> GArrayObjPtr;
typedef boost::shared_ptr<game_object_map> GMapObjPtr;
typedef boost::shared_ptr<field_base> GFieldPtr;
typedef boost::shared_ptr<field_string> GStringFieldPtr;
typedef boost::shared_ptr<field_array> GArrayFieldPtr;
typedef boost::shared_ptr<field_map> GMapFieldPtr;
typedef boost::shared_ptr<field_obj> GObjFieldPtr;
typedef boost::shared_ptr<field_oid> GOIDFieldPtr;
typedef boost::shared_ptr<field_intlist> GIntListFieldPtr;


#ifndef GOLD_OBJ_TYPE
#define GOLD_OBJ_TYPE e_got_int64
#endif
