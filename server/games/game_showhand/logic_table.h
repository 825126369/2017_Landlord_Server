#pragma once
#include "logic_def.h"
#include "i_game_def.h"
#include "game_showhand_def.pb.h"
#include "cardmanager.h"
#include <vector>
#include <string>
#include "ShowHand_RoomStockCFG.h"
class cardmanager;
SHOWHAND_SPACE_BEGIN

class logic_player;
class show_hand_result;
typedef std::shared_ptr<cardmanager> PCARDMANAGER;
class logic_table: public enable_obj_pool<logic_table>
{
public:
	logic_table(logic_room*,int);
	virtual ~logic_table();

	void heartbeat(double elapsed);

	virtual uint32_t get_id();
	int enter_table(logic_player* player);//进入桌子
	int quit_table(logic_player* player);//离开桌子

	logic_player* get_player_left();//得到左边玩家
	logic_player* get_player_right();//得到右边玩家

	game_showhand_protocols::p_bet_state* get_left_betstate();//得到左边玩家下注状态
	game_showhand_protocols::p_bet_state* get_right_betstate();//得到右边玩家下注状态

	GOLD_TYPE get_betcount(int32_t pid);//得到玩家总下注

	void set_bet_gold_count(); //设置积分
	GOLD_TYPE get_min_bet_gold();
	int get_player_count();//得到玩家数量
	bool is_empty();
	bool get_or_have_hasuo();
	game_showhand_protocols::e_game_state get_game_state();

	GOLD_TYPE get_rob_income();//收入
	GOLD_TYPE get_rob_outcome();//支出
	GOLD_TYPE set_rob_income();//设置机器人的收入
	PCARDMANAGER get_cardmanager();
	int get_currentCardIndex(){return currentCardIndex;}
	void initTablePlayer(bool left);
	void set_GM_CONTROL_COMMAND(int command);
public:
	void do_protobuf_player_ready(int pid,int result) ;//玩家准备
	void do_protobuf_notice_start_game();//桌子
	void do_protobuf_notice_view_card(int whoSeeCardId);//看牌
	void do_protobuf_notice_bet(int32_t playerId, const game_showhand_protocols::msg_bet_info& betinfo); //玩家下注
	void do_protobuf_notice_sendcard();//玩家发牌
	void do_protobuf_notice_award();//玩家奖励
	void do_protobuf_notice_enter_table(logic_player* player);//进入桌子（只通知桌子里的另一个人）
	void do_protobuf_notice_leave_table(int pid);//离开桌子（只通知桌子里另一个人）
	void do_protobuf_get_table_scene_info(logic_player* player);
	void do_protobuf_notice_gm_private_Info();
	void kich_table_rubbish(logic_player* lcplayer);
	//void kich_room_rubbish(logic_player* lcplayer);
	GOLD_TYPE get_bet_cout(const int32_t& pid,const game_showhand_protocols::msg_bet_info& betinfo);//得到玩家下注的数量
	void copy_table_info(game_showhand_protocols::msg_table_info* table_info);

	int get_once_bet_size(){ return once_bet_list.size();}
private:
	void adjust_earn_rate();
	void start_game_prepare();//游戏开始准备工作
	void ImmediatelyOpenAward();//立即开奖
	void set_bet_state(game_showhand_protocols::p_bet_state* bet_state,logic_player* player,int32_t player_id,const game_showhand_protocols::msg_bet_info* betinfo);
	void robot_heart(double elapsed);
public:
	//广播协议，立刻发送
	template<class T>
	int broadcast_msg_to_client(T msg, int32_t mypid = 0)
	{
		std::vector<uint32_t> pids;	
		if (m_player_left != nullptr && !m_player_left->is_android() && m_player_left->get_pid()!=mypid) 
		{
			pids.push_back(m_player_left->get_pid());
		}

		if (m_player_right != nullptr && !m_player_right->is_android() && m_player_right->get_pid()!=mypid) 
		{
			pids.push_back(m_player_right->get_pid());
		}
		
		return game_engine::instance().get_handler()->broadcast_msg_to_client(pids, msg->packet_id(), msg);
	}
private:
	double m_bet_time;//下注时间20秒
	double m_checksave;//检查保存的时间
	logic_room* m_room;
	int16_t m_table_Id;

	logic_player* m_player_left;
	logic_player* m_player_right;

	GOLD_TYPE min_bet_count;//本局游戏最小下注金额
	GOLD_TYPE max_bet_count;//本局游戏最大下注金额

	GOLD_TYPE left_bet_cout;//我下注总金额
	GOLD_TYPE right_bet_cout;//我下注总金额

	bool Is_left_abandonCard;//是否弃牌
	bool Is_right_abandonCard;

	bool Is_left_ShowHands;//是否哈所
	bool Is_right_ShowHands;

	int32_t left_escape_pid;//是否逃跑
	int32_t right_escape_pid;

	std::vector<GOLD_TYPE> once_bet_list;//每次发牌完的押注金额列表

	std::vector<game_showhand_protocols::msg_bet_info> once_betinfo_list;//用来断线重连（每次发完牌的押注信息列表）

	//std::vector<game_showhand_protocols::msg_bet_info> left_bet_list;//左边下注列表
	//std::vector<game_showhand_protocols::msg_bet_info> right_type_list;//右边下注列表

	game_showhand_protocols::p_bet_state* left_bet_state;//左边玩家状态
	game_showhand_protocols::p_bet_state* right_bet_state;//右边玩家状态
	int currentCardIndex;
	PCARDMANAGER m_cardmanager;
	game_showhand_protocols::e_game_state m_game_state;//游戏状态

	double award_time;
	double takecard_time;
	double left_kichRubbish_cd;
	double right_kichRubbish_cd;


	GOLD_TYPE m_rob_income;
	GOLD_TYPE m_rob_outcome;

	double m_rob_cd;

	int GM_COMMAND;//1:左边玩家赢，2：右边玩家赢
};

SHOWHAND_SPACE_END