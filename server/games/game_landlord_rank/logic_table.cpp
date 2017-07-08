#include "stdafx.h"
#include "logic_table.h"
#include "logic_player.h"
#include "game_db.h"
#include "game_engine.h"
#include "i_game_ehandler.h"
#include <enable_random.h>
#include "game_db_log.h"
#include "msg_type_def.pb.h"
#include <math.h>
#include "Landlord_Rank_RoomCFG.h"

using namespace std;

logic_table::logic_table(logic_room* room, int tableId) :playhand_cdTime(0), sendcard_cdTime(0), roblandlord_cdTime(0)
	,m_room(nullptr)
	,award_cdTime(0)
	,lastPlayhand_Id(0)
{
	deskCount = 3;
	deskList.resize(deskCount);
	m_room = room;
	gameState = e_game_state::e_game_state_none;
	m_tableId = tableId;
	logic_table_db::init_game_object();
	logic_table_db::m_db_table_id->set_value(m_tableId);
	if (!load_table())
	{
		create_table();
	}
	m_cfg = Landlord_Rank_RoomCFG::GetSingleton()->GetData(m_room->get_room_id());
	m_cardManager=new logic_core(m_cfg);
}

logic_table::~logic_table(void)
{
	if (m_cardManager != nullptr)
	{
		delete m_cardManager;
		m_cardManager = nullptr;
	}
}

void logic_table::heartbeat(double elapsed)
{
	if (gameState == e_game_state::e_game_state_none)
	{
		if (playerMap.size() > 0)
		{
			gameState = e_game_state::e_game_state_matching;
		}
		return;
	}
	else
	{
		if (playerMap.size() == 0)
		{
			gameState = e_game_state::e_game_state_none;
			return;
		}
	}

	if (gameState == e_game_state::e_game_state_matching)//匹配
	{
		if (!check_ExistRealPlayer() && playerMap.size()>0)
		{
			std::vector<LPlayerPtr> playList;
			for (auto& var : playerMap)
			{
				playList.push_back(var.second);
			}
			for (auto& var : playList)
			{
				var->leave_table();
			}
			playList.clear();
		}

		if (playerMap.size() >= deskCount)
		{
			bool allPrepare = true;
			for(auto& var: playerMap)
			{
				if (var.second->get_deskId() > 0 && var.second->get_deskId() <= deskCount)
				{
					if (var.second->get_player_game_state() != e_player_game_state::e_player_game_state_matching)
					{
						allPrepare = false;
						break;
					}
				}
				else
				{
					allPrepare = false;
					break;
				}
			}
			if (allPrepare)
			{
				gameState = e_game_state::e_game_state_startgame;
			}
		}
	}
	else if (gameState == e_game_state::e_game_state_startgame)//开始游戏，发完所有牌
	{
		do_protobuf_notice_start_game();
		sendcard_cdTime = 0;
		gameState = e_game_state::e_game_state_sendcarding;
	}
	else if (gameState == e_game_state::e_game_state_sendcarding)//发牌阶段
	{
		if (sendcard_cdTime > 11.0)
		{
			gameState = e_game_state::e_game_state_robLandlore;
			current_robLandlord_Id = m_cardManager->get_Landlord();
			do_protobuf_notice_robLandlord();
			sendcard_cdTime = 0;
		}
		else
		{
			sendcard_cdTime += elapsed;
		}
	}
	else if (gameState == e_game_state::e_game_state_robLandlore)//叫地主
	{
		if (roblandlord_cdTime > 10.0)
		{
			for (auto& var : playerMap)
			{
				if (var.second->get_deskId() == current_robLandlord_Id)
				{
					e_server_error_code result=var.second->robLandlord(2);
					if (result != e_server_error_code::e_error_code_success)
					{
						auto sendmsg = PACKET_CREATE(packetl2c_notice_rob_landlord_result, e_mst_l2c_notice_rob_landlord_result);
						sendmsg->set_result(result);
						var.second->send_msg_to_client(sendmsg);
					}
					break;
				}
			}
			roblandlord_cdTime = 0.0;
		}else
		{
			roblandlord_cdTime += elapsed;
		}	
	}
	else if (gameState == e_game_state::e_game_state_playhand)//玩家出牌
	{
		if (playhand_cdTime > 15.0)
		{
			for (auto& var : playerMap)
			{
				if (var.second->get_deskId() == current_playhand_Id)
				{
					game_landlord_rank_protocol::card_Info cards = this->get_rob_playhand(var.second.get());
					e_server_error_code result = var.second->playhand(cards);
					if (result != e_server_error_code::e_error_code_success)
					{
						auto sendmsg = PACKET_CREATE(packetl2c_notice_playhand, e_mst_l2c_notice_playhand);
						sendmsg->set_result(result);
						var.second->send_msg_to_client(sendmsg);					
					}
					break;
				}
			}
			playhand_cdTime = 0.0;
		}
		else
		{
			playhand_cdTime += elapsed;
		}
	}
	else if (gameState == e_game_state::e_game_state_award)//开奖
	{
		if (award_cdTime > 5)
		{
			gameState = e_game_state::e_game_state_none;
			for (auto& var : playerMap)
			{
				var.second->set_player_game_state(e_player_game_state::e_player_game_state_none);
			}
			award_cdTime = 0;
		}
		else
		{
			award_cdTime+=elapsed;
		}
	}

}

