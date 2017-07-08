#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "HappySupremacy_BaseCFG.h"
std::auto_ptr<HappySupremacy_BaseCFG> HappySupremacy_BaseCFG::msSingleton(nullptr);

int HappySupremacy_BaseCFG::GetCount()
{
	return (int)mMapData.size();
}

const HappySupremacy_BaseCFGData* HappySupremacy_BaseCFG::GetData(std::string Key)
{
	auto it = mMapData.find(Key);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<std::string, HappySupremacy_BaseCFGData>& HappySupremacy_BaseCFG::GetMapData()
{
	return mMapData;
}

void HappySupremacy_BaseCFG::Reload()
{
	mMapData.clear();
	Load();
}

void HappySupremacy_BaseCFG::Load()
{
	std::ifstream readStream("../Config/HappySupremacy_BaseCFG.xml", std::ios::binary);
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
		HappySupremacy_BaseCFGData data;
		data.mKey = element->Attribute("Key");
		data.mValue = element->IntAttribute("Value");
		if (mMapData.find(data.mKey) != mMapData.end())std::cout <<"data refind:" << data.mKey << std::endl;
		assert(mMapData.find(data.mKey) == mMapData.end());
		mMapData.insert(std::make_pair(data.mKey, data));
		element = element->NextSiblingElement();
	}
}

HappySupremacy_BaseCFG* HappySupremacy_BaseCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new HappySupremacy_BaseCFG());
	}
	return msSingleton.get();
}
