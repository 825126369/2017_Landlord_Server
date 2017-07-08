#pragma once
#include "logic_def.h"
#include "logic_core.h"
#include "proc_game_happysupremacy_protocol.h"

struct HappySupremacy_RoomCFGData;
struct HappySupremacy_RoomStockCFGData;
class HistoryArray;

class logic_room_db:public game_object
{
protected:
	void create_room();
	bool load_room();
	void reflush_rate();//重置收支
public:
	virtual void init_game_object() override;//注册属性
	virtual bool store_game_object(bool to_all = false) override;//非数组对象必须实现此接口

	Tfield<int16_t>::TFieldPtr		m_db_room_id;			//房间id
	Tfield<int64_t>::TFieldPtr		m_db_room_income;		//当前收入(所有玩家总押注)
	Tfield<int64_t>::TFieldPtr		m_db_room_outcome;		//当前消耗（所有玩家总收益）
	Tfield<int64_t>::TFieldPtr		m_db_enter_count;		//进入次数
	Tfield<double>::TFieldPtr		m_db_ExpectEarnRate;		//预期盈利率
	Tfield<int64_t>::TFieldPtr		m_db_player_charge;	//玩家上庄抽水

	GArrayFieldPtr					m_db_room_history;  //牌路
	boost::shared_ptr<HistoryArray> m_db_room_historyPtr;

	Tfield<int64_t>::TFieldPtr		m_db_rob_income;		//机器人收入
	Tfield<int64_t>::TFieldPtr		m_db_rob_outcome;		//机器人支出

	Tfield<double>::TFieldPtr		EarningsRate;	//抽水比率
	Tfield<int64_t>::TFieldPtr		TotalProfit;		//总盈利
	Tfield<int64_t>::TFieldPtr		TotalStock;			//总库存
};

class logic_room :public logic_room_db
{
public:
	logic_room(const HappySupremacy_RoomCFGData* cfg, logic_lobby* _lobby);
	~logic_room(void);

	void heartbeat( double elapsed );
	uint16_t get_room_id();			//房间ID
	e_game_state get_room_state(){return m_game_state;}
	void set_is_have_bet(bool is_have);		//下注期间是否有押注

	bool room_is_full();
	uint16_t enter_room(LPlayerPtr player);		//进入房间
	void leave_room(uint32_t playerid);			//离开房间

	const HappySupremacy_RoomCFGData* get_room_cfg() const;
	logic_lobby* get_lobby(){return m_lobby;};

	void adjust_earn_rate(); //发牌
	bool sync_bet_to_room(); //重新收集所有玩家下注信息
	void bc_begin_bet();		//通知开始下注
	void compute_reward();		//计算各个玩家的奖励
	bool refreshBanker();		//刷新庄家

	void bc_begin_reward();		//广播输赢情况
	void bc_sync_bet_info();	//广播总下注信息
	void bc_change_banker(bool is_rob);	//广播更换庄家
	void bc_rob_banker_info();	//广播抢庄信息

	msg_type_def::e_msg_result_def add_banker_list(uint32_t playerid);		//申请加入上庄列表
	int32_t get_continue_banker_count(){return m_continue_banker_count;}
	void set_now_banker_null(int32_t playerid);//（无庄家）初始化庄家状态
	void add_history_list();		//保存到牌路

	msg_type_def::e_msg_result_def set_rob_banker(int32_t playerid);//设置抢庄玩家信息
	int32_t get_now_banker_id(){return m_now_banker_id;}
	bool is_real_banker();	//是否真实庄家（与机器人的区别）

	GOLD_TYPE get_can_bet_count();//得到单个玩家还可以下注的数量
	int32_t get_win_index();
	int32_t get_no_banker_count();//得到连续无庄局数（但有系统小庄）
	int32_t get_banker_list_size(){return m_banker_list.size();}	//庄家申请列表
	GOLD_TYPE get_banker_win(){return m_banker_total_win;}	//得到庄家赢钱的数量
	int32_t get_continue_count(){return m_continue_banker_count;}	//得到继续上庄的局数
	double get_cd_time(){return m_cd_time;}

	GOLD_TYPE get_total_bet_count(){return m_total_bet_count;}
	double getRobEarnRate(){return m_rob_earn_rate;} //机器人赢得概率
	//gm相关
	void set_gm(int count);
	int get_gm_result();//后台设置下局结果

	void set_GM_CONTROL_COMMAND(int command){GM_CONTROL_COMMAND=command;}
	void set_GM_CONTROL_COMMAND_LIST(const std::vector<e_bet_type>& list)
	{
		GM_CONTROL_COMMAND_LIST=list;
	}

