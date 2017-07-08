#include "stdafx.h"
#include "star_lottery_sys.h"
#include "global_sys_mgr.h"
#include "M_BaseInfoCFG.h"
#include "game_db.h"

//////////////////////////////////////////////////////////////////////////
StarLotterySys::StarLotterySys()
	:m_total(0)
	,m_update(false)
{

}

void StarLotterySys::init_sys_object()
{

}

//加载数据
bool StarLotterySys::sys_load()
{
	mongo::BSONObj b = db_player::instance().findone(DB_COMMON_CONFIG, BSON("type"<<"cur_total_star"));
	if(b.isEmpty())
	{
		m_total = 0;
		db_player::instance().update(DB_COMMON_CONFIG,BSON("type"<<"cur_total_star"), BSON("type"<<"cur_total_star"<<"value" << m_total));
	}
	else
		m_total = b.getIntField("value");

	return true;
}

void StarLotterySys::sys_update(double delta)
{
	if(m_update)
	{
		db_player::instance().update(DB_COMMON_CONFIG,BSON("type"<<"cur_total_star"), BSON("type"<<"cur_total_star"<<"value" << m_total));
		m_update = false;
	}
}

//获取剩余值
int StarLotterySys::get_surplus()
{
	static int AwardMax = M_BaseInfoCFG::GetSingleton()->GetData("AwardMax")->mValue;
	return AwardMax - m_total;
}

//更新当前值
bool StarLotterySys::update_total(int v)
{
	static int AwardMax = M_BaseInfoCFG::GetSingleton()->GetData("AwardMax")->mValue;

	if(m_total+v > AwardMax)
		return false;

	m_total += v;	
	m_update = true;

	return true;
}