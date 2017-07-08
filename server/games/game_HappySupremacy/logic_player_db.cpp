#include "stdafx.h"
#include "logic_player.h"
#include "logic_room.h"
#include "logic_lobby.h"
#include <i_game_player.h>
#include <net\packet_manager.h>
#include "game_db.h"
#include "game_engine.h"
#include "game_db_log.h"
#include <enable_random.h>
#include <time_helper.h>
#include "HappySupremacy_RoomCFG.h"
#include "HappySupremacy_BaseCFG.h"
#include "HappySupremacy_BetMaxCFG.h"
#include "HappySupremacy_RobAICFG.h"

bool logic_player_db::load_player()
{
	mongo::BSONObj b = db_game::instance().findone(DB_HAPPYSUPREMACY_PLAYER, BSON("player_id" << m_player_id->get_value()));
	if(b.isEmpty())
		return false;

	return from_bson(b);
}

void logic_player_db::init_game_object()
{	
	m_player_id = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "player_id"));
	m_once_win_maxgold = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(e_got_int64, "OnceWinMaxGold"));
	m_win_count = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "win_count"));
}

bool logic_player_db::store_game_object(bool to_all)
{
	if(!has_update())
		return true;
	
	auto err = db_game::instance().update(DB_HAPPYSUPREMACY_PLAYER, BSON("player_id" << m_player_id->get_value()), BSON("$set"<<to_bson(to_all)));
	if(!err.empty())
	{
		SLOG_ERROR << "logic_player::store_game_object :" <<err;
		return false;
	}
	return true;
}