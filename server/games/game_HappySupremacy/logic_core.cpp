#include "stdafx.h"
#include "logic_core.h"
#include "logic_def.h"
#include <climits>
#include <algorithm>
#include <string>
#include <cstdlib>
#include "HappySupremacy_BaseCFG.h"
#include "HappySupremacy_MultiLanguageCFG.h"
#include <enable_random.h>

#define MINCOMBINEPOINT 100
logic_core::logic_core()
{
	init_card();
}

void logic_core::init_card()
{
	m_cards.clear();
	std::vector<Card>(m_cards).swap(m_cards);  

	const std::array<BYTE,7> mlist_spade={1,3,4,6,7,8,10};//黑
	const std::array<BYTE,10> mlist_heart={2,4,5,6,7,8,9,10,11,12};//红
	const std::array<BYTE,5> mlist_club={4,6,7,8,10};//，梅
	const std::array<BYTE,10> mlist_diamond={2,4,5,6,7,8,9,10,11,12};//方
	
	for(auto& a : mlist_spade)
	{
		Card mCard(e_flower_spade,a);
		m_cards.push_back(mCard);
	}
	for(auto& a : mlist_heart)
	{
		Card mCard(e_flower_heart,a);
		m_cards.push_back(mCard);
	}
	for(auto& a : mlist_club)
	{
		Card mCard(e_flower_club,a);
		m_cards.push_back(mCard);
	}
	for(auto& a : mlist_diamond)
	{
		Card mCard(e_flower_diamond,a);
		m_cards.push_back(mCard);
	}
	random_card();
}

void logic_core::send_cheat_card(bool or_SHOUFEN)//收分放分
{
	m_doorMap_card.clear();
	init_card();
	take_card();
	//send_GM_Cards();
	std::vector<CombineCardInfo> sort_list;
	for(auto& a : m_doorMap_card)
	{
		sort_list.push_back(a.second);
	}
	sort_list.resize(4);

	std::sort(sort_list.begin(),sort_list.end(),[&](CombineCardInfo x,CombineCardInfo y)
	{
		if(card_campare(x,y)==card_campare(y,x))
		{
			return false;
		}else
		{
			if(card_campare(x,y)<0)
			{
				return true;
			}else
			{
				return false;
			}
		}
	});

	assert(m_doorMap_card.size()==4);
	if(or_SHOUFEN)
	{
		for(auto& a : m_doorMap_card)
		{
			if(a.first==sort_list[3].owner)
			{
				m_doorMap_card[a.first]=m_doorMap_card[e_owner_banker];
				break;
			}
		}
		m_doorMap_card[e_owner_banker]=sort_list[3];
	}else
	{
		int index=global_random::instance().rand_int(0, 2);
		for(auto& a : m_doorMap_card)
		{
			if(a.first==sort_list[index].owner)
			{
				m_doorMap_card[a.first]=m_doorMap_card[e_owner_banker];
				break;
			}
		}
		m_doorMap_card[e_owner_banker]=sort_list[index];
	}
	set_result();
}

