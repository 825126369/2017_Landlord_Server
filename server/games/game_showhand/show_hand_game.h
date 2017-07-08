#pragma once
#include <map>
#include <vector>

#include "i_game_def.h"
#include "logic_def.h"

SHOWHAND_SPACE_BEGIN

//auto show_hand_game *g new show_hand_game();
//g->set_bring_in_score(2000);
//g->start_game(100001, 1000002);
//string cards = g->get_current_card(100001);
//string score = g->get_current_score();

//if (e_call_result::next_around == g->call(...);
//  show_hand_result::ptr shr = g->get_result();
//

#define OPERATION_FOLLOW           "f"
#define OPERATION_ADD              "a"
#define OPERATION_ABANDON          "g"
#define OPERATION_TIME_OUT_ABANDON "g"
#define OPERATION_SHOWHAND         "s"
#define OPERATION_ERROR            "e"

enum e_call_type {
	e_call_start			= 0,//开始游戏
	e_call_add				= 1,//加注
	e_call_follow			= 2,//跟注
	e_call_abandon			= 3,//弃牌
	e_call_showhand			= 4,//梭哈
	e_call_timeout_abandon	= 5,//超时弃牌
};

enum e_call_result {
    e_call_next_step        = 1,//继续CALL
    e_call_next_card        = 2,//发下一张牌
    e_call_next_around      = 3,//下一次游戏
    e_call_error            = 4,//错误的CALL类别
    e_player_gone           = 5,//对方消失了
};

enum cards {
    card_zero       = 0,
    //每局玩家牌数
    card_count      = 5,
    //玩家数量
    player_count    = 2,
    //总牌张数
    total_count     = 28,
	//黑桃
	spade_one       = 0x3E, spade_eight	    = 0x38,	spade_nine		= 0x39,	spade_ten		= 0x3A,	spade_eleven	= 0x3B,	spade_twelve	= 0x3C,	spade_thirteen	= 0x3D,
	//红桃
	heart_one		= 0x2E,	heart_eight		= 0x28,	heart_nine		= 0x29,	heart_ten		= 0x2A,	heart_eleven	= 0x2B,	heart_twelve	= 0x2C,	heart_thirteen	= 0x2D,
	//梅花
	club_one		= 0x1E,	club_eight		= 0x18,	club_nine		= 0x19,	club_ten		= 0x1A,	club_eleven		= 0x1B,	club_twelve		= 0x1C,	club_thirteen	= 0x1D,	
	//方片
	diamond_one		= 0x0E,	diamond_eight	= 0x08,	diamond_nine	= 0x09,	diamond_ten		= 0x0A,	diamond_eleven	= 0x0B,	diamond_twelve	= 0x0C,	diamond_thirteen= 0x0D,

    magic_card      = 0x07,
};

enum card_type {
    //其带有的数字大小属性， 同时也是游戏规则中牌大小的属性
    cards_straight_flush = 9,//同花顺
    cards_four_of_a_kind = 8,//四条
    cards_full_house     = 7,//满堂红
    cards_flush          = 6,//同花
    cards_straight       = 5,//顺子
    cards_three_of_a_kind= 4,//三条
    cards_two_pairs      = 3,//两对
    cards_one_pair       = 2,//一对
    cards_zilch          = 1,//散牌
};

class show_hand_call_data {
public:
    typedef boost::shared_ptr<show_hand_call_data> ptr;

    show_hand_call_data(int32_t i, int c, GOLD_TYPE s):player_id(i),
            call_type(c), score(s){
    }

    int32_t player_id;
    int     call_type;
    GOLD_TYPE score;
};

class show_hand_result {
public:
    typedef boost::shared_ptr<show_hand_result> ptr;

    show_hand_result():win_player_id(0), loss_player_id(0),
                       win_player_score(0), loss_player_score(0), is_show_card(false),
                       win_player_cards(""), loss_player_cards("") {
    }

    int32_t win_player_id;
    int32_t loss_player_id;

    GOLD_TYPE win_player_score;
    GOLD_TYPE loss_player_score;

    bool is_show_card;

    std::string win_player_cards;
    std::string loss_player_cards;
};

class show_hand_game: public enable_obj_pool<show_hand_game>{
public:
struct compare_in_pair {
    bool operator () (const std::pair<int, int> &lhs, const std::pair<int, int> &rhs) const {
        return lhs.second > rhs.second;
    }
};

public:
    typedef boost::shared_ptr<show_hand_game> ptr;

