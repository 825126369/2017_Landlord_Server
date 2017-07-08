#include "stdafx.h"

#include <enable_random.h>
#include <net/packet_manager.h>
#include <memory>

#include "game_db.h"
#include "game_engine.h"
#include "logic_player.h"
#include "logic_room.h"
#include "logic_table.h"
#include "msg_type_def.pb.h"
#include "ShowHand_RoomCFG.h"
#include "ShowHand_BaseCFG.h"
#include "ShowHand_RoomStockCFG.h"
#include "cardmanager.h"
#include "game_db_log.h"

#include "game_showhand_def.pb.h"
#include "game_showhand_protocol.pb.h"

using namespace game_showhand_protocols;
SHOWHAND_SPACE_BEGIN

static const double g_one_minute = 60;      // = 60秒

logic_room::logic_room(const ShowHand_RoomCFGData* cfg):
        m_check_rate(0.0),
		m_checksave(0.0),
        m_room_id(0),
		m_rob_count(0)
		,IsOpenRob(false)
		,m_rob_cd(0)
		,rob_count(0)
		,rob_count_cd(0.0)
		,Stock_add_value(0)
		,Profit_add_value(0)
{
    if (cfg) 
	{
        m_room_id       = cfg->mRoomId;
        m_room_name     = cfg->mRoomName;
        m_is_open       = cfg->mIsOpen;
        m_ante          = cfg->mAnte;
        m_room_enter_gold_condition = cfg->mEnterGoldCondition;
		m_table_enter_gold_condition=cfg->mEnterTableGoldCondition;
        m_room_max_player_count = cfg->mRoomMaxPlayerCount;
        m_room_max_table_count  = cfg->mTableMaxPlayerCount;
        m_illegal_quit_gold_count = cfg->mIllegalQuitGoldCount;
		IsOpenRob=ShowHand_BaseCFG::GetSingleton()->GetData("IsOpenRob")->mValue>0;
		m_rob_earn_rate=ShowHand_RoomCFG::GetSingleton()->GetData(get_room_id())->mRoomAndroidYield;
    }

	init_tables();

	init_game_object();
	m_db_RoomID->set_value(cfg->mRoomId);
	if(!load_room())
	{
		create_room();
	}
}

void logic_room::release()
{
	m_room_players.clear();
	m_tables.clear();
}

void logic_room::init_tables()
{
    for (int tid = 1; tid <= m_room_max_table_count; ++tid) 
	{
		m_tables.insert(std::make_pair(tid, boost::make_shared<logic_table>(this,tid)));
    }
}

logic_room::~logic_room(void) {
}

void logic_room::heartbeat( double elapsed ) 
{
	if (IsOpenRob)//创建机器人
	{
		static double robotEnterTime=3.0;
		m_rob_cd += elapsed;
		if(m_rob_cd > robotEnterTime)
		{
			create_robot();

			static int RobEnterMinTime=ShowHand_BaseCFG::GetSingleton()->GetData("RobEnterMinTime")->mValue;
			static int RobEnterMaxTime=ShowHand_BaseCFG::GetSingleton()->GetData("RobEnterMaxTime")->mValue;
			robotEnterTime = global_random::instance().rand_int(RobEnterMinTime,RobEnterMaxTime);
			m_rob_cd=0.0;
		}
	}
	for (auto it = m_tables.begin(); it != m_tables.end(); ++it) 
	{
		it->second->heartbeat(elapsed);
	}

	//每局保存一次到数据库，防止数据丢失
	m_checksave += elapsed;
	if (m_checksave > 30)
	{
		store_game_object();
		m_checksave = 0.0;
	}

	/*rob_count_cd+=elapsed;
	if(rob_count_cd>5 && get_room_id()==1)
	{
		rob_count=0;
		for(auto& i:m_room_players)
		{
			if(i.second->is_android())
			{
				rob_count++;
			}
		}
		std::cout<<"房间机器人数量："<<get_room_id()<<","<<rob_count<<","<<m_rob_count<<std::endl;
		rob_count_cd=0.0;
	}*/
}

void logic_room::add_rob_income(GOLD_TYPE income)
{
	m_db_rob_income->add_value(income);
}

void logic_room::add_rob_outcome(GOLD_TYPE outcome)
{
	m_db_rob_outcome->add_value(outcome);
}

void logic_room::set_rob_earn_rate()
{
	if (m_db_rob_income->get_value() != 0)
	{
		m_db_rob_EarningsRate->set_value(static_cast<double>(m_db_rob_income->get_value() - m_db_rob_outcome->get_value())/m_db_rob_income->get_value());
	}
}

double logic_room::get_rob_earn_rate()
{
	return m_db_rob_EarningsRate->get_value();
}

