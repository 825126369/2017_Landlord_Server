#include "stdafx.h"
#include "tool_helper.h"
#include <boost/regex.hpp>
#include "time_helper.h"

// 2014-01-01 12:00:00
static boost::regex DATE_TIME( "^\\s*(\\d{4})-(\\d{1,2})-(\\d{1,2})\\s+(\\d{1,2}):(\\d{1,2}):(\\d{1,2})$");

bool ToolHelper::parseTime(const std::string* strStart, 
						   const std::string* strEnd, 
						   time_t* outStartTime, 
						   time_t* outEndTime,
						   bool emtpyValid)
{
	bool res1 = parseTime(strStart, outStartTime, emtpyValid);
	bool res2 = parseTime(strEnd, outEndTime, emtpyValid);
	return res1 && res2;
}

bool ToolHelper::parseTime(const std::string* strTime, time_t* outTime, bool emtpyValid)
{
	if(!strTime || !outTime)
		return false;

	if(emtpyValid && strTime->empty())
		return true;

	bool res = regex_match(strTime->c_str(), DATE_TIME);
	if(!res)
	{
		SLOG_ERROR << boost::format("ToolHelper::parseTime1 [time1=%1%]格式不对") % *strTime;
		return false;
	}

	boost::posix_time::ptime tm = time_helper::convert_to_ptime(*strTime);
	*outTime = time_helper::convert_from_ptime(tm);
	return true;
}

bool ToolHelper::isSameActivity(time_t oldStartTime, time_t oldEndTime, time_t newStartTime, time_t newEndTime)
{
	if(oldEndTime <= newStartTime)
		return false;

	if(newEndTime <= oldStartTime)
		return false;

	return true;
}

int ToolHelper::getAddSkillPoint(time_t startT, time_t curT, int interval)
{
	if(interval == 0)
		return 0;

	double delta = curT - startT + 1;
	if(delta < 0)
		return 0;

	return (int)ceil(delta / interval);
}
