#include "stdafx.h"
#include "proc_logic2world_robot.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "robots_sys.h"

using namespace boost;

void initLogic2WorldRobot()
{
	packetl2w_request_robot_factory::regedit_factory();
}

// 请求机器人
bool packetl2w_request_robot_factory::packet_process(shared_ptr<world_peer> peer, shared_ptr<packetl2w_request_robot> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto robot = GLOBAL_SYS(RobotsSys)->request_robot(msg->needgold(), msg->needvip());

	//获取到机器人直接进入游戏
	if(robot != nullptr)
	{
		if(!robot->join_game(msg->gameid(), peer->get_remote_id()))
		{
			robot->resetGameIdServerId();
			robot->player_logout();
		}
	}
	else
	{
		SLOG_ERROR << "request_robot is null,  gameid:"<<msg->gameid();
	}


	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