uint16_t logic_table::get_table_id()
{
	return m_tableId;
}

bool logic_table::check_ExistRealPlayer()
{
	for (auto& var : playerMap)
	{
		if (!var.second->is_robot())
		{
			return true;
		}
	}
	return false;
}

e_server_error_code logic_table::enter_table(LPlayerPtr player)
{
	if (get_orFull())
	{
		SLOG_CRITICAL << "桌子满了";
		return e_error_code_failed;
	}
	auto it = playerMap.find(player->get_pid());
	if (it != playerMap.end())
	{
		SLOG_CRITICAL << "玩家已经在桌子里了";
		return e_error_code_success;
	}
	playerMap.insert(make_pair(player->get_pid(),player));
	int deskId=Find_DeskPos();
	if (deskId<=0)
	{
		SLOG_CRITICAL << "没有找到凳子";
		return e_error_code_failed;
	}
	deskList[deskId - 1] = 1;
	player->set_table(this);
	player->set_deskId(deskId);
	SLOG_CRITICAL << "进入桌子成功" << std::endl;
	return e_error_code_success;
}

e_server_error_code logic_table::leave_table(uint32_t playerId)
{
	auto it = playerMap.find(playerId);
	if (it == playerMap.end())
	{
		return e_error_code_success;
	}
	int deskId = it->second->get_deskId();
	if (deskId <= 0)
	{
		return e_error_code_failed;
	}
	deskList[deskId -1] = 0;
	playerMap.erase(it);
	return e_error_code_success;
}

int logic_table::Find_DeskPos()
{
	for (size_t i = 0; i <deskList.size(); i++)
	{
		if (deskList[i] == 0)
		{
			return i+1;
		}
	}
	return -1;
}

e_game_state logic_table::get_table_State()
{
	return gameState;
}

bool logic_table::get_orFull()
{
	return playerMap.size() >= deskCount;
}

