#pragma once
#include "game_sys_def.h"

class game_player;

// 签到结果
struct stSignResult
{
	// 是否有月卡奖励
	bool m_hasMonthCardReward;

	// 抽到的转盘编号，只有当第7天以后，这个值才起作用
	int m_dialNum;

	void reset()
	{
		m_hasMonthCardReward = false;
		m_dialNum = -1;
	}
};

// 转盘抽奖管理
class DialLotteryMgr : public enable_obj_pool<DialLotteryMgr>, public game_sys_base
{
public:
	DialLotteryMgr();

	~DialLotteryMgr();

	MAKE_SYS_TYPE(e_gst_dial_lottery);

	MAKE_GET_OWNER(game_player);

	virtual void init_sys_object();

	/*
			今日是否已抽过奖
	*/
	bool isLotteryToday();

	/*
			签到
	*/
	int doSign(time_t curTime);

	/*
			返回已连续签到次数
	*/
	int getHasSignCount(boost::gregorian::date& curDate);

	/*
			当天是否领取了月卡奖励
	*/
	bool hasReceiveMonthCardReward(boost::gregorian::date& curDate);

	/*
			领取月卡奖励
	*/
	int receiveMonthCardReward(time_t curTime);

	// 返回签到结果
	stSignResult* getSignResult(){ return &m_signResult; }
private:
	void _monthCardReward(time_t curTime, game_player *player);

	/*
			开始转盘抽奖
			num		返回编号
			返回值返回值 e_msg_result_def定义
	*/
	int _doLottery(int& num, game_player *player);

	bool _isLotteryToday(boost::gregorian::date& curDate);
public:
	// 抽奖时间
	Tfield<time_t>::TFieldPtr LotteryTime;	

	// 已连续签到次数
	Tfield<int32_t>::TFieldPtr m_hasSignCount;

	// 上次月卡奖励的领取时间
	Tfield<time_t>::TFieldPtr MonthCardRewardReceiveTime;	
private:
	stSignResult m_signResult;
};

