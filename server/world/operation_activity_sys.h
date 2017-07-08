#pragma once
#include "game_sys_def.h"
#include "operation_activity_type.h"

struct stActivityInfo;
struct M_ActivityCFGData;
class game_player;
class ActivityConditionFactory;
class OperationActivity;

// 运营活动系统
class OperationActivitySys : public game_sys_base
{
public:
	typedef boost::shared_ptr<stActivityInfo> TypeActivityInfo;
	typedef boost::shared_ptr<ActivityConditionFactory> TypeActivityConditionFactory;
public:
	MAKE_SYS_TYPE(e_gst_operation_activity);

	virtual void init_sys_object();

	virtual void sys_time_update();

	/*
			从数据表记录创建一个活动
			返回true成功
	*/
	bool createActivity(const M_ActivityCFGData* data);

	/*
			玩家活动事件
	*/
	int onPlayerEvent(game_player* player, stActivityEvent* evt, time_t curTime = 0);

	/*
			检测玩家活动是否完成
	*/
	bool isFinish(game_player* player, int activityId);

	bool isFinish(game_player* player, stActivityInfo* info);

	bool giveOutReward(game_player* player, stActivityInfo* info);

	TypeActivityInfo findActivity(int activityId);

	/*
			活动是否合法
	*/
	bool isActivityValid(int activityId, time_t curTime = 0);

	bool isInTimeRange(stActivityInfo* info, time_t curTime);

	/*
			手动领取活动奖励
	*/
	int receiveActivityRewardManual(game_player *player, int activityId, time_t curTime = 0);
private:
	void _createCondition(stActivityInfo* ptr, const M_ActivityCFGData* data);

	void _initFactory();

	int _onPlayerEvent(game_player* player, stActivityEvent* evt, time_t curTime);
private:
	// 活动ID-->活动信息
	std::map<int, TypeActivityInfo> m_act;

	// 活动类型--->活动ID列表
	std::map<int, std::vector<int> > m_atypeToId;

	// ActivityType 活动类型--->条件工厂
	std::map<int, TypeActivityConditionFactory> m_condFactory;

	std::string m_sender;

	// ActivityType 活动类型-->OperationActivity
	// 具体活动单元
	std::map<int, boost::shared_ptr<OperationActivity> > m_actUnit;
};





