#include "stdafx.h"
#include "logic_room.h"
#include "HappySupremacy_RoomCFG.h"
#include "HappySupremacy_BaseCFG.h"
#include "HappySupremacy_RobCFG.h"
#include "HappySupremacy_RoomStockCFG.h"
#include "HappySupremacy_RateCFG.h"
#include "logic_player.h"
#include "game_db.h"
#include "game_engine.h"
#include "i_game_ehandler.h"
#include <enable_random.h>
#include "game_db_log.h"
#include "msg_type_def.pb.h"
#include <math.h>

using namespace std;

static const int BANKER_MAX_COUNT = 10;	//申请上庄列表最大数量
static const int SYNC_BET_TIME = 1;		//几秒同步一次押注

logic_room::logic_room(const HappySupremacy_RoomCFGData* cfg, logic_lobby* _lobby):
	m_cd_time(0.0)
	,m_game_state(e_game_state::e_state_game_begin)
	,is_have_bet(false)
	,m_banker_once_win(0)
	,m_banker_total_win(0)
	,m_system_draw_water(0)
	,m_draw_water_rate(0.0)
	,m_once_income(0)
	,m_once_outcome(0)
	,m_continue_banker_count(0)
	,m_now_banker_id(0)
	,m_old_banker_id(0)
	,m_elapse(0.0)
	,is_change_banker(false)
	,m_checksave(0.0)
	,is_have_banker(false)
	,is_can_rob_banker(false)
	,m_rob_banker_id(0)
	,m_rob_banker_cost(0)
	,is_have_rob_banker(false)
	,m_total_bet_count(0)
	,is_gm(false)
	,gm_index(0)
	,gm_max(0)
	,m_rob_cd(0.0)
	,m_rob_count(0)
	,m_rob_earn_rate(0.0)
	,m_no_banker_count(0)
	,is_refresh_history(false)
	,IsOpenRob(false)
	,currentNeedRobCount_cd(0.0)
	,GM_CONTROL_COMMAND(0)
{
	m_cfg = cfg;
	m_StockCFG=HappySupremacy_RoomStockCFG::GetSingleton()->GetData(m_cfg->mRoomID);
	m_lobby = _lobby;

	m_draw_water_rate = HappySupremacy_BaseCFG::GetSingleton()->GetData("SystemDrawWater")->mValue/100.0;
	IsOpenRob = HappySupremacy_BaseCFG::GetSingleton()->GetData("IsOpenRob")->mValue;
	IsOpenGM = HappySupremacy_BaseCFG::GetSingleton()->GetData("IsOpenGM")->mValue;
	robMinCount = global_random::instance().rand_int(HappySupremacy_BaseCFG::GetSingleton()->GetData("RobMinCount")->mValue,HappySupremacy_BaseCFG::GetSingleton()->GetData("RobMaxCount")->mValue);

	m_core_engine = new logic_core;

	logic_room_db::init_game_object();
	m_db_room_id->set_value(m_cfg->mRoomID);
	if(!load_room())
	{
		create_room();
	}

	//历史记录
	m_history_list.clear();
	for(int i = 0; i < m_db_room_historyPtr->get_obj_count(); i++)
	{
		auto ptr = m_db_room_historyPtr->get_Tobj<HistoryItem>(i);
		history_info temp;

		msg_bet_result_info* info=temp.add_result_list();
		info->set_type(e_bettype_forwarddoor);
		if(ptr->is_forwarddoor_win->get_value())
		{
			info->set_result(e_betresult_win);
		}else
		{
			info->set_result(e_betresult_lose);
		}

		info=temp.add_result_list();
		info->set_type(e_bettype_oppositedoor);
		if(ptr->is_oppositedoor_win->get_value())
		{
			info->set_result(e_betresult_win);
		}else
		{
			info->set_result(e_betresult_lose);
		}

		info=temp.add_result_list();
		info->set_type(e_bettype_reversedoor);
		if(ptr->is_reversedoor_win->get_value())
		{
			info->set_result(e_betresult_win);
		}else
		{
			info->set_result(e_betresult_lose);
		}
		m_history_list.push_back(temp);
	}
}

logic_room::~logic_room(void)
{
	SAFE_DELETE(m_core_engine);
}

void logic_room::heartbeat( double elapsed )
{
	if (IsOpenRob > 0)//创建机器人
	{
		m_rob_cd += elapsed;
		if(m_rob_cd > 3.0)
		{
			create_robot();
			m_rob_cd = 1;
		}
	}

	if (m_game_state == e_game_state::e_state_game_begin)
	{
		static int BET_TIME = HappySupremacy_BaseCFG::GetSingleton()->GetData("BetTime")->mValue;
		m_cd_time = BET_TIME + 1;
		//清理下注信息
		InitBetInfo();

		bool orChangeBanker=refreshBanker();
		bc_change_banker(orChangeBanker);	//更改了上庄信息才发通知

		bc_begin_bet();			//广播开始下注
		m_game_state = e_game_state::e_state_game_bet;
	}
	else if (m_game_state == e_game_state::e_state_game_bet)	//押注期间
	{
		m_cd_time -= elapsed;
		if (m_cd_time <= 0)
		{
			if (sync_bet_to_room())			//最后一次同步押注
			{
				bc_sync_bet_info();
			}

			if (is_have_rob_banker)
			{
				bc_rob_banker_info();
				is_have_rob_banker = false;
			}

			adjust_earn_rate();
			compute_reward();
			bc_begin_reward();			//结算奖励以及广播开奖协议
			m_cd_time = 28.0;
			m_game_state = e_game_state::e_state_game_award;
		}
		else
		{
			m_elapse += elapsed;
			if (m_elapse > SYNC_BET_TIME)
			{
				m_elapse = 0.0;
				if (sync_bet_to_room())			//同步押注
				{
					bc_sync_bet_info();
				}
			}
			if (is_have_rob_banker)
			{
				bc_rob_banker_info();
				is_have_rob_banker = false;
			}
		}
	}
	else if (m_game_state == e_game_state::e_state_game_award)		//开奖期间
	{
		m_cd_time -= elapsed;
		if (m_cd_time <= 0)
		{
			add_history_list();	//保存到牌路
			m_game_state = e_game_state::e_state_game_begin;
		}
	}

	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		it->second->heartbeat(elapsed);
	}

	//保存历史记录
	m_checksave += elapsed;
	if (m_checksave > 30)
	{
		saveHistoryRecord();
		m_checksave = 0.0;
	}
}

