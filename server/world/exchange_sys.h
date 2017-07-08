#pragma once
#include "game_sys_def.h"
struct stExchangeInfo;

// 兑换系统
class ExchangeSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_exchange);

	/*
			兑换
			player		兑换玩家
			chgId		兑换id
			phone		玩家电话
			返回值  e_msg_result_def 定义
	*/
	int exchange(game_player* player, int chgId, const std::string& phone);

	int getExchangeList(game_player* player, std::vector<stExchangeInfo>& infoList);
private:
	void _notice(game_player* player);
};

struct stExchangeInfo
{
	time_t m_genTime;
	int m_chgId;
	bool m_isReceive;
};

