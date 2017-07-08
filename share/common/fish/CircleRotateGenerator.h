#pragma once
#include "BaseGenerator.h"

struct Fish_GeneratorCFGData;

//线条鱼阵
class CircleRotateGenerator : public BaseGenerator
{
public:
	CircleRotateGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~CircleRotateGenerator();
protected:
	virtual void update(float dt);
protected:
	int mRadius;

	int mFishCfgID;
	//每圈鱼数量
	int mCircleFishCount;

	int mRedFishIndex;
	int mRedFishID;

	float mRotation;
	float mRotateTime;
	float mMoveSpeed;
};