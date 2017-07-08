#pragma once
#include <enable_singleton.h>

class timer_manager:
	public enable_singleton<timer_manager>
{
public:
	timer_manager();
	~timer_manager();

	void init_timer(); 

private:
	void start_daily_check();
	void on_daily_check();
};
