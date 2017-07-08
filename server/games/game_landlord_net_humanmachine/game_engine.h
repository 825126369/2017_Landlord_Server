#pragma once

#include <enable_singleton.h>
#include "i_game_engine.h"
#include "logic_lobby.h"
#include "logic_room.h"
#include <enable_xml_config.h>

class game_engine :
	public i_game_engine
	,public enable_singleton<game_engine>
{
public:
	game_engine(void);
	virtual ~game_engine(void);

	virtual bool init_engine( enable_xml_config& config) override;		//初始化引擎
	virtual void heartbeat( double elapsed ) override;					//每帧调用
	virtual void exit_engine() override;									//退出引擎


	virtual bool player_enter_game(iGPlayerPtr igplayer) override;		//玩家进入游戏
	virtual void player_leave_game(uint32_t playerid, bool bforce = false) override;			//玩家离开游戏

	virtual int player_join_friend_game(iGPlayerPtr igplayer, uint32_t friendid) override;

	virtual uint16_t game_engine::get_gameid() override;
	//返回一个机器人 返回的机器人未进入房间？
	virtual void response_robot(int32_t playerid, int tag) override;

public:
	//请求一个机器人
	void request_robot(int tag, int needgold, int needvip = 0);
	//释放机器人
	void release_robot(int32_t playerid);

	logic_lobby& get_lobby() { return m_lobby; }
private:
	void init_db(enable_xml_config& xml_cfg);
	logic_lobby m_lobby;
};

