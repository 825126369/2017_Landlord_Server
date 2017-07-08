#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_GameCFGData
{
	//key
	int mID;
	//游戏名字
	std::string mGameName;
	//是否开放
	bool mIsOpen;
};

class M_GameCFG
{
public:
private:
	static std::auto_ptr<M_GameCFG> msSingleton;
public:
	int GetCount();
	const M_GameCFGData* GetData(int ID);
	boost::unordered_map<int, M_GameCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_GameCFG* GetSingleton();
private:
	boost::unordered_map<int, M_GameCFGData> mMapData;
};
