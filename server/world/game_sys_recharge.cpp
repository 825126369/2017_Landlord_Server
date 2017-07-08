#include "stdafx.h"
#include "game_sys_recharge.h"
#include "game_player.h"
#include "M_VIPProfitCFG.h"
#include "M_BaseInfoCFG.h"
#include "time_helper.h"
#include "M_RechangeCFG.h"
#include "proc_c2w_lobby_protocol.h"
#include "proc_logic2world_protocol.h"
#include "game_check.h"
#include "chat_sys.h"
#include "M_MultiLanguageCFG.h"
#include "global_sys_mgr.h"
#include "msg_type_def.pb.h"
#include "pump_type.pb.h"
#include "M_GiftRewardCFG.h"
#include "game_db.h"
#include "operation_activity_sys.h"
#include "M_RechangeExCFG.h"
#include "pump_type.pb.h"
#include "mail_sys.h"
#include "game_check_def.h"
#include "bag_mgr.h"
#include "M_RechangeLotteryCFG.h"
#include <enable_random.h>
#include "game_quest_mgr.h"
#include "game_quest.h"
#include "th_pay_process.h"
#include "world_server.h"
#include "shop_sys.h"

using namespace boost;

game_sys_recharge::game_sys_recharge()
	:m_last_recharge(0)
	,m_last_payid(0)
{
}

game_sys_recharge::~game_sys_recharge()
{
}


void game_sys_recharge::init_sys_object()
{
	VipLevel = CONVERT_POINT(Tfield<int16_t>, get_game_player()->regedit_tfield(e_got_int16, "VipLevel"));
	VipExp = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "VipExp"));
	VipCardEndTime = CONVERT_POINT(Tfield<time_t>, get_game_player()->regedit_tfield(e_got_date, "VipCardEndTime"));
	Recharged = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "recharged"));
	PaymentCheck = get_game_player()->regedit_arrfield("payment_check", check_array::malloc());
	PaymentCheck->get_Tarray<check_array>()->init_array("payment_check");
	RechargeRewardFlag = CONVERT_POINT(Tfield<bool>, get_game_player()->regedit_tfield(e_got_bool, "rechargeRewardFlag"));
	LotteryCount = CONVERT_POINT(Tfield<int16_t>, get_game_player()->regedit_tfield(e_got_int16, "LotteryCount"));
}

void game_sys_recharge::addVipExp(uint32_t exp)
{
	VipExp->add_value(exp);
	
	int old_vip = VipLevel->get_value();

	while (true)
	{
		auto data = M_VIPProfitCFG::GetSingleton()->GetData(VipLevel->get_value());
		if(data == nullptr || data->mVipExp > VipExp->get_value() || data->mVipExp == 0)		
			break;

		VipLevel->add_value(1);
	}	

	if(old_vip<VipLevel->get_value())
	{
		get_game_player()->change_vip(VipLevel->get_value());

		//get_game_player()->get_sys<game_quest_mgr>()->change_quest(e_qt_vip, VipLevel->get_value());

		if(VipLevel->get_value() > 5)
		{
			static const M_MultiLanguageCFGData *pData = M_MultiLanguageCFG::GetSingleton()->GetData("VipNotice");
			if(pData)
			{
				boost::format fmt = boost::format(pData->mName) % get_game_player()->NickName->get_string() % VipLevel->get_value();
				GLOBAL_SYS(ChatSys)->sysNotify(fmt.str(), msg_type_def::NotifyTypeImportantConsume);
			}
		}
	}
	get_game_player()->get_sys<game_quest_mgr>()->change_quest(e_qt_vip, VipLevel->get_value());
}


//////////////////////////////////////////////////////////////////////////

