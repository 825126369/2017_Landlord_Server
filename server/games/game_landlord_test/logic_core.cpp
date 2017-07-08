#include "stdafx.h"
#include "logic_core.h"
#include "logic_def.h"
#include <climits>
#include <algorithm>
#include <string>
#include <cstdlib>
#include "logic_table.h"
#include "Landlord_NetHumanMachine_RoomCFG.h"
#include "logic_playhandAI.h"
#include "logic_robLandlordAI.h"

#define CARDS_COUNT 54
logic_core::logic_core(const Landlord_NetHumanMachine_RoomCFGData* mcfg):
	landlord_id(0)
	,mOpenCard(0)
{
	this->m_cfg = mcfg;
	this->deskCount = mcfg->mdeskCount;
	m_playhandAI = new logic_playhandAI(this);
	m_robLandlordAI = new logic_robLandlordAI(this);
}

logic_core::~logic_core() 
{
	if (m_playhandAI != nullptr)
	{
		delete m_playhandAI;
		m_playhandAI = nullptr;
	}

	if (m_robLandlordAI != 0)
	{
		delete m_robLandlordAI;
		m_robLandlordAI = 0;
	}
}

void logic_core::init()
{
	m_cards.clear();
	m_cardMap.clear();
	m_RemainlandlordCards.clear();
	for (int i = 3; i <= 15; i++)
	{
		m_cards.push_back(i * 100 + 1);
		m_cards.push_back(i * 100 + 2);
		m_cards.push_back(i * 100 + 3);
		m_cards.push_back(i * 100 + 4);
	}
	m_cards.push_back(16 * 100);
	m_cards.push_back(17 * 100);
	assert(m_cards.size()==CARDS_COUNT);

	srand(time(NULL));
	std::random_shuffle(m_cards.begin(), m_cards.end(), [](int x) { return std::rand() % x; });//洗下牌
}

void logic_core::send_card()
{
	if (m_cfg->morShuffle)
	{
		init();
	}
	else
	{
		for (auto var : m_cardMap)
		{
			for (auto var1 : var.second)
			{
				m_cards.push_back(var1);
			}
		}
		if (m_cards.size() != CARDS_COUNT)
		{
			//SLOG_CRITICAL << "因回收卡牌有误,所以重新洗牌";
			init();
		}
	}

	m_playhandAI->InitAI();

	m_cardMap.clear();
	mOpenCard = m_cards[m_cards.size() - 1];

	int rand_1 = rand(deskCount*2, m_cards.size() - deskCount*2);
	std::vector<int> m_cards_1;
	for (int i = rand_1; i < m_cards.size(); i++)
	{
		m_cards_1.push_back(m_cards[i]);
	}
	for (int i = 0; i < rand_1; i++)
	{
		m_cards_1.push_back(m_cards[i]);
	}
	m_cards = m_cards_1;

	int i = 1;
	while (m_cards.size() > 3)
	{
		int card = take_one_card();
		if (mOpenCard == card)
		{
			landlord_id = i;
		}
		m_cardMap[i].push_back(card);
		i++;
		if (i > deskCount)
		{
			i = 1;
		}
	}

	assert(landlord_id >= 1 && landlord_id <= deskCount);
	m_RemainlandlordCards = m_cards;
	assert(m_RemainlandlordCards.size()==deskCount);

	m_cards.clear();
}

void logic_core::GameFinish()
{
	for (auto var : m_cardMap)
	{
		for (auto var1 : var.second)
		{
			m_cards.push_back(var1);
		}
	}
	m_cardMap.clear();
}

int logic_core::rand(int min, int max)
{
	return min +  std::rand()%(max-min+1);
}

int logic_core::take_one_card()
{
	int card=m_cards[m_cards.size() - 1];
	m_cards.pop_back();
	return card;
}

void logic_core::playhand(int deskid,std::vector<int>& cards)
{
	std::vector<int>& m_desk_cards=m_cardMap[deskid];
	for (auto& var : cards)
	{
		m_cards.push_back(var);
		for (auto var1 = m_desk_cards.begin(); var1 != m_desk_cards.end();)
		{
			if (*var1 == var)
			{
				m_desk_cards.erase(var1);
				break;
			}
			else
			{
				var1++;
				if (var1 == m_desk_cards.end())
				{
					assert(false);
					SLOG_CRITICAL << "此牌没有删掉，或不在牌里：" << var;
				}
			}
		}
	}
}

