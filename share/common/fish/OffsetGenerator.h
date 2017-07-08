#pragma once
#include "BaseGenerator.h"

struct Fish_GeneratorCFGData;

//Ëæ»úÓãÕó
class OffsetGenerator : public BaseGenerator
{
public:
	OffsetGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~OffsetGenerator();
protected:
	virtual void update(float dt);
protected:
	int mFishCfgID;
	int mRandOffset;

	std::list<Vec2> mOffsets;
};