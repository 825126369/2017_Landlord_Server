#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct HappySupremacy_RobCFGData
{
	//Key
	int mID;
	//玩家标签
	std::string mName;
	//金币最小值
	int mGoldMin;
	//金币最大值
	int mGoldMax;
	//VIP最小值
	int mVipMin;
	//VIP最大值
	int mVipMax;
};

class HappySupremacy_RobCFG
{
public:
private:
	static std::auto_ptr<HappySupremacy_RobCFG> msSingleton;
public:
	int GetCount();
	const HappySupremacy_RobCFGData* GetData(int ID);
	boost::unordered_map<int, HappySupremacy_RobCFGData>& GetMapData();
	void Reload();
	void Load();
	static HappySupremacy_RobCFG* GetSingleton();
private:
	boost::unordered_map<int, HappySupremacy_RobCFGData> mMapData;
};
