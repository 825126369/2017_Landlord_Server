#include "stdafx.h"
#include "proc_c2w_lobby_protocol.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "time_helper.h"
#include "command_mgr.h"
#include "payment_task.h"
#include "game_sys_recharge.h"
#include "game_engine_mgr.h"
#include "world_server.h"
#include "check_recharge_task.h"
#include "proc_logic2world_protocol.h"
#include "game_sys_recharge.h"
#include "gift_def.h"
#include "bag_mgr.h"
#include "bag_def.h"
#include "online_reward_mgr.h"
#include "safe_deposit_box_mgr.h"
#include "benefits_mgr.h"
#include "notice_sys.h"
#include "global_sys_mgr.h"
#include "daily_box_lottery_mgr.h"
#include "operation_activity_mgr.h"
#include "operation_activity_def.h"
#include "game_check.h"
#include "game_check_def.h"
#include "M_GameCFG.h"
#include "dial_lottery_mgr.h"
#include "th_pay_check.h"

using namespace boost;


void initLobbyProtocol()
{
	packetc2w_player_connect_factory::regedit_factory();
	packetw2c_player_connect_result_factory::regedit_factory();
	packetc2w_enter_game_factory::regedit_factory();
	packetw2c_enter_game_result_factory::regedit_factory();
	packetc2w_leave_game_factory::regedit_factory();
	packetw2c_leave_game_result_factory::regedit_factory();
	packet_c2w_timesync_factory::regedit_factory();
	packet_w2c_timesync_result_factory::regedit_factory();
	packetc2w_ask_login_factory::regedit_factory();
	packetw2c_ask_login_result_factory::regedit_factory();
	packetc2w_gm_command_factory::regedit_factory();
	packetw2c_gm_command_result_factory::regedit_factory();
	packetc2w_ask_check_payment_factory::regedit_factory();
	packetc2w_ask_test_payment_factory::regedit_factory();
	packetw2c_ask_check_payment_result_factory::regedit_factory();
	packetw2c_ask_check_payment_result2_factory::regedit_factory();

	packetw2c_player_kick_factory::regedit_factory();
	packetc2w_get_gamelist_factory::regedit_factory();
	packetw2c_get_gamelist_result_factory::regedit_factory();
	packetc2w_first_login_factory::regedit_factory();
	packetw2c_first_login_result_factory::regedit_factory();
}

