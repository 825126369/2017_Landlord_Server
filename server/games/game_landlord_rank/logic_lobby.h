#pragma once
#include "logic_def.h"
#include <i_game_def.h>
#include "logic_room.h"

class logic_lobby
{
public:
	logic_lobby(void);
	~logic_lobby(void);

	void init_game();					//游戏初始化	//游戏承载人数限制
	void release_game();							//卸载游戏	//清理玩家数据
	void heartbeat( double elapsed );				//游戏循环

	bool player_enter_game(iGPlayerPtr igplayer);	//玩家进入游戏
	void player_leave_game(uint32_t playerid);		//玩家离开游戏
	int player_join_friend_game(iGPlayerPtr igplayer, uint32_t friendid);	//加入好友房间

	//机器人进入房间
	int enter_room(uint32_t pid, uint16_t rid);		//进入
	void leave_room(uint32_t pid);					//离开桌子

	void response_robot(int32_t playerid, int tag);

	const LROOM_MAP& get_rooms() const;				//得到所有房间信息
	LPlayerPtr& get_player(uint32_t pid);			//返回玩家信息
private:
	void init_config();				//加载配置
	void init_protocol();			//加载协议
	void init_room();			//初始化房间
private:
	bool m_init;					//是否已初始化
	int m_max_player;				//最大人数限制

	LROOM_MAP roomMap;				//所有房间
	LPLAYER_MAP playerMap;		//所有玩家
	std::vector<LPlayerPtr> mdelplayerList;
};
