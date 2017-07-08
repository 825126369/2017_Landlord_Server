#pragma once
#include "game_sys_def.h"
struct stGift;

// 统计系统
class PumpSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_pump);

	virtual void init_sys_object();	

	virtual void sys_time_update();

	virtual void sys_update(double delta);

	/*
			统计玩家喜好
	*/
	void enterGame(int gameId, int playerId);

	/*
			金币，礼券变化总表
	*/
	void moneyTotalLog(game_player* player, int gameId, int itemId, GOLD_TYPE addValue, int reason, const std::string& param = "");

	/*
			变化量为0的特殊记录
	*/
	void moneyTotalLog(game_player* player, int gameId, int itemId, int reason, const std::string& param);

	/*
			增加通用统计
	*/
	void addGeneralStatLog(int statType);

	/*
			购买了道具
	*/
	void buyItemLog(int itemId);

	/*
			赠送礼物
	*/
	void sendGiftLog(int giftId);

	void sendGiftLog(std::vector<stGift>& giftList);

	void sendGiftLog(int senderId, int receiverId, stGift& giftInfo);

	//转账
	void pumpSendGold1(const std::string& mailid, game_player* sender, GOLD_TYPE sendgold, int recverid);
	void pumpSendGold2(const std::string& mailid, game_player* recver, GOLD_TYPE recvgold, GOLD_TYPE CounterFee);
private:
	// 活跃次数，重复计算
	void _activeCount(int gameId);

	void _activePerson(int gameId, int playerId);

	void _generalStat(int id, int tableType);

	void _recordCoinGrowth(int itemId, int addValue, game_player* player);

	void _statDailyMoney(int itemId, int addValue, int reason);

private:
	// 最高在线玩家个数
	int m_maxOnlinePlayerNum;
	// 检测时间
	double m_checkTime;
};








