#include "stdafx.h"
#include "operation_activity_sys.h"
#include "msg_type_def.pb.h"
#include "operation_activity_def.h"
#include "time_helper.h"
#include "operation_activity_mgr.h"
#include "game_player.h"
#include "operation_activity_condition.h"
#include "M_ActivityCFG.h"
#include "operation_activity_factory.h"
#include "mail_sys.h"
#include "global_sys_mgr.h"
#include "M_MultiLanguageCFG.h"
#include "tool_helper.h"
#include "operation_activity.h"
#include "pump_type.pb.h"
using namespace boost;

void OperationActivitySys::init_sys_object()
{
	_initFactory();

	const boost::unordered_map<int, M_ActivityCFGData>& datas = M_ActivityCFG::GetSingleton()->GetMapData();

	for(auto it = datas.begin(); it != datas.end(); ++it)
	{
		createActivity(&it->second);
	}

	const M_MultiLanguageCFGData *data = M_MultiLanguageCFG::GetSingleton()->GetData("System");
	if(data)
	{
		m_sender = data->mName;
	}
	else
	{
		SLOG_ERROR << boost::format("OperationActivitySys::init_sys_object, 找不到配置[System]");
	}
}

void OperationActivitySys::sys_time_update()
{
	/*m_act.clear();
	m_atypeToId.clear();
	m_condToId.clear();

	M_ActivityCFG::GetSingleton()->Reload();
	const boost::unordered_map<int, M_ActivityCFGData>& datas = M_ActivityCFG::GetSingleton()->GetMapData();
	for(auto it = datas.begin(); it != datas.end(); ++it)
	{
		createActivity(&it->second);
	}*/
}

bool OperationActivitySys::createActivity(const M_ActivityCFGData* data)
{
	if(data == nullptr)
		return false;

	if(m_act.find(data->mID) != m_act.end())
		return true;

	time_t start, end;
	bool res = ToolHelper::parseTime(&data->mStartTime, &data->mEndTime, &start, &end);
	if(!res)
		return false;

	if(data->mRewardList.size() != data->mRewardCount.size())
	{
		SLOG_ERROR << boost::format("OperationActivitySys::createActivity, 奖励道具列表与数量列表个数不同!");
		return false;
	}

	TypeActivityInfo ptr = boost::make_shared<stActivityInfo>();
	ptr->m_cfgData = data;
	ptr->m_activityId = data->mID;
	ptr->m_startTime = start;
	ptr->m_endTime = end;

	_createCondition(ptr.get(), data);
	m_act[data->mID] = ptr;
	m_atypeToId[data->mActivityType].push_back(data->mID);

	return true;
}

int OperationActivitySys::onPlayerEvent(game_player* player, stActivityEvent* evt, time_t curTime)
{
	if(player == nullptr || evt == nullptr)
		return msg_type_def::e_rmt_unknow;

	if(curTime == 0)
	{
		curTime = time_helper::instance().get_cur_time();
	}

	_onPlayerEvent(player, evt, curTime);

	// 充值活动，再调一次单笔充
	if(evt->m_activityType == activity_type_recharge)
	{
		stActivityEvent tmpEvt = *evt;
		tmpEvt.m_activityType = activity_type_single_recharge;
		_onPlayerEvent(player, &tmpEvt, curTime);
	}
	return msg_type_def::e_rmt_success;
}

int OperationActivitySys::_onPlayerEvent(game_player* player, stActivityEvent* evt, time_t curTime)
{
	auto it = m_atypeToId.find(evt->m_activityType);
	if(it == m_atypeToId.end())
		return msg_type_def::e_rmt_fail;

// 	if(curTime == 0)
// 	{
// 		curTime = time_helper::instance().get_cur_time();
// 	}

	auto unit = m_actUnit.find(evt->m_activityType);
	if(unit == m_actUnit.end())
		return msg_type_def::e_rmt_fail;

	ActParam param;
	param.m_player = player;
	param.m_evt = evt;
	param.m_curTime = curTime;
	param.m_actIdList = &it->second;
	unit->second->doActivity(&param, this);

	return msg_type_def::e_rmt_success;
}

bool OperationActivitySys::isFinish(game_player* player, int activityId)
{
	auto it = m_act.find(activityId);
	if(it == m_act.end())
		return false;

	return isFinish(player, it->second.get());
}

bool OperationActivitySys::isFinish(game_player* player, stActivityInfo* info)
{
	if(player == nullptr || info == nullptr)
		return false;

	ConditionParam param;
	param.m_player = player;
	param.m_info = info;

	bool finish = true;
	for(auto it = info->m_conditions.begin(); it != info->m_conditions.end(); ++it)
	{
		finish = it->second->isSatisfy(&param);
		if(!finish)
			break;
	}

	return finish;
}

