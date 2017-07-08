#include "stdafx.h"
#include "proc_player_quest.h"
#include "game_quest_mgr.h"
#include "game_player.h"
#include "game_quest.h"
#include "global_sys_mgr.h"
#include "pump_sys.h"

using namespace boost;

void initQuestPacket()
{
	packetc2w_get_questlist_factory::regedit_factory();
	packetw2c_get_questlist_result_factory::regedit_factory();
	packetc2w_receive_questreward_factory::regedit_factory();
	packetw2c_receive_questreward_result_factory::regedit_factory();
	packetw2c_change_quest_factory::regedit_factory();
}

bool packetc2w_get_questlist_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<game_player> player, shared_ptr<packetc2w_get_questlist> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_get_questlist_result, e_mst_w2c_get_questlist_result);
	sendmsg->set_type(msg->type());

	GMapObjPtr tlist = player->get_sys<game_quest_mgr>()->get_map(msg->type());
	auto rlist = sendmsg->mutable_questlist();
	rlist->Reserve(tlist->get_obj_count());
	for (auto it = tlist->begin(); it != tlist->end(); ++it)
	{
		auto pi = CONVERT_POINT(game_quest, it->second);
		auto ti = rlist->Add();
		ti->set_questid(pi->QuestID->get_value());
		ti->set_count(pi->Count->get_value());
		ti->set_received(pi->Received->get_value());
	}

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


bool packetc2w_receive_questreward_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<game_player> player, shared_ptr<packetc2w_receive_questreward> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_receive_questreward_result, e_mst_w2c_questreward_result);
	sendmsg->set_result(msg_type_def::e_rmt_fail);
	sendmsg->set_questid(msg->questid());
	sendmsg->set_type(msg->type());

	auto squest = player->get_sys<game_quest_mgr>();
	if (squest->check_quest(msg->type(), msg->questid()))
	{		
		std::vector<stItem> vec;
		squest->receive_quest(msg->type(), msg->questid(), vec);
		if(!vec.empty())
		{
			auto rlist = sendmsg->mutable_items();
			rlist->Reserve(vec.size());
			for (int i =0;i<vec.size();i++)
			{
				auto ti = rlist->Add();
				ti->set_itemid(vec[i].m_itemId);
				ti->set_count(vec[i].m_count);
			}
		}	

		//// 每日任务
		//if(msg->type() == 1)
		//{
		//	GLOBAL_SYS(PumpSys)->finishDailyTaskLog(player.get(), msg->questid(), vec);
		//}
		//else // 成就
		//{
		//	GLOBAL_SYS(PumpSys)->finishTaskLog(player.get(), msg->questid(), vec);
		//}
		sendmsg->set_result(msg_type_def::e_rmt_success);
	}

	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}