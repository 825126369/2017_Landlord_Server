#include "stdafx.h"

#include "show_hand_game.h"

#include "ShowHand_CallStep.h"

SHOWHAND_SPACE_USING

const static int quantum[cards::total_count] = {
    //黑桃
	spade_eight,	spade_nine,	spade_ten,	spade_eleven,	spade_twelve,	spade_thirteen,spade_one,
	//红桃
	heart_eight,	heart_nine,	heart_ten,	heart_eleven,	heart_twelve,	heart_thirteen,heart_one,
	//梅花
	club_eight,	club_nine,	club_ten,	club_eleven,	club_twelve,	club_thirteen,club_one,
	//方片
	diamond_eight,	diamond_nine,	diamond_ten,	diamond_eleven,	diamond_twelve,	diamond_thirteen,diamond_one,
};

show_hand_game::show_hand_game(): m_player1_id(0), m_player2_id(0),
    m_current_card_count(0), m_bring_in_score(0) {

    m_cards.clear();
    for (int i = cards::card_zero; i < cards::total_count; ++i)
        m_cards.insert(std::make_pair(i, quantum[i]));

    load_action();
}

show_hand_game::~show_hand_game() {

}

void show_hand_game::reset_game_data() {
    m_cards.clear();
    for (int i = cards::card_zero; i < cards::total_count; ++i)
        m_cards.insert(std::make_pair(i, quantum[i]));

    m_current_card_count = 0;

    m_next_action_player_id = 0;

    m_bring_in_score = 0;

    m_player1_id = 0;
    m_player2_id = 0;
    m_players_cards.clear();

    players_action_container pac;
    m_players_action.swap(pac);

    m_action_string = "";
}

bool show_hand_game::init_game(int32_t pid1, int32_t pid2) {
    reset_game_data();

    m_player1_id = pid1;
    m_player2_id = pid2;
    m_players_cards.clear();

    card_container card;
    for (int i = cards::card_zero; i < cards::card_count; ++i)
        card.insert(std::make_pair(i, cards::card_zero));
    m_players_cards.insert(std::make_pair(m_player1_id, card));

    card_container c2;
    for (int i = cards::card_zero; i < cards::card_count; ++i)
        c2.insert(std::make_pair(i, cards::card_zero));
    m_players_cards.insert(std::make_pair(m_player2_id, c2));

    return true;
}

show_hand_result::ptr show_hand_game::get_show_hand_result() {
    return m_show_hand_result;
}

int32_t show_hand_game::get_final_winner_id() {
    return get_player_id_max_card(true);
}

std::string show_hand_game::get_call_hint() {
    std::string available_actions("");

    if (m_current_card_count <= 2 && m_action_string.empty())
        available_actions = OPERATION_FOLLOW OPERATION_ADD OPERATION_ABANDON;

    else if (m_action_string.empty())
        available_actions = OPERATION_FOLLOW OPERATION_ADD OPERATION_ABANDON OPERATION_SHOWHAND;

    else {
        std::set<char> temp_re;
        for (action_container::iterator it = m_action_set.begin(); it != m_action_set.end(); ++it) {
            if (is_sub_action(m_action_string, it->first)) {
                char the_char = (it->first).at(m_action_string.size());
                if (m_current_card_count <= 2 && the_char == std::string(OPERATION_SHOWHAND).at(0))
                    temp_re.insert(the_char);

                if (temp_re.find(the_char) == temp_re.end()) {
                    available_actions.push_back(the_char);
                    temp_re.insert(the_char);
                }
            }
        }
    }

    return available_actions;
}

void show_hand_game::player_quit_ongoing_game(int32_t player_id) {
    m_next_action_player_id = 0;

    if (!m_show_hand_result)
        m_show_hand_result = show_hand_result::ptr(new show_hand_result);

    m_show_hand_result->is_show_card = false;
    m_show_hand_result->loss_player_id = player_id;
    m_show_hand_result->win_player_id = player_id == m_player1_id ? m_player2_id : m_player1_id;

    m_show_hand_result->win_player_cards = get_player_card(m_show_hand_result->win_player_id);
    m_show_hand_result->loss_player_cards = get_player_card(m_show_hand_result->loss_player_id);

    m_show_hand_result->win_player_score += m_bring_in_score;
    m_show_hand_result->loss_player_score += m_bring_in_score;

    for (players_action_container::iterator it = m_players_action.begin(); it != m_players_action.end(); ++it) {
        if ((*it)->player_id == m_show_hand_result->win_player_id) {
            m_show_hand_result->win_player_score += (*it)->score;

        } else {
            m_show_hand_result->loss_player_score += (*it)->score;
        }
    }
}

