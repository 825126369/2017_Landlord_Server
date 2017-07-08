#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "ShowHand_RobAICFG.h"
std::auto_ptr<ShowHand_RobAICFG> ShowHand_RobAICFG::msSingleton(nullptr);

int ShowHand_RobAICFG::GetCount()
{
	return (int)mMapData.size();
}

const ShowHand_RobAICFGData* ShowHand_RobAICFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, ShowHand_RobAICFGData>& ShowHand_RobAICFG::GetMapData()
{
	return mMapData;
}

void ShowHand_RobAICFG::Reload()
{
	mMapData.clear();
	Load();
}

void ShowHand_RobAICFG::Load()
{
	std::ifstream readStream("../Config/ShowHand_RobAICFG.xml", std::ios::binary);
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
		ShowHand_RobAICFGData data;
		data.mID = element->IntAttribute("ID");
		data.mBetName = element->Attribute("BetName");
		{
			const char* BetRate = element->Attribute("BetRate");
			std::vector<std::string> vecBetRate;
			boost::split(vecBetRate, BetRate, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecBetRate.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecBetRate[i].c_str(), &temp))
				{
					data.mBetRate.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

ShowHand_RobAICFG* ShowHand_RobAICFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new ShowHand_RobAICFG());
	}
	return msSingleton.get();
}
