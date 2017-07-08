#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "DialLotteryCFG.h"
std::auto_ptr<DialLotteryCFG> DialLotteryCFG::msSingleton(nullptr);

int DialLotteryCFG::GetCount()
{
	return (int)mMapData.size();
}

const DialLotteryCFGData* DialLotteryCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, DialLotteryCFGData>& DialLotteryCFG::GetMapData()
{
	return mMapData;
}

void DialLotteryCFG::Reload()
{
	mMapData.clear();
	Load();
}

void DialLotteryCFG::Load()
{
	std::ifstream readStream("../Config/DialLotteryCFG.xml", std::ios::binary);
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
		DialLotteryCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mRewardCoin = element->IntAttribute("RewardCoin");
		data.mProbability = element->IntAttribute("Probability");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

DialLotteryCFG* DialLotteryCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new DialLotteryCFG());
	}
	return msSingleton.get();
}
