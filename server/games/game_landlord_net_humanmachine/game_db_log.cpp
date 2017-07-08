#include "stdafx.h"
#include "game_db_log.h"
#include "time_helper.h"
#include "game_engine.h"

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
	static const std::string CROCODILELOG = "landlordEveryday";
	static const std::string BetLOG = "landlordBetInfo";
	static const std::string TOTAL_CONSUME = "pumpTotalConsume";
	static const std::string PLAYER_BANKER = "pumplandlordPlayerBanker";
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