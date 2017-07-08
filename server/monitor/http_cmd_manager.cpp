#include "stdafx.h"
#include "http_cmd_manager.h"
#include "proc_server_packet.h"
#include "servers_manager.h"

bool HttpCmdManager::addCommand(const std::string& cmd)
{
	if(m_cmdList.read_available() > max_count)
		return false;

	m_cmdList.push(cmd);
	return true;
}

void HttpCmdManager::heartbeat(double elapsed)
{
	if(m_cmdList.empty())
		return;

	auto sendmsg = PACKET_CREATE(packet_http_command, e_mst_http_command);

	std::string cmd;
	int i = 0;
	for(; i < process_count_each_frame && !m_cmdList.empty(); i++)
	{
		m_cmdList.pop(cmd);
		sendmsg->add_cmdstr(cmd);
	}

	auto world = servers_manager::instance().get_server_bytype(e_st_world);
	if (world)
	{
		world->send_msg(sendmsg);	
	}
}






















