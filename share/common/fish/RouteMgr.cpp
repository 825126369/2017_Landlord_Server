#include "stdafx.h"
#include "RouteMgr.h"
#include "RouteData.h"
#include "Route.h"
#include "FishTools.h"
#include "Fish_FishCFG.h"

std::auto_ptr<RouteMgr> RouteMgr::msSingleton(nullptr);

RouteMgr::RouteMgr()
{
}

RouteMgr::~RouteMgr()
{
	mMapRouteData.clear();
}

#ifdef CLIENT
#include "tinyxml2/tinyxml2.h"
void RouteMgr::loadRouteData()
{
	mMapRouteData.clear();

	tinyxml2::XMLDocument xmlDoc;
	std::string content = FileUtils::getInstance()->getStringFromFile("Config/RouteDataCFG.xml");
	if (content == "")
		return;

	auto result = xmlDoc.Parse(content.c_str(), content.length());
	if (result != tinyxml2::XML_SUCCESS)
	{
		CCLOGERROR("Result:%d", result);
		CCASSERT(false, "result != tinyxml2::XML_SUCCESS");
		return;
	}
	auto root = xmlDoc.RootElement();
	if (root == NULL)
	{
		CCASSERT(false, "root == NULL");
		return;
	}
	auto element = root->FirstChildElement("RouteData");
	while (element != NULL)
	{
		RouteData routeData;
		routeData.setRouteID(element->IntAttribute("ID"));
		routeData.setStartPosX(element->FloatAttribute("StartPosX"));
		routeData.setStartPosY(element->FloatAttribute("StartPosY"));
		routeData.setLifeTime(element->FloatAttribute("LifeTime"));
		auto pointElement = element->FirstChildElement("PointData");
		while (pointElement != NULL)
		{
			RoutePoint routePoint;
			routePoint.mSpeed = pointElement->FloatAttribute("Speed");
			routePoint.mRatotion = pointElement->FloatAttribute("Ratotion");
			routePoint.mGradient = pointElement->FloatAttribute("Gradient");
			routePoint.mDuration = pointElement->FloatAttribute("Duration");

			routeData.addRoutePoint(routePoint);

			pointElement = pointElement->NextSiblingElement();
		}

		CCASSERT(mMapRouteData.find(routeData.getRouteID()) == mMapRouteData.end(), "data.mKey is exists");
		mMapRouteData.insert(std::make_pair(routeData.getRouteID(), routeData));
		element = element->NextSiblingElement();
	}
	CCLOG("RouteDataCFG Loaded. Load Data:%u", mMapRouteData.size());
}

void RouteMgr::saveRouteData()
{
	tinyxml2::XMLDocument xmlDoc;
	auto rootElement = xmlDoc.NewElement("root");

	for (auto it = mMapRouteData.begin(); it != mMapRouteData.end(); ++it)
	{
		RouteData& routeData = it->second;
		auto routeElement = xmlDoc.NewElement("RouteData");
		routeElement->SetAttribute("ID", routeData.getRouteID());
		routeElement->SetAttribute("StartPosX", routeData.getStartPosX());
		routeElement->SetAttribute("StartPosY", routeData.getStartPosY());
		routeElement->SetAttribute("LifeTime", routeData.getLifeTime());

		for (unsigned int i = 0; i < routeData.getRoutePointSize(); i++)
		{
			auto& pointData = routeData.getRoutePoint(i);
			auto pointElement = xmlDoc.NewElement("PointData");

			pointElement->SetAttribute("Speed", pointData.mSpeed);
			pointElement->SetAttribute("Ratotion", pointData.mRatotion);
			pointElement->SetAttribute("Gradient", pointData.mGradient);
			pointElement->SetAttribute("Duration", pointData.mDuration);

			routeElement->LinkEndChild(pointElement);
		}
		rootElement->LinkEndChild(routeElement);
	}
	xmlDoc.LinkEndChild(rootElement);
	std::string fullPath = FileUtils::getInstance()->fullPathForFilename("res/RouteDataCFG.xml");
	xmlDoc.SaveFile(fullPath.c_str());

	CCLOG("RouteDataCFG save. save Data:%u", mMapRouteData.size());
}
#else
#include "tinyxml2.h"
void RouteMgr::loadRouteData(const std::string& path)
{
	mMapRouteData.clear();

	std::ifstream readStream(path, std::ios::binary);
	if (!readStream.is_open())
	{
		assert(false);
		return;
	}
	readStream.seekg(0, std::ios::end);
	int fileSize = readStream.tellg();
	boost::shared_array<char> buffer(new char[fileSize+1]);
	buffer.get()[fileSize] = '\0';
	readStream.seekg(0, std::ios::beg);
	readStream.read(buffer.get(), fileSize);
	readStream.close();

	tinyxml2::XMLDocument xmlDoc;
	auto result = xmlDoc.Parse(buffer.get(), fileSize);
	if (result != tinyxml2::XML_SUCCESS)
	{
		assert(false);
		return;
	}
	auto root = xmlDoc.RootElement();
	if (root == NULL)
	{
		assert(false);
		return;
	}
	auto element = root->FirstChildElement("RouteData");
	while (element != NULL)
	{
		RouteData routeData;
		routeData.setRouteID(element->IntAttribute("ID"));
		routeData.setStartPosX(element->FloatAttribute("StartPosX"));
		routeData.setStartPosY(element->FloatAttribute("StartPosY"));
		routeData.setLifeTime(element->FloatAttribute("LifeTime"));
		auto pointElement = element->FirstChildElement("PointData");
		while (pointElement != NULL)
		{
			RoutePoint routePoint;
			routePoint.mSpeed = pointElement->FloatAttribute("Speed");
			routePoint.mRatotion = pointElement->FloatAttribute("Ratotion");
			routePoint.mGradient = pointElement->FloatAttribute("Gradient");
			routePoint.mDuration = pointElement->FloatAttribute("Duration");

			routeData.addRoutePoint(routePoint);

			pointElement = pointElement->NextSiblingElement();
		}
		mMapRouteData.insert(std::make_pair(routeData.getRouteID(), routeData));
		element = element->NextSiblingElement();
	}
}

