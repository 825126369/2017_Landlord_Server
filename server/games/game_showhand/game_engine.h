#pragma once

#include <enable_singleton.h>
#include "i_game_engine.h"
#include "logic_lobby.h"
#include <enable_xml_config.h>

class game_engine :
	public i_game_engine
	,public enable_singleton<game_engine>
{
public:
	game_engine(void);
	virtual ~game_engine(void);

	//初始化引擎
	virtual bool init_engine(enable_xml_config& config);

	//每帧调用
	virtual void heartbeat( double elapsed );

	//退出引擎
	virtual void exit_engine();

	//////////////////////////////////////////////////////////////////////////
	//服务器通知游戏逻辑
	//玩家进入游戏
	virtual bool player_enter_game(iGPlayerPtr igplayer);

	//玩家离开游戏
	virtual void player_leave_game(uint32_t playerid, bool bforce = false);

	//玩家进入好友的桌子
	virtual int player_join_friend_game(iGPlayerPtr igplayer, uint32_t friendid);

	showhand_space::logic_lobby& get_lobby();

	virtual uint16_t game_engine::get_gameid();

	//返回一个机器人 返回的机器人未进入房间？
	virtual void response_robot(int32_t playerid, int tag);
	//请求一个机器人
	void request_robot(int tag, int needgold, int needvip = 0);
	//释放机器人
	void release_robot(int32_t playerid);
private:
	showhand_space::logic_lobby m_lobby;

	void init_db(enable_xml_config& xml_cfg);
};