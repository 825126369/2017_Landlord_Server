#pragma once
#ifdef CLIENT
#include "CocosHead.h"
#else
#include "logic_pos.h"
#endif

class FishTools
{
public:
	static float getRotateAngle(const Vec2& point);
	static Vec2 getRotateDirection(float angle);
	static float getMoveTime(const Vec2& pos, const Vec2& speed, float radius);
	static const float PI;
	static const float DEGRESSPI;
};