#include "stdafx.h"
#include "BaseGenerator.h"
#include "GeneratorMgr.h"
#include "Fish_GeneratorCFG.h"
#include "Route.h"

BaseGenerator::BaseGenerator(const Fish_GeneratorCFGData* generatorCfg, GeneratorMgr* generatorMgr)
	:mFishStartID(0)
	,mInitFishStartID(0)
	,mFishCount(0)
	,mGroupID(0)
	,mGeneratorCFGData(generatorCfg)
	,mGeneratorMgr(generatorMgr)
	,mElapsedTime(0)
	,mRouteID(0)
	,mPosition(-10000, -10000)
{
}

BaseGenerator::~BaseGenerator()
{

}

void BaseGenerator::updateTime(float dt)
{
	mElapsedTime += dt;
	update(dt);
}

void BaseGenerator::setElapsedTime(float elapsedTime)
{
	mElapsedTime = elapsedTime;
	mFishCount = 0;
}

float BaseGenerator::getElapsedTime()
{
	return mElapsedTime;
}

void BaseGenerator::update(float dt)
{

}

bool BaseGenerator::isEnd()
{
	return mFishCount == 0;
}

void BaseGenerator::setPosition(const Vec2& pos)
{
	mPosition = pos;
}

const Vec2& BaseGenerator::getPosition()
{
	return mPosition;
}

void BaseGenerator::setFishStartID(int startID)
{
	mFishStartID = startID;
	mInitFishStartID = startID;
}

int BaseGenerator::getInitStartID()
{
	return mInitFishStartID;
}

void BaseGenerator::setRouteID(int routeID)
{
	mRouteID = routeID;
}

int BaseGenerator::getRouteID()
{
	return mRouteID;
}

void BaseGenerator::setGroupID(int groupID)
{
	mGroupID = groupID;
}

int BaseGenerator::getGroupID()
{
	return mGroupID;
}

int BaseGenerator::getFishCount()
{
	return mFishCount;
}

int BaseGenerator::getGeneratorID()
{
	return mGeneratorCFGData->mID;
}

void BaseGenerator::setFishCount(int fishCount)
{
	mFishCount = fishCount;
}

void BaseGenerator::createFish(int fishCfgID, RoutePtr baseRoute)
{
	mGeneratorMgr->createFish(mFishStartID++, fishCfgID, baseRoute);
	mFishCount--;
}