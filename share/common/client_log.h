#pragma once
#include <exception>
#include "cocos2d.h"
#include "Tools.h"
#include <boost/format.hpp>

class client_log
{
public:
	inline void operator<<(const char* str)
	{
		std::string utf8 = Tools::str2utf8(str, "");
		CCLOG("%s", utf8.c_str());
	}
	inline void operator<<(const boost::format& str)
	{
		std::string utf8 = Tools::str2utf8(std::string(str.str().c_str()), "");
		CCLOG("%s", utf8.c_str());
	}
	static client_log msLog;
};

#define SLOG_ALERT client_log::msLog
#define SLOG_CRITICAL client_log::msLog
#define SLOG_ERROR client_log::msLog
#define SLOG_WARNING client_log::msLog
#define SLOG_NOTICE client_log::msLog
#define SLOG_INFO client_log::msLog
#define SLOG_DEBUG client_log::msLog

//////////////////////////////////////////////////////////////////////////
#include <exception>
#define LOG_FUNCTION CCLOG("%s:%d %s", __FILE__, __LINE__, __FUNCTION__);

#define __ENTER_FUNCTION try{
#define __LEAVE_FUNCTION }catch(std::exception* ex){LOG_FUNCTION; SLOG_ERROR<<ex->what();}

#define __ENTER_FUNCTION_CHECK bool EX_CHECK = false;try{
#define __LEAVE_FUNCTION_CHECK }catch(std::exception* ex){LOG_FUNCTION; SLOG_ERROR<<ex->what();EX_CHECK = true;}

#define __ASSERT(exp, msg) if(!(exp)){LOG_FUNCTION; SLOG_ALERT<<msg; throw new std::exception();}