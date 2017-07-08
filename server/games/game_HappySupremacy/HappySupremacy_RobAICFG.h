#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct HappySupremacy_RobAICFGData
{
	//Key
	int mID;
	//下注偏好
	std::string mBetName;
	//下注比例(顺序:0：对门,1：顺们，2：倒门,3:顺对门（角）,4:倒对门（角）,5:顺倒门（桥，横门）)(百分比)
	std::vector<int> mBetRate;
};

class HappySupremacy_RobAICFG
{
public:
private:
	static std::auto_ptr<HappySupremacy_RobAICFG> msSingleton;
public:
	int GetCount();
	const HappySupremacy_RobAICFGData* GetData(int ID);
	boost::unordered_map<int, HappySupremacy_RobAICFGData>& GetMapData();
	void Reload();
	void Load();
	static HappySupremacy_RobAICFG* GetSingleton();
private:
	boost::unordered_map<int, HappySupremacy_RobAICFGData> mMapData;
};
