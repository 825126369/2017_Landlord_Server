#include "stdafx.h"
#include "online_reward_mgr.h"
#include "game_player.h"
#include "msg_type_def.pb.h"
#include "time_helper.h"
#include "M_BaseInfoCFG.h"
#include "game_sys_recharge.h"
#include "pump_type.pb.h"
#include "daily_box_lottery_sys.h"
#include "global_sys_mgr.h"
#include "M_OnlineRewardCFG.h"

void OnlineRewardMgr::init_sys_object()
{
	m_lastReceiveTime = time_helper::instance().get_cur_time();

	m_onlineReward = get_game_player()->regedit_arrfield("onlineRewardArr", OnlineRewardArray::malloc());
	m_onlineRewardPtr = m_onlineReward->get_Tarray<OnlineRewardArray>();

	m_rewardId = 0;
}

void OnlineRewardMgr::sys_time_update()
{
	int count = m_onlineRewardPtr->get_obj_count();
	for(int i = 0; i < count; i++)
	{
		auto ptr = m_onlineRewardPtr->get_Tobj<OnlineRewardItem>(i);
		if(ptr)
		{
			ptr->m_isReceive->set_value(false);
		}
	}
	m_onlineReward->set_update();
}

int OnlineRewardMgr::receiveReward()
{
	boost::posix_time::ptime curPtime = time_helper::instance().get_cur_ptime();
	return receiveReward(curPtime);

	static GOLD_TYPE rewardGold = M_BaseInfoCFG::GetSingleton()->GetData("onlineRewardGold")->mValue;

	auto ptrPlayer = get_game_player();
	int maxReceiveCount = ptrPlayer->get_sys<game_sys_recharge>()->getVipProfit(e_evt_OnlineReward);
	if(ptrPlayer->OnlineRewardCount->get_value() >= maxReceiveCount)
		return msg_type_def::e_rmt_runout_count;

	time_t curTime = time_helper::instance().get_cur_time();
	int r = _getRemainTime(curTime);
	if(r > 0)
		return msg_type_def::e_rmt_time_not_arrive;

	m_lastReceiveTime = curTime;
	ptrPlayer->addItem(msg_type_def::e_itd_gold, rewardGold, type_reason_online_reward);
	ptrPlayer->OnlineRewardCount->add_value(1);
	//ptrPlayer->store_game_object();
	return msg_type_def::e_rmt_success;
}

int OnlineRewardMgr::receiveReward(boost::posix_time::ptime& curPtime)
{
	const OnlineRewardInfo *pInfo = GLOBAL_SYS(DailyBoxLotterySys)->getOnlineRewardData(curPtime);
	if(pInfo == nullptr)
		return msg_type_def::e_rmt_time_not_arrive;  // 时间没有到，不能领取

	const M_OnlineRewardCFGData *pData = pInfo->m_data;

	auto ptr = m_onlineRewardPtr->find_Tobj<OnlineRewardItem>(pData->mID);
	if(ptr == nullptr)
	{
		ptr = OnlineRewardItem::malloc();
		ptr->m_id->set_value(pData->mID);
		m_onlineRewardPtr->put_obj(ptr);
	}

	if(ptr == nullptr)
		return msg_type_def::e_rmt_unknow;

	if(ptr->m_isReceive->get_value())
		return msg_type_def::e_rmt_has_receive_reward;

	auto ptrPlayer = get_game_player();

	if(ptrPlayer->get_viplvl() < pData->mConditionVip)
		return msg_type_def::e_rmt_vip_under;

	ptr->m_isReceive->set_value(true);
	m_onlineReward->set_update();
	
	boost::format fmt = boost::format("start:%1%:%2%,end:%3%:%4%")
		% pInfo->m_start.hours() % pInfo->m_start.minutes()
		% pInfo->m_end.hours() % pInfo->m_end.minutes();

	ptrPlayer->addItem(msg_type_def::e_itd_gold, pData->mRewardCoin, type_reason_online_reward, fmt.str());
	m_rewardId = pInfo->m_data->mID;
	//ptrPlayer->store_game_object();
	return msg_type_def::e_rmt_success;
}

int OnlineRewardMgr::getRemainTime()
{
	time_t curTime = time_helper::instance().get_cur_time();
	return _getRemainTime(curTime);
}

int OnlineRewardMgr::getRecvCount()
{
	return m_onlineRewardPtr->get_obj_count();
}

OnlineRewardItem* OnlineRewardMgr::getOnlineRewardItem(int index)
{
	auto ptr = m_onlineRewardPtr->get_Tobj<OnlineRewardItem>(index);
	return ptr.get();
}

int OnlineRewardMgr::_getRemainTime(time_t curTime)
{
	static int timeInterval = M_BaseInfoCFG::GetSingleton()->GetData("onlineRewardTimeInterval")->mValue * 60;
	int delta = curTime - m_lastReceiveTime;

	if(delta >= timeInterval)
		return 0;

	return timeInterval - delta;
}

//////////////////////////////////////////////////////////////////////////

OnlineRewardItem::OnlineRewardItem()
{
	init_game_object();
}

void OnlineRewardItem::init_game_object()
{
	m_id =  CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "id"));
	m_isReceive =  CONVERT_POINT(Tfield<bool>, regedit_tfield(e_got_bool, "isReceive"));
}

//////////////////////////////////////////////////////////////////////////

const std::string& OnlineRewardArray::get_cells_name()
{
	static std::string cellsname = "onlineRewardArr";
	return cellsname;
}

const std::string& OnlineRewardArray::get_id_name()
{
	static std::string idname = "id";
	return idname;
}

GObjPtr OnlineRewardArray::create_game_object(uint32_t object_id)
{
	auto op = OnlineRewardItem::malloc();
	op->m_id->set_value(object_id);
	return op;
}

