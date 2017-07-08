#pragma once
#include <vector>
#include <string>
#include <enable_smart_ptr.h>

class game_player;
class world_peer;
class command_base
{
public:
	command_base(){};
	virtual ~command_base(){};

	virtual std::string get_cmdtype() = 0;
	virtual uint16_t get_level() = 0;
	virtual bool proc_cmd(boost::shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, const std::vector<std::string>& cmdvec) {return false;};

	virtual bool proc_cmd(boost::shared_ptr<game_player> player, const std::vector<std::string>& cmdvec) {return false;};
};