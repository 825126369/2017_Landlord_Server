#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_RechangeExCFG.h"
std::auto_ptr<M_RechangeExCFG> M_RechangeExCFG::msSingleton(nullptr);

int M_RechangeExCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_RechangeExCFGData* M_RechangeExCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_RechangeExCFGData>& M_RechangeExCFG::GetMapData()
{
	return mMapData;
}

void M_RechangeExCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_RechangeExCFG::Reload(const std::string& path)
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
		M_RechangeExCFGData data;
		data.mID = element->IntAttribute("ID");
		{
			const char* PayCodes = element->Attribute("PayCodes");
			std::vector<std::string> vecPayCodes;
			boost::split(vecPayCodes, PayCodes, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecPayCodes.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecPayCodes[i].c_str(), &temp))
				{
					data.mPayCodes.push_back(temp);
				}
			}
		}
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_RechangeExCFG::Load(const std::string& path)
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
		M_RechangeExCFGData data;
		data.mID = element->IntAttribute("ID");
		{
			const char* PayCodes = element->Attribute("PayCodes");
			std::vector<std::string> vecPayCodes;
			boost::split(vecPayCodes, PayCodes, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecPayCodes.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecPayCodes[i].c_str(), &temp))
				{
					data.mPayCodes.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_RechangeExCFG::Load()
{
	Load("../Config/M_RechangeExCFG.xml");
}

M_RechangeExCFG* M_RechangeExCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_RechangeExCFG());
	}
	return msSingleton.get();
}
