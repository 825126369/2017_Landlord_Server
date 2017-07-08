#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_ItemCFG.h"
std::auto_ptr<M_ItemCFG> M_ItemCFG::msSingleton(nullptr);

int M_ItemCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_ItemCFGData* M_ItemCFG::GetData(int ItemId)
{
	auto it = mMapData.find(ItemId);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_ItemCFGData>& M_ItemCFG::GetMapData()
{
	return mMapData;
}

void M_ItemCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_ItemCFG::Reload(const std::string& path)
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
		M_ItemCFGData data;
		data.mItemId = element->IntAttribute("ItemId");
		data.mItemName = element->Attribute("ItemName");
		data.mItemDesc = element->Attribute("ItemDesc");
		data.mIcon = element->Attribute("Icon");
		data.mItemCategory = element->IntAttribute("ItemCategory");
		data.mItemValue = element->IntAttribute("ItemValue");
		mMapData[data.mItemId] = data;
		element = element->NextSiblingElement();
	}
}

void M_ItemCFG::Load(const std::string& path)
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
		M_ItemCFGData data;
		data.mItemId = element->IntAttribute("ItemId");
		data.mItemName = element->Attribute("ItemName");
		data.mItemDesc = element->Attribute("ItemDesc");
		data.mIcon = element->Attribute("Icon");
		data.mItemCategory = element->IntAttribute("ItemCategory");
		data.mItemValue = element->IntAttribute("ItemValue");
		if (mMapData.find(data.mItemId) != mMapData.end())std::cout <<"data refind:" << data.mItemId << std::endl;
		assert(mMapData.find(data.mItemId) == mMapData.end());
		mMapData.insert(std::make_pair(data.mItemId, data));
		element = element->NextSiblingElement();
	}
}

void M_ItemCFG::Load()
{
	Load("../Config/M_ItemCFG.xml");
}

M_ItemCFG* M_ItemCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_ItemCFG());
	}
	return msSingleton.get();
}
