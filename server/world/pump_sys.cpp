#include "stdafx.h"
#include "pump_sys.h"
#include "game_db_log.h"
#include "game_player.h"
#include "game_db.h"
#include "time_helper.h"
#include "msg_type_def.pb.h"
#include "gift_def.h"
#include "M_ItemCFG.h"
#include "game_sys_recharge.h"
#include "pump_type.pb.h"
#include "game_player_mgr.h"

void PumpSys::init_sys_object()
{
	m_checkTime = 0;
	m_maxOnlinePlayerNum = 0;
}

void PumpSys::sys_time_update()
{
	// 0点清空这个记录
	db_player::instance().clearTable(DB_PLAYER_FAVOR);

	m_maxOnlinePlayerNum = 0;
}

void PumpSys::sys_update(double delta)
{
	m_checkTime += delta;
	if(m_checkTime > 60)
	{
		m_checkTime = 0;
		int cur = game_player_mgr::instance().get_player_map().size();
		if(cur > m_maxOnlinePlayerNum)
		{
			m_maxOnlinePlayerNum = cur;

			auto now = time_helper::instance().get_cur_date();
			time_t nt = time_helper::convert_from_date(now) * 1000;

			mongo::BSONObjBuilder builder;
			builder.appendTimeT("maxTimePoint", time_helper::instance().get_cur_time());
			builder.append("playerNum", m_maxOnlinePlayerNum);

			mongo::BSONObj cond = BSON("date" << mongo::Date_t(nt));
			db_log::instance().push_update(e_dlt_max_online_player, cond, BSON("$set" << builder.done()));
		}
	}
}

static mongo::BSONObj g_retField = BSON("playerId" << 1);

void PumpSys::enterGame(int gameId, int playerId)
{
	_activeCount(gameId);
	_activePerson(gameId, playerId);
}

void PumpSys::moneyTotalLog(game_player* player, int gameId, int itemId, GOLD_TYPE addValue, int reason, const std::string& param)
{
	if(player == nullptr)
		return;

	if(addValue == 0)
		return;

	if(player->IsRobot->get_value())
		return;

	GOLD_TYPE oldValue = 0, newValue = 0;

	switch (itemId)
	{
	case msg_type_def::e_itd_gold:
		{
			oldValue = player->Gold->get_value() - addValue;
			newValue = player->Gold->get_value();
		}
		break;
	case msg_type_def::e_itd_ticket:
		{
			oldValue = player->Ticket->get_value() - addValue;
			newValue = player->Ticket->get_value();
		}
		break;
	case msg_type_def::e_itd_chip:
		{
			oldValue = player->Chip->get_value() - addValue;
			newValue = player->Chip->get_value();
		}
		break;
	default:
		return;
		break;
	}

	mongo::BSONObjBuilder builder;

	builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
	builder.append("playerId", player->PlayerId->get_value());
	builder.append("gameId", gameId);
	builder.append("itemId", itemId);
	builder.append("oldValue", oldValue);
	builder.append("newValue", newValue);
	builder.append("addValue", addValue);
	builder.append("reason", reason);
	builder.append("param",param);

	db_log::instance().push_insert(e_dlt_pump_player_money, builder.done());

	_recordCoinGrowth(itemId, addValue, player);

	_statDailyMoney(itemId, addValue, reason);
	/*int changeType = 0;
	int64_t val = 0;
	if(addValue > 0)
	{
		changeType = 0; // 收入
		val = addValue;
	}
	else
	{
		changeType = 1; // 支出
		val = -addValue;
	}


	auto now = time_helper::instance().get_cur_date();
	time_t nt = time_helper::convert_from_date(now) * 1000;

	mongo::BSONObj cond = BSON("time" << mongo::Date_t(nt)
		<< "reason" << reason << "itemId" << itemId << "changeType" << changeType);
	db_log::instance().push_update(e_dlt_pump_total_consume, cond, BSON("$inc" << BSON("value" << val)));
	*/
}

