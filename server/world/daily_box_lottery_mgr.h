#pragma once
#include "game_sys_def.h"

class game_player;
class BoxLotteryArray;
class BoxLotteryItem;

// 抽奖结果
struct stBoxLotteryResult
{
	// 0 抽到大奖  
	// 1 抽到小奖  
	// 2 抽到小奖，并且添加了谢谢参与
	int m_resultType;
};

// 每日宝箱抽奖管理
class DailyBoxLotteryMgr : public enable_obj_pool<DailyBoxLotteryMgr>, public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_daily_box_lottery);

	MAKE_GET_OWNER(game_player);

	virtual void init_sys_object();	

	virtual void sys_init();

	virtual bool sys_load();

#ifdef _DEBUG
	void resetBigPrize(int index);
#endif

	/*
			重置宝箱及抽奖次数
	*/
	void reset(time_t curT = 0);

	/*
			是否抽到了大奖
	*/
	bool hasLotteryBigPrize();

	/*
			宝箱抽奖
	*/
	int doBoxLottery(int boxIndex);

	/*
			通过礼券抽奖，需要花费礼券
	*/
	int doBoxLotteryWithTicket(int boxIndex);

	/*
			谢谢参与兑换礼券
	*/
	int exchangeTicket();

	int getBoxCount();

	BoxLotteryItem* getBoxLotteryItem(int index);

	stBoxLotteryResult* getBoxLotteryResult(){ return &m_result; }

	int getLotteryCountToday(){ return m_lotteryCountToday->get_value(); }

	int getThankYouCount(){ return m_thankYouCount->get_value(); }

	void addThankYouCount(int cnt){ m_thankYouCount->add_value(cnt); }
private:
	void _randBoxReward();

	// 宝箱抽奖
	int _lotteryBox(int boxIndex, game_player *player);
private:
	// 宝箱数组
	GArrayFieldPtr m_boxArray;

	boost::shared_ptr<BoxLotteryArray> m_boxArrayPtr;

	// 上次宝箱的重置时间
	Tfield<time_t>::TFieldPtr m_lastBoxResetTime;

	// 今日已抽奖次数
	Tfield<int32_t>::TFieldPtr m_lotteryCountToday;

	// 谢谢参与次数
	Tfield<int32_t>::TFieldPtr m_thankYouCount;

	stBoxLotteryResult m_result;
};

//////////////////////////////////////////////////////////////////////////

class BoxLotteryItem : public game_object, public enable_obj_pool<BoxLotteryItem>
{
public:
	BoxLotteryItem();

	virtual void init_game_object();

	// 宝箱是否打开了
	Tfield<bool>::TFieldPtr m_isOpen;
	
	// 宝箱内所含金币
	Tfield<GOLD_TYPE>::TFieldPtr m_containGold;
};

//////////////////////////////////////////////////////////////////////////

class BoxLotteryArray : public game_object_array, public enable_obj_pool<BoxLotteryArray>
{
public:
	virtual const std::string& get_cells_name();	

	virtual const std::string& get_id_name();

	virtual GObjPtr create_game_object(uint32_t object_id);

	virtual bool update_all(){ return true; }
};
