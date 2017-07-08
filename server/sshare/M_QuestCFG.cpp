#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_QuestCFG.h"
std::auto_ptr<M_QuestCFG> M_QuestCFG::msSingleton(nullptr);

int M_QuestCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_QuestCFGData* M_QuestCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_QuestCFGData>& M_QuestCFG::GetMapData()
{
	return mMapData;
}

void M_QuestCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_QuestCFG::Reload(const std::string& path)
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
		M_QuestCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mName = element->Attribute("Name");
		data.mDesc = element->Attribute("Desc");
		data.mIcon1 = element->Attribute("Icon1");
		data.mIcon2 = element->Attribute("Icon2");
		data.mFrame = element->Attribute("Frame");
		data.mDefault = element->BoolAttribute("Default");
		data.mClass = element->IntAttribute("Class");
		data.mType = element->IntAttribute("Type");
		data.mCompleteType = element->IntAttribute("CompleteType");
		data.mCompleteCount = element->IntAttribute("CompleteCount");
		data.mCompleteParam = element->IntAttribute("CompleteParam");
		data.mNextQuestID = element->IntAttribute("NextQuestID");
		data.mIsSaveCount = element->BoolAttribute("IsSaveCount");
		{
			const char* AwardItemIDs = element->Attribute("AwardItemIDs");
			std::vector<std::string> vecAwardItemIDs;
			boost::split(vecAwardItemIDs, AwardItemIDs, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItemIDs.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItemIDs[i].c_str(), &temp))
				{
					data.mAwardItemIDs.push_back(temp);
				}
			}
		}
		{
			const char* AwardItemCounts = element->Attribute("AwardItemCounts");
			std::vector<std::string> vecAwardItemCounts;
			boost::split(vecAwardItemCounts, AwardItemCounts, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItemCounts.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItemCounts[i].c_str(), &temp))
				{
					data.mAwardItemCounts.push_back(temp);
				}
			}
		}
		data.mIsSet = element->BoolAttribute("IsSet");
		data.mGoTo = element->IntAttribute("GoTo");
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_QuestCFG::Load(const std::string& path)
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
		M_QuestCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mName = element->Attribute("Name");
		data.mDesc = element->Attribute("Desc");
		data.mIcon1 = element->Attribute("Icon1");
		data.mIcon2 = element->Attribute("Icon2");
		data.mFrame = element->Attribute("Frame");
		data.mDefault = element->BoolAttribute("Default");
		data.mClass = element->IntAttribute("Class");
		data.mType = element->IntAttribute("Type");
		data.mCompleteType = element->IntAttribute("CompleteType");
		data.mCompleteCount = element->IntAttribute("CompleteCount");
		data.mCompleteParam = element->IntAttribute("CompleteParam");
		data.mNextQuestID = element->IntAttribute("NextQuestID");
		data.mIsSaveCount = element->BoolAttribute("IsSaveCount");
		{
			const char* AwardItemIDs = element->Attribute("AwardItemIDs");
			std::vector<std::string> vecAwardItemIDs;
			boost::split(vecAwardItemIDs, AwardItemIDs, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItemIDs.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItemIDs[i].c_str(), &temp))
				{
					data.mAwardItemIDs.push_back(temp);
				}
			}
		}
		{
			const char* AwardItemCounts = element->Attribute("AwardItemCounts");
			std::vector<std::string> vecAwardItemCounts;
			boost::split(vecAwardItemCounts, AwardItemCounts, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItemCounts.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItemCounts[i].c_str(), &temp))
				{
					data.mAwardItemCounts.push_back(temp);
				}
			}
		}
		data.mIsSet = element->BoolAttribute("IsSet");
		data.mGoTo = element->IntAttribute("GoTo");
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_QuestCFG::Load()
{
	Load("../Config/M_QuestCFG.xml");
}

M_QuestCFG* M_QuestCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_QuestCFG());
	}
	return msSingleton.get();
}
