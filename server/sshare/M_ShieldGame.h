#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_ShieldGameData
{
	//渠道ID
	int mChannelID;
	//屏蔽类型(1为只屏蔽2为只开放)
	int mType;
	//渠道屏蔽\开放游戏列表
	std::vector<int> mShieldGameList;
	//显示成就类型
	std::vector<int> mShowAchieveList;
};

class M_ShieldGame
{
public:
private:
	static std::auto_ptr<M_ShieldGame> msSingleton;
public:
	int GetCount();
	const M_ShieldGameData* GetData(int ChannelID);
	boost::unordered_map<int, M_ShieldGameData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_ShieldGame* GetSingleton();
private:
	boost::unordered_map<int, M_ShieldGameData> mMapData;
};
