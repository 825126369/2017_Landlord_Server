#include "stdafx.h"
#include "logic_player.h"
#include "logic_room.h"
#include "logic_lobby.h"
#include <i_game_player.h>
#include <net\packet_manager.h>
#include "game_db.h"
#include "game_engine.h"
#include "game_db_log.h"
#include <enable_random.h>
#include <time_helper.h>
#include "Landlord_NetHumanMachine_RoomCFG.h"

logic_player::logic_player(void)
	:m_lobby(nullptr)
	, m_room(nullptr)
	, m_table(nullptr)
	, m_logic_gold(0)
	, m_change_gold(0)
	, m_checksave(0.0)
	, is_first_save(true)
	, deskId(0)
	, player_state(e_player_game_state::e_player_game_state_none)
	, m_player_online_state(e_player_state::e_ps_none)
	,rob_match_cd(0)
	,rob_robLandlord_cd(0)
	,rob_playhand_cd(0)
	,rob_match_max_cd(5)
	,rob_robLandlord_max_cd(5)
	,rob_playhand_max_cd(5)
	,orHang(false)
{
	logic_player_db::init_game_object();
}


logic_player::~logic_player(void)
{


}

//------------------------i_game_phandler-----------------------------------------
void logic_player::on_attribute_change(int atype, int v)
{
	if(atype == msg_type_def::e_itd_gold)
	{
		std::cout<<"玩家金币改变00："<<v<<std::endl;
		m_logic_gold += v;
	}
}

void logic_player::on_change_state()	//通知游戏状态改变，是否断线
{
	
}

void logic_player::on_attribute64_change(int atype, GOLD_TYPE v)
{
	std::cout<<"atype:"<<atype<<"  玩家金币改变11："<<v<<std::endl;
	m_logic_gold += v;
}
//-----------------------------------------------------------------


