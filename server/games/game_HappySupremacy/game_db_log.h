#pragma once

#include <enable_singleton.h>
#include <db_query.h>
#include "game_macro.h"

//日志表
enum e_db_log_table
{
	e_dlt_none = 0,
};

class logic_player;

//////////////////////////////////////////////////////////////////////////
//日志数据库
class db_log : public db_queue
	, public enable_singleton<db_log>
{
public:
	db_log();
	virtual ~db_log();
	virtual void init_index();

	virtual const std::string& get_tablename(uint16_t table_type);
	void property_log(logic_player* pl, int gameId, int ptype, GOLD_TYPE/*int*/ addValue, int reason, const std::string& param = "");

	void dayMoneyStat(int addValue, int reason, int itemId);

	void playerBanker(logic_player* pl,int bankerCount,GOLD_TYPE/*int*/ beforeGold,GOLD_TYPE/*int*/ nowGold, GOLD_TYPE/*int*/ resultValue,GOLD_TYPE/*int*/ sysGet);

	//在线游戏记录
	void joingame(int playerid, int roomid);
	void leavegame(int playerid);
private:
	void _recordCoinGrowth(int itemId, GOLD_TYPE/*int*/ addValue, logic_player* pl);
};
