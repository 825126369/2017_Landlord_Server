#include "stdafx.h"
#include "dial_lottery_mgr.h"
#include "enable_object_pool.h"
#include "game_player.h"
#include "msg_type_def.pb.h"
#include "global_sys_mgr.h"
#include "dial_lottery_sys.h"
#include "time_helper.h"
#include "pump_type.pb.h"
#include "game_sys_recharge.h"
#include "M_GiftRewardCFG.h"
#include "M_DailyLoginCFG.h"
#include "game_quest_mgr.h"
#include "game_quest.h"

DialLotteryMgr::DialLotteryMgr()
{
}

DialLotteryMgr::~DialLotteryMgr()
{
}

void DialLotteryMgr::init_sys_object()
{
	LotteryTime = CONVERT_POINT(Tfield<time_t>, get_game_player()->regedit_tfield(e_got_date, "lotteryTime"));
	m_hasSignCount = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "hasSignCount"));

	MonthCardRewardReceiveTime = CONVERT_POINT(Tfield<time_t>, get_game_player()->regedit_tfield(e_got_date, "monthCardRewardReceiveTime"));
}

bool DialLotteryMgr::isLotteryToday()
{
	boost::gregorian::date curDate = time_helper::instance().get_cur_date();
	return _isLotteryToday(curDate);
}

int DialLotteryMgr::doSign(time_t curTime)
{
	boost::gregorian::date curDate = time_helper::convert_to_date(curTime);

	if(_isLotteryToday(curDate)) // 今日已签到
		return msg_type_def::e_rmt_has_dial_lottery;

	int retCode = msg_type_def::e_rmt_success;
	auto player = get_game_player();
	m_signResult.reset();
	// 当前签到
	int curSignIndex = getHasSignCount(curDate) + 1;
	_monthCardReward(curTime, player);

	if(curSignIndex >= 7) // 已经到了第7天，此时开始转盘抽奖
	{
		retCode = _doLottery(m_signResult.m_dialNum, player);
	}
	else // 普通的奖励
	{
		const M_DailyLoginCFGData *pData = M_DailyLoginCFG::GetSingleton()->GetData(curSignIndex);
		if(pData == nullptr)
		{
			SLOG_ERROR << boost::format("DialLotteryMgr::doSign, 找不到第[%1%]天的签到信息") % (curSignIndex);
			return msg_type_def::e_rmt_unknow;
		}

		switch (pData->mTpye)
		{
		case msg_type_def::e_itd_gold: // 金币
		case msg_type_def::e_itd_ticket:
			{
				int count = pData->mCount;
				if(m_signResult.m_hasMonthCardReward)
				{
					count = (count << 1);
#ifdef _DEBUG
					SLOG_CRITICAL << boost::format("抽到第[%1%]天，签到奖励翻倍为[%2%]，翻倍前[%3%]") % curSignIndex % count % (count >> 1);
#endif
				}
#ifdef _DEBUG
				else
				{
					SLOG_CRITICAL << boost::format("抽到第[%1%]天，签到奖励为[%2%]") % curSignIndex % count;
				}
#endif
				player->addItem(pData->mTpye, count, type_reason_daily_sign);
			}
			break;
		case msg_type_def::e_itd_vip_experience_card: // vip体验卡
			{
				player->addExperienceVIP(pData->mCount);
			}
			break;
		}
	}

	player->get_sys<game_quest_mgr>()->change_quest(e_qt_daily_login);

	LotteryTime->set_value(time_helper::convert_from_date(curDate));
	m_hasSignCount->set_value(curSignIndex);
	//player->store_game_object();
	return retCode;
}

int DialLotteryMgr::getHasSignCount(boost::gregorian::date& curDate)
{
	// 还没有签到过
	if(LotteryTime->get_value() == 0)
		return 0;

	// 上一次抽奖时间
	boost::gregorian::date prevDate = time_helper::convert_to_date( LotteryTime->get_value() );
	if(prevDate == curDate)
		return m_hasSignCount->get_value();

	prevDate += boost::gregorian::days(1);
	if(prevDate == curDate) // 是连续登录
		return m_hasSignCount->get_value();

	return 0;
}