int show_hand_game::call(int32_t player_id, int call_type, GOLD_TYPE call_score, GOLD_TYPE &return_score) {
    switch (call_type) {
        case e_call_type::e_call_start:
            return_score = 0;
            if (m_current_card_count >= 2)
                return e_call_result::e_call_error;

            shuffle();
            complete();

            m_next_action_player_id = get_player_id_max_card(false);
        return e_call_result::e_call_next_card;

        default:
            if (player_id != m_next_action_player_id || call_score < 0)
                return e_call_result::e_call_error;
            else
                return do_call(show_hand_call_data::ptr(new show_hand_call_data(player_id, call_type, call_score)), return_score);
    }
}

int show_hand_game::do_call(show_hand_call_data::ptr shcd, GOLD_TYPE &return_score) {
    show_hand_call_data::ptr last_shcd(nullptr);
    if (m_players_action.size() > 0)
        last_shcd = m_players_action.front();

    return_score = shcd->score;

    switch (shcd->call_type) {
        case e_call_type::e_call_follow:
            if (last_shcd && m_action_string.size() > 0)
                shcd->score = last_shcd->score;
                return_score = shcd->score;
            m_action_string += OPERATION_FOLLOW;
        break;

        case e_call_type::e_call_abandon:
            return_score = 0;
            m_action_string += OPERATION_ABANDON;
        break;

        case e_call_type::e_call_add:
            if (last_shcd)
                if (last_shcd->call_type == e_call_type::e_call_add) {
                    show_hand_call_data::ptr add_shcd = show_hand_call_data::ptr(new show_hand_call_data(shcd->player_id, e_call_type::e_call_add, last_shcd->score));
                    m_players_action.push_front(add_shcd);
                }
            m_action_string += OPERATION_ADD;
        break;

        case e_call_type::e_call_showhand:
            if (m_current_card_count < 3)
                return e_call_result::e_call_error;
            m_action_string += OPERATION_SHOWHAND;
        break;

        case e_call_type::e_call_timeout_abandon:
            m_action_string += OPERATION_TIME_OUT_ABANDON;
        break;

        default:
            m_action_string += OPERATION_ERROR;
    }

    m_players_action.push_front(shcd);

    switch(next_action()) {
        case e_call_result::e_call_next_step:
            next_step();
        return e_call_result::e_call_next_step;

        case e_call_result::e_call_next_card:
            if (m_current_card_count < cards::card_count) {
                next_card();
                return e_call_result::e_call_next_card;

            } else {
                next_around();
                return e_call_result::e_call_next_around;
            }

        case e_call_result::e_call_next_around:
            next_around();
        return e_call_result::e_call_next_around;

        default:
            if (!m_action_string.empty())
                m_action_string.pop_back();

            if (!m_players_action.empty())
                m_players_action.pop_front();

        return e_call_result::e_call_error;
    }
}

int32_t show_hand_game::get_next_action_player_id() {
    return m_next_action_player_id; 
}

void show_hand_game::next_step() {
    show_hand_call_data::ptr shcd = m_players_action.front();
    if (shcd)
        m_next_action_player_id = shcd->player_id == m_player1_id ? m_player2_id : m_player1_id;

    else
        m_next_action_player_id = get_player_id_max_card();
}

void show_hand_game::next_card() {
    ++m_current_card_count;

    m_next_action_player_id = get_player_id_max_card();

    m_action_string = "";
}

