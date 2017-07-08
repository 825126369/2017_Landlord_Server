#include "stdafx.h"
#include "timer_manager.h"
#include "time_helper.h"
#include "logic_server.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "global_sys_mgr.h"

using namespace boost;

timer_manager::timer_manager()
{

}

timer_manager::~timer_manager()
{

}

void timer_manager::init_timer()
{
	start_daily_check();
}

void timer_manager::start_daily_check()
{
	auto nowp = time_helper::instance().get_cur_ptime();
	auto nextdate = nowp.date()+ gregorian::days(1);
	time_t nextt = time_helper::convert_from_date(nextdate);
	auto nextp = time_helper::convert_to_ptime(nextt);
	auto difft = nextp-nowp;
	int diffs = difft.total_seconds();
	//diffs = 15;//test
	logic_server::instance().add_server_timer(boost::bind(&timer_manager::on_daily_check, this), diffs);
}

void timer_manager::on_daily_check()
{
	//auto pmap = game_player_mgr::instance().get_player_map();	
	//for (auto it = pmap.begin(); it != pmap.end(); ++it)
	//{
	//	auto p = it->second;
	//	p->reset_playerinfo();
	//}

	global_sys_mgr::instance().sys_time_update();

	start_daily_check();
}