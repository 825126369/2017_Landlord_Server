#include "stdafx.h"
#include "daily_box_lottery_mgr.h"
#include "game_player.h"
#include "M_BaseInfoCFG.h"
#include "enable_random.h"
#include "time_helper.h"
#include "msg_type_def.pb.h"
#include "pump_type.pb.h"
#include "game_sys_recharge.h"

static time_t genTime()
{
	auto curPtime = time_helper::instance().get_cur_ptime();
	boost::gregorian::date curDate = curPtime.date();  
	boost::posix_time::ptime pt(boost::gregorian::date(curDate.year(), curDate.month(), curDate.day()),
		boost::posix_time::time_duration(12, 0, 0));
		
	boost::posix_time::time_duration td = curPtime.time_of_day(); 
	
	if(td.hours() < 12)
	{
		pt = pt + boost::gregorian::days(-1);
	}
	return time_helper::instance().convert_from_ptime(pt);
}
 
void DailyBoxLotteryMgr::init_sys_object()
{
	auto player = get_game_player();
	m_boxArray = player->regedit_arrfield("dailyBoxLottery", BoxLotteryArray::malloc());
	m_boxArrayPtr = m_boxArray->get_Tarray<BoxLotteryArray>();
	m_lastBoxResetTime = CONVERT_POINT(Tfield<time_t>, player->regedit_tfield(e_got_date, "lastBoxResetTime"));
	m_lotteryCountToday = CONVERT_POINT(Tfield<int32_t>, player->regedit_tfield(e_got_int32, "lotteryBoxCountToday"));
	m_thankYouCount = CONVERT_POINT(Tfield<int32_t>, player->regedit_tfield(e_got_int32, "thankYouCount"));
}

void DailyBoxLotteryMgr::sys_init()
{
	sys_load();
}

bool DailyBoxLotteryMgr::sys_load()
{
	int count = m_boxArrayPtr->get_obj_count();
	if(count == 0) // 新建玩家
	{
		m_lastBoxResetTime->set_value(genTime());

		int lotteryCount = M_BaseInfoCFG::GetSingleton()->GetData("lotteryBoxCount")->mValue;
		for(int i = 0; i < lotteryCount; i++)
		{
			auto ptr = BoxLotteryItem::malloc();
			m_boxArrayPtr->put_obj(ptr);
		}
		_randBoxReward();
		//get_game_player()->store_game_object();
	}
	else // 判断是否需要重置
	{
		time_t curT = time_helper::instance().get_cur_time();
		if(curT - m_lastBoxResetTime->get_value() >= SECONDS_PER_DAY)
		{
//#ifdef _DEBUG
//			boost::posix_time::ptime pt = time_helper::instance().get_cur_ptime();
//			boost::posix_time::time_duration td = pt.time_of_day(); 
//			SLOG_CRITICAL << boost::format("由于玩家重新登录时间到，重置每日宝箱抽奖，当前时间[%1%:%2%:%3%]") % td.hours() % td.minutes() % td.seconds();
//#endif
			reset(genTime());
		}
	}
	return true;
}

#ifdef _DEBUG

void DailyBoxLotteryMgr::resetBigPrize(int index)
{
	int bigPrize = M_BaseInfoCFG::GetSingleton()->GetData("lotteryBigPrize")->mValue;
	int smallPrize = M_BaseInfoCFG::GetSingleton()->GetData("lotterySmallPrize")->mValue;

	int count = m_boxArrayPtr->get_obj_count();
	for(int i = 0; i < count; i++)
	{
		auto ptr = m_boxArrayPtr->get_Tobj<BoxLotteryItem>(i);
		if(i == index)
		{
			ptr->m_containGold->set_value(bigPrize);
		}
		else
		{
			ptr->m_containGold->set_value(smallPrize);
		}
		ptr->m_isOpen->set_value(false);
	}
}

#endif

void DailyBoxLotteryMgr::reset(time_t curT)
{
	m_lotteryCountToday->set_value(0);
	// 只有当抽到大奖时，才会重置宝箱
	if(hasLotteryBigPrize())
	{
		_randBoxReward();
	}
	
	if(curT == 0)
	{
		curT = time_helper::instance().get_cur_time();
	}
	m_lastBoxResetTime->set_value(curT);
	//get_game_player()->store_game_object();
}

bool DailyBoxLotteryMgr::hasLotteryBigPrize()
{
	static GOLD_TYPE bigPrize = M_BaseInfoCFG::GetSingleton()->GetData("lotteryBigPrize")->mValue;

	int count = m_boxArrayPtr->get_obj_count();
	for(int i = 0; i < count; i++)
	{
		auto ptr = m_boxArrayPtr->get_Tobj<BoxLotteryItem>(i);
		if(ptr)
		{
			if(ptr->m_isOpen->get_value())
			{
				if(bigPrize == ptr->m_containGold->get_value())
				{
					return true;
				}
			}
		}
	}
	return false;
}

int DailyBoxLotteryMgr::doBoxLottery(int boxIndex)
{
	auto player = get_game_player();
	int maxLimit = player->get_sys<game_sys_recharge>()->getVipProfit(e_evt_MaxBoxLotterylimit);
	if(m_lotteryCountToday->get_value() >= maxLimit)
		return msg_type_def::e_rmt_runout_count;

	int retCode = _lotteryBox(boxIndex, player);
	if(retCode == msg_type_def::e_rmt_success)
	{
		m_lotteryCountToday->add_value(1);
		//player->store_game_object();
	}
	return retCode;
}

