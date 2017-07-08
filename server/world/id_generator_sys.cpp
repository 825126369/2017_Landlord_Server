#include "stdafx.h"
#include "id_generator_sys.h"
#include "time_helper.h"
#include "game_db.h"

std::string g_idKeyName[] =
{
	"newAccountId",
};

void IdGeneratorSys::init_sys_object()
{
	for(int i = IdTypeFirst; i < IdTypeEnd; i++)
	{
		mongo::BSONObj b = db_player::instance().findone(DB_COMMON_CONFIG, BSON("type" << g_idKeyName[i]));
		if(!b.isEmpty())
		{
			m_curIds[i] = b.getIntField("value");
		}
		else
		{
			m_curIds[i] = 0;
		}
	}
}

void IdGeneratorSys::sys_exit()
{
	for(int i = IdTypeFirst; i < IdTypeEnd; i++)
	{
		_saveId(i, m_curIds[i]);
	}
}

int IdGeneratorSys::getCurId(IdType t)
{
	std::map<int, int>::iterator it = m_curIds.find(t);
	if(it == m_curIds.end())
		return -1;

	it->second++;
	return it->second;
}

void IdGeneratorSys::_saveId(int idType, int curValue)
{
	db_player::instance().update(DB_COMMON_CONFIG, 
		BSON("type"<< g_idKeyName[idType]), 
		BSON("$set" << BSON("value" << curValue)));
}



