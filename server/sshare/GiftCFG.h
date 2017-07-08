#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct GiftCFGData
{
	//ÀñÎïID
	int mGiftId;
	//Ëùº¬½ð±Ò
	int mCoin;
};

class GiftCFG
{
public:
private:
	static std::auto_ptr<GiftCFG> msSingleton;
public:
	int GetCount();
	const GiftCFGData* GetData(int GiftId);
	boost::unordered_map<int, GiftCFGData>& GetMapData();
	void Reload();
	void Load();
	static GiftCFG* GetSingleton();
private:
	boost::unordered_map<int, GiftCFGData> mMapData;
};
