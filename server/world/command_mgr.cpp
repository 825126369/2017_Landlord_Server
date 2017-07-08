#include "stdafx.h"
#include "command_mgr.h"
#include "commands.h"
#include "game_player_mgr.h"
#include <boost/algorithm/string.hpp>
#include "game_player.h"
#include "world_server.h"
#include "url_param.h"
#include "server_msg_type.pb.h"
#include "global_sys_mgr.h"
#include "order_sys.h"
#include "payment_task.h"

using namespace boost;

command_mgr::command_mgr()
{
	//注册命令

	regedit_cmd(make_shared<cmd_recharge>());
}

command_mgr::~command_mgr()
{
}

bool command_mgr::parse_cmd(shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, const std::string& cmdstr)
{
	__ENTER_FUNCTION_CHECK;
	
	std::vector<std::string> vec;
	std::string endstr = cmdstr;
	boost::trim(endstr);
	split(vec, endstr, algorithm::is_space(), token_compress_on);
		
	if(vec.size()<1)
		return false;

	auto it = m_cmdmap.find(vec[0]);
	if(it == m_cmdmap.end())
		return false;

	static bool isdebug = world_server::instance().get_cfg().get<bool>("debug");
	if(!isdebug/* && player->GMlvl->get_value() < it->second->get_level()*/)
		return false;

	return it->second->proc_cmd(peer, player, vec);	

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

void command_mgr::regedit_cmd(shared_ptr<command_base> cmd)
{	
	m_cmdmap.insert(std::make_pair(cmd->get_cmdtype(), cmd));
}



bool command_mgr::parse_cmd(const std::string& cmdstr)
{
	__ENTER_FUNCTION_CHECK;
	
	UrlParam url;
	url.parse(cmdstr);
	int cmd = url.getIntValue("/cmd", -1);
	switch(cmd)
	{
	case server_protocols::e_cmd_order: // 当前的命令是实时订单处理
		{
			GLOBAL_SYS(OrderSys)->addOrder(url);
		}
		break;
	case server_protocols::e_cmd_recharge: // 当前的命令是实时订单处理
		{
			auto player = game_player_mgr::instance().find_player(url.getStringValue("account"));
			if(!player) // 玩家不在线	
				return true;	

			auto task = boost::make_shared<payment_task>(world_server::instance().get_io_service());
			task->init_task( url.getStringValue("orderid"), player);
		}
		break;
	case server_protocols::e_cmd_addlucky: // 修改幸运
		{
			auto player = game_player_mgr::instance().findPlayerById(url.getIntValue("playerid"));
			if(!player) // 玩家不在线	
				return true;	

			player->change_lucky(url.getIntValue("lucky"));
		}
		break;
	}
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
