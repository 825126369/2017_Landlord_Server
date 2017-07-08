#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "SensitiveWordCFG.h"
std::auto_ptr<SensitiveWordCFG> SensitiveWordCFG::msSingleton(nullptr);

int SensitiveWordCFG::GetCount()
{
	return (int)mMapData.size();
}

const SensitiveWordCFGData* SensitiveWordCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, SensitiveWordCFGData>& SensitiveWordCFG::GetMapData()
{
	return mMapData;
}

void SensitiveWordCFG::Reload()
{
	mMapData.clear();
	Load();
}

void SensitiveWordCFG::Reload(const std::string& path)
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
		SensitiveWordCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mSensitiveWord = element->Attribute("SensitiveWord");
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void SensitiveWordCFG::Load(const std::string& path)
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
		SensitiveWordCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mSensitiveWord = element->Attribute("SensitiveWord");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void SensitiveWordCFG::Load()
{
	Load("../Config/SensitiveWordCFG.xml");
}

SensitiveWordCFG* SensitiveWordCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new SensitiveWordCFG());
	}
	return msSingleton.get();
}
