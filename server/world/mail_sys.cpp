#include "stdafx.h"
#include "mail_sys.h"
#include "time_helper.h"
#include "msg_type_def.pb.h"
#include "game_db.h"
#include "boost/lexical_cast.hpp"
#include "game_player.h"
#include "M_GiftCFG.h"
#include "M_MultiLanguageCFG.h"
#include "M_ItemCFG.h"
#include "game_sys_recharge.h"
#include "pump_type.pb.h"
#include "player_log_mgr.h"
#include "M_BaseInfoCFG.h"
#include "global_sys_mgr.h"
#include "pump_sys.h"

int MailSys::sendMail(const std::string& title, 
					  const std::string& sender,
					  const std::string& content,
					  int senderId,
					  int toPlayerId, 
					  int durationDay, 
					  std::vector<stGift>* items, 
					  std::string *mailId
					  ,int needCoin)
{
	mongo::BSONObj obj;
	buildMail(title, sender, content, senderId, toPlayerId, durationDay, obj, items, mailId, needCoin);
	const std::string& res = db_player::instance().insert(DB_MAIL, obj);
	return res.empty() ? msg_type_def::e_rmt_success : msg_type_def::e_rmt_fail;
}

int MailSys::receiveGift(game_player* player, const std::string& mailId, std::vector<stGift>* resGifts)
{
	if(player == NULL)
	{
		SLOG_ERROR << boost::format("MailSys::receiveItem，指针空!");
		return msg_type_def::e_rmt_unknow;
	}

	mongo::OID tmpMailId;
	tmpMailId.init(mailId);

	mongo::BSONObj search = BSON("playerId" << player->PlayerId->get_value() << "_id" << tmpMailId << "isReceive" << false);
	mongo::BSONObj ret = db_player::instance().findone(DB_MAIL, search);
	if(ret.isEmpty()) 
	{
		return msg_type_def::e_rmt_fail;
	}

	if(!ret.hasField("gifts"))
	{
		db_player::instance().update(DB_MAIL, search, BSON("$set" << BSON("isReceive" << true)));
		return msg_type_def::e_rmt_success; 
	}

	int senderId = ret.getIntField("senderId");
	std::string senderIdStr = boost::lexical_cast<std::string>(senderId) + ":" + mailId;
	std::vector<stGift> itemList;

	mongo::BSONObj items = ret.getObjectField("gifts");
	for (int i = 0; i < items.nFields(); i++)
	{
		mongo::BSONObj  tmp = items.getObjectField(boost::lexical_cast<std::string>(i));
		stGift gift;
		gift.m_giftId = tmp.getIntField("giftId");
		gift.m_count = tmp.getField("count").Long();
		if(tmp.hasField("receive"))
		{
			gift.m_receive = tmp.getBoolField("receive");
		}
		else
		{
			gift.m_receive = false;
		}
		if(resGifts)
		{
			resGifts->push_back(gift);
		}
		itemList.push_back(gift);
	}

	int code = 0;
	int count = 0;

	for(int i = 0; i < itemList.size(); i++)
	{
		if(itemList[i].m_receive)
		{
			count++;
			continue;
		}

		code = player->addItem(itemList[i].m_giftId, itemList[i].m_count, type_reason_receive_mail, senderIdStr);
		if(code == msg_type_def::e_rmt_success)
		{
			boost::format fmt = boost::format("gifts.%1%.receive") % i;
			db_player::instance().update(DB_MAIL, search, BSON("$set" << BSON(fmt.str() << true)));
			count++;
		}
	}
	if(!itemList.empty())
	{
		//player->store_game_object();
	}

	if (senderId > 0)
	{
		static int sendGiftLogMaxCount = M_BaseInfoCFG::GetSingleton()->GetData("sendGiftLogMaxCount")->mValue;
		player->get_sys<PlayerLogMgr>()->addSendGiftLog(senderId, itemList[0].m_giftId, itemList[0].m_count, mailId, false, sendGiftLogMaxCount);
	}

	if(count >= (int)itemList.size())
	{
		db_player::instance().update(DB_MAIL, search, BSON("$set" << BSON("isReceive" << true)));
		return msg_type_def::e_rmt_success;
	}

	
	return msg_type_def::e_rmt_beyond_limit;
}

