#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ShowHand_RobCFGData
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

class ShowHand_RobCFG
{
public:
private:
	static std::auto_ptr<ShowHand_RobCFG> msSingleton;
public:
	int GetCount();
	const ShowHand_RobCFGData* GetData(int ID);
	boost::unordered_map<int, ShowHand_RobCFGData>& GetMapData();
	void Reload();
	void Load();
	static ShowHand_RobCFG* GetSingleton();
private:
	boost::unordered_map<int, ShowHand_RobCFGData> mMapData;
};
