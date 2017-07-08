#include "stdafx.h"
#include "proc_activity.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "time_helper.h"
#include "operation_activity_sys.h"
#include "M_ActivityCFG.h"

using namespace boost;

void initActivityPacket()
{
	packetc2w_receive_activity_reward_factory::regedit_factory();
	packetw2c_receive_activity_reward_result_factory::regedit_factory();
}

// 领取活动奖励
bool packetc2w_receive_activity_reward_factory::packet_process(shared_ptr<world_peer> peer, 
															   boost::shared_ptr<game_player> player, 
															   shared_ptr<packetc2w_receive_activity_reward> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	auto sendmsg = PACKET_CREATE(packetw2c_receive_activity_reward_result, e_mst_w2c_receive_activity_reward_result);
	auto sys = GLOBAL_SYS(OperationActivitySys);
	int result = sys->receiveActivityRewardManual(player.get(), msg->activityid());
	sendmsg->set_result(result);
	sendmsg->set_activityid(msg->activityid());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
