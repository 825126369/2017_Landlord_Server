#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "ShowHand_RoomCFG.h"
std::auto_ptr<ShowHand_RoomCFG> ShowHand_RoomCFG::msSingleton(nullptr);

int ShowHand_RoomCFG::GetCount()
{
	return (int)mMapData.size();
}

const ShowHand_RoomCFGData* ShowHand_RoomCFG::GetData(int RoomId)
{
	auto it = mMapData.find(RoomId);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, ShowHand_RoomCFGData>& ShowHand_RoomCFG::GetMapData()
{
	return mMapData;
}

void ShowHand_RoomCFG::Reload()
{
	mMapData.clear();
	Load();
}

void ShowHand_RoomCFG::Load()
{
	std::ifstream readStream("../Config/ShowHand_RoomCFG.xml", std::ios::binary);
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
		ShowHand_RoomCFGData data;
		data.mRoomId = element->IntAttribute("RoomId");
		data.mRoomName = element->Attribute("RoomName");
		data.mRoomMaxPlayerCount = element->IntAttribute("RoomMaxPlayerCount");
		data.mTableMaxPlayerCount = element->IntAttribute("TableMaxPlayerCount");
		data.mIsOpen = element->BoolAttribute("IsOpen");
		data.mEnterGoldCondition = element->IntAttribute("EnterGoldCondition");
		data.mEnterTableGoldCondition = element->IntAttribute("EnterTableGoldCondition");
		data.mIllegalQuitGoldCount = element->IntAttribute("IllegalQuitGoldCount");
		data.mAnte = element->IntAttribute("Ante");
		data.mAndroidCount = element->IntAttribute("AndroidCount");
		data.mRobMinGold = element->IntAttribute("RobMinGold");
		data.mRobMaxGold = element->IntAttribute("RobMaxGold");
		data.mRobMinVip = element->IntAttribute("RobMinVip");
		data.mRobMaxVip = element->IntAttribute("RobMaxVip");
		data.mRoomAndroidYield = element->FloatAttribute("RoomAndroidYield");
		if (mMapData.find(data.mRoomId) != mMapData.end())std::cout <<"data refind:" << data.mRoomId << std::endl;
		assert(mMapData.find(data.mRoomId) == mMapData.end());
		mMapData.insert(std::make_pair(data.mRoomId, data));
		element = element->NextSiblingElement();
	}
}

ShowHand_RoomCFG* ShowHand_RoomCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new ShowHand_RoomCFG());
	}
	return msSingleton.get();
}
