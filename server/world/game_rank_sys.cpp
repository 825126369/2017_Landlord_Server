#include "stdafx.h"
#include "game_rank_sys.h"
#include "msg_type_def.pb.h"
#include "game_db.h"
#include "game_player.h"
#include "game_db_log.h"
#include "time_helper.h"

int GameRankSys::getCoinRankList(game_player* player, std::vector<stRankInfo>& rankList, int& selfRank)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;
	
	static mongo::BSONObj g_rankField = BSON("player_id" << 1 << "nickname" << 1 << "gold" << 1 << "VipLevel" << 1);
	static mongo::BSONObj g_sortField = BSON("gold" << -1);
	static mongo::BSONObj g_condition = BSON("player_id" << mongo::GT << 0);

	bool hasMe = false;
	std::vector<mongo::BSONObj> vec;
	db_player::instance().find(vec, DB_PLAYER_INFO, g_condition, &g_rankField, 50, 0, &g_sortField);

	for(int i = 0; i < (int)vec.size(); i++)
	{
		mongo::BSONObj& obj = vec[i];
		stRankInfo rank;
		rank.m_playerId = obj.getIntField("player_id");
		rank.m_nickName = obj.getStringField("nickname");
		rank.m_gold = obj.getField("gold").Long();
		rank.m_vipLevel = obj.getIntField("VipLevel");
		if(rank.m_playerId == player->PlayerId->get_value())
		{
			selfRank = i;
			hasMe = true;
		}
		rankList.push_back(rank);
	}
	
	if(!hasMe)
	{
		mongo::BSONObj cond = BSON("gold" << mongo::GT << player->Gold->get_value());
		selfRank = db_player::instance().get_count(DB_PLAYER_INFO, cond);
	}

	return msg_type_def::e_rmt_success;
}

int GameRankSys::getRechargeRankList(game_player* player, std::vector<stRankInfo>& rankList, int& selfRank, int& selfrmb, bool yesterday)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	static mongo::BSONObj g_rankField = BSON("player_id" << 1 << "nickname" << 1 << "total_rmb" << 1 << "VipLevel" << 1);
	static mongo::BSONObj g_sortField = BSON("total_rmb" << -1);
	static mongo::BSONObj g_condition = BSON("player_id" << mongo::GT << 0);
	static mongo::BSONObj g_condition2 = BSON("total_rmb" << 1);

	bool hasMe = false;
	std::string table;
	if(yesterday)
		table = DB_YESTERDAY_RECHARGE;
	else
		table = DB_TODAY_RECHARGE;	

	std::vector<mongo::BSONObj> vec;
	db_player::instance().find(vec, table, g_condition, &g_rankField, 50, 0, &g_sortField);

	for(int i = 0; i < (int)vec.size(); i++)
	{
		mongo::BSONObj& obj = vec[i];
		stRankInfo rank;
		rank.m_playerId = obj.getIntField("player_id");
		rank.m_nickName = obj.getStringField("nickname");
		rank.m_gold = obj.getIntField("total_rmb");
		rank.m_vipLevel = obj.getIntField("VipLevel");
		if(rank.m_playerId == player->PlayerId->get_value())
		{
			selfRank = i;
			selfrmb = rank.m_gold;
			hasMe = true;
		}
		rankList.push_back(rank);
	}

	if(!hasMe)
	{
		auto ret = db_player::instance().findone(table, BSON("player_id" << player->PlayerId->get_value()), &g_condition2);
		if(!ret.isEmpty())
		{
			selfrmb = ret.getIntField("total_rmb");
		}

		mongo::BSONObj cond = BSON("total_rmb" << mongo::GT << selfrmb);
		selfRank = db_player::instance().get_count(table, cond);
	}

	return msg_type_def::e_rmt_success;
}

