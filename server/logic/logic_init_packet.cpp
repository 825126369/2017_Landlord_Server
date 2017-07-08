#include "stdafx.h"
#include "logic_server.h"
#include "proc_server_packet.h"
#include "proc_logic_packet.h"
#include "proc_world2logic_protocol.h"
#include "proc_world2logic_friend.h"

void logic_server::init_packet()
{
	__ENTER_FUNCTION;

	init_logic_protocol();

	init_server_protocol();

	init_world2logic_protocol();

	initWorld2LogicFriend();

	__LEAVE_FUNCTION
}