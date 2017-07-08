#include "stdafx.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "game_db.h"
#include "enable_random.h"
#include "global_sys_mgr.h"
#include "robots_sys.h"
#include "time_helper.h"
#include "proc_c2w_lobby_protocol.h"

game_player_mgr::game_player_mgr()
	:m_checktime(0)
	,b_closing(false)
{
	log_playercount();
}

game_player_mgr::~game_player_mgr()
{
	//这里不能调用  退出时堆栈错乱
	//log_playercount();
}


boost::shared_ptr<game_player> game_player_mgr::find_player(const std::string& acc)
{
	auto it = m_playersbyacc.find(acc);
	if(it != m_playersbyacc.end())
		return it->second;

	return nullptr;
}

boost::shared_ptr<game_player> game_player_mgr::find_player(uint32_t sessionid)
{
	auto it = m_playersbysid.find(sessionid);
	if(it != m_playersbysid.end())
		return it->second;

	return nullptr;
}

boost::shared_ptr<game_player> game_player_mgr::findPlayerById(int playerId)
{
	auto it = m_playerById.find(playerId);
	if(it != m_playerById.end())
		return it->second;

	return nullptr;
}

bool game_player_mgr::add_player(boost::shared_ptr<game_player> p)
{
	auto ret =m_playersbysid.insert(std::make_pair(p->get_sessionid(), p));
	if(!ret.second)
		return false;
	m_playersbyacc.insert(std::make_pair(p->Account->get_string(), p));	
	return true;
}

bool game_player_mgr::addPlayerById(boost::shared_ptr<game_player> p)
{
	auto ret = m_playerById.find(p->PlayerId->get_value());
	if(ret == m_playerById.end())
	{
		m_playerById.insert(std::make_pair(p->PlayerId->get_value(), p));
	}
	return true;
}

void game_player_mgr::remove_player(boost::shared_ptr<game_player> p)
{
	m_playersbysid.erase(p->get_sessionid());
	m_playersbyacc.erase(p->Account->get_string());
	m_playerById.erase(p->PlayerId->get_value());
}

void game_player_mgr::reset_player(boost::shared_ptr<game_player> p, uint32_t sessionid)
{
	//m_playersbysid.erase(p->get_sessionid());
	p->set_sessionid(sessionid);
	m_playersbysid.insert(std::make_pair(p->get_sessionid(), p));
}

void game_player_mgr::remove_session(uint32_t sessionid)
{
	m_playersbysid.erase(sessionid);
}

void game_player_mgr::heartbeat(double elapsed)
{
	for (auto it = m_playersbyacc.begin(); it != m_playersbyacc.end(); ++it)
	{
		it->second->heartbeat(elapsed);
	}

	static int maxplayer = 0;
	static int curplayer = 0;	

	m_checktime+=elapsed;
	if(m_checktime > 300)
	{
		if(curplayer != m_playersbyacc.size())
		{
			curplayer = m_playersbyacc.size();
			if(maxplayer < curplayer)
				maxplayer = curplayer;	

			SLOG_CRITICAL << "cur players count:" << curplayer << " max:"<<maxplayer <<" robots:"<< GLOBAL_SYS(RobotsSys)->get_count();
			log_playercount(curplayer);
		}
		
		m_checktime = 0;
	}

	static double check_kick = 0, recheck_kick = 0;
	check_kick += elapsed;
	recheck_kick += elapsed;
	if(recheck_kick>3)
	{
		recheck_kick = 0;
		
		if(!m_kicklist.empty())
		{
			for (auto it = m_kicklist.begin(); it != m_kicklist.end(); ++it)
			{
				auto player = findPlayerById(*it);
				if(player)
				{
					player->player_logout();
					remove_session(player->get_sessionid());
				}			
			}
			m_kicklist.clear();		
		}	

		//移除
		if(!m_dellist.empty())
		{
			for (auto it = m_dellist.begin(); it != m_dellist.end(); ++it)
			{			
				remove_player(*it);
				//it->second->on_logout();
			}
			m_dellist.clear();
		}
	}

	if(check_kick > 30)
	{
		check_kickplayer();
		check_kick = 0;
	}

	server_closing(elapsed);
}

