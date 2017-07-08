#include "stdafx.h"
#include "RouteData.h"
#include "Route.h"

RoutePoint RouteData::msEmptyRoutePoint;

RouteData::RouteData()
{
	clearUp();
}

RouteData::~RouteData()
{

}

void RouteData::clearUp()
{
	mRouteID = 1;
	mStartPosX = 0;
	mStartPosY = 0;

	mRoutePoints.clear();

	mLifeTime = 0;
}

int RouteData::getRouteID() const
{
	return mRouteID;
}

void RouteData::setRouteID(int routeID)
{
	mRouteID = routeID;
}

void RouteData::setStartPos(float posX, float posY)
{
	mStartPosX = posX;
	mStartPosY = posY;
}

void RouteData::setStartPosX(float posX)
{
	mStartPosX = posX;
}

void RouteData::setStartPosY(float posY)
{
	mStartPosY = posY;
}

float RouteData::getStartPosX() const
{
	return mStartPosX;
}

float RouteData::getStartPosY() const
{
	return mStartPosY;
}

unsigned int RouteData::getRoutePointSize() const
{
	return mRoutePoints.size();
}

const RoutePoint& RouteData::getRoutePoint(unsigned int index) const
{
	if (index < mRoutePoints.size())
	{
		return mRoutePoints[index];
	}
	return msEmptyRoutePoint;
}

void RouteData::addRoutePoint(RoutePoint& routePoint)
{
	mRoutePoints.push_back(routePoint);
}

void RouteData::updateRoutePoint(unsigned int index, RoutePoint& routePoint)
{
	if (index < mRoutePoints.size())
	{
		mRoutePoints[index] = routePoint;
	}
}

void RouteData::setLifeTime(float lifeTime)
{
	mLifeTime = lifeTime;
}

float RouteData::getLifeTime() const
{
	return mLifeTime;
}