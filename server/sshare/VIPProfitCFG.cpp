#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "VIPProfitCFG.h"
std::auto_ptr<VIPProfitCFG> VIPProfitCFG::msSingleton(nullptr);

int VIPProfitCFG::GetCount()
{
	return (int)mMapData.size();
}

const VIPProfitCFGData* VIPProfitCFG::GetData(int VipLv)
{
	auto it = mMapData.find(VipLv);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, VIPProfitCFGData>& VIPProfitCFG::GetMapData()
{
	return mMapData;
}

void VIPProfitCFG::Reload()
{
	mMapData.clear();
	Load();
}

void VIPProfitCFG::Load()
{
	std::ifstream readStream("../Config/VIPProfitCFG.xml", std::ios::binary);
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
		VIPProfitCFGData data;
		data.mVipLv = element->IntAttribute("VipLv");
		data.mVipExp = element->IntAttribute("VipExp");
		data.mOnlineReward = element->IntAttribute("OnlineReward");
		if (mMapData.find(data.mVipLv) != mMapData.end())std::cout <<"data refind:" << data.mVipLv << std::endl;
		assert(mMapData.find(data.mVipLv) == mMapData.end());
		mMapData.insert(std::make_pair(data.mVipLv, data));
		element = element->NextSiblingElement();
	}
}

VIPProfitCFG* VIPProfitCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new VIPProfitCFG());
	}
	return msSingleton.get();
}
