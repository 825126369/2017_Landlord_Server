#pragma once

struct ConditionParam;

class ActivityCondition
{
public:
	virtual ~ActivityCondition(){}

	virtual bool isSatisfy(ConditionParam* param) = 0;

	int getType(){ return m_type; }
protected:
	int m_type;
};

// 累计充值金额
class ConditionRecharge : public ActivityCondition
{
public:
	ConditionRecharge();
	virtual bool isSatisfy(ConditionParam* param);
	void set(int param);
	int getParam(){ return m_param; }
protected:
	int m_param;
};

// 单笔充值
class ConditionSingleRecharge : public ActivityCondition
{
public:
	ConditionSingleRecharge();
	virtual bool isSatisfy(ConditionParam* param);
	void set(int param);
	int getParam(){ return m_param; }
protected:
	int m_param;
};

// 指定日登录
class ConditionLoginAtDay : public ActivityCondition
{
public:
	ConditionLoginAtDay();
	virtual bool isSatisfy(ConditionParam* param);
	void set(boost::posix_time::ptime param);
protected:
	boost::posix_time::ptime m_param;
};

// 玩家VIP等级
class ConditionPlayerVIPLevel : public ActivityCondition
{
public:
	ConditionPlayerVIPLevel();
	virtual bool isSatisfy(ConditionParam* param);
	void set(int low, int up);
protected:
	int m_low;
	int m_up;
};