void show_hand_game::next_around() {
    m_next_action_player_id = 0;

    show_hand_call_data::ptr shcd = m_players_action.front();
    if (!shcd)
        return ;

    if (!m_show_hand_result)
        m_show_hand_result = show_hand_result::ptr(new show_hand_result);

    if (e_call_type::e_call_timeout_abandon == shcd->call_type || e_call_type::e_call_abandon == shcd->call_type) {
        m_show_hand_result->is_show_card = false;
        m_show_hand_result->loss_player_id = shcd->player_id;
        m_show_hand_result->win_player_id = shcd->player_id == m_player1_id ? m_player2_id : m_player1_id;

    } else {
        m_show_hand_result->is_show_card = true;

        m_show_hand_result->win_player_id = get_player_id_max_card(true);
        m_show_hand_result->loss_player_id = m_show_hand_result->win_player_id == m_player1_id ? m_player2_id: m_player1_id;

        if (m_show_hand_result->is_show_card) {
            m_show_hand_result->win_player_cards = get_player_card(m_show_hand_result->win_player_id);
            m_show_hand_result->loss_player_cards = get_player_card(m_show_hand_result->loss_player_id);
        }
    }

    m_show_hand_result->win_player_score = m_bring_in_score;
    m_show_hand_result->loss_player_score = m_bring_in_score;

    for (players_action_container::iterator it = m_players_action.begin(); it != m_players_action.end(); ++it) {
        if ((*it)->player_id == m_show_hand_result->win_player_id) {
            m_show_hand_result->win_player_score += (*it)->score;

        } else {
            m_show_hand_result->loss_player_score += (*it)->score;
        }
    }
}

std::string show_hand_game::get_player_card(int32_t player_id) {
    player_card_container::iterator it_player_card = m_players_cards.find(player_id);
    if (it_player_card == m_players_cards.end())
        return "";

    card_container::iterator it_begin_card = it_player_card->second.begin(); 
    card_container::iterator it_end_card  = it_player_card->second.end();

    std::stringstream ss("");
    int cc = cards::card_zero;
    while (it_begin_card != it_end_card && cc < m_current_card_count) {
        ss << it_begin_card->second;

        ++it_begin_card;
        ++cc;

        if (it_begin_card != it_end_card && cc < m_current_card_count)
            ss << ",";
    }

    return ss.str();
}

int show_hand_game::next_action() {
    for (action_container::iterator it = m_action_set.begin(); it != m_action_set.end(); ++it) {
        if (is_sub_action(m_action_string, it->first)) {
            return e_call_result::e_call_next_step;

        } else if (is_action(m_action_string, it->first)) {
            return it->second;
        }
    }

    return e_call_result::e_call_error;
}

bool show_hand_game::is_sub_action(const std::string &sub, const std::string &str) {
    size_t sub_size = sub.size();
    size_t str_size = str.size();

    if (sub_size >= str_size)
        return false;

    int index = 0;
    while (index  < sub_size) {
        if (sub.at(index) != str.at(index))
            break;
        ++index;
    }

    if (index == sub_size)
        return true;

    return false;
}

bool show_hand_game::is_action(const std::string &lh, const std::string &rh) {
    return lh == rh;
}

void show_hand_game::load_action() {

    m_action_set.clear();

    const ShowHand_CallStep::CallStepDataContainer &list = ShowHand_CallStep::GetSingleton()->GetMapData();

    for each (ShowHand_CallStep::CallStepDataContainer::value_type var in list)
        m_action_set.push_back(std::make_pair(var.second.mCallStep, var.second.mCallResult));
}

std::string show_hand_game::get_current_card(int32_t player_id, bool all) {

    std::stringstream ss("");

    for (player_card_container::iterator it = m_players_cards.begin(); it != m_players_cards.end();) {
        ss << it->first;
        card_container::iterator it_score = it->second.begin();
        int cc = (int)cards::card_zero;
        for (; it_score != it->second.end() && cc < m_current_card_count; ++it_score, ++cc) {

            ss << ",";
            if (player_id != it->first && cc == cards::card_zero && !all)
                ss <<  0;
            else
                ss << it->second[cc];
        }

        ++it;

        if (it != m_players_cards.end())
            ss << "|";
    }

    return ss.str();
}

std::string show_hand_game::get_current_score(GOLD_TYPE &total_score) {
    std::stringstream ss("");

    GOLD_TYPE player_1_score = m_bring_in_score;
    GOLD_TYPE player_2_score = m_bring_in_score;

    for (players_action_container::iterator it = m_players_action.begin(); it != m_players_action.end(); ++it) {
        if ((*it)->player_id == m_player1_id)
            player_1_score += (*it)->score;
        else
            player_2_score += (*it)->score;
    }

    total_score = player_1_score + player_2_score;

    ss << m_player1_id << "," << player_1_score << "|" << m_player2_id << "," << player_2_score;

    return ss.str();
}

