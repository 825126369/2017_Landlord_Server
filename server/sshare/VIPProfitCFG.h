#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct VIPProfitCFGData
{
	//vip等级
	int mVipLv;
	//升级所需经验
	int mVipExp;
	//领取在线奖励次数
	int mOnlineReward;
};

class VIPProfitCFG
{
public:
private:
	static std::auto_ptr<VIPProfitCFG> msSingleton;
public:
	int GetCount();
	const VIPProfitCFGData* GetData(int VipLv);
	boost::unordered_map<int, VIPProfitCFGData>& GetMapData();
	void Reload();
	void Load();
	static VIPProfitCFG* GetSingleton();
private:
	boost::unordered_map<int, VIPProfitCFGData> mMapData;
};