void game_player_mgr::set_del_player(boost::shared_ptr<game_player> p)
{
	//m_dellist.insert(std::make_pair(p->PlayerId->get_value(), p));
	m_dellist.push_back(p);
}

uint32_t game_player_mgr::generic_playerid()
{
	//每次都从服务器获取 保证ID完全对应
	mongo::BSONObj b = db_player::instance().findone(DB_COMMON_CONFIG, BSON("type"<<"cur_playerid"));
	if(b.isEmpty())
	{
		m_cur_playerid = 1000001;
		db_player::instance().update(DB_COMMON_CONFIG,BSON("type"<<"cur_playerid"), BSON("type"<<"cur_playerid"<<"value" << m_cur_playerid));
		return m_cur_playerid;
	}
	else
		m_cur_playerid = b.getIntField("value");	

	db_player::instance().update(DB_COMMON_CONFIG, BSON("type"<<"cur_playerid"), BSON("$inc"<<BSON("value" << 1)));
	return ++m_cur_playerid;	
}


void game_player_mgr::leave_game(uint16_t gameserverid)
{
	int gameid = 0;

	for (auto it = m_playerById.begin(); it != m_playerById.end(); ++it)
	{
		game_player::ObjPtr& p = it->second;
		if(p->get_logicid() == gameserverid)
		{
			if(gameid == 0)
				gameid = p->get_gameid();

			p->leave_game();
			if(p->IsRobot->get_value())
			{
				p->player_logout();				
			}
		}
	}

	if(gameid > 0)
		onClearGame(gameid);
}

void game_player_mgr::onEnterGame(int gameId)
{
	m_onlineNum[gameId]++;
}

void game_player_mgr::onExitGame(int gameId)
{
	if(m_onlineNum[gameId] > 0)
	{
		m_onlineNum[gameId]--;
	}
}

void game_player_mgr::onClearGame(int gameId)
{
	if(m_onlineNum[gameId] > 0)
	{
		m_onlineNum[gameId] = 0;
	}
}


int game_player_mgr::getOnlineNumInGame(int gameId)
{
	int oriNum = m_onlineNum[gameId];
	int r = global_random::instance().rand_int(1, 9);
	return	oriNum * 10 + r;
}

void game_player_mgr::log_playercount(int count)
{
	db_player::instance().update(DB_COMMON_CONFIG, BSON("type"<<"cur_playercount"), BSON("$set"<<BSON("value" << count)));
}

void game_player_mgr::check_kickplayer()
{
	static std::vector<mongo::BSONObj> vec;
	static mongo::BSONObj cond = BSON("time" << mongo::GT << 0);
	static std::string kicktable = "KickPlayer";

	time_t nowt = time_helper::instance().get_cur_time();
	db_player::instance().find(vec, kicktable, cond);
	for (auto it = vec.begin(); it != vec.end(); ++it)
	{
		std::string acc = it->getStringField("key");
		auto player = find_player(acc);
		if(player)//踢出玩家
		{
			auto msg2 = PACKET_CREATE(packetw2c_player_kick, e_mst_w2c_player_kick);
			msg2->set_kick_type(1);
			player->send_msg_to_client(msg2);

			int kt = it->getIntField("time");//秒
			player->KickEndTime->set_value(nowt+kt);
			m_kicklist.push_back(player->PlayerId->get_value());
			//player->player_logout();
			//remove_session(player->get_sessionid());
		}
	}
	if(!vec.empty())
		db_player::instance().clearTable(kicktable);
	vec.clear();
}

//////////////////////////////////////////////////////////////////////////
void game_player_mgr::set_close_state()
{
	b_closing = true;
}

#include "world_server.h"
void game_player_mgr::server_closing(double elapsed)
{
	if(!b_closing)
		return;

	if(!m_playerById.empty())//所有玩家退出
	{
		for (auto it = m_playerById.begin(); it != m_playerById.end(); ++it)
		{
			if(!it->second->is_gaming())
				it->second->player_logout();
		}

		return;
	}
	
	//所有玩家退出10秒后关闭服务器  来保证用户数据同步
	static double close_time = 0;
	close_time+= elapsed;
	if(close_time > 10) 
		world_server::instance().close();
}