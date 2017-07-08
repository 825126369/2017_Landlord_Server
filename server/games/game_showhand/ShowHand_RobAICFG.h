#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ShowHand_RobAICFGData
{
	//Key
	int mID;
	//下注偏好
	std::string mBetName;
	//下注比例(顺序:和、闲、闲对、将对、将)(百分比)
	std::vector<int> mBetRate;
};

class ShowHand_RobAICFG
{
public:
private:
	static std::auto_ptr<ShowHand_RobAICFG> msSingleton;
public:
	int GetCount();
	const ShowHand_RobAICFGData* GetData(int ID);
	boost::unordered_map<int, ShowHand_RobAICFGData>& GetMapData();
	void Reload();
	void Load();
	static ShowHand_RobAICFG* GetSingleton();
private:
	boost::unordered_map<int, ShowHand_RobAICFGData> mMapData;
};
