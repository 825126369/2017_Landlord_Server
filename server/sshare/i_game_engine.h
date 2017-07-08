#pragma once
#include <i_game_def.h>
#include <enable_xml_config.h>


//每个游戏必须实现的引擎
class i_game_engine
{
public:
	i_game_engine();
	virtual ~i_game_engine();

	//初始化引擎
	virtual bool init_engine( enable_xml_config& config) = 0;

	//每帧调用
	virtual void heartbeat( double elapsed ) = 0;

	//退出引擎
	virtual void exit_engine() = 0;

	//////////////////////////////////////////////////////////////////////////
	//服务器通知游戏逻辑
	//玩家进入游戏
	virtual bool player_enter_game(iGPlayerPtr igplayer) = 0;

	//玩家离开游戏
	virtual void player_leave_game(uint32_t playerid, bool bforce = false) = 0;

	//玩家进入好友的桌子
	virtual int player_join_friend_game(iGPlayerPtr igplayer, uint32_t friendid) = 0;

	virtual uint16_t get_gameid() =0;

	//返回一个机器人 返回的机器人未进入房间？
	virtual void response_robot(int32_t playerid, int tag) = 0;
public:
	//要在init_engine之前调用
	void set_handler(i_game_ehandler* ehandler);
	i_game_ehandler* get_handler();
private:
	i_game_ehandler* m_ehandler;

};