void game_sys_recharge::payment_once(int payid, int rmb, bool isGmOp, bool payment_lottery)
{
	//屏蔽转盘
	payment_lottery = false;
	auto pdata = M_RechangeCFG::GetSingleton()->GetData(payid);

	auto player = get_game_player();

	auto rex = M_RechangeExCFG::GetSingleton()->GetData(player->ChannelID->get_value());
	if(rex != nullptr)
	{
		bool bfind = false;//走后台充值的 会从渠道ID里找不到
		for (int i =0; i <rex->mPayCodes.size(); i++)
		{
			if( rex->mPayCodes[i] == payid)
			{
				payid = i+1;
				pdata = M_RechangeCFG::GetSingleton()->GetData(payid);
				bfind = true;
				break;
			}
		}
	}

	if(pdata != nullptr)
	{		
		GOLD_TYPE rechargeGold = 0;

		//检查合法性
		if(rmb>0 &&pdata->mPrice > rmb)
		{
			pdata = nullptr;
			int maxrmb = 0;
			boost::unordered_map<int, M_RechangeCFGData>& list = M_RechangeCFG::GetSingleton()->GetMapData();
			for (auto it = list.begin(); it != list.end(); ++it)
			{
				if(it->second.mPrice <= rmb && maxrmb < rmb)
				{
					maxrmb = it->second.mPrice;
					pdata = &(it->second);
					payid = pdata->mID;
					break;
				}
			}

			if(pdata == nullptr)
			{
				SLOG_ERROR << "payment_once illegal playerid:"<< player->PlayerId->get_value()<< " channel:"<<player->ChannelID->get_value() <<" pid:"<<payid<<" rmb:"<<rmb;
				return;
			}
		}

		
		boost::format fmt = boost::format("payId:%1%") % payid;

		int reasonRecharge = 0, reasonRechargeSend = 0;
		if(isGmOp) // 是GM通过后台完成的
		{
			reasonRecharge = type_reason_gm_recharge;
			reasonRechargeSend = type_reason_gm_recharge_send;
		}
		else
		{
			reasonRecharge = type_reason_recharge;
			reasonRechargeSend = type_reason_recharge_send;
		}

		int rate = 0;

		if(pdata->mType == 2)//月卡
		{
			auto vipdate = time_helper::convert_to_ptime(VipCardEndTime->get_value());
			auto nowdate = time_helper::instance().get_cur_ptime();
			if(vipdate > nowdate)
			{
				vipdate = vipdate + gregorian::days(30);
			}
			else
			{
				vipdate = nowdate + gregorian::days(30);
			}
			VipCardEndTime->set_value(time_helper::convert_from_ptime(vipdate));	
			_notifygame();

			get_game_player()->get_sys<game_quest_mgr>()->change_quest(e_qt_month_card);
		}
		else if (pdata->mType == 3)	//首冲
		{
			rechargeGold += pdata->mGold;
			get_game_player()->setFirstGift();
		}
		else
		{
			//player->addItem(msg_type_def::e_itd_gold, pdata->mGold, reasonRecharge, fmt.str());
			rechargeGold += pdata->mGold;

			if(payment_lottery)
			{
				rate = _lottery(payid);
				_sendLotteryNotice(rate);
			}
			else
			{
				m_last_payid = payid;
				m_last_recharge = time_helper::instance().get_cur_time();
			}
		}

		int endrate = rate;
		if(endrate == 0)
			endrate = 1;

		auto cmap = PaymentCheck->get_Tarray<check_array>();
		if(cmap->get_check(payid) <=0)
		{
			if(pdata->mFirstGold > 0)
			{
				player->addItem(msg_type_def::e_itd_gold, pdata->mFirstGold, reasonRechargeSend, fmt.str());
			}
			
			cmap->add_check(payid);
			PaymentCheck->set_update();
		}
		
		rechargeGold += pdata->mGiveGold;
		player->addItem(msg_type_def::e_itd_gold, rechargeGold, reasonRecharge, fmt.str());
		player->addItem(msg_type_def::e_itd_gold, rechargeGold * (endrate-1), type_reason_recharge_lottery, fmt.str());

		player->addItem(msg_type_def::e_itd_ticket, pdata->mGiveTicket, reasonRecharge, fmt.str());
		player->addItem(msg_type_def::e_itd_ticket, pdata->mGiveTicket * (endrate-1), type_reason_recharge_lottery, fmt.str());

		addVipExp(pdata->mVIPExp);
		Recharged->add_value(pdata->mPrice);
		_joinMemberMail();
		player->get_sys<BagMgr>()->doActivity(Recharged->get_value());

		// 充值活动
		auto sys = GLOBAL_SYS(OperationActivitySys);
		stActivityEvent evt(activity_type_recharge, pdata->mPrice);
		sys->onPlayerEvent(player, &evt);

		//player->store_game_object();

		db_player::instance().update(DB_TODAY_RECHARGE, BSON("player_id"<<player->PlayerId->get_value()), 
			BSON("$set"<<BSON("nickname"<< player->NickName->get_string() << "VipLevel" << VipLevel->get_value())));
		db_player::instance().update(DB_TODAY_RECHARGE, BSON("player_id"<<player->PlayerId->get_value()), 
			BSON("$inc"<<BSON("total_rmb"<<pdata->mPrice)));

		auto sendmsg = PACKET_CREATE(packetw2c_ask_check_payment_result, e_mst_w2c_ask_check_payment_result);
		sendmsg->set_payid(payid);
		sendmsg->set_lottery_rate(rate);
		player->send_msg_to_client(sendmsg);			

		SLOG_INFO << "payment_once playerid:"<< player->PlayerId->get_value()<< " pid:"<<payid<<" rmb:"<<rmb;

		GLOBAL_SYS(ShopSys)->player_recharge(player, payid);
		_sendReChargeNotice(pdata);
	}		
	else
	{
		SLOG_ERROR<<"can't find M_RechangeCFGData payid:"<<payid << " playerid:"<<player->PlayerId->get_value() << " channel:"<<player->ChannelID->get_value();
	}
}

