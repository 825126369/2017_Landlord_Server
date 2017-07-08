#include "stdafx.h"
#include "logic_lobby.h"
#include "logic_room.h"
#include "logic_player.h"
#include <i_game_player.h>
#include "time_helper.h"
#include "game_db_log.h"
#include "proc_game_happysupremacy_protocol.h"
#include "msg_type_def.pb.h"
#include "HappySupremacy_RoomCFG.h"
#include "HappySupremacy_BaseCFG.h"
#include "HappySupremacy_BetMaxCFG.h"
#include "HappySupremacy_RobAICFG.h"
#include "HappySupremacy_RobCFG.h"
#include "HappySupremacy_RoomStockCFG.h"
#include "HappySupremacy_RateCFG.h"

logic_lobby::logic_lobby(void)
:m_init(false)
,m_max_player(0)
,m_check_cache(0.0)
{
	resetToday();
}

logic_lobby::~logic_lobby(void)
{
}

void logic_lobby::init_config()
{
	HappySupremacy_RoomCFG::GetSingleton()->Load();
	HappySupremacy_BaseCFG::GetSingleton()->Load();
	HappySupremacy_BetMaxCFG::GetSingleton()->Load();
	HappySupremacy_RobAICFG::GetSingleton()->Load();
	HappySupremacy_RobCFG::GetSingleton()->Load();
	HappySupremacy_RoomStockCFG::GetSingleton()->Load();
	HappySupremacy_RateCFG::GetSingleton()->Load();
}
void logic_lobby::init_protocol()
{
	init_proc_happysupremacy_protocol();
}

void logic_lobby::init_room()
{
	const boost::unordered_map<int, HappySupremacy_RoomCFGData>& list = HappySupremacy_RoomCFG::GetSingleton()->GetMapData();
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
	m_max_player = 10000;

	init_protocol();
	init_config();
	init_room();

	m_init = true;
}

void logic_lobby::release_game()
{
	if(!m_init)return;

	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		it->second->release();
	}
	playerMap.clear();

	roomMap.clear();
}

void logic_lobby::heartbeat(double elapsed)
{
	if(!m_init)return;

	for (auto it = roomMap.begin(); it != roomMap.end(); ++it)
	{
		it->second->heartbeat(elapsed);
	}
	m_check_cache += elapsed;
	if(m_check_cache > 60)
	{
		save_cache();
		m_check_cache = 0;
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
	if(player_enter_game(igplayer))
	{
		auto it = playerMap.find(friendid);
		if(it == playerMap.end())
			return 2;

		auto room = it->second->get_room();
		if(room != nullptr)
			return enter_room(igplayer->get_playerid(),room->get_room_id());

		return 1;
	}

	return 2;
}

//-----------------------------------------------------------------------------------、

//pid:玩家ID,rid:房间ID
int logic_lobby::enter_room(uint32_t pid, uint16_t rid)
{
	auto it = playerMap.find(pid);
	if(it == playerMap.end())
		return msg_type_def::e_rmt_fail;										//返回失败

	auto room = roomMap.find(rid);
	if(room == roomMap.end())
		return msg_type_def::e_rmt_fail;										//返回失败

	return room->second->enter_room(it->second);
}

void logic_lobby::leave_room(uint32_t pid)
{
	auto it = playerMap.find(pid);
	if(it == playerMap.end())
		return;

	it->second->leave_room();
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

void logic_lobby::resetToday()
{
	TempTodayOutlay = 0;
	TempTodayIncome = 0;

	for(int i = 0; i < 4; i++)
	{
		TempTodayIncomeArr[i] = TempTodayOutlayArr[i] = 0;
	}
}

//缓存当日统计
void logic_lobby::save_cache()
{
	if(TempTodayOutlay >0 ||TempTodayIncome>0)
	{
		auto now = time_helper::instance().get_cur_date();
		time_t nt = time_helper::convert_from_date(now) * 1000;

		db_log::instance().push_update(2, BSON("Date"<<mongo::Date_t(nt)), 
			BSON("$inc"<<BSON("TodayOutlay"<<TempTodayOutlay << "TodayIncome" << TempTodayIncome
			<< "room1Income" << getTodayIncome(1) << "room1Outlay" << getTodayOutlay(1)  
			<< "room2Income" << getTodayIncome(2) << "room2Outlay" << getTodayOutlay(2)
			<< "room3Income" << getTodayIncome(3) << "room3Outlay" << getTodayOutlay(3)
			<< "room4Income" << getTodayIncome(4) << "room4Outlay" << getTodayOutlay(4)

			)));

		resetToday();
	}
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