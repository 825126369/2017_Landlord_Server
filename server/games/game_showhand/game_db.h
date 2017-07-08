#pragma once

#include <db_base.h>
#include <enable_singleton.h>

static const std::string unknown_table = "DefaultTable";
static const std::string DB_SHOWHAND_PLAYER = "showhand_player";
static const std::string DB_SHOWHAND_ROOM = "showhand_room";
static const std::string DB_SHOWHAND_TABLE = "showhand_table";

//Íæ¼ÒÊý¾Ý¿â
class db_game : public db_base, public enable_singleton<db_game>
{
public:
	db_game();
	virtual ~db_game();
	virtual void init_index();
};