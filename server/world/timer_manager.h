#pragma once
#include <enable_singleton.h>
#include "enable_hashmap.h"

struct stTimerInfo
{
	int m_clock;
	boost::function<void()> m_call;
	int m_count;
};

class timer_manager:
	public enable_singleton<timer_manager>
{
public:
	timer_manager();
	~timer_manager();

	void init_timer(); 

	// 注册一个定时器, clock今日几点触发, callBack触发时回调函数，count触发次数,-1表示永久
	bool regTimer(int clock, boost::function<void()> callBack, int count = -1);

private:
	void start_daily_check();
	void on_daily_check();

	int _getSeconds(int clock);

	void _happen(int id);

private:
	ENABLE_MAP<int, stTimerInfo> m_timers;
	int m_timerId;
};
