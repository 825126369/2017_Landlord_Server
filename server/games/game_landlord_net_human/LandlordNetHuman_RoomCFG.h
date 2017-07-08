#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct LandlordNetHuman_RoomCFGData
{
	//房间id
	int mRoomID;
	//房间名前缀
	std::string mRoomNamePrefix;
	//进场金币条件
	int mGoldCondition;
	//是否洗牌
	bool morShuffle;
	//凳子数量
	int mdeskCount;
	//底分
	int mScore;
};

class LandlordNetHuman_RoomCFG
{
public:
private:
	static std::auto_ptr<LandlordNetHuman_RoomCFG> msSingleton;
public:
	int GetCount();
	const LandlordNetHuman_RoomCFGData* GetData(int RoomID);
	boost::unordered_map<int, LandlordNetHuman_RoomCFGData>& GetMapData();
	void Reload();
	void Load();
	static LandlordNetHuman_RoomCFG* GetSingleton();
private:
	boost::unordered_map<int, LandlordNetHuman_RoomCFGData> mMapData;
};
