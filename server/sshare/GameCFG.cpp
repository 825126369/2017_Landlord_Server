#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "GameCFG.h"
std::auto_ptr<GameCFG> GameCFG::msSingleton(nullptr);

int GameCFG::GetCount()
{
	return (int)mMapData.size();
}

const GameCFGData* GameCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, GameCFGData>& GameCFG::GetMapData()
{
	return mMapData;
}

void GameCFG::Reload()
{
	mMapData.clear();
	Load();
}

void GameCFG::Load()
{
	std::ifstream readStream("../Config/GameCFG.xml", std::ios::binary);
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
		GameCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mGameName = element->Attribute("GameName");
		data.mGamePrefix = element->Attribute("GamePrefix");
		data.mUpdateUrl = element->Attribute("UpdateUrl");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

GameCFG* GameCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new GameCFG());
	}
	return msSingleton.get();
}
