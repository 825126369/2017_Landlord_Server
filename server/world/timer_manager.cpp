#include "stdafx.h"
#include "timer_manager.h"
#include "time_helper.h"
#include "world_server.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "global_sys_mgr.h"

using namespace boost;

const int TWO_DAYS = 2 * 24 * 3600;

timer_manager::timer_manager()
{
	m_timerId = 0;
}

timer_manager::~timer_manager()
{

}

void timer_manager::init_timer()
{
	start_daily_check();
}

bool timer_manager::regTimer(int clock, boost::function<void()> callBack, int count)
{
	if(clock <= 0 || clock >= 24)
		return false;

	stTimerInfo info;
	info.m_clock = clock;
	info.m_call = callBack;
	info.m_count = count;
	
	m_timerId++;
	m_timers[m_timerId] = info;

	world_server::instance().add_server_timer(bind(&timer_manager::_happen, this, m_timerId), _getSeconds(clock));
	return true;
}

void timer_manager::start_daily_check()
{
	auto nowp = time_helper::instance().get_cur_ptime();
	auto nextdate = nowp.date()+ gregorian::days(1);
	time_t nextt = time_helper::convert_from_date(nextdate);
	auto nextp = time_helper::convert_to_ptime(nextt);
	auto difft = nextp-nowp;
	int diffs = difft.total_seconds();
	world_server::instance().add_server_timer(bind(&timer_manager::on_daily_check, this), diffs);
}

void timer_manager::on_daily_check()
{
	auto pmap = game_player_mgr::instance().get_player_map();	
	for (auto it = pmap.begin(); it != pmap.end(); ++it)
	{
		auto p = it->second;
		p->resetPlayerInfo();
	}

	//执行逻辑
	global_sys_mgr::instance().sys_time_update();

	//下一次时间
	start_daily_check();
}

int timer_manager::_getSeconds(int clock)
{
	auto nowp = time_helper::instance().get_cur_ptime();

	int total = nowp.time_of_day().total_seconds();
	if(total < clock * 3600)
	{
		return clock * 3600 - total;
	}
	
	return TWO_DAYS - total;
}

void timer_manager::_happen(int timerId)
{
	auto it = m_timers.find(timerId);
	if(it == m_timers.end())
		return;

	if(!it->second.m_call.empty())
	{
		it->second.m_call();
	}

	if(it->second.m_count == -1)
	{
		world_server::instance().add_server_timer(bind(&timer_manager::_happen, this, timerId),
			_getSeconds(it->second.m_clock));
	}
	else
	{
		it->second.m_count--;
		if(it->second.m_count > 0)
		{
			world_server::instance().add_server_timer(bind(&timer_manager::_happen, this, timerId),
				_getSeconds(it->second.m_clock));
		}
		else
		{
			m_timers.erase(timerId);
		}
	}
}