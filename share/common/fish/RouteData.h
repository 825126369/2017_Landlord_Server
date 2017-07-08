#pragma once
#include "Route.h"

//路径配置数据
struct RouteData
{
public:
	RouteData();
	~RouteData();

	void clearUp();

	int getRouteID() const;
	void setRouteID(int routeID);

	void setStartPos(float posX, float posY);
	void setStartPosX(float posX);
	void setStartPosY(float posY);
	float getStartPosX() const;
	float getStartPosY() const;

	unsigned int getRoutePointSize() const;
	const RoutePoint& getRoutePoint(unsigned int index) const;
	void addRoutePoint(RoutePoint& routePoint);
	void updateRoutePoint(unsigned int index, RoutePoint& routePoint);

	void setLifeTime(float lifeTime);
	float getLifeTime() const;
protected:
	int mRouteID;

	//开始位置
	float mStartPosX;	
	float mStartPosY;

	//路径点
	std::vector<RoutePoint> mRoutePoints;

	float mLifeTime;

	static RoutePoint msEmptyRoutePoint;
};