void logic_room::saveHistoryRecord()
{
	m_db_room_historyPtr->clear_obj();
	for (auto it = m_history_list.begin(); it != m_history_list.end(); ++it)
	{
			auto ptr = HistoryItem::malloc();
			for(int i=0;i<it->result_list_size();i++)
			{
				e_bet_type bet_type=it->result_list().Get(i).type();
				e_bet_result bet_result=it->result_list().Get(i).result();
			
				if(bet_type==e_bettype_forwarddoor)
				{
					ptr->is_forwarddoor_win->set_value(bet_result==e_betresult_win?true:false);
				}else if(bet_type==e_bettype_oppositedoor)
				{
					ptr->is_oppositedoor_win->set_value(bet_result==e_betresult_win?true:false);
				}else if(bet_type==e_bettype_reversedoor)
				{
					ptr->is_reversedoor_win->set_value(bet_result==e_betresult_win?true:false);
				}
			}
			m_db_room_historyPtr->put_obj(ptr);
	}
	m_db_room_history->set_update();//告诉this,你的字段更新了，可以更新
	reflush_rate();
	logic_room_db::store_game_object();
}
void logic_room::InitBetInfo()
{
	m_total_bet_count=0;
	m_room_player_bet_list[e_bettype_forwarddoor]=0;
	m_room_player_bet_list[e_bettype_reversedoor]=0;
	m_room_player_bet_list[e_bettype_oppositedoor]=0;
	m_room_player_bet_list[e_bettype_forward_opposite_door]=0;
	m_room_player_bet_list[e_bettype_forward_reverse_door]=0;
	m_room_player_bet_list[e_bettype_reverse_opposite_door]=0;
	m_room_bet_list[e_bettype_forwarddoor]=0;
	m_room_bet_list[e_bettype_reversedoor]=0;
	m_room_bet_list[e_bettype_oppositedoor]=0;
	m_room_bet_list[e_bettype_forward_opposite_door]=0;
	m_room_bet_list[e_bettype_forward_reverse_door]=0;
	m_room_bet_list[e_bettype_reverse_opposite_door]=0;
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		it->second->clear_once_data();
	}
}
//得到机器人概率（没有引用）
int logic_room::GetPeopleRate(int iMaxNum)
{
	int  iNum = 0;
	__time64_t times;
	errno_t err;
	_time64(&times);
	tm tms;

	if (iMaxNum <= 0)
	{
		return 0;
	}

	err = _localtime64_s(&tms, &times);

	if (tms.tm_hour > 19)
	{
		iNum = iMaxNum / 2 + rand() % (iMaxNum / 2);
	}
	else if (tms.tm_hour > 4 && tms.tm_hour < 7)
	{
		iNum = -(2 + rand() % (iMaxNum / 2));
	}
	else
	{
		iNum = rand() % (iMaxNum / 2);
	}
	return iNum;
}
//设置机器人数量（没有引用）
int logic_room::SetCurrentNeedRobCount()
{
	int robcout=m_cfg->mRobCount;
	currentNeedRobCount=robcout + GetPeopleRate(robcout);
	//std::cout<<"机器人的数量："<<currentNeedRobCount<<std::endl;
	return 0;
}


int logic_room::get_gm_result()
{
	static auto ff = BSON("GmIndex"<<1);
	mongo::BSONObj b = db_game::instance().findone(DB_HAPPYSUPREMACY_ROOM, BSON("room_id"<<m_db_room_id->get_value()), &ff);
	if(!b.isEmpty() && b.hasField("GmIndex"))
	{
		int index = b.getIntField("GmIndex");
		if (index > 0)
		{
			db_game::instance().update(DB_HAPPYSUPREMACY_ROOM, BSON("room_id"<<m_db_room_id->get_value()), BSON("$set" << BSON("GmIndex"<<-1)));
			return index;
		}	
	}
	return 0;
}

