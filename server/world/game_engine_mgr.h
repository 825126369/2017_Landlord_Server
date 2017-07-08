#pragma once
#include <enable_hashmap.h>
#include <enable_singleton.h>

struct game_info
{
public:
	uint16_t GameID;
	uint16_t GameServerID;
	uint32_t GameVer;
	uint32_t GamePlayerCount;	
};

typedef ENABLE_MAP<uint16_t, game_info> GAME_LIST;
typedef GAME_LIST::iterator GAME_LIST_IT;

class game_engine_mgr:public enable_singleton<game_engine_mgr>
{
public:
	game_engine_mgr();
	virtual ~game_engine_mgr();

	bool add_game_info(game_info& ginfo);
	bool find_game_info(uint16_t serverid, game_info& retinfo);
	bool get_game_info(uint16_t gameid, game_info& retinfo);
	bool update_game_info(uint16_t serverid, bool addplayer = true);
	void remove_game_info(uint16_t serverid);

	GAME_LIST& get_gamelist()
	{
		return m_games;
	}
private:
	GAME_LIST m_games;
};

