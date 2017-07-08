#include "stdafx.h"
#include "CircleGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "RouteMgr.h"
#include "Route.h"

CircleGenerator::CircleGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	mPosition.x = generatorCFGdata->mParam[0];
	mPosition.y = generatorCFGdata->mParam[1];
	mRadius = generatorCFGdata->mParam[2];
	
	mFishCfgID = generatorCFGdata->mParam[3];

	mCircleFishCount = generatorCFGdata->mParam[4];

	mRedFishIndex = generatorCFGdata->mParam[5];
	mRedFishID = generatorCFGdata->mParam[6];
	
	mRouteID = generatorCFGdata->mParam[7];

	mSpeed.x = generatorCFGdata->mParam[8];
	mSpeed.y = generatorCFGdata->mParam[9];

	setFishCount(mCircleFishCount);
}

CircleGenerator::~CircleGenerator()
{

}

void CircleGenerator::update(float dt)
{
	if (isEnd())
		return;

	float startRotation = 0.0f;
	float addRotation = 6.28318f/mCircleFishCount;
		
	for (int i = 0; i < mCircleFishCount; i++)
	{
		Vec2 direction = Vec2::forAngle(startRotation + addRotation * i);
		Vec2 pos =  mPosition + direction * mRadius;
		int fishCfgID = mFishCfgID;
		if (mRedFishID != -1 && mRedFishIndex == i)
		{
			fishCfgID = mRedFishID;
		}
		if (mRouteID == 0)
		{
			auto route = RouteMgr::getSingleton()->createRoute(fishCfgID, pos, mSpeed);
			createFish(fishCfgID, route);
		}
		else
		{
			auto route = RouteMgr::getSingleton()->createRoute(mRouteID);
			createFish(fishCfgID, route);
		}
	}
}