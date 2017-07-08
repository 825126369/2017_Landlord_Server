#include "stdafx.h"
#include "CombineGenerator.h"
#include "GeneratorMgr.h"
#include "Fish_GeneratorCFG.h"

CombineGenerator::CombineGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr)
	:BaseGenerator(generatorCFGdata, generatorMgr)
{
	int totalFishCount = 0;
	for (unsigned int i = 0; i+1 < generatorCFGdata->mParam.size(); )
	{
		DelayGenerator delayGenerator;
		delayGenerator.mDelay = generatorCFGdata->mParam[i]/1000.0f;
		delayGenerator.mGenerator = GeneratorMgr::buildGenerator(generatorCFGdata->mParam[i+1], 0, generatorMgr);

		totalFishCount += delayGenerator.mGenerator->getFishCount();
		mDelayGenerators.push_back(delayGenerator);

		i += 2;
	}

	setFishCount(totalFishCount);
}

CombineGenerator::~CombineGenerator()
{

}

bool CombineGenerator::isEnd()
{
	return mDelayGenerators.size() == 0 && mActiveGenerator.size() == 0;
}

void CombineGenerator::setPosition(const Vec2& pos)
{
	for (auto it = mDelayGenerators.begin(); it != mDelayGenerators.end(); ++it)
	{
		it->mGenerator->setPosition(pos);
	}
	BaseGenerator::setPosition(pos);
}

void CombineGenerator::setElapsedTime(float elapsedTime)
{
	mElapsedTime = elapsedTime;
	float tempTime = elapsedTime;

	while (mDelayGenerators.size() > 0)
	{
		auto it = mDelayGenerators.begin();
		if (tempTime >= it->mDelay)
		{
			tempTime -= it->mDelay;

			it->mGenerator->setFishStartID(mFishStartID);
			mFishStartID += it->mGenerator->getFishCount();

			it->mGenerator->setElapsedTime(tempTime);
			if (it->mGenerator->isEnd() == false)
			{
				mActiveGenerator.push_back(it->mGenerator);
			}
			else
			{
				GeneratorMgr::deleteGenerator(it->mGenerator);
			}
			mDelayGenerators.erase(it);
		}
		else
		{
			it->mDelay -= tempTime;
			break;
		}
	}
}

void CombineGenerator::update(float dt)
{
	if (isEnd())
		return;

	while (mDelayGenerators.size() > 0)
	{
		auto it = mDelayGenerators.begin();
		if (it->mDelay > dt)
		{
			it->mDelay -= dt;
			break;
		}
		else
		{
			dt -= it->mDelay;
			it->mGenerator->setFishStartID(mFishStartID);
			mFishStartID += it->mGenerator->getFishCount();
			mActiveGenerator.push_back(it->mGenerator);
			mDelayGenerators.erase(it);
		}
	}

	auto it = mActiveGenerator.begin();
	while (it != mActiveGenerator.end())
	{
		(*it)->updateTime(dt);
		if ((*it)->isEnd())
		{
			GeneratorMgr::deleteGenerator(*it);
			it = mActiveGenerator.erase(it);
		}
		else
		{
			it++;
		}
	}
}