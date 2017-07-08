#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "LandlordNetHuman_RoomCFG.h"
std::auto_ptr<LandlordNetHuman_RoomCFG> LandlordNetHuman_RoomCFG::msSingleton(nullptr);

int LandlordNetHuman_RoomCFG::GetCount()
{
	return (int)mMapData.size();
}

const LandlordNetHuman_RoomCFGData* LandlordNetHuman_RoomCFG::GetData(int RoomID)
{
	auto it = mMapData.find(RoomID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, LandlordNetHuman_RoomCFGData>& LandlordNetHuman_RoomCFG::GetMapData()
{
	return mMapData;
}

void LandlordNetHuman_RoomCFG::Reload()
{
	mMapData.clear();
	Load();
}

void LandlordNetHuman_RoomCFG::Load()
{
	std::ifstream readStream("../Config/LandlordNetHuman_RoomCFG.xml", std::ios::binary);
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
		LandlordNetHuman_RoomCFGData data;
		data.mRoomID = element->IntAttribute("RoomID");
		data.mRoomNamePrefix = element->Attribute("RoomNamePrefix");
		data.mGoldCondition = element->IntAttribute("GoldCondition");
		data.morShuffle = element->BoolAttribute("orShuffle");
		data.mdeskCount = element->IntAttribute("deskCount");
		data.mScore = element->IntAttribute("Score");
		if (mMapData.find(data.mRoomID) != mMapData.end())std::cout <<"data refind:" << data.mRoomID << std::endl;
		assert(mMapData.find(data.mRoomID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mRoomID, data));
		element = element->NextSiblingElement();
	}
}

LandlordNetHuman_RoomCFG* LandlordNetHuman_RoomCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new LandlordNetHuman_RoomCFG());
	}
	return msSingleton.get();
}