int MailSys::removeMail(game_player* player, const std::string& mailId)
{	
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	mongo::OID tmpMailId;
	tmpMailId.init(mailId);

	mongo::BSONObj search = BSON("_id" << tmpMailId);
	mongo::BSONObj ret = db_player::instance().findone(DB_MAIL, search);
	if(!ret.isEmpty()) 
	{
		if (ret.hasField("isReceive"))
		{
			bool isReceive = ret.getBoolField("isReceive");
			if (!isReceive)
			{
				return msg_type_def::e_rmt_fail;
			}
		}
	}
	else
	{
		return msg_type_def::e_rmt_fail;
	}

	db_player::instance().remove(DB_MAIL, BSON("playerId" << player->PlayerId->get_value() << "_id" << tmpMailId), true);
	return msg_type_def::e_rmt_success;
}

int MailSys::getMail(game_player* player, std::vector<stMail>& res, time_t t, time_t* Last)
{
	if(player == NULL)
	{
		SLOG_ERROR << boost::format("MailSys::getMail指针空!");
		return msg_type_def::e_rmt_unknow;
	}

	/*if(t == 0)
	{
		time_t curT = time_helper::instance().get_cur_time();
		mongo::BSONObj delCond = BSON("playerId" << player->PlayerId->get_value() << "deadTime" << mongo::LT << mongo::Date_t(curT * 1000));
		// 删除过期邮件
		db_player::instance().remove(DB_MAIL, delCond, false);
	}*/

	static mongo::BSONObj s_sortField = BSON("time" << -1);

	mongo::BSONObj condition = BSON("playerId" << player->PlayerId->get_value() << "time" << mongo::GT << mongo::Date_t(t * 1000));
	std::vector<mongo::BSONObj> mail;
	db_player::instance().find(mail, DB_MAIL, condition, nullptr, 100, 0, &s_sortField);
	time_t max_t = 0;
	for(int i = 0; i < mail.size(); i++)
	{
		stMail m;
		mongo::BSONObj& b = mail[i];
		m.m_mailId = b.getField("_id").OID().toString();
		m.m_time   = b.getField("time").Date().toTimeT();
		m.m_title  = b.getStringField("title");
		m.m_sender = b.getStringField("sender");
		m.m_content = b.getStringField("content");
		m.m_isRecvive = b.getBoolField("isReceive");
		if(b.hasField("senderId"))
		{
			m.m_senderId = b.getIntField("senderId");
		}
		else
		{
			m.m_senderId = 0;
		}
		if(m.m_time > max_t)
		{
			max_t = m.m_time;
		}
		if(b.hasField("gifts"))
		{
			mongo::BSONObj obj = b.getObjectField("gifts");
			for (int j = 0; j < obj.nFields(); j++)
			{
				mongo::BSONObj tmp = obj.getObjectField(boost::lexical_cast<std::string>(j));
				stGift item;
				item.m_giftId = tmp.getIntField("giftId");
				item.m_count  = tmp.getField("count").Long();
				m.m_items.push_back(item);
			}
		}
		res.push_back(m);
	}

	if(Last && mail.size() > 0)
	{
		*Last = max_t;
	}
	return msg_type_def::e_rmt_success;
}


int MailSys::buildMail(const std::string& title, 
						   const std::string& sender, 
						   const std::string& content, 
						   int senderId,
						   int toPlayerId, 
						   int durationDay, 
						   mongo::BSONObj& mailResult, 
						   std::vector<stGift>* items,
						   std::string *mailId
						   ,int needCoin)
{
	mongo::OID oid;
	oid.init();

	mongo::BSONObjBuilder builder;
	// 玩家id
	builder.append("playerId", toPlayerId);
	builder.append("senderId", senderId);
	if(mailId)
	{
		*mailId = oid.toString();
	}
	// 邮件ID
	builder.appendOID("_id", &oid);

	// 发送时间
	builder.appendTimeT("time", time_helper::instance().get_cur_time());

	if(durationDay == 0)
	{
		durationDay = 3650;
	}
	boost::posix_time::ptime deadTime = time_helper::instance().get_cur_ptime() + boost::gregorian::days(durationDay);
	builder.appendTimeT("deadTime", time_helper::instance().convert_from_ptime(deadTime));

	// 邮件标题
	builder.append("title", title);
	// 发件人
	builder.append("sender", sender);
	// 邮件内容
	builder.append("content", content);
	// 是否已领取道具
	builder.appendBool("isReceive", false);
	if(items)
	{
		mongo::BSONArrayBuilder arr; 
		for(int i = 0; i < items->size(); i++)
		{
			mongo::BSONObjBuilder b;
			// 礼物ID
			b.append("giftId", items->at(i).m_giftId);
			// 数量
			b.append("count", items->at(i).m_count);
			b.append("receive", false);
			arr.append(b.done());
		}
		builder.append("gifts", arr.done());
	}

	builder.append("totalcoin", needCoin);
	mailResult = builder.done().copy();
	return msg_type_def::e_rmt_success;
}

