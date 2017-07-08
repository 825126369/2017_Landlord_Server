#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ShowHand_RoomCFGData
{
	//房间ID
	int mRoomId;
	//房间名字
	std::string mRoomName;
	//房间最大玩家数量
	int mRoomMaxPlayerCount;
	//房间最大桌子数量
	int mTableMaxPlayerCount;
	//是否开放
	bool mIsOpen;
	//进入金币数量条件
	int mEnterGoldCondition;
	//进入桌子金币数量条件
	int mEnterTableGoldCondition;
	//非法退出游戏惩罚金币数
	int mIllegalQuitGoldCount;
	//底注
	int mAnte;
	//机器人数量
	int mAndroidCount;
	//机器人最小金币数
	int mRobMinGold;
	//机器人最大金币数
	int mRobMaxGold;
	//机器人最大VIP
	int mRobMinVip;
	//机器人最大VIP
	int mRobMaxVip;
	//房间机器人盈利率平衡线
	float mRoomAndroidYield;
};

class ShowHand_RoomCFG
{
public:
private:
	static std::auto_ptr<ShowHand_RoomCFG> msSingleton;
public:
	int GetCount();
	const ShowHand_RoomCFGData* GetData(int RoomId);
	boost::unordered_map<int, ShowHand_RoomCFGData>& GetMapData();
	void Reload();
	void Load();
	static ShowHand_RoomCFG* GetSingleton();
private:
	boost::unordered_map<int, ShowHand_RoomCFGData> mMapData;
};