void logic_room::add_Stock_income(GOLD_TYPE Income,GOLD_TYPE outcome)//可正可负
{
	Stock_add_value=0;
	Profit_add_value=0;
	if(Income!=0 || outcome!=0)
	{
		Profit_add_value=floor(Income*EarningsRate->get_value()+0.5);
		TotalProfit->add_value(Stock_add_value);//这才是系统想要的收益
		GOLD_TYPE now_Stock=Income-Stock_add_value-outcome;
		Stock_add_value=now_Stock-TotalStock->get_value();
		TotalStock->set_value(now_Stock);
	}
}

void logic_room::do_protobuf_notice_Stock_Info(logic_player* player)
{
	auto sendmsg = PACKET_CREATE(packetl2c_notice_gm_stock_info,e_mst_l2c_notice_gm_stock_info);
	sendmsg->set_stock_total_count(TotalStock->get_value());
	sendmsg->set_stock_add_count(Stock_add_value);

	sendmsg->set_stock_earnrate(EarningsRate->get_value());
	sendmsg->set_stock_total_earnrate_income(TotalProfit->get_value());
	sendmsg->set_stock_add_earnrate_income(Profit_add_value);

	if(player==nullptr)
	{
		for(auto& var :m_room_players)
		{
			if(var.second->is_GM_CONTROL())
			{
				var.second->send_msg_to_client(sendmsg);
			}
		}
	}else if(player->is_GM_CONTROL())
	{
		player->send_msg_to_client(sendmsg);
	}

	Stock_add_value=0;
	Profit_add_value=0;
}

uint16_t logic_room::get_player_cout() 
{
	return m_room_players.size();
}

int logic_room::can_enter_room(LPlayerPtr p) {
    if (! p)//判断指针是否为空
        return msg_type_def::e_rmt_player_not_exists;

    if (p->get_gold() < m_room_enter_gold_condition)//进入房间金币条件
		return msg_type_def::e_rmt_gold_not_enough;

    if (!m_is_open)//房间是否开放
        return msg_type_def::e_rmt_room_notopen;

    return msg_type_def::e_rmt_success;  
}

//玩家进入房间
int logic_room::enter_room(LPlayerPtr p) {
     if (! p)
        return msg_type_def::e_rmt_player_not_exists;

    if (p->get_gold() < m_room_enter_gold_condition)
        return msg_type_def::e_rmt_gold_not_enough;

    if (!m_is_open)
        return msg_type_def::e_rmt_room_notopen;

	if(p->is_android())
	{
		m_rob_count++;
	}

    m_room_players.insert(LPLAYER_MAP::value_type(p->get_pid(), p));

	if (!p->is_android())
	{
		db_log::instance().joingame(p->get_pid(), get_room_id());
	}
    return msg_type_def::e_rmt_success;
}

//玩家离开房间
int logic_room::leave_room(logic_player* player) {
    LPLAYER_MAP::iterator it = m_room_players.find(player->get_pid());

    if (it == m_room_players.end())
		return msg_type_def::e_rmt_fail;   
	
	if(player->is_android())
	{
		m_rob_count--;
	}

	if (!player->is_android())
	{
		db_log::instance().leavegame(player->get_pid());
	}
    m_room_players.erase(it);

	return msg_type_def::e_msg_result_def::e_rmt_success;
}

LTABLE_MAP &logic_room::get_all_tables() {
    return m_tables;
}

LTablePtr logic_room::get_table(int table_id) 
{
	if (table_id==0)
        return LTablePtr(NULL);
	if (m_tables.find(table_id)!=m_tables.end())
		return m_tables[table_id];
    return LTablePtr(NULL);
}
//---------------------------------数据库------------------------------------------------------------

void logic_room::create_room()
{
	m_db_TotalIncome->set_value(0);		//当前收入
	m_db_TotalOutlay->set_value(0);		//当前消耗
	m_db_EnterCount->set_value(0);	   //进入次数

	m_db_rob_income->set_value(0); 	//机器人收入
	m_db_rob_outcome->set_value(0);    //机器人支出
	m_db_rob_EarningsRate->set_value(0.0);//机器人盈利率

	GOLD_TYPE defaultStock=ShowHand_RoomStockCFG::GetSingleton()->GetData(get_room_id())->mDefaultStock;
	TotalStock->set_value(defaultStock);

	double earnRate=ShowHand_RoomStockCFG::GetSingleton()->GetData(get_room_id())->mDeduct;
	EarningsRate->set_value(earnRate);

	TotalProfit->set_value(0);

}
bool logic_room::load_room()
{
	mongo::BSONObj b = db_game::instance().findone(DB_SHOWHAND_ROOM, BSON("room_id"<<m_db_RoomID->get_value()));
	if(b.isEmpty())
		return false;	

	return from_bson(b);
}

