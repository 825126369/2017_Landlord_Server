#include "stdafx.h"
#include "operation_activity_def.h"
#include <boost/regex.hpp>

// "^\s*(\d{4})/(\d{1,2})/(\d{1,2})\s+(\d{1,2}):(\d{1,2})$"

boost::regex DATE_TIME2( "^\\s*(\\d{4})/(\\d{1,2})/(\\d{1,2})\\s+(\\d{1,2}):(\\d{1,2})$");

// 2014-01-01 12:00:00
boost::regex DATE_TIME( "^\\s*(\\d{4})-(\\d{1,2})-(\\d{1,2})\\s+(\\d{1,2}):(\\d{1,2}):(\\d{1,2})$");

ActivityStoreInfo::ActivityStoreInfo()
{
	init_game_object();
}

void ActivityStoreInfo::init_game_object()
{
	m_activityId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "activityId"));

	m_param1 = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "param1"));
	
	m_param2 = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "param2"));

	m_isFinish = CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "isFinish"));
	m_isFinish->set_value(false);
}

const std::string& ActivityStoreInfoArray::get_cells_name()
{
	static std::string cellsname = "activityInfo";
	return cellsname;
}

const std::string& ActivityStoreInfoArray::get_id_name()
{
	static std::string idname = "activityId";
	return idname;
}

GObjPtr ActivityStoreInfoArray::create_game_object(uint32_t object_id)
{
	auto sp = ActivityStoreInfo::malloc();
	return sp;
}
