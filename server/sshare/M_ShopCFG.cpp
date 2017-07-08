#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_ShopCFG.h"
std::auto_ptr<M_ShopCFG> M_ShopCFG::msSingleton(nullptr);

int M_ShopCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_ShopCFGData* M_ShopCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_ShopCFGData>& M_ShopCFG::GetMapData()
{
	return mMapData;
}

void M_ShopCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_ShopCFG::Reload(const std::string& path)
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
		M_ShopCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mName = element->Attribute("Name");
		data.mDesc = element->Attribute("Desc");
		data.mNameIcon = element->Attribute("NameIcon");
		data.mIcon = element->Attribute("Icon");
		data.mType = element->IntAttribute("Type");
		data.mPrice = element->IntAttribute("Price");
		data.mGold = element->IntAttribute("Gold");
		data.mIndex = element->IntAttribute("Index");
		data.mShopType = element->IntAttribute("ShopType");
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_ShopCFG::Load(const std::string& path)
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
		M_ShopCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mName = element->Attribute("Name");
		data.mDesc = element->Attribute("Desc");
		data.mNameIcon = element->Attribute("NameIcon");
		data.mIcon = element->Attribute("Icon");
		data.mType = element->IntAttribute("Type");
		data.mPrice = element->IntAttribute("Price");
		data.mGold = element->IntAttribute("Gold");
		data.mIndex = element->IntAttribute("Index");
		data.mShopType = element->IntAttribute("ShopType");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_ShopCFG::Load()
{
	Load("../Config/M_ShopCFG.xml");
}

M_ShopCFG* M_ShopCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_ShopCFG());
	}
	return msSingleton.get();
}
