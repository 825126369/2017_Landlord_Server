#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "HappySupremacy_RoomCFG.h"
std::auto_ptr<HappySupremacy_RoomCFG> HappySupremacy_RoomCFG::msSingleton(nullptr);

int HappySupremacy_RoomCFG::GetCount()
{
	return (int)mMapData.size();
}

const HappySupremacy_RoomCFGData* HappySupremacy_RoomCFG::GetData(int RoomID)
{
	auto it = mMapData.find(RoomID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, HappySupremacy_RoomCFGData>& HappySupremacy_RoomCFG::GetMapData()
{
	return mMapData;
}

void HappySupremacy_RoomCFG::Reload()
{
	mMapData.clear();
	Load();
}

void HappySupremacy_RoomCFG::Load()
{
	std::ifstream readStream("../Config/HappySupremacy_RoomCFG.xml", std::ios::binary);
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
		HappySupremacy_RoomCFGData data;
		data.mRoomID = element->IntAttribute("RoomID");
		data.mRoomName = element->Attribute("RoomName");
		{
			const char* WeightList = element->Attribute("WeightList");
			std::vector<std::string> vecWeightList;
			boost::split(vecWeightList, WeightList, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecWeightList.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecWeightList[i].c_str(), &temp))
				{
					data.mWeightList.push_back(temp);
				}
			}
		}
		data.mIsOpen = element->BoolAttribute("IsOpen");
		data.mBankerCondition = element->IntAttribute("BankerCondition");
		data.mFirstBankerCost = element->IntAttribute("FirstBankerCost");
		data.mAddBankerCost = element->IntAttribute("AddBankerCost");
		data.mLeaveBankerCost = element->IntAttribute("LeaveBankerCost");
		data.mAutoLeaveBanker = element->IntAttribute("AutoLeaveBanker");
		data.mPlayerMaxCount = element->IntAttribute("PlayerMaxCount");
		{
			const char* RobTag = element->Attribute("RobTag");
			std::vector<std::string> vecRobTag;
			boost::split(vecRobTag, RobTag, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecRobTag.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecRobTag[i].c_str(), &temp))
				{
					data.mRobTag.push_back(temp);
				}
			}
		}
		{
			const char* CreateProb = element->Attribute("CreateProb");
			std::vector<std::string> vecCreateProb;
			boost::split(vecCreateProb, CreateProb, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecCreateProb.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecCreateProb[i].c_str(), &temp))
				{
					data.mCreateProb.push_back(temp);
				}
			}
		}
		data.mRoomImage = element->Attribute("RoomImage");
		data.mVipCondition = element->IntAttribute("VipCondition");
		data.mGoldCondition = element->IntAttribute("GoldCondition");
		data.mBetMax = element->IntAttribute("BetMax");
		data.mRobCount = element->IntAttribute("RobCount");
		if (mMapData.find(data.mRoomID) != mMapData.end())std::cout <<"data refind:" << data.mRoomID << std::endl;
		assert(mMapData.find(data.mRoomID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mRoomID, data));
		element = element->NextSiblingElement();
	}
}

HappySupremacy_RoomCFG* HappySupremacy_RoomCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new HappySupremacy_RoomCFG());
	}
	return msSingleton.get();
}