void logic_core::send_cheat_card(std::vector<e_bet_type> win_type_list)//GM 控制
{
	init_card();
	take_card();

	std::vector<CombineCardInfo> sort_list;
	for(auto& a : m_doorMap_card)
	{
		sort_list.push_back(a.second);
	}

	std::sort(sort_list.begin(),sort_list.end(),[&](CombineCardInfo x,CombineCardInfo y)
	{
		if(card_campare(x,y)==card_campare(y,x))
		{
			return false;
		}else
		{
			if(card_campare(x,y)<0)
			{
				return true;
			}else
			{
				return false;
			}
		}
	});

	bool or_forwarddoor_win=false;
	bool or_oppositedoor_win=false;
	bool or_reversedoor_win=false;
	for(int i=0;i<win_type_list.size();i++)
	{
		if(win_type_list[i]==e_bettype_forward_opposite_door)
		{
			or_forwarddoor_win=true;
			or_oppositedoor_win=true;
		}else if(win_type_list[i]==e_bettype_forward_reverse_door)
		{
			or_reversedoor_win=true;
			or_forwarddoor_win=true;
		}
		else if(win_type_list[i]==e_bettype_reverse_opposite_door)
		{
			or_reversedoor_win=true;
			or_oppositedoor_win=true;
		}
		else if(win_type_list[i]==e_bettype_forwarddoor)
		{
			or_forwarddoor_win=true;
		}else if(win_type_list[i]==e_bettype_oppositedoor)
		{
			or_oppositedoor_win=true;
		}else if(win_type_list[i]==e_bettype_reversedoor)
		{
			or_reversedoor_win=true;
		}
	}

	int banker_index=3;
	if(or_forwarddoor_win)
	{
		banker_index--;
	}
	if(or_oppositedoor_win)
	{
		banker_index--;
	}
	if(or_reversedoor_win)
	{
		banker_index--;
	}
	for(auto& a : m_doorMap_card)
	{
		if(a.first==sort_list[banker_index].owner)
		{
			m_doorMap_card[a.first]=m_doorMap_card[e_owner_banker];
			break;
		}
	}
	m_doorMap_card[e_owner_banker]=sort_list[banker_index];

	int index=3;
	if(or_forwarddoor_win)
	{
		for(auto& a : m_doorMap_card)
		{
			if(a.first==sort_list[index].owner)
			{
				m_doorMap_card[a.first]=m_doorMap_card[e_owner_forwarddoor];
				index--;
				break;
			}
		}
		m_doorMap_card[e_owner_forwarddoor]=sort_list[index];
	}

	if(or_oppositedoor_win)
	{
		for(auto& a : m_doorMap_card)
		{
			if(a.first==sort_list[index].owner)
			{
				m_doorMap_card[a.first]=m_doorMap_card[e_owner_oppositedoor];
				index--;
				break;
			}
		}
		m_doorMap_card[e_owner_oppositedoor]=sort_list[index];
	}

	if(or_reversedoor_win)
	{
		for(auto& a : m_doorMap_card)
		{
			if(a.first==sort_list[index].owner)
			{
				m_doorMap_card[a.first]=m_doorMap_card[e_owner_reversedoor];
				index--;
				break;
			}
		}
		m_doorMap_card[e_owner_reversedoor]=sort_list[index];
	}

	set_result();
}

void logic_core::send_card()
{
	init_card();
	take_card();
	CombineCardInfo m_banker_cards=m_doorMap_card[e_owner_banker];

	bool or_forwarddoor=false;
	bool or_reversedoor=false;
	bool or_oppositedoor=false;
	for(auto& a : m_doorMap_card)
	{
		if(a.first!=e_owner_banker)
		{
			int com_result=card_campare(a.second,m_banker_cards);
			if(a.first==e_owner_forwarddoor && com_result>0)
			{
				or_forwarddoor=true;
			}else if(a.first==e_owner_reversedoor && com_result>0)
			{
				or_reversedoor=true;
			}else if(a.first==e_owner_oppositedoor && com_result>0)
			{
				or_oppositedoor=true;
			}	
		}
	}
	set_result();
}

void logic_core::set_result()
{
	CombineCardInfo m_banker_cards=m_doorMap_card[e_owner_banker];

	bool or_forwarddoor=false;
	bool or_reversedoor=false;
	bool or_oppositedoor=false;
	for(auto& a : m_doorMap_card)
	{
		if(a.first!=e_owner_banker)
		{
			int com_result=card_campare(a.second,m_banker_cards);
			if(a.first==e_owner_forwarddoor && com_result>0)
			{
				or_forwarddoor=true;
			}else if(a.first==e_owner_reversedoor && com_result>0)
			{
				or_reversedoor=true;
			}else if(a.first==e_owner_oppositedoor && com_result>0)
			{
				or_oppositedoor=true;
			}			
		}
	}

	if(or_forwarddoor==true)
	{
		m_result_list[e_bettype_forwarddoor]=e_betresult_win;
	}else
	{
		m_result_list[e_bettype_forwarddoor]=e_betresult_lose;
	}

	if(or_reversedoor==true)
	{
		m_result_list[e_bettype_reversedoor]=e_betresult_win;
	}else
	{
		m_result_list[e_bettype_reversedoor]=e_betresult_lose;
	}
	if(or_oppositedoor==true)
	{
		m_result_list[e_bettype_oppositedoor]=e_betresult_win;
	}else
	{
		m_result_list[e_bettype_oppositedoor]=e_betresult_lose;
	}

	if(or_forwarddoor==true && or_oppositedoor==true)// 顺对角
	{
		m_result_list[e_bettype_forward_opposite_door]=e_betresult_win;
	}else if(or_forwarddoor==false && or_oppositedoor==false)
	{
		m_result_list[e_bettype_forward_opposite_door]=e_betresult_lose;
	}else
	{
		m_result_list[e_bettype_forward_opposite_door]=e_betresult_nowin_nolose;
	}

	if(or_reversedoor==true && or_oppositedoor==true)//倒对角
	{
		m_result_list[e_bettype_reverse_opposite_door]=e_betresult_win;
	}else if(or_reversedoor==false && or_oppositedoor==false)
	{
		m_result_list[e_bettype_reverse_opposite_door]=e_betresult_lose;
	}else
	{
		m_result_list[e_bettype_reverse_opposite_door]=e_betresult_nowin_nolose;
	}

	if(or_forwarddoor==true && or_reversedoor==true) //横门
	{
		m_result_list[e_bettype_forward_reverse_door]=e_betresult_win;
	}else if(or_forwarddoor==false && or_reversedoor==false)
	{
		m_result_list[e_bettype_forward_reverse_door]=e_betresult_lose;
	}else
	{
		m_result_list[e_bettype_forward_reverse_door]=e_betresult_nowin_nolose;
	}

	set_result_point();
}

