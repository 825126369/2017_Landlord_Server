#include "stdafx.h"
#include "commands.h"
#include "game_player.h"
#include <boost/lexical_cast.hpp>


using namespace boost;
//////////////////////////////////////////////////////////////////////////
//cmd_recharge
#include <task_manager.h>
bool cmd_recharge::proc_cmd(shared_ptr<world_peer> peer, shared_ptr<game_player> player, const std::vector<std::string>& cmdvec)
{
	__ENTER_FUNCTION_CHECK;

	if(cmdvec.size() <2)
		return false;

	int payid = lexical_cast<int>(cmdvec[1]);	

	//player->payment_once(payid);

	return true;

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}