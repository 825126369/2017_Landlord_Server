#include "stdafx.h"
#include "Route.h"
#include "FishTools.h"
#include "RouteData.h"

BaseRoute::BaseRoute()
{
	mSimTime = 0.0f;
	mLifeTime = 60.0f;
	mIsEnd = false;
	mRotation = FishTools::getRotateAngle(mSpeed);

	mElapsedTime = 0;
}

BaseRoute::BaseRoute(const Vec2& pos, const Vec2& speed)
{
	mSimTime = 0.0f;
	mLifeTime = 60.0f;
	mPosition = pos;
	mSpeed = speed;
	mRotation =  FishTools::getRotateAngle(speed);
	mIsEnd = false;

	pushRouteParam(RouteType_Base);
	pushRouteParam(pos);
	pushRouteParam(speed);
	mElapsedTime = 0;
}

BaseRoute::~BaseRoute()
{

}

void BaseRoute::updateTime(float dt)
{
	mElapsedTime += dt;
	update(dt);
}

void BaseRoute::update(float dt)
{
	mPosition += mSpeed * dt;
}

void BaseRoute::setSimTime(float simTime)
{
	if (mSimTime < simTime)
	{
		float dt = simTime - mSimTime;
		mSimTime = simTime;
		if (dt > 60)
		{
			dt = 60;
		}
		while (dt > 0.1)
		{
			updateTime(0.1f);
			dt -= 0.1f;
		}
	}
}

const Vec2& BaseRoute::getPosition() const
{
	return mPosition;
}

float BaseRoute::getRotation() const
{
	return mRotation;
}

bool BaseRoute::isEnd() const
{
	return mIsEnd;
}

bool BaseRoute::canAutoEnd()
{
	return true;
}

void BaseRoute::setLifeTime(float lifeTime)
{
	mLifeTime = lifeTime;
}

float BaseRoute::getLifeTime()
{
	return mLifeTime;
}

void BaseRoute::pushRouteParam(int param)
{
	mRouteParam.push_back(param);
}

void BaseRoute::pushRouteParam(const Vec2& pos)
{
	mRouteParam.push_back(pos.x);
	mRouteParam.push_back(pos.y);
}

const std::vector<int32_t>& BaseRoute::getRouteParam()
{
	return mRouteParam;
}

float BaseRoute::getElapsedTime()
{
	return mElapsedTime;
}

void BaseRoute::setElapsedTime(float elapsedTime)
{
	setSimTime(elapsedTime);
}

AdvancedRoute::AdvancedRoute(const RouteData* routeData)
{
	mRouteData = routeData;

	mPosition.x = routeData->getStartPosX();
	mPosition.y = routeData->getStartPosY();

	mCurRotation = 0;
	mCurSpeed = 0;

	setLifeTime(routeData->getLifeTime());
	setRoutePoint(0);

	pushRouteParam(RouteType_Advanced);
	pushRouteParam(routeData->getRouteID());
}

AdvancedRoute::~AdvancedRoute()
{

}

void AdvancedRoute::update(float dt)
{
	mElapsedTime += dt;

	if (mIsEnd)
		return;

	//渐变时间
	if (mGradientTime > 0.0f)
	{
		mGradientTime -= dt;
		if (mGradientTime <= 0.0f)
		{
			auto& routePoint = mRouteData->getRoutePoint(mPointIndex);
			mCurSpeed = routePoint.mSpeed;
			mCurRotation = routePoint.mRatotion;
		}
		else
		{
			mCurSpeed += mAccSpeed * dt;
			mCurRotation += mAccRotation * dt;
		}
		mSpeed = mCurSpeed * FishTools::getRotateDirection(mCurRotation);
		mRotation = mCurRotation;
	}
	//存在持续时间
	if (mDuration > 0.0f)
	{
		if (mDuration > dt)
		{
			mDuration -= dt;
			mPosition += mSpeed * dt;
		}
		else
		{
			mPosition += mSpeed * mDuration;
			dt -= mDuration;
			setRoutePoint(++mPointIndex);
			update(dt);
		}
	}
	else
	{
		mPosition += mSpeed * dt;
	}
}

