#include "stdafx.h"
#include "speaker_mgr.h"
#include "game_player.h"
#include "game_db.h"
#include "global_sys_mgr.h"
#include "chat_sys.h"

bool SpeakerMgr::sys_load()
{
	auto player = get_game_player();

	mongo::BSONObj cond = BSON("playerId" << player->PlayerId->get_value());

	std::vector<mongo::BSONObj> vec;
	db_player::instance().find(vec, DB_SPEAKER, cond);

	if(!vec.empty())
	{
		auto sys = GLOBAL_SYS(ChatSys);

		for(int i = 0; i < (int)vec.size(); i++)
		{
			mongo::BSONObj& obj = vec[i];

			std::string id = obj.getField("_id").OID().toString();
			std::string content = obj.getStringField("content");
			int remainCount = obj.getIntField("remainCount");
			int vipLevel = obj.getIntField("vipLevel");
			std::string nickName = obj.getStringField("nickName");

			sys->addContinuousSendSpeakerFromDb(player->PlayerId->get_value(),
				vipLevel, nickName, content, remainCount, id);
		}
	}
	
	return true;
}
