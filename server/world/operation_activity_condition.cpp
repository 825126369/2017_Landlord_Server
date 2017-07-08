#include "stdafx.h"
#include "operation_activity_condition.h"
#include "operation_activity_def.h"
#include "operation_activity_mgr.h"
#include "game_player.h"
#include "time_helper.h"
#include "operation_activity_type.h"
#include "game_sys_recharge.h"

ConditionRecharge::ConditionRecharge()
{
	m_type = cond_type_ConditionRecharge;
}

bool ConditionRecharge::isSatisfy(ConditionParam* param)
{
	game_player* player = param->m_player;
	stActivityInfo* info = param->m_info;

	auto mgr = player->get_sys<OperationActivityMgr>();
	auto ptr = mgr->findActivity(info->m_activityId);
	if(!ptr)
		return false;

	return ptr->m_param1->get_value() >= m_param;
}

void ConditionRecharge::set(int param)
{
	m_param = param;
}

//////////////////////////////////////////////////////////////////////////

ConditionSingleRecharge::ConditionSingleRecharge()
{
	m_type = cond_type_ConditionSingleRecharge;
}

bool ConditionSingleRecharge::isSatisfy(ConditionParam* param)
{
	game_player* player = param->m_player;
	stActivityInfo* info = param->m_info;

	auto mgr = player->get_sys<OperationActivityMgr>();
	auto ptr = mgr->findActivity(info->m_activityId);
	if(!ptr)
		return false;

	return ptr->m_param1->get_value() == m_param;
}

void ConditionSingleRecharge::set(int param)
{
	m_param = param;
}

//////////////////////////////////////////////////////////////////////////

ConditionLoginAtDay::ConditionLoginAtDay()
{
	m_type = cond_type_ConditionLoginAtDay;
}

bool ConditionLoginAtDay::isSatisfy(ConditionParam* param)
{
	game_player* player = param->m_player;
	stActivityInfo* info = param->m_info;

	auto mgr = player->get_sys<OperationActivityMgr>();
	auto ptr = mgr->findActivity(info->m_activityId);
	if(!ptr)
		return false;

	boost::posix_time::ptime t = time_helper::convert_to_ptime(ptr->m_param2->get_value());
	return t.date() == m_param.date();
}

void ConditionLoginAtDay::set(boost::posix_time::ptime param)
{
	m_param = param;
}

//////////////////////////////////////////////////////////////////////////

ConditionPlayerVIPLevel::ConditionPlayerVIPLevel()
{
	m_type = cond_type_ConditionPlayerVIPLevel;
}

bool ConditionPlayerVIPLevel::isSatisfy(ConditionParam* param)
{
	game_player* player = param->m_player;
	auto mgr = player->get_sys<game_sys_recharge>();

	int vipLevel = mgr->VipLevel->get_value();

	if(vipLevel >= m_low /*&& vipLevel <= m_up*/)
		return true;

	return false;
}

void ConditionPlayerVIPLevel::set(int low, int up)
{
	m_low = low;
	m_up = up;
}





