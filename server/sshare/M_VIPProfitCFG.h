#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_VIPProfitCFGData
{
	//vip等级
	int mVipLv;
	//升级所需经验
	int mVipExp;
	//领取在线奖励次数
	int mOnlineReward;
	//每日送礼上限
	long long mMaxGiftslimit;
	//解锁礼物
	std::vector<std::string> mGiftUnlock;
	//房间解锁
	std::vector<std::string> mRoomUnlock;
	//捕鱼道具解锁
	std::vector<std::string> mFishItemUnlock;
	//尊贵VIP标志
	int mVipName;
	//每天赠送礼券
	int mGiveTicket;
	//每日抽奖次数上限
	int mDailyLottery;
};

class M_VIPProfitCFG
{
public:
private:
	static std::auto_ptr<M_VIPProfitCFG> msSingleton;
public:
	int GetCount();
	const M_VIPProfitCFGData* GetData(int VipLv);
	boost::unordered_map<int, M_VIPProfitCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_VIPProfitCFG* GetSingleton();
private:
	boost::unordered_map<int, M_VIPProfitCFGData> mMapData;
};
