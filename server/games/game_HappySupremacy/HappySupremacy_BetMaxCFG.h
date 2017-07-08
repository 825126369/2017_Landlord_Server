#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct HappySupremacy_BetMaxCFGData
{
	//Key
	int mID;
	//金币数量
	int mGoldCount;
	//单个投注上限
	int mBetMax;
	//可用筹码序号
	std::vector<int> mCanUseWeight;
};

class HappySupremacy_BetMaxCFG
{
public:
private:
	static std::auto_ptr<HappySupremacy_BetMaxCFG> msSingleton;
public:
	int GetCount();
	const HappySupremacy_BetMaxCFGData* GetData(int ID);
	boost::unordered_map<int, HappySupremacy_BetMaxCFGData>& GetMapData();
	void Reload();
	void Load();
	static HappySupremacy_BetMaxCFG* GetSingleton();
private:
	boost::unordered_map<int, HappySupremacy_BetMaxCFGData> mMapData;
};
