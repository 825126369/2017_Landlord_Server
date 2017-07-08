#pragma once
#include "game_sys_def.h"
#include <enable_object_pool.h>
struct M_RechangeCFGData;

enum e_vip_type
{
	e_evt_OnlineReward,         // 在线奖励可领取次数
	e_evt_MaxGiftslimit,        // 每日赠送礼物上限
	e_evt_MaxBoxLotterylimit,   // 每日宝箱抽奖
};

class game_sys_recharge : public game_sys_base
	,public enable_obj_pool<game_sys_recharge>
{
public:
	game_sys_recharge();
	virtual ~game_sys_recharge();
	
	MAKE_SYS_TYPE(e_gst_recharge);
	MAKE_GET_OWNER(game_player);

	virtual void init_sys_object();

	// isGmOp是否gm后台充
	void payment_once(int payid, int rmb = 0, bool isGmOp = false, bool payment_lottery = false);

	void payment_once(const std::string& orderid, int pay_type, int pay_value, int rmb = 0);

	int shopping(int shopid);

	int getVipProfit(e_vip_type viptype);

	GOLD_TYPE getMaxLimit();

	/*
			领取充值奖励
	*/
	int recvRechargeReward();

	/*
			返回月卡剩余秒数
	*/
	int getMonthCardRemainSecondTime();

	int getMonthCardRemainSecondTime(time_t curTime);

	/*
			VIP卡增加天数
	*/
	void addVipCardDays(int days, bool save);

	bool isBuyItem(int32_t payid);

public:
	Tfield<int16_t>::TFieldPtr		VipLevel;		    //VIP等级
	Tfield<int32_t>::TFieldPtr		VipExp;		        //VIP经验
	Tfield<time_t>::TFieldPtr		VipCardEndTime;		//vip卡到期时间
	GArrayFieldPtr					PaymentCheck;		//支付标记
	Tfield<int32_t>::TFieldPtr		Recharged;			//累计充值金额
	Tfield<int16_t>::TFieldPtr		LotteryCount;		//累计抽奖次数
	// 是否领取过充值奖励标记，只要充过值就可以领取一次奖励，且只能领取一次
	Tfield<bool>::TFieldPtr			RechargeRewardFlag; 
private:
	void addVipExp(uint32_t exp);

	void _joinMemberMail();

	void _notifygame();

	int _sendMail(int playerId, const std::string& title, const std::string& content);

	time_t m_last_recharge;
	int m_last_payid;

	int _lottery(int payid);
	
	// 发送抽奖通告
	void _sendLotteryNotice(int rate);

	// 充值通告
	void _sendReChargeNotice(const M_RechangeCFGData *data);
};

