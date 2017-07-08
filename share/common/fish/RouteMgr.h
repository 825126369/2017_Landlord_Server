#pragma once
#ifdef CLIENT
#include "CocosHead.h"
#else
#include "logic_pos.h"
#endif
#include "FishCommonHead.h"

struct RouteData;
class RouteMgr
{
public:
	RouteMgr();
	virtual ~RouteMgr();
	// [6/4/2015 wangwei] 加载资源
	void loadRouteData();
	void loadRouteData(const std::string& path);
	// [6/4/2015 wangwei] 保存资源
	void saveRouteData();

	// [6/4/2015 wangwei] 更新路径
	void updateRouteData(const RouteData& routeData);
	// [6/4/2015 wangwei] 删除路径
	void deleteRouteData(int routeID);

	typedef std::map<int, RouteData> MapRouteData;
	const MapRouteData& getMapRouteData();

	RoutePtr createRoute(int routeID);
	RoutePtr createRoute(int routeID, const Vec2& initOffset, int randOffset, int randSeed);
	RoutePtr createRoute(int fishID, const Vec2& pos, const Vec2& speed);
	RoutePtr createRotateRoute(int fishID, const Vec2& centerPos, const Vec2& pos, float rotation, float rotateTime, float mMoveSpeed);
	RoutePtr createSpecialRoute(int fishID, const Vec2& pos, const Vec2& speed, float delayTime, float moveTime, float waitTime);
	RoutePtr createRoute(int fishID, std::vector<int32_t> routeParam, float elapsedTime);

	const RouteData* getRouteData(int routeID);

	static RouteMgr* getSingleton();
	static void release();
protected:
	MapRouteData mMapRouteData;

	static std::auto_ptr<RouteMgr> msSingleton;
};