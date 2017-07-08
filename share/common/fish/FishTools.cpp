#include "stdafx.h"
#include "FishTools.h"

const float FishTools::PI = 3.141596265f;
const float FishTools::DEGRESSPI = 180;
float FishTools::getRotateAngle(const Vec2& direction)
{
	float angle = - direction.getAngle() / PI * DEGRESSPI + 90;
	return angle;
}

Vec2 FishTools::getRotateDirection(float angle)
{
	return Vec2::forAngle((90 - angle)/DEGRESSPI*PI);
}

float FishTools::getMoveTime(const Vec2& pos, const Vec2& speed, float radius)
{
	float minX = -radius;
	float maxX = 1280 + radius;
	float minY = -radius;
	float maxY = 720 + radius;

	float moveX = 60.0f;
	if (speed.x > 0 && pos.x < maxX)
	{
		moveX = (maxX - pos.x)/speed.x;
	}
	else if (speed.x < 0 && pos.x > minX)
	{
		moveX = (minX - pos.x)/speed.x;
	}
	
	float moveY = 60.0f;
	if (speed.y > 0 && pos.y < maxY)
	{
		moveY = (maxY - pos.y)/speed.y;
	}
	else if (speed.y < 0 && pos.y > minY)
	{
		moveY = (minY - pos.y)/speed.y;
	}

	return moveX < moveY ? moveX : moveY;
}