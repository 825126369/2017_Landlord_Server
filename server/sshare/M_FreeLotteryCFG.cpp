#include "stdafx.h"
#include <cassert>
#include <fstream>
#include <iostream>
#include <iostream>
#include <boost/smart_ptr.hpp>
#include <boost/algorithm/string.hpp>
#include "tinyxml2.h"
#include "M_FreeLotteryCFG.h"
std::auto_ptr<M_FreeLotteryCFG> M_FreeLotteryCFG::msSingleton(nullptr);

int M_FreeLotteryCFG::GetCount()
{
	return (int)mMapData.size();
}

const M_FreeLotteryCFGData* M_FreeLotteryCFG::GetData(int ID)
{
	auto it = mMapData.find(ID);
	if (it != mMapData.end())
	{
		return &it->second;
	}
	return NULL;
}

boost::unordered_map<int, M_FreeLotteryCFGData>& M_FreeLotteryCFG::GetMapData()
{
	return mMapData;
}

void M_FreeLotteryCFG::Reload()
{
	mMapData.clear();
	Load();
}

void M_FreeLotteryCFG::Reload(const std::string& path)
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
		M_FreeLotteryCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mInfo = element->Attribute("Info");
		data.mBaseGold = element->IntAttribute("BaseGold");
		data.mAwardIcon = element->Attribute("AwardIcon");
		data.mAwardTitle = element->Attribute("AwardTitle");
		{
			const char* AwardItem1 = element->Attribute("AwardItem1");
			std::vector<std::string> vecAwardItem1;
			boost::split(vecAwardItem1, AwardItem1, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem1.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem1[i].c_str(), &temp))
				{
					data.mAwardItem1.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem2 = element->Attribute("AwardItem2");
			std::vector<std::string> vecAwardItem2;
			boost::split(vecAwardItem2, AwardItem2, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem2.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem2[i].c_str(), &temp))
				{
					data.mAwardItem2.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem3 = element->Attribute("AwardItem3");
			std::vector<std::string> vecAwardItem3;
			boost::split(vecAwardItem3, AwardItem3, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem3.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem3[i].c_str(), &temp))
				{
					data.mAwardItem3.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem4 = element->Attribute("AwardItem4");
			std::vector<std::string> vecAwardItem4;
			boost::split(vecAwardItem4, AwardItem4, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem4.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem4[i].c_str(), &temp))
				{
					data.mAwardItem4.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem5 = element->Attribute("AwardItem5");
			std::vector<std::string> vecAwardItem5;
			boost::split(vecAwardItem5, AwardItem5, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem5.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem5[i].c_str(), &temp))
				{
					data.mAwardItem5.push_back(temp);
				}
			}
		}
		mMapData[data.mID] = data;
		element = element->NextSiblingElement();
	}
}

void M_FreeLotteryCFG::Load(const std::string& path)
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
		M_FreeLotteryCFGData data;
		data.mID = element->IntAttribute("ID");
		data.mInfo = element->Attribute("Info");
		data.mBaseGold = element->IntAttribute("BaseGold");
		data.mAwardIcon = element->Attribute("AwardIcon");
		data.mAwardTitle = element->Attribute("AwardTitle");
		{
			const char* AwardItem1 = element->Attribute("AwardItem1");
			std::vector<std::string> vecAwardItem1;
			boost::split(vecAwardItem1, AwardItem1, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem1.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem1[i].c_str(), &temp))
				{
					data.mAwardItem1.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem2 = element->Attribute("AwardItem2");
			std::vector<std::string> vecAwardItem2;
			boost::split(vecAwardItem2, AwardItem2, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem2.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem2[i].c_str(), &temp))
				{
					data.mAwardItem2.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem3 = element->Attribute("AwardItem3");
			std::vector<std::string> vecAwardItem3;
			boost::split(vecAwardItem3, AwardItem3, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem3.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem3[i].c_str(), &temp))
				{
					data.mAwardItem3.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem4 = element->Attribute("AwardItem4");
			std::vector<std::string> vecAwardItem4;
			boost::split(vecAwardItem4, AwardItem4, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem4.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem4[i].c_str(), &temp))
				{
					data.mAwardItem4.push_back(temp);
				}
			}
		}
		{
			const char* AwardItem5 = element->Attribute("AwardItem5");
			std::vector<std::string> vecAwardItem5;
			boost::split(vecAwardItem5, AwardItem5, boost::is_any_of(","));
			int temp;
			for (unsigned int i = 0; i < vecAwardItem5.size(); i++)
			{
				if (tinyxml2::XMLUtil::ToInt(vecAwardItem5[i].c_str(), &temp))
				{
					data.mAwardItem5.push_back(temp);
				}
			}
		}
		if (mMapData.find(data.mID) != mMapData.end())std::cout <<"data refind:" << data.mID << std::endl;
		assert(mMapData.find(data.mID) == mMapData.end());
		mMapData.insert(std::make_pair(data.mID, data));
		element = element->NextSiblingElement();
	}
}

void M_FreeLotteryCFG::Load()
{
	Load("../Config/M_FreeLotteryCFG.xml");
}

M_FreeLotteryCFG* M_FreeLotteryCFG::GetSingleton()
{
	if (msSingleton.get() == nullptr)
	{
		msSingleton.reset(new M_FreeLotteryCFG());
	}
	return msSingleton.get();
}