void logic_core::set_result_point()
{
	for(auto& cd1:m_doorMap_card)
	{
		BYTE point1=get_card_Point(cd1.second.cards);
		if(point1>MINCOMBINEPOINT)
		{
			m_result_point_list[cd1.first].combineCards_Id=get_combineCard_Id(cd1.second.cards);
		}else
		{
			m_result_point_list[cd1.first].combineCard_point=point1;
		}
	}
}

void logic_core::random_card()
{
	srand(time(NULL));
	std::random_shuffle(m_cards.begin(), m_cards.end(), [](int x) {return rand() % x; });//洗下牌
}

void logic_core::take_open_card()  //取牌
{
	mOpenCard = m_cards[m_cards.size()-1];
	m_cards.pop_back();
}

void logic_core::take_one_card(e_card_owner owner,uint16_t index)  //取牌
{
	m_doorMap_card[owner].owner=owner;
	m_doorMap_card[owner].cards[index]= m_cards[m_cards.size()-1];
	m_cards.pop_back();
}

void logic_core::insert_to_sortcardlist(const e_card_owner owner,const Card card)
{
	CardInfo ci;
	ci.owner=owner;
	ci.card=card;
	m_sortcart_list.push_back(ci);
}

void logic_core::take_card()//取牌全过程（这个是最正规的发牌规则，可惜不适合GM人员）
{
	m_doorMap_card.clear();
	m_sortcart_list.clear();
	m_result_list.clear();
	m_result_point_list.clear();

	take_open_card();
	insert_to_sortcardlist(e_owner_none,mOpenCard);
	
	int dice=mOpenCard.point%4;
	switch (dice)
	{
	case 1:
		take_one_card(e_owner_banker,0);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[0]);
		take_one_card(e_owner_forwarddoor,0);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[0]);
		take_one_card(e_owner_oppositedoor,0);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[0]);
		take_one_card(e_owner_reversedoor,0);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[0]);

		take_one_card(e_owner_banker,1);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[1]);
		take_one_card(e_owner_forwarddoor,1);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[1]);
		take_one_card(e_owner_oppositedoor,1);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[1]);
		take_one_card(e_owner_reversedoor,1);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[1]);
		break;
	case 2:
		take_one_card(e_owner_forwarddoor,0);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[0]);
		take_one_card(e_owner_oppositedoor,0);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[0]);
		take_one_card(e_owner_reversedoor,0);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[0]);
		take_one_card(e_owner_banker,0);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[0]);

		take_one_card(e_owner_forwarddoor,1);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[1]);
		take_one_card(e_owner_oppositedoor,1);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[1]);
		take_one_card(e_owner_reversedoor,1);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[1]);
		take_one_card(e_owner_banker,1);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[1]);

		break;
	case 3:
		take_one_card(e_owner_oppositedoor,0);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[0]);
		take_one_card(e_owner_reversedoor,0);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[0]);
		take_one_card(e_owner_banker,0);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[0]);
		take_one_card(e_owner_forwarddoor,0);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[0]);

		take_one_card(e_owner_oppositedoor,1);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[1]);
		take_one_card(e_owner_reversedoor,1);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[1]);
		take_one_card(e_owner_banker,1);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[1]);
		take_one_card(e_owner_forwarddoor,1);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[1]);
		break;
	case 0:
		take_one_card(e_owner_reversedoor,0);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[0]);
		take_one_card(e_owner_banker,0);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[0]);
		take_one_card(e_owner_forwarddoor,0);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[0]);
		take_one_card(e_owner_oppositedoor,0);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[0]);

		take_one_card(e_owner_reversedoor,1);
		insert_to_sortcardlist(e_owner_reversedoor,m_doorMap_card[e_owner_reversedoor].cards[1]);
		take_one_card(e_owner_banker,1);
		insert_to_sortcardlist(e_owner_banker,m_doorMap_card[e_owner_banker].cards[1]);
		take_one_card(e_owner_forwarddoor,1);
		insert_to_sortcardlist(e_owner_forwarddoor,m_doorMap_card[e_owner_forwarddoor].cards[1]);
		take_one_card(e_owner_oppositedoor,1);
		insert_to_sortcardlist(e_owner_oppositedoor,m_doorMap_card[e_owner_oppositedoor].cards[1]);
		break;
	}

}
/*	e_flower_diamond = 0;	//方块
	e_flower_club = 1;		//梅花
	e_flower_spade = 2;		//黑桃
	e_flower_heart = 3;		//红心*/
