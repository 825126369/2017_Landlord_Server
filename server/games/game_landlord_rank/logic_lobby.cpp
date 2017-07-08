#include "stdafx.h"
#include "logic_lobby.h"
#include "logic_player.h"
#include <i_game_player.h>
#include "time_helper.h"
#include "game_db_log.h"
#include "msg_type_def.pb.h"
#include "Landlord_Rank_RoomCFG.h"

logic_lobby::logic_lobby(void)
:m_init(false)
,m_max_player(0)
{
	
}

logic_lobby::~logic_lobby(void)
{
}

void logic_lobby::init_config()
{
	Landlord_Rank_RoomCFG::GetSingleton()->Load();
}
void logic_lobby::init_protocol()
{
	init_proc_landlord_protocol();
}

void logic_lobby::init_room()
{
	const boost::unordered_map<int, Landlord_Rank_RoomCFGData>& list = Landlord_Rank_RoomCFG::GetSingleton()->GetMapData();
	for (auto it = list.begin(); it != list.end(); ++it)
	{
		if(!it->second.mIsOpen)
		{
			continue;
		}
		auto room = boost::make_shared<logic_room>(&(it->second),this);
		roomMap.insert(std::make_pair(it->second.mRoomID, room));
	}
}
//游戏引擎初始化大厅入口
void logic_lobby::init_game()
{
	m_max_player = 20000;

	init_protocol();
	init_config();
	init_room();

	m_init = true;
}

void logic_lobby::release_game()
{
	if (!m_init)
	{
		return;
	}
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		it->second->release();
	}

	playerMap.clear();
	roomMap.clear();
}

void logic_lobby::heartbeat(double elapsed)
{
	if (!m_init)
	{
		return;
	}


	for (auto it = roomMap.begin(); it != roomMap.end(); ++it)
	{
		it->second->heartbeat(elapsed);
	}
}

bool logic_lobby::player_enter_game(iGPlayerPtr igplayer)
{
	if(!m_init)return false;

	if(playerMap.size()>= m_max_player)
		return false;

	if(playerMap.find(igplayer->get_playerid()) != playerMap.end())
		return false;

	auto lp = logic_player::malloc();
	lp->set_player(igplayer);
	igplayer->set_handler(lp);
	lp->enter_game(this);

	playerMap.insert(std::make_pair(igplayer->get_playerid(), lp));
	return true;
}

void logic_lobby::player_leave_game(uint32_t playerid)
{
	if(!m_init)return;

	auto it = playerMap.find(playerid);
	if(it == playerMap.end())
		return;

	it->second->leave_room();
	playerMap.erase(it);
}

int logic_lobby::player_join_friend_game(iGPlayerPtr igplayer, uint32_t friendid)
{
	return 2;
}

//-----------------------------------------------------------------------------------、

//pid:玩家ID,rid:房间ID
int logic_lobby::enter_room(uint32_t pid, uint16_t rid)
{
	auto it = playerMap.find(pid);
	if (it == playerMap.end())
	{
		return e_error_code_failed;
	}

	auto room = roomMap.find(rid);
	if(room == roomMap.end())
		return e_error_code_failed;;										//返回失败

	return room->second->enter_room(it->second);
	return 0;
}

void logic_lobby::leave_room(uint32_t pid)
{
	auto it = playerMap.find(pid);
	if(it == playerMap.end())
		return;

	it->second->leave_table();
}

const LROOM_MAP& logic_lobby::get_rooms() const
{
	return roomMap;
}

LPlayerPtr& logic_lobby::get_player(uint32_t pid)
{
	auto it = playerMap.find(pid);
	if(it != playerMap.end())
	{
		return it->second;
	}
	return logic_player::EmptyPtr;
}

//返回一个机器人 返回的机器人未进入房间？
void logic_lobby::response_robot(int32_t playerid, int tag)
{
	auto it = playerMap.find(playerid);
	if(it != playerMap.end())
		enter_room(playerid, tag);
	else
		std::cout << "logic_lobby::response_robot()  player is null" << std::endl;
}