void show_hand_game::set_bring_in_score(GOLD_TYPE g) {
    m_bring_in_score = g;
}

GOLD_TYPE show_hand_game::get_bring_in_score() {
    return m_bring_in_score;
}

void show_hand_game::complete() {
    for (int cc = card_zero; cc < card_count; ++cc) {
        m_players_cards[m_player1_id][cc] = m_cards[cc];
        m_players_cards[m_player2_id][cc] = m_cards[cc + card_count];
    }

    m_current_card_count = 2;
}

void show_hand_game::change(int pos) {
    int t = m_cards[0];
    m_cards[0] = m_cards[pos];
    m_cards[pos] = t;
}

void show_hand_game::shuffle() {
    int tt = total_count * total_count;
    srand(time(nullptr));
    for (int t = 0; t < tt; ++t) {
        int pos = rand() % cards::total_count;
        change(pos);
    }
}

int32_t show_hand_game::get_player_id_max_card(bool is_next_around) {
    card_container player1_id_card(m_players_cards.find(m_player1_id)->second);
    card_container player2_id_card(m_players_cards.find(m_player2_id)->second);

    int ct_player_1 = get_card_type(player1_id_card, is_next_around);
    int ct_player_2 = get_card_type(player2_id_card, is_next_around);

    if (!is_next_around && m_current_card_count >= cards::card_count) {//5
        if (card_type::cards_three_of_a_kind == ct_player_1 &&
                (card_type::cards_flush == ct_player_2 || card_type::cards_straight == ct_player_2))
            return m_player1_id;

        else if (card_type::cards_three_of_a_kind == ct_player_2 &&
                (card_type::cards_flush == ct_player_1 || card_type::cards_straight == ct_player_1))
            return m_player2_id;

        else {
            if (ct_player_1 != ct_player_2)
                return ct_player_1 > ct_player_2 ? m_player1_id : m_player2_id;

            return get_the_max_in_same_type(player1_id_card, player2_id_card, ct_player_1, is_next_around);
        }            

    } else if (!is_next_around && m_current_card_count >= (cards::card_count - 1)) {//4
        if (card_type::cards_three_of_a_kind == ct_player_1 && card_type::cards_three_of_a_kind != ct_player_2)
            return m_player1_id;

        else if (card_type::cards_three_of_a_kind != ct_player_2 && card_type::cards_three_of_a_kind == ct_player_2)
            return m_player2_id;

        else if (card_type::cards_three_of_a_kind == ct_player_1 && card_type::cards_three_of_a_kind == ct_player_2)
            return get_the_max_in_same_type(player1_id_card, player2_id_card, card_type::cards_three_of_a_kind , is_next_around);

        else if (ct_player_1 == card_type::cards_one_pair && ct_player_2 != card_type::cards_one_pair)
            return m_player1_id;

        else if (ct_player_2 == card_type::cards_one_pair && card_type::cards_one_pair != ct_player_1)
            return m_player2_id;

        else if (card_type::cards_one_pair == ct_player_1 && card_type::cards_one_pair == ct_player_2)
            return get_the_max_in_same_type(player1_id_card, player2_id_card, card_type::cards_one_pair, is_next_around);

        else
            return get_the_max_in_same_type(player1_id_card, player2_id_card, card_type::cards_zilch, is_next_around);

    } else if (!is_next_around && m_current_card_count >= (cards::card_count - 2)) {//3
        if (ct_player_1 == card_type::cards_one_pair && ct_player_2 != card_type::cards_one_pair)
            return m_player1_id;

        else if (ct_player_2 == card_type::cards_one_pair && card_type::cards_one_pair != ct_player_1)
            return m_player2_id;

        else if (card_type::cards_one_pair == ct_player_1 && card_type::cards_one_pair == ct_player_2)
            return get_the_max_in_same_type(player1_id_card, player2_id_card, card_type::cards_one_pair, is_next_around);

        else
            return get_the_max_in_same_type(player1_id_card, player2_id_card, card_type::cards_zilch, is_next_around);

    } else if (!is_next_around && m_current_card_count >= (cards::card_count - 3)) {//2
        if ((player1_id_card[1] & 0x0F) > (player2_id_card[1] & 0x0F))
            return m_player1_id;
        else if ((player1_id_card[1] & 0x0F) < (player2_id_card[1] & 0x0F))
            return m_player2_id;
        else if ((player1_id_card[1] & 0xF0) > (player2_id_card[1] & 0xF0))
            return m_player1_id;
        else
            return m_player2_id;

    } else {
        //先判断牌类型，返回类型较大的牌
        if (ct_player_1 != ct_player_2 && m_current_card_count > 2)
            return ct_player_1 > ct_player_2 ? m_player1_id : m_player2_id;

        //如果两个牌是同类型的牌， 则在其中比较大小。
        return get_the_max_in_same_type(player1_id_card, player2_id_card, ct_player_1, is_next_around);
    }
}

