#include "stdafx.h"
#include "game_engine.h"
#include <i_game_engine.h>
#include <i_game_ehandler.h>
#include "logic_lobby.h"
#include "game_db.h"
#include "game_db_log.h"
#include <i_game_player.h>
//#include "global_sys_mgr.h"

SHOWHAND_SPACE_USING

game_engine::game_engine(void)
{

}


game_engine::~game_engine(void)
{
}

static const uint16_t GAME_ID = 13;
//static const uint32_t GAME_VER = 1;

//初始化引擎
bool game_engine::init_engine(enable_xml_config& config)
{
	if(get_handler() == nullptr)
		return false;

	std::string filename = config.get<std::string>("game_dll");
	size_t npos = filename.find_last_of('/');
	if(npos != std::string::npos)
		filename = filename.erase(0, npos+1);
	npos = filename.find_last_of('.');
	if(npos != std::string::npos)
		filename = filename.erase(npos);
	com_log::InitLog(filename);

	init_db(config);

	int max_player = config.get<int>("max_player");
	m_lobby.init_game(max_player);

	std::string game_ver = config.get_ex<std::string>("game_ver", "1.0.0");
	get_handler()->on_init_engine(GAME_ID, game_ver);

	//std::cout<<"-----------游戏引擎开始-----------"<<std::endl;
	return true;
}


uint16_t game_engine::get_gameid()
{
	return GAME_ID;
}

//每帧调用
void game_engine::heartbeat( double elapsed )
{
	m_lobby.heartbeat(elapsed);
}

//退出引擎
void game_engine::exit_engine()
{
	//std::cout<<"-----------游戏引擎结束-----------"<<std::endl;
	m_lobby.release_game();

	if(get_handler() != nullptr)
		get_handler()->on_exit_engine();

	com_log::flush();
}

//////////////////////////////////////////////////////////////////////////
//服务器通知游戏逻辑
//玩家进入游戏
bool game_engine::player_enter_game(iGPlayerPtr igplayer)
{
	//std::cout<<"-----------玩家进入游戏-----------"<<std::endl;
	return m_lobby.player_enter_game(igplayer);
}


//玩家离开游戏
void game_engine::player_leave_game(uint32_t playerid, bool bforce)
{
	//std::cout<<"------------------玩家离开游戏------------------"<<std::endl;
	m_lobby.player_leave_game(playerid);
}

int game_engine::player_join_friend_game(iGPlayerPtr igplayer, uint32_t friendid)
{
	int ret = m_lobby.player_join_friend_game(igplayer, friendid);
	if(ret != 1)
		m_lobby.player_leave_game(igplayer->get_playerid());

	return ret;
}

logic_lobby& game_engine::get_lobby()
{
	return m_lobby;
}

void game_engine::init_db(enable_xml_config& xml_cfg)
{
	if(xml_cfg.check("gamedb_url") && xml_cfg.check("gamedb_name"))
	{
		db_game::instance().init_db(xml_cfg.get<std::string>("gamedb_url"), xml_cfg.get<std::string>("gamedb_name"));
	}

	if(xml_cfg.check("logdb_url") && xml_cfg.check("logdb_name"))
	{
		db_log::instance().init_db(xml_cfg.get<std::string>("logdb_url"), xml_cfg.get<std::string>("logdb_name"));
	}

	//db初始化之后才能加载系统
	//global_sys_mgr::instance().sys_load();
}


//返回一个机器人 返回的机器人未进入房间？
void game_engine::response_robot(int32_t playerid, int tag)
{
	//std::cout<<"返回一个机器人：PID:"<<playerid<<"TAG:"<<tag<<std::endl;
	m_lobby.response_robot(playerid, tag);
}

//请求一个机器人
void game_engine::request_robot(int tag, int needgold, int needvip)
{
	get_handler()->request_robot(tag, needgold, needvip);
}


void  game_engine::release_robot(int32_t playerid)
{
	get_handler()->release_robot(playerid);
}