#include "stdafx.h"
#include "game_db_log.h"
#include "time_helper.h"
#include "game_player.h"

static std::string g_pumpTable[] = 
{
	"pumpActiveCount",

	"pumpActivePerson",

	"pumpPlayerMoney",

	"pumpSendGift",

	"pumpPhotoFrame",

	"pumpGeneralStat",

	"pumpPersonalSendGift",

	"pumpTotalConsume",

	"pumpCoinGrowth",

	"pumpCoinGrowthHistory",

	"pumpPlayerGold",

	"pumpMaxOnlinePlayer",

	"pumpPlayerLogin",

	"pumpPlayerSendGold",
};

//////////////////////////////////////////////////////////////////////////
db_log::db_log()
{

}
db_log::~db_log()
{

}
void db_log::init_index()
{
}

const std::string& db_log::get_tablename(uint16_t table_type)
{
	if(table_type < 0 || table_type >= e_dlt_max)
		throw new std::exception("table name out of range");

	return g_pumpTable[table_type];
}

void db_log::player_gold_log(int32_t player_id, GOLD_TYPE old_value, GOLD_TYPE new_value, int32_t reason)
{
	mongo::BSONObjBuilder builder;
	builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
	builder.append("playerId", player_id);
	builder.append("oldValue", old_value);
	builder.append("newValue", new_value);
	builder.append("addValue", new_value - old_value);
	builder.append("reason", reason);

	push_insert(10, builder.obj());
}

void db_log::player_login_log(int32_t player_id, const std::string& ip, int port, const std::string& mcode,const std::string& mtype)
{
	mongo::BSONObjBuilder builder;
	builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
	builder.append("playerId", player_id);
	builder.append("ip", ip);
	builder.append("port", port);
	builder.append("Machine_code", mcode);
	builder.append("Machine_type", mtype);

	push_insert(12, builder.obj());
}