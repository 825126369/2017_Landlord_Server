#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ShowHand_RoomStockCFGData
{
	//房间id
	int mRoomID;
	//抽水
	float mDeduct;
	//默认库存
	int mDefaultStock;
	//库存水位(10个)
	std::vector<int> mStock;
	//放分概率（10个）
	std::vector<int> mScoreId;
	//幸运库存扩展
	int mLuckyEx;
	//幸运与金币比例
	int mLuckyGoldRate;
	//幸运增益
	float mLuckyIncBuff;
	//幸运减益
	float mLuckyDecBuff;
};

class ShowHand_RoomStockCFG
{
public:
private:
	static std::auto_ptr<ShowHand_RoomStockCFG> msSingleton;
public:
	int GetCount();
	const ShowHand_RoomStockCFGData* GetData(int RoomID);
	boost::unordered_map<int, ShowHand_RoomStockCFGData>& GetMapData();
	void Reload();
	void Load();
	static ShowHand_RoomStockCFG* GetSingleton();
private:
	boost::unordered_map<int, ShowHand_RoomStockCFGData> mMapData;
};
