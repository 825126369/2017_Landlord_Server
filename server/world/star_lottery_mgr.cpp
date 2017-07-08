#include "stdafx.h"
#include "star_lottery_mgr.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "M_BaseInfoCFG.h"
#include "msg_type_def.pb.h"
#include "pump_type.pb.h"
#include "M_StarLotteryCFG.h"
#include "enable_random.h"
#include "game_sys_recharge.h"
#include "game_db.h"
#include "star_lottery_sys.h"
#include "M_FreeLotteryCFG.h"
#include "game_quest_mgr.h"
#include "game_quest.h"
#include "M_MultiLanguageCFG.h"
#include "chat_sys.h"

StarLotteryMgr::StarLotteryMgr()
{
}

StarLotteryMgr::~StarLotteryMgr()
{
}

void StarLotteryMgr::init_sys_object()
{
	CurStar = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "CurStar"));
	CurAward = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "CurAward"));
	TotalChip = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "TotalChip"));
	CurCount = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "CurCount"));
}

bool StarLotteryMgr::Lottery(stStarResult& result)
{
	static int StarMax = M_BaseInfoCFG::GetSingleton()->GetData("StarMax")->mValue;	
	static GOLD_TYPE StarLvl2 = M_FreeLotteryCFG::GetSingleton()->GetData(2)->mBaseGold;
	static GOLD_TYPE StarLvl3 = M_FreeLotteryCFG::GetSingleton()->GetData(3)->mBaseGold;
	static GOLD_TYPE StarLvl4 = M_FreeLotteryCFG::GetSingleton()->GetData(4)->mBaseGold;
	static GOLD_TYPE StarLvl5 = M_FreeLotteryCFG::GetSingleton()->GetData(5)->mBaseGold;


	if(StarMax > CurStar->get_value())
		return false;
	
	std::string param = "starlvl:";
	if(CurAward->get_value() >= StarLvl5)
	{
		_lottery(5, result);
		param += "5";
	}
	else if(CurAward->get_value() >= StarLvl4)
	{
		_lottery(4, result);
		param += "4";
	}
	else if(CurAward->get_value() >= StarLvl3)
	{
		_lottery(3, result);
		param += "3";
	}
	else if(CurAward->get_value() >= StarLvl2)
	{
		_lottery(2, result);
		param += "2";
	}
	else
	{
		_lottery(1, result);
		param += "1";
	}

	get_game_player()->addItem(result.itemtype, result.itemcount, type_reason_star_lottery, param);
	get_game_player()->addItem(msg_type_def::e_itd_gold, CurAward->get_value(), type_reason_star_award, param);

	
	result.award = CurAward->get_value();
	CurAward->set_value(0);
	CurStar->set_value(0);

	get_game_player()->get_sys<game_quest_mgr>()->change_quest(e_qt_star_lottery);

	//get_game_player()->store_game_object();

	_sendLotteryNotice(result.itemtype, result.itemcount);
	return true;
}

void StarLotteryMgr::_lottery(int lvl, stStarResult& result)
{
	static int StarCount = M_BaseInfoCFG::GetSingleton()->GetData("StarCount")->mValue;

	auto list = M_StarLotteryCFG::GetSingleton()->GetMapData();
	int randv = global_random::instance().rand_1w();
	int tempv = 999999;

	for (auto it = list.begin(); it != list.end(); ++it)
	{
		M_StarLotteryCFGData& rdata = it->second;
		if(rdata.mStarLvl == lvl)
		{
			if(CurCount->get_value() <= StarCount)
			{
				if(rdata.mRate1>0 &&rdata.mRate1 > randv && tempv >= rdata.mRate1)
				{
					tempv = rdata.mRate1;
					result.itemtype = rdata.mItemType;
					result.itemcount = rdata.mItemCount;
				}
			}
			else if(CurCount->get_value() <= StarCount*2)
			{
				if(rdata.mRate2>0 &&rdata.mRate2 > randv && tempv >= rdata.mRate2)
				{
					tempv = rdata.mRate2;
					result.itemtype = rdata.mItemType;
					result.itemcount = rdata.mItemCount;
				}
			}
			else if(CurCount->get_value() <= StarCount*3)
			{
				if(rdata.mRate3>0 &&rdata.mRate3 > randv && tempv >= rdata.mRate3)
				{
					tempv = rdata.mRate3;
					result.itemtype = rdata.mItemType;
					result.itemcount = rdata.mItemCount;
				}
			}			
			else
			{
				if(rdata.mRate4>0 &&rdata.mRate4 > randv && tempv >= rdata.mRate4)
				{
					tempv = rdata.mRate4;
					result.itemtype = rdata.mItemType;
					result.itemcount = rdata.mItemCount;
				}
			}						
		}
	}

	//给玩家奖励
	if(result.itemtype == msg_type_def::e_itd_chip || result.itemtype == 0)
	{//判断是否超过限制
		auto ssys = GLOBAL_SYS(StarLotterySys);
		if(ssys->get_surplus() >= result.itemcount)//全服
		{
			static int BaseChip = M_BaseInfoCFG::GetSingleton()->GetData("BaseChip")->mValue;
			static int ChipRate = M_BaseInfoCFG::GetSingleton()->GetData("ChipRate")->mValue;

			int rmb = get_game_player()->get_sys<game_sys_recharge>()->Recharged->get_value();
			if((BaseChip + rmb - TotalChip->get_value()/ChipRate) >= result.itemcount)//个人
			{
				get_game_player()->addItem(result.itemtype, result.itemcount, type_reason_star_lottery);
				TotalChip->add_value(result.itemcount);
				ssys->update_total(result.itemcount);
			}
			else
			{
				auto sl = M_StarLotteryCFG::GetSingleton()->GetData((lvl-1)*5+1);
				result.itemtype = sl->mItemType;
				result.itemcount = sl->mItemCount;
			}
		}
		else
		{
			auto sl = M_StarLotteryCFG::GetSingleton()->GetData((lvl-1)*5+1);
			result.itemtype = sl->mItemType;
			result.itemcount = sl->mItemCount;
		}
	}

	CurCount->add_value(1);
}

void StarLotteryMgr::_sendLotteryNotice(int itemType, int count)
{
	if(itemType == msg_type_def::e_itd_chip && count >= 10)
	{
		static const M_MultiLanguageCFGData *pData = M_MultiLanguageCFG::GetSingleton()->GetData("StarLotteryNotice");
		if(pData)
		{
			boost::format fmt = boost::format(pData->mName) % get_game_player()->NickName->get_string() % (count / 10);
			GLOBAL_SYS(ChatSys)->sysNotify(fmt.str(), msg_type_def::NotifyTypeWinningPrize);
		}
	}
}
