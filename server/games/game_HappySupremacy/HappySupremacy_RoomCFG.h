#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct HappySupremacy_RoomCFGData
{
	//房间id
	int mRoomID;
	//房间名
	std::string mRoomName;
	//砝码列表
	std::vector<int> mWeightList;
	//是否开放
	bool mIsOpen;
	//上庄条件(金币)
	int mBankerCondition;
	//抢庄花费(金币)
	int mFirstBankerCost;
	//每次抢庄竞价累计值(金币)
	int mAddBankerCost;
	//提前下庄花费(礼券)
	int mLeaveBankerCost;
	//自动下庄条件(金币)
	int mAutoLeaveBanker;
	//房间最大人数限制
	int mPlayerMaxCount;
	//机器人标签
	std::vector<int> mRobTag;
	//对应概率(百分比)
	std::vector<int> mCreateProb;
	//房间图片
	std::string mRoomImage;
	//进场VIP条件
	int mVipCondition;
	//进场金币条件
	int mGoldCondition;
	//最大下注额
	int mBetMax;
	//机器人数量
	int mRobCount;
};

class HappySupremacy_RoomCFG
{
public:
private:
	static std::auto_ptr<HappySupremacy_RoomCFG> msSingleton;
public:
	int GetCount();
	const HappySupremacy_RoomCFGData* GetData(int RoomID);
	boost::unordered_map<int, HappySupremacy_RoomCFGData>& GetMapData();
	void Reload();
	void Load();
	static HappySupremacy_RoomCFG* GetSingleton();
private:
	boost::unordered_map<int, HappySupremacy_RoomCFGData> mMapData;
};
