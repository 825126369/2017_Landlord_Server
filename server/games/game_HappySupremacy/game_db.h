#pragma once

#include <db_base.h>
#include <enable_singleton.h>

//////////////////////////////////////////////////////////////////////////
static const std::string unknown_table = "DefaultTable";
static const std::string DB_HAPPYSUPREMACY_PLAYER = "happySupremacy_player";
static const std::string DB_HAPPYSUPREMACY_ROOM = "happySupremacy_room";

//Íæ¼ÒÊý¾Ý¿â
class db_game : public db_base, public enable_singleton<db_game>
{
public:
	db_game();
	virtual ~db_game();
	virtual void init_index();
};