void AdvancedRoute::setRoutePoint(unsigned int pointIndex)
{
	mPointIndex = pointIndex;
	if (mPointIndex >= mRouteData->getRoutePointSize())
	{
		mIsEnd = true;
	}
	else
	{
		auto& routePoint = mRouteData->getRoutePoint(mPointIndex);
		mGradientTime = routePoint.mGradient;
		mDuration = routePoint.mDuration;
		//存在渐变
		if (mGradientTime > 0)
		{
			mAccRotation = (routePoint.mRatotion - mCurRotation)/mGradientTime;
			mAccSpeed = (routePoint.mSpeed - mCurSpeed)/mGradientTime;
		}
		else
		{
			mAccRotation = 0;
			mAccSpeed = 0;
			mCurSpeed = routePoint.mSpeed;
			mCurRotation = routePoint.mRatotion;

			mSpeed = mCurSpeed * FishTools::getRotateDirection(mCurRotation);
			mRotation = mCurRotation;
		}
	}
}

float AdvancedRoute::getCurSpeed()
{
	return mCurSpeed;
}

float AdvancedRoute::getDuration()
{
	return mDuration;
}

bool AdvancedRoute::canAutoEnd()
{
	if (mCurSpeed == 0.0f && mDuration == 0.0f)
	{
		return true;
	}
	return false;
}

CustomRoute::CustomRoute()
{
	mLifeTime = 60.0f;
	mCurRotation = 0;
	mCurSpeed = 0;

	mAccSpeed = 0;
	mAccRotation = 0;

	mGradientTime = 0;
	mDuration = 0;

	mPointIndex = -1;
}

CustomRoute::~CustomRoute()
{

}

void CustomRoute::init(const Vec2& pos, float rotation, float speed)
{
	mPosition = pos;
	mRotation = rotation;
	mCurRotation = rotation;
	mCurSpeed = speed;
}

void CustomRoute::addRoutePoint(const RoutePoint& routePoint)
{
	mRoutePoints.push_back(routePoint);
}

void CustomRoute::getNextRoutePoint()
{
	mPointIndex++;
	if (mPointIndex >= mRoutePoints.size())
	{
		mIsEnd = true;
	}
	else
	{
		RoutePoint& routePoint = mRoutePoints[mPointIndex];
		mGradientTime = routePoint.mGradient;
		mDuration = routePoint.mDuration;
		//存在渐变
		if (mGradientTime > 0)
		{
			mAccRotation = (routePoint.mRatotion - mCurRotation)/mGradientTime;
			mAccSpeed = (routePoint.mSpeed - mCurSpeed)/mGradientTime;
		}
		else
		{
			mAccRotation = 0;
			mAccSpeed = 0;
			mCurSpeed = routePoint.mSpeed;
			mCurRotation = routePoint.mRatotion;

			mSpeed = mCurSpeed * FishTools::getRotateDirection(mCurRotation);
			mRotation = mCurRotation;
		}
	}
}

void CustomRoute::update(float dt)
{
	mElapsedTime += dt;

	if (mPointIndex == -1)
	{
		getNextRoutePoint();
	}

	if (mIsEnd)
		return;

	//渐变时间
	if (mGradientTime > 0.0f)
	{
		mGradientTime -= dt;
		if (mGradientTime <= 0.0f)
		{
			auto& routePoint = mRoutePoints[mPointIndex];
			mCurSpeed = routePoint.mSpeed;
			mCurRotation = routePoint.mRatotion;
		}
		else
		{
			mCurSpeed += mAccSpeed * dt;
			mCurRotation += mAccRotation * dt;
		}
		mSpeed = mCurSpeed * FishTools::getRotateDirection(mCurRotation);
		mRotation = mCurRotation;
	}
	//存在持续时间
	if (mDuration > 0.0f)
	{
		if (mDuration > dt)
		{
			mDuration -= dt;
			mPosition += mSpeed * dt;
		}
		else
		{
			mPosition += mSpeed * mDuration;
			dt -= mDuration;
			getNextRoutePoint();
			update(dt);
		}
	}
	else
	{
		mPosition += mSpeed * dt;
	}
}