e_server_error_code logic_table::rob_Landlord(logic_player* player, int orRob)
{
	do_protobuf_notice_robLandlord_result(player, orRob);
	if (orRob == 1)
	{
		realLandlord_Id = player->get_deskId();
		m_cardManager->SureLandlord(realLandlord_Id);
		gameState = e_game_state::e_game_state_playhand;
		current_playhand_Id = realLandlord_Id;
		do_protobuf_notice_who_playhand();
	}
	else if(orRob==2)
	{
		current_robLandlord_Id++;
		if (current_robLandlord_Id > deskCount)
		{
			current_robLandlord_Id = 1;
		}
		int landlord_id = m_cardManager->get_Landlord();
		if (current_robLandlord_Id == landlord_id)
		{
			realLandlord_Id = landlord_id;
			m_cardManager->SureLandlord(realLandlord_Id);
			gameState = e_game_state::e_game_state_playhand;
			current_playhand_Id = realLandlord_Id;
			do_protobuf_notice_who_playhand();
		}
		else
		{
			do_protobuf_notice_robLandlord();
		}
	}

	return e_server_error_code::e_error_code_success;
}
bool logic_table::orMustPlayHand(logic_player* player)
{
	if (player->get_deskId() == lastPlayhand_Id || lastPlayhand_Id == 0)
	{
		return true;
	}
	else
	{
		return false;
	}
}

e_server_error_code logic_table::playhand(logic_player* player, const game_landlord_rank_protocol::card_Info& cards)
{
	assert(player->get_deskId()==cards.deskid());
	if (cards.cards_size() > 0)
	{
		std::vector<int> m_cards;
		for (int i = 0; i < cards.cards_size(); i++)
		{
			m_cards.push_back(cards.cards().Get(i));
		}

		if (!m_cardManager->check_cards(player->get_deskId(), m_cards))
		{
			return e_server_error_code::e_error_code_failed;
		}

		if (lastPlayhand_Id != cards.deskid() && lastPlayhand_Id>0)
		{
			int result = m_cardManager->compare_card(m_cards, lastPlayhand);
			if (result <= 0)
			{
				SLOG_CRITICAL << "出牌信息有误";
				return e_server_error_code::e_error_code_failed;
			}
		}
		lastPlayhand = m_cards;
		lastPlayhand_Id = cards.deskid();
		m_cardManager->playhand(cards.deskid(), m_cards);
	}
	else
	{
		if (lastPlayhand_Id == cards.deskid() || lastPlayhand_Id==0)
		{
			return e_server_error_code::e_error_code_failed;
		}
	}

	do_protobuf_notice_playhand(player,cards);
	if (m_cardManager->get_cards_info(cards.deskid()).size() == 0)
	{
		do_protobuf_notice_winlose(cards.deskid());
		gameState = e_game_state::e_game_state_award;
		return e_server_error_code::e_error_code_success;
	}
	current_playhand_Id++;
	if (current_playhand_Id > deskCount)
	{
		current_playhand_Id = 1;
	}
	do_protobuf_notice_who_playhand();
	return e_server_error_code::e_error_code_success;
}

void logic_table::do_protobuf_notice_start_game()//开始游戏
{
	SLOG_CRITICAL << "开始游戏" << std::endl;
	m_cardManager->send_card();
	realLandlord_Id = m_cardManager->get_Landlord();
	lastPlayhand_Id = 0;

	auto sendmsg = PACKET_CREATE(packetl2c_notice_startgame, e_mst_l2c_notice_startgame);
	int openCard=m_cardManager->get_openCard();
	sendmsg->set_opencard(openCard);

	const std::vector<int>& m_remainLandlord_cards = m_cardManager->get_RemainLandlordCards();
	for (auto& var : m_remainLandlord_cards)
	{
		sendmsg->add_cards_1(var);
	}

	for (auto& var : playerMap)
	{
		auto player= sendmsg->add_playerlist();
		player->set_deskid(var.second->get_deskId());
		player->set_player_id(var.second->get_pid());
		player->set_player_nickname(var.second->get_nickname());
		player->set_player_head_frame(var.second->get_head_frame_id());
		player->set_player_gold(var.second->get_gold());
		player->set_player_sex(var.second->get_player_sex());
		player->set_player_vip_lv(var.second->get_viplvl());
	}

	int landlord_deskId = m_cardManager->get_Landlord();
	sendmsg->set_landlord_id(landlord_deskId);

	int score = Landlord_Rank_RoomCFG::GetSingleton()->GetData(m_room->get_room_id())->mScore;
	sendmsg->set_min_score(score*2);
	for (auto& var : playerMap)
	{
		sendmsg->clear_cards();
		std::vector<int>& m_cards = m_cardManager->get_cards_info(var.second->get_deskId());
		for (auto& var1 : m_cards)
		{
			sendmsg->add_cards(var1);
		}
		var.second->send_msg_to_client(sendmsg);
	}

	for (auto& var : playerMap)
	{
		var.second->set_player_game_state(e_player_game_state::e_player_game_state_sendcarding);
	}
}