//红Q>红2>红8>红4>红10>红6>黑4>红J>黑6>红9>黑8>黑7>红5>黑A>黑3（黑桃A和3大小相同，红桃Q和方块Q大小相同；
//得到(非组合牌)里最大的一张牌的点数
BYTE logic_core::get_single_card_point(const Card& cd)
{
	if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==12)//双天
	{
		return UCHAR_MAX;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==2)//双地
	{
		return UCHAR_MAX-1;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==8)//双人
	{
		return UCHAR_MAX-2;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==4)//双和
	{
		return UCHAR_MAX-3;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==10)//双梅
	{
		return UCHAR_MAX-4;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==6)//双长
	{
		return UCHAR_MAX-5;
	}
	else if((cd.flower==e_flower_spade || cd.flower==e_flower_club) && cd.point==4)//双板凳
	{
		return UCHAR_MAX-6;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==11)//双斧头
	{
		return UCHAR_MAX-7;
	}
	else if((cd.flower==e_flower_spade || cd.flower==e_flower_club) && cd.point==10)//双红头
	{
		return UCHAR_MAX-8;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==7)//双铜棰
	{
		return UCHAR_MAX-9;
	}
	else if((cd.flower==e_flower_spade|| cd.flower==e_flower_club) && cd.point==6)//双幺五
	{
		return UCHAR_MAX-10;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==9)//杂九
	{
		return UCHAR_MAX-11;
	}
	else if((cd.flower==e_flower_spade || cd.flower==e_flower_club) && cd.point==8)//杂八
	{
		return UCHAR_MAX-12;
	}
	else if((cd.flower==e_flower_spade || cd.flower==e_flower_club) && cd.point==7)//杂七
	{
		return UCHAR_MAX-13;
	}
	else if((cd.flower==e_flower_heart || cd.flower==e_flower_diamond) && cd.point==5)//杂五
	{
		return UCHAR_MAX-14;
	}
	else if(cd.flower==e_flower_spade && (cd.point==1 ||cd.point==3))//至尊宝
	{
		return UCHAR_MAX-15;
	}else
	{
		SLOG_CRITICAL <<"花色："<<cd.flower<<" 点数："<<(int)cd.point<<std::endl;
		assert(false);
	}
	return 0;
}

/*	e_flower_diamond = 0;	//方块
	e_flower_club = 1;		//梅花
	e_flower_spade = 2;		//黑桃
	e_flower_heart = 3;		//红心*/
