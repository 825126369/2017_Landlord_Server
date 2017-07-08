#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_ThirdPluginCFG.h"
std::auto_ptr<M_ThirdPluginCFG> M_ThirdPluginCFG::msSingleton(nullptr);

int M_ThirdPluginCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_ThirdPluginCFGData* M_ThirdPluginCFG::GetData(std::string ChannelID)
{
	auto it = mMapData.find(ChannelID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<std::string, M_ThirdPluginCFGData>& M_ThirdPluginCFG::GetMapData()
{
	return mMapData;
}

void M_ThirdPluginCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_ThirdPluginCFG::Reload(const std::string& path)
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
		M_ThirdPluginCFGData data;
		data.mChannelID = element->Attribute("ChannelID");
		data.mPlatform = element->Attribute("Platform");
		data.mLuaFilePath = element->Attribute("LuaFilePath");
		data.mRemark = element->Attribute("Remark");
		mMapData[data.mChannelID] = data;
		element = element->NextSiblingElement();
	}
}

void M_ThirdPluginCFG::Load(const std::string& path)
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
		M_ThirdPluginCFGData data;
		data.mChannelID = element->Attribute("ChannelID");
		data.mPlatform = element->Attribute("Platform");
		data.mLuaFilePath = element->Attribute("LuaFilePath");
		data.mRemark = element->Attribute("Remark");
		if (mMapData.find(data.mChannelID) != mMapData.end())std::cout <<"data refind:" << data.mChannelID << std::endl;
		assert(mMapData.find(data.mChannelID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mChannelID, data));
		element = element->NextSiblingElement();
	}
}

void M_ThirdPluginCFG::Load()
{
	Load("../Config/M_ThirdPluginCFG.xml");
}

M_ThirdPluginCFG* M_ThirdPluginCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_ThirdPluginCFG());
	}
	return msSingleton.get();
}