void logic_player::heartbeat(double elapsed)
{
	m_checksave += elapsed;
	if (m_checksave > 30)
	{
		if (is_first_save)
		{
			logic_player_db::store_game_object(true);
			is_first_save = false;
		}
		else
		{
			logic_player_db::store_game_object();
		}
		m_checksave = 0.0;
	}

	if (!is_robot())
	{
		if (get_game_state() != m_player_online_state)
		{
			m_player_online_state = get_game_state();
			if (m_player_online_state == e_player_state::e_ps_none)
			{
				SLOG_CRITICAL << "--------------玩家状态 none------------------";
			}
			else if (m_player_online_state == e_player_state::e_ps_loading)
			{
				SLOG_CRITICAL << "-------------玩家状态 加载---------------------";
			}
			else if (m_player_online_state == e_player_state::e_ps_playing)
			{
				SLOG_CRITICAL << "-------------玩家状态 正在玩------------------";
			}
			else if (m_player_online_state == e_player_state::e_ps_disconnect)
			{
				SLOG_CRITICAL << "---------------玩家 掉线------------------------";
				if (player_state == e_player_game_state::e_player_game_state_none || player_state == e_player_game_state::e_player_game_state_matching || player_state == e_player_game_state::e_player_game_state_awarding)
				{
					//leave_room();
					//m_player->leave_game();
					//m_pla
				}
			}
		}
	}

	if (m_room == nullptr)
	{
		return;
	}

	if (is_robot())//机器人AI
	{
		if (player_state == e_player_game_state::e_player_game_state_none)
		{
			if (rob_match_cd > rob_match_max_cd)
			{
				if (m_table == nullptr)
				{
					LTablePtr table = m_room->rob_find_realplayer_table();
					if (table.get() != nullptr)
					{
						LPlayerPtr player = game_engine::instance().get_lobby().get_player(this->get_pid());
						if (player != logic_player::EmptyPtr)
						{
							e_server_error_code result = m_room->rob_enter_table(player, table);
							if (result == e_server_error_code::e_error_code_success)
							{
								player_state = e_player_game_state::e_player_game_state_matching;
							}
						}
					}
				}
				else
				{
					if (player_state==e_player_game_state::e_player_game_state_none)
					{
						player_state = e_player_game_state::e_player_game_state_matching;
					}
				}
				rob_match_cd = 0;
				rob_match_max_cd = global_random::instance().rand_int(1,5);
			}
			else
			{
				rob_match_cd += elapsed;
			}
		}
		else if (player_state == e_player_game_state::e_player_game_state_matching)
		{

			
		}
		else if (player_state == e_player_game_state::e_player_game_state_sendcarding)
		{
			
		}
		else if (player_state == e_player_game_state::e_player_game_state_robLandlord)
		{
			if (rob_robLandlord_cd > rob_robLandlord_max_cd)
			{			
				int or_Rob = m_table->judge_rob_orLandlord(this) ? 1 : 2;
				robLandlord(or_Rob);

				rob_robLandlord_cd = 0;
				rob_robLandlord_max_cd= global_random::instance().rand_int(2, 5);
			}
			else
			{
				rob_robLandlord_cd += elapsed;
			}
		}
		else if (player_state == e_player_game_state::e_player_game_state_other_robLandlord)
		{
			
		}
		else if (player_state == e_player_game_state::e_player_game_state_playhanding)
		{
			if (rob_playhand_cd > rob_playhand_max_cd)
			{
				game_landlord_net_humanmachine_protocol::card_Info cards = m_table->get_rob_playhand(this);
				e_server_error_code result=playhand(cards);
				if (result == e_server_error_code::e_error_code_failed)
				{
					std::string str;
					for (int i = 0; i < cards.cards_size(); i++)
					{
						str += std::to_string(cards.cards().Get(i))+" ";
					}
					SLOG_CRITICAL << "机器人出牌 出错啦: "<<str;
				}

				rob_playhand_cd = 0;
				rob_playhand_max_cd = global_random::instance().rand_int(2, 5);
			}
			else
			{
				rob_playhand_cd += elapsed;
			}
			
		}
		else if (player_state == e_player_game_state::e_player_game_state_other_playhanding)
		{
			
		}
		else if (player_state == e_player_game_state::e_player_game_state_awarding)
		{

		}
	}
	else
	{
		if (orHang)
		{
			if (player_state == e_player_game_state::e_player_game_state_none)
			{

			}
			else if (player_state == e_player_game_state::e_player_game_state_matching)
			{


			}
			else if (player_state == e_player_game_state::e_player_game_state_sendcarding)
			{

			}
			else if (player_state == e_player_game_state::e_player_game_state_robLandlord)
			{
				if (rob_robLandlord_cd > rob_robLandlord_max_cd)
				{
					int or_Rob = m_table->judge_rob_orLandlord(this) ? 1 : 2;
					robLandlord(or_Rob);

					rob_robLandlord_cd = 0;
					rob_robLandlord_max_cd = global_random::instance().rand_int(1, 4);
				}
				else
				{
					rob_robLandlord_cd += elapsed;
				}
			}
			else if (player_state == e_player_game_state::e_player_game_state_other_robLandlord)
			{

			}
			else if (player_state == e_player_game_state::e_player_game_state_playhanding)
			{
				if (rob_playhand_cd > rob_playhand_max_cd)
				{
					game_landlord_net_humanmachine_protocol::card_Info cards = m_table->get_rob_playhand(this);
					e_server_error_code result = playhand(cards);
					if (result == e_server_error_code::e_error_code_failed)
					{
						std::string str;
						for (int i = 0; i < cards.cards_size(); i++)
						{
							str += std::to_string(cards.cards().Get(i)) + " ";
						}
						SLOG_CRITICAL << "机器人出牌 出错啦: " << str;
					}

					rob_playhand_cd = 0;
					rob_playhand_max_cd = global_random::instance().rand_int(1, 4);
				}
				else
				{
					rob_playhand_cd += elapsed;
				}

			}
			else if (player_state == e_player_game_state::e_player_game_state_other_playhanding)
			{

			}
			else if (player_state == e_player_game_state::e_player_game_state_awarding)
			{

			}
		}
	}
}

