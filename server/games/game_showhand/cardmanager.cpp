#include "stdafx.h"
#include "cardmanager.h"

cardmanager::cardmanager()
{
	init_cards();
}

void cardmanager::init_cards()
{
	p_left_cards.reserve(5);
	p_right_cards.reserve(5);

	m_cards.reserve(28);
	std::array<int,7> aaa = { 8,9,10,11,12,13,14};
	for(auto& var : aaa)
	{
		m_cards.push_back(var * 100 + 1);//方
		m_cards.push_back(var * 100 + 2);//梅
		m_cards.push_back(var * 100 + 3);//红
		m_cards.push_back(var * 100 + 4);//黑
	}

	std::random_shuffle(m_cards.begin(),m_cards.end());//洗下牌
	assert(m_cards.capacity() == 28);
}

void cardmanager::sendcard()
{
	p_left_cards.clear();
	p_right_cards.clear();

	srand(time(NULL));
	std::random_shuffle(m_cards.begin(), m_cards.end(), [](int x) {return rand() % x; });//洗下牌

	for (int i = 0; i < 5; i++)
	{
		p_left_cards.push_back(m_cards[i]);
	}

	for (int i = 5; i < 10; i++)
	{
		p_right_cards.push_back(m_cards[i]);
	}

	//send_GM_Cards();
	set_result();
}

void cardmanager::send_cheat_card(bool left_win)
{
	sendcard();
	if(left_win!=get_left_result())
	{
		orleftwin=!orleftwin;

		std::vector<int> temp=p_left_cards;
		p_left_cards=p_right_cards;
		p_right_cards=temp;
	}
	set_result();
	assert(left_win==get_left_result());
}

void cardmanager::gm_exchange_card()
{
	std::vector<int> temp=p_left_cards;
	p_left_cards=p_right_cards;
	p_right_cards=temp;
	set_result();
}

bool cardmanager::get_left_result()
{
	return orleftwin;
}

std::vector<int>& cardmanager::get_left_cards()
{
	return p_left_cards;
}

std::vector<int>& cardmanager::get_right_cards()
{
	return p_right_cards;
}

void cardmanager::set_result()
{
	int result= compare_cards();
	if(compare_cards()>0)
	{
		orleftwin=true;
	}else
	{
		orleftwin=false;
	}
	//std::cout << (result > 0 ? "左赢" : "右赢") << std::endl;

	set_whofirstbetlist();//每局谁先下注
	set_whowinratelist();
}

void cardmanager::set_whofirstbetlist()
{
	firstbet_list.clear();
	firstbet_list.resize(4);
	std::vector<int> left_sortcards;
	std::vector<int> right_sortcards;
	for (int i = 1; i < 5; i++)
	{
		left_sortcards.push_back(p_left_cards[i]);
		right_sortcards.push_back(p_right_cards[i]);

		int point1 = get_whofirstbet_point (i,left_sortcards);
		int point2 = get_whofirstbet_point (i,right_sortcards);
		firstbet_list[i-1]=point1>point2;

		//std::cout << "谁先下注：" << (firstbet_list[i - 1] ? "左":"右") << std::endl;
	}
}

void cardmanager::set_whowinratelist()
{
	left_win_rate.clear();
	left_win_rate.resize(4);
	std::vector<int> left_sortcards;
	std::vector<int> right_sortcards;
	left_sortcards.push_back(p_left_cards[0]);//第一张牌
	for (int i = 1; i < 5; i++)
	{
		left_sortcards.push_back(p_left_cards[i]);
		right_sortcards.push_back(p_right_cards[i]);

		int point1 = get_win_rate_point(i,left_sortcards);
		int point2 = get_win_rate_point(i,right_sortcards);
		left_win_rate[i-1]=point1>point2;
	}

	right_win_rate.clear();
	right_win_rate.resize(4);
	left_sortcards.clear();
	right_sortcards.clear();
	right_sortcards.push_back(p_left_cards[0]);//第一张牌
	for (int i = 1; i < 5; i++)
	{
		left_sortcards.push_back(p_left_cards[i]);
		right_sortcards.push_back(p_right_cards[i]);

		int point1 = get_win_rate_point(i,left_sortcards);
		int point2 = get_win_rate_point(i,right_sortcards);
		right_win_rate[i-1]=point1<point2;
	}

}

