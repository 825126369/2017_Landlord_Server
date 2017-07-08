#include "stdafx.h"
#include "game_quest.h"
#include "game_db.h"
#include "game_player.h"
#include "M_QuestCFG.h"
#include "time_helper.h"
#include "proc_player_quest.h"
#include "pump_type.pb.h"
//enable_obj_pool_init(game_quest, boost::null_mutex);
game_quest::game_quest()
{
	init_game_object();
}
game_quest::~game_quest()
{

}

void game_quest::init_game_object()//注册属性
{
	QuestID = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "quest_id"));
	Count = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "count"));
	Received = CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "received"));
}

bool game_quest::init_quest(int questid, int qtype)
{
	m_sdata = M_QuestCFG::GetSingleton()->GetData(questid);
	if(m_sdata == nullptr)
	{
		SLOG_ERROR << "game_quest init QuestCFG id:"<< questid;
		return false;
	}

	if(m_sdata->mType != qtype)
		return false;

	QuestID->set_value(questid);
	return true;
}

uint32_t game_quest::get_id()
{
	return QuestID->get_value();
}

const M_QuestCFGData* game_quest::get_quest_data()
{
	return m_sdata;
}


//////////////////////////////////////////////////////////////////////////
//enable_obj_pool_init(quest_map, boost::null_mutex);

quest_map::quest_map()
	:m_player(nullptr)
	,m_qtype(0)
{

}

quest_map::~quest_map()
{
}

const std::string& quest_map::get_cells_name()		//map名
{
	return m_cellsname;
}
const std::string& quest_map::get_id_name()		//map key 名
{
	static std::string idname = "quest_id";
	return idname;
}

GObjPtr quest_map::create_game_object(uint32_t object_id)	//通过id创建数组对象
{
	auto hero = game_quest::malloc();
	if(!hero->init_quest(object_id, m_qtype))
		return nullptr;

	return hero;
}

const std::string& quest_map::get_container_name()	//表名		
{
	return DB_PLAYER_QUEST;
}
bool quest_map::is_load()
{
	return m_player != nullptr;
}
uint32_t quest_map::get_index_id()
{
	return m_player->PlayerId->get_value();
}
const std::string& quest_map::get_index_name()
{
	return DB_PLAYER_INDEX;
}
db_base* quest_map::get_db()
{
	return &db_player::instance();
}

const mongo::BSONObj& quest_map::get_id_finder()
{
	return m_player->get_id_finder();
}

void quest_map::attach(game_player* player, std::string cellsname, int qtype)
{
	m_player = player;
	m_cellsname = cellsname;
	m_qtype = qtype;
}

void quest_map::init_quest()
{
	auto list = M_QuestCFG::GetSingleton()->GetMapData();
	
	for (auto it = list.begin(); it != list.end(); ++it)
	{
		M_QuestCFGData& qd = it->second;

		if(qd.mType != m_qtype || !qd.mDefault )
			continue;			

		if(have_obj(qd.mID))
			continue;

		auto pi = game_quest::malloc();	
		if(pi->init_quest(qd.mID, m_qtype))				
		{
			if(put_obj(pi))
			{
				pi->set_update();
				//db_add(pi);	
			}
		}			
	}
}


void quest_map::insert_quest(uint16_t questtype, uint32_t count, uint32_t param)
{
	auto list = M_QuestCFG::GetSingleton()->GetMapData();

	auto peer = m_player->get_gate();
	for (auto it = list.begin(); it != list.end(); ++it)
	{
		M_QuestCFGData& qd = it->second;

		if(qd.mCompleteParam != param || qd.mType != m_qtype )
			continue;				

		auto pi = game_quest::malloc();	
		if(pi->init_quest(qd.mID, m_qtype))				
		{
			if(put_obj(pi))
			{
				pi->set_update();
				//db_add(pi);
				if(peer != nullptr)
				{
					auto sendmsg = PACKET_CREATE(packetw2c_change_quest, e_mst_w2c_change_quest);
					sendmsg->set_type(m_qtype);
					auto tqi = sendmsg->mutable_qinfo();
					tqi->set_questid(pi->QuestID->get_value());
					tqi->set_count(pi->Count->get_value());
					//tqi->set_received(pi->Received->get_value());
					peer->send_msg_to_client(m_player->get_sessionid(), sendmsg);
				}				
			}
		}			
	}
}


bool quest_map::change_quest(uint16_t questtype, uint32_t count, uint32_t param)
{
	bool ret = false;
	for (auto it = begin();it != end(); ++it)
	{
		auto pi = CONVERT_POINT(game_quest, it->second);
		auto pdata = pi->get_quest_data();
		if(pi->Received->get_value() || pdata->mCompleteType != questtype 
			|| (pdata->mCompleteParam > 0 && pdata->mCompleteParam != param ))
			continue;

		if(pdata->mNextQuestID <=0 && pi->Count->get_value() >= pdata->mCompleteCount)
			continue;

		int oldv = pi->Count->get_value();

		if(pdata->mIsSet)
		{
			if(oldv == count)
				continue;

			pi->Count->set_value(count);	
		}
		else
			pi->Count->add_value(count);	

		//db_update(pi);		
		ret = true;

		if(oldv >= pdata->mCompleteCount)
			continue;

		auto peer = m_player->get_gate();
		if(peer != nullptr)
		{
			auto sendmsg = PACKET_CREATE(packetw2c_change_quest, e_mst_w2c_change_quest);
			sendmsg->set_type(m_qtype);
			auto tqi = sendmsg->mutable_qinfo();
			tqi->set_questid(pi->QuestID->get_value());
			tqi->set_count(pi->Count->get_value());
			//tqi->set_received(pi->Received->get_value());
			peer->send_msg_to_client(m_player->get_sessionid(), sendmsg);
		}	
	}	

	return ret;
}