void PumpSys::_statDailyMoney(int itemId, int addValue, int reason)
{
	int changeType = 0;
	int64_t val = 0;
	if(addValue > 0)
	{
		changeType = 0; // 收入
		val = addValue;
	}
	else
	{
		changeType = 1; // 支出
		val = -addValue;
	}
	
	mongo::BSONObj upData;
	switch(reason)
	{
	case type_reason_dial_lottery:  // 每日登录转盘抽奖
	case type_reason_daily_sign:	// 每日签到
	case type_reason_daily_box_lottery: // 每日宝箱抽奖
	case type_reason_recharge_lottery: // 充值抽奖
		{
			upData = BSON("$inc" << BSON("value" << val << "count" << 1));
		}
		break;
	default:
		{
			upData = BSON("$inc" << BSON("value" << val));
		}
		break;
	}

	auto now = time_helper::instance().get_cur_date();
	time_t nt = time_helper::convert_from_date(now) * 1000;

	mongo::BSONObj cond = BSON("time" << mongo::Date_t(nt)
		<< "reason" << reason << "itemId" << itemId << "changeType" << changeType);
	db_log::instance().push_update(e_dlt_pump_total_consume, cond, upData);
}

void PumpSys::moneyTotalLog(game_player* player, int gameId, int itemId, int reason, const std::string& param)
{
	if(player == nullptr)
		return;

	GOLD_TYPE oldValue = 0, newValue = 0;
	GOLD_TYPE addValue = 0;
	switch (itemId)
	{
	case msg_type_def::e_itd_gold:
		{
			oldValue = player->Gold->get_value();
			newValue = player->Gold->get_value();
		}
		break;
	case msg_type_def::e_itd_ticket:
		{
			oldValue = player->Ticket->get_value();
			newValue = player->Ticket->get_value();
		}
		break;
	default:
		return;
		break;
	}

	mongo::BSONObjBuilder builder;

	builder.appendTimeT("genTime", time_helper::instance().get_cur_time());
	builder.append("playerId", player->PlayerId->get_value());
	builder.append("gameId", gameId);
	builder.append("itemId", itemId);
	builder.append("oldValue", oldValue);
	builder.append("newValue", newValue);
	builder.append("addValue", addValue);
	builder.append("reason", reason);
	builder.append("param",param);

	db_log::instance().push_insert(e_dlt_pump_player_money, builder.done());
}

void PumpSys::addGeneralStatLog(int statType)
{
	_generalStat(statType, e_dlt_pump_general_stat);
}

void PumpSys::buyItemLog(int itemId)
{
	const M_ItemCFGData* data = M_ItemCFG::GetSingleton()->GetData(itemId);
	if(data == nullptr)
		return;

	if(data->mItemCategory != msg_type_def::e_itd_photoframe) // 统计相框
		return;

	_generalStat(itemId, e_dlt_pump_photo_frame);
}

void PumpSys::sendGiftLog(int giftId)
{
	_generalStat(giftId, e_dlt_pump_send_gift);
}

void PumpSys::sendGiftLog(std::vector<stGift>& giftList)
{
	for(auto it = giftList.begin(); it != giftList.end(); ++it)
	{
		sendGiftLog(it->m_giftId);
	}
}

void PumpSys::sendGiftLog(int senderId, int receiverId, stGift& giftInfo)
{
	mongo::BSONObjBuilder builder;
	time_t curT = time_helper::instance().get_cur_time();

	builder.appendTimeT("sendTime", curT);
	builder.append("senderId", senderId);
	builder.append("receiverId", receiverId);
	builder.append("giftId", giftInfo.m_giftId);
	builder.append("count", giftInfo.m_count);

	db_log::instance().push_insert(e_dlt_pump_personal_send_gift, builder.done());
}

void PumpSys::pumpSendGold1(const std::string& mailid, game_player* sender, GOLD_TYPE sendgold, int recverid)
{
	mongo::BSONObjBuilder builder;
	time_t curT = time_helper::instance().get_cur_time();

	builder.append("MailId", mailid);
	builder.appendTimeT("SendTime", curT);
	builder.append("SenderId", sender->PlayerId->get_value());
	builder.append("SenderName", sender->NickName->get_string());
	builder.append("SenderIsGoldShop", sender->is_goldshop());
	builder.append("SendGold", sendgold);
	builder.append("RecverId", recverid);

	db_log::instance().push_insert(e_dlt_pump_send_gold, builder.obj());
}