    typedef std::map<int, int> card_container;
    typedef std::map<int32_t, card_container> player_card_container;
    typedef std::list<show_hand_call_data::ptr> players_action_container;
    typedef std::list<std::pair<std::string, int>> action_container;
    typedef std::vector<std::pair<int, int>> vector_pair_container;

public:
    show_hand_game();
    ~show_hand_game();

    std::string get_current_card(int32_t player_id, bool all = false);
    std::string get_current_score(GOLD_TYPE &total_score);
    bool init_game(int32_t, int32_t);
    int call(int32_t player_id, int call_type, GOLD_TYPE call_score, GOLD_TYPE &return_score);
    void player_quit_ongoing_game(int32_t player_id);
    //设置bring in 分数
    void set_bring_in_score(GOLD_TYPE g);
    //获取下一个动作的player id
    int32_t get_next_action_player_id();
    show_hand_result::ptr get_show_hand_result();
    std::string get_call_hint();
    int32_t get_final_winner_id();
    GOLD_TYPE get_bring_in_score();

private:
    //洗牌
    void shuffle();
    //一次把牌发完
    void complete();
    //把m_cards 中 位置0 与位置 pos 的牌交换
    void change(int pos);
    void reset_game_data();
    void reset_call_time_out();
    //获取最大牌玩家的ID
    int32_t get_player_id_max_card(bool is_next_around = false);
    //获取玩家牌的类型
    int get_card_type(card_container &cc, bool is_next_around);
    //获取同类型牌中较大牌的id
    int get_the_max_in_same_type(card_container &player1_card, card_container &player2_card, int card_type, bool is_next_around);
    //获取四条类型 数字
    int get_the_four_of_a_kind_card(card_container &player_card);
    //获取二对类型中的 二数字
    void get_the_two_pairs_card(vector_pair_container &player_card, int &card_big_out, int &card_small_out);
    //获取花色 s h c d
    int get_the_card_shcd(card_container &player_card, int card_in);
    //获取三张牌的数字
    int get_the_three_of_a_kind(card_container &player_card);
    //获取满堂红牌的数字
    void get_the_full_house_card(card_container &player_card_in, int &card_three_out, int &card_two_out);
    //获取一对牌中的数字
    int get_the_one_pair_card(card_container &player_card);
    //获取顺子大的玩家id
    int32_t get_the_cards_straight(card_container &player1_card, card_container &player2_card);
    //获取散牌里面玩家最大牌
    int32_t get_the_cards_zilch(card_container &player1_card, card_container  &player2_card);
    //发下一张牌
    void next_card();
    //本张牌的下一次CALL
    void next_step();
    //下一局游戏
    void next_around();
    int do_call(show_hand_call_data::ptr shcd, GOLD_TYPE &return_score);
    //玩家CALL的动作
    int next_action();
    std::string get_player_card(int32_t player_id);
    bool is_sub_action(const std::string &sub, const std::string &str);
    bool is_action(const std::string &lh, const std::string &rh);
    void load_action();

private:
    //所有牌
    card_container m_cards;
    //玩家手中的牌
    //player id - cards
    player_card_container m_players_cards;
    //当前已发牌数
    int m_current_card_count;
    //当前牌的setp (跟牌，加注，放弃，全押 四种类型的（符合规则的）全迪卡尔积）
    int m_current_step;
    //下一个动作的玩家ID, 或者赢的玩家ID
    int32_t m_next_action_player_id;
    GOLD_TYPE m_bring_in_score;
    //当前两个玩家ID
    int32_t m_player1_id;
    int32_t m_player2_id;
    //玩家call type数据 of one time game
    players_action_container m_players_action;
    //当前牌下两个玩家的所有顺序操作， 该字符串用来判断部分操作的下一个玩家
    //f:跟牌， a:加注， g:放弃， s:ShowHand --
    std::string m_action_string;
    //当前牌下的操作集合, 用来判断操作是否合规
    //如果全部匹配其中一个Key， 则返回一个操作，
    //如果匹配一个Key的部分， 则返回继续CALL
    action_container m_action_set;
    //游戏结果
    show_hand_result::ptr m_show_hand_result;
};

SHOWHAND_SPACE_END