int show_hand_game::get_card_type(card_container &cc, bool is_next_around) {

    if (m_current_card_count <= 2)
        return card_type::cards_zilch;

    //note the m_current_card_count
    card_container::iterator cc_beg = cc.begin();
    int ccc = 0;

    if (!is_next_around && cc_beg != cc.end()) {
        ++cc_beg;
        ++ccc;
    }

    int ct = -1;
    if (cc_beg != cc.end())
        ct = (cc_beg->second) & 0xF0;

    std::map<int, int> statistic;
    //判断是否同花
    bool is_cards_flush = true;
    for (; cc_beg != cc.end() && ccc < m_current_card_count; ++cc_beg, ++ccc) {
        if (ct != ((cc_beg->second) & 0xF0))
            is_cards_flush = false;

        statistic[(cc_beg->second) & 0x0F]++;
    }

    //判断是否顺子
    std::vector<std::pair<int, int>> temp;
    cc_beg = cc.begin();
    ccc = 0;
    if (!is_next_around && cc_beg != cc.end()) {
        ++cc_beg;
        ++ccc;
    }

    for (; cc_beg != cc.end() && ccc < m_current_card_count; ++cc_beg, ++ccc)
        temp.push_back(std::make_pair(cc_beg->first, (cc_beg->second) & 0x0F));

    std::sort(temp.begin(), temp.end(), compare_in_pair());

    std::vector<std::pair<int, int>>::iterator it_pre = temp.begin();
    std::vector<std::pair<int, int>>::iterator it_aft = it_pre;
    if (it_pre != temp.end())
        ++it_aft;

    bool is_cards_straight = true;
    for (; it_pre != temp.end() && it_aft != temp.end(); ++it_pre, ++it_aft) {
        int distance = it_pre->second - it_aft->second;
        if (distance != (1)) {
            is_cards_straight = false;
            break;
        }
    }

    if (!is_cards_straight && is_next_around) {
        for (it_pre = temp.begin(); it_pre != temp.end(); ++it_pre)
            if (it_pre->second == cards::diamond_one)
                it_pre->second = cards::magic_card;

        std::sort(temp.begin(), temp.end(), compare_in_pair());

        is_cards_straight = true;
        it_pre = temp.begin();
        it_aft = it_pre;
        if (it_pre != temp.end())
            ++it_aft;

        for (; it_pre != temp.end() && it_aft != temp.end(); ++it_pre, ++it_aft) {
            int distance = it_pre->second - it_aft->second;
            if (distance != (1)) {
                is_cards_straight = false;
                break;
            }
        }
    }

    //判断是否同花顺
    if (is_cards_flush && is_cards_straight)
        return card_type::cards_straight_flush;

    //相同数字牌
    int four_of_a_kind = 0;
    int three_of_a_kind = 0;
    int two_of_a_kind = 0;

    for (std::map<int, int>::iterator it = statistic.begin(); it != statistic.end(); ++it) {
        switch (it->second) {
            case 4:
                ++four_of_a_kind;
            break;

            case 3:
                ++three_of_a_kind;
            break;

            case 2:
                ++two_of_a_kind;
            break;
        }
    }

    //是否为四条
    if (four_of_a_kind > 0)
        return card_type::cards_four_of_a_kind;

    //是否为满堂红
    if (three_of_a_kind > 0 && two_of_a_kind)
        return card_type::cards_full_house;

    //是否为同花
    if (is_cards_flush)
        return card_type::cards_flush;

    //是否为顺子
    if (is_cards_straight)
        return card_type::cards_straight;

    //是否为三条
    if (three_of_a_kind > 0)
        return card_type::cards_three_of_a_kind;

    //是否为二对
    if (two_of_a_kind > 1)
        return card_type::cards_two_pairs;

    if (two_of_a_kind > 0)
        return card_type::cards_one_pair;

    return card_type::cards_zilch;
}

