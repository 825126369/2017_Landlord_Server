#include "stdafx.h"
#include "game_player.h"
#include "operation_activity_mgr.h"
#include "operation_activity_def.h"
#include "global_sys_mgr.h"
#include "operation_activity_sys.h"
#include "time_helper.h"

void OperationActivityMgr::init_sys_object()
{
	m_activityInfo = get_game_player()->regedit_arrfield("activityInfo", ActivityStoreInfoArray::malloc());
	m_activityInfoPtr = m_activityInfo->get_Tarray<ActivityStoreInfoArray>();
}

bool OperationActivityMgr::sys_load()
{
	int count = m_activityInfoPtr->get_obj_count();
	if(count == 0)
		return true;

	auto sys = GLOBAL_SYS(OperationActivitySys);
	time_t curTime = time_helper::instance().get_cur_time();

	bool res = false;
	bool save = false;

	for(int i = count - 1; i >= 0; i--)
	{
		auto ptr = m_activityInfoPtr->get_Tobj<ActivityStoreInfo>(i);
		res = sys->isActivityValid(ptr->m_activityId->get_value(), curTime);
		if(!res)
		{
			m_activityInfoPtr->del_obj_by_index(i);
			save = true;
		}
	}

	if(save)
	{
		saveData();
	}

	return true;
}

boost::shared_ptr<ActivityStoreInfo> OperationActivityMgr::findActivity(int activityId)
{
	return m_activityInfoPtr->find_Tobj<ActivityStoreInfo>(activityId);
}

boost::shared_ptr<ActivityStoreInfo> OperationActivityMgr::addActivity(int activityId)
{
	auto ptr = findActivity(activityId);
	if(ptr)
		return ptr;

	ptr = ActivityStoreInfo::malloc();
	ptr->m_activityId->set_value(activityId);
	m_activityInfoPtr->put_obj(ptr);
	return ptr;
}

void OperationActivityMgr::saveData()
{
	m_activityInfo->set_update();
	//get_game_player()->store_game_object();
}