void logic_table::do_protobuf_notice_who_playhand()//通知  某某人出牌
{
	auto sendmsg = PACKET_CREATE(packetl2c_notice_who_playhand, e_mst_l2c_notice_who_playhand);
	sendmsg->set_deskid(current_playhand_Id);
	broadcast_msg_to_client(sendmsg);

	for (auto& var : playerMap)
	{
		if (var.second->get_deskId() ==current_playhand_Id)
		{
			var.second->set_player_game_state(e_player_game_state::e_player_game_state_playhanding);
		}
		else
		{
			var.second->set_player_game_state(e_player_game_state::e_player_game_state_other_playhanding);
		}
	}

	playhand_cdTime = 0.0;
}

void logic_table::do_protobuf_notice_playhand(logic_player* player, const game_landlord_rank_protocol::card_Info& cards)//通知  所有人 出牌信息
{
	auto sendmsg = PACKET_CREATE(packetl2c_notice_playhand, e_mst_l2c_notice_playhand);
	sendmsg->mutable_cards()->CopyFrom(cards);
	broadcast_msg_to_client(sendmsg);
}

void logic_table::do_protobuf_notice_robLandlord()//通知 某某抢地主
{
	auto sendmsg = PACKET_CREATE(packetl2c_notice_rob_landlord, e_mst_l2c_notice_rob_landlord);
	sendmsg->set_deskid(current_robLandlord_Id);
	broadcast_msg_to_client(sendmsg);

	for (auto& var : playerMap)
	{
		if (var.second->get_deskId() == current_robLandlord_Id)
		{
			var.second->set_player_game_state(e_player_game_state::e_player_game_state_robLandlord);
		}
		else
		{
			var.second->set_player_game_state(e_player_game_state::e_player_game_state_other_robLandlord);
		}
	}

	roblandlord_cdTime = 0;
}

void logic_table::do_protobuf_notice_robLandlord_result(logic_player* player,int orRob)//通知 所有人 抢地主的结果
{
	auto sendmsg = PACKET_CREATE(packetl2c_notice_rob_landlord_result, e_mst_l2c_notice_rob_landlord_result);
	sendmsg->set_deskid(player->get_deskId());
	sendmsg->set_or_rob(orRob);
	broadcast_msg_to_client(sendmsg);
}

void logic_table::do_protobuf_notice_winlose(int win_deskId)//通知 开奖
{
	bool Landlord_Win = false;
	int score = Landlord_Rank_RoomCFG::GetSingleton()->GetData(m_room->get_room_id())->mScore;
	if (win_deskId == m_cardManager->get_Landlord())
	{
		Landlord_Win = true;
	}

	auto sendmsg = PACKET_CREATE(packetl2c_notice_winlose, e_mst_l2c_notice_winlose);
	for (auto& var : playerMap)
	{
		auto deskAward = sendmsg->add_m_desk_awards();
		int deskId = var.second->get_deskId();
		deskAward->set_deskid(deskId);

		int award = 0;
		if (Landlord_Win == true)
		{
			if (deskId == m_cardManager->get_Landlord())
			{
				award=score*2;
			}
			else
			{
				award=-score;
			}
		}
		else
		{
			if (deskId == m_cardManager->get_Landlord())
			{
				award=-score*2;
			}
			else
			{
				award=score;
			}
		}

		deskAward->set_win_count(award);
		var.second->change_gold(award);//改变玩家金币
	}

	for (auto& var : m_cardManager->get_cards_info())
	{
		auto desk_remain_cards = sendmsg->add_remain_cards();
		int deskId = var.first;

		desk_remain_cards->set_deskid(var.first);
		for (auto& var1 : var.second)
		{
			desk_remain_cards->add_cards(var1);
		}
	}
	broadcast_msg_to_client(sendmsg);
	m_cardManager->GameFinish();
	for (auto& var : playerMap)
	{
		var.second->set_player_game_state(e_player_game_state::e_player_game_state_awarding);
		var.second->SetOrHang(false);
	}
}

