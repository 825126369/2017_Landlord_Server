#pragma once

class game_player;
struct stActivityEvent;
struct ActivityStoreInfo;
class OperationActivityMgr;
class OperationActivitySys;

struct ActParam
{
	game_player *m_player;

	stActivityEvent *m_evt;

	time_t m_curTime;

	std::vector<int> *m_actIdList;
};

class OperationActivity
{
public:
	virtual ~OperationActivity(){}

	void doActivity(ActParam *actParam, OperationActivitySys *sys);
protected:
	virtual bool _doAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr) = 0;

	// 处理后
	virtual bool _afterAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr);
};

// 累计充值
class OperationActivityAccumulativeRecharge : public OperationActivity
{
protected:
	virtual bool _doAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr);
};

// 单笔充值
class OperationActivitySingleRecharge : public OperationActivity
{
protected:
	virtual bool _doAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr);

	virtual bool _afterAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr);
};

// 在指定日登录
class OperationActivityLoginAtDay : public OperationActivity
{
protected:
	virtual bool _doAct(ActParam *actParam, ActivityStoreInfo *pInfo, OperationActivityMgr *mgr);
};