void logic_room::adjust_earn_rate()
{
	int index_Stock=0;
	for(int i=0;i<m_StockCFG->mStock.size();i++)
	{
		if(TotalStock->get_value()>=static_cast<GOLD_TYPE>(m_StockCFG->mStock[i]))
		{
			index_Stock=i;
		}else
		{
			break;
		}
	}

	 int rand_1=global_random::instance().rand_int(0, 100);
	 bool or_Cheat_FANGFEN=false;
	 bool or_Cheat_SHOUFEN=false;
	 if(rand_1<= m_StockCFG->mScoreId[index_Stock])
	 {
		 or_Cheat_FANGFEN=true;
	 }

	 set_player_Stock_bet_count();
	 m_core_engine->send_card();
	 GOLD_TYPE Stock_Income=0;
	 GOLD_TYPE Stock_outcome=0;
	 GOLD_TYPE Stock_pre_total=0;
	 get_pre_Stock_Income(&Stock_Income,&Stock_outcome);
	 GOLD_TYPE addIncome=Stock_Income-Stock_outcome;

	 if(GM_CONTROL_COMMAND_LIST.size()>0 || GM_CONTROL_COMMAND==3)//不受库存限制
	 {
		int index=0;
		while (true)
		{
			index++;
			m_core_engine->send_cheat_card(GM_CONTROL_COMMAND_LIST);
			get_pre_Stock_Income(&Stock_Income,&Stock_outcome);
			bool or_all_match=true;
			for(auto& var : GM_CONTROL_COMMAND_LIST)
			{
				if(m_core_engine->get_result_list()[var] !=2)
				{
					or_all_match=false;
					break;
				}
			}
			if(or_all_match)
			{
				//SLOG_CRITICAL<<"GM COMMAND 影响："<<index<<","<<addIncome<<std::endl;
				break;
			}

			if(index>10000)
			{
				SLOG_CRITICAL<<"GM 随机次数太多："<<index<<","<<addIncome<<std::endl;
				break;
			}
		}
	 }
	 else if((or_Cheat_FANGFEN && addIncome>0) || GM_CONTROL_COMMAND==2)//放分
	 {
		int index=0;
		while (true)
		{
			index++;
			m_core_engine->send_cheat_card(false);
			get_pre_Stock_Income(&Stock_Income,&Stock_outcome);
			addIncome=Stock_Income-Stock_outcome;
			if(addIncome<=0)
			{
				//std::cout<<"放分："<<index<<","<<addIncome<<std::endl;
				break;
			}

			if(index>1000)
			{
				SLOG_CRITICAL<<"放分 随机次数太多"<<index<<","<<addIncome<<std::endl;
				break;
			}
		}
	 }

	 addIncome=Stock_Income-Stock_outcome;
	 if(TotalStock->get_value()+addIncome<m_StockCFG->mStock[0])
	 {
		//std::cout<<"突破低分"<<std::endl;
		or_Cheat_SHOUFEN=true;
	 }else
	 {
		//std::cout<<"正常发牌："<<addIncome<<std::endl;
	 }

	 if((or_Cheat_SHOUFEN && addIncome<0) || GM_CONTROL_COMMAND==1)//收分
	 {
		 int index=0;
		 while (true)
		{
			index++;
			m_core_engine->send_cheat_card(true);
			get_pre_Stock_Income(&Stock_Income,&Stock_outcome);
			addIncome=Stock_Income-Stock_outcome;
			if(addIncome>=0)
			{
				//std::cout<<"突破底分，收分："<<index<<","<<addIncome<<std::endl;
				break;
			}

			if(index>1000)
			{
				SLOG_CRITICAL<<"收分 随机次数太多："<<index<<","<<addIncome<<std::endl;
				break;
			}
		}
	 }
	 
	 if(GM_CONTROL_COMMAND>0)
	 {
		 GM_CONTROL_COMMAND=0;
	 }
	 if(GM_CONTROL_COMMAND_LIST.size()>0)
	 {
		 GM_CONTROL_COMMAND_LIST.clear();
	 }
	 set_Stock(Stock_Income,Stock_outcome);//设置库存
}

void logic_room::set_player_Stock_bet_count()
{
	m_room_player_bet_list.clear();
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		if(!it->second->is_robot() && !it->second->get_is_banker())
		{
			std::map<e_bet_type,GOLD_TYPE>& temp_bet = it->second->get_bet_list();
			for (auto& var : temp_bet)
			{
				m_room_player_bet_list[var.first] += var.second;
			}
		}
	}
}

void logic_room::get_pre_Stock_Income(GOLD_TYPE* income,GOLD_TYPE* outcome)
{
	std::map<e_bet_type,e_bet_result>& m_result_list=m_core_engine->get_result_list();

	GOLD_TYPE ToTal_Income=0;

	 std::map<e_bet_type,GOLD_TYPE> temp_result_list;
	 temp_result_list[e_bettype_forwarddoor]=0;
	 temp_result_list[e_bettype_reversedoor]=0;
	 temp_result_list[e_bettype_oppositedoor]=0;
	 temp_result_list[e_bettype_forward_opposite_door]=0;
	 temp_result_list[e_bettype_forward_reverse_door]=0;
	 temp_result_list[e_bettype_reverse_opposite_door]=0;
	if(is_real_banker())//计算机器人的下注
	{
		for(auto& var: m_room_bet_list)
		{
			if(m_room_player_bet_list.find(var.first)!=m_room_player_bet_list.end())
			{
				temp_result_list[var.first]=var.second-m_room_player_bet_list[var.first];
			}else
			{
				temp_result_list[var.first]=var.second;
			}
		}
	}else//计算玩家的下注
	{
		for(auto& var: m_room_player_bet_list)
		{
			temp_result_list[var.first]=var.second;		
		}
	}

	if(	m_result_list[e_bet_type::e_bettype_forwarddoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_forwarddoor]*HappySupremacy_RateCFG::GetSingleton()->GetData(1)->mRate1;
	}

	if(	m_result_list[e_bet_type::e_bettype_oppositedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_oppositedoor]*HappySupremacy_RateCFG::GetSingleton()->GetData(2)->mRate1;
	}

	if(	m_result_list[e_bet_type::e_bettype_reversedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_forward_reverse_door]*HappySupremacy_RateCFG::GetSingleton()->GetData(3)->mRate1;
	}

	if(	m_result_list[e_bet_type::e_bettype_forwarddoor]==e_bet_result::e_betresult_win && m_result_list[e_bet_type::e_bettype_oppositedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_forward_opposite_door]*HappySupremacy_RateCFG::GetSingleton()->GetData(4)->mRate1;
	}else if(m_result_list[e_bet_type::e_bettype_forwarddoor]==e_bet_result::e_betresult_win || m_result_list[e_bet_type::e_bettype_oppositedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_forward_opposite_door]*HappySupremacy_RateCFG::GetSingleton()->GetData(4)->mRate2;
	}

	if(	m_result_list[e_bet_type::e_bettype_reversedoor]==e_bet_result::e_betresult_win && m_result_list[e_bet_type::e_bettype_oppositedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_reverse_opposite_door]*HappySupremacy_RateCFG::GetSingleton()->GetData(5)->mRate1;
	}else if(m_result_list[e_bet_type::e_bettype_reversedoor]==e_bet_result::e_betresult_win || m_result_list[e_bet_type::e_bettype_oppositedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_reverse_opposite_door]*HappySupremacy_RateCFG::GetSingleton()->GetData(5)->mRate2;
	}

	if(	m_result_list[e_bet_type::e_bettype_forwarddoor]==e_bet_result::e_betresult_win && m_result_list[e_bet_type::e_bettype_reversedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_forward_reverse_door]*HappySupremacy_RateCFG::GetSingleton()->GetData(6)->mRate1;
	}else if(m_result_list[e_bet_type::e_bettype_forwarddoor]==e_bet_result::e_betresult_win || m_result_list[e_bet_type::e_bettype_reversedoor]==e_bet_result::e_betresult_win)
	{
		ToTal_Income+=temp_result_list[e_bet_type::e_bettype_forward_reverse_door]*HappySupremacy_RateCFG::GetSingleton()->GetData(6 )->mRate2;
	}

	GOLD_TYPE Total_bet_cout=0;
	for(auto& var :temp_result_list)
	{
		Total_bet_cout+=var.second;
	}

	if(is_real_banker())
	{
		*income=ToTal_Income;
		*outcome=Total_bet_cout;
	}else
	{
		*income=Total_bet_cout;
		*outcome=ToTal_Income;
	}
}

