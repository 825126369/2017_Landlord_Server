#pragma once

enum ActivityType
{
	// 类型-累计充值
	activity_type_recharge = 1,

	// 类型-单笔充值
	activity_type_single_recharge,

	// 类型-在指定日登录
	activity_type_login_at_day,
};

// 条件类型
enum ConditionType
{
	// 条件-充值金额
	cond_type_ConditionRecharge,    

	// 条件-单笔充值
	cond_type_ConditionSingleRecharge,

	// 条件-在指定日登录
	cond_type_ConditionLoginAtDay,

	// 条件-VIP等级
	cond_type_ConditionPlayerVIPLevel,
};

struct stActivityEvent
{
	int m_activityType;
	int m_param1;
	int m_param2;

	stActivityEvent(){}

	stActivityEvent(int atype, int param1)
	{
		m_activityType = atype;
		m_param1 = param1;
	}
};