void logic_table::get_Scene_Info(logic_player* player, game_landlord_rank_protocol::packetl2c_get_room_scene_info_result* sendmsg)
{
	sendmsg->set_gamestate(gameState);
	e_player_game_state player_state = player->get_player_game_state();
	if (gameState>=2 && gameState<=5)
	{
		int openCard = m_cardManager->get_openCard();
		sendmsg->set_opencard(openCard);

		const std::vector<int>& m_remainLandlord_cards = m_cardManager->get_RemainLandlordCards();
		for (auto& var : m_remainLandlord_cards)
		{
			sendmsg->add_cards_1(var);
		}

		for (auto& var : playerMap)
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

		int landlord_deskId = m_cardManager->get_Landlord();
		sendmsg->set_landlord_id(landlord_deskId);

		for (auto& var : playerMap)
		{
			std::vector<int>& m_cards = m_cardManager->get_cards_info(var.second->get_deskId());
			if (var.second->get_pid() == player->get_pid())
			{
				for (auto& var1 : m_cards)
				{
					sendmsg->add_cards(var1);
				}
			}
			else
			{
				auto othercardInfo= sendmsg->add_other_cards_info();
				othercardInfo->set_deskid(var.second->get_deskId());
				othercardInfo->set_cards_count(m_cards.size());
			}
		}

		int score = Landlord_Rank_RoomCFG::GetSingleton()->GetData(m_room->get_room_id())->mScore;
		sendmsg->set_score(score*2);
	}
	
	if (gameState==e_game_state::e_game_state_robLandlore)
	{
		sendmsg->set_cdtime(10-roblandlord_cdTime);
		sendmsg->set_current_who_roblandlording(current_robLandlord_Id);
	}
	else if (gameState==e_game_state::e_game_state_playhand)
	{
		sendmsg->set_cdtime(15 - playhand_cdTime);
		sendmsg->set_current_who_playhanding(current_playhand_Id);
		if (lastPlayhand_Id >0)
		{
			auto lastplayhand_info = sendmsg->mutable_lastplayhand();
			lastplayhand_info->set_deskid(lastPlayhand_Id);
			for (auto& var : lastPlayhand)
			{
				lastplayhand_info->add_cards(var);
			}
		}
	}
	else if (player_state == e_player_game_state::e_player_game_state_awarding)
	{

	}
}




void logic_table_db::create_table()
{

}

//房间数据是要保存到数据库的
bool logic_table_db::load_table()
{
	mongo::BSONObj b = db_game::instance().findone(DB_LANDLORD_TABLE, BSON("table_id" << m_db_table_id->get_value()));
	//如果刚开始数据里没有这个数据
	if (b.isEmpty())
		return false;
	return from_bson(b);
}

void logic_table_db::init_game_object()
{
	m_db_table_id = CONVERT_POINT(Tfield<int16_t>, regedit_tfield(e_got_int16, "table_id"));
}

bool logic_table_db::store_game_object(bool to_all)
{
	if (!has_update())
		return true;

	auto err = db_game::instance().update(DB_LANDLORD_ROOM, BSON("room_id" << m_db_table_id->get_value()), BSON("$set" << to_bson(to_all)));
	if (!err.empty())
	{
		SLOG_ERROR << "logic_room::store_game_object :" << err;
		return false;
	}

	return true;
}







