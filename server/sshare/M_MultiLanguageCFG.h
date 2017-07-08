#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_MultiLanguageCFGData
{
	//Key
	std::string mID;
	//ÖÐÎÄÃû×Ö
	std::string mName;
};

class M_MultiLanguageCFG
{
public:
private:
	static std::auto_ptr<M_MultiLanguageCFG> msSingleton;
public:
	int GetCount();
	const M_MultiLanguageCFGData* GetData(std::string ID);
	boost::unordered_map<std::string, M_MultiLanguageCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_MultiLanguageCFG* GetSingleton();
private:
	boost::unordered_map<std::string, M_MultiLanguageCFGData> mMapData;
};