void logic_core::playhand(std::vector<int>& m_desk_cards, std::vector<int>& cards)
{
	for (auto& var : cards)
	{
		m_cards.push_back(var);
		for (auto var1 = m_desk_cards.begin(); var1 != m_desk_cards.end();)
		{
			if (*var1 == var)
			{
				m_desk_cards.erase(var1);
				break;
			}
			else
			{
				var1++;
				if (var1 == m_desk_cards.end())
				{
					assert(false);
					SLOG_CRITICAL << "此牌没有删掉，或不在牌里：" << var;
				}
			}
		}
	}

}

void logic_core::SureLandlord(int deskId)
{
	std::vector<int>& m_desk_cards = m_cardMap[deskId];
	for (int i = 0; i < m_RemainlandlordCards.size(); i++)
	{
		m_desk_cards.push_back(m_RemainlandlordCards[i]);
	}
	//m_RemainlandlordCards.clear();
	landlord_id = deskId;
	//SLOG_CRITICAL << "地主花落谁家：" << deskId;
}

bool logic_core::check_cards(int deskId, const std::vector<int>& cards_1)
{
	std::vector<int> sort_cards_1 = cards_1;
	std::sort(sort_cards_1.begin(), sort_cards_1.end(), [](int x, int y) { return x<y; });

	cardType type1 = GetPlayHandType(sort_cards_1);
	if (type1 == cardType::NONE)
	{
		SLOG_CRITICAL << "出牌类型 错误";
		return false;
	}

	const std::vector<int>& m_desk_cards =m_cardMap[deskId];
	for (auto& var : cards_1)
	{
		auto& result=find(m_desk_cards.begin(), m_desk_cards.end(), var);
		if (result == m_desk_cards.end())
		{
			SLOG_CRITICAL << "手里没有 这张牌"<<var;
			return false;
		}
	}
	return true;
}

int logic_core::compare_card(std::vector<int>& cards_1, std::vector<int>& cards_2)
{
	std::vector<int> sort_cards_1 = cards_1;
	std::sort(sort_cards_1.begin(), sort_cards_1.end(), [](int x, int y) { return x<y; });

	std::vector<int> sort_cards_2 = cards_2;
	std::sort(sort_cards_2.begin(), sort_cards_2.end(), [](int x, int y) { return x<y; });

	cardType type1 = GetPlayHandType(sort_cards_1);
	cardType type2 = GetPlayHandType(sort_cards_2);
	if (type1!=type2)
	{
		if (type2 == cardType::SHUANGFEI)
		{
			return -1;
		}

		if (type1 == cardType::SHUANGFEI || type1 == cardType::SIZHANGPAI)
		{
			return 1;
		}
		else 
		{
			return -1;
		}
	}
	else if(sort_cards_1.size() != sort_cards_2.size())
	{
		return -1;
	}

	int point1=getPoint(type1,sort_cards_1);
	int point2 = getPoint(type2,sort_cards_2);	

	if (point1 > point2)
	{
		return 1;
	}
	else
	{
		return -1;
	}
	
	assert(false);
	return 0;
}

int logic_core::getPoint(cardType type, std::vector<int>& cards)
{
	switch (type)
	{
	case cardType::SHUANGFEI:
		return FEN_SHUANGFEI(cards);
	case cardType::SIZHANGPAI:
	case cardType::SIDAIYI:
	case cardType::SIDAIER:
	case cardType::SIDAIYIDUI:
	case cardType::SIDAIERDUI:
	case cardType::SIZHANGPAISHUNZI:
	case cardType::SIDAIYISHUNZI:
	case cardType::SIDAIERSHUNZI:
	case cardType::SIDAIYIDUISHUNZI:
	case cardType::SIDAIERDUISHUNZI:
		return FEN_SIPAI(cards);
	case cardType::SANZHANGPAI:
	case cardType::SANDAIYI:
	case cardType::SANDAIYIDUI:
	case cardType::SANZHANGPAISHUNZI:
	case cardType::SANDAIYISHUNZI:
	case cardType::SANDAIYIDUISHUNZI:
		return FEN_SANPAI(cards);
	case cardType::ERZHANGPAI:
	case cardType::ERZHANGPAISHUNZI:
		return FEN_ERPAI(cards);
	case cardType::DANZHANGPAI:
	case cardType::DANPAISHUNZI:
		return FEN_DANPAI(cards);
	default:
		assert(false);
		return -1;
	}
}