void game_sys_recharge::payment_once(const std::string& orderid, int pay_type, int pay_value, int rmb)
{
	if(pay_type <=0 ||pay_value<=0 || orderid.empty())//非法充值
		return;

	auto player = get_game_player();
	
	switch (pay_type)
	{
	case 1://金币
		{
			player->addItem(msg_type_def::e_itd_gold, pay_value, type_reason_recharge);
		}
		break;
	case 2://钻石
		{
			player->addItem(msg_type_def::e_itd_ticket, pay_value, type_reason_recharge);
		}
		break;
	case 3:	//月卡
		{
			auto vipdate = time_helper::convert_to_ptime(VipCardEndTime->get_value());
			auto nowdate = time_helper::instance().get_cur_ptime();
			if(vipdate > nowdate)
			{
				vipdate = vipdate + gregorian::days(30);
			}
			else
			{
				vipdate = nowdate + gregorian::days(30);
			}
			VipCardEndTime->set_value(time_helper::convert_from_ptime(vipdate));	
			_notifygame();

			player->get_sys<game_quest_mgr>()->change_quest(e_qt_month_card);

		}
		break;
	default:
		return;
	}

	if(rmb > 0)
	{
		addVipExp(rmb);
		Recharged->add_value(rmb);
	}

	//通知数据库改变状态
	auto pfix = boost::make_shared<th_pay_process>(world_server::instance().get_io_service());
	pfix->init_task(orderid, player->Account->get_string());

	auto sendmsg = PACKET_CREATE(packetw2c_ask_check_payment_result2, e_mst_w2c_ask_check_payment_result2);
	sendmsg->set_pay_type(pay_type);
	sendmsg->set_pay_value(pay_value);
	sendmsg->set_vip_exp(rmb);
	player->send_msg_to_client(sendmsg);	
}
	

