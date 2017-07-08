#include "stdafx.h"
#include "game_quest_mgr.h"
#include "game_db.h"
#include "game_player.h"
#include "M_QuestCFG.h"
#include "time_helper.h"
#include "game_quest.h"




//////////////////////////////////////////////////////////////////////////

game_quest_mgr::game_quest_mgr()
	:m_checktime(0)
{
	init_game_object();
}

game_quest_mgr::~game_quest_mgr()
{
}

void game_quest_mgr::on_attach()
{
	Quests->get_Tmap<quest_map>()->attach(get_game_player(), "quests");
	Activitys->get_Tmap<quest_map>()->attach(get_game_player(), "activitys", 1);
	
}

bool game_quest_mgr::sys_load()
{	
	mongo::BSONObj b = db_player::instance().findone(DB_PLAYER_QUEST, get_game_player()->get_id_finder());
	if(b.isEmpty())
	{
		sys_init();
		return true;
	}

	bool ret =  from_bson(b);
	
	//检测新增加任务成就
	if(NeedReflush->get_value())
	{
		init_quest();
		NeedReflush->set_value(false);
		store_game_object();		
	}

	return ret;
}

void game_quest_mgr::sys_init()
{
	mongo::BSONObjBuilder b2;
	mongo::BSONArrayBuilder ba1;
	mongo::BSONArrayBuilder ba2;

	auto list = M_QuestCFG::GetSingleton()->GetMapData();
	for (auto it = list.begin(); it != list.end(); ++it)
	{
		M_QuestCFGData& qd = it->second;

		if(!qd.mDefault)
			continue;

		auto pi = game_quest::malloc();	
		if(pi->init_quest(qd.mID, qd.mType))				
		{
			if(qd.mType == 1)
			{
				Activitys->get_map()->put_obj(pi);
			}
			else
			{
				Quests->get_map()->put_obj(pi);
			}
		}		
	}

	store_game_object(true);
}

void game_quest_mgr::sys_time_update()
{
	if(Activitys->get_map()->get_obj_count() <=0)
		return;

	auto amap = Activitys->get_map();
	for (auto it = amap->begin(); it != amap->end(); ++it)
	{
		auto pi = CONVERT_POINT(game_quest, it->second);
		pi->Count->set_value(0);
		pi->Received->set_value(false);	
	}
		
	Activitys->set_update();
	store_game_object();
}

void game_quest_mgr::change_quest(uint16_t questtype, uint32_t count, uint32_t param)
{
	if(Quests->get_Tmap<quest_map>()->change_quest(questtype, count, param))
		Quests->set_update();

	if(Activitys->get_Tmap<quest_map>()->change_quest(questtype, count, param))
		Activitys->set_update();
}
void game_quest_mgr::init_quest()
{
	Quests->get_Tmap<quest_map>()->init_quest();
	Activitys->get_Tmap<quest_map>()->init_quest();
}
void game_quest_mgr::insert_quest(uint16_t questtype, uint32_t count, uint32_t param)
{
	Quests->get_Tmap<quest_map>()->insert_quest(questtype, count, param);
	Activitys->get_Tmap<quest_map>()->insert_quest(questtype, count, param);
}

void game_quest_mgr::remove_quest(uint16_t questtype)
{
	Quests->get_Tmap<quest_map>()->remove_quest(questtype);
	Activitys->get_Tmap<quest_map>()->remove_quest(questtype);
}
void game_quest_mgr::add_quest(uint16_t questtype, uint32_t count, uint32_t param)
{
	Quests->get_Tmap<quest_map>()->add_quest(questtype, count, param);
	Activitys->get_Tmap<quest_map>()->add_quest(questtype, count, param);
}

GMapObjPtr game_quest_mgr::get_map(int qtype)
{
	if(qtype == 1)
		return Activitys->get_map();
	else
		return Quests->get_map();
}
bool game_quest_mgr::check_quest(int qtype, uint32_t questid)
{
	if(qtype == 1) // 每日
		return Activitys->get_Tmap<quest_map>()->check_quest(questid);
	else
		return Quests->get_Tmap<quest_map>()->check_quest(questid);
}
void game_quest_mgr::receive_quest(int qtype, uint32_t questid, std::vector<stItem>& items)
{
	if(qtype == 1)
		Activitys->get_Tmap<quest_map>()->receive_quest(questid, items);
	else
		Quests->get_Tmap<quest_map>()->receive_quest(questid, items);
}

//////////////////////////////////////////////////////////////////////////
void game_quest_mgr::init_game_object()//注册属性
{
	//LastCheckTime = CONVERT_POINT(Tfield<time_t>, regedit_tfield(e_got_date, "lastcheck_time"));
	Quests = regedit_mapfield("quests", quest_map::malloc());
	Activitys = regedit_mapfield("activitys", quest_map::malloc());
	NeedReflush = CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "NeedReflush"));
}

bool game_quest_mgr::store_game_object(bool to_all)
{
	if(!has_update())
		return true;

	if(!get_game_player()->IsLogin())
		return true;

	auto err = db_player::instance().update(DB_PLAYER_QUEST, get_game_player()->get_id_finder(), BSON("$set"<<to_bson(to_all)));
	if(!err.empty())
	{
		SLOG_ERROR << "game_quest_mgr::store_game_object :" <<err << " fields:"<<get_errorfields();
		return false;
	}
	return true;
}

// 每帧更新
void game_quest_mgr::sys_update(double delta)
{
	m_checktime+=delta;
	if(m_checktime >1)
	{
		store_game_object();
		m_checktime = 0;
	}
}

// 系统退出
void game_quest_mgr::sys_exit()
{
	store_game_object();
}