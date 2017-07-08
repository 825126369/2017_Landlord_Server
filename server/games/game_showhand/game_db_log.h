#pragma once

#include <enable_singleton.h>
#include <db_query.h>
#include "logic_def.h"

//日志表
enum e_db_log_table
{
	e_dlt_none = 0,
};

SHOWHAND_SPACE_BEGIN;

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

	void property_log(logic_player* player, int gameId, int ptype, int addValue, int reason, const std::string& param = "");

		//在线游戏记录
	void joingame(int playerid, int roomid);
	void leavegame(int playerid);

	void pumpPlayerLucky(logic_player* player, int beforelucky);
};

SHOWHAND_SPACE_END;