#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_FaceCFGData
{
	//ID
	int mID;
	//Ãû×Ö
	std::string mName;
	//Â·¾¶
	std::string mIcon;
};

class M_FaceCFG
{
public:
private:
	static std::auto_ptr<M_FaceCFG> msSingleton;
public:
	int GetCount();
	const M_FaceCFGData* GetData(int ID);
	boost::unordered_map<int, M_FaceCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_FaceCFG* GetSingleton();
private:
	boost::unordered_map<int, M_FaceCFGData> mMapData;
};