cardType logic_core::GetPlayHandType(std::vector<int>& sort_cards)
{
	if (IS_SHUANGFEI(sort_cards))//1
	{
		return cardType::SHUANGFEI;
	}
	else if (IS_SIZHANGPAI(sort_cards))//2
	{
		return cardType::SIZHANGPAI;
	}
	else if (IS_SANPAI(sort_cards))//3
	{
		return cardType::SANZHANGPAI;
	}
	else if (IS_ERPAI(sort_cards))//4
	{
		return cardType::ERZHANGPAI;
	}
	else if (IS_DANPAI(sort_cards))//5
	{
		return cardType::DANZHANGPAI;
	}
	else if (IS_SIDAIYI(sort_cards))//6
	{
		return cardType::SIDAIYI;
	}
	else if (IS_SIDAIYIDUI(sort_cards))//7
	{
		return cardType::SIDAIYIDUI;
	}
	else if (IS_SIDAIER(sort_cards))//8
	{
		return cardType::SIDAIER;
	}
	else if (IS_SIDAIERDUI(sort_cards))//9
	{
		return cardType::SIDAIERDUI;
	}
	else if (IS_SANDAIYI(sort_cards))//10
	{
		return cardType::SANDAIYI;
	}
	else if (IS_SANDAIYIDUI(sort_cards))//11
	{
		return cardType::SANDAIYIDUI;
	}
	else if (IS_SIZHANGPAISHUNZI(sort_cards))//12
	{
		return cardType::SIZHANGPAISHUNZI;
	}
	else if (IS_SIDAIYISHUNZI(sort_cards))//13
	{
		return cardType::SIDAIYISHUNZI;
	}
	else if (IS_SIDAIYIDUISHUNZI(sort_cards))//14
	{
		return cardType::SIDAIYIDUISHUNZI;
	}
	else if (IS_SIDAIERSHUNZI(sort_cards))//15
	{
		return cardType::SIDAIERSHUNZI;
	}
	else if (IS_SIDAIERDUISHUNZI(sort_cards))//16
	{
		return cardType::SIDAIERDUISHUNZI;
	}
	else if (IS_SANZHANGPAISHUNZI(sort_cards))//17
	{
		return cardType::SANZHANGPAISHUNZI;
	}
	else if (IS_SANDAIYISHUNZI(sort_cards))//18
	{
		return cardType::SANDAIYISHUNZI;
	}
	else if (IS_SANDAIYIDUISHUNZI(sort_cards))//19
	{
		return cardType::SANDAIYIDUISHUNZI;
	}
	else if (IS_ERPAISHUN(sort_cards))//20
	{
		return cardType::ERZHANGPAISHUNZI;
	}
	else if (IS_DANPAISHUN(sort_cards))//21
	{
		return cardType::DANPAISHUNZI;
	}
	else
	{
		return cardType::NONE;
	}
}

//双飞
bool logic_core::IS_SHUANGFEI(std::vector<int>& cards)
{
	if (cards.size()==2 && (cards[0] / 100 == 16 || cards[0] / 100 == 17) && (cards[1] / 100 == 16 || cards[1] / 100 == 17))
	{
		return true;
	}
	else
	{
		return false;
	}
}

//四张牌
bool logic_core::IS_SIZHANGPAI(std::vector<int>& cards)
{
	if (cards.size() == 4)
	{
		if (cards[0] / 100 == cards[1] / 100 && cards[0] / 100 == cards[2] / 100 && cards[0] / 100 == cards[3] / 100)
		{
			return true;
		}
	}
	return false;
}

//三张牌
bool logic_core::IS_SANPAI(std::vector<int>& cards)
{
	if (cards.size() ==3)
	{
		if (cards[0] / 100 == cards[1] / 100 && cards[0] / 100 == cards[2] / 100)
		{
			return true;
		}
	}
	return false;
}

//两张牌
bool logic_core::IS_ERPAI(std::vector<int>& cards)
{
	if (cards.size() == 2 && cards[0] / 100 == cards[1] / 100)
	{
		return true;
	}
	else
	{
		return false;
	}
}

//单张牌
bool logic_core::IS_DANPAI(std::vector<int>& cards)
{
	if (cards.size() == 1)
	{
		return true;
	}
	else
	{
		return false;
	}
}

