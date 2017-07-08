#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_GiftRewardCFGData
{
	//奖励类型
	int mRewardType;
	//道具ID
	std::vector<int> mItemId;
	//数量
	std::vector<int> mItemCount;
};

class M_GiftRewardCFG
{
public:
private:
	static std::auto_ptr<M_GiftRewardCFG> msSingleton;
public:
	int GetCount();
	const M_GiftRewardCFGData* GetData(int RewardType);
	boost::unordered_map<int, M_GiftRewardCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_GiftRewardCFG* GetSingleton();
private:
	boost::unordered_map<int, M_GiftRewardCFGData> mMapData;
};