//得到的点数
BYTE logic_core::get_card_Point(const std::array<Card,2>& cd) const
{
	if(cd[0].flower==e_flower_spade && cd[0].point==1 && cd[1].flower==e_flower_spade && cd[1].point==3)//1至尊宝
	{
		return UCHAR_MAX;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==12 && cd[1].flower==e_flower_diamond && cd[1].point==12)//2双天
	{
		return UCHAR_MAX-1;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==2 && cd[1].flower==e_flower_diamond && cd[1].point==2)//3双地
	{
		return UCHAR_MAX-2;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==8 && cd[1].flower==e_flower_diamond && cd[1].point==8)//4双人
	{
		return UCHAR_MAX-3;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==4 && cd[1].flower==e_flower_diamond && cd[1].point==4)//5双和
	{
		return UCHAR_MAX-4;
	}
	else if((cd[0].flower==e_flower_heart && cd[0].point==10 && cd[1].flower==e_flower_diamond && cd[1].point==10) ||//6双梅，双长，双板凳
		(cd[0].flower==e_flower_heart && cd[0].point==6 && cd[1].flower==e_flower_diamond && cd[1].point==6)||
	(cd[0].flower==e_flower_spade && cd[0].point==4 && cd[1].flower==e_flower_club && cd[1].point==4))
	{
		return UCHAR_MAX-5;	
	}
	else if((cd[0].flower==e_flower_heart && cd[0].point==11 && cd[1].flower==e_flower_diamond && cd[1].point==11)||//7双斧头，双红头，双铜棰，双幺五
	(cd[0].flower==e_flower_spade && cd[0].point==10 && cd[1].flower==e_flower_club && cd[1].point==10)||
	(cd[0].flower==e_flower_heart && cd[0].point==7 && cd[1].flower==e_flower_diamond && cd[1].point==7)||
	(cd[0].flower==e_flower_spade && cd[0].point==6 && cd[1].flower==e_flower_club && cd[1].point==6))
	{
		return UCHAR_MAX-6;	
	}
	else if((cd[0].flower==e_flower_heart && cd[0].point==9 && cd[1].flower==e_flower_diamond && cd[1].point==9) || //8杂九杂八杂七杂五
	(cd[0].flower==e_flower_spade && cd[0].point==8 && cd[1].flower==e_flower_club && cd[1].point==8)||
	(cd[0].flower==e_flower_spade && cd[0].point==7 && cd[1].flower==e_flower_club && cd[1].point==7)||
	(cd[0].flower==e_flower_heart && cd[0].point==5 && cd[1].flower==e_flower_diamond && cd[1].point==5))
	{
		return UCHAR_MAX-7;	
	}
	else if(((cd[0].flower==e_flower_heart && cd[0].point==12 || cd[1].flower==e_flower_heart && cd[1].point==12)||//天九王
	(cd[0].flower==e_flower_diamond && cd[0].point==12 || cd[1].flower==e_flower_diamond && cd[1].point==12)) &&
	(cd[0].point==9 || cd[1].point==9))
	{
		return UCHAR_MAX-8;
	}
	else if(((cd[0].flower==e_flower_heart && cd[0].point==12 || cd[1].flower==e_flower_heart && cd[1].point==12)||//10天槓
	(cd[0].flower==e_flower_diamond && cd[0].point==12 || cd[1].flower==e_flower_diamond && cd[1].point==12)) &&
	(cd[0].point==8 || cd[1].point==8))
	{
		return UCHAR_MAX-9;
	}
	else if(((cd[0].flower==e_flower_heart && cd[0].point==2 || cd[1].flower==e_flower_heart && cd[1].point==2)||
	(cd[0].flower==e_flower_diamond && cd[0].point==2 || cd[1].flower==e_flower_diamond && cd[1].point==2)) && //11地槓
	(cd[0].point==8 || cd[1].point==8))
	{
		return UCHAR_MAX-10;
	}else
	{
		BYTE point=(cd[0].ToPoint()+cd[1].ToPoint())%10;		
		return point;
	}
}

//得到(非组合牌)里最大的一张牌的点数
const Card& logic_core::get_single_maxcard(const std::array<Card,2>& cd)
{
	/*if(cd[0].ToPoint()>cd[1].ToPoint())
	{
		return cd[0];
	}else
	{
		return cd[1];
	}*/
	if(get_single_card_point(cd[0])>get_single_card_point(cd[1]))
	{
		return cd[0];
	}else
	{
		return cd[1];
	}
}

int logic_core::card_campare_single(const CombineCardInfo& cd1,const CombineCardInfo& cd2)
{
	const Card& cc1=get_single_maxcard(cd1.cards);
	const Card& cc2=get_single_maxcard(cd2.cards);

	const BYTE point1=get_single_card_point(cc1);
	const BYTE point2=get_single_card_point(cc2);
	if(point1>point2)
	{
		return 1;
	}else
	{
		return -1;
	}
}