void game_sys_recharge::_notifygame()
{
	auto peer = get_game_player()->get_logic();
	if(peer)
	{
		auto sendmsg = PACKET_CREATE(packetw2l_change_player_property, e_mst_w2l_change_player_property);
		sendmsg->set_playerid(get_game_player()->PlayerId->get_value());	
		auto cinfo = sendmsg->mutable_change_info();
		cinfo->set_monthcard_time(VipCardEndTime->get_value());
		peer->send_msg(sendmsg);
	}
}

int game_sys_recharge::getVipProfit(e_vip_type viptype)
{
	auto data = M_VIPProfitCFG::GetSingleton()->GetData(VipLevel->get_value());
	if(data == nullptr)
		return 0;

	switch (viptype)
	{
	case e_evt_OnlineReward:
		return data->mOnlineReward;
		break;
	case e_evt_MaxGiftslimit:
		return data->mMaxGiftslimit;
		break;
	case e_evt_MaxBoxLotterylimit:
		return data->mDailyLottery;
		break;
	}
	return 0;
}

GOLD_TYPE game_sys_recharge::getMaxLimit()
{
	auto data = M_VIPProfitCFG::GetSingleton()->GetData(VipLevel->get_value());
	if(data == nullptr)
		return 0;

	return data->mMaxGiftslimit;
}

int game_sys_recharge::recvRechargeReward()
{
	if(RechargeRewardFlag->get_value())
		return msg_type_def::e_rmt_has_receive_reward;

	if(Recharged->get_value() <= 0)
		return msg_type_def::e_rmt_not_recharge;

	const M_GiftRewardCFGData* data = M_GiftRewardCFG::GetSingleton()->GetData(2);
	if(data == nullptr)
	{
		SLOG_ERROR << boost::format("game_sys_recharge::recvRechargeReward, 找不到充值礼包奖励类型[2]");
		return msg_type_def::e_rmt_unknow;
	}

	if(data->mItemId.size() != data->mItemCount.size())
	{
		SLOG_ERROR << boost::format("game_sys_recharge::recvRechargeReward, 大小不一致");
		return msg_type_def::e_rmt_unknow;
	}

	auto player = get_game_player();

	for(int i = 0; i < (int)data->mItemId.size(); i++)
	{
		player->addItem(data->mItemId[i], data->mItemCount[i], type_reason_recharge_gift);
	}

	RechargeRewardFlag->set_value(true);
	//get_game_player()->store_game_object();
	return msg_type_def::e_rmt_success;
}

int game_sys_recharge::getMonthCardRemainSecondTime()
{
	if(VipCardEndTime->get_value() == 0)
		return -1;

	int curTime = time_helper::instance().get_cur_time();
	if(curTime >= VipCardEndTime->get_value())
		return 0;

	return VipCardEndTime->get_value() - curTime;
}

int game_sys_recharge::getMonthCardRemainSecondTime(time_t curTime)
{
	if(VipCardEndTime->get_value() == 0)
		return -1;

	if(curTime >= VipCardEndTime->get_value())
		return 0;

	return VipCardEndTime->get_value() - curTime;
}

void game_sys_recharge::addVipCardDays(int days, bool save)
{
	auto vipdate = time_helper::convert_to_ptime(VipCardEndTime->get_value());
	auto nowdate = time_helper::instance().get_cur_ptime();
	if(vipdate > nowdate)
	{
		vipdate = vipdate + gregorian::days(days);
	}
	else
	{
		vipdate = nowdate + gregorian::days(days);
	}
	VipCardEndTime->set_value(time_helper::convert_from_ptime(vipdate));
	
	if(save)
	{
		//get_game_player()->store_game_object();
	}
}

bool game_sys_recharge::isBuyItem(int32_t payid)
{
	auto payList = PaymentCheck->get_array();
	for (auto it = payList->begin(); it != payList->end(); ++it)
	{
		if ((*it)->get_id() == payid)
		{
			return true;
		}
	}
	return false;
}

