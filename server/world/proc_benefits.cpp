#include <stdafx.h>
#include <proc_benefits.h>
#include <M_BaseInfoCFG.h>

#include "game_player.h"
#include "safe_deposit_box_mgr.h"
#include "benefits_mgr.h"
#include "pump_type.pb.h"
#include "pump_type.h"
#include "pump_sys.h"
#include "global_sys_mgr.h"

void initBenefitsPacket()
{
	packet_c2w_benefits_factory::regedit_factory();
	packet_w2c_benefits_result_factory::regedit_factory();
}

using namespace boost;
bool packet_c2w_benefits_factory::packet_process(shared_ptr<world_peer> peer,
											boost::shared_ptr<game_player> player, shared_ptr<packet_c2w_benefits> msg)
{	
	__ENTER_FUNCTION_CHECK

	auto sendmsg = PACKET_CREATE(packet_w2c_benefits_result, e_mst_w2c_benefits_result);
	
	sendmsg->set_result(msg_type_def::e_rmt_fail);

	auto base_info_object=M_BaseInfoCFG::GetSingleton();

	//用户金币小于多少才能领取
	static const int benefits_limit = base_info_object->GetData("almsLimit")->mValue;
	//每次领取多少金币
	static const int benefits_value = base_info_object->GetData("almsGoldCount")->mValue;
	//最多能领取多少次
	static const int collect_limit = base_info_object->GetData("almsMaxCount")->mValue;

	auto sys=player->get_sys<BenefitsMgr>();

	/*
		判断用户是否满足领取身份,满足则设置好领取次数和金钱后给客户端
	*/
	GOLD_TYPE total_gold=
		player->Gold->get_value()
		+
		player->get_sys<SafeDepositBoxMgr>()->m_gold->get_value();

	int collected=sys->collected();

	if(collected < collect_limit && total_gold < benefits_limit)
	{
		sys->m_collected->add_value(1);
		auto state = player->addItem(msg_type_def::e_itd_gold,benefits_value, type_reason_receive_alms);

		if(state == msg_type_def::e_rmt_success)
		{
			//player->store_game_object();
			sendmsg->set_result(msg_type_def::e_rmt_success);

			GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_relief);
		}
	}
	else
	{
		sendmsg->set_result(msg_type_def::e_rmt_cannot_collect);
	}


	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}