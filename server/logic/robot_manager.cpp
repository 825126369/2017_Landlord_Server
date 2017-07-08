#include "stdafx.h"
#include "robot_manager.h"
#include <net/packet_manager.h>
#include <logic2world_robot.pb.h>
#include "game_player_mgr.h"
#include "game_player.h"
#include "backstage_manager.h"
#include "server_peer.h"

using namespace boost;
using namespace logic2world_protocols;

robot_manager::robot_manager()
	:m_count(0)
{

}

robot_manager::~robot_manager()
{

}


//请求机器人  返回-1为需要向world请求新机器人
int robot_manager::request_robot(int tag, GOLD_TYPE needgold, int needvip)
{
	if(game_player_mgr::instance().is_closing())
		return -2;

	//直接向world 请求机器人
	auto peer = backstage_manager::instance().get_world();
	if(peer != nullptr)
	{
		auto sendmsg = PACKET_CREATE(packetl2w_request_robot, e_mst_l2w_robot_request);
		sendmsg->set_needvip(needvip);
		sendmsg->set_needgold(needgold);
		sendmsg->set_gameid(m_gameid);
		peer->send_msg(sendmsg);
		m_reqlist.push(tag);
	}
	return -1;
}

//释放机器人
void robot_manager::release_robot(int playerid)
{
	/*auto fit = std::find(m_dellist.begin(), m_dellist.end(), playerid);
	if(fit != m_dellist.end())
		return;

	m_dellist.push_back(playerid);*/

	leave_robot(playerid);
}

void robot_manager::heartbeat(double elapsed)
{
	/*if(m_dellist.empty())
		return;

	for (int i = 0;i<m_dellist.size(); i++)
	{
		leave_robot(m_dellist[i]);
	}
	m_dellist.clear();*/
}

void robot_manager::leave_robot(int playerid)
{
	auto p = game_player_mgr::instance().find_playerbyid(playerid);
	if(p!=nullptr)
	{
		game_player_mgr::instance().del_player(playerid);
		m_count--;
	}
}

int robot_manager::pop_tag()
{
	int tag = -1;
	m_reqlist.pop(tag);
	return tag;
}

void robot_manager::set_gameid(int gameid)
{
	m_gameid = gameid;
}

int robot_manager::get_count()
{
	return m_count;
}

void robot_manager::inc_robot()
{
	m_count++;
}