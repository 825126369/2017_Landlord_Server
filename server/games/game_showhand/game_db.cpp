#include "stdafx.h"
#include "game_db.h"


db_game::db_game()
{

}
db_game::~db_game()
{

}
void db_game::init_index()
{
	ensure_index(DB_SHOWHAND_PLAYER, BSON("player_id"<<1), "_player_id_");
}
