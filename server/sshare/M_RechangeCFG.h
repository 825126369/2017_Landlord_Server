#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_RechangeCFGData
{
	//充值ID
	int mID;
	//名字
	std::string mName;
	//描述
	std::string mDesc;
	//名字ICON
	std::string mNameIcon;
	//ICON
	std::string mIcon;
	//支付类型 1第三方平台 2独立支付宝
	int mPayType;
	//类型 1钻石或钻石 2月卡 3礼包
	int mType;
	//价格
	int mPrice;
	//金币
	int mGold;
	//首次送金币
	int mFirstGold;
	//每次送金币
	int mGiveGold;
	//每次送礼券
	int mGiveTicket;
	//增加vip经验
	int mVIPExp;
	//显示序号
	int mIndex;
	//显示商店
	int mShopType;
	//畅销标记
	bool mFlag;
	//AppStore商品ID
	std::string mAppStoreID;
};

class M_RechangeCFG
{
public:
private:
	static std::auto_ptr<M_RechangeCFG> msSingleton;
public:
	int GetCount();
	const M_RechangeCFGData* GetData(int ID);
	boost::unordered_map<int, M_RechangeCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_RechangeCFG* GetSingleton();
private:
	boost::unordered_map<int, M_RechangeCFGData> mMapData;
};
