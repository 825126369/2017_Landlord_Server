#include "stdafx.h"
#include "game_object.h"
#include <mongo/db/jsobj.h>
#include "game_object_field.h"
#include "game_object_array.h"
#include "game_object_map.h"

game_object::game_object()
	:Handler(nullptr)
	,m_has_update(false)
{
	
}

game_object::~game_object()
{
	m_fields.clear();
}

const std::string& game_object::get_errorfields()
{
	return m_errorfields;
}

mongo::BSONObj game_object::to_bson(bool to_all, bool use_ex)
{
	mongo::BSONObjBuilder ba;
	if(use_ex)
		to_bson_ex(ba);

	m_errorfields.clear();

	for (auto it = m_fields.begin(); it != m_fields.end(); ++it)
	{		
		GFieldPtr& goi = *it;

		if(!to_all && !goi->is_update())
			continue;

		goi->set_update(false);

		m_errorfields += goi->get_fieldname();

		switch (goi->get_fieldtype())
		{
		case e_got_int8:
			{
				Tfield<int8_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int8_t>, goi);				
				ba.appendIntOrLL(fh->get_fieldname(), fh->get_value());
			}
			break;
		case e_got_int16:
			{
				Tfield<int16_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int16_t>, goi);				
				ba.appendIntOrLL(fh->get_fieldname(), fh->get_value());
			}
			break;
		case e_got_int32:
			{
				Tfield<int32_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int32_t>, goi);				
				ba.appendIntOrLL(fh->get_fieldname(), fh->get_value());
			}
			break;
		case e_got_int64:
			{
				Tfield<int64_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int64_t>, goi);				
				ba.append(fh->get_fieldname(), fh->get_value());
			}
			break;
		case e_got_bool:
			{
				Tfield<bool>::TFieldPtr fh = CONVERT_POINT(Tfield<bool>, goi);				
				ba.appendBool(fh->get_fieldname(), fh->get_value());
			}
			break;
		case e_got_double:
			{
				Tfield<double>::TFieldPtr fh = CONVERT_POINT(Tfield<double>, goi);				
				ba.append(fh->get_fieldname(), fh->get_value());
			}
			break;
		case e_got_date:
			{
				Tfield<time_t>::TFieldPtr fh = CONVERT_POINT(Tfield<time_t>, goi);				
				ba.appendTimeT(fh->get_fieldname(), fh->get_value());
			}
			break;
		case e_got_string:
			{
				GStringFieldPtr fh = CONVERT_POINT(field_string, goi);				
				ba.append(fh->get_fieldname(), fh->get_string());	
			}
			break;
		case e_got_array:
			{
				GArrayFieldPtr fh = CONVERT_POINT(field_array, goi);				
				ba.appendArray(fh->get_fieldname(), fh->get_array()->to_bsonarr(true));				
			}
			break;
		case e_got_map:
			{
				GMapFieldPtr fh = CONVERT_POINT(field_map, goi);				
				ba.appendArray(fh->get_fieldname(), fh->get_map()->to_bsonarr(true));				
			}
			break;
		case e_got_object:
			{
				GObjFieldPtr fh = CONVERT_POINT(field_obj, goi);				
				ba.append(fh->get_fieldname(), fh->get_obj()->to_bson(true));				
			}
			break;
		case e_got_oid:
			{
				GOIDFieldPtr fh = CONVERT_POINT(field_oid, goi);
				ba.append(fh->get_fieldname(), fh->get_oid());
			}
			break;
		case e_got_intlist:
			{
				GIntListFieldPtr fh = CONVERT_POINT(field_intlist, goi);
				ba.append(fh->get_fieldname(), *fh->get_intlist());
			}
			break;
		}
	}
	m_has_update = false;
	return ba.obj();
}

