#include "stdafx.h"
#include "daily_box_lottery_sys.h"
#include "timer_manager.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "daily_box_lottery_mgr.h"
#include "time_helper.h"
#include "M_OnlineRewardCFG.h"
#include "tool_helper.h"

void DailyBoxLotterySys::init_sys_object()
{
	timer_manager::instance().regTimer(12, boost::bind(&DailyBoxLotterySys::resetBoxLottery, this));

	_loadOnlineRewardData();
}

void DailyBoxLotterySys::resetBoxLottery()
{
#ifdef _DEBUG
	boost::posix_time::ptime pt = time_helper::instance().get_cur_ptime();
	boost::posix_time::time_duration td = pt.time_of_day(); 
	SLOG_CRITICAL << boost::format("时间到了，重置所有玩家的每日宝箱抽奖，当前时间[%1%:%2%:%3%]") % td.hours() % td.minutes() % td.seconds();
#endif

	time_t curT = time_helper::instance().get_cur_time();

	auto pmap = game_player_mgr::instance().get_player_map();	
	for (auto it = pmap.begin(); it != pmap.end(); ++it)
	{
		auto p = it->second;
		p->get_sys<DailyBoxLotteryMgr>()->reset(curT);
	}
}

const OnlineRewardInfo* DailyBoxLotterySys::getOnlineRewardData(boost::posix_time::ptime& curPtime)
{
	boost::posix_time::time_duration t = curPtime.time_of_day();
	for(auto it = m_onlineRewardData.begin(); it != m_onlineRewardData.end(); ++it)
	{
		if(t >= it->m_start && t <= it->m_end)
			return &(*it);
	}

	return nullptr;
}

void DailyBoxLotterySys::_loadOnlineRewardData()
{
	boost::unordered_map<int, M_OnlineRewardCFGData>& all = M_OnlineRewardCFG::GetSingleton()->GetMapData();
	for(auto it = all.begin(); it != all.end(); ++it)
	{
		OnlineRewardInfo tmp;
		tmp.m_data = &it->second;

		time_t start, end;
		bool res = ToolHelper::parseTime(&it->second.mStartTime, &it->second.mEndTime, &start, &end);
		if(!res)
		{
			SLOG_ERROR << boost::format("在线奖励时间格式出错[id=%1%]") % it->second.mID;
			continue;
		}

		boost::posix_time::ptime p = time_helper::convert_to_ptime(start);
		tmp.m_start = p.time_of_day();

		p = time_helper::convert_to_ptime(end);
		tmp.m_end = p.time_of_day();

		m_onlineRewardData.push_back(tmp);
#ifdef _DEBUG
		//SLOG_CRITICAL << boost::format("解析出start时间[%1%:%2%:%3%]") % tmp.m_start.hours() % tmp.m_start.minutes() % tmp.m_start.seconds();
		//SLOG_CRITICAL << boost::format("解析出end时间[%1%:%2%:%3%]") % tmp.m_end.hours() % tmp.m_end.minutes() % tmp.m_end.seconds();
#endif
	}
}

