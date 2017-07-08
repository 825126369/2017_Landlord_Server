#include "stdafx.h"
#include "CircleRandGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "RouteMgr.h"
#include "FishTools.h"
#include "Route.h"

CircleRandGenerator::CircleRandGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
	,m_random(1)
{
	mElapsed = 0;

	mPosition.x = generatorCFGdata->mParam[0];
	mPosition.y = generatorCFGdata->mParam[1];

	mDirCount = generatorCFGdata->mParam[2];
	mRandDir = generatorCFGdata->mParam[3];

	mInterval = generatorCFGdata->mParam[4]/1000.0f;
	mOutCount = generatorCFGdata->mParam[5];

	mTotalCount = generatorCFGdata->mParam[6];
	mMoveSpeed = generatorCFGdata->mParam[7];
	mFishCfgID = generatorCFGdata->mParam[8];

	setFishCount(mDirCount*mOutCount*mTotalCount);
}

CircleRandGenerator::~CircleRandGenerator()
{

}

void CircleRandGenerator::setElapsedTime(float elapsedTime)
{
	mElapsedTime = elapsedTime;

	float tempTime = mElapsedTime;
	while (tempTime >= mElapsed)
	{
		tempTime -= mElapsed;
		mFishStartID += mDirCount*mOutCount;
		mFishCount -= mDirCount*mOutCount;

		mTotalCount--;
		mElapsed = mInterval;
	}

	mElapsed -= tempTime;
}

void CircleRandGenerator::update(float dt)
{
	if (isEnd())
		return;

	mElapsed -= dt;

	float startRotation = 0.0f;
	float addRotation = 2*FishTools::PI/mDirCount;
	float randRotation = mRandDir/180.0f * FishTools::PI;
	if (mElapsed < 0)
	{
		for (int i = 0; i < mDirCount; i++)
		{
			for (int j = 0; j < mOutCount; j++)
			{
				Vec2 direction = Vec2::forAngle(startRotation + addRotation * i + m_random.rand_double(-randRotation, randRotation));
				Vec2 pos = mPosition + direction * 5;
				auto route = RouteMgr::getSingleton()->createRoute(mFishCfgID, mPosition, direction * mMoveSpeed);
				createFish(mFishCfgID, route);
			}
		}

		mTotalCount--;
		mElapsed = mInterval;
	}
}