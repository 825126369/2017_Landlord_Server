#include "stdafx.h"
#include "shop_sys.h"
#include "msg_type_def.pb.h"
#include "M_ItemCFG.h"
#include "game_player.h"
#include "M_CommodityCFG.h"
#include "pump_type.pb.h"
#include "pump_sys.h"
#include "pump_type.h"
#include "global_sys_mgr.h"
#include "enable_random.h"
#include "M_RechangeCFG.h"
#include "M_RobotNameCFG.h"
#include "M_MultiLanguageCFG.h"
#include "M_RechangeCFG.h"
#include "game_db.h"

// 是否足够买商品
static int moneyEnough(game_player* player, const M_CommodityCFGData* shop)
{
	switch (shop->mPriceType)
	{
	case msg_type_def::e_itd_gold:
		{
			if(player->Gold->get_value() < shop->mPrice)
				return msg_type_def::e_rmt_gold_not_enough;
		}
		break;
	case msg_type_def::e_itd_ticket:
		{
			if(player->Ticket->get_value() < shop->mPrice)
				return msg_type_def::e_rmt_ticket_not_enough;
		}
		break;
	default:
		{
			SLOG_ERROR << boost::format("moneyEnough，无法识别的货币类型[type = %1%]") % shop->mPriceType;
			return msg_type_def::e_rmt_unknow;
		}
		break;
	}

	return msg_type_def::e_rmt_success;
}

int ShopSys::buyCommodity(game_player* player, int commodityId)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;
	
	const M_CommodityCFGData* shop = M_CommodityCFG::GetSingleton()->GetData(commodityId);
	if(shop == nullptr)
	{
		return msg_type_def::e_rmt_unknow;
	}

	int i = 0;
	if(shop->mItem.size() != shop->mCount.size())
	{
		SLOG_ERROR << boost::format("ShopSys::buyCommodity, Item列表与Count列表个数不一样[id = %1%]") % commodityId;
		return msg_type_def::e_rmt_unknow;
	}

	if(shop->mItem.empty())
	{
		SLOG_ERROR << boost::format("ShopSys::buyCommodity, Item列表与Count列表空[id = %1%]") % commodityId;
		return msg_type_def::e_rmt_unknow;
	}

	int res = moneyEnough(player, shop);
	if(res != msg_type_def::e_rmt_success)
		return res;

	const M_ItemCFGData* pItem = nullptr;
	bool find = false;

	boost::format fmt = boost::format("commodityId:%1%") % commodityId;

	auto sys = GLOBAL_SYS(PumpSys);
	for(i = 0; i < (int)shop->mItem.size(); i++)
	{
		pItem = M_ItemCFG::GetSingleton()->GetData(shop->mItem[i]);
		if(pItem)
		{
			player->addItem(shop->mItem[i], shop->mCount[i], type_reason_buy_commodity_gain, fmt.str());
			find = true;

			sys->buyItemLog(shop->mItem[i]);
		}
		else
		{
			SLOG_ERROR << boost::format("ShopSys::buyCommodity, 找不到道具[id = %1%]，商品[id = %1%]") % shop->mItem[i] % commodityId;
		}
	}

	if(find)
	{
		switch (shop->mPriceType)
		{
		case msg_type_def::e_itd_gold:
			{
				player->addItem(msg_type_def::e_itd_gold, -shop->mPrice, type_reason_buy_commodity_expend, fmt.str());
			}
			break;
		case msg_type_def::e_itd_ticket:
			{
				player->addItem(msg_type_def::e_itd_ticket, -shop->mPrice, type_reason_buy_commodity_expend, fmt.str());
			}
			break;
		}

		//player->store_game_object();
	}

	return msg_type_def::e_rmt_success;
}

bool ShopSys::sys_load()
{
	mongo::BSONObj cond = BSON("index" << mongo::GT << 0);

	std::vector<mongo::BSONObj> vec;
	mongo::BSONObj sortField = BSON("index" << -1);
	db_game::instance().find(vec, DB_RECHARGE_HISTORY, cond);

	for(int i = 0; i < vec.size(); i++)
	{
		mongo::BSONObj& obj = vec[i];
		std::string info = obj.getStringField("info");
		m_historys.push_back(info);
	}
	return true;
}

void ShopSys::init_sys_object()
{
	m_elapsed = 0;
}

void ShopSys::sys_update(double delta)
{
	m_elapsed += delta;
	if (m_elapsed >= 60)
	{
		robot_recharge();
	}
}

void ShopSys::player_recharge(game_player* player, int payId)
{
	auto rechangecfg = M_RechangeCFG::GetSingleton()->GetData(payId);
	if (rechangecfg == nullptr)
	{
		return;
	}
	add_charge_history(player->NickName->get_string(), rechangecfg->mName);
}

void ShopSys::robot_recharge()
{
	//昵称
	int maxcount = M_RobotNameCFG::GetSingleton()->GetCount()-1;
	int index = global_random::instance().rand_int(0, maxcount);
	auto robotcfg = M_RobotNameCFG::GetSingleton()->GetData(index);

	int paycount = M_RechangeCFG::GetSingleton()->GetCount();
	int payindex = global_random::instance().rand_int(1, paycount-1);
	auto rechangecfg = M_RechangeCFG::GetSingleton()->GetData(payindex);
	if (robotcfg != nullptr && rechangecfg != nullptr)
	{
		add_charge_history(robotcfg->mNickName, rechangecfg->mName);
	}
}

void ShopSys::add_charge_history(const std::string& playername, const std::string& payname)
{
	m_elapsed = 0;
	auto languagestr = M_MultiLanguageCFG::GetSingleton()->GetData("RechargeHistory");
	boost::format fmt = boost::format(languagestr->mName) % playername % payname;
	m_historys.push_back(fmt.str());
	if (m_historys.size() > 30)
	{
		m_historys.pop_front();
	}
	save_historys();
}

const std::list<std::string>& ShopSys::get_charge_historys()
{
	return m_historys;
}

void ShopSys::save_historys()
{
	db_game::instance().clearTable(DB_RECHARGE_HISTORY);

	int i = 1;
	for (auto it = m_historys.begin(); it != m_historys.end(); it++)
	{
		mongo::BSONObjBuilder builder;
		builder.append("index",  i++);
		builder.append("info", *it);
		db_game::instance().insert(DB_RECHARGE_HISTORY, builder.obj());
	}
}