uint32_t logic_player::get_pid()
{
	return m_player->get_playerid();
}

uint16_t logic_player::get_viplvl()
{
	return m_player->get_attribute(msg_type_def::e_itd_vip);
}

GOLD_TYPE logic_player::get_gold()
{
	return m_logic_gold;
}

bool logic_player::change_gold(GOLD_TYPE v)
{
	if(m_logic_gold>=-v)
	{
		m_logic_gold += v;
		m_change_gold += v;
		return true;
	}
	return false;
}

void logic_player::sycn_gold()
{
	if (m_change_gold != 0)
	{
		m_player->change_gold(m_change_gold);
		m_change_gold = 0;
	}
}

bool logic_player::is_robot()
{
	return m_player->is_robot();
}

bool logic_player::change_ticket(int count,int reason)
{
	m_player->change_ticket(count);
	if (reason != -1)
	{
		
	}
	return true;
}

int logic_player::get_ticket()
{
	return m_player->get_attribute(msg_type_def::e_itd_ticket);
}


const std::string& logic_player::get_nickname()
{
	return m_player->get_nickname();
}

const std::string& logic_player::get_icon_custom()
{
	return m_player->get_icon_custom();
}

const uint32_t logic_player::get_head_frame_id()
{
	return m_player->get_attribute(msg_type_def::e_itd_photoframe);
}

const int16_t logic_player::get_player_sex()
{
	return m_player->get_attribute(msg_type_def::e_itd_sex);
}

void logic_player::onAcceptGift(int receiverId, int giftId)
{
	if (m_room != nullptr)
	{
		//m_table->bc_accept_gift(receiverId,giftId);
	}
}

void logic_player::quest_change(int quest_type, int count, int param)
{
	if (is_robot())
		return;

	m_player->quest_change(quest_type, count, param);
}

void logic_player::release()
{
	leave_room();
	m_player.reset();
}

e_player_state logic_player::get_game_state()
{
	return m_player->get_state();
}

void logic_player::set_player_game_state(e_player_game_state state) 
{ 
	player_state = state; 
	if (player_state == e_player_game_state::e_player_game_state_none)
	{
		rob_match_cd = 0;
	}
	else if (player_state == e_player_game_state::e_player_game_state_matching)
	{

	}
	else if (player_state == e_player_game_state::e_player_game_state_sendcarding)
	{

	}
	else if (player_state == e_player_game_state::e_player_game_state_robLandlord)
	{
		rob_robLandlord_cd = 0;
	}
	else if (player_state == e_player_game_state::e_player_game_state_other_robLandlord)
	{

	}
	else if (player_state == e_player_game_state::e_player_game_state_playhanding)
	{
		rob_playhand_cd = 0;
	}
	else if (player_state == e_player_game_state::e_player_game_state_other_playhanding)
	{

	}
	else if (player_state == e_player_game_state::e_player_game_state_awarding)
	{

	}
}

void logic_player::enter_game(logic_lobby* lobby)
{
	m_lobby = lobby;
	m_player_id->set_value(get_pid());
	logic_player_db::load_player();

	m_logic_gold = m_player->get_attribute64(msg_type_def::e_itd_gold);
}

e_server_error_code logic_player::enter_room(logic_room* room)
{
	if (m_room != nullptr || room == nullptr)
	{
		return e_server_error_code::e_error_code_failed;
	}

	m_room = room;
	m_logic_gold = m_player->get_attribute64(msg_type_def::e_itd_gold);
	player_state = e_player_game_state::e_player_game_state_none;
	return e_server_error_code::e_error_code_success;
}

e_server_error_code logic_player::leave_room()
{
	e_server_error_code result_ta= leave_table();
	if (result_ta !=e_server_error_code::e_error_code_success)
	{
		return result_ta;
	}

	if (m_room != nullptr)
	{
		e_server_error_code result_ro=m_room->leave_room(this->get_pid());
		if (result_ro == e_server_error_code::e_error_code_success)
		{
			m_room = nullptr;
		}
		else
		{
			return e_server_error_code::e_error_code_failed;
		}
	}

	sycn_gold();
	if (is_first_save)		//保存数据
	{
		logic_player_db::store_game_object(true);
		is_first_save = false;
	}
	else
	{
		logic_player_db::store_game_object();
	}
	return e_server_error_code::e_error_code_success;
}


