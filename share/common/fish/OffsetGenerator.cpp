#include "stdafx.h"
#include "OffsetGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "RouteMgr.h"
#include "Route.h"

OffsetGenerator::OffsetGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	mFishCfgID = generatorCFGdata->mParam[0];
	mRandOffset = generatorCFGdata->mParam[1];

	for (unsigned int i = 2; i+1 < generatorCFGdata->mParam.size();)
	{
		mOffsets.push_back(Vec2(generatorCFGdata->mParam[i], generatorCFGdata->mParam[i+1]));
		i += 2;
	}

	setFishCount(mOffsets.size());
}

OffsetGenerator::~OffsetGenerator()
{

}

void OffsetGenerator::update(float dt)
{
	if (isEnd())
		return;

	auto it = mOffsets.begin();
	while (it != mOffsets.end())
	{
		auto route = RouteMgr::getSingleton()->createRoute(mRouteID, (*it), mRandOffset, mFishStartID);
		createFish(mFishCfgID, route);
		it++;
	}
}