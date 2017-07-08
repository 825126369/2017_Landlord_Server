#pragma once
#ifdef CLIENT
#include "CocosHead.h"
#else
#include "logic_pos.h"
#endif
#include "FishCommonHead.h"
#include "enable_random.h"

struct RoutePoint
{
	float mSpeed;			//速度
	float mRatotion;		//角度
	float mGradient;		//渐变时间
	float mDuration;		//持续时间
};

enum RouteType
{
	RouteType_Base = 1,
	RouteType_Advanced,
	RouteType_Rotate,
	RouteType_Special,
	RouteType_Offset,
};

//路径类
class BaseRoute
{
public:
	BaseRoute(const Vec2& pos, const Vec2& speed);
	virtual ~BaseRoute();

	void updateTime(float dt);
	void setSimTime(float simTime);

	const Vec2& getPosition() const;
	float getRotation() const;
	bool isEnd() const;
	//自己判断位置是否结束
	virtual bool canAutoEnd();	

	float getLifeTime();
	void setLifeTime(float lifeTime);

	//初始化参数
	void pushRouteParam(int param);
	void pushRouteParam(const Vec2& pos);
	const std::vector<int32_t>& getRouteParam();
	float getElapsedTime();
	void setElapsedTime(float elapsedTime);
protected:
	BaseRoute();
	virtual void update(float dt);
protected:
	bool mIsEnd;
	Vec2 mPosition;
	Vec2 mSpeed;
	float mRotation;

	float mLifeTime;
	float mSimTime;		//模拟时间

	std::vector<int32_t> mRouteParam;
	float mElapsedTime;
};

struct RouteData;
class AdvancedRoute : public BaseRoute
{
public:
	AdvancedRoute(const RouteData* routeData);
	virtual ~AdvancedRoute();

	float getCurSpeed();
	float getDuration();
	virtual bool canAutoEnd();	
protected:
	virtual void update(float dt);
	void setRoutePoint(unsigned int pointIndex);
protected:
	//目前速度
	float mCurSpeed;
	float mCurRotation;
	//加速度
	float mAccSpeed;
	float mAccRotation;

	//渐变时间
	float mGradientTime;
	//持续时间
	float mDuration;

	unsigned int mPointIndex;
	const RouteData* mRouteData;
};

class CustomRoute : public BaseRoute
{
public:
	CustomRoute();
	virtual ~CustomRoute();

	void init(const Vec2& pos, float rotation, float speed);
	void addRoutePoint(const RoutePoint& routePoint);

	void setRoutePoint(unsigned int pointIndex);

	float getCurSpeed();
	float getDuration();
	virtual bool canAutoEnd();
protected:
	virtual void update(float dt);
	void getNextRoutePoint();
protected:
	//目前速度
	float mCurSpeed;
	float mCurRotation;
	//加速度
	float mAccSpeed;
	float mAccRotation;

	//渐变时间
	float mGradientTime;
	//持续时间
	float mDuration;

	int mPointIndex;
	std::vector<RoutePoint> mRoutePoints;
};

class OffsetRoute : public BaseRoute
{
public:
	OffsetRoute(const RouteData* routeData, const Vec2& initOffset, int randOffset, int randSeed);
	virtual ~OffsetRoute();

	virtual bool canAutoEnd();
protected:
	virtual void update(float dt);
	void resetRandOffset();
	void setState(int state);
private:
	AdvancedRoute* mRoute;

	Vec2 mCurOffset;
	Vec2 mNextOffset;
	//Vec2 mDirection;
	int mOffset;

	float mMoveDistance;
	float mDistance;
	//加速时间
	float mAccTime;
	int mState;

	enable_random<boost::rand48> m_random;
};