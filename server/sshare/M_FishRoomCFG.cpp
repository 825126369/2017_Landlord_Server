#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_FishRoomCFG.h"
std::auto_ptr<M_FishRoomCFG> M_FishRoomCFG::msSingleton(nullptr);

int M_FishRoomCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_FishRoomCFGData* M_FishRoomCFG::GetData(int RoomID)
{
	auto it = mMapData.find(RoomID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_FishRoomCFGData>& M_FishRoomCFG::GetMapData()
{
	return mMapData;
}

void M_FishRoomCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_FishRoomCFG::Reload(const std::string& path)
{
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
	auto element = root->FirstChildElement("Data");
	while (element != NULL)
	{
		M_FishRoomCFGData data;
		data.mRoomID = element->IntAttribute("RoomID");
		data.mRoomName = element->Attribute("RoomName");
		data.mRoomImage = element->Attribute("RoomImage");
		data.mArmatureName = element->Attribute("ArmatureName");
		data.mArmaturePath = element->Attribute("ArmaturePath");
		data.mGoldCondition = element->IntAttribute("GoldCondition");
		data.mVipCondition = element->IntAttribute("VipCondition");
		data.mTicketCondition = element->IntAttribute("TicketCondition");
		data.mLevelCondition = element->IntAttribute("LevelCondition");
		data.mMinRate = element->IntAttribute("MinRate");
		data.mMaxRate = element->IntAttribute("MaxRate");
		data.mStepRate = element->IntAttribute("StepRate");
		data.mTableCount = element->IntAttribute("TableCount");
		data.mIsOpen = element->BoolAttribute("IsOpen");
		data.mPowerParam = element->IntAttribute("PowerParam");
		data.mBuyPowerCost = element->IntAttribute("BuyPowerCost");
		data.mMissileCost = element->IntAttribute("MissileCost");
		data.mOpenProtect = element->BoolAttribute("OpenProtect");
		data.mCanGetExp = element->BoolAttribute("CanGetExp");
		data.mCheckOpenRate = element->BoolAttribute("CheckOpenRate");
		data.mFreeItem = element->BoolAttribute("FreeItem");
		mMapData[data.mRoomID] = data;
		element = element->NextSiblingElement();
	}
}

void M_FishRoomCFG::Load(const std::string& path)
{
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
	auto element = root->FirstChildElement("Data");
	while (element != NULL)
	{
		M_FishRoomCFGData data;
		data.mRoomID = element->IntAttribute("RoomID");
		data.mRoomName = element->Attribute("RoomName");
		data.mRoomImage = element->Attribute("RoomImage");
		data.mArmatureName = element->Attribute("ArmatureName");
		data.mArmaturePath = element->Attribute("ArmaturePath");
		data.mGoldCondition = element->IntAttribute("GoldCondition");
		data.mVipCondition = element->IntAttribute("VipCondition");
		data.mTicketCondition = element->IntAttribute("TicketCondition");
		data.mLevelCondition = element->IntAttribute("LevelCondition");
		data.mMinRate = element->IntAttribute("MinRate");
		data.mMaxRate = element->IntAttribute("MaxRate");
		data.mStepRate = element->IntAttribute("StepRate");
		data.mTableCount = element->IntAttribute("TableCount");
		data.mIsOpen = element->BoolAttribute("IsOpen");
		data.mPowerParam = element->IntAttribute("PowerParam");
		data.mBuyPowerCost = element->IntAttribute("BuyPowerCost");
		data.mMissileCost = element->IntAttribute("MissileCost");
		data.mOpenProtect = element->BoolAttribute("OpenProtect");
		data.mCanGetExp = element->BoolAttribute("CanGetExp");
		data.mCheckOpenRate = element->BoolAttribute("CheckOpenRate");
		data.mFreeItem = element->BoolAttribute("FreeItem");
		if (mMapData.find(data.mRoomID) != mMapData.end())std::cout <<"data refind:" << data.mRoomID << std::endl;
		assert(mMapData.find(data.mRoomID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mRoomID, data));
		element = element->NextSiblingElement();
	}
}

void M_FishRoomCFG::Load()
{
	Load("../Config/M_FishRoomCFG.xml");
}

M_FishRoomCFG* M_FishRoomCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_FishRoomCFG());
	}
	return msSingleton.get();
}
