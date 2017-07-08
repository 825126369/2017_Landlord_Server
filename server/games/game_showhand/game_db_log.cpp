#include "stdafx.h"
#include "game_db_log.h"
#include "logic_player.h"
#include "time_helper.h"
#include "game_engine.h"
#include "logic_room.h"

SHOWHAND_SPACE_USING;

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
	static const std::string unknown_table = "DefaultTable";
		static const std::string GAMELOG = "pumpPlayerMoney";
	static const std::string ONLINE_GAME = "pumpOnlineGaming";
	static const std::string PlayerLuckyLog = "pumpPlayerLucky";
	switch ((e_db_log_table)table_type)
	{
	case 1:
		return GAMELOG;
		break;
	case 2:
		return ONLINE_GAME;
		break;
	case 3:
		return PlayerLuckyLog;
		break;
	default:
		break;
	}

	return unknown_table;
}


void db_log::property_log(logic_player* player, int gameId, int ptype, int addValue, int reason, const std::string& param)
{
	if(addValue == 0)
		return;

	mongo::BSONObjBuilder builder;
	builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
	builder.append("playerId", player->get_pid());
	builder.append("gameId", gameId);
	builder.append("itemId", ptype);

	int newValue = 0;
	if(ptype == 1)
	{		
		newValue = player->get_gold();	
	}
	else if(ptype == 2)
	{
		newValue = player->get_ticket();
	}

	builder.append("oldValue", newValue-addValue);	
	builder.append("newValue", newValue);
	builder.append("addValue", addValue);

	builder.append("reason", reason);
	builder.append("param",param);

	push_insert(1,builder.obj());
}

void db_log::joingame(int playerid, int roomid)
{
	mongo::BSONObjBuilder builder;
	builder.append("playerId", playerid);
	builder.append("gameId",game_engine::instance().get_gameid());
	builder.append("roomId", roomid);
	push_update(2, BSON("playerId"<<playerid), BSON("$set" <<builder.obj()));
}

void db_log::leavegame(int playerid)
{
	push_delete(2, BSON("playerId"<<playerid));
}

void db_log::pumpPlayerLucky(logic_player* player, int beforelucky)
{
	if(player->is_android())
		return;

	mongo::BSONObjBuilder builder;
	builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
	builder.append("playerId", player->get_pid());
	builder.append("gameId", game_engine::instance().get_gameid());
	builder.append("roomId", player->get_room()->get_room_id());	
	builder.append("oldValue", beforelucky);	
	builder.append("newValue", player->get_lucky());

	push_insert(3,builder.obj());
}