void GameRankSys::sys_time_update()
{

	static mongo::BSONObj g_condition = BSON("player_id" << mongo::GT << 0);
	static mongo::BSONObj g_sortField = BSON("total_rmb" << -1);

	_coinGrowthProcess();

	//发奖
	std::vector<mongo::BSONObj> vec;
	std::vector<mongo::BSONObj> savevec;
	db_player::instance().find(vec, DB_TODAY_RECHARGE, g_condition,nullptr, 10,0,&g_sortField);
	uint32_t playerId =0;
	int rmb = 0;
	for (int i = 0;i<vec.size();i++)
	{
		if(i==10)break;
		mongo::BSONObj& obj = vec[i];
		playerId = obj.getIntField("player_id");
		rmb = obj.getIntField("total_rmb");




		savevec.push_back(obj);
	}	
	
		//移除旧排行
	db_player::instance().clearTable(DB_YESTERDAY_RECHARGE);
	db_player::instance().insert(DB_YESTERDAY_RECHARGE, savevec);
	db_player::instance().clearTable(DB_TODAY_RECHARGE);
}

static const std::string COIN_GROWTH = "pumpCoinGrowth";

int GameRankSys::getCoinGrowthList(game_player* player, std::vector<stRankInfo>& rankList, int& selfRank, GOLD_TYPE& selfGold)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	//static mongo::BSONObj g_rankField = BSON("player_id" << 1 << "nickname" << 1 << "gold" << 1 << "VipLevel" << 1);
	static mongo::BSONObj g_sortField = BSON("gold" << -1);
	static mongo::BSONObj g_condition = BSON("playerId" << mongo::GT << 0);

	bool hasMe = false;
	std::vector<mongo::BSONObj> vec;
	db_log::instance().find(vec, COIN_GROWTH, g_condition, nullptr, 20, 0, &g_sortField);

	for(int i = 0; i < (int)vec.size(); i++)
	{
		mongo::BSONObj& obj = vec[i];
		stRankInfo rank;
		rank.m_playerId = obj.getIntField("playerId");
		rank.m_nickName = obj.getStringField("nickName");
		rank.m_gold = obj.getField("gold").Long();
		rank.m_vipLevel = obj.getIntField("vipLevel");
		if(rank.m_playerId == player->PlayerId->get_value())
		{
			selfRank = i;
			selfGold = rank.m_gold;
			hasMe = true;
		}
		rankList.push_back(rank);
	}

	if(!hasMe)
	{
		mongo::BSONObj tmpCond = BSON("playerId" << player->PlayerId->get_value());
		mongo::BSONObj self = db_log::instance().findone(COIN_GROWTH, tmpCond);
		if(!self.isEmpty())
		{
			selfGold = self.getField("gold").Long();
			mongo::BSONObj cond = BSON("gold" << mongo::GT << selfGold);
			selfRank = db_log::instance().get_count(COIN_GROWTH, cond);
		}
		else
		{
			selfRank = -1;
		}
	}

	return msg_type_def::e_rmt_success;
}

//每周重置
void GameRankSys::_coinGrowthProcess()
{
	auto nowt = time_helper::instance().get_cur_date();
	if(nowt.day_of_week() != 1)//不是星期一
		return;

	static mongo::BSONObj g_sortField = BSON("gold" << -1);
	static mongo::BSONObj g_condition = BSON("playerId" << mongo::GT << 0);

	std::vector<mongo::BSONObj> vec;
	db_log::instance().find(vec, COIN_GROWTH, g_condition, nullptr, 20, 0, &g_sortField);
	time_t tdate = time_helper::convert_from_date(nowt);

	for(int i = 0; i < (int)vec.size(); i++)
	{
		mongo::BSONObj& obj = vec[i];

		mongo::BSONObjBuilder builder;
		builder.appendTimeT("genTime", tdate);
		builder.append("rank",  i+1);
		builder.append("playerId", obj.getIntField("playerId"));
		builder.append("nickName", obj.getStringField("nickName"));
		builder.append("gold", obj.getField("gold").Long());
		builder.append("vipLevel", obj.getIntField("vipLevel"));

		db_log::instance().push_insert(e_dlt_pump_coin_growth_history, builder.obj());
	}

	db_log::instance().clearTable(COIN_GROWTH);
}