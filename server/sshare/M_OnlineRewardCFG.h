#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_OnlineRewardCFGData
{
	//Key
	int mID;
	//开始时间,日期部分不起作用
	std::string mStartTime;
	//结束时间,,日期部分不起作用
	std::string mEndTime;
	//VIP条件
	int mConditionVip;
	//金币奖励
	int mRewardCoin;
};

class M_OnlineRewardCFG
{
public:
private:
	static std::auto_ptr<M_OnlineRewardCFG> msSingleton;
public:
	int GetCount();
	const M_OnlineRewardCFGData* GetData(int ID);
	boost::unordered_map<int, M_OnlineRewardCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_OnlineRewardCFG* GetSingleton();
private:
	boost::unordered_map<int, M_OnlineRewardCFGData> mMapData;
};
