#include "stdafx.h"
#include "logic_lobby.h"
#include "logic_player.h"
#include <i_game_player.h>
#include "time_helper.h"
#include "game_db_log.h"
#include "msg_type_def.pb.h"
#include "game_engine.h"

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
	LandlordNetHuman_RoomCFG::GetSingleton()->Load();
}
void logic_lobby::init_protocol()
{
	init_proc_landlord_protocol();
}

void logic_lobby::init_room()
{
	for (int i=0;i<10;i++)
	{
		int roomId = Find_RoomId();
		auto room = boost::make_shared<logic_table>(roomId);
		roomMap.insert(std::make_pair(roomId, room));
		roomIdList[roomId - 1000] = true;
	}
}

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

	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		it->second->heartbeat(elapsed);
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

int logic_lobby::rob_enter_room(uint32_t pid, uint16_t rid,int32_t deskId)
{
	auto it = playerMap.find(pid);
	if (it == playerMap.end())
	{
		return e_error_code_failed;
	}
	return it->second->rob_enter_room(rid,deskId);
}

void logic_lobby::rob_leave_room(uint32_t pid)
{
	auto it = playerMap.find(pid);
	if (it == playerMap.end())
	{
		return;
	}
	it->second->leave_room();
}

const LTABLE_MAP& logic_lobby::get_rooms() const
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

logic_table* logic_lobby::get_room(uint32_t rid)
{
	auto it = roomMap.find(rid);
	if (it != roomMap.end())
	{
		return it->second.get();
	}
	return nullptr;
}

void logic_lobby::response_robot(int32_t playerid, int tag)
{
	auto it = playerMap.find(playerid);
	if(it != playerMap.end())
		rob_enter_room(playerid, tag/100,tag%100);
	else
		std::cout << "logic_lobby::response_robot()  player is null" << std::endl;
}

int logic_lobby ::Find_RoomId()
{
	for (size_t i = 0; i <roomIdList.size(); i++)
	{
		if (roomIdList[i] ==false)
		{
			return i+1000;
		}
	}

	int room_Id= roomIdList.size() + 1000;
	roomIdList.push_back(false);
	return room_Id;
}

logic_table* logic_lobby::AutoMatchRoom()
{
	for (auto& var : roomMap)
	{
		if (!var.second->get_orFull())
		{
			return var.second.get();
		}
	}
	return nullptr;
}

e_server_error_code logic_lobby::CreateRoom(LPlayerPtr player, packetc2l_create_room* msg)
{
	int roomId = Find_RoomId();
	roomIdList[roomId - 1000] = true;
	auto room = boost::make_shared<logic_table>(roomId);
	e_server_error_code result = room->Set_Room_Info(msg->roomname(), msg->password(), msg->orshuffle());
	if (result != e_server_error_code::e_error_code_success)
	{
		return result;
	}
	roomMap.insert(std::make_pair(roomId, room));
	return player->enter_room(roomId,msg->password());
}

void logic_lobby::get_all_invite_player_list(int pid, packetl2c_join_player_list_result* sendmsg)
{
	for (auto& var : playerMap)
	{
		if (!var.second->is_inTable() && !var.second->is_robot() && var.first!=pid)
		{
			auto player = sendmsg->add_playerlist();
			player->set_deskid(var.second->get_deskId());
			player->set_player_id(var.second->get_pid());
			player->set_player_nickname(var.second->get_nickname());
			player->set_player_head_frame(var.second->get_head_frame_id());
			player->set_player_gold(var.second->get_gold());
			player->set_player_sex(var.second->get_player_sex());
			player->set_player_vip_lv(var.second->get_viplvl());
		}
	}
}

void logic_lobby::get_all_room_list(packetl2c_roomlist_result* sendmsg)
{
	std::vector<int> roomList;
	for (auto& var : roomMap)
	{
		auto roomInfo = sendmsg->add_roominfolist();
		roomInfo->set_room_id(var.first);
		roomInfo->set_roomname(var.second->get_roomName());
		roomInfo->set_orneedpassword(var.second->orNeedPassword());
		roomInfo->set_player_count(var.second->getTablePlayerCount());
		roomInfo->set_orshuffle(var.second->get_roomOrShuffle());
		if (var.second->orInGame())
		{
			roomInfo->set_room_state(2);
		}
		else
		{
			roomInfo->set_room_state(1);
		}
	}
}

//通知 房间外 房间信息
void logic_lobby::do_protobuf_notice_room_change_info(logic_table* table,int action)
{
	auto sendmsg = PACKET_CREATE(packetl2c_notice_room_change_info, e_mst_l2c_notice_room_change_info);
	sendmsg->set_action(action);

	auto roomInfo = sendmsg->mutable_roominfo();
	roomInfo->set_room_id(table->get_table_id());
	roomInfo->set_player_count(table->getTablePlayerCount());
	roomInfo->set_roomname(table->get_roomName());
	roomInfo->set_orneedpassword(table->orNeedPassword());
	roomInfo->set_room_state(table->orInGame());

	std::vector<uint32_t> pids;
	for (auto it = playerMap.begin(); it != playerMap.end(); it++)
	{
		if (!it->second->is_inTable() && !it->second->is_robot())
		{
			pids.push_back(it->second->get_pid());
		}
	}
	game_engine::instance().get_handler()->broadcast_msg_to_client(pids, sendmsg->packet_id(), sendmsg);
}

std::vector<int> logic_lobby::broadcast_msg_to_client(uint32_t except_id)
{
	std::vector<int> pids;
	for (auto it = playerMap.begin(); it != playerMap.end(); it++)
	{
		if (!it->second->is_inTable() && !it->second->is_robot() && it->first!=except_id)
		{
			pids.push_back(it->second->get_pid());
		}
	}
	return pids;
}