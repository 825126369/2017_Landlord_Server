#pragma once
#include "BaseGenerator.h"

struct Fish_GeneratorCFGData;

//线条鱼阵
class CircleGenerator : public BaseGenerator
{
public:
	CircleGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~CircleGenerator();
protected:
	virtual void update(float dt);
protected:
	int mRadius;

	int mFishCfgID;
	//每圈鱼数量
	int mCircleFishCount;

	int mRedFishIndex;
	int mRedFishID;

	int mRouteID;
	Vec2 mSpeed;
};