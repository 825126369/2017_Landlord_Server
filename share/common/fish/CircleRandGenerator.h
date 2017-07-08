#pragma once
#include "BaseGenerator.h"
#include "enable_random.h"

struct Fish_GeneratorCFGData;

//œﬂÃı”„’Û
class CircleRandGenerator : public BaseGenerator
{
public:
	CircleRandGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~CircleRandGenerator();

	virtual void setElapsedTime(float elapsedTime);
protected:
	virtual void update(float dt);
protected:
	float mElapsed;

	int mDirCount;
	int mRandDir;

	float mInterval;
	int mOutCount;

	int mTotalCount;

	int mFishCfgID;
	int mMoveSpeed;

	enable_random<boost::rand48> m_random;
};