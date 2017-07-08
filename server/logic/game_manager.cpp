#include "stdafx.h"
#include "game_manager.h"
#include "logic_server.h"
#include "i_game_engine.h"
#include "proc_world2logic_protocol.h"
#include "backstage_manager.h"
#include <boost/algorithm/string.hpp>
#include <boost/lexical_cast.hpp>
#include "global_sys_mgr.h"
#include "count_sys.h"
#include "../games/game_common/game_common_def.h"
#include "game_player_mgr.h"

using namespace boost;

game_manager::game_manager()
	:m_mod(nullptr)
	,m_engine(nullptr)
	,m_pkmgr(nullptr)
	,m_gameid(0)
	,m_gamever(0)
{
}

game_manager::~game_manager()
{
	close();
}

bool game_manager::open()
{
	if(m_mod != nullptr)
	{
		game_regedit();
		return false;
	}

	std::string dllname = logic_server::instance().get_cfg().get<std::string>("game_dll");

	m_mod = LoadLibraryA(dllname.c_str());
	if (nullptr == m_mod)
	{
		return false;
	}
	return true;
}

void game_manager::close()
{
	m_engine = nullptr;
	m_pkmgr = nullptr;
	if(m_mod != nullptr)
	{
		FreeLibrary(m_mod);
		m_mod = nullptr;
	}
}

i_game_engine* game_manager::get_game_engine()
{
	if(m_mod == nullptr) return nullptr;

	if(m_engine == nullptr)
	{		
		FARPROC m_func = GetProcAddress (m_mod, "get_game_engine");
		if (nullptr == m_func)
		{
			return nullptr;
		}		

		m_engine = (i_game_engine*)m_func();
		m_engine->set_handler(this);
	}

	return m_engine;
}

packet_manager* game_manager::get_packet_mgr()
{
	if(m_mod == nullptr) return nullptr;

	if(m_pkmgr == nullptr)
	{		
		FARPROC m_func = GetProcAddress (m_mod, "get_packet_mgr");
		if (nullptr == m_func)
		{
			return nullptr;
		}		

		m_pkmgr = (packet_manager*)m_func();		
	}

	return m_pkmgr;
}


void game_manager::on_init_engine(uint16_t game_id, const std::string& game_ver)
{	
	m_gameid = game_id;	
	m_gamever = 0;
	std::vector<std::string> vec;
	split(vec, game_ver, is_any_of("."), token_compress_on);

	m_gamever += lexical_cast<int>(vec[0]) * 1000000;
	m_gamever += lexical_cast<int>(vec[1]) * 10000;
	m_gamever += lexical_cast<int>(vec[2]);
	
	m_robotmgr.set_gameid(m_gameid);
	game_regedit();
}

uint16_t game_manager::get_gameid()
{
	return m_gameid;
}
int game_manager::get_gamever()
{
	return m_gamever;
}

void game_manager::game_regedit()
{
	if(m_gameid<=0)
		return;

	auto peer = backstage_manager::instance().get_world();
	if(peer)
	{
		auto sendmsg = PACKET_CREATE(packetl2w_game_ready, e_mst_l2w_game_ready);
		sendmsg->set_game_id(m_gameid);
		sendmsg->set_game_ver(m_gamever);
		peer->send_msg(sendmsg);
	}
}


void game_manager::on_exit_engine()
{

}

//请求机器人 请求的机器人一定 要及时返回  
//要求的vip 要求的gold  自定义标志tag
void game_manager::request_robot(int tag, GOLD_TYPE needgold, int needvip)
{
	int playerid = m_robotmgr.request_robot(tag, needgold, needvip);
	if(playerid >0)//找到机器人及时返回
	{
		m_engine->response_robot(playerid, tag);
	}
}

//当不需要使用机器人时 只要退出到房间选择然后调用此函数
void game_manager::release_robot(int playerid)
{
	m_robotmgr.release_robot(playerid);
}

void game_manager::release_player(uint32_t pid)
{
	game_player_mgr::instance().del_player(pid);
}

void game_manager::heartbeat(double elapsed)
{
	m_robotmgr.heartbeat(elapsed);
}

int game_manager::get_robot_count()
{
	return m_robotmgr.get_count();
}

void game_manager::response_robot(int32_t playerid)
{
	m_robotmgr.inc_robot();
	m_engine->response_robot(playerid, m_robotmgr.pop_tag());
}


#include "servers_manager.h"
#include "game_player_mgr.h"
#include "game_player.h"
//广播协议
int game_manager::broadcast_msg_to_client(std::vector<uint32_t>& pids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg)
{
	std::list<uint32_t> sids;
	for (int i =0;i<pids.size();i++)
	{
		auto p = game_player_mgr::instance().find_playerbyid(pids[i]);
		if(p != nullptr && p->get_sessionid() > 0)
			sids.push_back(p->get_sessionid());
	}

	return servers_manager::instance().send_msg_to_client(sids, packet_id, msg);
}

#include "proc_logic_packet.h"
//发送协议列表
int game_manager::broadcast_msglist_to_client(std::vector<uint32_t>& pids, std::vector<msg_packet_one>& msglist)
{
	std::list<uint32_t> sids;
	for (int i =0;i<pids.size();i++)
	{
		auto p = game_player_mgr::instance().find_playerbyid(pids[i]);
		if(p != nullptr && p->get_sessionid() > 0)
			sids.push_back(p->get_sessionid());
	}

	auto sendmsg = PACKET_CREATE(packet_g2c_send_msglist, e_mst_g2c_send_msglist);

	sendmsg->mutable_msgpaks()->Reserve(msglist.size());
	for (int i =0;i<msglist.size();i++)
	{
		auto pk = sendmsg->add_msgpaks();
		pk->set_msgid(msglist[i].packet_id);
		msglist[i].msg_packet->SerializeToString(pk->mutable_msginfo());		
	}

	return servers_manager::instance().send_msg_to_client(sids, sendmsg->packet_id(), sendmsg);
}

bool game_manager::setGameDb(db_base *dgGame)
{
	return GLOBAL_SYS(CountSys)->setGameDb(dgGame);
}

int64_t game_manager::getCurId(const std::string& key)
{
	return GLOBAL_SYS(CountSys)->getCurId(key);
}
