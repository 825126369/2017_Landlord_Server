#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_ShieldGame.h"
std::auto_ptr<M_ShieldGame> M_ShieldGame::msSingleton(nullptr);

int M_ShieldGame::GetCount()
{
	return (int)mMapData.size();
}

const M_ShieldGameData* M_ShieldGame::GetData(int ChannelID)
{
	auto it = mMapData.find(ChannelID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_ShieldGameData>& M_ShieldGame::GetMapData()
{
	return mMapData;
}

void M_ShieldGame::Reload()
{
	mMapData.clear();
	Load();
}

void M_ShieldGame::Reload(const std::string& path)
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
		M_ShieldGameData data;
		data.mChannelID = element->IntAttribute("ChannelID");
		data.mType = element->IntAttribute("Type");
		{
			const char* ShieldGameList = element->Attribute("ShieldGameList");
			std::vector<std::string> vecShieldGameList;
			boost::split(vecShieldGameList, ShieldGameList, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecShieldGameList.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecShieldGameList[i].c_str(), &temp))
				{
					data.mShieldGameList.push_back(temp);
				}
			}
		}
		{
			const char* ShowAchieveList = element->Attribute("ShowAchieveList");
			std::vector<std::string> vecShowAchieveList;
			boost::split(vecShowAchieveList, ShowAchieveList, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecShowAchieveList.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecShowAchieveList[i].c_str(), &temp))
				{
					data.mShowAchieveList.push_back(temp);
				}
			}
		}
		mMapData[data.mChannelID] = data;
		element = element->NextSiblingElement();
	}
}

void M_ShieldGame::Load(const std::string& path)
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
		M_ShieldGameData data;
		data.mChannelID = element->IntAttribute("ChannelID");
		data.mType = element->IntAttribute("Type");
		{
			const char* ShieldGameList = element->Attribute("ShieldGameList");
			std::vector<std::string> vecShieldGameList;
			boost::split(vecShieldGameList, ShieldGameList, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecShieldGameList.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecShieldGameList[i].c_str(), &temp))
				{
					data.mShieldGameList.push_back(temp);
				}
			}
		}
		{
			const char* ShowAchieveList = element->Attribute("ShowAchieveList");
			std::vector<std::string> vecShowAchieveList;
			boost::split(vecShowAchieveList, ShowAchieveList, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecShowAchieveList.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecShowAchieveList[i].c_str(), &temp))
				{
					data.mShowAchieveList.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mChannelID) != mMapData.end())std::cout <<"data refind:" << data.mChannelID << std::endl;
		assert(mMapData.find(data.mChannelID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mChannelID, data));
		element = element->NextSiblingElement();
	}
}

void M_ShieldGame::Load()
{
	Load("../Config/M_ShieldGame.xml");
}

M_ShieldGame* M_ShieldGame::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_ShieldGame());
	}
	return msSingleton.get();
}