int32_t show_hand_game::get_the_max_in_same_type(card_container &player1_card, card_container &player2_card, int card_type, bool is_next_around) {
    //note the m_current_card_count
    if (player1_card.empty() || player2_card.empty())
        return -1;

    card_container::iterator p1_it = player1_card.begin();
    int cc = 0;
    if (!is_next_around) {
        ++cc;
        ++p1_it;
    }

    std::vector<std::pair<int, int>> temp1;

    //去掉花色
    for (; p1_it != player1_card.end() && cc < m_current_card_count; ++p1_it, ++cc) {
        temp1.push_back(std::make_pair(p1_it->first, ((p1_it->second) & 0x0F)));
    }
    //排序
    std::sort(temp1.begin(), temp1.end(), compare_in_pair());

    card_container::iterator p2_it = player2_card.begin();
    int cc1 = 0;
    if (!is_next_around) {
        ++cc1;
        ++p2_it;
    }
    std::vector<std::pair<int, int>> temp2;
    //去掉花色
    for (; p2_it != player2_card.end() && cc1 < m_current_card_count; ++p2_it, ++cc1) {
        temp2.push_back(std::make_pair(p2_it->first, ((p2_it->second) & 0x0F)));
    }
    //排序
    std::sort(temp2.begin(), temp2.end(), compare_in_pair());

    /*
    player2_card.clear();

    for (int index = 0; index < temp2.size(); index++)
        player2_card.insert(std::make_pair(index, temp2[index].second));
    */

    if (card_type::cards_straight_flush == card_type) {
        //同花顺比较第二张， 避免magic card影响

        if (temp1.begin()->second > temp2.begin()->second)
            return m_player1_id;

        else if (temp1.begin()->second < temp2.begin()->second)
            return m_player2_id;

        else if (((player1_card.begin()->second) & 0xF0) > ((player1_card.begin()->second) & 0xF0))
            return m_player1_id;

        else
            return m_player2_id;

    } else if (card_type::cards_four_of_a_kind == card_type) {
        int card1 = get_the_four_of_a_kind_card(player1_card);
        int card2 = get_the_four_of_a_kind_card(player2_card);
        if (card1 > card2)

            return m_player1_id;
        else
            return m_player2_id;

    } else if (card_type::cards_two_pairs == card_type) {
        int player_1_card_big = 0;
        int player_1_card_small = 0;
        int player_2_card_big = 0;
        int player_2_card_small = 0;

        get_the_two_pairs_card(temp1, player_1_card_big, player_1_card_small);
        get_the_two_pairs_card(temp2, player_2_card_big, player_2_card_small);

        if (player_1_card_big > player_2_card_big)
            return m_player1_id;

        else if (player_1_card_big < player_2_card_big)
            return m_player2_id;

        else if (player_1_card_small > player_2_card_small)
            return m_player1_id;

        else if (player_1_card_small < player_2_card_small)
            return m_player2_id;

        else if (get_the_card_shcd(player1_card, player_1_card_big) > get_the_card_shcd(player2_card, player_2_card_big))
            return m_player1_id;

        else
            return m_player2_id;

    } else if (card_type::cards_three_of_a_kind == card_type) {
        if (get_the_three_of_a_kind(player1_card) > get_the_three_of_a_kind(player2_card))
            return m_player1_id;

        else
            return m_player2_id;

    } else if (card_type::cards_full_house == card_type) {
        int player_1_card_three = 0;
        int player_2_card_three = 0;
        get_the_full_house_card(player1_card, player_1_card_three, player_1_card_three);
        get_the_full_house_card(player2_card, player_2_card_three, player_2_card_three);
        if (player_1_card_three >player_2_card_three)
            return m_player1_id;

        else
            return m_player2_id;

    } else if (card_type::cards_one_pair == card_type) {
        int card_p1 = get_the_one_pair_card(player1_card);
        int card_p2 = get_the_one_pair_card(player2_card);

        if (card_p1 > card_p2)
            return m_player1_id;

        else if (card_p2 < card_p1)
            return m_player2_id;

        else {
            vector_pair_container::iterator temp1_it = temp1.begin();
            vector_pair_container::iterator temp2_it = temp2.begin();

            for (;temp1_it != temp1.end() && temp2_it != temp2.end(); ++temp1_it, ++temp2_it)
                if (temp1_it->second > temp2_it->second)
                    return m_player1_id;

                else if (temp1_it->second < temp2_it->second)
                    return m_player2_id;

            return 0;

        }

    } else if (card_type::cards_zilch == card_type) {
        //散牌先比较牌的数值大小，若无返回，再比较花色决定了牌的大小
        vector_pair_container::iterator temp1_it = temp1.begin();
        vector_pair_container::iterator temp2_it = temp2.begin();
        for (;temp1_it != temp1.end() && temp2_it != temp2.end(); ++temp1_it, ++temp2_it)
            if (temp1_it->second > temp2_it->second)
                return m_player1_id;
            else if (temp1_it->second < temp2_it->second)
                return m_player2_id;
            else
                continue;
        //两张牌的大小是一样的
        if (get_the_card_shcd(player1_card,temp1[0].second) > get_the_card_shcd(player2_card, temp2[0].second))
            return m_player1_id;
        else
            return m_player2_id;

    } else if (card_type == card_type::cards_straight) {
        //顺子比较第二张牌， 跳过magic card

        vector_pair_container::iterator temp1_it = temp1.begin();
        ++temp1_it;
        vector_pair_container::iterator temp2_it = temp2.begin();
        ++temp2_it;

        if (temp1_it->second > temp2_it->second)
            return m_player1_id;

        else if (temp1_it->second < temp2_it->second)
            return m_player2_id;

        else for (p1_it = player1_card.begin(), p2_it = player2_card.begin();
                    p1_it != player1_card.end() && p2_it != player2_card.end(); ++p1_it, ++p2_it)

            if (((p1_it->second) & 0x0F) == temp1_it->second)
                cc = ((p1_it->second) & 0xF0);

            else if (((p2_it->second) & 0x0F) == temp2_it->second)

                cc1 = (p2_it->second) & 0xF0;

        if (cc > cc1)
            return m_player1_id;
        else if (cc1 > cc)
            return m_player2_id;
        else
            return 0;

    //} else if (card_type::cards_flush == card_type) {
    //    return get_the_cards_zilch(player1_card, player2_card);
    } else {
        if (temp1.begin()->second > temp2.begin()->second)
            return m_player1_id;

        else if (temp1.begin()->second < temp2.begin()->second)
            return m_player2_id;

        else for (p1_it = player1_card.begin(), p2_it = player2_card.begin();
                    p1_it != player1_card.end() && p2_it != player2_card.end(); ++p1_it, ++p2_it)

            if (((p1_it->second) & 0x0F) == temp1.begin()->second)
                cc = ((p1_it->second) & 0xF0);

            else if (((p2_it->second) & 0x0F) == temp2.begin()->second)

                cc1 = (p2_it->second) & 0xF0;
        
        if (cc > cc1)
            return m_player1_id;
        else if (cc1 > cc)
            return m_player2_id;
        else
            return 0;
    }
}