int DailyBoxLotteryMgr::doBoxLotteryWithTicket(int boxIndex)
{
	static int ticketCost = M_BaseInfoCFG::GetSingleton()->GetData("ticketLotteryCost")->mValue;
	auto player = get_game_player();
	if(player->Ticket->get_value() < ticketCost)
		return msg_type_def::e_rmt_ticket_not_enough;

	int retCode = _lotteryBox(boxIndex, player);
	if(retCode == msg_type_def::e_rmt_success)
	{
		player->addItem(msg_type_def::e_itd_ticket, -ticketCost, type_reason_daily_box_lottery);
		//player->store_game_object();
	}
	return retCode;
}

int DailyBoxLotteryMgr::exchangeTicket()
{
	static int needCount = M_BaseInfoCFG::GetSingleton()->GetData("thankYouExchangeLimit")->mValue;
	static int exchangeCount = M_BaseInfoCFG::GetSingleton()->GetData("thankYouJoinExchangeTicket")->mValue;

	if(m_thankYouCount->get_value() < needCount)
		return msg_type_def::e_rmt_thank_you_not_enough;

	auto player = get_game_player();
	m_thankYouCount->add_value(-needCount);
	player->addItem(msg_type_def::e_itd_ticket, exchangeCount, type_reason_thank_you_exchange);
	return msg_type_def::e_rmt_success;
}

int DailyBoxLotteryMgr::getBoxCount()
{ 
	return m_boxArrayPtr->get_obj_count();
}

BoxLotteryItem* DailyBoxLotteryMgr::getBoxLotteryItem(int index)
{
	return m_boxArrayPtr->get_Tobj<BoxLotteryItem>(index).get();
}

// 随机宝箱奖励
void DailyBoxLotteryMgr::_randBoxReward()
{
	int count = m_boxArrayPtr->get_obj_count();
	if(count == 0)
	{
		SLOG_ERROR << boost::format("DailyBoxLotteryMgr::_randBoxReward, 宝箱个为0");
		return;
	}

	int bigPrize = M_BaseInfoCFG::GetSingleton()->GetData("lotteryBigPrize")->mValue;
	int smallPrize = M_BaseInfoCFG::GetSingleton()->GetData("lotterySmallPrize")->mValue;

	// 随机一个大奖
	int rindex = global_random::instance().rand_int(0, count - 1);
	auto ptr = m_boxArrayPtr->get_Tobj<BoxLotteryItem>(rindex);
	if(ptr)
	{
		ptr->m_isOpen->set_value(false);
		ptr->m_containGold->set_value(bigPrize);
	}

	for(int i = 0; i < count; i++)
	{
		if(i != rindex)
		{
			 ptr = m_boxArrayPtr->get_Tobj<BoxLotteryItem>(i);
			 if(ptr)
			 {
				 ptr->m_isOpen->set_value(false);
				 ptr->m_containGold->set_value(smallPrize);
			 }
		}
	}
	m_boxArray->set_update();
}

int DailyBoxLotteryMgr::_lotteryBox(int boxIndex, game_player *player)
{
	auto boxPtr = m_boxArrayPtr->get_Tobj<BoxLotteryItem>(boxIndex);
	if(!boxPtr)
		return msg_type_def::e_rmt_box_not_exist;

	if(boxPtr->m_isOpen->get_value())
		return msg_type_def::e_rmt_box_has_opened;

	boxPtr->m_isOpen->set_value(true);
	player->addItem(msg_type_def::e_itd_gold, boxPtr->m_containGold->get_value(), type_reason_daily_box_lottery);

	static int smallPrize = M_BaseInfoCFG::GetSingleton()->GetData("lotterySmallPrize")->mValue;
	// 抽到了小奖，增加谢谢参与次数
	if(smallPrize == boxPtr->m_containGold->get_value())
	{
		static int thankYouMaxCount = M_BaseInfoCFG::GetSingleton()->GetData("tankkYouCountLimit")->mValue;
		if(m_thankYouCount->get_value() < thankYouMaxCount)
		{
			m_thankYouCount->add_value(1);
			m_result.m_resultType = msg_type_def::result_thank_you;
		}
		else
		{
			m_result.m_resultType = msg_type_def::result_samll_prize;
		}
	}
	else
	{
		m_result.m_resultType = msg_type_def::result_big_prize;
	}
	m_boxArray->set_update();
	return msg_type_def::e_rmt_success;
}

//////////////////////////////////////////////////////////////////////////

BoxLotteryItem::BoxLotteryItem()
{
	init_game_object();
}

void BoxLotteryItem::init_game_object()
{
	m_isOpen =  CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "isOpen"));
	m_containGold = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(GOLD_OBJ_TYPE, "containGold"));
}

//////////////////////////////////////////////////////////////////////////

const std::string& BoxLotteryArray::get_cells_name()
{
	static std::string cellsname = "dailyBoxLottery";
	return cellsname;
}

const std::string& BoxLotteryArray::get_id_name()
{
	static std::string idname = "containGold";
	return idname;
}

GObjPtr BoxLotteryArray::create_game_object(uint32_t object_id)
{
	auto op = BoxLotteryItem::malloc();
	return op;
}