bool quest_map::check_quest(uint32_t questid)
{	
	auto pi = CONVERT_POINT(game_quest, find_obj(questid));
	if(pi == nullptr)
		return false;

	if(pi->Received->get_value())
		return false;

	auto data = pi->get_quest_data();

	if(data->mCompleteCount > pi->Count->get_value())
		return false;

	////特殊任务判定
	//switch ((e_quest_type)data->mCompleteType)
	//{
	//case e_qt_addspirit://体力
	//	{
	//		auto stime = time_helper::convert_to_ptime(data->mStartTime).time_of_day();			
	//		auto etime = time_helper::convert_to_ptime(data->mEndTime).time_of_day();			
	//		auto ntime = time_helper::instance().get_cur_ptime().time_of_day();

	//		if(ntime>stime && ntime<etime)
	//			return true;

	//		return false;
	//	}
	//case e_qt_holiday://节日
	//	{			
	//		auto stime = time_helper::convert_to_ptime(data->mStartTime);
	//		auto etime = time_helper::convert_to_ptime(data->mEndTime);
	//		auto ntime = time_helper::instance().get_cur_ptime();
	//		if(ntime>stime && ntime<etime)
	//			return true;
	//		
	//		return false;
	//	}
	//	break;
	//case e_qt_monthcard://月卡
	//	{
	//		//auto vipdate = time_helper::convert_to_date(m_player->get_sys<game_sys_vip>()->VipCardEndTime->get_value());
	//		//if(vipdate < time_helper::instance().get_cur_date())
	//		//	return false;
	//	}
	//	break;	
	//}

	return true;
}

void quest_map::receive_quest(uint32_t questid, std::vector<stItem>& items)
{
	items.clear();
	auto data = M_QuestCFG::GetSingleton()->GetData(questid);
	if(data == nullptr)
		return;

	auto pi = CONVERT_POINT(game_quest, find_obj(questid));
	if(pi == nullptr)
		return;

	boost::format fmt = boost::format("questId:%1%") % questid;

	int reason = (m_qtype == 1 ? type_reason_daily_task : type_reason_achievement); 

	//获取奖励改变领取状态
	for (int i =0; i<data->mAwardItemIDs.size(); i++)
	{
		stItem si(data->mAwardItemIDs[i], data->mAwardItemCounts[i]);
		m_player->addItem(si.m_itemId,si.m_count, reason, fmt.str());
		items.push_back(si);
	}
	pi->Received->set_value(true);

	//m_player->store_game_object();
	db_update(pi);	

	//增加后续任务
	if(data->mNextQuestID >0)
	{
		auto pi2 = game_quest::malloc();	
		if(pi2->init_quest(data->mNextQuestID, m_qtype))				
		{
			if(data->mIsSaveCount)
				pi2->Count->set_value(pi->Count->get_value());
			
			if(put_obj(pi2))
				db_add(pi2);

			auto peer = m_player->get_gate();
			if(peer != nullptr)
			{
				auto sendmsg = PACKET_CREATE(packetw2c_change_quest, e_mst_w2c_change_quest);
				sendmsg->set_type(m_qtype);
				auto tqi = sendmsg->mutable_qinfo();
				tqi->set_questid(pi2->QuestID->get_value());
				tqi->set_count(pi2->Count->get_value());
				//tqi->set_received(pi->Received->get_value());
				peer->send_msg_to_client(m_player->get_sessionid(), sendmsg);
			}	
		}	
	}
}


void quest_map::remove_quest(uint16_t questtype)
{
	for (auto it = begin();it != end();)
	{
		auto pi = CONVERT_POINT(game_quest, it->second);
		if(pi->get_quest_data()->mCompleteType == questtype
			&&pi->get_quest_data()->mType == m_qtype)
		{
			//db_del(it->second);
			set_update();
			auto peer = m_player->get_gate();
			if(peer != nullptr)
			{
				auto sendmsg = PACKET_CREATE(packetw2c_change_quest, e_mst_w2c_change_quest);
				sendmsg->set_type(m_qtype);
				auto tqi = sendmsg->mutable_qinfo();
				tqi->set_questid(pi->QuestID->get_value());
				tqi->set_count(-1);
				//tqi->set_received(pi->Received->get_value());
				peer->send_msg_to_client(m_player->get_sessionid(), sendmsg);
			}	
			it = m_cells.erase(it);
		}
		else
		{
			++it;
		}
	}
}

void quest_map::add_quest(uint16_t questtype, uint32_t count, uint32_t param)
{
	auto list = M_QuestCFG::GetSingleton()->GetMapData();

	auto peer = m_player->get_gate();
	for (auto it = list.begin(); it != list.end(); ++it)
	{
		M_QuestCFGData& qd = it->second;

		if(qd.mType != m_qtype || qd.mCompleteType != questtype || qd.mCompleteParam != param)
			continue;		
		
		if(have_obj(qd.mID))
			continue;

		auto pi = game_quest::malloc();	
		if(pi->init_quest(qd.mID, m_qtype))				
		{
			pi->Count->set_value(count);
			put_obj(pi);
			pi->set_update();
			//if(put_obj(pi))			
				//db_add(pi);		

			auto peer = m_player->get_gate();
			if(peer != nullptr)
			{
				auto sendmsg = PACKET_CREATE(packetw2c_change_quest, e_mst_w2c_change_quest);
				sendmsg->set_type(m_qtype);
				auto tqi = sendmsg->mutable_qinfo();
				tqi->set_questid(pi->QuestID->get_value());
				tqi->set_count(pi->Count->get_value());
				//tqi->set_received(pi->Received->get_value());
				peer->send_msg_to_client(m_player->get_sessionid(), sendmsg);
			}	
		}
	}
}