void game_sys_recharge::_joinMemberMail()
{
	static int silverM = M_BaseInfoCFG::GetSingleton()->GetData("silverMember")->mValue;
	static int goldM = M_BaseInfoCFG::GetSingleton()->GetData("goldMember")->mValue;
	if(Recharged->get_value() >= goldM)
	{
		auto player = get_game_player();
		int isSend = player->CheckMap->get_Tmap<check_map>()->get_check(check_gold_member);
		if(!isSend)
		{
			int retCode = _sendMail(player->PlayerId->get_value(), "Mail_Title_2", "Mail_Text_2");
			if(retCode == msg_type_def::e_rmt_success)
			{
				player->CheckMap->get_Tmap<check_map>()->add_check(check_gold_member);
			}
		}
	}
	if(Recharged->get_value() >= silverM)
	{
		auto player = get_game_player();
		int isSend = player->CheckMap->get_Tmap<check_map>()->get_check(check_silver_member);
		if(!isSend)
		{
			int retCode = _sendMail(player->PlayerId->get_value(), "Mail_Title_1", "Mail_Text_1");
			if(retCode == msg_type_def::e_rmt_success)
			{
				player->CheckMap->get_Tmap<check_map>()->add_check(check_silver_member);
			}
		}
	}
}

const std::string EMPTY = "";

int game_sys_recharge::_sendMail(int playerId, const std::string& title, const std::string& content)
{
	const std::string *pTitle = &EMPTY;
	const std::string *pContent = &EMPTY;

	const M_MultiLanguageCFGData* pTitleData = M_MultiLanguageCFG::GetSingleton()->GetData(title);
	const M_MultiLanguageCFGData* pContentData = M_MultiLanguageCFG::GetSingleton()->GetData(content);
	const M_MultiLanguageCFGData* pSender = M_MultiLanguageCFG::GetSingleton()->GetData("Mail_From");
	if(pTitleData)
	{
		pTitle = &pTitleData->mName;
	}
	if(pContentData)
	{
		pContent = &pContentData->mName;
	}

	return GLOBAL_SYS(MailSys)->sendMail(*pTitle, pSender->mName, *pContent, 0, playerId, 0);
}

int game_sys_recharge::_lottery(int payid)
{
	int ret = 0;
	if(m_last_payid == payid)//判定抽奖
	{
		time_t nowt = time_helper::instance().get_cur_time();
		if(m_last_recharge + 900 > nowt)//时间判定15分钟内
		{
			auto list = M_RechangeLotteryCFG::GetSingleton()->GetMapData();
			int randv = global_random::instance().rand_1w();
			int tempv = 999999;

			for (auto it = list.begin(); it!= list.end(); ++it)
			{
				M_RechangeLotteryCFGData& rdata = it->second;
				if(LotteryCount->get_value() > 0)
				{
					if(rdata.mDefaultRate>0 &&rdata.mDefaultRate > randv && tempv >= rdata.mDefaultRate)
					{
						tempv = rdata.mDefaultRate;
						ret = rdata.mMuch;
					}
				}
				else
				{
					if(rdata.mFirstRate>0 &&rdata.mFirstRate > randv && tempv >= rdata.mFirstRate)
					{
						tempv = rdata.mFirstRate;
						ret = rdata.mMuch;
					}
				}			
			}

			LotteryCount->add_value(1);
			m_last_payid = 0;
			m_last_recharge = 0;
			return ret;
		}
	}

	m_last_payid = payid;
	m_last_recharge = time_helper::instance().get_cur_time();

	return ret;
}

void game_sys_recharge::_sendLotteryNotice(int rate)
{
	if(rate >= 2)
	{
		static const M_MultiLanguageCFGData *pData = M_MultiLanguageCFG::GetSingleton()->GetData("RechargeLotteryNotice");
		if(pData)
		{
			boost::format fmt = boost::format(pData->mName) % get_game_player()->NickName->get_string() % (rate + 1);
			GLOBAL_SYS(ChatSys)->sysNotify(fmt.str(), msg_type_def::NotifyTypeWinningPrize);
		}
	}
}

