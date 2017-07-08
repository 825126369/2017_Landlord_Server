#pragma once
#include "game_sys_def.h"
class game_player;
class ActivityStoreInfoArray;
struct ActivityStoreInfo;

// 运营活动管理
class OperationActivityMgr : public game_sys_base, public enable_obj_pool<OperationActivityMgr>
{
public:
	MAKE_SYS_TYPE(e_gst_operation_activity);
	
	MAKE_GET_OWNER(game_player);

	virtual void init_sys_object();

	virtual bool sys_load();

	/*
			查询活动
	*/
	boost::shared_ptr<ActivityStoreInfo> findActivity(int activityId);

	/*
			增加一个活动
	*/
	boost::shared_ptr<ActivityStoreInfo> addActivity(int activityId);

	/*
			保存数据到db
	*/
	void saveData();
public:
	GArrayFieldPtr m_activityInfo;

	boost::shared_ptr<ActivityStoreInfoArray> m_activityInfoPtr;
};




