#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_BaseInfoCFGData
{
	//Key
	std::string mKey;
	//ÊýÖµ
	int mValue;
};

class M_BaseInfoCFG
{
public:
private:
	static std::auto_ptr<M_BaseInfoCFG> msSingleton;
public:
	int GetCount();
	const M_BaseInfoCFGData* GetData(std::string Key);
	boost::unordered_map<std::string, M_BaseInfoCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_BaseInfoCFG* GetSingleton();
private:
	boost::unordered_map<std::string, M_BaseInfoCFGData> mMapData;
};