void logic_room::add_history_list()
{
	static int HISTORY_MAX_LENGTH = HappySupremacy_BaseCFG::GetSingleton()->GetData("HistoryMaxCount")->mValue;
	if (m_history_list.size() >= HISTORY_MAX_LENGTH)
	{
		is_refresh_history = true;
		m_history_list.clear();
	}
	else
	{
		is_refresh_history = false;
	}
	std::map<e_bet_type,e_bet_result>& result_list = m_core_engine->get_result_list();
	history_info temp;
	for(auto& it : result_list)
	{
		auto temp1= temp.add_result_list();
		temp1->set_type(it.first);
		temp1->set_result(it.second);
	}
	m_history_list.push_back(temp);
}

msg_type_def::e_msg_result_def logic_room::set_rob_banker(int32_t playerid)
{
	auto temp = playerMap.find(playerid);
	if(temp == playerMap.end())
		return msg_type_def::e_rmt_fail;

	if(playerid == m_now_banker_id)
		return msg_type_def::e_rmt_now_is_banker;

	static int MinBankerCount = HappySupremacy_BaseCFG::GetSingleton()->GetData("MinBankerCount")->mValue;//最小连庄次数
	if (m_now_banker_id != 0 &&m_continue_banker_count < MinBankerCount)//达到最小连庄次数后才可抢庄
		return msg_type_def::e_rmt_fail;

	if (m_rob_banker_id == 0)
		m_rob_banker_cost = m_cfg->mFirstBankerCost;
	else
		m_rob_banker_cost = m_rob_banker_cost + m_cfg->mAddBankerCost;

	if (temp->second->get_gold() < m_cfg->mBankerCondition+m_rob_banker_cost)	//（抢庄+坐庄）金币不足
		return msg_type_def::e_rmt_gold_not_enough;

	if (m_rob_banker_id == playerid)					//如果已抢庄
		return msg_type_def::e_rmt_now_is_you;

	m_rob_banker_id = playerid;

	is_have_rob_banker = true;
	return msg_type_def::e_rmt_success;
}

bool logic_room::sync_bet_to_room()
{
	if (!is_have_bet)
		return false;

	m_room_bet_list.clear();
	m_total_bet_count = 0;
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		std::map<e_bet_type,GOLD_TYPE>& temp_bet = it->second->get_bet_list();
		for (auto& it : temp_bet)
		{
			m_room_bet_list[it.first] += it.second;
			m_total_bet_count += it.second;
		}
	}

	is_have_bet = false;
	return true;
}

uint16_t logic_room::get_room_id()
{
	return m_cfg->mRoomID;
}

void logic_room::set_is_have_bet(bool is_have)
{
	is_have_bet = is_have;
}

bool logic_room::room_is_full()
{
	if (playerMap.size() >= m_cfg->mPlayerMaxCount)
		return true;
	return false;
}

//玩家进入房间
uint16_t logic_room::enter_room(LPlayerPtr player)
{
	if (playerMap.find(player->get_pid()) != playerMap.end())
		return msg_type_def::e_rmt_fail;

	if (playerMap.size() >= m_cfg->mPlayerMaxCount)		//房间已满
		return msg_type_def::e_rmt_room_full;

	if (!player->enter_room(this))
		return msg_type_def::e_rmt_fail;
	
	playerMap.insert(std::make_pair(player->get_pid(), player));

	if (player->is_robot())
		m_rob_count++;

	m_db_enter_count->add_value(1);

	if (!player->is_robot())
		db_log::instance().joingame(player->get_pid(), get_room_id());

	return msg_type_def::e_rmt_success;
}

void logic_room::leave_room(uint32_t playerid)
{
	auto it = playerMap.find(playerid);
	if(it == playerMap.end())
	{
		return;
	}
	if (it->second->get_is_banker())
	{
		it->second->set_is_banker(false);
		set_now_banker_null(playerid);
	}

	if (it->second->is_robot())
	{
		m_rob_count-- ;
	}

	if (!it->second->is_robot())//只记录非机器人的
	{
		db_log::instance().leavegame(playerid);
	}

	playerMap.erase(it);

	for (auto ita = m_banker_list.begin(); ita != m_banker_list.end(); )
	{
		if ((*ita) == playerid)
		{
			ita = m_banker_list.erase(ita);
		}
		else
		{
			++ita;
		}
	}

	if (playerid == m_rob_banker_id)
	{
		m_rob_banker_id = 0;
		bc_rob_banker_info();//
	}
}

const HappySupremacy_RoomCFGData* logic_room::get_room_cfg() const
{
	return m_cfg;
}

//剩余总下注数量
GOLD_TYPE logic_room::get_can_bet_count()
{
	static int BetLimitRate = HappySupremacy_BaseCFG::GetSingleton()->GetData("BetLimitRate")->mValue;
	if (m_now_banker_id != 0)
	{
		auto tempBanker = playerMap.find(m_now_banker_id);
		if(tempBanker != playerMap.end())
		{
			if (m_total_bet_count < tempBanker->second->get_gold()/BetLimitRate)
				return (tempBanker->second->get_gold()/BetLimitRate - m_total_bet_count);
			else
				return 0;			
		}
	}
	return -1;
}