bool DialLotteryMgr::hasReceiveMonthCardReward(boost::gregorian::date& curDate)
{
	boost::gregorian::date prevDate = time_helper::convert_to_date( MonthCardRewardReceiveTime->get_value() );
	return prevDate == curDate;
}

int DialLotteryMgr::receiveMonthCardReward(time_t curTime)
{
	boost::gregorian::date curDate = time_helper::convert_to_date(curTime);
	if(hasReceiveMonthCardReward(curDate))
		return msg_type_def::e_rmt_has_receive_reward;

	auto player = get_game_player();

	if(curTime >= player->get_sys<game_sys_recharge>()->VipCardEndTime->get_value())
	{
		return msg_type_def::e_rmt_month_card_out_date;
	}

	const M_GiftRewardCFGData* data = M_GiftRewardCFG::GetSingleton()->GetData(1);
	if(data == nullptr)
	{
		SLOG_ERROR << boost::format("DialLotteryMgr::doLottery, 找不到月卡奖励类型[1]");
		return msg_type_def::e_rmt_unknow;
	}

	if(data->mItemId.size() != data->mItemCount.size())
	{
		SLOG_ERROR << boost::format("DialLotteryMgr::doLottery, 大小不一致");
		return msg_type_def::e_rmt_unknow;
	}

	for(int i = 0; i < (int)data->mItemId.size(); i++)
	{
		player->addItem(data->mItemId[i], data->mItemCount[i], type_reason_month_card_daily_recv);
	}
	MonthCardRewardReceiveTime->set_value(curTime);
	//player->store_game_object();
	return msg_type_def::e_rmt_success;
}

void DialLotteryMgr::_monthCardReward(time_t curTime, game_player *player)
{
	if(curTime >= player->get_sys<game_sys_recharge>()->VipCardEndTime->get_value())
	{
		m_signResult.m_hasMonthCardReward = false;
	}
	else
	{
		m_signResult.m_hasMonthCardReward = true;

		/*const M_GiftRewardCFGData* data = M_GiftRewardCFG::GetSingleton()->GetData(1);
		if(data == nullptr)
		{
			SLOG_ERROR << boost::format("DialLotteryMgr::doLottery, 找不到月卡奖励类型[1]");
			return;
		}

		if(data->mItemId.size() != data->mItemCount.size())
		{
			SLOG_ERROR << boost::format("DialLotteryMgr::doLottery, 大小不一致");
			return;
		}

		for(int i = 0; i < (int)data->mItemId.size(); i++)
		{
			player->addItem(data->mItemId[i], data->mItemCount[i], type_reason_month_card_daily_recv);
		}*/
	}
}

int DialLotteryMgr::_doLottery(int& num, game_player *player)
{
	GOLD_TYPE coin = 0;
	int rtype = 0;
	bool res = GLOBAL_SYS(DialLotterySys)->doLottery(num, coin, rtype);
	if(!res)
		return msg_type_def::e_rmt_unknow;

	// 有月卡，奖励翻倍
	if(m_signResult.m_hasMonthCardReward)
	{
		coin = (coin << 1);
#ifdef _DEBUG
		SLOG_CRITICAL << boost::format("抽到第7天，转盘抽奖，奖励翻倍为[%1%]，翻倍前[%2%]") % coin % (coin >> 1);
#endif
	}
#ifdef _DEBUG
	else
	{
		SLOG_CRITICAL << boost::format("抽到第7天，转盘抽奖，奖励为[%1%]") % coin;
	}
#endif

	switch(rtype)
	{
	case 1: // 金币
		{
			player->addItem(msg_type_def::e_itd_gold, coin, type_reason_dial_lottery);
		}
		break;
	case 2: // 礼券
		{
			player->addItem(msg_type_def::e_itd_ticket, coin, type_reason_dial_lottery);
		}
		break;
	}	

	return msg_type_def::e_rmt_success;
}

bool DialLotteryMgr::_isLotteryToday(boost::gregorian::date& curDate)
{
	time_t time = LotteryTime->get_value();		
	boost::gregorian::date lastTime = time_helper::convert_to_date(time);
	return curDate == lastTime;
}
