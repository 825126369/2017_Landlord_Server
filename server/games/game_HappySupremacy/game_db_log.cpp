#include "stdafx.h"
#include "game_db_log.h"
#include "logic_player.h"
#include "time_helper.h"
#include "game_engine.h"

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
	static const std::string CROCODILELOG = "HappySupremacyEveryday";
	static const std::string BetLOG = "HappySupremacyBetInfo";
	static const std::string TOTAL_CONSUME = "pumpTotalConsume";
	static const std::string PLAYER_BANKER = "pumpHappySupremacyPlayerBanker";
	static const std::string COIN_GROWTH = "pumpCoinGrowth";

	static const std::string ONLINE_GAME = "pumpOnlineGaming";

	switch ((e_db_log_table)table_type)
	{
	case 1:
		return GAMELOG;
		break;
	case 2:
		return CROCODILELOG;
		break;
	case 3:
		return BetLOG;
		break;
	case 4:
		return TOTAL_CONSUME;
		break;
	case 5:
		return PLAYER_BANKER;
		break;
	case 6:
		return COIN_GROWTH;
		break;
	case 7:
		return ONLINE_GAME;
		break;
	default:
		break;
	}

	return unknown_table;
}

//ptype:是金币还是钻石
void db_log::property_log(logic_player* pl, int gameId, int ptype, GOLD_TYPE addValue, int reason, const std::string& param)
{
	if(addValue == 0)
		return;

	if(pl->is_robot())
		return;

	mongo::BSONObjBuilder builder;
	builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
	builder.append("playerId", pl->get_pid());
	builder.append("gameId", gameId);
	builder.append("itemId", ptype);

	int newValue = 0;
	if(ptype == 1)
	{		
		newValue = pl->get_gold();	
	}
	else if(ptype == 2)
	{
		newValue = pl->get_ticket();
	}

	_recordCoinGrowth(ptype, addValue, pl);

	builder.append("oldValue", newValue-addValue);	
	builder.append("newValue", newValue);
	builder.append("addValue", addValue);
	builder.append("reason", reason);
	builder.append("param",param);

	push_insert(1,builder.obj());

	dayMoneyStat(addValue, reason, ptype);
}

void db_log::dayMoneyStat(int addValue, int reason, int itemId)
{
	int changeType = 0;
	int64_t val = 0;
	if(addValue > 0)
	{
		changeType = 0; // 收入
		val = addValue;
	}
	else
	{
		changeType = 1; // 支出
		val = -addValue;
	}

	auto now = time_helper::instance().get_cur_date();
	time_t nt = time_helper::convert_from_date(now) * 1000;

	mongo::BSONObj cond = BSON("time" << mongo::Date_t(nt)
		<< "reason" << reason << "itemId" << itemId << "changeType" << changeType);
	db_log::instance().push_update(4, cond, BSON("$inc" << BSON("value" << val)));
}

void db_log::playerBanker(logic_player* pl,int bankerCount,GOLD_TYPE/*int*/ beforeGold,GOLD_TYPE/*int*/ nowGold, GOLD_TYPE/*int*/ resultValue,GOLD_TYPE/*int*/ sysGet)
{
	if(pl == nullptr)
		return;

	auto now = time_helper::instance().get_cur_time();
	
	mongo::BSONObjBuilder builder;
	builder.appendTimeT("genTime", now);
	builder.append("playerId", pl->get_pid());
	builder.append("playerName", pl->get_nickname());
	builder.append("bankerCount", bankerCount);
	builder.append("beforeGold", beforeGold);
	builder.append("nowGold", nowGold);
	builder.append("resultValue", resultValue);
	builder.append("sysGet", sysGet);

	db_log::instance().push_insert(5, builder.done());
}

void db_log::_recordCoinGrowth(int itemId, GOLD_TYPE/*int*/ addValue, logic_player* pl)
{
	//	if(addValue < 0)
	//		return;

	if(itemId == 1)
	{
		mongo::BSONObj cond = BSON("playerId" << pl->get_pid());
		int64_t v = addValue;
		db_log::instance().push_update(6, cond, BSON( "$inc" << BSON("gold" << v) ));

		db_log::instance().push_update(6, cond, BSON("$set" << BSON(
			"nickName" << pl->get_nickname() 
			<< "vipLevel" << pl->get_viplvl() )
			));
	}
}

void db_log::joingame(int playerid, int roomid)
{
	mongo::BSONObjBuilder builder;
	builder.append("playerId", playerid);
	builder.append("gameId", game_engine::instance().get_gameid());
	builder.append("roomId", roomid);
	push_update(7, BSON("playerId"<<playerid), BSON("$set" <<builder.obj()));
}

void db_log::leavegame(int playerid)
{
	push_delete(7, BSON("playerId"<<playerid));
}