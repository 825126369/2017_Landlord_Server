#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_ActivityMultiLanguageCFGData
{
	//Key
	std::string mID;
	//ÖÐÎÄÃû×Ö
	std::string mName;
};

class M_ActivityMultiLanguageCFG
{
public:
private:
	static std::auto_ptr<M_ActivityMultiLanguageCFG> msSingleton;
public:
	int GetCount();
	const M_ActivityMultiLanguageCFGData* GetData(std::string ID);
	boost::unordered_map<std::string, M_ActivityMultiLanguageCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_ActivityMultiLanguageCFG* GetSingleton();
private:
	boost::unordered_map<std::string, M_ActivityMultiLanguageCFGData> mMapData;
};