std::vector<bool>& cardmanager::get_who_first_bet_list()
{
	return firstbet_list;
}

std::vector<bool>& cardmanager::get_left_win_rate()//得到左边玩家赢得概率
{
	return left_win_rate;
}

std::vector<bool>& cardmanager::get_right_win_rate()//得到右边玩家赢得概率
{
	return right_win_rate;
}

int cardmanager::compare_cards()
{
	int point1=get_cards_point(p_left_cards);
	int point2 = get_cards_point(p_right_cards);

	assert(point1!=point2);
	if(point1>point2)
	{
		return 1;
	}else
	{
		return -1;
	}
}
//根据几张明排
int cardmanager::get_whofirstbet_point(int currentCardIndex,const std::vector<int>& cards)
{
	std::vector<int> sort_cards = cards;
	std::sort(sort_cards.begin(), sort_cards.end(), [](int x, int y) { return x<y; });
	if(currentCardIndex==1)
	{
		return get_point_SANPAI(sort_cards);
	}else if(currentCardIndex==2)
	{
		if(Is_DUIZI(sort_cards))
		{
			return get_point_DUIZI(sort_cards);
		}
		else
		{
			return get_point_SANPAI(sort_cards);
		}
	}else if(currentCardIndex==3)
	{
		if(Is_SANTIAO(sort_cards))
		{
			return get_point_SANTIAO(sort_cards);
		}else if(Is_DUIZI(sort_cards))
		{
			return get_point_DUIZI(sort_cards);
		}else
		{
			return get_point_SANPAI(sort_cards);
		}
	}else if(currentCardIndex==4)
	{
		if(Is_TIEZHI(sort_cards))
		{
			return get_point_TIEZHI(sort_cards);
		}
		else if (Is_SANTIAO(sort_cards))
		{
			return get_point_SANTIAO(sort_cards);
		}
		else if(Is_LIANGDUI(sort_cards))
		{
			return get_point_LIANGDUI(sort_cards);
		}
		else if (Is_DUIZI(sort_cards))
		{
			return get_point_DUIZI(sort_cards);
		}
		else
		{
			return get_point_SANPAI(sort_cards);
		}
	}
	return 0;
}

int cardmanager::get_win_rate_point(int currentCardIndex, const std::vector<int>& cards)
{
	if(cards.size()<5)
	{
		/*if(cards.size()==4)
		{
			std::vector<int> sort_cards = cards;
			std::sort(sort_cards.begin(), sort_cards.end(), [](int x, int y) { return x<y; });
			if(Is_TIEZHI(sort_cards))
			{
				return get_point_TIEZHI(sort_cards);
			}
			else if (Is_SANTIAO(sort_cards))
			{
				return get_point_SANTIAO(sort_cards);
			}
			else if(Is_LIANGDUI(sort_cards))
			{
				return get_point_LIANGDUI(sort_cards);
			}
			else if (Is_DUIZI(sort_cards))
			{
				return get_point_DUIZI(sort_cards);
			}
			else
			{
				return get_point_SANPAI(sort_cards);
			}
		}*/
		return get_whofirstbet_point(currentCardIndex,cards);
	}else
	{
		return get_cards_point(cards);
	}
}

int cardmanager::get_cards_point(const std::vector<int>& cards)//A还可以当作7组成顺子，如A、8、9、10、J  ???
{
	assert(cards.size()==5);
	int result = -1;
	std::vector<int> sort_cards = cards;
	std::sort(sort_cards.begin(), sort_cards.end(), [](int x, int y) { return x<y; }); 
	if(Is_TONGHUASHUN(sort_cards))//同花顺1
	{
		result = get_point_TONGHUASHUN(sort_cards);
	}else if(Is_TIEZHI(sort_cards))//铁枝2
	{
		result = get_point_TIEZHI(sort_cards);
	}else if(Is_HULU(sort_cards))//葫芦3
	{
		result = get_point_HULU(sort_cards);
	}
	else if (Is_TONGHUA(sort_cards))//同花4
	{
		result = get_point_TONGHUA(sort_cards);
	}
	else if (Is_SHUNZI(sort_cards))//顺子5
	{
		result = get_point_SHUNZI(sort_cards);
	}
	else if (Is_SANTIAO(sort_cards))//三条6
	{
		result = get_point_SANTIAO(sort_cards);
	}
	else if (Is_LIANGDUI(sort_cards))//两对7
	{
		result = get_point_LIANGDUI(sort_cards);
	}
	else if (Is_DUIZI(sort_cards))//对子8
	{
		result = get_point_DUIZI(sort_cards);
	}
	else//散牌9
	{
		result =get_point_SANPAI(sort_cards);
	}

	return result;
}

