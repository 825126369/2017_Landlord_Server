#include "stdafx.h"
#include "gm_sys.h"
#include "game_db.h"
#include "game_player.h"
#include "game_sys_recharge.h"

using namespace boost;

enum RechargeType
{
	// 充人民币
	rechargeRMB,

	// 删除自定义头像
	delIconCustom,
};

//////////////////////////////////////////////////////////////////////////

void GmSys::init_sys_object()
{
}

void GmSys::onPlayerLogin(game_player* player, bool isReLoadPlayerData)
{
	if(player == nullptr)
		return;

	mongo::BSONObj search = BSON("playerId" << player->PlayerId->get_value());

	int rtype = 0, param = 0;
	mongo::BSONObj* ptr = nullptr;
	std::vector<mongo::BSONObj> res;
	db_player::instance().find(res, DB_GM_RECHARGE, search);

	for(int i = 0; i < res.size(); i++)
	{
		ptr = &res[i];
		rtype = ptr->getIntField("rtype");

		switch(rtype)
		{
		case rechargeRMB:
			{
				param = ptr->getIntField("param");
				_payRMB(player, param);
			}
			break;
		case delIconCustom:
			{
				time_t t   = ptr->getField("paramTime").Date().toTimeT();
				_delIconCustom(player, t);
			}
			break;
		}
	}

	if(!res.empty())
	{
		db_player::instance().remove(DB_GM_RECHARGE, search);
	}
}

void GmSys::_payRMB(game_player* player, int payId)
{
	player->get_sys<game_sys_recharge>()->payment_once(payId, 0, true);
}

void GmSys::_delIconCustom(game_player* player, time_t deadTime)
{
	player->IconCustom->set_string("");
	player->UpLoadCustomHeadFreezeDeadTime->set_value(deadTime);
	//player->store_game_object();
}
