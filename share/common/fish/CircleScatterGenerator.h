#pragma once
#include "BaseGenerator.h"

struct Fish_GeneratorCFGData;

//线条鱼阵
class CircleScatterGenerator : public BaseGenerator
{
public:
	CircleScatterGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~CircleScatterGenerator();
protected:
	virtual void update(float dt);
protected:
	int mRadius;

	int mFishCfgID;
	//每圈鱼数量
	int mCircleFishCount;

	int mRedFishIndex;
	int mRedFishID;
	//移动速度
	float mMoveSpeed;
};