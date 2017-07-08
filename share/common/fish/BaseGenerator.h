#pragma once
#ifdef CLIENT
#include "CocosHead.h"
#else
#include "logic_pos.h"
#endif
#include "FishCommonHead.h"

struct Fish_GeneratorCFGData;
class GeneratorMgr;
class BaseRoute;

//œﬂÃı”„’Û
class BaseGenerator
{
public:
	BaseGenerator(const Fish_GeneratorCFGData* generatorCfg, GeneratorMgr* generatorMgr);
	virtual ~BaseGenerator();

	void updateTime(float dt);

	float getElapsedTime();
	virtual void setElapsedTime(float elapsedTime);

	virtual	bool isEnd();
	virtual void setPosition(const Vec2& pos);
	const Vec2& getPosition();

	void setGroupID(int groupID);
	int getGroupID();

	void setFishStartID(int startID);
	int getInitStartID();

	void setRouteID(int routeID);
	int getRouteID();

	int getGeneratorID();
	int getFishCount();
protected:
	virtual void update(float dt);

	void setFishCount(int fishCount);

	void createFish(int fishCfgID, RoutePtr baseRoute);
protected:
	const Fish_GeneratorCFGData* mGeneratorCFGData;
	int mGroupID;
	int mFishStartID;
	int mFishCount;

	GeneratorMgr* mGeneratorMgr;

	float mElapsedTime;
	Vec2 mPosition;
	int mRouteID;
	int mInitFishStartID;
};