bool packetc2w_player_connect_factory::packet_process(shared_ptr<world_peer> peer, uint32_t sessionid, shared_ptr<packetc2w_player_connect> msg)
{	
	__ENTER_FUNCTION_CHECK;
	SLOG_CRITICAL << "玩家连接:"<<std::endl;

	if(game_player::check_token(msg->account(), msg->token(), msg->sign()))
	{
		auto p = game_player_mgr::instance().find_player(msg->account());
		if(p != nullptr)
		{	
			if(p->get_state() != e_ps_checking)
			{
				if(p->get_sessionid() != sessionid)
				{
					auto msg2 = PACKET_CREATE(packetw2c_player_kick, e_mst_w2c_player_kick);
					p->send_msg_to_client(msg2);

					//sessionid 可能与原来的不同  情况为踢人				
					game_player_mgr::instance().reset_player(p, sessionid);
					p->player_relogin(msg->token());	
					p->MachineCode = msg->machine_code();
					p->MachineType = msg->machine_type();

					return true;
				}
				//不应该过来这里 表示玩家之前没有退出或者断开连接
			}
			else//重复请求抛弃
				return true;
		}
		else
		{
			p = game_player::malloc();
			p->player_login(sessionid, msg->account(), msg->token(), msg->platform(), msg->login_platform());
			game_player_mgr::instance().add_player(p);
			p->MachineCode = msg->machine_code();
			p->MachineType = msg->machine_type();
			p->m_channel = msg->channelid();
			return true;
		}
	}

	SLOG_CRITICAL << "世界服务器连接失败" << std::endl;

	//验证失败
	auto sendmsg =  PACKET_CREATE(packetw2c_player_connect_result, e_mst_w2c_player_connect_result);
	sendmsg->set_result(msg_type_def::e_rmt_fail);
	peer->send_msg_to_client(sessionid, sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求进入游戏
bool packetc2w_enter_game_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_enter_game> msg)
{	
	__ENTER_FUNCTION_CHECK;

	SLOG_CRITICAL << "请求进入游戏" << std::endl;
	auto sendmsg = PACKET_CREATE(packetw2c_enter_game_result, e_mst_w2c_enter_game_result);
	game_info gi;
	if(game_engine_mgr::instance().get_game_info(msg->gameid(), gi))
	{
		auto ginfo = M_GameCFG::GetSingleton()->GetData(msg->gameid());
		if(ginfo)
		{
			if(gi.GameVer == msg->gamever())
			{
				SLOG_CRITICAL << "当前游戏版本：" << gi.GameVer;
				if(player->join_game(gi.GameID, gi.GameServerID))
					return true;//要从logic返回成功才返回信息给客户端
			}		
		}
	}
	else
	{
		sendmsg->set_result(msg_type_def::e_rmt_player_max);
	}

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求离开游戏
bool packetc2w_leave_game_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_leave_game> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto peer2 = player->get_logic();
	if(peer2)
	{
		auto msg =  PACKET_CREATE(packetw2l_player_logout, e_mst_w2l_player_logout);		
		msg->set_playerid(player->PlayerId->get_value());
		peer2->send_msg(msg);
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求时间同步
bool packet_c2w_timesync_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packet_c2w_timesync> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packet_w2c_timesync_result, e_mst_w2c_timesync_result);
	sendmsg->set_server_time(time_helper::instance().get_cur_time());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求登陆数据
bool packetc2w_ask_login_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_ask_login> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_ask_login_result, e_mst_w2c_asklogin_result);
	auto ainfo = sendmsg->mutable_account_info();
	ainfo->set_aid(player->PlayerId->get_value());
	//ainfo->set_account(player->Account->get_string());
	ainfo->set_nickname(player->NickName->get_string());
	ainfo->set_gold(player->Gold->get_value());
	auto vipsys = player->get_sys<game_sys_recharge>();
	ainfo->set_viplvl(vipsys->VipLevel->get_value());
	ainfo->set_vipexp(vipsys->VipExp->get_value());
	ainfo->set_monthcardremainsecondtime(vipsys->getMonthCardRemainSecondTime());
	ainfo->set_monthcard_time(vipsys->VipCardEndTime->get_value());
	ainfo->set_hasreceiverechargereward(vipsys->RechargeRewardFlag->get_value());
	ainfo->set_currecharge(vipsys->Recharged->get_value());
	ainfo->set_experience_vip(player->ExperienceVIP->get_value());
	ainfo->set_create_time(player->CreateTime->get_value());
	ainfo->set_firstgifttime(player->FirstGiftTime->get_value());
	//ainfo->set_playertype(player->PlayerType->get_value());//xk

	// 支付信息 
	auto payList = vipsys->PaymentCheck->get_array();
	for (auto it = payList->begin(); it != payList->end(); ++it)
	{
		ainfo->add_payids((*it)->get_id());
	}

	int islogin = player->CheckMap->get_Tmap<check_map>()->get_check(check_first_login);
	ainfo->set_isfirstlogined((islogin > 0));
	ainfo->set_chip(player->Chip->get_value());

	//ainfo->set_icon_id(player->IconId->get_value());
	//if(player->IconId->get_value()<0)
	ainfo->set_icon_custom(player->IconCustom->get_string());
	ainfo->set_collected(player->get_sys<BenefitsMgr>()->collected());
	ainfo->set_sex(player->Sex->get_value());
	ainfo->set_selfsignature(player->SelfSignature->get_string());
	ainfo->set_ticket(player->Ticket->get_value());
	ainfo->set_newguildhasfinishstep(player->NewGuildHasFinishStep->get_value());
	ainfo->set_sendgiftcoincount(player->SendGiftCoinCount->get_value());

	// 礼物统计相关
	for(auto git = player->m_giftStatPtr->begin(); git != player->m_giftStatPtr->end(); ++git)
	{
		auto ptr = CONVERT_POINT(GiftInfo, git->second);
		auto pInfo = ainfo->add_giftstat();
		pInfo->set_giftid(ptr->m_giftId->get_value());
		pInfo->set_count(ptr->m_count->get_value());
	}

	// 背包数据
	auto pBagItem = ainfo->mutable_bagitems();
	auto pBagMap = player->get_sys<BagMgr>()->getBagMap();
	pBagItem->Reserve(pBagMap->get_obj_count());

	for(auto it = pBagMap->begin(); it != pBagMap->end(); ++it)
	{
		auto pItem = pBagItem->Add();
		auto tmp = CONVERT_POINT(GameItem, it->second);
		pItem->set_itemid(tmp->m_itemId->get_value());
		pItem->set_count(tmp->m_itemCount->get_value());
	}
	ainfo->set_curphotoframeid(player->PhotoFrameId->get_value());

	ainfo->set_hasreceiveonlinerewardcount(player->OnlineRewardCount->get_value());
	ainfo->set_remainonlinerewardtime(player->get_sys<OnlineRewardMgr>()->getRemainTime());

	auto ptrSafeBox = player->get_sys<SafeDepositBoxMgr>();
	ainfo->set_issafedepositboxpwdempty(ptrSafeBox->isPasswordEmpty());
	ainfo->set_safeboxgold(ptrSafeBox->m_gold->get_value());

	auto gamelist = game_engine_mgr::instance().get_gamelist();
	sendmsg->mutable_game_list()->Reserve(gamelist.size());
	for (auto it = gamelist.begin(); it != gamelist.end(); ++it)
	{
		auto gi = sendmsg->add_game_list();
		gi->set_gameid(it->second.GameID);
		gi->set_gamever(it->second.GameVer);

		int n = game_player_mgr::instance().getOnlineNumInGame(it->second.GameID);
		gi->set_curonlinenum(n);
	}

	if(player->is_gaming())
	{
		//player->on_joingame();
		sendmsg->set_gaming(player->get_gameid());
	}

	ainfo->set_update_icon_count(player->UpdateIconCount->get_value());
	ainfo->set_updatenicknamecount(player->UpdateNickCount->get_value());
	ainfo->set_isbindmobilephone(!player->BindPhone->get_string().empty());

	std::vector<stServiceInfo>* sInfoList = nullptr;
	bool res = GLOBAL_SYS(NoticeSys)->getServiceInfo(player->Platform->get_string(), sInfoList);
	if(res)
	{
		auto pSInfo = ainfo->mutable_serviceinfos();
		for(auto it = sInfoList->begin(); it != sInfoList->end(); ++it)
		{
			auto item = pSInfo->Add();
			item->set_infotype(it->m_infoType);
			item->set_key(it->m_key);
			item->set_value(it->m_value);
		}
	}

	// 每日抽奖信息
	auto boxLotteryMgr = player->get_sys<DailyBoxLotteryMgr>();
	ainfo->set_boxlotterycounttoday(boxLotteryMgr->getLotteryCountToday());
	ainfo->set_thankyoujoincount(boxLotteryMgr->getThankYouCount());
	auto pBoxInfo = ainfo->mutable_boxinfolist();
	int boxCount = boxLotteryMgr->getBoxCount();
	pBoxInfo->Reserve(boxCount);
	for(int i = 0; i < boxCount; i++)
	{
		auto ptr = boxLotteryMgr->getBoxLotteryItem(i);
		if(ptr)
		{
			auto th = pBoxInfo->Add();
			th->set_isopen(ptr->m_isOpen->get_value());
			th->set_containgold(ptr->m_containGold->get_value());
		}
	}

	// 活动领取的相关信息
	auto activityMgr = player->get_sys<OperationActivityMgr>();
	auto pReceiveInfo = ainfo->mutable_receiveinfo();
	pReceiveInfo->Reserve(7);
	for(int i = 19; i <= 25; i++)
	{
		auto item = pReceiveInfo->Add();
		item->set_activityid(i);
		auto ptr = activityMgr->findActivity(i);
		if(ptr)
		{
			item->set_isreceive(ptr->m_isFinish->get_value());
		}
		else
		{
			item->set_isreceive(false);
		}
	}

	// 月卡领取信息
	auto dialMgr = player->get_sys<DialLotteryMgr>();
	bool isReceive = dialMgr->hasReceiveMonthCardReward(time_helper::instance().get_cur_date());
	ainfo->set_hasreceivemonthcardreward(isReceive);

	//特权
	ainfo->set_privilege(player->Privilege->get_value());

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);


	//登陆大厅则检测一次充值数据

	if(world_server::instance().get_cfg().check("use_th"))
	{
		auto seach = boost::make_shared<th_pay_check>(world_server::instance().get_io_service());
		seach->init_task(player);	
	}
	else
	{
		auto seach = boost::make_shared<check_recharge_task>(world_server::instance().get_io_service());
		seach->init_task(player);	
	}
	

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求gm命令
bool packetc2w_gm_command_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_gm_command> msg)
{	
	__ENTER_FUNCTION_CHECK;

	bool ret = command_mgr::instance().parse_cmd(peer, player, msg->command());

	auto sendmsg = PACKET_CREATE(packetw2c_gm_command_result, e_mst_w2c_command_result);

	if(ret)
		sendmsg->set_result(msg_type_def::e_rmt_success);
	else
		sendmsg->set_result(msg_type_def::e_rmt_fail);

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);


	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求检验支付
