#include "stdafx.h"
#include "game_engine_mgr.h"

game_engine_mgr::game_engine_mgr()
{
}

game_engine_mgr::~game_engine_mgr()
{
	m_games.clear();
}


bool game_engine_mgr::add_game_info(game_info& ginfo)
{
	auto ret = m_games.insert(std::make_pair(ginfo.GameServerID, ginfo));
	return ret.second;
}

bool game_engine_mgr::find_game_info(uint16_t serverid, game_info& retinfo)
{
	auto it = m_games.find(serverid);
	if(it==m_games.end())
		return false;

	retinfo = it->second;
	return true;
}

bool game_engine_mgr::get_game_info(uint16_t gameid, game_info& retinfo)
{
	for (auto it = m_games.begin(); it != m_games.end(); ++it)
	{
		if(it->second.GameID != gameid)
			continue;

		game_info& gi = it->second;
		
		if(gi.GamePlayerCount<1000)
		{
			retinfo = it->second;
			return true;
		}
	}
	return false;
}

bool game_engine_mgr::update_game_info(uint16_t serverid,bool addplayer)
{
	auto it = m_games.find(serverid);
	if(it==m_games.end())
		return false;

	if(addplayer)
		it->second.GamePlayerCount++;
	else
		it->second.GamePlayerCount--;

	return true;
}

void game_engine_mgr::remove_game_info(uint16_t serverid)
{
	auto it = m_games.find(serverid);
	if(it!=m_games.end())
		m_games.erase(it);
}