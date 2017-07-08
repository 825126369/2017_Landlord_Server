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
#include "HappySupremacy_RoomCFG.h"
#include "HappySupremacy_BaseCFG.h"
#include "HappySupremacy_BetMaxCFG.h"
#include "HappySupremacy_RobAICFG.h"

logic_player::logic_player(void)
:m_lobby(nullptr)
,m_room(nullptr)
,m_logic_gold(0)
,m_change_gold(0)
,m_once_win_gold(0)
,m_bet_gold_count(0)
,m_checksave(0.0)
,is_first_save(true)
,is_banker(false)
,m_bet_max(0)
,m_rob_bet_cd(0.0)
,m_rob_bet_max(0)
,m_rob_bet_remain(0)
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
	//leave_room();
}

void logic_player::on_attribute64_change(int atype, GOLD_TYPE v)
{
	std::cout<<"atype:"<<atype<<"  玩家金币改变11："<<v<<std::endl;
	m_logic_gold += v;
}
//-----------------------------------------------------------------


void logic_player::heartbeat( double elapsed )
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
			logic_player_db::store_game_object();
		m_checksave = 0.0;
	}

	if (m_room == nullptr)
		return;

	robot_heartbeat(elapsed);
}

void logic_player::enter_game(logic_lobby* lobby)
{
	m_lobby = lobby;
	m_player_id->set_value(get_pid());
	logic_player_db::load_player();

	m_logic_gold = m_player->get_attribute64(msg_type_def::e_itd_gold);//m_player->get_attribute(msg_type_def::e_itd_gold);

}

bool logic_player::enter_room(logic_room* room)
{
	if(m_room != nullptr && room == nullptr)
		return false;


	m_room = room;
	m_logic_gold = m_player->get_attribute64(msg_type_def::e_itd_gold);

	if(is_GM_CONTROL())
	{
		m_room->set_Stock(0,0);
	}

	return true;
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
//copy bacarat code，why write style?
void logic_player::set_bet_max()
{
	m_bet_max = 100;
	const boost::unordered_map<int, HappySupremacy_BetMaxCFGData>& list = HappySupremacy_BetMaxCFG::GetSingleton()->GetMapData();
	for (int i = 0; i <HappySupremacy_BetMaxCFG::GetSingleton()->GetCount(); i++)
	{
		auto temp_data =HappySupremacy_BetMaxCFG::GetSingleton()->GetData((i + 1));
		if (get_gold() >= temp_data->mGoldCount)
		{
			m_bet_max = temp_data->mBetMax;
		}
		else
		    break;
	}
	
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
		db_log::instance().property_log(this, game_engine::instance().get_gameid(), 1,m_change_gold,11);
		m_change_gold = 0;
	}
}

bool logic_player::is_GM_CONTROL()
{
	return m_player->get_attribute(msg_type_def::e_itd_privilege)> 0;
}


bool logic_player::is_robot()
{
	return m_player->is_robot();
}

bool logic_player::change_gold2(int v, int season)
{
	bool ret = m_player->change_gold(v);
	if (ret)
	{
		m_logic_gold += v;
		db_log::instance().property_log(this, game_engine::instance().get_gameid(), msg_type_def::e_item_type_def::e_itd_gold, v, season);
	}
	return ret;
}

