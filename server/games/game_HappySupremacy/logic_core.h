#pragma once

#include "proc_game_happysupremacy_protocol.h"

struct Card
{
public:
	BYTE point;
	e_card_flower flower;
	Card()
	{

	}

	Card(const e_card_flower type,const BYTE point)
	{
		this->point=point;
		this->flower=type;
	}

	BYTE ToPoint() const
	{
		if(flower==e_flower_spade && point==1)
		{
			return 6;
		}else
		{
			return point;
		}
	}
};
struct CardInfo
{
	e_card_owner owner;
	Card card;
};

struct CombineCardInfo
{
	e_card_owner owner;
	std::array<Card,2> cards;
};

struct CombinePointInfo
{
	e_card_owner owner;
	BYTE combineCards_Id;
	BYTE combineCard_point;
	BYTE maxsingleCard_point;

	CombinePointInfo()
	{
		combineCards_Id=0;
		combineCard_point=0;
		maxsingleCard_point=0;
	}
};

class logic_core
{
public:
	logic_core();
	void send_card();
	void send_cheat_card(bool or_SHOUFEN);//如果庄家是机器人的话
	void send_cheat_card(std::vector<e_bet_type> win_type_list);//欺骗牌
	std::vector<CardInfo>& get_sort_player_card() {return m_sortcart_list;}
	std::map<e_bet_type,e_bet_result>& get_result_list(){return m_result_list;}
	std::map<e_card_owner,CombinePointInfo>& get_result_point_list(){return m_result_point_list;}
	BYTE get_remain_card_count(){return static_cast<BYTE>(m_cards.size());}
private:
	friend std::ostream& operator<<(std::ostream& stream, const logic_core& cardEngine);
	void init_card();
	void random_card();
	void take_open_card();  //翻牌
	void take_one_card(e_card_owner owner,uint16_t index);  //取牌
	void take_card();//取所有牌

	int card_campare_single(const CombineCardInfo& cd1,const CombineCardInfo& cd2);
	int card_campare(const CombineCardInfo& cd1,const CombineCardInfo& cd2);
	const Card& get_single_maxcard(const std::array<Card,2>& cd);
	BYTE get_single_card_point(const Card& cd);
	BYTE get_card_Point(const std::array<Card,2>& cd) const;//统一转换为比较的虚拟点数

	void set_result();
	void insert_to_sortcardlist(e_card_owner owner,Card card);
	void set_result_point();
	BYTE get_combineCard_Id(const std::array<Card,2>& cd) const;
	
	void send_GM_Cards();
private:
	std::vector<Card> m_cards;//全部的牌
	std::map<e_card_owner,CombineCardInfo> m_doorMap_card;
	std::vector<CardInfo> m_sortcart_list;	//9张牌(包括翻牌的那张)
	Card mOpenCard;//取牌前，翻牌确定谁先取牌
	//对门，顺门，倒门
	std::map<e_bet_type,e_bet_result> m_result_list;//下注结果
	std::map<e_card_owner,CombinePointInfo> m_result_point_list;//组合牌“点数”列表
};
