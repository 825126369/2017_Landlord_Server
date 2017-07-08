#pragma once
#include "logic_def.h"
#include <i_game_phandler.h>
#include <vector>
#include "proc_game_happysupremacy_protocol.h"
#include "i_game_player.h"

class logic_lobby;

class logic_player_db:public game_object
{
public:
	bool load_player();
	virtual void init_game_object() override;//注册属性
	virtual bool store_game_object(bool to_all = false) override;//非数组对象必须实现此接口
public:
	Tfield<int32_t>::TFieldPtr m_player_id;
	Tfield<GOLD_TYPE>::TFieldPtr m_once_win_maxgold;			//单局最大金币盈利数
	Tfield<int32_t>::TFieldPtr m_win_count;    //星星抽奖累计赢钱局数 
};

class logic_player: 
	public enable_obj_pool<logic_player>
	,public i_game_phandler
	,public logic_player_db
{
public:
	logic_player(void);
	virtual ~logic_player(void);

	void heartbeat( double elapsed );

	//-----------------------------------------------------------------------
	//从服务器通知逻辑的接口
	virtual void on_attribute_change(int atype, int v) override;			//金钱改变

	virtual void on_change_state() override;					//玩家掉线

	virtual void on_attribute64_change(int atype, GOLD_TYPE v = 0) override;

	//------------------------------------------------------------------------
	bool is_robot();

	const std::string& get_nickname();
	const std::string& get_icon_custom();
	const uint32_t get_head_frame_id();
	const int16_t get_player_sex();
	bool is_GM_CONTROL();
	uint32_t get_pid();
	uint16_t get_viplvl();
	//获取玩家当前金币(游戏调用)
	GOLD_TYPE get_gold();
	//获取礼券
	int get_ticket();
	//改变金币(游戏调用)
	bool change_gold(GOLD_TYPE v);
	bool change_gold2(int v, int season);
	bool change_ticket(int count,int reason = -1);

	void set_bet_max();

	void enter_game(logic_lobby* lobby);		//进入游戏
	bool enter_room(logic_room* room);			//进入房间
	void leave_room();							//离开房间
	void escapeHandle();
	logic_room* get_room(){return m_room;}
	e_player_state get_game_state();
	void release();								//退出整个游戏客户端

	void clear_once_data();						//清理单局押注
	
	msg_type_def::e_msg_result_def add_bet(e_bet_type,int count);		//下注
	msg_type_def::e_msg_result_def repeat_bet();						//续压
	msg_type_def::e_msg_result_def clear_bet();							//清零
	msg_type_def::e_msg_result_def leave_banker();						//下庄
	void add_bet_win(GOLD_TYPE count);										//单局盈利
	GOLD_TYPE get_bet_win(){return m_once_win_gold;}
	std::map<e_bet_type,GOLD_TYPE>& get_bet_list();
	GOLD_TYPE get_bet_total_count(){return m_bet_gold_count;}
	bool get_is_banker(){return is_banker;}
	bool set_is_banker(bool isbanker);

	void sycn_gold();
	void onAcceptGift(int receiverId, int giftId);

	void add_star_lottery_info(int32_t award,int32_t star = 0);

	void quest_change(int quest_type,int count,int param = 0);
public:
	void clear_table_data();		//离开某张桌子时的清理

	void bc_game_msg(int money, const std::string& sinfo, int mtype = 1);

	template<class T>
	int send_msg_to_client(T msg)
	{
		return m_player->send_msg_to_client(msg);
	};
private:
	logic_lobby* m_lobby;
	logic_room* m_room;

	GOLD_TYPE m_logic_gold;					//当前拥有金币数量
	GOLD_TYPE m_change_gold;					//单局变化金币

	GOLD_TYPE m_once_win_gold;				//单局赢得的金币数量

	std::map<e_bet_type,GOLD_TYPE> m_bet_list;			//当局下注总数
	std::map<e_bet_type,GOLD_TYPE> m_old_bet_list;		//上局下注总数
	GOLD_TYPE m_bet_gold_count;			//本次下注总金额

	bool is_banker;	//是否是庄家

	GOLD_TYPE m_bet_max;//当前下注上限

	double m_checksave;
	bool is_first_save;
private:
	double m_rob_bet_cd;				//机器人下注CD
	int m_rob_bet_max;					//单个机器人下注最大值
	int m_rob_bet_remain;					//单个机器人剩余下注
	int get_rob_bet();
public:
	void robot_heartbeat(double elapsed);
};