const std::string EMPTY = "";

bool OperationActivitySys::giveOutReward(game_player* player, stActivityInfo* info)
{
	if(info->m_cfgData->mRewardList.empty())
		return false;

	std::vector<stGift> items;

	auto mailSys = GLOBAL_SYS(MailSys);

	for(int i = 0; i < info->m_cfgData->mRewardList.size(); i++)
	{
		items.push_back(stGift(info->m_cfgData->mRewardList[i], info->m_cfgData->mRewardCount[i]));
	}

	const std::string* title = &EMPTY;
	const std::string* content = &EMPTY;
	const M_MultiLanguageCFGData* pTitle = M_MultiLanguageCFG::GetSingleton()->GetData(info->m_cfgData->mActivityRewardMailTitle);
	const M_MultiLanguageCFGData* pContent = M_MultiLanguageCFG::GetSingleton()->GetData(info->m_cfgData->mActivityRewardMailContent);
	if(pTitle)
	{
		title = &pTitle->mName;
	}
	else
	{
		SLOG_ERROR << boost::format("OperationActivitySys::_giveOutReward,多语言找不到配置[%1%]") % info->m_cfgData->mActivityRewardMailTitle;
	}
	if(pContent)
	{
		content = &pContent->mName;
	}
	else
	{
		SLOG_ERROR << boost::format("OperationActivitySys::_giveOutReward,多语言找不到配置[%1%]") % info->m_cfgData->mActivityRewardMailContent;
	}

	mailSys->sendMail(*title, m_sender, *content, 0, player->PlayerId->get_value(), 0, &items);

	return true;
}

OperationActivitySys::TypeActivityInfo OperationActivitySys::findActivity(int activityId)
{
	auto it = m_act.find(activityId);
	if(it == m_act.end())
		return nullptr;

	return it->second;
}

bool OperationActivitySys::isActivityValid(int activityId, time_t curTime)
{
	auto ptr = findActivity(activityId);
	if(ptr == nullptr)
		return false;

	if(curTime == 0)
	{
		curTime = time_helper::instance().get_cur_time();
	}

	return isInTimeRange(ptr.get(), curTime);
}

bool OperationActivitySys::isInTimeRange(stActivityInfo* info, time_t curTime)
{
	if(curTime >= info->m_startTime && curTime <= info->m_endTime)
		return true;

	return false;
}

int OperationActivitySys::receiveActivityRewardManual(game_player *player, int activityId, time_t curTime)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	bool res = isActivityValid(activityId, curTime);
	if(!res)
		return msg_type_def::e_rmt_activity_outofdate;

	auto data = findActivity(activityId);
	res = isFinish(player, data.get());
	if(!res)
		return msg_type_def::e_rmt_activity_not_satisfy_cond;

	auto mgr = player->get_sys<OperationActivityMgr>();
	auto ptr = mgr->addActivity(activityId);
	if(ptr->m_isFinish->get_value() == true)
		return msg_type_def::e_rmt_has_receive_reward;

	for(int i = 0; i < data->m_cfgData->mRewardList.size(); i++)
	{
		boost::format fmt = boost::format("id:%1%") % data->m_cfgData->mID;

		player->addItem(data->m_cfgData->mRewardList[i], 
						data->m_cfgData->mRewardCount[i],
						type_reason_receive_activity_reward,
						fmt.str());
	}
	ptr->m_isFinish->set_value(true);
	mgr->saveData();
	return msg_type_def::e_rmt_success;
}

void OperationActivitySys::_createCondition(stActivityInfo* ptr, const M_ActivityCFGData* data)
{
	auto it = m_condFactory.find(data->mActivityType);
	if(it == m_condFactory.end())
		return;

	ConditionFactoryParam param;
	param.m_ptr = ptr;
	param.m_data = data;
	it->second->createCondition(&param);
}

void OperationActivitySys::_initFactory()
{
	// 构造条件工厂
	m_condFactory[activity_type_recharge] = boost::make_shared<ConditionRechargeFactory>();
	m_condFactory[activity_type_single_recharge] = boost::make_shared<ConditionSingleRechargeFactory>();
	m_condFactory[activity_type_login_at_day] = boost::make_shared<ConditionLoginAtDayFactory>();

	// 构造活动单元
	m_actUnit[activity_type_recharge] = boost::make_shared<OperationActivityAccumulativeRecharge>();
	m_actUnit[activity_type_single_recharge] = boost::make_shared<OperationActivitySingleRecharge>();
	m_actUnit[activity_type_login_at_day] = boost::make_shared<OperationActivityLoginAtDay>();
}

















