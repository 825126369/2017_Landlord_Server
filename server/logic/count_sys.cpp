#include "stdafx.h"
#include "count_sys.h"
#include "db_base.h"
#include "../games/game_common/game_common_def.h"
#include "game_manager.h"
#include "time_helper.h"

static const std::string DB_COMMON_CONFIG = "common_config";
static const std::string DB_COMMON_CHECK  = "common_check";

void CountSys::init_sys_object()
{
	m_dbGame = nullptr;
}

bool CountSys::setGameDb(db_base *dbGame)
{
	if(!dbGame)
		return false;

	m_dbGame = dbGame;

	if(game_manager::instance().get_gameid() == game_baccracat)
	{
		_check(DAY_COUNT_BACCARAT);
	}
	else if(game_manager::instance().get_gameid() == game_shcd)
	{
		_check(DAY_COUNT_SHCD);
	}
	return true;
}

void CountSys::sys_time_update()
{
	if(m_dbGame == nullptr)
		return;

	// 百家乐每日清空 靴计数
	if(game_manager::instance().get_gameid() == game_baccracat)
	{
		m_dbGame->update(DB_COMMON_CONFIG, BSON("type" << DAY_COUNT_BACCARAT), BSON("$set" << BSON("value" << 0)));

		_updateCheckTime(DAY_COUNT_BACCARAT);
	}
	else if(game_manager::instance().get_gameid() == game_shcd)
	{
		m_dbGame->update(DB_COMMON_CONFIG, BSON("type" << DAY_COUNT_SHCD), BSON("$set" << BSON("value" << 0)));

		_updateCheckTime(DAY_COUNT_SHCD);
	}
}

int64_t CountSys::getCurId(const std::string& key)
{
	if(m_dbGame == nullptr)
		return -1;

	int64_t curId = 0;

	//每次都从服务器获取 保证ID完全对应
	mongo::BSONObj b = m_dbGame->findone(DB_COMMON_CONFIG, BSON("type" << key));
	if(b.isEmpty())
	{
		curId = 1;
		m_dbGame->update(DB_COMMON_CONFIG,BSON("type" << key), 
			BSON("type" << key << "value" << curId));
		return curId;
	}
	else
		curId = b.getIntField("value");	

	m_dbGame->update(DB_COMMON_CONFIG, BSON("type" << key),
		BSON("$inc" <<BSON("value" << 1)));

	return ++curId;	
}

bool CountSys::_checkReflush(time_t lastCheckTime)
{
	auto pt = time_helper::convert_to_date(lastCheckTime);
	if(pt >= time_helper::instance().get_cur_date())
		return false;

	return true;
}

bool CountSys::_updateCheckTime(const std::string& typeContent)
{
	if(m_dbGame == nullptr)
		return false;
	
	time_t nt = time_helper::instance().get_cur_time() * 1000;

	m_dbGame->update(DB_COMMON_CHECK, BSON("type" << typeContent), 
		BSON("$set" << BSON("checkTime" << mongo::Date_t(nt)) ));

	return true;
}

void CountSys::_check(const std::string& typeContent)
{
	mongo::BSONObj b = m_dbGame->findone(DB_COMMON_CHECK, BSON("type" << typeContent));
	if(b.isEmpty())
	{
		_updateCheckTime(typeContent);
	}
	else
	{
		time_t lastCheckTime = b.getField("checkTime").Date().toTimeT();
		bool res = _checkReflush(lastCheckTime);
		if(res)
		{
			sys_time_update();
		}
	}
}

