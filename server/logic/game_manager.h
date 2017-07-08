#pragma once

//此管理仅支持win，可以改用ace来跨平台

#include "windows.h"
#include <enable_singleton.h>
#include "i_game_ehandler.h"
#include "robot_manager.h"

class i_game_engine;
class packet_manager;

class game_manager : 
	public enable_singleton<game_manager>
	,public i_game_ehandler
{
public:
	game_manager();
	~game_manager();


	bool open();
	void close();

	i_game_engine* get_game_engine();
	packet_manager* get_packet_mgr();

	//初始化引擎
	virtual void on_init_engine(uint16_t game_id, const std::string& game_ver);

	//游戏关闭
	virtual void on_exit_engine();

	//广播协议
	virtual int broadcast_msg_to_client(std::vector<uint32_t>& pids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);

	//发送协议列表
	virtual int broadcast_msglist_to_client(std::vector<uint32_t>& pids, std::vector<msg_packet_one>& msglist);

	uint16_t get_gameid();
	int get_gamever();

	virtual void game_manager::release_player(uint32_t pid) override;
	//请求机器人 请求的机器人不一定及时返回  
	//要求的vip 要求的gold  自定义标志tag
	virtual void request_robot(int tag, GOLD_TYPE needgold, int needvip = 0);
	
	//当不需要使用机器人时 只要退出到房间选择然后调用此函数
	virtual void release_robot(int playerid);
	//获取当前机器人数
	virtual int get_robot_count();
	//返回机器人
	void response_robot(int32_t playerid);

	void heartbeat(double elapsed);

	virtual bool setGameDb(db_base *dgGame);

	virtual int64_t getCurId(const std::string& key);
private:
	void game_regedit();

	robot_manager m_robotmgr;
	i_game_engine* m_engine;
	packet_manager* m_pkmgr;
	uint16_t m_gameid;
	int m_gamever;
	HMODULE m_mod;
};

