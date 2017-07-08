#pragma once
#include "BaseGenerator.h"
#include <vector>

struct Fish_GeneratorCFGData;

//œﬂÃı”„’Û
class CombineGenerator : public BaseGenerator
{
public:
	CombineGenerator(const Fish_GeneratorCFGData* generatorCFGdata, GeneratorMgr* generatorMgr);
	virtual ~CombineGenerator();

	virtual bool isEnd();
	virtual void setPosition(const Vec2& pos);
	virtual void setElapsedTime(float elapsedTime);
protected:
	virtual void update(float dt);
protected:
	struct DelayGenerator
	{
		float mDelay;
		BaseGenerator* mGenerator;
	};

	std::list<DelayGenerator> mDelayGenerators;

	std::list<BaseGenerator*> mActiveGenerator;
};