int logic_core::card_campare(const CombineCardInfo& cd1,const CombineCardInfo& cd2)
{
	BYTE point1=get_card_Point(cd1.cards);
	BYTE point2=get_card_Point(cd2.cards);
	if(point1>point2)
	{
		return 1;
	}else if(point1<point2)
	{
		return -1;
	}else if(point1==point2 && point1>100)//都是组合牌，且地位相等时
	{
		return -1;
	}else
	{
		if(point1==point2 && point1==0)
		{
			return -1;
		}else
		{
			return card_campare_single(cd1,cd2);
		}
	}
}

//配置表中配置了 特殊牌（ID,name），此函数返回特殊牌的ID
BYTE logic_core::get_combineCard_Id(const std::array<Card,2>& cd) const
{
	if(cd[0].flower==e_flower_spade && cd[0].point==1 && cd[1].flower==e_flower_spade && cd[1].point==3)//1至尊宝
	{
		return 1;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==12 && cd[1].flower==e_flower_diamond && cd[1].point==12)//2双天
	{
		return 2;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==2 && cd[1].flower==e_flower_diamond && cd[1].point==2)//3双地
	{
		return 3;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==8 && cd[1].flower==e_flower_diamond && cd[1].point==8)//4双人
	{
		return 4;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==4 && cd[1].flower==e_flower_diamond && cd[1].point==4)//5双和
	{
		return 5;
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==10 && cd[1].flower==e_flower_diamond && cd[1].point==10)//双梅
	{
		return 6;	
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==6 && cd[1].flower==e_flower_diamond && cd[1].point==6)//双长
	{
		return 7;
	}
	else if(cd[0].flower==e_flower_spade && cd[0].point==4 && cd[1].flower==e_flower_club && cd[1].point==4)//双板凳
	{
		return 8;	
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==11 && cd[1].flower==e_flower_diamond && cd[1].point==11)//双斧头
	{
		return 9;	
	}
	else if (cd[0].flower==e_flower_spade && cd[0].point==10 && cd[1].flower==e_flower_club && cd[1].point==10)//双红头
	{
		return 10;	
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==7 && cd[1].flower==e_flower_diamond && cd[1].point==7)//双铜棰
	{
		return 11;	
	}
	else if(cd[0].flower==e_flower_spade && cd[0].point==6 && cd[1].flower==e_flower_club && cd[1].point==6)//双幺五
	{
		return 12;	
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==9 && cd[1].flower==e_flower_diamond && cd[1].point==9)//杂九
	{
		return 13;	
	}
	else if(cd[0].flower==e_flower_spade && cd[0].point==8 && cd[1].flower==e_flower_club && cd[1].point==8)//杂八
	{
		return 14;	
	}
	else if (cd[0].flower==e_flower_spade && cd[0].point==7 && cd[1].flower==e_flower_club && cd[1].point==7)//杂七
	{
		return 15;	
	}
	else if(cd[0].flower==e_flower_heart && cd[0].point==5 && cd[1].flower==e_flower_diamond && cd[1].point==5)//杂五
	{
		return 16;	
	}
	else if(((cd[0].flower==e_flower_heart && cd[0].point==12 || cd[1].flower==e_flower_heart && cd[1].point==12)||//天九王
	(cd[0].flower==e_flower_diamond && cd[0].point==12 || cd[1].flower==e_flower_diamond && cd[1].point==12)) &&
	(cd[0].point==9 || cd[1].point==9))
	{
		return 17;
	}
	else if(((cd[0].flower==e_flower_heart && cd[0].point==12 || cd[1].flower==e_flower_heart && cd[1].point==12)||//10天槓
	(cd[0].flower==e_flower_diamond && cd[0].point==12 || cd[1].flower==e_flower_diamond && cd[1].point==12)) &&
	(cd[0].point==8 || cd[1].point==8))
	{
		return 18;
	}
	else if(((cd[0].flower==e_flower_heart && cd[0].point==2 || cd[1].flower==e_flower_heart && cd[1].point==2)||
	(cd[0].flower==e_flower_diamond && cd[0].point==2 || cd[1].flower==e_flower_diamond && cd[1].point==2)) && //11地槓
	(cd[0].point==8 || cd[1].point==8))
	{
		return 19;
	}else
	{
		assert(false);
		return 0;
	}
}

void logic_core::send_GM_Cards()
{
	init_card();
	Card mCard1(e_flower_spade,1);
	m_cards.insert(m_cards.end()-2,mCard1);

	Card mCard2(e_flower_spade,3);
	m_cards.insert(m_cards.end()-7,mCard2);

	take_card();
	set_result();
}