void logic_room::set_now_banker_null(int32_t playerid)
{
	m_old_banker_id = playerid;
	m_now_banker_id = 0;
	is_change_banker = true;
}

msg_type_def::e_msg_result_def logic_room::add_banker_list(uint32_t playerid)
{
	auto temp = playerMap.find(playerid);
	if(temp == playerMap.end())
	{
		return msg_type_def::e_rmt_fail;
	}

	if(playerid == m_now_banker_id)
	{
		return msg_type_def::e_rmt_now_is_banker;
	}
	for (auto it = m_banker_list.begin(); it != m_banker_list.end(); ++it)	//已在列表中
	{
		if ((*it) == playerid)
		{
			return msg_type_def::e_rmt_has_in_banker_list;
		}
	}

	if (temp->second->get_gold() < m_cfg->mBankerCondition)	//金币不足
	{
		return msg_type_def::e_rmt_gold_not_enough;
	}
	if (m_banker_list.size() >= BANKER_MAX_COUNT)		//上庄列表已满
	{
		return msg_type_def::e_rmt_banker_is_full;
	}
	m_banker_list.push_back(playerid);				//加入申请上庄列表

	return msg_type_def::e_rmt_success;
}

//判断是否为抢庄，返回抢庄信息true:抢庄，false:其他庄家信息变化情况
bool logic_room::refreshBanker()
{
	if (m_now_banker_id == 0)
	{
		m_no_banker_count ++;
	}
	else
	{
		m_no_banker_count = 0;
	}

	static int MinBankerCount = HappySupremacy_BaseCFG::GetSingleton()->GetData("MinBankerCount")->mValue;

	is_can_rob_banker = false;
	if (m_rob_banker_id > 0)
	{
		auto temp_player = playerMap.find(m_rob_banker_id);
		if(temp_player != playerMap.end())
		{
			if (temp_player->second->set_is_banker(true))
			{
				if (m_now_banker_id > 0)
				{
					auto temp_player2 = playerMap.find(m_now_banker_id);
					if(temp_player2 != playerMap.end())
					{
						temp_player2->second->set_is_banker(false);
					}
				}

				m_old_banker_id = m_now_banker_id;
				m_now_banker_id = m_rob_banker_id;
				m_rob_banker_id = 0;

				temp_player->second->quest_change(502,1); //累计连庄次数

				m_continue_banker_count = 1;		//成功上庄
				m_system_draw_water = 0;
				m_banker_total_win = 0;
				is_change_banker = true;
				is_have_banker = true;

				temp_player->second->change_gold2(-m_rob_banker_cost,33);

				//如果申请列表里有，就干掉
				for (auto ita = m_banker_list.begin(); ita != m_banker_list.end(); )
				{
					if ((*ita) == m_now_banker_id)
					{
						ita = m_banker_list.erase(ita);
					}
					else
					{
						++ita;
					}
				}

				return true;
			}
		}
	}
	m_rob_banker_id = 0;

	if (m_now_banker_id > 0)	//有庄家时判断是否能够继续坐庄
	{
		auto tempBanker = playerMap.find(m_now_banker_id);
		if(tempBanker != playerMap.end())
		{
			if (tempBanker->second->get_game_state() == e_ps_disconnect && m_continue_banker_count >= MinBankerCount)
			{
				m_old_banker_id = m_now_banker_id;
				m_now_banker_id = 0;
				tempBanker->second->set_is_banker(false);
				is_change_banker = true;
				is_have_banker = false;
			}
			else if (tempBanker->second->is_robot() && m_continue_banker_count > global_random::instance().rand_int(10,30))
			{
				m_old_banker_id = m_now_banker_id;
				m_now_banker_id = 0;
				tempBanker->second->set_is_banker(false);
				is_change_banker = true;
				is_have_banker = false;
			}
			else
			{
				//满足继续上庄的条件
				if (tempBanker->second->get_gold() >= m_cfg->mAutoLeaveBanker)
				{
					m_continue_banker_count++;		//继续连庄
					is_have_banker = true;

					tempBanker->second->quest_change(502,1); //累计连庄次数
				}
				else//金币不足自动下庄
				{
					m_old_banker_id = m_now_banker_id;
					m_now_banker_id = 0;
					m_system_draw_water = 0;
					m_banker_total_win = 0;
					tempBanker->second->set_is_banker(false);
					is_change_banker = true;
					is_have_banker = false;
				}
			}
		}
		else
		{
			m_now_banker_id = 0;
		}
	}

	if (m_now_banker_id == 0)		//当前无庄家
	{
		is_have_banker = false;
		for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
		{
			it->second->set_is_banker(false);
		}

		if (m_banker_list.empty())
		{
			m_continue_banker_count = 0;
			is_can_rob_banker = true;
			return false;
		}

		while (true)
		{
			if (m_banker_list.empty())
			{
				m_now_banker_id = 0;
				m_continue_banker_count = 0;
				is_change_banker = true;
				is_have_banker = false;
				break;
			}

			m_now_banker_id = m_banker_list.front();
			m_banker_list.pop_front();

			auto temp_player = playerMap.find(m_now_banker_id);
			if(temp_player == playerMap.end())
				continue;

			if (temp_player->second->set_is_banker(true))
			{
				temp_player->second->quest_change(502,1); //累计连庄次数

				m_continue_banker_count = 1;		//成功上庄
				m_system_draw_water = 0;
				m_banker_total_win = 0;
				is_change_banker = true;
				is_have_banker = true;
				break;
			}
			else
				continue;
		}
	}
	
	if (m_now_banker_id == 0 || m_continue_banker_count >= MinBankerCount)
	{
		is_can_rob_banker = true;
	}
	return false;
}

//---------------------------------------------协议相关---------------------------------------------
//通知开始下注
void logic_room::bc_begin_bet()
{
	if (is_gm)
	{
		return;
	}
	auto sendmsg = PACKET_CREATE(packetl2c_bc_begin_bet, e_mst_l2c_bc_begin_bet);
	sendmsg->set_is_can_rob_banker(is_can_rob_banker);
	broadcast_msg_to_client(sendmsg);
}

