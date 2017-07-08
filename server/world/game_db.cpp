#include "stdafx.h"
#include "game_db.h"


//////////////////////////////////////////////////////////////////////////
db_player::db_player()
{

}
db_player::~db_player()
{

}
void db_player::init_index()
{
	//player_info
	ensure_index(DB_PLAYER_INFO, BSON("account"<<1), "_account_",true);
	ensure_index(DB_PLAYER_INFO, BSON("player_id"<<1), "_playerid_", true);
	//ensure_index(DB_PLAYER_INFO, BSON("player_id"<<1<<"platform"<<1), "_player_platform_");
	//ensure_index(DB_PLAYER_INFO, BSON("account"<<1<<"platform"<<1<<"delete"<<1), "_account_platform_delete_");

	//注意 排序字段也需要索引
	//ensure_index(DB_PLAYER_INFO, BSON("player_id"<<1<<"level"<<-1), "_playerid_level_");
	ensure_index(DB_PLAYER_INFO, BSON("level"<<-1), "_level_");
	ensure_index(DB_PLAYER_INFO, BSON("bindPhone" << -1), "_bindPhone_");

	//
	ensure_index(DB_TODAY_RECHARGE, BSON("player_id"<<1), "_playerid_", true);
	ensure_index(DB_YESTERDAY_RECHARGE, BSON("player_id"<<1), "_playerid_", true);


	ensure_index(DB_MAIL, BSON("player_id"<<1), "_playerid_");
	ensure_index(DB_MAIL, BSON("time"<<1), "_time_");
	ensure_index(DB_MAIL, BSON("player_id"<<1<<"time"<<1), "_playerid_time_");
	ensure_index(DB_PLAYER_QUEST, BSON("player_id"<<1), "_playerid_");

	ensure_index(DB_COMMON_CONFIG, BSON("type"<<1), "_type_");

	ensure_index(DB_EXCHANGE, BSON("genTime"<<1), "_genTime_");
	ensure_index(DB_EXCHANGE, BSON("playerId"<<1), "_playerid_");

	ensure_index(DB_PLAYER_BAG, BSON("player_id"<<1), "_playerid_");

	ensure_index(DB_PLAYER_FAVOR, BSON("gameId"<<1<<"playerId"<<1), "_gameId_playerId_");

	ensure_index(DB_GM_RECHARGE, BSON("playerId"<<1), "_playerid_");

	ensure_index(DB_TODAY_RECHARGE, BSON("player_id"<<1), "_playerid_");
	ensure_index(DB_TODAY_RECHARGE, BSON("total_rmb"<<1), "_total_rmb_");

	ensure_index(DB_SPEAKER, BSON("playerId"<<1), "_playerid_");
}

//////////////////////////////////////////////////////////////////////////
/*db_log::db_log()
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
	return unknown_table;
}
*/
//////////////////////////////////////////////////////////////////////////

db_game::db_game()
{

}

db_game::~db_game()
{

}

void db_game::init_index()
{

}