//-----------------------------判断类型------------------------------------------------

//同花顺：同一种花色的顺子，比如全部是黑桃的10、J、Q、K、A；
bool cardmanager::Is_TONGHUASHUN(const std::vector<int>& sort_cards)
{
	return (Is_TONGHUA(sort_cards) && Is_SHUNZI(sort_cards));
}

//四张牌点相同的牌加一张其他的牌。如：88889；
bool cardmanager::Is_TIEZHI(const std::vector<int>& sort_cards)
{
	bool result = false;
	for (size_t i = 0,length=sort_cards.size()-3; i<length; i++)
	{
		int count = 1;
		for (size_t j = i + 1,length=sort_cards.size(); j<length; j++)
		{
			if (sort_cards[i] / 100 == sort_cards[j] / 100)
			{
				count++;
			}
			else
			{
				break;
			}
		}
		if (count == 4)
		{
			result = true;
			break;
		}
	}
	return result;
}

//葫芦：三张牌点相同的牌加一对，比如88899；
bool cardmanager::Is_HULU(const std::vector<int>& sort_cards)//葫芦
{
	bool or_3_equal = false;
	bool or_pair = false;

	int count_1 = 0;
	for (int i = 0; i<4;)
	{
		count_1 = 1;
		for (int j = i + 1; j<5; j++)
		{
			if (sort_cards[i] / 100 == sort_cards[j] / 100)
			{
				count_1++;
			}
		}
		if(count_1==3)
		{
			or_3_equal = true;
			i += 3;
		}else if (count_1 == 2)
		{
			or_pair = true;
			i += 2;
		}else
		{
			i++;
		}
	}
	return or_3_equal && or_pair;

}

//同花：同一种花色的五张任意牌；不全一样
bool cardmanager::Is_TONGHUA(const std::vector<int>& sort_cards)//同花
{
	bool or_flower_equal = false;
	for (int i = 1; i < 5; i++)//先比较花色是否相同
	{
		int count = 0;
		for (int j = 0; j < sort_cards.size(); j++)
		{
			if (sort_cards[j] % 100 != i)
			{
				break;
			}
			else
			{
				count++;
			}
		}
		if (count == sort_cards.size())
		{
			or_flower_equal = true;
			break;
		}
	}
	return or_flower_equal;
}

//顺子：五张牌点相连、花色不全相同的牌；
bool cardmanager::Is_SHUNZI(const std::vector<int>& sort_cards)//顺子
{
	bool or_sort_no_equal = true;
	for (int i = 0; i < sort_cards.size()-1 && sort_cards.size()>=2; i++)
	{
		if (sort_cards[i] / 100 + 1 != sort_cards[i + 1] / 100)
		{
			if (i==sort_cards.size()-2 && sort_cards[sort_cards.size()-1] / 100 == 14 && sort_cards[0]/100 == 8)//A 在顺子中可看做7
			{
				//std::cout << "A as 7" << std::endl;
			}
			else
			{
				or_sort_no_equal = false;
				break;
			}
		}
	}
	return or_sort_no_equal;
}

//三条：三张牌点相同的牌加两张散牌，如：888JQ
bool cardmanager::Is_SANTIAO(const std::vector<int>& sort_cards)//三条
{
	bool or_3_equal = false;
	int count = 0;
	for (size_t i = 0, length = sort_cards.size()-1; i < length;i++)
	{
		if (sort_cards[i] / 100 == sort_cards[i+1] / 100)
		{
			count++;
			if (count == 2)
			{
				or_3_equal = true;
				break;
			}
		}
		else
		{
			count = 0;
		}
	}
	return or_3_equal;

}

//两对：五张牌中有两个对子；
bool cardmanager::Is_LIANGDUI(const std::vector<int>& sort_cards)//两对
{
	int count = 0;
	for (size_t i = 0,length=sort_cards.size()-1; i < length;)
	{
		if (sort_cards[i] / 100 == sort_cards[i + 1] / 100)
		{
			count++;
			i += 2;
		}else
		{
			i++;
		}
	}
	return count==2;
}

