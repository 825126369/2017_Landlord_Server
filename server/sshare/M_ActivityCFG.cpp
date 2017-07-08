#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_ActivityCFG.h"
std::auto_ptr<M_ActivityCFG> M_ActivityCFG::msSingleton(nullptr);

int M_ActivityCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_ActivityCFGData* M_ActivityCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_ActivityCFGData>& M_ActivityCFG::GetMapData()
{
	return mMapData;
}

void M_ActivityCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_ActivityCFG::Reload(const std::string& path)
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
		M_ActivityCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mActivityName = element->Attribute("ActivityName");
		data.mStartTime = element->Attribute("StartTime");
		data.mEndTime = element->Attribute("EndTime");
		data.mActivityType = element->IntAttribute("ActivityType");
		data.mRechargeRMB = element->IntAttribute("RechargeRMB");
		data.mVipLevel = element->IntAttribute("VipLevel");
		data.mLoginDay = element->Attribute("LoginDay");
		{
			const char* RewardList = element->Attribute("RewardList");
			std::vector<std::string> vecRewardList;
			boost::split(vecRewardList, RewardList, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecRewardList.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecRewardList[i].c_str(), &temp))
				{
					data.mRewardList.push_back(temp);
				}
			}
		}
		{
			const char* RewardCount = element->Attribute("RewardCount");
			std::vector<std::string> vecRewardCount;
			boost::split(vecRewardCount, RewardCount, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecRewardCount.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecRewardCount[i].c_str(), &temp))
				{
					data.mRewardCount.push_back(temp);
				}
			}
		}
		data.mActivityRewardMailTitle = element->Attribute("ActivityRewardMailTitle");
		data.mActivityRewardMailContent = element->Attribute("ActivityRewardMailContent");
		data.mReceiveWay = element->IntAttribute("ReceiveWay");
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_ActivityCFG::Load(const std::string& path)
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
		M_ActivityCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mActivityName = element->Attribute("ActivityName");
		data.mStartTime = element->Attribute("StartTime");
		data.mEndTime = element->Attribute("EndTime");
		data.mActivityType = element->IntAttribute("ActivityType");
		data.mRechargeRMB = element->IntAttribute("RechargeRMB");
		data.mVipLevel = element->IntAttribute("VipLevel");
		data.mLoginDay = element->Attribute("LoginDay");
		{
			const char* RewardList = element->Attribute("RewardList");
			std::vector<std::string> vecRewardList;
			boost::split(vecRewardList, RewardList, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecRewardList.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecRewardList[i].c_str(), &temp))
				{
					data.mRewardList.push_back(temp);
				}
			}
		}
		{
			const char* RewardCount = element->Attribute("RewardCount");
			std::vector<std::string> vecRewardCount;
			boost::split(vecRewardCount, RewardCount, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecRewardCount.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecRewardCount[i].c_str(), &temp))
				{
					data.mRewardCount.push_back(temp);
				}
			}
		}
		data.mActivityRewardMailTitle = element->Attribute("ActivityRewardMailTitle");
		data.mActivityRewardMailContent = element->Attribute("ActivityRewardMailContent");
		data.mReceiveWay = element->IntAttribute("ReceiveWay");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_ActivityCFG::Load()
{
	Load("../Config/M_ActivityCFG.xml");
}

M_ActivityCFG* M_ActivityCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_ActivityCFG());
	}
	return msSingleton.get();
}
