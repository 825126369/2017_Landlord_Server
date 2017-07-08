#pragma once
#include "command_base.h"


//////////////////////////////////////////////////////////////////////////
class cmd_recharge:
	public command_base
{
public:
	cmd_recharge(){};
	virtual ~cmd_recharge(){};
	virtual uint16_t get_level()
	{
		return 1;
	}
	virtual std::string get_cmdtype()
	{
		return "recharge";
	};
	virtual bool proc_cmd(boost::shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, const std::vector<std::string>& cmdvec);
};