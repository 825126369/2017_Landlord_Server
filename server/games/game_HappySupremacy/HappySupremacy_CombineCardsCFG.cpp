#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "HappySupremacy_CombineCardsCFG.h"
std::auto_ptr<HappySupremacy_CombineCardsCFG> HappySupremacy_CombineCardsCFG::msSingleton(nullptr);

int HappySupremacy_CombineCardsCFG::GetCount()
{
	return (int)mMapData.size();
}

const HappySupremacy_CombineCardsCFGData* HappySupremacy_CombineCardsCFG::GetData(int CombineCardID)
{
	auto it = mMapData.find(CombineCardID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, HappySupremacy_CombineCardsCFGData>& HappySupremacy_CombineCardsCFG::GetMapData()
{
	return mMapData;
}

void HappySupremacy_CombineCardsCFG::Reload()
{
	mMapData.clear();
	Load();
}

void HappySupremacy_CombineCardsCFG::Load()
{
	std::ifstream readStream("../Config/HappySupremacy_CombineCardsCFG.xml", std::ios::binary);
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
		HappySupremacy_CombineCardsCFGData data;
		data.mCombineCardID = element->IntAttribute("CombineCardID");
		data.mCombineCardsName = element->Attribute("CombineCardsName");
		if (mMapData.find(data.mCombineCardID) != mMapData.end())std::cout <<"data refind:" << data.mCombineCardID << std::endl;
		assert(mMapData.find(data.mCombineCardID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mCombineCardID, data));
		element = element->NextSiblingElement();
	}
}

HappySupremacy_CombineCardsCFG* HappySupremacy_CombineCardsCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new HappySupremacy_CombineCardsCFG());
	}
	return msSingleton.get();
}
