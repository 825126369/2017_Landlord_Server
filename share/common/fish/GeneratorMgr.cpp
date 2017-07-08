#include "stdafx.h"
#include "GeneratorMgr.h"
#include "LineGenerator.h"
#include "LineDelayGenerator.h"
#include "DelayGenerator.h"
#include "CircleScatterGenerator.h"
#include "CircleGenerator.h"
#include "CombineGenerator.h"
#include "CircleRotateGenerator.h"
#include "CircleRandGenerator.h"
#include "FixGenerator.h"
#include "OffsetGenerator.h"
#include "Fish_GeneratorCFG.h"

GeneratorMgr::GeneratorMgr()
{
}

GeneratorMgr::~GeneratorMgr()
{
}

BaseGenerator* GeneratorMgr::buildGenerator(int generatorID, int routeID, GeneratorMgr* generatorMgr)
{
	auto generatorCFGData = Fish_GeneratorCFG::GetSingleton()->GetData(generatorID);
	if (generatorCFGData == nullptr)
		return nullptr;

	BaseGenerator* generator = nullptr;

	if (generatorCFGData->mType == 1)
	{
		generator = new DelayGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 2)
	{
		generator = new CircleScatterGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 3)
	{
		generator = new CircleGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 4)
	{
		generator = new CircleRotateGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 5)
	{
		generator = new LineGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 6)
	{
		generator = new LineDelayGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 7)
	{
		generator = new CircleRandGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 8)
	{
		generator = new FixGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 9)
	{
		generator = new OffsetGenerator(generatorCFGData, generatorMgr);
	}
	else if (generatorCFGData->mType == 10)
	{
		generator = new CombineGenerator(generatorCFGData, generatorMgr);
	}
	if (generator != nullptr)
	{
		generator->setRouteID(routeID);
	}
	return generator;
}

void GeneratorMgr::deleteGenerator(BaseGenerator* generator)
{
	delete (generator);
}