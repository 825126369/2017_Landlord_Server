#pragma once
#include <enable_singleton.h>
#include <boost/date_time.hpp>

class time_helper:
	public enable_singleton<time_helper>
{
public:
	time_helper(void);
	~time_helper(void);

	void set_base_time(time_t _t);

	time_t get_cur_time();
	time_t get_tick_count();
	boost::gregorian::date get_cur_date();
	boost::posix_time::ptime get_cur_ptime();

	//
	static boost::gregorian::date convert_to_date(time_t t);
	static boost::posix_time::ptime convert_to_ptime(time_t t);
	static boost::posix_time::ptime convert_to_ptime(const std::string& datestr);

	static time_t convert_from_date(boost::gregorian::date& d);
	static time_t convert_from_ptime(boost::posix_time::ptime& p);

private:
	time_t m_base_time;
	time_t m_start_time;
};

// 每天多少秒
const int32_t SECONDS_PER_DAY = 24 * 3600;
