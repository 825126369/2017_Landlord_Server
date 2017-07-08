#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "ShoaHand_BetMaxCFG.h"
std::auto_ptr<ShoaHand_BetMaxCFG> ShoaHand_BetMaxCFG::msSingleton(nullptr);

int ShoaHand_BetMaxCFG::GetCount()
{
	return (int)mMapData.size();
}

const ShoaHand_BetMaxCFGData* ShoaHand_BetMaxCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, ShoaHand_BetMaxCFGData>& ShoaHand_BetMaxCFG::GetMapData()
{
	return mMapData;
}

void ShoaHand_BetMaxCFG::Reload()
{
	mMapData.clear();
	Load();
}

void ShoaHand_BetMaxCFG::Load()
{
	std::ifstream readStream("../Config/ShoaHand_BetMaxCFG.xml", std::ios::binary);
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
		ShoaHand_BetMaxCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mGoldCount = element->IntAttribute("GoldCount");
		data.mBetMax = element->IntAttribute("BetMax");
		{
			const char* CanUseWeight = element->Attribute("CanUseWeight");
			std::vector<std::string> vecCanUseWeight;
			boost::split(vecCanUseWeight, CanUseWeight, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecCanUseWeight.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecCanUseWeight[i].c_str(), &temp))
				{
					data.mCanUseWeight.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

ShoaHand_BetMaxCFG* ShoaHand_BetMaxCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new ShoaHand_BetMaxCFG());
	}
	return msSingleton.get();
}
