#pragma once
#include "logic_def.h"
#include <i_game_phandler.h>
#include <i_game_player.h>
#include "game_showhand_protocol.pb.h"

SHOWHAND_SPACE_BEGIN

enum player_table_state
{
	e_table_state_none=0,
	e_table_state_no_prepare=1,//未准备
	e_table_state_prepare=2,//准备
	e_table_state_in_game=3,//在游戏中
};

class logic_player_db:public game_object
{
public:
	bool create_player();
	bool load_player();
	virtual void init_game_object() override;//注册属性
	virtual bool store_game_object(bool to_all = false) override;//非数组对象必须实现此接口
public:
	Tfield<int32_t>::TFieldPtr m_db_player_id;
	Tfield<GOLD_TYPE>::TFieldPtr m_db_once_win_maxgold;			//单局最大金币盈利数
	Tfield<int32_t>::TFieldPtr m_db_win_count;    //星星抽奖累计赢钱局数 
};

class logic_player: public enable_obj_pool<logic_player>,public i_game_phandler, public logic_player_db
{
public:
    typedef boost::shared_ptr<logic_player> ptr;

	logic_player(void);
	virtual ~logic_player(void);

	void heartbeat( double elapsed );

	//从服务器通知逻辑的接口
	//属性
	virtual void on_attribute_change(int atype, int v);

	virtual void on_attribute64_change(int atype, GOLD_TYPE v = 0);

    //玩家状态改变接口
	virtual void on_change_state(void);
    //退出整个游戏
	void release();

    bool is_offline();
	uint32_t get_pid();//玩家ID
	int get_lucky();
	bool change_lucky(int v);
	bool Is_Luckly_Valid(int addgold);
	bool is_GM_CONTROL();
	//获取玩家当前金币(游戏调用)
	GOLD_TYPE get_gold();
	//获取玩家VIP等级
	int16_t get_viplvl();
	//是否体验VIP
	bool is_ExperienceVIP();
	//是否机器人
    bool is_android();
	//获取礼券
	int get_ticket();
	//改变金币(游戏调用)
	bool change_gold(GOLD_TYPE v, bool needbc = false);
    bool synchronization_gold();
	//改变礼券(游戏调用)
	bool change_ticket(int v, bool needbc = false);
	//获取昵称
	const std::string& get_nickname();
	int get_sex();
	int get_photo_frame();
	const std::string& get_icon_custom();
	void bc_game_msg(int money, const std::string& sinfo, int mtype = 1);

	void init_tickettime();
	int check_ticket();//检测每日送礼券
    int get_player_type();

	bool is_inRoom();
	bool is_inTable();

	//得到玩家世界状态
    int get_player_world_state();

	//得到玩家桌子状态
    int get_player_table_state();
	void set_player_table_state(player_table_state state);

    void start_offline_timer();

	template<class T>
	int send_msg_to_client(T msg) 
	{
		return m_player->send_msg_to_client(msg->packet_id(), msg);
	}

	void copy_player_info(game_showhand_protocols::msg_player_info* player_info);
public:
	void enter_game(logic_lobby* lobby);
	int enter_lobby();
	int leave_lobby();
	int enter_room(int);
	int leave_room();
	int enter_table(int);
	int leave_table(); 
	logic_room* get_room();
	logic_table* get_table();

	int prepare_game();
	void seecard();
	int set_min_bet(GOLD_TYPE count);
	int add_bet(const game_showhand_protocols::msg_bet_info& betinfo); 
	GOLD_TYPE getbetCount();//得到总压注
	GOLD_TYPE getlastbetCount();//得到上一次的押注
	int get_last_select_table_id(){return last_select_table_id;}
	void set_last_select_table_id(int t_id){last_select_table_id=t_id;}
	void do_protobuf_notice_Luck_Info();
	GOLD_TYPE TempIncome;	//幸运值收入
	GOLD_TYPE TotalIncome; //幸运值总收入
private:
	void robot_heartbeat(double elapsed );
	void get_rob_bet(game_showhand_protocols::p_bet_state* mstate,game_showhand_protocols::msg_bet_info* betinfo1);
	bool judge_rob_win_lose(int currentCardIndex);
	void get_rob_cheat_bet(game_showhand_protocols::p_bet_state* mstate,game_showhand_protocols::msg_bet_info* betinfo1);
	game_showhand_protocols::e_bet_type get_rob_bet_type(game_showhand_protocols::p_bet_state* mstate);//随机AIs
	game_showhand_protocols::e_bet_type get_rob_bet_type_1(game_showhand_protocols::p_bet_state* mstate);//高级AI
private:
	int last_select_table_id;
	int64_t m_logic_gold;
	int64_t m_change_gold;

	double m_check_sync;
	double m_check_ticket;
	double m_cur_tickettime;
	double m_kich_rubbish_cd;

	logic_room* m_room;
	logic_table* m_table;
	player_table_state m_player_table_state;
	//game_showhand_protocols::msg_bet_info mLastTakeCard;
	//std::vector<game_showhand_protocols::msg_bet_info> bet_list;//下注列表

	double clearrubbish_cd;
	double entertable_cd;
	double prepare_cd;
	double bet_cd;
	double bet_max_cd;
	double viewcard_cd;
	double viewcard_max_cd;
	bool or_once_bet;
	int rob_continuePlayCount;
	int rob_continuePlayCount_max;
	double robot_auto_quittable_cd;
	double quit_table_rob_cd_max;

	int Luck_add_value;//增加的幸运值
};

SHOWHAND_SPACE_END