bool packetc2w_ask_check_payment_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_ask_check_payment> msg)
{	
	__ENTER_FUNCTION_CHECK;
	SLOG_CRITICAL << "请求检验支付" << std::endl;
	if(!msg->orderid().empty())
	{
		auto task = boost::make_shared<payment_task>(world_server::instance().get_io_service());
		task->init_task( msg->orderid(), player, msg->ex_mark());
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//请求支付测试
bool packetc2w_ask_test_payment_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_ask_test_payment> msg)
{	
	__ENTER_FUNCTION_CHECK;
	SLOG_CRITICAL << "请求支付测试000000000000000" << std::endl;
#ifdef _DEBUG
	player->get_sys<game_sys_recharge>()->payment_once(msg->payid(), 0, false, msg->ex_mark());
#endif
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


bool packetc2w_get_gamelist_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_get_gamelist> msg)
{	
	__ENTER_FUNCTION_CHECK;
	SLOG_CRITICAL << "请求所有游戏列表" << std::endl;
	auto sendmsg = PACKET_CREATE(packetw2c_get_gamelist_result, e_mst_w2c_get_gamelist_result);

	auto gamelist = game_engine_mgr::instance().get_gamelist();
	sendmsg->mutable_game_list()->Reserve(gamelist.size());
	for (auto it = gamelist.begin(); it != gamelist.end(); ++it)
	{
		auto gi = sendmsg->add_game_list();
		gi->set_gameid(it->second.GameID);
		gi->set_gamever(it->second.GameVer);

		int n = game_player_mgr::instance().getOnlineNumInGame(it->second.GameID);
		gi->set_curonlinenum(n);
	}

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


//第一次登陆
bool packetc2w_first_login_factory::packet_process(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, shared_ptr<packetc2w_first_login> msg)
{	
	__ENTER_FUNCTION_CHECK;
	SLOG_CRITICAL << "第一次登陆" << std::endl;
	auto sendmsg = PACKET_CREATE(packetw2c_first_login_result, e_mst_w2c_first_login_result);

	if(player->first_login())
		sendmsg->set_result(msg_type_def::e_rmt_success);

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}