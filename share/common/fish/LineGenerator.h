#pragma once
#include "BaseGenerator.h"

struct Fish_GeneratorCFGData;

//线条鱼阵
class LineGenerator : public BaseGenerator
{
public:
	LineGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~LineGenerator();

	virtual void setElapsedTime(float elapsedTime);
protected:
	virtual void update(float dt);
protected:
	float mElapsed;

	Vec2 mStartPosition;
	Vec2 mEndPosition;

	int mFishCfgID;
	//每条鱼数量
	int mLineFishCount;

	int mTotalCount;

	float mInterval;

	Vec2 mSpeed;
};