void RouteMgr::loadRouteData()
{
	loadRouteData("../Config/RouteDataCFG.xml");
}

void RouteMgr::saveRouteData()
{
	throw 1;
}
#endif

void RouteMgr::updateRouteData(const RouteData& routeData)
{
	auto it = mMapRouteData.find(routeData.getRouteID());
	if (it != mMapRouteData.end())
	{
		mMapRouteData.erase(it);
	}
	
	mMapRouteData.insert(std::make_pair(routeData.getRouteID(), routeData));
	saveRouteData();
}

void RouteMgr::deleteRouteData(int routeID)
{
	auto it = mMapRouteData.find(routeID);
	if (it != mMapRouteData.end())
	{
		mMapRouteData.erase(it);
		saveRouteData();
	}
}

const RouteMgr::MapRouteData& RouteMgr::getMapRouteData()
{
	return mMapRouteData;
}

RoutePtr RouteMgr::createRoute(int fishID, const Vec2& pos, const Vec2& speed)
{
	auto route = new BaseRoute(pos, speed);
	auto fishCfgData = Fish_FishCFG::GetSingleton()->GetData(fishID);
	float lifeTime = (fishCfgData == nullptr)?60.0f:FishTools::getMoveTime(pos, speed, fishCfgData->mShowRadius);
	route->setLifeTime(lifeTime);
	return RoutePtr(route);
}

RoutePtr RouteMgr::createRoute(int routeID)
{
	auto it = mMapRouteData.find(routeID);
	if (it != mMapRouteData.end())
	{
		return RoutePtr(new AdvancedRoute(&it->second));
	}
	return RoutePtr(nullptr);
}

RoutePtr RouteMgr::createRoute(int routeID, const Vec2& initOffset, int randOffset, int randSeed)
{
	auto it = mMapRouteData.find(routeID);
	if (it != mMapRouteData.end())
	{
		return RoutePtr(new OffsetRoute(&it->second, initOffset, randOffset, randSeed));
	}
	return RoutePtr(nullptr);
}

RoutePtr RouteMgr::createRotateRoute(int fishID, const Vec2& centerPos, const Vec2& pos, float rotation, float rotateTime, float mMoveSpeed)
{
	auto route = new CustomRoute();
	route->pushRouteParam(RouteType_Rotate);
	route->pushRouteParam(centerPos);
	route->pushRouteParam(pos);
	route->pushRouteParam(rotation);
	route->pushRouteParam(rotateTime);
	route->pushRouteParam(mMoveSpeed);

	Vec2 dir = pos - centerPos;
	dir.rotate(Vec2::ZERO, -FishTools::PI/2);
	float initRotation = FishTools::getRotateAngle(dir);
	float speed = dir.length() * FishTools::PI/(180/rotation);
	route->init(pos, initRotation, speed);
	RoutePoint routePoint;
	routePoint.mDuration = rotateTime;
	routePoint.mGradient = rotateTime;
	routePoint.mRatotion = initRotation + rotation * rotateTime;
	routePoint.mSpeed = speed;
	route->addRoutePoint(routePoint);
	routePoint.mDuration = 0;
	routePoint.mGradient = 0;
	routePoint.mRatotion = initRotation + rotation * rotateTime;
	routePoint.mSpeed = mMoveSpeed;
	route->addRoutePoint(routePoint);

	//计算存活时间
	float lifeTime = rotateTime;

	dir = pos - centerPos;
	float angle = dir.getAngle() + rotation * rotateTime;
	Vec2 realPos = centerPos + Vec2::forAngle(angle) * dir.length();
	Vec2 moveSpeed = FishTools::getRotateDirection(initRotation + rotation * rotateTime) * mMoveSpeed;
	auto fishCfgData = Fish_FishCFG::GetSingleton()->GetData(fishID);

	lifeTime += (fishCfgData == nullptr)?60.0f:FishTools::getMoveTime(realPos, moveSpeed, fishCfgData->mLayer);
	route->setLifeTime(lifeTime);

	return RoutePtr(route);
}

