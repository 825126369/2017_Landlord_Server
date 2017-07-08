#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "HappySupremacy_RoomStockCFG.h"
std::auto_ptr<HappySupremacy_RoomStockCFG> HappySupremacy_RoomStockCFG::msSingleton(nullptr);

int HappySupremacy_RoomStockCFG::GetCount()
{
	return (int)mMapData.size();
}

const HappySupremacy_RoomStockCFGData* HappySupremacy_RoomStockCFG::GetData(int RoomID)
{
	auto it = mMapData.find(RoomID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, HappySupremacy_RoomStockCFGData>& HappySupremacy_RoomStockCFG::GetMapData()
{
	return mMapData;
}

void HappySupremacy_RoomStockCFG::Reload()
{
	mMapData.clear();
	Load();
}

void HappySupremacy_RoomStockCFG::Load()
{
	std::ifstream readStream("../Config/HappySupremacy_RoomStockCFG.xml", std::ios::binary);
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
		HappySupremacy_RoomStockCFGData data;
		data.mRoomID = element->IntAttribute("RoomID");
		data.mDeduct = element->FloatAttribute("Deduct");
		data.mDefaultStock = element->IntAttribute("DefaultStock");
		{
			const char* Stock = element->Attribute("Stock");
			std::vector<std::string> vecStock;
			boost::split(vecStock, Stock, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecStock.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecStock[i].c_str(), &temp))
				{
					data.mStock.push_back(temp);
				}
			}
		}
		{
			const char* ScoreId = element->Attribute("ScoreId");
			std::vector<std::string> vecScoreId;
			boost::split(vecScoreId, ScoreId, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecScoreId.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecScoreId[i].c_str(), &temp))
				{
					data.mScoreId.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mRoomID) != mMapData.end())std::cout <<"data refind:" << data.mRoomID << std::endl;
		assert(mMapData.find(data.mRoomID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mRoomID, data));
		element = element->NextSiblingElement();
	}
}

HappySupremacy_RoomStockCFG* HappySupremacy_RoomStockCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new HappySupremacy_RoomStockCFG());
	}
	return msSingleton.get();
}
