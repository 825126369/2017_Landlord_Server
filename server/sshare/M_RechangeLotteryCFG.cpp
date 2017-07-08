#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_RechangeLotteryCFG.h"
std::auto_ptr<M_RechangeLotteryCFG> M_RechangeLotteryCFG::msSingleton(nullptr);

int M_RechangeLotteryCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_RechangeLotteryCFGData* M_RechangeLotteryCFG::GetData(int Much)
{
	auto it = mMapData.find(Much);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_RechangeLotteryCFGData>& M_RechangeLotteryCFG::GetMapData()
{
	return mMapData;
}

void M_RechangeLotteryCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_RechangeLotteryCFG::Reload(const std::string& path)
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
		M_RechangeLotteryCFGData data;
		data.mMuch = element->IntAttribute("Much");
		data.mFirstRate = element->IntAttribute("FirstRate");
		data.mDefaultRate = element->IntAttribute("DefaultRate");
		mMapData[data.mMuch] = data;
		element = element->NextSiblingElement();
	}
}

void M_RechangeLotteryCFG::Load(const std::string& path)
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
		M_RechangeLotteryCFGData data;
		data.mMuch = element->IntAttribute("Much");
		data.mFirstRate = element->IntAttribute("FirstRate");
		data.mDefaultRate = element->IntAttribute("DefaultRate");
		if (mMapData.find(data.mMuch) != mMapData.end())std::cout <<"data refind:" << data.mMuch << std::endl;
		assert(mMapData.find(data.mMuch) == mMapData.end());
		mMapData.insert(std::make_pair(data.mMuch, data));
		element = element->NextSiblingElement();
	}
}

void M_RechangeLotteryCFG::Load()
{
	Load("../Config/M_RechangeLotteryCFG.xml");
}

M_RechangeLotteryCFG* M_RechangeLotteryCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_RechangeLotteryCFG());
	}
	return msSingleton.get();
}