void PumpSys::pumpSendGold2(const std::string& mailid, game_player* recver, GOLD_TYPE recvgold, GOLD_TYPE CounterFee)
{
	mongo::BSONObjBuilder builder;
	time_t curT = time_helper::instance().get_cur_time();
	builder.appendTimeT("RecvTime", curT);	
	builder.append("RecvName", recver->NickName->get_string());
	builder.append("RecverIsGoldShop", recver->is_goldshop());
	builder.append("RecvGold", recvgold);
	builder.append("CounterFee", CounterFee);

	db_log::instance().push_update(e_dlt_pump_send_gold, BSON("MailId" << mailid), BSON("$set" << builder.obj()));
}

void PumpSys::_activeCount(int gameId)
{
	//mongo::BSONObjBuilder builder;
	//time_t curT = time_helper::instance().get_cur_time();

	auto now = time_helper::instance().get_cur_date();
	time_t nt = time_helper::convert_from_date(now) * 1000;

	mongo::BSONObj cond = BSON("genTime" << mongo::Date_t(nt) << "gameId" << gameId);
	//builder.appendTimeT("genTime", curT);
	//builder.append("gameId", gameId);
	//BSON("$inc" << BSON("value" << 1)

	db_log::instance().push_update(e_dlt_pump_active_count, cond, BSON("$inc" << BSON("value" << 1)));

	//db_log::instance().push_insert(e_dlt_pump_active_count, builder.done());
}

void PumpSys::_activePerson(int gameId, int playerId)
{
	mongo::BSONObj cond = BSON("gameId" << gameId << "playerId" << playerId);

	mongo::BSONObj obj = db_player::instance().findone(DB_PLAYER_FAVOR, cond, &g_retField);
	if(!obj.isEmpty())
		return;

	//mongo::BSONObjBuilder builder;
	//time_t curT = time_helper::instance().get_cur_time();

	//builder.appendTimeT("genTime", curT);
	//builder.append("gameId", gameId);

	//db_log::instance().push_insert(e_dlt_pump_active_person, builder.done());

	auto now = time_helper::instance().get_cur_date();
	time_t nt = time_helper::convert_from_date(now) * 1000;
	mongo::BSONObj cond1 = BSON("genTime" << mongo::Date_t(nt) << "gameId" << gameId);
	db_log::instance().push_update(e_dlt_pump_active_person, cond1, BSON("$inc" << BSON("value" << 1)));

	// 记录该玩家已登入
	mongo::BSONObjBuilder bud;
	bud.append("gameId", gameId);
	bud.append("playerId", playerId);
	db_player::instance().insert(DB_PLAYER_FAVOR, bud.done());
}

void PumpSys::_generalStat(int keyValue, int tableType)
{
	//const std::string& tableName = db_log::instance().get_tablename(tableType);
	mongo::BSONObj cond = BSON("key" << keyValue);
	/*mongo::BSONObj b = db_log::instance().findone(tableName, cond);
	if(b.isEmpty())
	{
		mongo::BSONObjBuilder builder;
		builder.append("key", keyValue);
		builder.append("value", 1);

		db_log::instance().push_insert(tableType, builder.done());

	}
	else*/
	{
		db_log::instance().push_update(tableType, cond, BSON("$inc" << BSON("value" << 1)));
	}
}

void PumpSys::_recordCoinGrowth(int itemId, int addValue, game_player* player)
{
//	if(addValue < 0)
//		return;

	if(itemId == msg_type_def::e_itd_gold)
	{
		mongo::BSONObj cond = BSON("playerId" << player->PlayerId->get_value());
		int64_t v = addValue;
		db_log::instance().push_update(e_dlt_pump_coin_growth, cond, BSON( "$inc" << BSON("gold" << v) ));

		db_log::instance().push_update(e_dlt_pump_coin_growth, cond, BSON("$set" << BSON(
			 "nickName" << player->NickName->get_string() 
			<< "vipLevel" << player->get_viplvl() )
			));
	}
}
