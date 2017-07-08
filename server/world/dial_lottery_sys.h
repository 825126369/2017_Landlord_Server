#pragma once
#include "game_sys_def.h"
#include "M_DialLotteryCFG.h"
#include "probability.h"

// 转盘抽奖系统
class DialLotterySys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_dial_lottery);

	virtual void init_sys_object();		

	/*
			抽奖
			num   返回奖励编号
			coin  返回奖励的金币
			rtype 奖励类型
			返回抽奖是否成功，true成功,false失败
	*/
	bool doLottery(int& num, GOLD_TYPE& coin, int& rtype);
private:
	std::vector<M_DialLotteryCFGData> m_items;

	Probability m_prob;
};