float CustomRoute::getCurSpeed()
{
	return mCurSpeed;
}

float CustomRoute::getDuration()
{
	return mDuration;
}

bool CustomRoute::canAutoEnd()
{
	if (mCurSpeed == 0.0f && mDuration == 0.0f)
	{
		return true;
	}
	return false;
}

OffsetRoute::OffsetRoute(const RouteData* routeData, const Vec2& initOffset, int randOffset, int randSeed)
	:m_random(randSeed)
{
	mCurOffset = initOffset;
	mOffset = randOffset;

	mRoute = new AdvancedRoute(routeData);
	setLifeTime(mRoute->getLifeTime());

	mRotation = mRoute->getRotation();
	mPosition = mRoute->getPosition() + mCurOffset;
	if (mRotation < 0.0f)
	{
		mRotation += 360.0f;
	}

	resetRandOffset();

	pushRouteParam(RouteType_Offset);
	pushRouteParam(routeData->getRouteID());
	pushRouteParam(initOffset);
	pushRouteParam(randOffset);
	pushRouteParam(randSeed);
}

OffsetRoute::~OffsetRoute()
{
	delete mRoute;
}

void OffsetRoute::resetRandOffset()
{
	mNextOffset = Vec2::forAngle(m_random.rand_int(0, 360)) * mOffset;
	mDistance = (mNextOffset - mCurOffset).length();

	mMoveDistance = 0.0f;
	mSpeed = (mNextOffset - mCurOffset).getNormalized();
	setState(1);
	mAccTime = 0.0f;
}

void OffsetRoute::setState(int state)
{
	mState = state;
}

void OffsetRoute::update(float dt)
{
	mElapsedTime += dt;

	if (dt == 0.0f)
		return;

	mRoute->updateTime(dt);
	mIsEnd = mRoute->isEnd();
	
	//固定偏移
	if (mOffset == 0.0f)
	{
		mPosition = mRoute->getPosition() + mCurOffset;
		mRotation = mRoute->getRotation();
	}
	else
	{
		float moveDis = mRoute->getCurSpeed() * 0.5f * dt;

		if (mState == 1)
		{
			mAccTime += dt*2;
			if (mAccTime >= 1.0f)
			{
				setState(2);
				mAccTime = 1.0f;
			}
			moveDis = moveDis * mAccTime;
		}
		else if (mState == 3)
		{
			mAccTime -= dt*2;
			if (mAccTime < 0.1f)
			{
				mAccTime = 0.1f;
			}
			moveDis = moveDis *mAccTime;
		}

		mMoveDistance += moveDis;
		if (mMoveDistance >= mDistance)
		{
			mCurOffset = mNextOffset;
			resetRandOffset();
		}
		else
		{
			mCurOffset += (mSpeed * moveDis);
			if (mState == 1 || mState == 2)
			{
				if (mDistance - mMoveDistance <= mRoute->getCurSpeed() * 0.25)
				{
					setState(3);
				}
			}
		}
		
		Vec2 oldPosition = mPosition;
		mPosition = mRoute->getPosition() + mCurOffset;
		auto rotation = FishTools::getRotateAngle(mPosition - oldPosition);
		if (rotation < 0)
		{
			rotation += 360.0f;
		}
		mRotation = rotation;
	}
}

bool OffsetRoute::canAutoEnd()
{
	return mRoute->canAutoEnd();
}