bool logic_player::change_ticket(int count,int reason)
{
	m_player->change_ticket(count);
	if (reason != -1)
	{
		db_log::instance().property_log(this, game_engine::instance().get_gameid(), 2,count,reason);
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

void logic_player::leave_room()
{
	if(m_room != nullptr)
	{
		if (m_room->get_room_state() == e_game_state::e_state_game_bet)
		{
			clear_bet();		//退出桌子押注数据无效
			m_room->set_is_have_bet(true);
		}		
		m_room->leave_room(get_pid());
		m_room = nullptr;
	}	
	clear_table_data();

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
}

void logic_player::escapeHandle()
{


}

e_player_state logic_player::get_game_state()
{
	return m_player->get_state();
}

void logic_player::release()
{
	leave_room();
	m_player.reset();
}

void logic_player::clear_once_data()
{
	m_old_bet_list.clear();
	for (auto& it : m_bet_list)
	{
		m_old_bet_list[it.first] = it.second;
	}
	m_bet_list.clear();
	m_bet_gold_count = 0;
	m_once_win_gold = 0;
	set_bet_max();
}

void logic_player::clear_table_data()
{
	m_bet_list.clear();
	m_old_bet_list.clear();
	is_banker = false;
}

msg_type_def::e_msg_result_def logic_player::add_bet(e_bet_type type,int count)
{
	if (is_banker)
		return msg_type_def::e_rmt_banker_not_bet;

	if (m_logic_gold < count)
		return msg_type_def::e_rmt_gold_not_enough;		//金币不足

	if (m_room == nullptr)
		return msg_type_def::e_rmt_no_find_table;
	
	if ((m_bet_list[type] + count) >  m_bet_max)
		return msg_type_def::e_rmt_outof_bet_limit;			//超过单个押注上限

	GOLD_TYPE temp_count = m_room->get_can_bet_count();
	if (temp_count >= 0)
	{
		if (count > temp_count)
			return msg_type_def::e_rmt_outof_bet_limit;			//超过单个押注上限
	}

	m_logic_gold -= count;
	m_change_gold -= count;
	m_bet_gold_count += count;
	m_bet_list[type] += count;
	m_room->set_is_have_bet(true);

	return msg_type_def::e_rmt_success;			//成功
}

msg_type_def::e_msg_result_def logic_player::repeat_bet()
{
	if (is_banker)
		return msg_type_def::e_rmt_banker_not_bet;

	clear_bet();		//清零原有下注

	GOLD_TYPE temp_gold = 0;
	for (auto& it : m_old_bet_list)
	{
		temp_gold += it.second;
	}
	if (m_logic_gold < temp_gold)
		return msg_type_def::e_rmt_gold_not_enough;

	if (m_room == nullptr)
		return msg_type_def::e_rmt_no_find_table;

	GOLD_TYPE temp_count = m_room->get_can_bet_count();
	if (temp_count >= 0)
	{
		if (temp_gold > temp_count)
			return msg_type_def::e_rmt_outof_bet_limit;
	}

	for (auto& it : m_old_bet_list)
	{
		if (it.second > m_bet_max)
			return msg_type_def::e_rmt_outof_bet_limit;			//超过单个押注上限
		m_bet_list[it.first] = it.second;
		m_bet_gold_count += it.second;

	}
	m_change_gold -=temp_gold;
	m_logic_gold -= temp_gold;
	m_room->set_is_have_bet(true);

	return msg_type_def::e_rmt_success;
}

msg_type_def::e_msg_result_def logic_player::clear_bet()
{
	for (auto& a:m_bet_list)
	{
		m_logic_gold += a.second;
		m_change_gold += a.second;
	}
	m_bet_list.clear();
	m_bet_gold_count = 0;

	return msg_type_def::e_rmt_success;
}

msg_type_def::e_msg_result_def logic_player::leave_banker()
{
	if (!is_banker)
		return msg_type_def::e_rmt_success;

	static int MinBankerCount = HappySupremacy_BaseCFG::GetSingleton()->GetData("MinBankerCount")->mValue;
	if (m_room->get_continue_banker_count() >= MinBankerCount)		//达到最小连庄次数
	{
		if (m_room->get_now_banker_id() == get_pid())
			m_room->set_now_banker_null(get_pid());
		else
			return msg_type_def::e_rmt_fail;
	}
	else
	{
		if (get_ticket() >= m_room->get_room_cfg()->mLeaveBankerCost)	//扣除提前下庄的礼券
		{
			if (m_room->get_now_banker_id() == get_pid())
			{
				change_ticket(-m_room->get_room_cfg()->mLeaveBankerCost,34);
				m_room->set_now_banker_null(get_pid());
			}
			else
				return msg_type_def::e_rmt_fail;
		}
		else
			return msg_type_def::e_rmt_ticket_not_enough;
	}

	return msg_type_def::e_rmt_success;
}

void logic_player::add_bet_win(GOLD_TYPE count)
{
	m_once_win_gold += count;
	change_gold(count);
}

std::map<e_bet_type,GOLD_TYPE>& logic_player::get_bet_list()
{
	return m_bet_list;
}

bool logic_player::set_is_banker(bool isbanker)
{
	if (m_room == nullptr)
		return false;

	if (!isbanker)
	{
		if (is_banker)
		{
			static double SystemRate = HappySupremacy_BaseCFG::GetSingleton()->GetData("SystemDrawWater")->mValue/100.0;
			GOLD_TYPE banker_win = m_room->get_banker_win();
			GOLD_TYPE sys_win = 0;
			if (banker_win > 0)
				sys_win = banker_win*SystemRate;

			change_gold(-sys_win);

			if (!is_robot())
				db_log::instance().playerBanker(this,m_room->get_continue_count(),(m_logic_gold - banker_win),m_logic_gold,banker_win,sys_win);
		}
	}

	if (isbanker)
	{
		if (m_logic_gold >= m_room->get_room_cfg()->mBankerCondition)
		{
			is_banker = true;
			return true;
		}
		return false;
	}
	else
		is_banker = false;

	return true;
}
//修改星星数量
void logic_player::add_star_lottery_info(int32_t award,int32_t star)
{
	m_player->add_starinfo(award);
	if (star <= 0)
		return;

	m_win_count->add_value(1);
	static int WinStarCount = HappySupremacy_BaseCFG::GetSingleton()->GetData("WinStarCount")->mValue;
	if (m_win_count->get_value() > WinStarCount)
	{
		m_win_count->set_value(0);
		m_player->add_starinfo(0,1);
	}
}

void logic_player::quest_change(int quest_type,int count,int param)
{
	if (is_robot())
		return;

	m_player->quest_change(quest_type,count,param);
}

//废弃的
void logic_player::bc_game_msg(int money, const std::string& sinfo, int mtype)
{
	if(m_room)
	{
		//auto rcfg = m_table->get_room()->get_data();
		//m_player->game_broadcast(rcfg->mRoomName, 2, sinfo, money, mtype);
	}
}





