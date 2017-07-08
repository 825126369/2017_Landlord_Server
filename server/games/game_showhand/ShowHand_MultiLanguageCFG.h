#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ShowHand_MultiLanguageCFGData
{
	//Key
	std::string mID;
	//ÖÐÎÄÃû×Ö
	std::string mName;
};

class ShowHand_MultiLanguageCFG
{
public:
private:
	static std::auto_ptr<ShowHand_MultiLanguageCFG> msSingleton;
public:
	int GetCount();
	const ShowHand_MultiLanguageCFGData* GetData(std::string ID);
	boost::unordered_map<std::string, ShowHand_MultiLanguageCFGData>& GetMapData();
	void Reload();
	void Load();
	static ShowHand_MultiLanguageCFG* GetSingleton();
private:
	boost::unordered_map<std::string, ShowHand_MultiLanguageCFGData> mMapData;
};