RoutePtr RouteMgr::createSpecialRoute(int fishID, const Vec2& pos, const Vec2& speed, float delayTime, float moveTime, float waitTime)
{
	auto route = new CustomRoute();
	route->pushRouteParam(RouteType_Rotate);
	route->pushRouteParam(pos);
	route->pushRouteParam(speed);
	route->pushRouteParam(delayTime);
	route->pushRouteParam(moveTime);
	route->pushRouteParam(waitTime);

	float initRotation = FishTools::getRotateAngle(speed);
	route->init(pos, initRotation, 0);
	RoutePoint routePoint;
	routePoint.mDuration = delayTime;
	routePoint.mGradient = 0;
	routePoint.mRatotion = initRotation;
	routePoint.mSpeed = 0;
	route->addRoutePoint(routePoint);
	routePoint.mDuration = moveTime;
	routePoint.mGradient = 0;
	routePoint.mRatotion = initRotation;
	routePoint.mSpeed = speed.length();
	route->addRoutePoint(routePoint);
	routePoint.mDuration = waitTime;
	routePoint.mGradient = 0;
	routePoint.mRatotion = initRotation;
	routePoint.mSpeed = 0;
	route->addRoutePoint(routePoint);
	routePoint.mDuration = 0;
	routePoint.mGradient = 0;
	routePoint.mRatotion = initRotation;
	routePoint.mSpeed = speed.length();
	route->addRoutePoint(routePoint);

	//计算存活时间
	float lifeTime = delayTime + waitTime;
	auto fishCfgData = Fish_FishCFG::GetSingleton()->GetData(fishID);
	lifeTime += (fishCfgData == nullptr)?60.0f:FishTools::getMoveTime(pos, speed, fishCfgData->mShowRadius);
	route->setLifeTime(lifeTime);
	return RoutePtr(route);
}

RoutePtr RouteMgr::createRoute(int fishID, std::vector<int32_t> routeParam, float elapsedTime)
{
	if (routeParam.size() < 1)
	{
		return RoutePtr(nullptr);
	}

	RouteType routeType = (RouteType)routeParam[0];
	switch (routeType)
	{
	case RouteType_Base:
		{
			RoutePtr route = createRoute(fishID, Vec2(routeParam[1], routeParam[2]), Vec2(routeParam[3], routeParam[4]));
			route->setElapsedTime(elapsedTime);
			return route;
		}
	case RouteType_Advanced:
		{
			RoutePtr route = createRoute(routeParam[1]);
			route->setElapsedTime(elapsedTime);
			return route;
		}
	case RouteType_Rotate:
		{
			RoutePtr route = createRotateRoute(fishID, Vec2(routeParam[1], routeParam[2]), Vec2(routeParam[3], routeParam[4]), routeParam[5], routeParam[6], routeParam[7]);
			route->setElapsedTime(elapsedTime);
			return route;
		}
	case RouteType_Special:
		{
			RoutePtr route = createSpecialRoute(fishID, Vec2(routeParam[1], routeParam[2]), Vec2(routeParam[3], routeParam[4]), routeParam[5], routeParam[6], routeParam[7]);
			route->setElapsedTime(elapsedTime);
			return route;
		}
	case RouteType_Offset:
		{
			RoutePtr route = createRoute(routeParam[1], Vec2(routeParam[2], routeParam[3]), routeParam[4], routeParam[5]);
			route->setElapsedTime(elapsedTime);
			return route;
		}
	default:
		{
			return RoutePtr(nullptr);
		}
	}
}

const RouteData* RouteMgr::getRouteData(int routeID)
{
	auto it = mMapRouteData.find(routeID);
	if (it != mMapRouteData.end())
	{
		return &(it->second);
	}
	return nullptr;
}

RouteMgr* RouteMgr::getSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new RouteMgr());
	}
	return msSingleton.get();
}

void RouteMgr::release()
{
	msSingleton.reset(nullptr);
}