e_server_error_code logic_player::enter_table()
{
	if (m_table != nullptr)
	{
		return e_server_error_code::e_error_code_failed;
	}

	LPlayerPtr player = game_engine::instance().get_lobby().get_player(this->get_pid());
	if (player == logic_player::EmptyPtr)
	{
		return e_server_error_code::e_error_code_failed;
	}
	e_server_error_code result=m_room->enter_table(player);
	SetOrHang(false);
	return result;
}

e_server_error_code logic_player::leave_table()
{
	if (m_table != nullptr)
	{
		e_server_error_code result=m_table->leave_table(get_pid());
		if (result == e_server_error_code::e_error_code_success)
		{
			m_table = nullptr;
			deskId = 0;
			player_state = e_player_game_state::e_player_game_state_none;
		}
		return result;
	}
	else
	{
		return e_server_error_code::e_error_code_success;
	}
}


e_server_error_code logic_player::start_match()
{
	e_server_error_code result = e_server_error_code::e_error_code_failed;
	if (m_table == nullptr)
	{
		result = enter_table();
		if (result == e_server_error_code::e_error_code_success)
		{
			player_state = e_player_game_state::e_player_game_state_matching;
		}
	}
	else
	{
		if (player_state == e_player_game_state::e_player_game_state_none)
		{
			player_state = e_player_game_state::e_player_game_state_matching;
			result = e_server_error_code::e_error_code_success;
		}
	}
	return result;
}

int logic_player::get_wait_time()// 抢地主
{
	return 10;
}

e_server_error_code logic_player::robLandlord(int or_Rob)// 抢地主
{
	if (player_state != e_player_game_state::e_player_game_state_robLandlord)
	{
		return e_server_error_code::e_error_code_failed;;
	}
	return m_table->rob_Landlord(this,or_Rob);
}

//出牌
e_server_error_code logic_player::playhand(const game_landlord_net_humanmachine_protocol::card_Info& cards)
{
	if (player_state != e_player_game_state::e_player_game_state_playhanding)
	{
		return e_server_error_code::e_error_code_failed;
	}
	if (cards.deskid() != get_deskId())
	{
		return e_server_error_code::e_error_code_failed;
	}
	return m_table->playhand(this,cards);
}

//挂机
e_server_error_code logic_player::SetOrHang(bool orHang)
{
	this->orHang = orHang;
	return e_server_error_code::e_error_code_success;
}


//得到场景信息
void logic_player::do_protobuf_send_SceneInfo()
{
	auto sendmsg = PACKET_CREATE(packetl2c_get_room_scene_info_result, e_mst_l2c_get_room_scene_info);
	if (is_inRoom())
	{
		sendmsg->set_room_id(m_room->get_room_id());
		sendmsg->set_playerstate(player_state);
		sendmsg->set_orhang(orHang);
		if (m_table != nullptr)
		{
			m_table->get_Scene_Info(this, sendmsg.get());
		}
	}
	m_player->send_msg_to_client(sendmsg);
}



bool logic_player_db::load_player()
{
	mongo::BSONObj b = db_game::instance().findone(DB_LANDLORD_PLAYER, BSON("player_id" << m_player_id->get_value()));
	if (b.isEmpty())
		return false;

	return from_bson(b);
}

void logic_player_db::init_game_object()
{
	m_player_id = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "player_id"));
}

//to_all
bool logic_player_db::store_game_object(bool to_all)
{
	if (!has_update())
		return true;

	auto err = db_game::instance().update(DB_LANDLORD_PLAYER, BSON("player_id" << m_player_id->get_value()), BSON("$set" << to_bson(to_all)));
	if (!err.empty())
	{
		SLOG_ERROR << "logic_player::store_game_object :" << err;
		return false;
	}
	return true;
}