void logic_room::compute_reward()		//计算各个玩家的奖励()
{
	//初始化房间收益
	GOLD_TYPE real_income = 0;
	GOLD_TYPE real_outcome = 0;
	m_once_income = m_total_bet_count;		
	m_once_outcome = 0;	

	//计算真正的收入
	for (auto& it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		if (it->second->is_robot())
		{
			continue;
		}
		std::map<e_bet_type,GOLD_TYPE>& temp_bet = it->second->get_bet_list();
		for (auto& id:temp_bet)
		{
			real_income += temp_bet[id.first];
		}
	}
	
	//玩家收益
	map<e_bet_type,e_bet_result>& result_list = m_core_engine->get_result_list();
	int32_t win_count = 0;
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		if (it->second->get_is_banker())
		{
			continue;
		}
		win_count = 0;
		std::map<e_bet_type,GOLD_TYPE>& temp_bet = it->second->get_bet_list();
		int32_t total_bet = 0;
		for (auto& a : result_list)
		{
			total_bet += temp_bet[a.first];

		    if(a.second==e_betresult_lose)
			{
				win_count+=0;
			}else if(a.second==e_betresult_win)
			{
				win_count +=temp_bet[a.first]*2;		//赢得话就赢2倍的下注金额
			}else if(a.second==e_betresult_nowin_nolose)
			{
				win_count +=temp_bet[a.first];			//不输不赢1倍的下注金额
			}
		}

		if (win_count > 0)
		{
			it->second->add_bet_win(win_count);//玩家金钱变化

			m_once_outcome += win_count;//房间支出（包括机器人的）
			if (!it->second->is_robot())
			{
				real_outcome += win_count;//真正的收入是不包括机器人的
			}
			if (it->second->is_robot() && is_real_banker())
			{
				m_db_rob_income->add_value(win_count);
			}
		}
	}

	//庄家收益
	m_banker_once_win = m_once_income - m_once_outcome;
	//房间收益
	if (is_have_banker)
	{
		m_banker_total_win += m_banker_once_win;
		m_system_draw_water = m_banker_total_win * m_draw_water_rate;//系统抽水收益

		if (m_banker_once_win > 0)
		{
			auto tempBanker = playerMap.find(m_now_banker_id);
			if(tempBanker != playerMap.end())
			{
				tempBanker->second->add_star_lottery_info(0,1);
			}
		}
	}else
	{
		m_db_room_income->add_value(real_income);
		m_lobby->addTodayIncome(m_cfg->mRoomID,real_income);

		m_db_room_outcome->add_value(real_outcome);
		m_lobby->addTodayOutlay(m_cfg->mRoomID,real_outcome);
	}

	//计算机器人赢得概率
	m_rob_earn_rate = 0.0;
	if ((m_db_rob_income->get_value() - m_db_rob_outcome->get_value()) !=0 && m_db_rob_income->get_value() != 0)
	{
		m_rob_earn_rate = static_cast<double>(m_db_rob_income->get_value() - m_db_rob_outcome->get_value())/m_db_rob_income->get_value();
	}

	//房间里的所有玩家同步金币
	for(auto it=playerMap.begin();it!=playerMap.end();it++)
	{
		if (it->second->get_is_banker())
		{
			it->second->change_gold(m_banker_once_win);
		}
		it->second->sycn_gold();
	}
}

void logic_room::set_Stock(GOLD_TYPE Income,GOLD_TYPE outcome)//设置库存
{
	GOLD_TYPE add_value=0;
	if(Income!=0 || outcome!=0)
	{
		GOLD_TYPE add_value=0;
		if(!is_real_banker())//计算非机器人时，才抽水
		{
			//std::cout<<"当前盈利率"<<EarningsRate->get_value()<<std::endl;
			add_value=floor(Income*EarningsRate->get_value()+0.5);
			TotalProfit->add_value(add_value);//这才是系统想要的收益
		}
		TotalStock->add_value(Income-add_value-outcome);
	}
	/*auto sendmsg = PACKET_CREATE(packetl2c_notice_gm_stock_info,e_mst_l2c_notice_gm_stock_info);
	sendmsg->set_stock_total_count(TotalStock->get_value());
	sendmsg->set_stock_add_count(add_value);

	sendmsg->set_stock_earnrate(EarningsRate->get_value());
	sendmsg->set_stock_total_earnrate_income(TotalProfit->get_value());
	sendmsg->set_stock_add_earnrate_income(add_value);

	for(auto& var :playerMap)
	{
		if(var.second->is_GM_CONTROL())
		{
			var.second->send_msg_to_client(sendmsg);
		}
	}*/
}

void logic_room::bc_begin_reward()
{
	map<e_bet_type,e_bet_result>& result_list = m_core_engine->get_result_list();
	std::vector<CardInfo>& player_card = m_core_engine->get_sort_player_card();
	std::map<e_card_owner,CombinePointInfo>& result_point_list = m_core_engine->get_result_point_list();
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		auto sendmsg = PACKET_CREATE(packetl2c_bc_begin_award, e_mst_l2c_bc_begin_award);

		sendmsg->mutable_result_list()->Reserve(result_list.size());
		for (auto& it : result_list)
		{
			msg_bet_result_info* mbetresultInfo=sendmsg->add_result_list();;
			mbetresultInfo->set_type(it.first);
			mbetresultInfo->set_result(it.second);
		}

		for (auto& it1 : player_card)
		{
			auto temp_player = sendmsg->add_player_card();
			temp_player->set_card_flower(it1.card.flower);
			temp_player->set_card_point(it1.card.point);
			temp_player->set_owner(it1.owner);
		}

		for(auto & it: result_point_list)
		{
			auto temp=sendmsg->add_result_point_info();
			temp->set_owner(it.first);
			temp->set_combinecardid(it.second.combineCards_Id);
			//std::cout<<"分数:"<<(int)it.second.combineCard_point<<std::endl;
		}

		sendmsg->set_banker_win_gold(m_banker_once_win);
		sendmsg->set_self_win_gold(it->second->get_bet_win());

		if (it->second->get_is_banker())
		{
			sendmsg->set_banker_gold_count(it->second->get_gold());
		}
		sendmsg->set_is_refresh_history(is_refresh_history);
		it->second->send_msg_to_client(sendmsg);
	}
}

