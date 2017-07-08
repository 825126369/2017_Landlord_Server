#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_VIPProfitCFG.h"
std::auto_ptr<M_VIPProfitCFG> M_VIPProfitCFG::msSingleton(nullptr);

int M_VIPProfitCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_VIPProfitCFGData* M_VIPProfitCFG::GetData(int VipLv)
{
	auto it = mMapData.find(VipLv);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_VIPProfitCFGData>& M_VIPProfitCFG::GetMapData()
{
	return mMapData;
}

void M_VIPProfitCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_VIPProfitCFG::Reload(const std::string& path)
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
		M_VIPProfitCFGData data;
		data.mVipLv = element->IntAttribute("VipLv");
		data.mVipExp = element->IntAttribute("VipExp");
		data.mOnlineReward = element->IntAttribute("OnlineReward");
		data.mMaxGiftslimit = element->Int64Attribute("MaxGiftslimit");
		{
			const char* GiftUnlock = element->Attribute("GiftUnlock");
			std::vector<std::string> vecGiftUnlock;
			boost::split(vecGiftUnlock, GiftUnlock, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecGiftUnlock.size(); i++)
			{
				data.mGiftUnlock.push_back(vecGiftUnlock[i]);
			}
		}
		{
			const char* RoomUnlock = element->Attribute("RoomUnlock");
			std::vector<std::string> vecRoomUnlock;
			boost::split(vecRoomUnlock, RoomUnlock, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecRoomUnlock.size(); i++)
			{
				data.mRoomUnlock.push_back(vecRoomUnlock[i]);
			}
		}
		{
			const char* FishItemUnlock = element->Attribute("FishItemUnlock");
			std::vector<std::string> vecFishItemUnlock;
			boost::split(vecFishItemUnlock, FishItemUnlock, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecFishItemUnlock.size(); i++)
			{
				data.mFishItemUnlock.push_back(vecFishItemUnlock[i]);
			}
		}
		data.mVipName = element->IntAttribute("VipName");
		data.mGiveTicket = element->IntAttribute("GiveTicket");
		data.mDailyLottery = element->IntAttribute("DailyLottery");
		mMapData[data.mVipLv] = data;
		element = element->NextSiblingElement();
	}
}

void M_VIPProfitCFG::Load(const std::string& path)
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
		M_VIPProfitCFGData data;
		data.mVipLv = element->IntAttribute("VipLv");
		data.mVipExp = element->IntAttribute("VipExp");
		data.mOnlineReward = element->IntAttribute("OnlineReward");
		data.mMaxGiftslimit = element->Int64Attribute("MaxGiftslimit");
		{
			const char* GiftUnlock = element->Attribute("GiftUnlock");
			std::vector<std::string> vecGiftUnlock;
			boost::split(vecGiftUnlock, GiftUnlock, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecGiftUnlock.size(); i++)
			{
				data.mGiftUnlock.push_back(vecGiftUnlock[i]);
			}
		}
		{
			const char* RoomUnlock = element->Attribute("RoomUnlock");
			std::vector<std::string> vecRoomUnlock;
			boost::split(vecRoomUnlock, RoomUnlock, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecRoomUnlock.size(); i++)
			{
				data.mRoomUnlock.push_back(vecRoomUnlock[i]);
			}
		}
		{
			const char* FishItemUnlock = element->Attribute("FishItemUnlock");
			std::vector<std::string> vecFishItemUnlock;
			boost::split(vecFishItemUnlock, FishItemUnlock, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecFishItemUnlock.size(); i++)
			{
				data.mFishItemUnlock.push_back(vecFishItemUnlock[i]);
			}
		}
		data.mVipName = element->IntAttribute("VipName");
		data.mGiveTicket = element->IntAttribute("GiveTicket");
		data.mDailyLottery = element->IntAttribute("DailyLottery");
		if (mMapData.find(data.mVipLv) != mMapData.end())std::cout <<"data refind:" << data.mVipLv << std::endl;
		assert(mMapData.find(data.mVipLv) == mMapData.end());
		mMapData.insert(std::make_pair(data.mVipLv, data));
		element = element->NextSiblingElement();
	}
}

void M_VIPProfitCFG::Load()
{
	Load("../Config/M_VIPProfitCFG.xml");
}

M_VIPProfitCFG* M_VIPProfitCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_VIPProfitCFG());
	}
	return msSingleton.get();
}