	void set_Stock(GOLD_TYPE Income,GOLD_TYPE outcome);//设置库存//设置库存
	void get_pre_Stock_Income(GOLD_TYPE* income,GOLD_TYPE* outcome);
	void set_player_Stock_bet_count();
public:
	template<class T>
	int broadcast_msg_to_client(T msg, uint32_t except_id = 0)
	{
		return broadcast_msg_to_client(msg->packet_id(), msg, except_id);
	};
	int broadcast_msg_to_client(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg, uint32_t except_id);

	boost::shared_ptr<game_happysupremacy_protocols::packetl2c_get_room_scene_info_result> get_room_scene_info();		//获得场景协议
	boost::shared_ptr<game_happysupremacy_protocols::packetl2c_ask_for_player_list_result> get_room_player_list();	//获得玩家列表协议
	boost::shared_ptr<game_happysupremacy_protocols::packetl2c_ask_for_banker_list_result> get_room_banker_list();	//获得庄家列表协议
	boost::shared_ptr<game_happysupremacy_protocols::packetl2c_ask_for_history_list_result> get_room_history_list();	//获得庄家列表协议
private:
	const HappySupremacy_RoomCFGData* m_cfg;
	const HappySupremacy_RoomStockCFGData* m_StockCFG;
	logic_lobby* m_lobby;
	LPLAYER_MAP playerMap;		//所有玩家字典
	logic_core* m_core_engine;	//发牌器

	e_game_state m_game_state;
	double m_cd_time;
	bool is_have_bet;
	double m_elapse;
	double m_checksave;

	double m_rob_cd;	//机器人进入间隔
	int m_rob_count;	//机器人数量
	double	m_rob_earn_rate; //机器人的概率

	int m_no_banker_count;			//无庄连续局数

	GOLD_TYPE m_total_bet_count; //房间总下注数量
	std::map<e_bet_type,GOLD_TYPE> m_room_bet_list;//各个类型的下注数量
	std::map<e_bet_type,GOLD_TYPE> m_room_player_bet_list;//机器人下注数量

	std::list<history_info> m_history_list;	//记录下注结果的历史记录
	bool is_refresh_history;				//是否刷新牌路

	GOLD_TYPE m_once_income;			//单局收到总押注
	GOLD_TYPE m_once_outcome;			//单局赔出的金币
	GOLD_TYPE m_banker_once_win;		//单局庄家收益
	GOLD_TYPE m_banker_total_win;		//庄家总收益（上庄后一直累加）

	double m_draw_water_rate;		//收税比例
	GOLD_TYPE m_system_draw_water;    //庄家下庄后系统收税比例（更换庄家时，才结算税收）

	std::list<uint32_t> m_banker_list;   //申请上庄列表
	int32_t m_continue_banker_count;		//连庄次数
	int32_t m_now_banker_id;				//当前庄家ID

	int32_t m_old_banker_id;	            //上次庄家ID

	bool is_change_banker;					//庄家是否有改变
	bool is_have_banker;					//当前是否有玩家坐庄

	bool is_can_rob_banker;					//是否能抢庄
	int32_t m_rob_banker_id;				//当前抢庄玩家ID
	GOLD_TYPE m_rob_banker_cost;			//当前抢庄花费
	bool is_have_rob_banker;				//是否需要广播抢庄信息

	bool is_gm;
	int32_t gm_index;
	int32_t gm_max;

	int GM_CONTROL_COMMAND;//是否控制客户端发来请求1:收分，2:放分,3:全输
	std::vector<e_bet_type> GM_CONTROL_COMMAND_LIST;//7（全输）
private:
	int IsOpenRob;
	int IsOpenGM;
	int robMinCount;
	int currentNeedRobCount;
	double currentNeedRobCount_cd;
	void create_robot();
private:
	int GetPeopleRate(int iMaxNum);
	int SetCurrentNeedRobCount();
	void InitBetInfo();//初始化房间里所有下注信息
	void saveHistoryRecord();
};

class HistoryItem : public game_object, public enable_obj_pool<HistoryItem>
{
public:
	HistoryItem();

	virtual void init_game_object() override;

	Tfield<bool>::TFieldPtr is_forwarddoor_win;
	Tfield<bool>::TFieldPtr is_reversedoor_win;
	Tfield<bool>::TFieldPtr is_oppositedoor_win;
};

class HistoryArray : public game_object_array, public enable_obj_pool<HistoryArray>
{
public:
	virtual const std::string& get_cells_name() override;	

	virtual GObjPtr create_game_object(uint32_t object_id) override;

	virtual const std::string& get_id_name() override;

	virtual bool update_all() override { return true; }
};
