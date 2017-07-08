#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct Landlord_NetHumanMachine_RoomCFGData
{
	//房间id
	int mRoomID;
	//房间名
	std::string mRoomName;
	//是否开放
	bool mIsOpen;
	//房间图片
	std::string mRoomImage;
	//进场金币条件
	int mGoldCondition;
	//是否洗牌
	bool morShuffle;
	//凳子数量
	int mdeskCount;
	//底分
	int mScore;
};

class Landlord_NetHumanMachine_RoomCFG
{
public:
private:
	static std::auto_ptr<Landlord_NetHumanMachine_RoomCFG> msSingleton;
public:
	int GetCount();
	const Landlord_NetHumanMachine_RoomCFGData* GetData(int RoomID);
	boost::unordered_map<int, Landlord_NetHumanMachine_RoomCFGData>& GetMapData();
	void Reload();
	void Load();
	static Landlord_NetHumanMachine_RoomCFG* GetSingleton();
private:
	boost::unordered_map<int, Landlord_NetHumanMachine_RoomCFGData> mMapData;
};