bool logic_core::IS_SIDAIYI(std::vector<int>& cards)
{
	if (cards.size() == 5)
	{
		if (cards[0] / 100 == cards[1] / 100 && cards[0] / 100 == cards[2] / 100 && cards[0] / 100 == cards[3] / 100)
		{
			return true;
		}

		if (cards[1] / 100 == cards[2] / 100 && cards[1] / 100 == cards[3] / 100 && cards[1] / 100 == cards[4] / 100)
		{
			return true;
		}
	}
	return false;
}
bool logic_core::IS_SIDAIYIDUI(std::vector<int>& cards)
{
	if (cards.size() == 6)
	{
		if ((cards[0] / 100 == cards[1] / 100 && cards[0] / 100 == cards[2] / 100 && cards[0] / 100 == cards[3] / 100) && (cards[4]/100==cards[5]/100))
		{
			return true;
		}

		if ((cards[2] / 100 == cards[3] / 100 && cards[2] / 100 == cards[4] / 100 && cards[2] / 100 == cards[5] / 100) && (cards[0] / 100 == cards[1] / 100))
		{
			return true;
		}
	}
	return false;
}
bool logic_core::IS_SIDAIER(std::vector<int>& cards)
{
	if (cards.size() == 6)
	{
		int index = -1;
		for (int i = 0; i < 3; i++)
		{
			if (cards[i] / 100 == cards[i+1] / 100 && cards[i] / 100 == cards[i+2] / 100 && cards[i] / 100 == cards[i+3] / 100)
			{
				index = i;
				break;
			}
		}
		if (index >= 0)
		{
			if (index == 0)
			{
				if (cards[4] / 100 != cards[5] / 100)
				{
					return true;
				}
			}
			else if (index == 1)
			{
				if (cards[0] / 100 != cards[5] / 100)
				{
					return true;
				}
			}
			else if (index == 2)
			{
				if (cards[0] / 100 != cards[1] / 100)
				{
					return true;
				}
			}
		}
	}
	return false;
}
bool logic_core::IS_SIDAIERDUI(std::vector<int>& cards)
{
	if (cards.size() == 8)
	{
		int index = -1;
		for (int i = 0; i < 5; i++)
		{
			if (cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100 && cards[i] / 100 == cards[i + 3] / 100)
			{
				index = i;
				break;
			}
		}
		if (index >= 0 && index%2==0)
		{
			if (index == 0)
			{
				if (cards[4] / 100 == cards[5] / 100 && cards[6] / 100 == cards[7] / 100 && cards[4]/100!=cards[6]/100)
				{
					return true;
				}
			}
			else if (index == 2)
			{
				if (cards[0] / 100 == cards[1] / 100 && cards[6] / 100 == cards[7] / 100)
				{
					return true;
				}
			}
			else if (index == 4)
			{
				if (cards[0] / 100 == cards[1] / 100 && cards[2] / 100 == cards[3] / 100 && cards[0] / 100 != cards[2] / 100)
				{
					return true;
				}
			}
		}
	}
	return false;
}

bool logic_core::IS_SANDAIYI(std::vector<int>& cards)
{
	if (cards.size() == 4)
	{
		if (cards[0] / 100 == cards[1] / 100 && cards[0] / 100 == cards[2] / 100 && cards[0] / 100 != cards[3] / 100)
		{
			return true;
		}

		if (cards[1] / 100 == cards[2] / 100 && cards[1] / 100 == cards[3] / 100 && cards[0] / 100 != cards[1] / 100)
		{
			return true;
		}
	}
	return false;
}

bool logic_core::IS_SANDAIYIDUI(std::vector<int>& cards)
{
	if (cards.size() == 5)
	{
		if (cards[0] / 100 == cards[1] / 100 && cards[0] / 100 == cards[2] / 100 && cards[3] / 100 == cards[4] / 100)
		{
			return true;
		}

		if (cards[2] / 100 == cards[3] / 100 && cards[2] / 100 == cards[4] / 100 && cards[0] / 100 == cards[1] / 100)
		{
			return true;
		}
	}
	return false;
}

