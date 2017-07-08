#include "stdafx.h"
#include "CircleScatterGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "RouteMgr.h"
#include "Route.h"
#include "FishTools.h"

CircleScatterGenerator::CircleScatterGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	mPosition.x = generatorCFGdata->mParam[0];
	mPosition.y = generatorCFGdata->mParam[1];
	mRadius = generatorCFGdata->mParam[2];
	if (mRadius < 10)
	{
		mRadius = 10;
	}
	
	mFishCfgID = generatorCFGdata->mParam[3];

	mCircleFishCount = generatorCFGdata->mParam[4];
	if (mCircleFishCount == 0)
	{
		mCircleFishCount = 1;
	}

	mRedFishIndex = generatorCFGdata->mParam[5];
	mRedFishID = generatorCFGdata->mParam[6];

	mMoveSpeed = generatorCFGdata->mParam[7];

	setFishCount(mCircleFishCount);
}

CircleScatterGenerator::~CircleScatterGenerator()
{

}

void CircleScatterGenerator::update(float dt)
{
	if (isEnd())
		return;

	float startRotation = 0.0f;
	float addRotation = FishTools::PI*2/mCircleFishCount;

	if (mRedFishIndex == -10)
	{
		float curRotation = startRotation;
		float angle = (Vec2(640, 360) - mPosition).getAngle();
		if (angle < 0)
		{
			angle += FishTools::PI * 2;
		}
		
		for (int i = 0; i < mCircleFishCount; i++)
		{
			if (angle >= curRotation && angle < curRotation + addRotation)
			{
				mRedFishIndex = i;
				break;
			}
			curRotation += addRotation;
		}
	}
	

	for (int i = 0; i < mCircleFishCount; i++)
	{
		Vec2 direction = Vec2::forAngle(startRotation + addRotation * i);
		Vec2 pos =  mPosition + direction * mRadius;

		int fishCfgID = mFishCfgID;
		if (mRedFishID != -1 && mRedFishIndex == i)
		{
			fishCfgID = mRedFishID;
		}
		auto route = RouteMgr::getSingleton()->createRoute(fishCfgID, pos, direction * mMoveSpeed);
		createFish(fishCfgID, route);
	}
}