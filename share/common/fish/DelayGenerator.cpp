#include "stdafx.h"
#include "DelayGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "GeneratorMgr.h"
#include "RouteMgr.h"
#include "Route.h"

DelayGenerator::DelayGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	mElapsed = 0;

	mRouteID = generatorCFGdata->mParam[0];

	mStartPos.x = generatorCFGdata->mParam[1];
	mStartPos.y = generatorCFGdata->mParam[2];
	mSpeed.x = generatorCFGdata->mParam[3];
	mSpeed.y = generatorCFGdata->mParam[4];

	int totalCount = 0;
	for (unsigned int i = 5; i+2 < generatorCFGdata->mParam.size();)
	{
		DelayData delayData;
		delayData.mFishCfgID = generatorCFGdata->mParam[i];
		delayData.mInterval = generatorCFGdata->mParam[i+1]/1000.0f;
		delayData.mFishCount = generatorCFGdata->mParam[i+2];
		mDelayDatas.push_back(delayData);

		totalCount += delayData.mFishCount;

		i += 3;
	}

	setFishCount(totalCount);
}

DelayGenerator::~DelayGenerator()
{

}

void DelayGenerator::update(float dt)
{
	if (isEnd())
		return;

	mElapsed -= dt;

	if (mElapsed <= 0.0f)
	{
		auto it = mDelayDatas.begin();
		if (it == mDelayDatas.end())
			return;

		if (mRouteID != 0)
		{
			auto route = RouteMgr::getSingleton()->createRoute(mRouteID);
			createFish(it->mFishCfgID, route);
		}
		else
		{
			auto route = RouteMgr::getSingleton()->createRoute(it->mFishCfgID, mStartPos, mSpeed);
			createFish(it->mFishCfgID, route);
		}

		it->mFishCount--;
		mElapsed = it->mInterval;

		if (it->mFishCount == 0)
		{
			mDelayDatas.pop_front();
			it = mDelayDatas.begin();
			if (it != mDelayDatas.end())
			{
				mElapsed = it->mInterval;
			}
		}
	}
}

void DelayGenerator::setElapsedTime(float elapsedTime)
{
	mElapsedTime = elapsedTime;
	float tempTime = elapsedTime;

	while (tempTime >= mElapsed && mFishCount > 0)
	{
		tempTime -= mElapsed;

		auto it = mDelayDatas.begin();
		if (it == mDelayDatas.end())
			break;

		mFishCount--;
		mFishStartID++;

		it->mFishCount--;
		mElapsed = it->mInterval;

		if (it->mFishCount == 0)
		{
			mDelayDatas.pop_front();
			it = mDelayDatas.begin();
			if (it != mDelayDatas.end())
			{
				mElapsed = it->mInterval;
			}
		}
	}

	mElapsed -= tempTime;
}