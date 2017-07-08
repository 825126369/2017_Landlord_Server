#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ShowHand_BaseCFGData
{
	//key
	std::string mKey;
	//ÊýÖµ
	int mValue;
};

class ShowHand_BaseCFG
{
public:
private:
	static std::auto_ptr<ShowHand_BaseCFG> msSingleton;
public:
	int GetCount();
	const ShowHand_BaseCFGData* GetData(std::string Key);
	boost::unordered_map<std::string, ShowHand_BaseCFGData>& GetMapData();
	void Reload();
	void Load();
	static ShowHand_BaseCFG* GetSingleton();
private:
	boost::unordered_map<std::string, ShowHand_BaseCFGData> mMapData;
};
