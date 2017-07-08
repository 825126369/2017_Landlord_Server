#include "stdafx.h"
#include "LineGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "RouteMgr.h"
#include "Route.h"

LineGenerator::LineGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	mElapsed = 0;

	mStartPosition.x = generatorCFGdata->mParam[0];
	mStartPosition.y = generatorCFGdata->mParam[1];

	mEndPosition.x = generatorCFGdata->mParam[2];
	mEndPosition.y = generatorCFGdata->mParam[3];

	mFishCfgID = generatorCFGdata->mParam[4];
	mLineFishCount = generatorCFGdata->mParam[5];

	mTotalCount = generatorCFGdata->mParam[6];
	mInterval = generatorCFGdata->mParam[7]/1000.0f;

	mSpeed.x = generatorCFGdata->mParam[8];
	mSpeed.y = generatorCFGdata->mParam[9];

	setFishCount(mLineFishCount*mTotalCount);
}

LineGenerator::~LineGenerator()
{

}

void LineGenerator::setElapsedTime(float elapsedTime)
{
	mElapsedTime = elapsedTime;
	float tempTime = elapsedTime;

	while (tempTime >= mElapsed && mFishCount > 0)
	{
		tempTime -= mElapsed;
		mFishCount -= mLineFishCount;
		mFishStartID += mLineFishCount;

		mTotalCount--;
		mElapsed = mInterval;
	}
	mElapsed -= tempTime;
}


void LineGenerator::update(float dt)
{
	if (isEnd())
		return;

	mElapsed -= dt;
	if (mElapsed < 0)
	{
		Vec2 offset = (mEndPosition - mStartPosition)/mLineFishCount;
		for (int i = 0; i < mLineFishCount; i++)
		{
			Vec2 pos = mStartPosition + offset * i;
			auto route = RouteMgr::getSingleton()->createRoute(mFishCfgID, pos, mSpeed);
			createFish(mFishCfgID, route);
		}

		mTotalCount--;
		mElapsed = mInterval;
	}
}