#include "stdafx.h"
#include "com_log.h"

#include <stdexcept>
#include <iostream>
#include <enable_smart_ptr.h>

#include <boost/log/common.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/utility/setup/file.hpp>
#include <boost/log/utility/setup/console.hpp>
#include <boost/log/utility/setup/common_attributes.hpp>
#include <boost/log/attributes/timer.hpp>
#include <boost/log/attributes/named_scope.hpp>
#include <boost/log/sources/logger.hpp>
#include <boost/log/support/date_time.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/attributes.hpp>
#include <boost/log/sinks.hpp>

#include <enable_processinfo.h>

using namespace boost;
namespace logging = boost::log;
namespace attrs = boost::log::attributes;
namespace src = boost::log::sources;
namespace sinks = boost::log::sinks;
namespace expr = boost::log::expressions;
namespace keywords = boost::log::keywords;

//boost::log::sources::severity_logger< severity_levels > com_log::slg;

template< typename CharT, typename TraitsT >
inline std::basic_ostream< CharT, TraitsT >& operator<< (
	std::basic_ostream< CharT, TraitsT >& strm, severity_levels lvl)
{
	static const char* const str[] =
	{
		"emergency",
		"alert",
		"critical",
		"error",
		"warning",
		"notice",
		"info",
		"debug"
	};
	if (static_cast< std::size_t >(lvl) < (sizeof(str) / sizeof(*str)))
		strm << str[lvl];
	else
		strm << static_cast< int >(lvl);
	return strm;
};

//BOOST_LOG_ATTRIBUTE_KEYWORD(_severity, "Severity", severity_levels);
//BOOST_LOG_ATTRIBUTE_KEYWORD(_timestamp, "TimeStamp", boost::posix_time::ptime);
//BOOST_LOG_ATTRIBUTE_KEYWORD(_uptime, "Uptime", attrs::timer::value_type);
//BOOST_LOG_ATTRIBUTE_KEYWORD(_scope, "Scope", attrs::named_scope::value_type);


com_log::com_log(void)
{

}

com_log::~com_log(void)
{
	
}

void com_log::clear()
{
	logging::core::get()->remove_all_sinks();
}

typedef sinks::synchronous_sink< sinks::text_file_backend > file_sink;

void com_log::InitLog(const std::string& filename)
{
	logging::add_common_attributes();
	logging::core::get()->add_thread_attribute("Scope", attrs::named_scope());
	logging::core::get()->add_thread_attribute("Uptime", attrs::timer());

	std::string tempstr = filename;
	if(tempstr.empty())
		tempstr = enable_processinfo::get_processname();

	for (int i = slog_emergency; i<=slog_debug;i++)
	{
		std::stringstream allfilename;
		allfilename << "logs\\" << tempstr <<"\\"<< tempstr << "_"<<(severity_levels)i << "_%Y%m%d_%H.log";
		
		boost::shared_ptr< file_sink > sink(new file_sink(
			keywords::file_name = allfilename.str(),      // file name pattern
			keywords::rotation_size = 10*1024*1024,                  
			keywords::time_based_rotation=sinks::file::rotation_at_time_point(0,0,0),
			keywords::open_mode = std::ios::app,
			keywords::auto_flush = false
			));

		sink->locked_backend()->set_file_collector(sinks::file::make_collector(
			keywords::target = "logs\\"+tempstr,                         
			keywords::max_size = 30 * 1024 * 1024,             
			keywords::min_free_space = 100 * 1024 * 1024           
			));

		sink->locked_backend()->scan_for_files();

		if(i<=slog_error)
		{
			sink->locked_backend()->auto_flush();
		}

		sink->set_filter(expr::attr< severity_levels >("Severity") == (severity_levels)i);

		sink->set_formatter
			(
			expr::format("(%1%)<%2%_%3%>{%4%}: %5%")
			% expr::attr< attrs::current_thread_id::value_type >("ThreadID")
			% expr::format_date_time< boost::posix_time::ptime >("TimeStamp", "%Y-%m-%d,%H:%M:%S.%f")
			% expr::attr< boost::posix_time::time_duration >("Uptime")
			% expr::format_named_scope("Scope", keywords::format = "%n[%f:%l]", keywords::depth = 1)
			% expr::smessage
			);		

		log::core::get()->add_sink(sink);		
	}    

	SetConsole();
}

void com_log::SetConsole()
{
	auto asink = log::add_console_log(std::clog, keywords::format = expr::stream
		<< expr::format_date_time< boost::posix_time::ptime >("TimeStamp", "[%Y-%m-%d,%H:%M:%S.%f]")
		<< " <" << expr::attr< severity_levels >("Severity")
		<< ">: " << expr::message);

	asink->set_filter(expr::attr< severity_levels >("Severity") <= slog_error);	
}

void com_log::SetLevel(severity_levels sl)
{
	logging::core::get()->set_filter(expr::attr< severity_levels >("Severity") <= sl);
}

void com_log::flush()
{
	logging::core::get()->flush();
}