void logic_room::bc_sync_bet_info()
{
	if (is_gm)
		return;

	auto sendmsg = PACKET_CREATE(packetl2c_bc_total_bet_info, e_mst_l2c_bc_total_bet_info);
	sendmsg->mutable_bet_info_list()->Reserve(MAX_BET_COUNT);
	for (auto& it : m_room_bet_list)
	{
		msg_bet_info* info= sendmsg->add_bet_info_list();
		info->set_type(it.first);
		info->set_bet_count(it.second);
	}
	broadcast_msg_to_client(sendmsg);

	/*auto sendmsg1 = PACKET_CREATE(packetl2c_notice_gm_all_bet_info, e_mst_l2c_notice_gm_all_bet_info);
	for(auto& it : playerMap)
	{
		if(it.second->get_bet_total_count()>0 && !it.second->is_robot() && !it.second->get_is_banker())
		{
			gm_msg_bet_info* gm_bet_info=sendmsg1->add_bet_info_list();
			auto p_list=it.second->get_bet_list();
			for(auto& it1 : p_list)
			{
				if(it1.second>0)
				{
					msg_bet_info* info= gm_bet_info->add_bet_info();
					info->set_type(it1.first);
					info->set_bet_count(it1.second);
				}
			}

			gm_bet_info->mutable_player_info()->set_player_id(it.second->get_pid());
			gm_bet_info->mutable_player_info()->set_player_nickname(it.second->get_nickname());
			gm_bet_info->mutable_player_info()->set_player_head_frame(it.second->get_head_frame_id());
			gm_bet_info->mutable_player_info()->set_player_head_custom(it.second->get_icon_custom());
			gm_bet_info->mutable_player_info()->set_player_gold(it.second->get_gold());
			gm_bet_info->mutable_player_info()->set_player_sex(it.second->get_player_sex());
			gm_bet_info->mutable_player_info()->set_player_vip_lv(it.second->get_viplvl());
		}
	}

	for(auto& var :playerMap)
	{
		if(var.second->is_GM_CONTROL())
		{
			var.second->send_msg_to_client(sendmsg1);
		}
	}*/
}

void logic_room::bc_rob_banker_info()
{
	if (is_gm)
		return;

	auto sendmsg = PACKET_CREATE(packetl2c_bc_rob_banker_info, e_mst_l2c_bc_rob_banker_info);
	sendmsg->set_player_id(m_rob_banker_id);
	sendmsg->set_pay_count(m_rob_banker_cost);
	broadcast_msg_to_client(sendmsg);
}

void logic_room::bc_change_banker(bool is_rob)
{
	if (is_gm)
		return;

	if (is_change_banker)
	{
		auto sendmsg = PACKET_CREATE(packetl2c_bc_change_banker, e_mst_l2c_bc_change_banker);
		auto banker_info = sendmsg->mutable_banker_info();
		bool is_system_banker = true;
		if (m_now_banker_id > 0)
		{
			auto temp_banker = playerMap.find(m_now_banker_id);
			if (temp_banker != playerMap.end())
			{
				banker_info->set_player_id(m_now_banker_id);
				banker_info->set_player_nickname(temp_banker->second->get_nickname());
				banker_info->set_player_head_frame(temp_banker->second->get_head_frame_id());
				banker_info->set_player_head_custom(temp_banker->second->get_icon_custom());
				banker_info->set_player_gold(temp_banker->second->get_gold());
				banker_info->set_player_sex(temp_banker->second->get_player_sex());
				banker_info->set_player_vip_lv(temp_banker->second->get_viplvl());

				is_system_banker = false;
			}
		}
		if (is_system_banker)
		{
			banker_info->set_player_id(0);
			banker_info->set_player_nickname("");
			banker_info->set_player_head_frame(0);
			banker_info->set_player_head_custom("");
			banker_info->set_player_gold(0);
			banker_info->set_player_sex(1);
			banker_info->set_player_vip_lv(0);
		}
		sendmsg->set_is_rob(is_rob);
		if (m_system_draw_water > 0)
		{
			auto old_banker = playerMap.find(m_old_banker_id);
			if (old_banker != playerMap.end())
			{
				if (!old_banker->second->is_robot())
				{
					m_lobby->addTodayIncome(m_cfg->mRoomID,m_system_draw_water);
					m_db_player_charge->add_value(m_system_draw_water);
				}
			}
			sendmsg->set_system_draw_water(m_system_draw_water);
		}
		else
			sendmsg->set_system_draw_water(0);
		m_system_draw_water = 0;

		if (m_old_banker_id > 0)
		{
			sendmsg->set_old_banker_id(m_old_banker_id);
			m_old_banker_id = 0;
		}

		broadcast_msg_to_client(sendmsg);
		is_change_banker = false;
	}
}

int logic_room::broadcast_msg_to_client(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint32_t except_id)
{
	if(playerMap.size() <= 0)
		return -1;

	std::vector<uint32_t> pids;
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		pids.push_back(it->second->get_pid());
	}

	return game_engine::instance().get_handler()->broadcast_msg_to_client(pids, packet_id, msg);
}