//对子：五张牌中有一个对子；
bool cardmanager::Is_DUIZI(const std::vector<int>& sort_cards)//对子
{
	int count = 0;
	for (size_t i = 0,length=sort_cards.size()-1; i < length; i++)
	{
		if (sort_cards[i] / 100 == sort_cards[i + 1] / 100)
		{
			count++;
			break;
		}
	}
	return count == 1;
}


int cardmanager::get_point_TONGHUASHUN(const std::vector<int>& sort_cards)//同花顺（最大单牌花色）？？？
{
	const int min_point = 100000;
	int result_point = min_point;
	int max = 0;
	if(sort_cards[0]/100==8 && sort_cards[4]/100==14)
	{
		result_point += sort_cards[sort_cards.size()-2];
	}else
	{
		result_point += sort_cards[sort_cards.size()-1];
	}
	//std::cout << "同花顺 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_TIEZHI(const std::vector<int>& sort_cards)//铁枝（不需要考虑花色）
{
	const int min_point = 90000;
	int result_point = min_point;
	result_point += sort_cards[2];
	//std::cout << "铁枝 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_HULU(const std::vector<int>& sort_cards)//葫芦（不需要考虑花色）
{
	const int min_point = 80000;
	int result_point = min_point;

	result_point += sort_cards[2];
	//std::cout << "葫芦 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_TONGHUA(const std::vector<int>& sort_cards)//同花（最大单牌花色）？？？
{
	const int min_point = 70000;
	int result_point = min_point;

	result_point += sort_cards[sort_cards.size()-1];
	//std::cout << "同花 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_SHUNZI(const std::vector<int>& sort_cards)//顺子（最大单牌花色）？？？
{
	const int min_point = 60000;
	int result_point = min_point;

	result_point += sort_cards[sort_cards.size()-1];
	//std::cout << "顺子 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_SANTIAO(const std::vector<int>& sort_cards)//三条（不需要考虑花色）
{
	const int min_point = 50000;
	int result_point = min_point;
	result_point += sort_cards[2];
	//std::cout << "三条 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_LIANGDUI(const std::vector<int>& sort_cards)//两对  ??? 需要考虑花色
{
	const int min_point = 40000;
	int result_point = min_point;
	int count = 0;
	for (size_t i = 0, length = sort_cards.size() - 1; i<length;)
	{
		if (sort_cards[i]/100 == sort_cards[i + 1]/100)
		{
			count++;
			if (count == 1)
			{
				result_point += sort_cards[i + 1]/100*10;
			}else
			{
				result_point += sort_cards[i + 1];
			}
			i += 2;
		}else
		{
			i++;
		}
	}
	//std::cout << "两对 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_DUIZI(const std::vector<int>& sort_cards)//对子 ???
{
	const int min_point = 30000;
	int result_point = min_point;
	for(size_t i=0,length= sort_cards.size() - 1;i<length;i++)
	{
		if(sort_cards[i]/100==sort_cards[i+1]/100)
		{
			result_point += sort_cards[i + 1];
			break;
		}
	}
	//std::cout << "对子 分数：" << result_point << std::endl;
	return result_point;
}
int cardmanager::get_point_SANPAI(const std::vector<int>& sort_cards)//散牌
{
	const int min_point = 20000;
	int result_point = min_point;
	result_point += sort_cards[sort_cards.size()-1];
	//std::cout << "散牌 分数：" << result_point << std::endl;
	return result_point;
}

//----------------------------------------------------------------------------------------

int cardmanager::get_cards_type(const std::vector<int>& cards)
{
	std::vector<int> sort_cards=cards;
	std::sort(sort_cards.begin(), sort_cards.end(), [](int x, int y) { return x<y; }); 
	if(Is_TONGHUASHUN(sort_cards))
	{
		return 1;
	}
	else if(Is_TIEZHI(sort_cards))
	{
		return 2;
	}
	else if(Is_HULU(sort_cards))
	{
		return 3;
	}
	else if(Is_TONGHUA(sort_cards))
	{
		return 4;
	}
	else if(Is_SHUNZI(sort_cards))
	{
		return 5;
	}
	else if(Is_SANTIAO(sort_cards))
	{
		return 6;
	}
	else if(Is_LIANGDUI(sort_cards))
	{
		return 7;
	}
	else if(Is_DUIZI(sort_cards))
	{
		return 8;
	}
	else 
	{
		return 9;
	}
}


//--------------------------------------------测试数据-------------------------------------

void cardmanager::send_GM_Cards()
{
	p_left_cards.resize(5);
	p_right_cards.resize(5);

	static int type = 1;
	if(type==1)//同花顺：同一种花色的顺子，比如全部是黑桃的10、J、Q、K、A；
	{
		p_left_cards[0] = 14 * 100 + 1;
		p_left_cards[1] = 8 * 100 + 1;
		p_left_cards[2] = 9 * 100 + 1;
		p_left_cards[3] = 10 * 100 + 1;
		p_left_cards[4] = 11 * 100 + 1;

		p_right_cards[0] = 14 * 100 + 2;
		p_right_cards[1] = 8 * 100 + 2;
		p_right_cards[2] = 9 * 100 + 2;
		p_right_cards[3] = 10 * 100 + 2;
		p_right_cards[4] = 11 * 100 + 2;
	}
	else if (type == 2)//铁支：四张牌点相同的牌加一张其他的牌。如：88889；
	{
		p_left_cards[0] = 9 * 100 + 1;
		p_left_cards[1] = 9 * 100 + 2;
		p_left_cards[2] = 9 * 100 + 3;
		p_left_cards[3] = 9 * 100 + 4;
		p_left_cards[4] = 10 * 100 + 1;

		p_right_cards[0] = 8 * 100 + 2;
		p_right_cards[1] = 8* 100 + 1;
		p_right_cards[2] = 8 * 100 + 2;
		p_right_cards[3] = 8 * 100 + 3;
		p_right_cards[4] = 14 * 100 + 4;
	}
	else if (type == 3)///葫芦：三张牌点相同的牌加一对，比如88899
	{
		p_left_cards[0] = 9 * 100 + 1;
		p_left_cards[1] = 9 * 100 + 2;
		p_left_cards[2] = 9 * 100 + 3;
		p_left_cards[3] = 10 * 100 + 1;
		p_left_cards[4] = 10 * 100 + 2;

		p_right_cards[0] = 14 * 100 + 1;
		p_right_cards[1] = 14 * 100 + 2;
		p_right_cards[2] = 14 * 100 + 3;
		p_right_cards[3] = 10 * 100 + 3;
		p_right_cards[4] = 10 * 100 + 4;
	}
	else if (type == 4)//同花：同一种花色的五张任意牌；
	{
		p_left_cards[0] = 9 * 100 + 1;
		p_left_cards[1] = 10 * 100 + 1;
		p_left_cards[2] = 11 * 100 + 1;
		p_left_cards[3] = 12 * 100 + 1;
		p_left_cards[4] = 14 * 100 + 1;

		p_right_cards[0] = 13 * 100 + 2;
		p_right_cards[1] = 8 * 100 + 2;
		p_right_cards[2] = 9 * 100 + 2;
		p_right_cards[3] = 10 * 100 + 2;
		p_right_cards[4] = 11 * 100 + 2;
	}
	else if (type == 5)//顺子：五张牌点相连、花色不同的牌；
	{
		p_left_cards[0] = 14 * 100 + 1;
		p_left_cards[1] = 8 * 100 + 2;
		p_left_cards[2] = 9 * 100 + 3;
		p_left_cards[3] = 10 * 100 + 4;
		p_left_cards[4] = 11 * 100 + 1;

		p_right_cards[0] = 14 * 100 + 2;
		p_right_cards[1] = 8 * 100 + 1;
		p_right_cards[2] = 9 * 100 + 2;
		p_right_cards[3] = 10 * 100 + 3;
		p_right_cards[4] = 11 * 100 + 2;
	}
	else if (type == 6)//三条：三张牌点相同的牌加两张散牌，如：888JQ
	{
		p_left_cards[0] = 9 * 100 + 1;
		p_left_cards[1] = 9 * 100 + 2;
		p_left_cards[2] = 9 * 100 + 3;
		p_left_cards[3] = 12 * 100 + 4;
		p_left_cards[4] = 13 * 100 + 1;

		p_right_cards[0] = 14 * 100 + 2;
		p_right_cards[1] = 14* 100 + 1;
		p_right_cards[2] = 14* 100 + 3;
		p_right_cards[3] = 12 * 100 + 3;
		p_right_cards[4] = 13 * 100 + 2;

	}
	else if (type == 7)//两对：五张牌中有两个对子；
	{
		p_left_cards[0] = 14 * 100 + 1;
		p_left_cards[1] = 14 * 100 + 2;
		p_left_cards[2] = 11 * 100 + 3;
		p_left_cards[3] = 11 * 100 + 4;
		p_left_cards[4] = 13 * 100 + 1;

		p_right_cards[0] = 14 * 100 + 3;
		p_right_cards[1] = 14 * 100 + 4;
		p_right_cards[2] = 13 * 100 + 2;
		p_right_cards[3] = 11 * 100 + 2;
		p_right_cards[4] = 11 * 100 + 1;
	}
	else if (type == 8)//对子：五张牌中有一个对子；
	{
		p_left_cards[0] = 14 * 100 + 1;
		p_left_cards[1] = 14 * 100 + 2;
		p_left_cards[2] = 9 * 100 + 1;
		p_left_cards[3] = 10 * 100 + 4;
		p_left_cards[4] = 11 * 100 + 3;

		p_right_cards[0] = 14 * 100 + 3;
		p_right_cards[1] = 14 * 100 + 4;
		p_right_cards[2] = 9 * 100 + 2;
		p_right_cards[3] = 10 * 100 + 2;
		p_right_cards[4] = 11 * 100 + 2;
	}
	else if (type == 9)//散牌
	{
		p_left_cards[0] = 8 * 100 + 1;
		p_left_cards[1] = 10 * 100 + 2;
		p_left_cards[2] = 11 * 100 + 3;
		p_left_cards[3] = 12 * 100 + 4;
		p_left_cards[4] = 13 * 100 + 1;

		p_right_cards[0] = 13 * 100 + 2;
		p_right_cards[1] = 8 * 100 + 2;
		p_right_cards[2] = 9 * 100 + 3;
		p_right_cards[3] = 10 * 100 + 3;
		p_right_cards[4] = 11 * 100 + 2;
	}

	if (type > 10)
	{
		int index = 0;
		for(auto& var : m_cards)
		{
			bool or_used = false;
			for(auto& var1 : p_left_cards)
			{
				if (var1 == var)
				{
					or_used = true;
				}
			}
			if (or_used == false)
			{
				p_right_cards[index] = var;
				index++;
				if (index >= 5)
				{
					break;
				}
			}
		}
	}

	type++;
	if(type>=20)
	{
		type = 0;
	}
}

std::ostream& operator<<(std::ostream& stream, const cardmanager& cm)
{
	stream << "左：";
	std::vector<int> sort_left_cards = cm.p_left_cards;
	//std::sort(sort_left_cards.begin(), sort_left_cards.end(), [](int x, int y) { return x<y; });
	for(auto& a : sort_left_cards)
	{
		std::string flowerStr = "";
		int point = a / 100;
		int flower = a % 100;
		switch (flower)
		{
		case 4:
			flowerStr = "黑桃";
			break;
		case 3:
			flowerStr = "红桃";
			break;
		case 2:
			flowerStr = "梅芳";
			break;
		case 1:
			flowerStr = "方块";
			break;
		default:
			break;
		}

		stream<< flowerStr<<point <<"  ";
	}
	stream << std::endl;

	stream << "右：";
	std::vector<int> sort_right_cards = cm.p_right_cards;
	//std::sort(sort_right_cards.begin(), sort_right_cards.end(), [](int x, int y) { return x<y; });
	for (auto& a : sort_right_cards)
	{
		std::string flowerStr = "";
		int point = a / 100;
		int flower = a % 100;
		switch (flower)
		{
		case 4:
			flowerStr = "黑桃";
			break;
		case 3:
			flowerStr = "红桃";
			break;
		case 2:
			flowerStr = "梅芳";
			break;
		case 1:
			flowerStr = "方块";
			break;
		default:
			break;
		}

		stream << flowerStr << point <<"  ";
	}
	stream << std::endl;

	return stream;
}
