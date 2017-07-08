#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_OnlineRewardCFG.h"
std::auto_ptr<M_OnlineRewardCFG> M_OnlineRewardCFG::msSingleton(nullptr);

int M_OnlineRewardCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_OnlineRewardCFGData* M_OnlineRewardCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_OnlineRewardCFGData>& M_OnlineRewardCFG::GetMapData()
{
	return mMapData;
}

void M_OnlineRewardCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_OnlineRewardCFG::Reload(const std::string& path)
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
		M_OnlineRewardCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mStartTime = element->Attribute("StartTime");
		data.mEndTime = element->Attribute("EndTime");
		data.mConditionVip = element->IntAttribute("ConditionVip");
		data.mRewardCoin = element->IntAttribute("RewardCoin");
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_OnlineRewardCFG::Load(const std::string& path)
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
		M_OnlineRewardCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mStartTime = element->Attribute("StartTime");
		data.mEndTime = element->Attribute("EndTime");
		data.mConditionVip = element->IntAttribute("ConditionVip");
		data.mRewardCoin = element->IntAttribute("RewardCoin");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_OnlineRewardCFG::Load()
{
	Load("../Config/M_OnlineRewardCFG.xml");
}

M_OnlineRewardCFG* M_OnlineRewardCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_OnlineRewardCFG());
	}
	return msSingleton.get();
}
