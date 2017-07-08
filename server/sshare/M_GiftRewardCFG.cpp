#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_GiftRewardCFG.h"
std::auto_ptr<M_GiftRewardCFG> M_GiftRewardCFG::msSingleton(nullptr);

int M_GiftRewardCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_GiftRewardCFGData* M_GiftRewardCFG::GetData(int RewardType)
{
	auto it = mMapData.find(RewardType);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_GiftRewardCFGData>& M_GiftRewardCFG::GetMapData()
{
	return mMapData;
}

void M_GiftRewardCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_GiftRewardCFG::Reload(const std::string& path)
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
		M_GiftRewardCFGData data;
		data.mRewardType = element->IntAttribute("RewardType");
		{
			const char* ItemId = element->Attribute("ItemId");
			std::vector<std::string> vecItemId;
			boost::split(vecItemId, ItemId, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecItemId.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecItemId[i].c_str(), &temp))
				{
					data.mItemId.push_back(temp);
				}
			}
		}
		{
			const char* ItemCount = element->Attribute("ItemCount");
			std::vector<std::string> vecItemCount;
			boost::split(vecItemCount, ItemCount, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecItemCount.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecItemCount[i].c_str(), &temp))
				{
					data.mItemCount.push_back(temp);
				}
			}
		}
		mMapData[data.mRewardType] = data;
		element = element->NextSiblingElement();
	}
}

void M_GiftRewardCFG::Load(const std::string& path)
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
		M_GiftRewardCFGData data;
		data.mRewardType = element->IntAttribute("RewardType");
		{
			const char* ItemId = element->Attribute("ItemId");
			std::vector<std::string> vecItemId;
			boost::split(vecItemId, ItemId, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecItemId.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecItemId[i].c_str(), &temp))
				{
					data.mItemId.push_back(temp);
				}
			}
		}
		{
			const char* ItemCount = element->Attribute("ItemCount");
			std::vector<std::string> vecItemCount;
			boost::split(vecItemCount, ItemCount, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecItemCount.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecItemCount[i].c_str(), &temp))
				{
					data.mItemCount.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mRewardType) != mMapData.end())std::cout <<"data refind:" << data.mRewardType << std::endl;
		assert(mMapData.find(data.mRewardType) == mMapData.end());
		mMapData.insert(std::make_pair(data.mRewardType, data));
		element = element->NextSiblingElement();
	}
}

void M_GiftRewardCFG::Load()
{
	Load("../Config/M_GiftRewardCFG.xml");
}

M_GiftRewardCFG* M_GiftRewardCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_GiftRewardCFG());
	}
	return msSingleton.get();
}
