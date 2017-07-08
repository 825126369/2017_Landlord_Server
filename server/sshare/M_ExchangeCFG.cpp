#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_ExchangeCFG.h"
std::auto_ptr<M_ExchangeCFG> M_ExchangeCFG::msSingleton(nullptr);

int M_ExchangeCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_ExchangeCFGData* M_ExchangeCFG::GetData(int ChangeId)
{
	auto it = mMapData.find(ChangeId);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_ExchangeCFGData>& M_ExchangeCFG::GetMapData()
{
	return mMapData;
}

void M_ExchangeCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_ExchangeCFG::Reload(const std::string& path)
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
		M_ExchangeCFGData data;
		data.mChangeId = element->IntAttribute("ChangeId");
		data.mCostTicket = element->IntAttribute("CostTicket");
		data.mItemId = element->IntAttribute("ItemId");
		data.mVip = element->IntAttribute("Vip");
		data.mItemIcon = element->Attribute("ItemIcon");
		data.mItemName = element->Attribute("ItemName");
		data.mItemCount = element->IntAttribute("ItemCount");
		mMapData[data.mChangeId] = data;
		element = element->NextSiblingElement();
	}
}

void M_ExchangeCFG::Load(const std::string& path)
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
		M_ExchangeCFGData data;
		data.mChangeId = element->IntAttribute("ChangeId");
		data.mCostTicket = element->IntAttribute("CostTicket");
		data.mItemId = element->IntAttribute("ItemId");
		data.mVip = element->IntAttribute("Vip");
		data.mItemIcon = element->Attribute("ItemIcon");
		data.mItemName = element->Attribute("ItemName");
		data.mItemCount = element->IntAttribute("ItemCount");
		if (mMapData.find(data.mChangeId) != mMapData.end())std::cout <<"data refind:" << data.mChangeId << std::endl;
		assert(mMapData.find(data.mChangeId) == mMapData.end());
		mMapData.insert(std::make_pair(data.mChangeId, data));
		element = element->NextSiblingElement();
	}
}

void M_ExchangeCFG::Load()
{
	Load("../Config/M_ExchangeCFG.xml");
}

M_ExchangeCFG* M_ExchangeCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_ExchangeCFG());
	}
	return msSingleton.get();
}
