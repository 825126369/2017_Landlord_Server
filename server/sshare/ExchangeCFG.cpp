#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "ExchangeCFG.h"
std::auto_ptr<ExchangeCFG> ExchangeCFG::msSingleton(nullptr);

int ExchangeCFG::GetCount()
{
	return (int)mMapData.size();
}

const ExchangeCFGData* ExchangeCFG::GetData(int ChangeId)
{
	auto it = mMapData.find(ChangeId);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, ExchangeCFGData>& ExchangeCFG::GetMapData()
{
	return mMapData;
}

void ExchangeCFG::Reload()
{
	mMapData.clear();
	Load();
}

void ExchangeCFG::Load()
{
	std::ifstream readStream("../Config/ExchangeCFG.xml", std::ios::binary);
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
		ExchangeCFGData data;
		data.mChangeId = element->IntAttribute("ChangeId");
		data.mCostTicket = element->IntAttribute("CostTicket");
		data.mItemId = element->IntAttribute("ItemId");
		if (mMapData.find(data.mChangeId) != mMapData.end())std::cout <<"data refind:" << data.mChangeId << std::endl;
		assert(mMapData.find(data.mChangeId) == mMapData.end());
		mMapData.insert(std::make_pair(data.mChangeId, data));
		element = element->NextSiblingElement();
	}
}

ExchangeCFG* ExchangeCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new ExchangeCFG());
	}
	return msSingleton.get();
}