int32_t show_hand_game::get_the_cards_zilch(card_container &player1_card, card_container  &player2_card) {/**/

    card_container::iterator p1_it = player1_card.begin();
    int cc = 0;
    std::vector<std::pair<int, int>> temp;

    //去掉花色
    for (; p1_it != player1_card.end() && cc < m_current_card_count; ++p1_it, ++cc) {
        temp.push_back(std::make_pair(p1_it->first, ((p1_it->second) & 0x0F)));
    }

    //排序
    std::sort(temp.begin(), temp.end(), compare_in_pair());
    player1_card.clear();

    for (int index = 0; index < temp.size(); index++)
        player1_card.insert(std::make_pair(index, temp[index].second));


    card_container::iterator p2_it = player2_card.begin();
    cc = 0;
    temp.clear();
    //去掉花色
    for (; p2_it != player2_card.end() && cc < m_current_card_count; ++p2_it, ++cc) {
        temp.push_back(std::make_pair(p2_it->first, ((p2_it->second) & 0x0F)));
    }

    //排序
    std::sort(temp.begin(), temp.end(), compare_in_pair());
    player2_card.clear();

    for (int index = 0; index < temp.size(); index++)
        player2_card.insert(std::make_pair(index, temp[index].second));
    //比较大小
    p1_it = player1_card.begin();
    p2_it = player2_card.begin();

    for (;p1_it != player1_card.end() && p2_it != player2_card.end(); ++p1_it, ++p2_it) {
        if (p1_it->second > p2_it->second)
            return m_player1_id;
        else if (p1_it->second < p2_it->second)
            return m_player2_id;
    }

    return 0;
}

