#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "HappySupremacy_RateCFG.h"
std::auto_ptr<HappySupremacy_RateCFG> HappySupremacy_RateCFG::msSingleton(nullptr);

int HappySupremacy_RateCFG::GetCount()
{
	return (int)mMapData.size();
}

const HappySupremacy_RateCFGData* HappySupremacy_RateCFG::GetData(int Key)
{
	auto it = mMapData.find(Key);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, HappySupremacy_RateCFGData>& HappySupremacy_RateCFG::GetMapData()
{
	return mMapData;
}

void HappySupremacy_RateCFG::Reload()
{
	mMapData.clear();
	Load();
}

void HappySupremacy_RateCFG::Load()
{
	std::ifstream readStream("../Config/HappySupremacy_RateCFG.xml", std::ios::binary);
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
		HappySupremacy_RateCFGData data;
		data.mKey = element->IntAttribute("Key");
		data.mName = element->Attribute("Name");
		data.mRate1 = element->IntAttribute("Rate1");
		data.mRate2 = element->IntAttribute("Rate2");
		if (mMapData.find(data.mKey) != mMapData.end())std::cout <<"data refind:" << data.mKey << std::endl;
		assert(mMapData.find(data.mKey) == mMapData.end());
		mMapData.insert(std::make_pair(data.mKey, data));
		element = element->NextSiblingElement();
	}
}

HappySupremacy_RateCFG* HappySupremacy_RateCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new HappySupremacy_RateCFG());
	}
	return msSingleton.get();
}