bool logic_core::IS_SIZHANGPAISHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 8 && cards.size()%4==0)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i + 3<cards.size() && cards[i]/100!=15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100 && cards[i] / 100 == cards[i + 3] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 4;
			}
			else
			{
				return false;
			}
		}

		if (count>=2 && count * 4== cards.size())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	return false;
}
bool logic_core::IS_SIDAIYISHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 10 && cards.size() % 5 == 0)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i+3<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i+1] / 100 && cards[i] / 100 == cards[i+2] / 100 && cards[i] / 100 == cards[i+3] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 4;
			}
			else  if ((i + 1 < cards.size() && cards[i] / 100 != cards[i + 1] / 100) || i==cards.size()-1)
			{
				i++;
			}else
			{
				return false;
			}
		}
		if (count>=2 && count * 4 + count == cards.size())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	return false;
}
bool logic_core::IS_SIDAIYIDUISHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 12 && cards.size() % 6 == 0)
	{
		int lastpai = -1;
		int count = 0;
		int duiCount = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i+3<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100 && cards[i] / 100 == cards[i + 3] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 4;
			}
			else if (i + 1 < cards.size() && cards[i] / 100 == cards[i + 1] / 100)
			{
				i += 2;
				duiCount++;
			}
			else
			{
				return false;
			}
		}
		if (count>=2 && duiCount==count && count * 4 + count*2 == cards.size())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	return false;
}
bool logic_core::IS_SIDAIERSHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 12 && cards.size() % 6 == 0)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i+3<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100 && cards[i] / 100 == cards[i + 3] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 4;
			}
			else if ((i+1<cards.size() && cards[i]/100 != cards[i + 1]/100) || i==cards.size()-1)
			{
				i++;
			}
			else
			{
				return false;
			}
		}
		if (count>=2 && count * 4 + count*2 == cards.size())
		{
			return true;
		}
		else
		{
			false;
		}
	}
	return false;

}
bool logic_core::IS_SIDAIERDUISHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 16 && cards.size() % 8 == 0)
	{
		int lastpai = -1;
		int count = 0;
		int duiCount = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i + 3<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100 && cards[i] / 100 == cards[i + 3] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 4;
			}
			else if (i + 1<cards.size() && cards[i] / 100 == cards[i + 1] / 100)
			{
				i += 2;
				duiCount++;
			}
			else 
			{
				return false;
			}
		}
		if (count>=2 && duiCount/2==count && count * 4 + count * 4 == cards.size())
		{
			return true;
		}
		else
		{
			false;
		}
	}
	return false;
}

bool logic_core::IS_SANZHANGPAISHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 6 && cards.size() % 3 == 0)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i + 2<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 3;
			}
			else
			{
				return false;
			}
		}
		if (count>=2 && count * 3 == cards.size())
		{
			return true;
		}
		else
		{
			false;
		}
	}
	return false;

}
bool logic_core::IS_SANDAIYISHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 8 && cards.size() % 4 == 0)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i + 2<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 3;
			}
			else if ((i + 1 < cards.size() && cards[i]/100 != cards[i + 1]/100) || i==cards.size()-1)
			{
				i++;
			}
			else
			{
				return false;
			}
		}

		if (count>=2 && count * 3+count == cards.size())
		{
			return true;
		}
		else
		{
			false;
		}
	}
	return false;

}
bool logic_core::IS_SANDAIYIDUISHUNZI(std::vector<int>& cards)
{
	if (cards.size() >= 10 && cards.size() % 5 == 0)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = 0; i < cards.size();)
		{
			if (i + 2<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						return false;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 3;
			}
			else if (i + 1 < cards.size() && cards[i]/100 == cards[i + 1]/100)
			{
				i += 2;
			}
			else
			{
				return false;
			}
		}

		if (count>=2 && count * 3 + count*2 == cards.size())
		{
			return true;
		}
		else
		{
			false;
		}
	}
	return false;

}

//二排顺
bool logic_core::IS_ERPAISHUN(std::vector<int>& cards)
{
	if (cards.size() >= 6 && cards.size()%2==0)
	{
		if (cards[cards.size() - 1] == 15)
		{
			return false;
		}
		int lastpai = -1;
		for (size_t i = 0; i < cards.size(); i+=2)
		{
			if (cards[i] / 100 == cards[i + 1] / 100)
			{
				if (lastpai > 0 && lastpai + 1 != cards[i] / 100)
				{
					return false;
				}
				lastpai = cards[i] / 100;
			}
			else
			{
				return false;
			}
		}
		return true;
	}
	else
	{
		return false;
	}
}

//单排顺
bool logic_core::IS_DANPAISHUN(std::vector<int>& cards)
{
	if (cards.size()>=5)
	{
		if (cards[cards.size() - 1]==15)
		{
			return false;
		}
		for (size_t i = 0; i < cards.size(); i++)
		{
			if (i+1<cards.size() && cards[i] / 100 + 1 != cards[i + 1] / 100)
			{
				return false;
			}
		}
		return true;
	}
	else
	{
		return false;
	}
}

int logic_core::FEN_SHUANGFEI(std::vector<int>& cards)	//双飞
{
	return INT_MAX;
}

