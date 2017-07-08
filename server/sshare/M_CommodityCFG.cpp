#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_CommodityCFG.h"
std::auto_ptr<M_CommodityCFG> M_CommodityCFG::msSingleton(nullptr);

int M_CommodityCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_CommodityCFGData* M_CommodityCFG::GetData(int CommodityId)
{
	auto it = mMapData.find(CommodityId);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_CommodityCFGData>& M_CommodityCFG::GetMapData()
{
	return mMapData;
}

void M_CommodityCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_CommodityCFG::Reload(const std::string& path)
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
		M_CommodityCFGData data;
		data.mCommodityId = element->IntAttribute("CommodityId");
		data.mName = element->Attribute("Name");
		data.mCommodityType = element->IntAttribute("CommodityType");
		data.mPriceType = element->IntAttribute("PriceType");
		data.mPrice = element->IntAttribute("Price");
		{
			const char* Item = element->Attribute("Item");
			std::vector<std::string> vecItem;
			boost::split(vecItem, Item, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecItem.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecItem[i].c_str(), &temp))
				{
					data.mItem.push_back(temp);
				}
			}
		}
		data.mIsCanBuy = element->BoolAttribute("IsCanBuy");
		{
			const char* Count = element->Attribute("Count");
			std::vector<std::string> vecCount;
			boost::split(vecCount, Count, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecCount.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecCount[i].c_str(), &temp))
				{
					data.mCount.push_back(temp);
				}
			}
		}
		mMapData[data.mCommodityId] = data;
		element = element->NextSiblingElement();
	}
}

void M_CommodityCFG::Load(const std::string& path)
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
		M_CommodityCFGData data;
		data.mCommodityId = element->IntAttribute("CommodityId");
		data.mName = element->Attribute("Name");
		data.mCommodityType = element->IntAttribute("CommodityType");
		data.mPriceType = element->IntAttribute("PriceType");
		data.mPrice = element->IntAttribute("Price");
		{
			const char* Item = element->Attribute("Item");
			std::vector<std::string> vecItem;
			boost::split(vecItem, Item, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecItem.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecItem[i].c_str(), &temp))
				{
					data.mItem.push_back(temp);
				}
			}
		}
		data.mIsCanBuy = element->BoolAttribute("IsCanBuy");
		{
			const char* Count = element->Attribute("Count");
			std::vector<std::string> vecCount;
			boost::split(vecCount, Count, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecCount.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecCount[i].c_str(), &temp))
				{
					data.mCount.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mCommodityId) != mMapData.end())std::cout <<"data refind:" << data.mCommodityId << std::endl;
		assert(mMapData.find(data.mCommodityId) == mMapData.end());
		mMapData.insert(std::make_pair(data.mCommodityId, data));
		element = element->NextSiblingElement();
	}
}

void M_CommodityCFG::Load()
{
	Load("../Config/M_CommodityCFG.xml");
}

M_CommodityCFG* M_CommodityCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_CommodityCFG());
	}
	return msSingleton.get();
}
