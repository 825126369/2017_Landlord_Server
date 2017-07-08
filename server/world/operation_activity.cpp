#include "stdafx.h"
#include "operation_activity.h"
#include "operation_activity_mgr.h"
#include "game_player.h"
#include "operation_activity_def.h"
#include "operation_activity_sys.h"
#include "global_sys_mgr.h"
#include "M_ActivityCFG.h"

void OperationActivity::doActivity(ActParam *actParam, OperationActivitySys *sys)
{
	if(actParam == nullptr)
		return;

	auto mgr = actParam->m_player->get_sys<OperationActivityMgr>();
	bool save = false;

	for(auto tmpIt = actParam->m_actIdList->begin(); tmpIt != actParam->m_actIdList->end(); ++tmpIt)
	{
		auto data = sys->findActivity(*tmpIt);
		if(data)
		{
			if(sys->isInTimeRange(data.get(), actParam->m_curTime))
			{
				auto ptr = mgr->addActivity(*tmpIt);
				if(ptr->m_isFinish->get_value() == true)
					continue;

				_doAct(actParam, ptr.get(), mgr.get());

				if(data->m_cfgData->mReceiveWay == 0)  // 通过邮件领取
				{
					bool res = sys->isFinish(actParam->m_player, data.get());
					if(res)
					{
						sys->giveOutReward(actParam->m_player, data.get());
						ptr->m_isFinish->set_value(true);
					}
				}

				_afterAct(actParam, ptr.get(), mgr.get());
				save = true;
			}
		}
	}

	if(save)
	{
		mgr->saveData();
	}
}

bool OperationActivity::_afterAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr)
{
	return true;
}

//////////////////////////////////////////////////////////////////////////

bool OperationActivityAccumulativeRecharge::_doAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr)
{
	// param1存储已充值金额
	pInfo->m_param1->add_value(actParam->m_evt->m_param1);
	return true;
}

//////////////////////////////////////////////////////////////////////////

bool OperationActivitySingleRecharge::_doAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr)
{
	// param1存储已充值金额
	pInfo->m_param1->add_value(actParam->m_evt->m_param1);
	return true;
}

bool OperationActivitySingleRecharge::_afterAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr)
{
	// 单笔充值，每次充值满指定金额都会给奖励
	pInfo->m_param1->set_value(0);
	pInfo->m_isFinish->set_value(false);
	return true;
}

//////////////////////////////////////////////////////////////////////////

bool OperationActivityLoginAtDay::_doAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr)
{
	// 存储登录时间
	pInfo->m_param2->set_value(actParam->m_curTime);
	return true;
}











