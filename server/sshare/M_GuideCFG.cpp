#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_GuideCFG.h"
std::auto_ptr<M_GuideCFG> M_GuideCFG::msSingleton(nullptr);

int M_GuideCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_GuideCFGData* M_GuideCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_GuideCFGData>& M_GuideCFG::GetMapData()
{
	return mMapData;
}

void M_GuideCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_GuideCFG::Load()
{
	std::ifstream readStream("../Config/M_GuideCFG.xml", std::ios::binary);
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
		M_GuideCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mContent = element->Attribute("Content");
		data.mSendGold = element->IntAttribute("SendGold");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

M_GuideCFG* M_GuideCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_GuideCFG());
	}
	return msSingleton.get();
}