bool game_object::from_bson(mongo::BSONObj& s)
{
	for (auto it = m_fields.begin(); it != m_fields.end(); ++it)
	{
		GFieldPtr& goi = *it;

		if(!s.hasField(goi->get_fieldname()))
			continue;

		switch (goi->get_fieldtype())
		{
		case e_got_int8:
			{
				Tfield<int8_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int8_t>, goi);				
				fh->set_value( s.getIntField(fh->get_fieldname()));
			}
			break;
		case e_got_int16:
			{
				Tfield<int16_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int16_t>, goi);				
				fh->set_value( s.getIntField(fh->get_fieldname()));
			}
			break;
		case e_got_int32:
			{
				Tfield<int32_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int32_t>, goi);				
				fh->set_value( s.getIntField(fh->get_fieldname()));
			}
			break;
		case e_got_int64:
			{
				Tfield<int64_t>::TFieldPtr fh = CONVERT_POINT(Tfield<int64_t>, goi);				
				fh->set_value( s.getField(fh->get_fieldname()).Long());
			}
			break;
		case e_got_bool:
			{
				Tfield<bool>::TFieldPtr fh = CONVERT_POINT(Tfield<bool>, goi);				
				fh->set_value( s.getBoolField(fh->get_fieldname()));
			}
			break;	
		case e_got_double:
			{
				Tfield<double>::TFieldPtr fh = CONVERT_POINT(Tfield<double>, goi);				
				fh->set_value( s.getField(fh->get_fieldname()).Double());
			}
			break;
		case e_got_date:
			{
				Tfield<time_t>::TFieldPtr fh = CONVERT_POINT(Tfield<time_t>, goi);				
				fh->set_value( s.getField(fh->get_fieldname()).Date().toTimeT() );
			}
			break;		
		case e_got_string:
			{
				GStringFieldPtr fh = CONVERT_POINT(field_string, goi);			
				fh->set_string( s.getStringField(fh->get_fieldname()));
			}
			break;
		case e_got_array:
			{
				GArrayFieldPtr fh = CONVERT_POINT(field_array, goi);				
				fh->get_array()->from_bson(s);			
			}
			break;	
		case e_got_map:
			{
				GMapFieldPtr fh = CONVERT_POINT(field_map, goi);				
				fh->get_map()->from_bson(s);			
			}
			break;	
		case e_got_object:
			{
				GObjFieldPtr fh = CONVERT_POINT(field_obj, goi);					
				fh->get_obj()->from_bson(s.getField(fh->get_fieldname()).Obj());			
			}
			break;	
		case e_got_oid:
			{
				GOIDFieldPtr fh = CONVERT_POINT(field_oid, goi);					
				fh->set_oid(s.getField(fh->get_fieldname()).OID());			
			}
			break;
		case e_got_intlist:
			{
				GIntListFieldPtr fh = CONVERT_POINT(field_intlist, goi);				
				auto vec = s.getField(fh->get_fieldname()).Array();
				auto ilist= fh->get_intlist();
				ilist->clear();
				for (auto it = vec.begin(); it != vec.end(); ++ it)
				{					
					ilist->push_back(it->numberInt());
				}
					
			}
			break;
		}

		goi->set_update(false);
	}
	m_has_update = false;

	return true;
}

//e_game_object_type
GFieldPtr game_object::regedit_tfield(e_game_object_type etype, const std::string& fieldname)
{
	GFieldPtr fp;
	switch (etype)
	{
	case e_got_int8:
		fp = Tfield<int8_t>::malloc();	
		break;
	case e_got_int16:
		fp = Tfield<int16_t>::malloc();	
		break;
	case e_got_int32:
		fp = Tfield<int32_t>::malloc();	
		break;
	case e_got_int64:
		fp = Tfield<int64_t>::malloc();	
		break;
	case e_got_bool:
		fp = Tfield<bool>::malloc();	
		break;
	case e_got_double:
		fp = Tfield<double>::malloc();	
		break;
	case e_got_date:
		fp = Tfield<time_t>::malloc();	
		break;
	default:
		return nullptr;
		break;
	}

	if(!fp->init_field(etype, fieldname, this))
		return nullptr;

	m_fields.push_back(fp);

	return fp;
}

//string
GStringFieldPtr game_object::regedit_strfield(const std::string& fieldname)
{
	auto ap  =field_string::malloc();
	if(!ap->init_strfield(fieldname, this))
		return nullptr;

	m_fields.push_back(ap);

	return ap;
}

//mongodb array
GArrayFieldPtr game_object::regedit_arrfield(const std::string& fieldname, GArrayObjPtr arrptr)
{
	auto ap  =field_array::malloc();
	if(!ap->init_arrayfield(fieldname, arrptr, this))
		return nullptr;

	m_fields.push_back(ap);

	return ap;
}

//mongodb array -> map
GMapFieldPtr game_object::regedit_mapfield(const std::string& fieldname, GMapObjPtr mapptr)
{
	auto ap  =field_map::malloc();
	if(!ap->init_mapfield(fieldname, mapptr, this))
		return nullptr;

	m_fields.push_back(ap);

	return ap;
}

//game_object
GObjFieldPtr game_object::regedit_objfield(const std::string& fieldname, GObjPtr objptr)
{
	auto ap  =field_obj::malloc();
	if(!ap->init_objfield(fieldname, objptr, this))
		return nullptr;

	m_fields.push_back(ap);

	return ap;
}

//OID
GOIDFieldPtr game_object::regedit_oidfield(std::string fieldname)
{
	auto ap  =field_oid::malloc();
	if(!ap->init_oidfield(fieldname, this))
		return nullptr;

	m_fields.push_back(ap);

	return ap;
}
//intlist
GIntListFieldPtr game_object::regedit_intlistfield(const std::string& fieldname)
{
	auto ap  =field_intlist::malloc();
	if(!ap->init_intlistfield(fieldname, this))
		return nullptr;

	m_fields.push_back(ap);

	return ap;
}

void game_object::set_hanlder(game_object_handler* hanlder)
{
	Handler = hanlder;
}

bool game_object::store_game_object(bool to_all)
{
	if(Handler == nullptr)
		return false;

	return Handler->db_update(this);
}

bool game_object::has_update()
{
	return m_has_update;
	//auto fit = std::find_if(m_fields.begin(), m_fields.end(), [](GFieldPtr gfp)->bool{ return gfp->is_update();});
	//return fit != m_fields.end();
}

void game_object::set_update(bool is_update)
{
	m_has_update = is_update;
}