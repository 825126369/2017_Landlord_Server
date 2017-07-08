#pragma once
#include <enable_singleton.h>
#include "game_def.h"


class game_player_mgr
	:public enable_singleton<game_player_mgr>
{
public:
	game_player_mgr();
	~game_player_mgr();

	GPlayerPtr find_playerbyid(uint32_t playerid);
	GPlayerPtr find_player(uint32_t sessionid);

	bool add_player(GPlayerPtr p);
	void remove_player(GPlayerPtr p);
	void remove_session(uint32_t sessionid);
	void reset_player(GPlayerPtr p, uint32_t sessionid);
	void heartbeat(double elapsed);
	void serverdown(bool bclose = false);

	//uint32_t generic_playerid();
	void del_player(uint32_t pid);

	GPlayerMap& get_player_map()
	{
		return m_playersbypid;
	}

	bool is_closing();

private:
	GPlayerMap m_playersbypid;
	GPlayerMap m_playersbysid;
	//uint32_t m_cur_playerid;

	std::vector<uint32_t> m_playerdels;
	bool b_closing;
	void server_closing(double elapsed);
};