int MailSys::sendMail(std::vector<mongo::BSONObj>& mailList)
{
	const std::string& res = db_player::instance().insert(DB_MAIL, mailList);
	return res.empty() ? msg_type_def::e_rmt_success : msg_type_def::e_rmt_fail;
}

std::string g_emptyStr;

int MailSys::sendGift(game_player* player, 
					  const std::string& title, 
					  int toPlayerId, 
					  int durationDay, 
					  std::vector<stGift>& gifts, 
					  std::string *mailId)
{
	if(gifts.empty() || player == nullptr)
		return msg_type_def::e_rmt_unknow;

	//游戏中不能送礼
	if(player->is_gaming())
		return msg_type_def::e_rmt_unknow;

	if(player->PlayerId->get_value() == toPlayerId)
		return msg_type_def::e_rmt_cannot_sendto_self;
	
	auto vipMgr = player->get_sys<game_sys_recharge>();
	GOLD_TYPE maxLimit = vipMgr->getMaxLimit();

	std::vector<stGift> itemList;
	GOLD_TYPE needCoin = 0;
	for(int i = 0; i < (int)gifts.size(); i++)
	{
		const M_GiftCFGData* data = M_GiftCFG::GetSingleton()->GetData(gifts[i].m_giftId);
		if(data == nullptr)
			return msg_type_def::e_rmt_unknow;

		const M_ItemCFGData* pItem = M_ItemCFG::GetSingleton()->GetData(data->mGiftId);
		if(pItem)
		{
			if(pItem->mItemCategory != msg_type_def::e_itd_gift)
			{
				SLOG_ERROR << boost::format("MailSys::sendGift, 商品[%1%], 道具[%2%]不是礼物") % gifts[i].m_giftId % pItem->mItemId;
				return msg_type_def::e_rmt_unknow;
			}
		}
		else
		{
			SLOG_ERROR << boost::format("MailSys::sendGift, 商品[%1%], 找不到对应道具[%2%]") % gifts[i].m_giftId % data->mGiftId;
			return msg_type_def::e_rmt_unknow;
		}
		if(vipMgr->VipLevel->get_value() < data->mVip)
			return msg_type_def::e_rmt_vip_under;

		if(gifts[i].m_count <= 0)
			return msg_type_def::e_rmt_beyond_limit;

		needCoin += data->mCoin * gifts[i].m_count;
		itemList.push_back(stGift(data->mGiftId, gifts[i].m_count));
	}
	
	if(player->Gold->get_value() < needCoin)
		return msg_type_def::e_rmt_gold_not_enough;

	if(player->SendGiftCoinCount->get_value() + needCoin > maxLimit)
		return msg_type_def::e_rmt_beyond_limit;

	const std::string* pContent = &g_emptyStr;
	static const M_MultiLanguageCFGData* data = M_MultiLanguageCFG::GetSingleton()->GetData("SendGiftContent");
	if(data)
	{
		pContent = &data->mName;
	}
	else
	{
		SLOG_ERROR << boost::format("找不到多语言[SendGiftContent]");
	}

	const std::string* giftTitle = &title;
	static const M_MultiLanguageCFGData* data1 = M_MultiLanguageCFG::GetSingleton()->GetData("SendGiftTitle");
	if(data1)
	{
		giftTitle = &data1->mName;
	}

	int result = sendMail(*giftTitle, player->NickName->get_string(), *pContent, 
		player->PlayerId->get_value(), toPlayerId, durationDay, &itemList, mailId, needCoin);
	if(result == msg_type_def::e_rmt_success)
	{
		boost::format fmt = boost::format("giftId:%1%,toPlayerId:%2%") % itemList[0].m_giftId % toPlayerId;
		player->addItem(msg_type_def::e_itd_gold, -needCoin, type_reason_send_gift, fmt.str());
		player->SendGiftCoinCount->add_value(needCoin);
		//player->store_game_object();

		if (itemList[0].m_giftId == 30107)//交易金币
		{
			GLOBAL_SYS(PumpSys)->pumpSendGold1(*mailId, player, needCoin, toPlayerId);
		}
	}
	return result;
}
