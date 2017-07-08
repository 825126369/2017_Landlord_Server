#include "stdafx.h"
#include "game_HappySupremacy.h"

GAME_HAPPYSUPREMACY_API void* get_game_engine()
{
	return &game_engine::instance();
}

GAME_HAPPYSUPREMACY_API void* get_packet_mgr()
{
	return &packet_manager::instance();
}