#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_ChipCFG.h"
std::auto_ptr<M_ChipCFG> M_ChipCFG::msSingleton(nullptr);

int M_ChipCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_ChipCFGData* M_ChipCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_ChipCFGData>& M_ChipCFG::GetMapData()
{
	return mMapData;
}

void M_ChipCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_ChipCFG::Reload(const std::string& path)
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
		M_ChipCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mCount = element->IntAttribute("Count");
		data.mIcon = element->Attribute("Icon");
		data.mIsCheck = element->BoolAttribute("IsCheck");
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_ChipCFG::Load(const std::string& path)
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
		M_ChipCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mCount = element->IntAttribute("Count");
		data.mIcon = element->Attribute("Icon");
		data.mIsCheck = element->BoolAttribute("IsCheck");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_ChipCFG::Load()
{
	Load("../Config/M_ChipCFG.xml");
}

M_ChipCFG* M_ChipCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_ChipCFG());
	}
	return msSingleton.get();
}
