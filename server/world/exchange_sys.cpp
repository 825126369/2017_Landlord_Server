#include "stdafx.h"
#include "exchange_sys.h"
#include "msg_type_def.pb.h"
#include "M_ExchangeCFG.h"
#include "M_ItemCFG.h"
#include "game_player.h"
#include "time_helper.h"
#include "game_db.h"
#include "pump_type.pb.h"
#include "M_BaseInfoCFG.h"
#include "game_sys_recharge.h"
#include "M_MultiLanguageCFG.h"
#include "chat_sys.h"
#include "global_sys_mgr.h"

int ExchangeSys::exchange(game_player* player, int chgId, const std::string& phone)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;
	
	const M_ExchangeCFGData* chgData = M_ExchangeCFG::GetSingleton()->GetData(chgId);
	if(chgData == nullptr)
		return msg_type_def::e_rmt_unknow;

	int curVipLevel = player->get_viplvl();
	if(curVipLevel < chgData->mVip)
		return msg_type_def::e_rmt_vip_under;

	if(player->Chip->get_value() < chgData->mCostTicket)
		return msg_type_def::e_rmt_chip_not_enough;

	const M_ItemCFGData* pItem = M_ItemCFG::GetSingleton()->GetData(chgData->mItemId);
	if(pItem == nullptr)
	{
		SLOG_ERROR << boost::format("兑换 ExchangeSys::exchange, 兑换[id = %1%]，找不到对应道具[id = %2%]") % chgId % chgData->mItemId;
		return msg_type_def::e_rmt_unknow;
	}

	boost::format fmt = boost::format("exchangeId:%1%") % chgId;

	if(pItem->mItemCategory == msg_type_def::e_itd_gold ||
	   pItem->mItemCategory == msg_type_def::e_itd_ticket) // 兑换钻石或金币
	{
		player->addItem(pItem->mItemCategory, chgData->mItemCount, type_reason_exchange, fmt.str());
	}
	else
	{
		mongo::OID oid;
		oid.init();

		mongo::BSONObjBuilder builder;
		builder.append("exchangeId", oid.toString());
		builder.append("playerId", player->PlayerId->get_value());
		builder.append("itemName", pItem->mItemName);
		builder.append("isReceive", false);
		builder.append("phone", phone);
		builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
		builder.append("chgId", chgId);
		const std::string& res = db_player::instance().insert(DB_EXCHANGE, builder.done());
		if(!res.empty())
		{
			return msg_type_def::e_rmt_unknow;
		}

		_notice(player);
	}
	
	player->addItem(msg_type_def::e_itd_chip, -chgData->mCostTicket, type_reason_exchange, fmt.str());
	//player->store_game_object();
	return msg_type_def::e_rmt_success;
}

int ExchangeSys::getExchangeList(game_player* player, std::vector<stExchangeInfo>& infoList)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	static mongo::BSONObj g_sortField = BSON("genTime" << -1);
	static mongo::BSONObj retField = BSON("playerId" << 1 << "isReceive" << 1 << "chgId" << 1 << "genTime" << 1);
	mongo::BSONObj condition = BSON("playerId" << player->PlayerId->get_value());
	std::vector<mongo::BSONObj> bsonList;
	static int maxCount = M_BaseInfoCFG::GetSingleton()->GetData("exchangeMaxCount")->mValue;
	db_player::instance().find(bsonList, DB_EXCHANGE, condition, &retField, maxCount, 0, &g_sortField);
	for(int i = 0; i < bsonList.size(); i++)
	{
		stExchangeInfo info;
		mongo::BSONObj& obj = bsonList[i];
		info.m_genTime   = obj.getField("genTime").Date().toTimeT();
		info.m_chgId  = obj.getIntField("chgId");
		info.m_isReceive = obj.getBoolField("isReceive");
		infoList.push_back(info);
	}

	return msg_type_def::e_rmt_success;
}

void ExchangeSys::_notice(game_player* player)
{
	static const M_MultiLanguageCFGData *pNotify = M_MultiLanguageCFG::GetSingleton()->GetData("ExchangeTelCharge");
	if(pNotify)
	{
		boost::format fmt = boost::format(pNotify->mName) % player->NickName->get_string();

		GLOBAL_SYS(ChatSys)->sysNotify(fmt.str(), msg_type_def::NotifyTypeImportantConsume);
	}
}

