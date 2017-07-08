#pragma once
#include "BaseGenerator.h"

struct Fish_GeneratorCFGData;
class GeneratorMgr;
//—”≥Ÿ”„’Û
class DelayGenerator : public BaseGenerator
{
public:
	DelayGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~DelayGenerator();

	virtual void setElapsedTime(float elapsedTime);
protected:
	virtual void update(float dt);
protected:
	float mElapsed;

	struct DelayData
	{
		int mFishCfgID;
		float mInterval;
		int mFishCount;
	};

	Vec2 mStartPos;
	Vec2 mSpeed;

	std::list<DelayData> mDelayDatas;
};