std::ostream& operator<<(std::ostream& stream, const logic_core& cardEngine)
{
	stream<<std::endl;
	for_each(cardEngine.m_sortcart_list.begin(),cardEngine.m_sortcart_list.end(),[&](CardInfo info)
	{
		std::string cardowner="";
		switch (info.owner)
		{
		case game_happysupremacy_protocols::e_owner_none:
			cardowner="翻牌";
			break;
		case game_happysupremacy_protocols::e_owner_banker:
			cardowner="庄家";
			break;
		case game_happysupremacy_protocols::e_owner_forwarddoor:
			cardowner="顺门";
			break;
		case game_happysupremacy_protocols::e_owner_reversedoor:
			cardowner="倒门";
			break;
		case game_happysupremacy_protocols::e_owner_oppositedoor:
			cardowner="对门";
			break;
		default:
			break;
		}
		std::string cardtype="";
		switch (info.card.flower)
		{
		case game_happysupremacy_protocols::e_flower_diamond:
			cardtype="方块";
			break;
		case game_happysupremacy_protocols::e_flower_club:
			cardtype="梅花";
			break;
		case game_happysupremacy_protocols::e_flower_spade:
			cardtype="黑桃";
			break;
		case game_happysupremacy_protocols::e_flower_heart:
			cardtype="红心";
			break;
		default:
			break;
		}
		stream<<"发牌："<<cardowner<<":"<<cardtype<<(int)info.card.point<<std::endl;	
	});
	stream<<std::endl;
	stream<<"组合牌点数:"<<std::endl;
	for_each(cardEngine.m_doorMap_card.begin(),cardEngine.m_doorMap_card.end(),[&](std::pair<e_card_owner,CombineCardInfo> info)
	{
		std::string resultName="";
		BYTE point=0;
		switch (info.first)
		{
		case game_happysupremacy_protocols::e_owner_banker:
			resultName="庄家点数: ";
			break;
		case game_happysupremacy_protocols::e_owner_forwarddoor:
			resultName="顺门点数: ";
			break;
		case game_happysupremacy_protocols::e_owner_reversedoor:
			resultName="倒门点数: ";
			break;
		case game_happysupremacy_protocols::e_owner_oppositedoor:
			resultName="对门点数: ";
			break;
		default:
			break;
		}

		std::string name="";
		point=cardEngine.get_card_Point(info.second.cards);
		if(point>MINCOMBINEPOINT)
		{
			BYTE id=cardEngine.get_combineCard_Id(info.second.cards);
			char* b=itoa(id,NULL,0);
			name= HappySupremacy_MultiLanguageCFG::GetSingleton()->GetData("CombineCardID_"+std::to_string((int)id))->mName;
		}else
		{
			name=std::to_string(point);
		}
		stream<<resultName<<name<<std::endl;
	});
	stream<<std::endl;

	stream<<"下注结果："<<std::endl;
	for_each(cardEngine.m_result_list.begin(),cardEngine.m_result_list.end(),[&](std::pair<e_bet_type,e_bet_result> info)
	{
		std::string resultName="";
		switch (info.first)
		{
		case game_happysupremacy_protocols::e_bettype_forwarddoor:
			resultName="顺门";
			break;
		case game_happysupremacy_protocols::e_bettype_reversedoor:
			resultName="倒门";
			break;
		case game_happysupremacy_protocols::e_bettype_oppositedoor:
			resultName="对门";
			break;
		case game_happysupremacy_protocols::e_bettype_forward_opposite_door:
			resultName="顺对门";
			break;
		case game_happysupremacy_protocols::e_bettype_reverse_opposite_door:
			resultName="倒对门";
			break;
		case game_happysupremacy_protocols::e_bettype_forward_reverse_door:
			resultName="顺倒门";
			break;
		default:
			break;
		}
		std::string result="";
		switch (info.second)
		{
		case game_happysupremacy_protocols::e_betresult_nowin_nolose:
			result="不输不赢";
			break;
		case game_happysupremacy_protocols::e_betresult_win:
			result="赢钱";
			break;
		case game_happysupremacy_protocols::e_betresult_lose:
			result="输钱";
			break;
		default:
			break;
		}
		stream<<resultName<<":"<<result<<std::endl;
	});
	return stream;

}