int logic_core::FEN_SIPAI(std::vector<int>& cards)	//四排
{
	for (size_t i = cards.size() - 1; i >= 0; i--)
	{
		if (cards[i] / 100 == cards[i-1] / 100 && cards[i] / 100 == cards[i - 2] / 100 && cards[i] / 100 == cards[i - 3] / 100)
		{
			return cards[i]/100;
		}
	}
	return 0;
}

int logic_core::FEN_SANPAI(std::vector<int>& cards)	//三排 
{
	for (size_t i = cards.size() - 1; i >= 0; i--)
	{
		if (cards[i] / 100 == cards[i - 1] / 100 && cards[i] / 100 == cards[i - 2] / 100)
		{
			return cards[i]/100;
		}
	}
	return 0;
}
int logic_core::FEN_ERPAI(std::vector<int>& cards)	//二排
{
	return cards[cards.size() - 1]/100;
}
int logic_core::FEN_DANPAI(std::vector<int>& cards)	//单排顺
{
	return cards[cards.size()-1]/100;
}

//----------------------------------------------------GM TEST--------------------------------------------------------------

void logic_core::TEST_GetPlayHandType(std::vector<int>& sort_cards)
{
	if (IS_SHUANGFEI(sort_cards))//1
	{
		std::cout << "双飞" << std::endl;
	}
	else if (IS_SIZHANGPAI(sort_cards))//2
	{
		std::cout << "炸弹" << std::endl;
	}
	else if (IS_SANPAI(sort_cards))//3
	{
		std::cout << "三张牌" << std::endl;
	}
	else if (IS_ERPAI(sort_cards))//4
	{
		std::cout << "二张牌" << std::endl;
	}
	else if (IS_DANPAI(sort_cards))//5
	{
		std::cout << "单牌" << std::endl;
	}
	else if (IS_SIDAIYI(sort_cards))//6
	{
		std::cout << "四代一" << std::endl;
	}
	else if (IS_SIDAIYIDUI(sort_cards))//7
	{
		std::cout << "四代一对" << std::endl;
	}
	else if (IS_SIDAIER(sort_cards))//8
	{
		std::cout << "四代二" << std::endl;
	}
	else if (IS_SIDAIERDUI(sort_cards))//9
	{
		std::cout << "四带二对" << std::endl;
	}
	else if (IS_SANDAIYI(sort_cards))//10
	{
		std::cout << "三带一" << std::endl;
	}
	else if (IS_SANDAIYIDUI(sort_cards))//11
	{
		std::cout << "三带一对" << std::endl;
	}
	else if (IS_SIZHANGPAISHUNZI(sort_cards))//12
	{
		std::cout << "四张牌顺子" << std::endl;
	}
	else if (IS_SIDAIYISHUNZI(sort_cards))//13
	{
		std::cout << "四带一顺子" << std::endl;
	}
	else if (IS_SIDAIYIDUISHUNZI(sort_cards))//14
	{
		std::cout << "四代一对顺子" << std::endl;
	}
	else if (IS_SIDAIERSHUNZI(sort_cards))//15
	{
		std::cout << "四带二顺子" << std::endl;
	}
	else if (IS_SIDAIERDUISHUNZI(sort_cards))//16
	{
		std::cout << "四带二对顺子" << std::endl;
	}
	else if (IS_SANZHANGPAISHUNZI(sort_cards))//17
	{
		std::cout << "三张牌顺子" << std::endl;
	}
	else if (IS_SANDAIYISHUNZI(sort_cards))//18
	{
		std::cout << "三代一顺子" << std::endl;
	}
	else if (IS_SANDAIYIDUISHUNZI(sort_cards))//19
	{
		std::cout << "三带一对顺子" << std::endl;
	}
	else if (IS_ERPAISHUN(sort_cards))//20
	{
		std::cout << "二牌顺子" << std::endl;
	}
	else if (IS_DANPAISHUN(sort_cards))//21
	{
		std::cout << "单排顺子" << std::endl;
	}
	else
	{
		std::cout << "！！！！！！！！！！数据类型错误" << std::endl;
	}
}

void logic_core::GM_TEST()
{
	std::vector<int> testList = {301,302,303,304,401,402,501,502};
	TEST_GetPlayHandType(testList);
}

game_landlord_net_humanmachine_protocol::card_Info logic_core::get_rob_playhand(logic_player* player)
{
	return m_playhandAI->get_rob_playhand(player);
}

bool logic_core::judge_rob_orRobLandlord(logic_player* player)
{
	return m_robLandlordAI->orRobLandlord(player->get_deskId());
}
