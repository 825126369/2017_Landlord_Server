#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct HappySupremacy_BaseCFGData
{
	//KeyÖµ
	std::string mKey;
	//ÊýÖµ
	int mValue;
};

class HappySupremacy_BaseCFG
{
public:
private:
	static std::auto_ptr<HappySupremacy_BaseCFG> msSingleton;
public:
	int GetCount();
	const HappySupremacy_BaseCFGData* GetData(std::string Key);
	boost::unordered_map<std::string, HappySupremacy_BaseCFGData>& GetMapData();
	void Reload();
	void Load();
	static HappySupremacy_BaseCFG* GetSingleton();
private:
	boost::unordered_map<std::string, HappySupremacy_BaseCFGData> mMapData;
};
