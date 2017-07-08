#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "ShowHand_SoundCFG.h"
std::auto_ptr<ShowHand_SoundCFG> ShowHand_SoundCFG::msSingleton(nullptr);

int ShowHand_SoundCFG::GetCount()
{
	return (int)mMapData.size();
}

const ShowHand_SoundCFGData* ShowHand_SoundCFG::GetData(int SoundID)
{
	auto it = mMapData.find(SoundID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, ShowHand_SoundCFGData>& ShowHand_SoundCFG::GetMapData()
{
	return mMapData;
}

void ShowHand_SoundCFG::Reload()
{
	mMapData.clear();
	Load();
}

void ShowHand_SoundCFG::Load()
{
	std::ifstream readStream("../Config/ShowHand_SoundCFG.xml", std::ios::binary);
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
		ShowHand_SoundCFGData data;
		data.mSoundID = element->IntAttribute("SoundID");
		data.mSoundName = element->Attribute("SoundName");
		data.mSoundPath = element->Attribute("SoundPath");
		data.mSoundTime = element->IntAttribute("SoundTime");
		if (mMapData.find(data.mSoundID) != mMapData.end())std::cout <<"data refind:" << data.mSoundID << std::endl;
		assert(mMapData.find(data.mSoundID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mSoundID, data));
		element = element->NextSiblingElement();
	}
}

ShowHand_SoundCFG* ShowHand_SoundCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new ShowHand_SoundCFG());
	}
	return msSingleton.get();
}
