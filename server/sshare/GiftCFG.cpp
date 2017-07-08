#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "GiftCFG.h"
std::auto_ptr<GiftCFG> GiftCFG::msSingleton(nullptr);

int GiftCFG::GetCount()
{
	return (int)mMapData.size();
}

const GiftCFGData* GiftCFG::GetData(int GiftId)
{
	auto it = mMapData.find(GiftId);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, GiftCFGData>& GiftCFG::GetMapData()
{
	return mMapData;
}

void GiftCFG::Reload()
{
	mMapData.clear();
	Load();
}

void GiftCFG::Load()
{
	std::ifstream readStream("../Config/GiftCFG.xml", std::ios::binary);
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
		GiftCFGData data;
		data.mGiftId = element->IntAttribute("GiftId");
		data.mCoin = element->IntAttribute("Coin");
		if (mMapData.find(data.mGiftId) != mMapData.end())std::cout <<"data refind:" << data.mGiftId << std::endl;
		assert(mMapData.find(data.mGiftId) == mMapData.end());
		mMapData.insert(std::make_pair(data.mGiftId, data));
		element = element->NextSiblingElement();
	}
}

GiftCFG* GiftCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new GiftCFG());
	}
	return msSingleton.get();
}
