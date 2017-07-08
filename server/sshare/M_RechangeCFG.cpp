#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_RechangeCFG.h"
std::auto_ptr<M_RechangeCFG> M_RechangeCFG::msSingleton(nullptr);

int M_RechangeCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_RechangeCFGData* M_RechangeCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_RechangeCFGData>& M_RechangeCFG::GetMapData()
{
	return mMapData;
}

void M_RechangeCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_RechangeCFG::Reload(const std::string& path)
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
		M_RechangeCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mName = element->Attribute("Name");
		data.mDesc = element->Attribute("Desc");
		data.mNameIcon = element->Attribute("NameIcon");
		data.mIcon = element->Attribute("Icon");
		data.mPayType = element->IntAttribute("PayType");
		data.mType = element->IntAttribute("Type");
		data.mPrice = element->IntAttribute("Price");
		data.mGold = element->IntAttribute("Gold");
		data.mFirstGold = element->IntAttribute("FirstGold");
		data.mGiveGold = element->IntAttribute("GiveGold");
		data.mGiveTicket = element->IntAttribute("GiveTicket");
		data.mVIPExp = element->IntAttribute("VIPExp");
		data.mIndex = element->IntAttribute("Index");
		data.mShopType = element->IntAttribute("ShopType");
		data.mFlag = element->BoolAttribute("Flag");
		data.mAppStoreID = element->Attribute("AppStoreID");
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_RechangeCFG::Load(const std::string& path)
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
		M_RechangeCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mName = element->Attribute("Name");
		data.mDesc = element->Attribute("Desc");
		data.mNameIcon = element->Attribute("NameIcon");
		data.mIcon = element->Attribute("Icon");
		data.mPayType = element->IntAttribute("PayType");
		data.mType = element->IntAttribute("Type");
		data.mPrice = element->IntAttribute("Price");
		data.mGold = element->IntAttribute("Gold");
		data.mFirstGold = element->IntAttribute("FirstGold");
		data.mGiveGold = element->IntAttribute("GiveGold");
		data.mGiveTicket = element->IntAttribute("GiveTicket");
		data.mVIPExp = element->IntAttribute("VIPExp");
		data.mIndex = element->IntAttribute("Index");
		data.mShopType = element->IntAttribute("ShopType");
		data.mFlag = element->BoolAttribute("Flag");
		data.mAppStoreID = element->Attribute("AppStoreID");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_RechangeCFG::Load()
{
	Load("../Config/M_RechangeCFG.xml");
}

M_RechangeCFG* M_RechangeCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_RechangeCFG());
	}
	return msSingleton.get();
}