void logic_room::init_game_object() {
	m_db_RoomID = CONVERT_POINT(Tfield<int16_t>, regedit_tfield(e_got_int16, "room_id"));

	m_db_TotalIncome = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "TotalIncome"));
	m_db_TotalOutlay = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "TotalOutlay"));
	m_db_EnterCount = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "EnterCount"));

	m_db_rob_EarningsRate = CONVERT_POINT(Tfield<double>, regedit_tfield(e_got_double, "rob_EarningsRate"));
	m_db_rob_income = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "rob_income"));
	m_db_rob_outcome = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "rob_outcome"));


	EarningsRate = CONVERT_POINT(Tfield<double>, regedit_tfield(e_got_double, "EarningsRate"));
	TotalStock = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "TotalStock"));
	TotalProfit = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "TotalProfit"));
}

bool logic_room::store_game_object(bool to_all)
{
	if(!has_update())
		return true;

	auto err = db_game::instance().update(DB_SHOWHAND_ROOM, BSON("room_id"<<m_db_RoomID->get_value()), BSON("$set"<<to_bson(to_all)));
	if(!err.empty())
	{
		SLOG_ERROR << "logic_room::store_game_object :" <<err;
		return false;
	}
	return true;
}


//---------------------------------------返回Protobuf协议----------------------------------------
//返回所有桌子列表信息
void logic_room::copy_tablelist(google::protobuf::RepeatedPtrField< ::game_showhand_protocols::msg_table_info >* table_list)
{
	for (auto it = m_tables.begin(); it != m_tables.end(); ++it) 
	{
		if(it->second->is_empty())
			continue;

		it->second->copy_table_info(table_list->Add());
	}
}

//找到一个适合的桌子
int logic_room::match_auto_table(int last_select_table_id)
{
	std::vector<int> mids;
	for(auto& a : m_tables)
	{
		if(a.second->get_player_count()==1 && a.first!=last_select_table_id)
		{
			mids.push_back(a.first);
		}
	}
	if(mids.size()==0)
	{
		for(auto& a : m_tables)
		{
			if(a.second->get_player_count()==0 && a.first!=last_select_table_id)
			{
				mids.push_back(a.first);
			}
		}
	}
	if(mids.size()>0)
	{
		std::random_shuffle(mids.begin(),mids.end());
		return mids[0];
	}
	return 0;
}

//找到一个适合的桌子
int logic_room::rob_match_auto_table()
{
	std::vector<int> mids;
	for(auto& a : m_tables)
	{
		if(a.first<=20 && a.second->get_player_count()==0)
		{
			mids.push_back(a.first);
		}
	}
	std::random_shuffle(mids.begin(),mids.end());
	if(mids.size()>0)
	{
		return mids[0];
	}
	return 0;
}

logic_player* logic_room::get_room_Idle_robot()
{
	for(auto& a : m_room_players)
	{
		if(!a.second->is_inTable() && a.second->is_android())
		{
			return a.second.get();
		}
	}
	return nullptr;
}

GOLD_TYPE logic_room::get_EnterGold()
{
	return m_room_enter_gold_condition;
}

GOLD_TYPE logic_room::get_EnterTableGold()
{
	return m_table_enter_gold_condition;
}

//通知 玩家进入
void logic_room::do_protobuf_notice_enter_table(logic_player* player) 
{
	auto sendmsg = PACKET_CREATE(game_showhand_protocols::packetl2c_notice_join_table , game_showhand_protocols::e_mst_l2c_notice_join_table_message);
	sendmsg->set_table_id(player->get_table()->get_id());
	player->copy_player_info(sendmsg->mutable_player_info());
	broast_msg_to_room_layers(sendmsg, player->get_pid());

	//std::cout<<"通知 房间 有人进入桌子："<<player->get_pid()<<std::endl;
}

//通知 玩家离开
void logic_room::do_protobuf_notice_leave_table(int table_id, int pid) 
{
	auto sendmsg = PACKET_CREATE(game_showhand_protocols::packetl2c_notice_leave_table , game_showhand_protocols::e_mst_l2c_notice_leave_table_message);
	sendmsg->set_table_id(table_id);
	sendmsg->set_player_id(pid);
	broast_msg_to_room_layers(sendmsg, pid);

	//std::cout<<"通知 房间 有人离开桌子："<<pid<<std::endl;
}

//通知桌子里玩家的状态
void logic_room::do_protobuf_notice_table_player_state(logic_table* table) 
{
	auto sendmsg = PACKET_CREATE(game_showhand_protocols::packetl2c_notice_table_player_state , game_showhand_protocols::e_mst_l2c_notice_table_player_message);
	sendmsg->set_table_id(table->get_id());
	if(table->get_player_left()!=nullptr)
	{
		sendmsg->set_left_state(table->get_player_left()->get_player_table_state());
	}
	if(table->get_player_right()!=nullptr)
	{
		sendmsg->set_right_state(table->get_player_right()->get_player_table_state());
	}
	broast_msg_to_room_layers(sendmsg,0);

	//std::cout<<"通知 房间 有人离开桌子："<<pid<<std::endl;
}

SHOWHAND_SPACE_END