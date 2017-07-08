#pragma once
#include <memory>
#include <vector>
#include <array>
#include <cassert>
#include <algorithm>
#include <iostream>
#include <string>
#include <cstdlib>
#include <ctime>

/*
规则：
所有牌的点数花色的公式为：
8,9,10,11,12,13,14,分别代表：8,9,10,11,12,13,A
牌唯一点：牌原始点*100+花色点数（4:黑，3：红，2：梅，1：方）
且此牌点数越大，则牌唯一点也越大
*/
class cardmanager
{
private:
	std::vector<int> p_left_cards;//左牌
	std::vector<int> p_right_cards;//右牌

	std::vector<int> m_cards;//所有的牌 
	std::vector<bool> firstbet_list;
	bool orleftwin;

	std::vector<bool> left_win_rate;
	std::vector<bool> right_win_rate;
public:
	cardmanager();
	void sendcard();
	void send_cheat_card(bool left_win);//是否让左边赢
	void gm_exchange_card();
	std::vector<bool>& get_who_first_bet_list();
	std::vector<bool>& get_left_win_rate();//得到左边玩家赢得概率
	std::vector<bool>& get_right_win_rate();//得到右边玩家赢得概率
	std::vector<int>& get_left_cards();
	std::vector<int>& get_right_cards();
	bool get_left_result();
	int get_cards_type(const std::vector<int>& cards);
private:
	void init_cards();
	int compare_cards();
	void set_result();
	void set_whofirstbetlist();
	void set_whowinratelist();

	int get_whofirstbet_point(int currentCardIndex, const std::vector<int>& cards);
	int get_win_rate_point(int currentCardIndex, const std::vector<int>& cards);
	int get_cards_point(const std::vector<int>& cards);

	bool Is_TONGHUASHUN(const std::vector<int>& sort_cards);//同花顺
	bool Is_TIEZHI(const std::vector<int>& sort_cards);//铁支
	bool Is_HULU(const std::vector<int>& sort_cards);//葫芦
	bool Is_TONGHUA(const std::vector<int>& sort_cards);//同花
	bool Is_SHUNZI(const std::vector<int>& sort_cards);//顺子
	bool Is_SANTIAO(const std::vector<int>& sort_cards);//三条
	bool Is_LIANGDUI(const std::vector<int>& sort_cards);//两对
	bool Is_DUIZI(const std::vector<int>& sort_cards);//对子

	int get_point_TONGHUASHUN(const std::vector<int>& sort_cards);//同花顺
	int get_point_TIEZHI(const std::vector<int>& sort_cards);//铁枝
	int get_point_HULU(const std::vector<int>& sort_cards);//葫芦
	int get_point_TONGHUA(const std::vector<int>& sort_cards);//同花
	int get_point_SHUNZI(const std::vector<int>& sort_cards);//顺子
	int get_point_SANTIAO(const std::vector<int>& sort_cards);//三条
	int get_point_LIANGDUI(const std::vector<int>& sort_cards);//两对
	int get_point_DUIZI(const std::vector<int>& sort_cards);//对子

	int get_point_SANPAI(const std::vector<int>& sort_cards);//散牌

	void send_GM_Cards();

	friend std::ostream& operator<<(std::ostream& stream, const cardmanager& cm);
};