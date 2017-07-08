#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_GiftCFG.h"
std::auto_ptr<M_GiftCFG> M_GiftCFG::msSingleton(nullptr);

int M_GiftCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_GiftCFGData* M_GiftCFG::GetData(int GiftId)
{
	auto it = mMapData.find(GiftId);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_GiftCFGData>& M_GiftCFG::GetMapData()
{
	return mMapData;
}

void M_GiftCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_GiftCFG::Reload(const std::string& path)
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
		M_GiftCFGData data;
		data.mGiftId = element->IntAttribute("GiftId");
		data.mCoin = element->IntAttribute("Coin");
		data.misShow = element->IntAttribute("isShow");
		data.mPercent = element->FloatAttribute("Percent");
		data.mVip = element->IntAttribute("Vip");
		data.mAction = element->Attribute("Action");
		data.mTextIcon = element->Attribute("TextIcon");
		mMapData[data.mGiftId] = data;
		element = element->NextSiblingElement();
	}
}

void M_GiftCFG::Load(const std::string& path)
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
		M_GiftCFGData data;
		data.mGiftId = element->IntAttribute("GiftId");
		data.mCoin = element->IntAttribute("Coin");
		data.misShow = element->IntAttribute("isShow");
		data.mPercent = element->FloatAttribute("Percent");
		data.mVip = element->IntAttribute("Vip");
		data.mAction = element->Attribute("Action");
		data.mTextIcon = element->Attribute("TextIcon");
		if (mMapData.find(data.mGiftId) != mMapData.end())std::cout <<"data refind:" << data.mGiftId << std::endl;
		assert(mMapData.find(data.mGiftId) == mMapData.end());
		mMapData.insert(std::make_pair(data.mGiftId, data));
		element = element->NextSiblingElement();
	}
}

void M_GiftCFG::Load()
{
	Load("../Config/M_GiftCFG.xml");
}

M_GiftCFG* M_GiftCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_GiftCFG());
	}
	return msSingleton.get();
}
