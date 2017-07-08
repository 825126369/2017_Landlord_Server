#include "stdafx.h"
#include "LineDelayGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "RouteMgr.h"
#include "Route.h"

LineDelayGenerator::LineDelayGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	mStartPosition.x = generatorCFGdata->mParam[0];
	mStartPosition.y = generatorCFGdata->mParam[1];

	mEndPosition.x = generatorCFGdata->mParam[2];
	mEndPosition.y = generatorCFGdata->mParam[3];

	mBlockCount = generatorCFGdata->mParam[4];
	mFishCfgID = generatorCFGdata->mParam[5];
	mLineFishCount = generatorCFGdata->mParam[6];

	mSpeed.x = generatorCFGdata->mParam[7];
	mSpeed.y = generatorCFGdata->mParam[8];

	mDelayTime = generatorCFGdata->mParam[9]/1000.0f;
	mMoveTime = generatorCFGdata->mParam[10]/1000.0f;
	mWaitTime = generatorCFGdata->mParam[11]/1000.0f;

	setFishCount(mBlockCount*mLineFishCount);
}

LineDelayGenerator::~LineDelayGenerator()
{

}

void LineDelayGenerator::update(float dt)
{
	if (isEnd())
		return;

	Vec2 blockOffset = (mEndPosition - mStartPosition)/mBlockCount;
	Vec2 offset = blockOffset/mLineFishCount;
	for (int i = 0; i < mLineFishCount; i++)
	{
		for (int j = 0; j < mBlockCount; j++)
		{
			Vec2 pos = mStartPosition + blockOffset * j + offset * i;
			float delayTime = 0.1f + mDelayTime * i;
			auto route = RouteMgr::getSingleton()->createSpecialRoute(mFishCfgID, pos, mSpeed, delayTime, mMoveTime, mWaitTime);
			createFish(mFishCfgID, route);
		}
	}
}