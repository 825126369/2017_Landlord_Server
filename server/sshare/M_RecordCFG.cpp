#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_RecordCFG.h"
std::auto_ptr<M_RecordCFG> M_RecordCFG::msSingleton(nullptr);

int M_RecordCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_RecordCFGData* M_RecordCFG::GetData(int RecordID)
{
	auto it = mMapData.find(RecordID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_RecordCFGData>& M_RecordCFG::GetMapData()
{
	return mMapData;
}

void M_RecordCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_RecordCFG::Reload(const std::string& path)
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
		M_RecordCFGData data;
		data.mRecordID = element->IntAttribute("RecordID");
		data.mRecordName = element->Attribute("RecordName");
		{
			const char* RecordInfoList = element->Attribute("RecordInfoList");
			std::vector<std::string> vecRecordInfoList;
			boost::split(vecRecordInfoList, RecordInfoList, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecRecordInfoList.size(); i++)
			{
				data.mRecordInfoList.push_back(vecRecordInfoList[i]);
			}
		}
		mMapData[data.mRecordID] = data;
		element = element->NextSiblingElement();
	}
}

void M_RecordCFG::Load(const std::string& path)
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
		M_RecordCFGData data;
		data.mRecordID = element->IntAttribute("RecordID");
		data.mRecordName = element->Attribute("RecordName");
		{
			const char* RecordInfoList = element->Attribute("RecordInfoList");
			std::vector<std::string> vecRecordInfoList;
			boost::split(vecRecordInfoList, RecordInfoList, boost::is_any_of(","));
			for (unsigned int i = 0; i < vecRecordInfoList.size(); i++)
			{
				data.mRecordInfoList.push_back(vecRecordInfoList[i]);
			}
		}
		if (mMapData.find(data.mRecordID) != mMapData.end())std::cout <<"data refind:" << data.mRecordID << std::endl;
		assert(mMapData.find(data.mRecordID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mRecordID, data));
		element = element->NextSiblingElement();
	}
}

void M_RecordCFG::Load()
{
	Load("../Config/M_RecordCFG.xml");
}

M_RecordCFG* M_RecordCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_RecordCFG());
	}
	return msSingleton.get();
}
