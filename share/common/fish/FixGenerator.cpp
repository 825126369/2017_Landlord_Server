#include "stdafx.h"
#include "FixGenerator.h"
#include "Fish_GeneratorCFG.h"
#include "RouteMgr.h"
#include "Route.h"

FixGenerator::FixGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	mSpeed.x = generatorCFGdata->mParam[0];
	mSpeed.y = generatorCFGdata->mParam[1];

	for (unsigned int i = 2; i+2 < generatorCFGdata->mParam.size();)
	{
		FixFishData fixFishData;
		fixFishData.mPosition.x = generatorCFGdata->mParam[i];
		fixFishData.mPosition.y = generatorCFGdata->mParam[i+1];
		fixFishData.mFishCfgID = generatorCFGdata->mParam[i+2];

		mFisFishDatas.push_back(fixFishData);
		i += 3;
	}

	setFishCount(mFisFishDatas.size());
}

FixGenerator::~FixGenerator()
{

}

void FixGenerator::update(float dt)
{
	if (isEnd())
		return;

	auto it = mFisFishDatas.begin();
	while (it != mFisFishDatas.end())
	{
		auto route = RouteMgr::getSingleton()->createRoute(it->mFishCfgID, it->mPosition, mSpeed);
		createFish(it->mFishCfgID, route);
		it++;
	}
}