#include "stdafx.h"
#include "game_player_mgr.h"
#include "game_player.h"
//#include "game_db.h"
#include "game_manager.h"
#include <i_game_engine.h>

game_player_mgr::game_player_mgr()
	//:m_cur_playerid(0)
	:b_closing(false)
{
}

game_player_mgr::~game_player_mgr()
{
}


GPlayerPtr game_player_mgr::find_playerbyid(uint32_t playerid)
{
	auto it = m_playersbypid.find(playerid);
	if(it != m_playersbypid.end())
		return it->second;

	return nullptr;
}

GPlayerPtr game_player_mgr::find_player(uint32_t sessionid)
{
	auto it = m_playersbysid.find(sessionid);
	if(it != m_playersbysid.end())
		return it->second;

	return nullptr;
}

bool game_player_mgr::add_player(GPlayerPtr p)
{
	auto ret = m_playersbypid.insert(std::make_pair(p->PlayerID, p));	 
	if(!ret.second)
		return false;

	if(p->get_sessionid() > 0)
		m_playersbysid.insert(std::make_pair(p->get_sessionid(), p));
	
	return true;
}

void game_player_mgr::remove_player(GPlayerPtr p)
{
	m_playersbysid.erase(p->get_sessionid());
	m_playersbypid.erase(p->PlayerID);
}

void game_player_mgr::reset_player(GPlayerPtr p, uint32_t sessionid)
{
	if(sessionid<=0)
		return;

	m_playersbysid.erase(p->get_sessionid());
	p->set_sessionid(sessionid);
	m_playersbysid.insert(std::make_pair(p->get_sessionid(), p));
}

void game_player_mgr::remove_session(uint32_t sessionid)
{
	m_playersbysid.erase(sessionid);
}

void game_player_mgr::heartbeat(double elapsed)
{
	for (auto it = m_playerdels.begin(); it != m_playerdels.end(); ++it)
	{
		auto p = find_playerbyid(*it);
		if(p)
		{
			p->leave_game();
			remove_player(p);
		}		
	}
	m_playerdels.clear();

	for (auto it = m_playersbypid.begin(); it != m_playersbypid.end(); ++it)
	{
		it->second->heartbeat(elapsed);
	}

	server_closing(elapsed);
}

void game_player_mgr::serverdown(bool bclose)
{
	b_closing = bclose;

	i_game_engine* eng = game_manager::instance().get_game_engine();
	for (auto it = m_playersbypid.begin(); it != m_playersbypid.end(); ++it)
	{
		eng->player_leave_game(it->second->PlayerID, true);	 //服务器关闭是强行退出	
	}
	m_playersbysid.clear();
	m_playersbypid.clear();	
}

bool game_player_mgr::is_closing()
{
	return b_closing;
}


void game_player_mgr::del_player(uint32_t pid)
{
	m_playerdels.push_back(pid);
}

#include "logic_server.h"
void game_player_mgr::server_closing(double elapsed)
{
	if(is_closing())//5秒后关闭 来保证玩家数据全部返回
	{
		static double close_time = 0;
		close_time+= elapsed;
		if(close_time > 5) 
		{
			if(!m_playersbypid.empty())//有可能有后进入的玩家
			{
				serverdown(true);
				close_time = 0;
				return;
			}

			game_manager::instance().get_game_engine()->exit_engine();
			logic_server::instance().close();
		}
	}
}