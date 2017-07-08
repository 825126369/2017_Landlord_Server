#pragma once

struct ConditionFactoryParam;

class ActivityConditionFactory
{
public:
	virtual ~ActivityConditionFactory(){}

	virtual bool createCondition(ConditionFactoryParam* param) = 0;
};

// 累计充值活动
class ConditionRechargeFactory : public ActivityConditionFactory
{
public:	
	virtual bool createCondition(ConditionFactoryParam* param);
};

// 单笔充值活动
class ConditionSingleRechargeFactory : public ActivityConditionFactory
{
public:	
	virtual bool createCondition(ConditionFactoryParam* param);
};

// 指定日登录
class ConditionLoginAtDayFactory : public ActivityConditionFactory
{
public:
	virtual bool createCondition(ConditionFactoryParam* param);
};

