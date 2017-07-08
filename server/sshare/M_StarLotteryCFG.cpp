#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_StarLotteryCFG.h"
std::auto_ptr<M_StarLotteryCFG> M_StarLotteryCFG::msSingleton(nullptr);

int M_StarLotteryCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_StarLotteryCFGData* M_StarLotteryCFG::GetData(int Index)
{
	auto it = mMapData.find(Index);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_StarLotteryCFGData>& M_StarLotteryCFG::GetMapData()
{
	return mMapData;
}

void M_StarLotteryCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_StarLotteryCFG::Reload(const std::string& path)
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
		M_StarLotteryCFGData data;
		data.mIndex = element->IntAttribute("Index");
		data.mStarLvl = element->IntAttribute("StarLvl");
		data.mItemType = element->IntAttribute("ItemType");
		data.mItemCount = element->IntAttribute("ItemCount");
		data.mRate1 = element->IntAttribute("Rate1");
		data.mRate2 = element->IntAttribute("Rate2");
		data.mRate3 = element->IntAttribute("Rate3");
		data.mRate4 = element->IntAttribute("Rate4");
		mMapData[data.mIndex] = data;
		element = element->NextSiblingElement();
	}
}

void M_StarLotteryCFG::Load(const std::string& path)
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
		M_StarLotteryCFGData data;
		data.mIndex = element->IntAttribute("Index");
		data.mStarLvl = element->IntAttribute("StarLvl");
		data.mItemType = element->IntAttribute("ItemType");
		data.mItemCount = element->IntAttribute("ItemCount");
		data.mRate1 = element->IntAttribute("Rate1");
		data.mRate2 = element->IntAttribute("Rate2");
		data.mRate3 = element->IntAttribute("Rate3");
		data.mRate4 = element->IntAttribute("Rate4");
		if (mMapData.find(data.mIndex) != mMapData.end())std::cout <<"data refind:" << data.mIndex << std::endl;
		assert(mMapData.find(data.mIndex) == mMapData.end());
		mMapData.insert(std::make_pair(data.mIndex, data));
		element = element->NextSiblingElement();
	}
}

void M_StarLotteryCFG::Load()
{
	Load("../Config/M_StarLotteryCFG.xml");
}

M_StarLotteryCFG* M_StarLotteryCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_StarLotteryCFG());
	}
	return msSingleton.get();
}
