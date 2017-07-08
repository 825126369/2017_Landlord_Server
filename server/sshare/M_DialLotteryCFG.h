#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_DialLotteryCFGData
{
	//编号
	int mID;
	//奖励金币
	int mRewardCoin;
	//奖励类型(1:金币    2:礼券)
	int mRewardType;
	//图标缩放比例
	float mScale;
	//物品ICON
	std::string mIcon;
	//获奖概率
	int mProbability;
};

class M_DialLotteryCFG
{
public:
private:
	static std::auto_ptr<M_DialLotteryCFG> msSingleton;
public:
	int GetCount();
	const M_DialLotteryCFGData* GetData(int ID);
	boost::unordered_map<int, M_DialLotteryCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_DialLotteryCFG* GetSingleton();
private:
	boost::unordered_map<int, M_DialLotteryCFGData> mMapData;
};
