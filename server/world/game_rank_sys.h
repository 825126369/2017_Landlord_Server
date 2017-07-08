#pragma once
#include "game_sys_def.h"

struct stRankInfo
{
	int m_playerId;
	std::string m_nickName;
	GOLD_TYPE m_gold;
	int16_t m_vipLevel;
};

// 排行系统
class GameRankSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_rank);

	/*
			返回金币排行榜
			rankList		返回列表
			selfRank		返回player的排行, 从0开始计数
	*/
	int getCoinRankList(game_player* player, std::vector<stRankInfo>& rankList, int& selfRank);
	int getRechargeRankList(game_player* player, std::vector<stRankInfo>& rankList, int& selfRank, int& selfrmb, bool yesterday = false);

	virtual void sys_time_update();

	/*
			金币增长
	*/
	int getCoinGrowthList(game_player* player, std::vector<stRankInfo>& rankList, int& selfRank, GOLD_TYPE& selfGold);
private:
	void _coinGrowthProcess();
};

