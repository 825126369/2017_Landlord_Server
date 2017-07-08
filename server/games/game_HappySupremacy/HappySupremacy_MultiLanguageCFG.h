#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct HappySupremacy_MultiLanguageCFGData
{
	//Key
	std::string mID;
	//ÖÐÎÄÃû×Ö
	std::string mName;
};

class HappySupremacy_MultiLanguageCFG
{
public:
private:
	static std::auto_ptr<HappySupremacy_MultiLanguageCFG> msSingleton;
public:
	int GetCount();
	const HappySupremacy_MultiLanguageCFGData* GetData(std::string ID);
	boost::unordered_map<std::string, HappySupremacy_MultiLanguageCFGData>& GetMapData();
	void Reload();
	void Load();
	static HappySupremacy_MultiLanguageCFG* GetSingleton();
private:
	boost::unordered_map<std::string, HappySupremacy_MultiLanguageCFGData> mMapData;
};
