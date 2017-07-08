#pragma once
#include "logic_def.h"
#include "game_showhand_def.pb.h"
#include "ShowHand_RoomCFG.h"
#include "i_game_ehandler.h"
SHOWHAND_SPACE_BEGIN

class logic_room :public game_object 
{
public:
	logic_room(const ShowHand_RoomCFGData* cfg);
	~logic_room(void);

	void heartbeat(double elapsed);

    int enter_room(LPlayerPtr player);
    int leave_room(logic_player*  player);

	void release();

	uint16_t get_player_cout();
    LTABLE_MAP &get_all_tables();
    LTablePtr get_table(int table_id);
	int get_room_id(){return m_room_id;}
    void inform_others(LPlayerPtr, int, int, int);
	GOLD_TYPE get_EnterGold();
	GOLD_TYPE get_EnterTableGold();
	void add_rob_income(GOLD_TYPE income);
	void add_rob_outcome(GOLD_TYPE outcome);

	void add_Stock_income(GOLD_TYPE Income,GOLD_TYPE outcome);//可正可负
	double get_rob_earn_rate();
	void set_rob_earn_rate();
	int get_rob_count(){return m_rob_count;}
    template <class T>
    void broast_msg_to_room_layers(T msg, uint32_t mypid) 
	{
        LPLAYER_MAP::iterator it = m_room_players.begin();
		std::vector<uint32_t> pids;
        for (; it != m_room_players.end(); ++it) 
		{
			if(!it->second->is_inTable() &&it->second->get_pid() != mypid && !it->second->is_android())
				pids.push_back(it->second->get_pid());
        }
		game_engine::instance().get_handler()->broadcast_msg_to_client(pids, msg->packet_id(), msg);
    };

//--------------------------------协议相关------------------------------------------
	int match_auto_table(int last_select_table_id=0);//自动匹配桌子
	void do_protobuf_notice_enter_table(logic_player* player); 
	void do_protobuf_notice_leave_table(int table_id,int pid);
	void do_protobuf_notice_table_player_state(logic_table* table);
	void do_protobuf_notice_Stock_Info(logic_player* player=nullptr);
	void copy_tablelist(google::protobuf::RepeatedPtrField< ::game_showhand_protocols::msg_table_info >* table_list);

	int set_and_decrease_count();//得到机器人的数量
	int rob_match_auto_table();
	logic_player* get_room_Idle_robot();
//-----------------------------------------------------------------------------------------
private:
    void init_tables();
	void create_room();
	bool load_room();
    bool test_probability(int p);
    int can_enter_room(LPlayerPtr);
private:
	void create_robot();
public:
	virtual void init_game_object();//注册属性
	virtual bool store_game_object(bool to_all = false);//非数组对象必须实现此接口

	Tfield<int16_t>::TFieldPtr		m_db_RoomID;			//房间id
	Tfield<int64_t>::TFieldPtr		m_db_TotalIncome;		//当前收入
	Tfield<int64_t>::TFieldPtr		m_db_TotalOutlay;		//当前消耗
	Tfield<int64_t>::TFieldPtr		m_db_EnterCount;	   //进入次数

	Tfield<int64_t>::TFieldPtr		m_db_rob_income;		//机器人收入
	Tfield<int64_t>::TFieldPtr		m_db_rob_outcome;		//机器人支出
	Tfield<double>::TFieldPtr		m_db_rob_EarningsRate;	//盈利率

	Tfield<double>::TFieldPtr		EarningsRate;	    //玩家抽水
	Tfield<int64_t>::TFieldPtr		TotalProfit;		//总盈利
	Tfield<int64_t>::TFieldPtr		TotalStock;			//总库存

private:
	LTABLE_MAP m_tables;
	double m_check_rate;
	double m_checksave;//检查保存的时间
    int m_room_id;
	std::string	m_room_name;
    int m_room_max_player_count;
    int m_room_max_table_count;
	double	m_rob_earn_rate;//机器人盈利率
    bool m_is_open;
    int m_room_enter_gold_condition;
	int m_table_enter_gold_condition;
    int m_illegal_quit_gold_count;
    //底注
    int m_ante;
    //当前房间的玩家
    LPLAYER_MAP m_room_players;

	bool IsOpenRob;
	double m_rob_cd;
	GOLD_TYPE Stock_add_value;
	GOLD_TYPE Profit_add_value;
private:
	int m_rob_count;

	int rob_count;
	double rob_count_cd;
};


SHOWHAND_SPACE_END
