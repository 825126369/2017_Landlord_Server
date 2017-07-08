#include "stdafx.h"
#include "operation_activity_factory.h"
#include "operation_activity_condition.h"
#include "M_ActivityCFG.h"
#include "operation_activity_def.h"
#include "operation_activity_type.h"
#include "time_helper.h"
#include <boost/regex.hpp>

using namespace boost;

// 累计充值活动
bool ConditionRechargeFactory::createCondition(ConditionFactoryParam* param)
{
	stActivityInfo* ptr = param->m_ptr;
	const M_ActivityCFGData* data = param->m_data;

	auto cond = boost::make_shared<ConditionRecharge>();
	cond->set(data->mRechargeRMB);
	ptr->m_conditions[cond_type_ConditionRecharge] = cond;

	return true;
}

// 单笔充值活动
bool ConditionSingleRechargeFactory::createCondition(ConditionFactoryParam* param)
{
	stActivityInfo* ptr = param->m_ptr;
	const M_ActivityCFGData* data = param->m_data;

	auto cond = boost::make_shared<ConditionSingleRecharge>();
	cond->set(data->mRechargeRMB);
	ptr->m_conditions[cond_type_ConditionSingleRecharge] = cond;

	return true;
}

// 指定日登录
bool ConditionLoginAtDayFactory::createCondition(ConditionFactoryParam* param)
{
	stActivityInfo* ptr = param->m_ptr;
	const M_ActivityCFGData* data = param->m_data;

	bool res = regex_match(data->mLoginDay.c_str(), DATE_TIME);
	if(!res)
	{
		SLOG_ERROR << boost::format("ConditionLoginAtDayFactory::createCondition [time=%1%]格式不对") % data->mLoginDay;
		return false;
	}
	
	boost::posix_time::ptime t = time_helper::convert_to_ptime(data->mLoginDay);
	auto cond = boost::make_shared<ConditionLoginAtDay>();
	cond->set(t);
	ptr->m_conditions[cond_type_ConditionLoginAtDay] = cond;

	// VIP条件
	if(data->mVipLevel > 0)
	{
		auto cond = boost::make_shared<ConditionPlayerVIPLevel>();
		cond->set(data->mVipLevel, 1000000000);
		ptr->m_conditions[cond_type_ConditionPlayerVIPLevel] = cond;
	}

	return true;
}
