#include "stdafx.h"
#include "time_helper.h"
#include <boost/timer.hpp>



time_helper::time_helper(void)
{
	m_start_time = std::time(nullptr);//std::clock();
	m_base_time = 0;
}

time_helper::~time_helper(void)
{
}

void time_helper::set_base_time(time_t _t)
{
	m_base_time = _t;
	m_start_time = std::time(nullptr);//std::clock();
}

time_t time_helper::get_cur_time()
{
	if(m_base_time <=0)
		return std::time(nullptr);

	return m_base_time + get_tick_count();
}

time_t time_helper::get_tick_count()
{
	//return (std::clock() - m_start_time)/CLOCKS_PER_SEC;
	return (std::time(nullptr) - m_start_time);
}

//////////////////////////////////////////////////////////////////////////
using namespace boost::gregorian;
using namespace boost::posix_time;

boost::gregorian::date time_helper::get_cur_date()
{
	return convert_to_date(get_cur_time());
}
boost::posix_time::ptime time_helper::get_cur_ptime()
{
	return convert_to_ptime(get_cur_time());
}

boost::gregorian::date time_helper::convert_to_date(time_t t)
{
	tm* t2 = localtime(&t);
	return date_from_tm(*t2);
}

boost::posix_time::ptime time_helper::convert_to_ptime(time_t t)
{
	tm* t2 = localtime(&t);
	return ptime_from_tm(*t2);
	
	//return from_time_t( t );
}

boost::posix_time::ptime time_helper::convert_to_ptime(const std::string& datestr)
{
	return time_from_string(datestr);
}

time_t time_helper::convert_from_date(boost::gregorian::date& d)
{
	// 如果日期无效，返回0
	if(d.is_not_a_date())
		return 0;

	tm t = to_tm(d);
	return mktime(&t);
}

time_t time_helper::convert_from_ptime(boost::posix_time::ptime& p)
{
	if(p.is_not_a_date_time())
		return 0;

	tm t = to_tm( p );
	return mktime( &t );
}