boost::shared_ptr<game_happysupremacy_protocols::packetl2c_get_room_scene_info_result> logic_room::get_room_scene_info()
{
	auto sendmsg = PACKET_CREATE(game_happysupremacy_protocols::packetl2c_get_room_scene_info_result, game_happysupremacy_protocols::e_mst_l2c_get_room_scene_info);
	sendmsg->set_room_id(m_cfg->mRoomID);
	sendmsg->set_room_state(m_game_state);
	sendmsg->set_cd_time(m_cd_time);
	sendmsg->mutable_bet_info_list()->Reserve(m_room_bet_list.size());
	for (auto& it : m_room_bet_list)
	{
		msg_bet_info* m_betInfo=sendmsg->add_bet_info_list();
		m_betInfo->set_type(it.first);
		m_betInfo->set_bet_count(it.second);
	}
	sendmsg->set_remain_card_count(m_core_engine->get_remain_card_count());
	sendmsg->set_banker_continue_count(m_continue_banker_count);
	sendmsg->mutable_history_list()->Reserve(m_history_list.size());

	//得到历史记录
	for (auto it = m_history_list.begin(); it != m_history_list.end(); ++it)
	{
		auto temp = sendmsg->add_history_list();
		for(int i=0;i<it->result_list_size();i++)
		{
			auto temp1= temp->add_result_list();
			temp1->set_type(it->result_list().Get(i).type());
			temp1->set_result(it->result_list().Get(i).result());
		}
	}

	sendmsg->set_banker_win_gold(m_banker_total_win);
	sendmsg->set_is_can_rob_banker(is_can_rob_banker);
	auto banker_info = sendmsg->mutable_banker_info();

	bool is_system_banker = true;
	if (m_now_banker_id > 0)
	{
		auto temp_banker = playerMap.find(m_now_banker_id);
		if (temp_banker != playerMap.end())
		{
			banker_info->set_player_id(m_now_banker_id);
			banker_info->set_player_nickname(temp_banker->second->get_nickname());
			banker_info->set_player_head_frame(temp_banker->second->get_head_frame_id());
			banker_info->set_player_head_custom(temp_banker->second->get_icon_custom());
			banker_info->set_player_gold(temp_banker->second->get_gold());
			banker_info->set_player_sex(temp_banker->second->get_player_sex());
			banker_info->set_player_vip_lv(temp_banker->second->get_viplvl());

			is_system_banker = false;
		}
	}
	if (is_system_banker)
	{
		banker_info->set_player_id(0);
		banker_info->set_player_nickname("");
		banker_info->set_player_head_frame(0);
		banker_info->set_player_head_custom("");
		banker_info->set_player_gold(0);
		banker_info->set_player_sex(1);
		banker_info->set_player_vip_lv(0);
	}

	return sendmsg;
}

boost::shared_ptr<game_happysupremacy_protocols::packetl2c_ask_for_player_list_result> logic_room::get_room_player_list()
{
	auto sendmsg = PACKET_CREATE(game_happysupremacy_protocols::packetl2c_ask_for_player_list_result, game_happysupremacy_protocols::e_mst_l2c_ask_player_list);
	sendmsg->mutable_player_list()->Reserve(playerMap.size());
	for (auto it = playerMap.begin(); it != playerMap.end(); ++it)
	{
		auto temp = sendmsg->add_player_list();
		temp->set_player_id(it->second->get_pid());
		temp->set_player_nickname(it->second->get_nickname());
		temp->set_player_head_frame(it->second->get_head_frame_id());
		temp->set_player_head_custom(it->second->get_icon_custom());
		temp->set_player_gold(it->second->get_gold());
		temp->set_player_sex(it->second->get_player_sex());
		temp->set_player_vip_lv(it->second->get_viplvl());
	}

	return sendmsg;
}

boost::shared_ptr<game_happysupremacy_protocols::packetl2c_ask_for_banker_list_result> logic_room::get_room_banker_list()
{
	auto sendmsg = PACKET_CREATE(game_happysupremacy_protocols::packetl2c_ask_for_banker_list_result, game_happysupremacy_protocols::e_mst_l2c_ask_banker_list);
	sendmsg->mutable_banker_list()->Reserve(m_banker_list.size());
	for (auto it = m_banker_list.begin(); it != m_banker_list.end(); ++it)
	{
		auto tempPlayer = playerMap.find(*it);
		if (tempPlayer != playerMap.end())
		{
			auto temp = sendmsg->add_banker_list();
			temp->set_player_id(tempPlayer->second->get_pid());
			temp->set_player_nickname(tempPlayer->second->get_nickname());
			temp->set_player_head_frame(tempPlayer->second->get_head_frame_id());
			temp->set_player_head_custom(tempPlayer->second->get_icon_custom());
			temp->set_player_gold(tempPlayer->second->get_gold());
			temp->set_player_sex(tempPlayer->second->get_player_sex());
			temp->set_player_vip_lv(tempPlayer->second->get_viplvl());
		}
	}

	return sendmsg;
}

boost::shared_ptr<game_happysupremacy_protocols::packetl2c_ask_for_history_list_result> logic_room::get_room_history_list()
{
	auto sendmsg = PACKET_CREATE(game_happysupremacy_protocols::packetl2c_ask_for_history_list_result, game_happysupremacy_protocols::e_mst_l2c_ask_history_list);
	sendmsg->mutable_history_list()->Reserve(m_history_list.size());
	for (auto it = m_history_list.begin(); it != m_history_list.end(); ++it)
	{
		auto temp = sendmsg->add_history_list();
		for(int i=0;i<it->result_list_size();i++)
		{
			auto temp1= temp->add_result_list();
			temp1->set_type(it->result_list().Get(i).type());
			temp1->set_result(it->result_list().Get(i).result());
		}
	}

	return sendmsg;
}

void logic_room::set_gm(int count)
{
	is_gm = true;
	gm_index = 0;
	gm_max = count;
}

bool logic_room::is_real_banker()
{
	if (m_now_banker_id == 0)
		return false;
	auto tempBanker = playerMap.find(m_now_banker_id);
	if (tempBanker != playerMap.end())
	{
		if(!tempBanker->second->is_robot())
			return true;
	}
	return false;
}

//废弃的方法
int32_t logic_room::get_win_index()
{
	int32_t index = -1;
	/*std::map<e_bet_type,e_bet_result>& result_list = m_core_engine->get_result_list();
	if (result_list[e_bettype_forwarddoor])
	{
		index = 1;
	}
	else if (result_list[4])
	{
		index = 4;
	}*/

	return index;
}

int32_t logic_room::get_no_banker_count()
{
	return m_no_banker_count;
}






