#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct GameCFGData
{
	//key
	int mID;
	//游戏名字
	std::string mGameName;
	//游戏路径
	std::string mGamePrefix;
	//更新地址
	std::string mUpdateUrl;
};

class GameCFG
{
public:
private:
	static std::auto_ptr<GameCFG> msSingleton;
public:
	int GetCount();
	const GameCFGData* GetData(int ID);
	boost::unordered_map<int, GameCFGData>& GetMapData();
	void Reload();
	void Load();
	static GameCFG* GetSingleton();
private:
	boost::unordered_map<int, GameCFGData> mMapData;
};
