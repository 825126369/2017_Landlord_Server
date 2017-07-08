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

void logic_room_db::create_room()
{
	m_db_room_income->set_value(0);
	m_db_room_outcome->set_value(0);
	m_db_enter_count->set_value(0);
	m_db_ExpectEarnRate->set_value(0.05);//默认值
	m_db_player_charge->set_value(0);

	m_db_rob_income->set_value(0);
	m_db_rob_outcome->set_value(0);

	GOLD_TYPE defaultStock=HappySupremacy_RoomStockCFG::GetSingleton()->GetData(m_db_room_id->get_value())->mDefaultStock;
	TotalStock->set_value(defaultStock);

	double earnRate=HappySupremacy_RoomStockCFG::GetSingleton()->GetData(m_db_room_id->get_value())->mDeduct;
	EarningsRate->set_value(earnRate);
	TotalProfit->set_value(0);
}

//房间数据是要保存到数据库的
bool logic_room_db::load_room()
{
	mongo::BSONObj b = db_game::instance().findone(DB_HAPPYSUPREMACY_ROOM, BSON("room_id"<<m_db_room_id->get_value()));
	//如果刚开始数据里没有这个数据
	if(b.isEmpty())
		return false;	
	return from_bson(b);
}

void logic_room_db::reflush_rate()
{
	static mongo::BSONObj ff = BSON("ExpectEarnRate"<<1<<"room_income"<<1<<"room_outcome"<<1);
	//std::cout<<"Monogo："<<ff.toString()<<std::endl;
	mongo::BSONObj b = db_game::instance().findone(DB_HAPPYSUPREMACY_ROOM, BSON("room_id"<<m_db_room_id->get_value()), &ff);
	if(!b.isEmpty() && b.hasField("ExpectEarnRate"))
	{
		m_db_ExpectEarnRate->set_value(b.getField("ExpectEarnRate").Double(), false);

		if(b.getField("room_income").Long() <0 && b.getField("room_outcome").Long()<0)
		{
			m_db_room_income->set_value(0);
			m_db_room_outcome->set_value(0);
			m_db_rob_income->set_value(0);
			m_db_rob_outcome->set_value(0);
			m_db_player_charge->set_value(0);
		}
	}	
}

void logic_room_db::init_game_object()
{
	m_db_room_id = CONVERT_POINT(Tfield<int16_t>, regedit_tfield(e_got_int16, "room_id"));
	m_db_room_income = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "room_income"));
	m_db_room_outcome = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "room_outcome"));
	m_db_enter_count = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "enter_count"));
	m_db_ExpectEarnRate = CONVERT_POINT(Tfield<double>, regedit_tfield(e_got_double, "ExpectEarnRate"));
	m_db_player_charge = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "player_charge"));

	m_db_room_history = regedit_arrfield("history_List", HistoryArray::malloc());
	m_db_room_historyPtr = m_db_room_history->get_Tarray<HistoryArray>();

	m_db_rob_income = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "rob_income"));
	m_db_rob_outcome = CONVERT_POINT(Tfield<int64_t>, regedit_tfield(e_got_int64, "rob_outcome"));

	TotalStock=CONVERT_POINT(Tfield<int64_t>,regedit_tfield(e_got_int64,"TotalStock"));//总库存
	TotalProfit=CONVERT_POINT(Tfield<int64_t>,regedit_tfield(e_got_int64,"TotalProfit"));//总抽水
	EarningsRate=CONVERT_POINT(Tfield<double>,regedit_tfield(e_got_double,"EarningsRate"));//抽水比例
}

bool logic_room_db::store_game_object(bool to_all)
{
	if(!has_update())
		return true;

	auto err = db_game::instance().update(DB_HAPPYSUPREMACY_ROOM, BSON("room_id"<<m_db_room_id->get_value()), BSON("$set"<<to_bson(to_all)));
	if(!err.empty())
	{
		SLOG_ERROR << "logic_room::store_game_object :" <<err;
		return false;
	}

	return true;
}

HistoryItem::HistoryItem()
{
	init_game_object();
}

void HistoryItem::init_game_object()
{
	is_forwarddoor_win = CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "is_forwarddoor_win"));
	is_oppositedoor_win = CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "is_oppositedoor_win"));
	is_reversedoor_win=CONVERT_POINT(Tfield<bool>,regedit_tfield(e_got_bool,"is_reversedoor_win"));
}

const std::string& HistoryArray::get_cells_name()
{
	static std::string cellsname = "history_List";
	return cellsname;
}

const std::string& HistoryArray::get_id_name()
{
	static std::string idname = "history_array";
	return idname;
}

GObjPtr HistoryArray::create_game_object(uint32_t object_id)
{
	auto op = HistoryItem::malloc();
	return op;
}

