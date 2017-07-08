#pragma once
#include "BaseGenerator.h"

struct Fish_GeneratorCFGData;

//œﬂÃı”„’Û
class FixGenerator : public BaseGenerator
{
public:
	FixGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~FixGenerator();
protected:
	virtual void update(float dt);
protected:
	struct FixFishData
	{
		Vec2 mPosition;
		int mFishCfgID;
	};
	std::list<FixFishData> mFisFishDatas;

	Vec2 mSpeed;
};