int32_t show_hand_game::get_the_cards_straight(card_container &player1_card, card_container &player2_card) {
    if (((player1_card.begin()->second) & 0x0F) > ((player2_card.begin()->second) & 0x0F))
        return m_player1_id;
    else
        return m_player2_id;
}

int show_hand_game::get_the_one_pair_card(card_container &player_card) {
    for (card_container::iterator it = player_card.begin(); it != player_card.end(); ++it) {
        for (card_container::iterator it_inner = it; it_inner != player_card.end(); ++it_inner) {
            if (((it->second) & 0x0F) == ((it_inner->second) & 0x0F) && it->first != it_inner->first)
                return ((it->second) & 0x0F);
        }
    }

    return 0;
}

void show_hand_game::get_the_full_house_card(card_container &player_card_in, int &card_three_out, int &card_two_out) {
    std::map<int, int> ccc;
    for (card_container::iterator it = player_card_in.begin(); it != player_card_in.end(); ++it) {
        ccc[(it->second) & 0x0F]++;
    }

    for (std::map<int, int>::iterator it = ccc.begin(); it != ccc.end(); ++it) {
        if (it->second == 3)
            card_three_out = it->first;
    }
}

int show_hand_game::get_the_three_of_a_kind(card_container &player_card) {
    std::map<int, int> card_map;
    for (card_container::iterator it = player_card.begin(); it != player_card.end(); ++it) {
        card_map[((it->second) & 0x0F)]++;
    }
    for (std::map<int, int>::iterator card_map_it = card_map.begin(); card_map_it != card_map.end(); ++ card_map_it)
        if (card_map_it->second == 3)
            return card_map_it->first;

    return 0;
}

//获取二对类型中的 二数字
void show_hand_game::get_the_two_pairs_card(vector_pair_container &player_card, int &card_big_out, int &card_small_out) {
    std::vector<int> cars;

    card_container statistic;
    for (vector_pair_container::iterator it = player_card.begin(); it != player_card.end(); ++it) {
        statistic[it->second]++;
    }

    for (card_container::iterator it_cc = statistic.begin(); it_cc != statistic.end(); ++it_cc)
        if (it_cc->second > 1)
            cars.push_back(it_cc->first);

    if (cars.size() > 1) {
        card_big_out = cars[0] > cars[1] ? cars[0] : cars[1];
        card_small_out = cars[0] < cars[1] ? cars[0] : cars[1];
    }
}

//获取花色 s h c d
int show_hand_game::get_the_card_shcd(card_container &player_card, int card_in) {
    for (card_container::iterator it = player_card.begin(); it != player_card.end(); ++it) {
        if (((it->second) & 0x0F) == card_in)
            return ((it->second) & 0xF0);
    }

    return -1;
}

int show_hand_game::get_the_four_of_a_kind_card(card_container &player_card) {
    int card1 = 0, card2 = 0;
    card_container::iterator it = player_card.begin();
    int flag = 1;
    for (; it != player_card.end(); ++it, flag++) {
        if (flag == 1)
            card1 = ((it->second) & 0x0F);
        else if (flag == 2)
            if (card1 == ((it->second) & 0x0F))
                return card1;
            else
                card2 = ((it->second) & 0x0F);
        else if (flag == 3)
            if (card1 == ((it->second) & 0x0F))
                return card1;
            else
                return card2;
        else
            return 0;
    }

    return 0;
}