void game_sys_recharge::_sendReChargeNotice(const M_RechangeCFGData *data)
{
	if(data == nullptr)
		return;

	static const int rechargeNum = M_BaseInfoCFG::GetSingleton()->GetData("rechargeNumSpeaker")->mValue;
	if(data->mPrice >= rechargeNum)
	{			
		boost::format fmt;
		if(data->mType == 2) // 购买了月卡
		{
			static const M_MultiLanguageCFGData *pNotify = M_MultiLanguageCFG::GetSingleton()->GetData("RechargeNotify04");
			if(pNotify)
			{
				fmt = boost::format(pNotify->mName) 
					% VipLevel->get_value() 
					% get_game_player()->NickName->get_string() 
					% data->mPrice;
			}
		}
		else if(data->mGold > 0) // 买的是金币
		{
			GOLD_TYPE sendGold = data->mFirstGold + data->mGiveGold;
			if(sendGold > 0) // 送的金币
			{
				static const M_MultiLanguageCFGData *pNotify = M_MultiLanguageCFG::GetSingleton()->GetData("RechargeNotify01");
				if(pNotify)
				{
					fmt = boost::format(pNotify->mName) 
						% VipLevel->get_value() 
						% get_game_player()->NickName->get_string()
						% data->mPrice
						% data->mGold
						% sendGold;
				}
			}
			else
			{
				static const M_MultiLanguageCFGData *pNotify = M_MultiLanguageCFG::GetSingleton()->GetData("RechargeNotify03");
				if(pNotify)
				{
					fmt = boost::format(pNotify->mName) 
						% VipLevel->get_value() 
						% get_game_player()->NickName->get_string() 
						% data->mPrice
						% data->mGold;
				}
			}
		}
		else if(data->mGiveTicket > 0) // 买的是钻石
		{
			static const M_MultiLanguageCFGData *pNotify = M_MultiLanguageCFG::GetSingleton()->GetData("RechargeNotify02");
			if(pNotify)
			{
				fmt = boost::format(pNotify->mName) 
					% VipLevel->get_value() 
					% get_game_player()->NickName->get_string() 
					% data->mPrice
					% data->mGiveTicket;
			}
		}

		if(fmt.size() > 0)
		{
			GLOBAL_SYS(ChatSys)->sysNotify(fmt.str(), msg_type_def::NotifyTypeImportantConsume);
		}
	}
}


#include "M_ShopCFG.h"
int game_sys_recharge::shopping(int shopid)
{
	auto player = get_game_player();
	auto info  =M_ShopCFG::GetSingleton()->GetData(shopid);
	if(info != nullptr)
	{
		if(player->Ticket->get_value() < info->mPrice)
			return 11;
		else
		{
			player->Ticket->add_value(-info->mPrice);
			switch (info->mType)
			{
			case 1:
			case 3:
				{
					player->addItem(msg_type_def::e_itd_gold, info->mGold, type_reason_shopping);

					if(info->mType == 3)
						player->setFirstGift();
				}
				break;
			case 2:
				{
					auto vipdate = time_helper::convert_to_ptime(VipCardEndTime->get_value());
					auto nowdate = time_helper::instance().get_cur_ptime();
					if(vipdate > nowdate)
					{
						vipdate = vipdate + gregorian::days(30);
					}
					else
					{
						vipdate = nowdate + gregorian::days(30);
					}
					VipCardEndTime->set_value(time_helper::convert_from_ptime(vipdate));	
					if (info->mGold > 0)
					{
						player->addItem(msg_type_def::e_itd_gold, info->mGold, type_reason_shopping);
					}
					_notifygame();

					player->get_sys<game_quest_mgr>()->change_quest(e_qt_month_card);
				}
				break;
			default:
				return 0;
				break;
			}			
			return 1;
		}
	}

	return 0;
}