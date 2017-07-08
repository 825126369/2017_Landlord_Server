#include "stdafx.h"

#include <i_game_player.h>
#include "game_db.h"
#include "game_db_log.h"
#include "logic_lobby.h"
#include "logic_room.h"
#include "logic_player.h"
#include "logic_table.h"
#include "msg_type_def.pb.h"
#include "proc_showhand_logic.h"
#include "ShowHand_BaseCFG.h"
#include "ShowHand_RoomCFG.h"
#include "time_helper.h"

#include "cardmanager.h"

SHOWHAND_SPACE_USING

logic_lobby::logic_lobby(void): m_init(false) 
{
	m_max_player=20000;
}

logic_lobby::~logic_lobby(void) {

}

void logic_lobby::init_config() {
    ShowHand_RoomCFG::GetSingleton()->Load();
    ShowHand_BaseCFG::GetSingleton()->Load();
	ShowHand_RoomStockCFG::GetSingleton()->Load();
}

void logic_lobby::init_protocol() {
    init_proc_showhand_logic();
}

void logic_lobby::init_game(int count) {
    //加载该游戏相关初始数据
    init_config();
	init_protocol();

	const auto& list = ShowHand_RoomCFG::GetSingleton()->GetMapData();
    for each (auto& var in list) 
	{
		if (var.second.mIsOpen) 
		{
			LRoomPtr r = boost::make_shared<logic_room>(&(var.second));
            m_rooms.insert(std::make_pair(var.second.mRoomId, r));
        }else
		{
			SLOG_CRITICAL<<"欢乐五张功能未开启"<<std::endl;
		}
    }

	m_init = true;
}
void logic_lobby::heartbeat(double elapsed) {
	if (! m_init) return;

	for (auto it = m_rooms.begin(); it != m_rooms.end(); ++it) {
		it->second->heartbeat(elapsed);
	}

    for (auto it_player = m_all_players.begin(); it_player != m_all_players.end(); ++it_player)
        it_player->second->heartbeat(elapsed);

}

//--------------------------------------------------------------------------------------
void logic_lobby::release_game() {
	if (!m_init)
        return;

	for (auto it = m_all_players.begin(); it != m_all_players.end(); ++it) 
	{
		it->second->release();
	}

	for (auto it = m_rooms.begin(); it != m_rooms.end(); ++it)
	{
		it->second->release();
	}

	m_rooms.clear();
	m_all_players.clear();	
}

bool logic_lobby::player_enter_game(iGPlayerPtr igplayer) {
	if (! m_init)
        return false;

    if (m_all_players.size() >= m_max_player)
        return false;

	if (m_all_players.find(igplayer->get_playerid()) != m_all_players.end())
		return true;

	auto lp = logic_player::malloc();
	lp->set_player(igplayer);
	igplayer->set_handler(lp);
	lp->enter_game(this);

	m_all_players.insert(std::make_pair(igplayer->get_playerid(), lp));
	return true;
}

void logic_lobby::player_leave_game(uint32_t playerid) 
{
	if (! m_init)
        return;

	auto it = m_all_players.find(playerid);
	if (it == m_all_players.end())
		return;

	it->second->leave_room();
	m_all_players.erase(it);
}


int logic_lobby::player_join_friend_game(iGPlayerPtr player, int fid) {

    return 1;
}

//---------------------------------------------------------------------------
LRoomPtr logic_lobby::get_room(int32_t rid) {
    auto room = m_rooms.find(rid);
    if (room == m_rooms.end())
        return LRoomPtr(nullptr);

    return room->second;
}

const LROOM_MAP& logic_lobby::get_rooms() {
	return m_rooms;
}

LPlayerPtr logic_lobby::get_player(uint32_t pid) {
	auto it = m_all_players.find(pid);
	if (it != m_all_players.end()) {
		return it->second;
	}

	return logic_player::EmptyPtr;
}

//返回一个机器人 返回的机器人进入大厅了
void logic_lobby::response_robot(int32_t playerid, int tag) {

	int room_id=tag/10000;
	int table_id=tag%10000;
	auto room_it = m_rooms.find(room_id);
    if (room_it == m_rooms.end())
        return;

    auto lcplayer = get_player(playerid);
    if (!lcplayer)
        return ;

	lcplayer->enter_room(room_id);
	if(table_id>0)
	{
		lcplayer->enter_table(table_id);
	}
   //room_it->second->enter_room(lcplayer);
}

