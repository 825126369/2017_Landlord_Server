#pragma once
#include <enable_hashmap.h>
#include <string>
#include <enable_smart_ptr.h>
#include <enable_singleton.h>


class command_base;
class world_peer;
class game_player;
class command_mgr:
	public enable_singleton<command_mgr>
{
public:
	command_mgr();
	~command_mgr();

	bool parse_cmd(boost::shared_ptr<world_peer> peer, boost::shared_ptr<game_player> player, const std::string& cmdstr);

	bool parse_cmd(const std::string& cmdstr);

private:
	void regedit_cmd(boost::shared_ptr<command_base> cmd);

	ENABLE_MAP<std::string, boost::shared_ptr<command_base>> m_cmdmap;
};
