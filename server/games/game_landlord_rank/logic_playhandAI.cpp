#include "stdafx.h"
#include "logic_playhandAI.h"

logic_playhandAI::logic_playhandAI(logic_core* cardmanager) :
	currentwhoplayHandId(-1),
	orrenege(false),
	orcamp(false)
{
	this->m_cardmanager = cardmanager;
}

void logic_playhandAI::playhand(std::vector<int>& m_desk_cards, std::vector<int>& cards)
{
	for (auto& var : cards)
	{
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

//-----------------------------------------------------机器人AI---------------------------------------------------------------

//双飞
bool logic_playhandAI::IS_SHUANGFEI(std::vector<int>& cards, int index)
{
	if (index + 1<cards.size() && (cards[index] / 100 == 16 || cards[index] / 100 == 17) && (cards[index + 1] / 100 == 16 || cards[index + 1] / 100 == 17))
	{
		return true;
	}
	else
	{
		return false;
	}
}

int logic_playhandAI::IS_JIZHANGPAI(std::vector<int>& cards, int index)
{
	if (index + 3<cards.size() && cards[index] / 100 == cards[index + 1] / 100 && cards[index] / 100 == cards[index + 2] / 100 && cards[index] / 100 == cards[index + 3] / 100)
	{
		return 4;
	}
	else if (index + 2<cards.size() && cards[index] / 100 == cards[index + 1] / 100 && cards[index] / 100 == cards[index + 2] / 100)
	{
		return 3;
	}
	else if (index + 1<cards.size() && cards[index] / 100 == cards[index + 1] / 100)
	{
		return 2;
	}
	else
	{
		return 1;
	}
	assert(false);
	return -1;
}

//-----------------------------------------------------------------------------------------------------------------

int logic_playhandAI::IS_DANZHANGPAISHUNZI(std::vector<int>& cards, int index)
{
	if (cards.size() - index >= 5)
	{
		int result = 1;
		for (size_t i = index; i < cards.size(); i++)
		{
			if (i + 1<cards.size() && cards[i + 1] / 100<15 && cards[i] / 100 + 1 == cards[i + 1] / 100)
			{
				result++;
			}
			else
			{
				break;
			}
		}
		if (result >= 5)
		{
			return result;
		}
		else
		{
			return -1;
		}
	}
	return -1;
}

int logic_playhandAI::IS_ERZHANGPAISHUNZI(std::vector<int>& cards, int index)
{
	if (cards.size() - index >= 6)
	{
		int result = 0;
		if (cards[index] / 100 == cards[index + 1] / 100)
		{
			result += 2;
			for (size_t i = index; i < cards.size(); i += 2)
			{
				if (i + 4<cards.size() && cards[i + 3] / 100 <15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 + 1 == cards[i + 2] / 100 && cards[i + 2] / 100 == cards[i + 3] / 100 && cards[i + 3] / 100!= cards[i + 4] / 100)
				{
					result += 2;
				}
				else if (i + 3<cards.size() && cards[i + 3] / 100 <15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 + 1 == cards[i + 2] / 100 && cards[i + 2] / 100 == cards[i + 3] / 100)
				{
					result += 2;
				}
				else
				{
					break;
				}
			}
		}
		if (result >= 6)
		{
			return result;
		}
		else
		{
			return -1;
		}
	}
	else
	{
		return -1;
	}
}

int logic_playhandAI::IS_SANZHANGPAISHUNZI(std::vector<int>& cards, int index)
{
	if (cards.size() - index >= 6)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = index; i < cards.size();)
		{
			if (i + 3 < cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100 && cards[i + 2] / 100 != cards[i + 3] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						break;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 3;
			}
			else if (i + 2 < cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						break;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 3;
			}
			else
			{
				break;
			}
		}
		if (count >= 2)
		{
			return count * 3;
		}
		else
		{
			return -1;
		}
	}
	return -1;
}

int logic_playhandAI::IS_SIZHANGPAISHUNZI(std::vector<int>& cards, int index)
{
	if (cards.size() - index >= 8)
	{
		int lastpai = -1;
		int count = 0;
		for (size_t i = index; i < cards.size();)
		{
			if (i + 3<cards.size() && cards[i] / 100 != 15 && cards[i] / 100 == cards[i + 1] / 100 && cards[i] / 100 == cards[i + 2] / 100 && cards[i] / 100 == cards[i + 3] / 100)
			{
				if (lastpai > 0)
				{
					if (lastpai + 1 != cards[i] / 100)
					{
						break;
					}
				}
				lastpai = cards[i] / 100;
				count++;
				i += 4;
			}
			else
			{
				break;
			}
		}

		if (count >= 2)
		{
			return count * 4;
		}
		else
		{
			return -1;
		}
	}

	return -1;
}


//得到多张单排
std::vector<int> logic_playhandAI::Get_Playhand_DANPAI_1(int count)
{
	std::vector<int> cardlist;

	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::DANZHANGPAI)
		{
			cardlist.push_back(var.cards[0]);
			if (cardlist.size() >= count)
			{
				return cardlist;
			}
		}
	}

	int count_1 = count - cardlist.size();
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::DANPAISHUNZI && (var.cards.size() - 5 >= count_1 || var.cards.size()  == count_1))
		{
			for (int i = 0; i < count_1; i++)
			{
				cardlist.push_back(var.cards[i]);
			}
			if (cardlist.size() >= count)
			{
				return cardlist;
			}
		}
	}

	if (cardlist.size() != count)
	{
		cardlist.clear();
	}
	return cardlist;
}

//得到多张对牌
std::vector<int> logic_playhandAI::Get_Playhand_DUIPAI_1(int count)
{
	std::vector<int> cardlist;

	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::ERZHANGPAI)
		{
			std::for_each(var.cards.begin(), var.cards.end(), [&](int x) {cardlist.push_back(x); });
			if (cardlist.size() >= count)
			{
				return cardlist;
			}
		}
	}

	int count_1 = count - cardlist.size();
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && (var.cards.size() - 6 >= count_1 || var.cards.size() == count_1))
		{
			for (int i = 0; i < count_1; i++)
			{
				cardlist.push_back(var.cards[i]);
			}
			if (cardlist.size() >= count)
			{
				return cardlist;
			}
		}
	}

	if (cardlist.size() != count)
	{
		cardlist.clear();
	}
	return cardlist;
}


//得到一张单排
std::vector<int> logic_playhandAI::Get_Playhand_DANPAI(int point)
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::DANZHANGPAI)
		{
			if (m_cardmanager->getPoint(cardType::DANZHANGPAI, var.cards) > point)
			{
				cardlist.push_back(var.cards[0]);
				return cardlist;
			}
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	//这里进行拆牌，得到一个具有优化全局的单排
	for (auto& var : mCardTypeList)
	{
		if (var.cards[0] / 100 == 15)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				cardlist.push_back(var.cards[0]);
				return cardlist;
			}
		}
	}

	for (auto& var : mCardTypeList)//拆掉二张牌
	{
		if (var.type == cardType::ERZHANGPAI)
		{
			if (m_cardmanager->getPoint(cardType::DANZHANGPAI, var.cards) > point)
			{
				cardlist.push_back(var.cards[0]);
				return cardlist;
			}
		}
	}

	for (auto& var : mCardTypeList)///拆连6 
	{
		if (var.type == cardType::DANPAISHUNZI && var.cards.size()>=6)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				std::vector<int> list = { var.cards[0] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					cardlist.push_back(var.cards[0]);
					return cardlist;
				}

				list = { var.cards[var.cards.size() - 1] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					cardlist.push_back(var.cards[var.cards.size() - 1]);
					return cardlist;
				}
			}
		}
	}

	for (auto& var : mCardTypeList)//取三条中的单牌
	{
		if (var.type == cardType::SANZHANGPAI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				cardlist.push_back(var.cards[0]);
				return cardlist;
			}
		}
	}

	for (auto& var : mCardTypeList)///取三顺中的单排
	{
		if (var.type == cardType::SANZHANGPAISHUNZI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (auto& var1 : var.cards)
				{
					std::vector<int> list = { var1 };
					if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
					{
						cardlist.push_back(var1);
						return cardlist;
					}
				}
			}
		}
	}

	for (auto& var : mCardTypeList)///拆连5
	{
		if (var.type == cardType::DANPAISHUNZI && var.cards.size() == 5)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (auto& var1 : var.cards)
				{
					std::vector<int> list = { var1 };
					if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
					{
						cardlist.push_back(var1);
						return cardlist;
					}
				}
			}
		}
	}

	for (auto& var : mCardTypeList)///拆 双顺4
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && var.cards.size() >=8)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				std::vector<int> list = { var.cards[0] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					cardlist.push_back(var.cards[0]);
					return cardlist;
				}

				list = { var.cards[var.cards.size() - 1] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					cardlist.push_back(var.cards[var.cards.size() - 1]);
					return cardlist;
				}
			}
		}
	}

	for (auto& var : mCardTypeList)///拆 双顺3
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && var.cards.size() ==6)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (auto& var1 : var.cards)
				{
					std::vector<int> list = { var1 };
					if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
					{
						cardlist.push_back(var1);
						return cardlist;
					}
				}
			}
		}
	}
	return cardlist;
}

std::vector<int> logic_playhandAI::Get_PlayHand_DANPAISHUNZI(int point, int count)
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::DANPAISHUNZI && var.cards.size() == count && m_cardmanager->getPoint(var.type,var.cards)>point)
		{
			for (auto& var1 : var.cards)
			{
				cardlist.push_back(var1);
			}
			return cardlist;
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	for (auto& var : mCardTypeList)//拆二顺
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && var.cards.size() == 2*count && m_cardmanager->getPoint(var.type, var.cards)>point)
		{
			for (int i = 0; i<var.cards.size(); i += 2)
			{
				cardlist.push_back(var.cards[i]);
			}
			return cardlist;
		}
	}

	for (auto& var : mCardTypeList)//拆三顺
	{
		if (var.type == cardType::SANZHANGPAISHUNZI && var.cards.size() == 3 * count && m_cardmanager->getPoint(var.type, var.cards)>point)
		{
			for (int i=0;i<var.cards.size();i+=3)
			{
				cardlist.push_back(var.cards[i]);
			}
			return cardlist;
		}
	}

	for (auto& var : mCardTypeList)//拆单顺方法1
	{
		if (var.type == cardType::DANPAISHUNZI && var.cards.size() >=count+5 && m_cardmanager->getPoint(var.type, var.cards)>point)
		{
			int startIndex = 0;
			int endIndex = 0;
			std::vector<int> list = { var.cards[count-1] };
			if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
			{
				startIndex = 0;
				endIndex = count - 1;
			}
			else
			{
				endIndex =var.cards.size()-1;
				startIndex =endIndex-(count-1);
			}
			for (int i = startIndex; i<=endIndex; i++)
			{
				cardlist.push_back(var.cards[i]);
			}
			return cardlist;
		}
	}

	for (auto& var : mCardTypeList)//拆单顺 方法2
	{
		if (var.type == cardType::DANPAISHUNZI && var.cards.size() > count && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int i = count-1; i < var.cards.size(); i++)
			{
				std::vector<int> list = { var.cards[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i-count+1; j <=i; j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
			}
		}
	}

	for (auto& var : mCardTypeList)//拆双顺
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && var.cards.size() > count * 2 && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			std::vector<int> list_1 = { };
			for (size_t i = 0; i < var.cards.size(); i += 2)
			{
				list_1.push_back(var.cards[i]);
			}

			for (int i = count-1; i < list_1.size(); i++)
			{
				std::vector<int> list = { list_1[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i - count + 1; j <= i; j++)
					{
						cardlist.push_back(list_1[j]);
					}
					return cardlist;
				}
			}
		}
	}

	for (auto& var : mCardTypeList)//拆三顺
	{
		if (var.type == cardType::SANZHANGPAISHUNZI && var.cards.size() > count *3 && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			std::vector<int> list_1 = {};
			for (size_t i = 0; i < var.cards.size(); i += 3)
			{
				list_1.push_back(var.cards[i]);
			}

			for (int i = count-1; i < list_1.size(); i++)
			{
				std::vector<int> list = { list_1[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i - count + 1; j <= i; j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
			}
		}
	}
	return cardlist;
}

std::vector<int> logic_playhandAI::Get_PlayHand_DUIPAI(int point)//跟牌：对牌
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::ERZHANGPAI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (int i = 0; i < var.cards.size(); i++)
				{
					cardlist.push_back(var.cards[i]);
				}
				return cardlist;
			}
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	for (auto& var : mCardTypeList)//拆4顺以上
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && var.cards.size() >= 8)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				std::vector<int> list = { var.cards[0] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = 0; j < 2; j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
				else
				{
					for (int j = var.cards.size() - 2; j < var.cards.size(); j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
			}
		}
	}

	for (auto& var : mCardTypeList)//拆三条
	{
		if (var.type == cardType::SANZHANGPAI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (int j = 0; j < 2; j++)
				{
					cardlist.push_back(var.cards[j]);
				}
				return cardlist;
			}
		}
	}

	for (auto& var : mCardTypeList)//拆双顺
	{
		if (var.type == cardType::ERZHANGPAISHUNZI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (int i = 0; i < var.cards.size(); i += 2)
				{
					std::vector<int> list = { var.cards[i] };
					if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
					{
						for (int j = i; j < i + 2; j++)
						{
							cardlist.push_back(var.cards[j]);
						}
						return cardlist;
					}
				}
			}
		}
	}

	for (auto& var : mCardTypeList)//拆3顺
	{
		if (var.type == cardType::SANZHANGPAISHUNZI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (int i = 0; i < var.cards.size(); i += 3)
				{
					std::vector<int> list = { var.cards[i] };
					if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
					{
						for (int j = i; j < i + 2; j++)
						{
							cardlist.push_back(var.cards[j]);
						}
						return cardlist;
					}
				}
			}
		}
	}
	return cardlist;
}

std::vector<int> logic_playhandAI::Get_PlayHand_DUIPAISHUNZI(int point, int count)//跟牌：对牌顺子
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && var.cards.size() == count && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int j = 0; j < var.cards.size(); j++)
			{
				cardlist.push_back(var.cards[j]);
			}
			return cardlist;
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	for (auto& var : mCardTypeList)//拆2顺
	{
		if (var.type == cardType::ERZHANGPAISHUNZI && var.cards.size() > count && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int i = count - 1; i < var.cards.size(); i += 2)
			{
				std::vector<int> list = { var.cards[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i - count + 1; j <= i; j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
			}
		}
	}

	for (auto& var : mCardTypeList)//拆3顺
	{
		if (var.type == cardType::SANZHANGPAISHUNZI &&  var.cards.size() >= count/2*3 && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			std::vector<int> list_1 = {};
			for (int i = 0; i < var.cards.size(); i += 3)
			{
				list_1.push_back(var.cards[i]);
				list_1.push_back(var.cards[i + 1]);
			}

			for (int i = count - 1; i < list_1.size(); i += 2)
			{
				std::vector<int> list = { list_1[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i - count + 1; j <= i; j++)
					{
						cardlist.push_back(list_1[j]);
					}
					return cardlist;
				}
			}
		}
	}
	return cardlist;
}

std::vector<int> logic_playhandAI::Get_PlayHand_SANTIAO(int point)//跟牌：三条
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SANZHANGPAI && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int j = 0; j < var.cards.size(); j++)
			{
				cardlist.push_back(var.cards[j]);
			}
			return cardlist;
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SANZHANGPAISHUNZI && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int i = 0; i < var.cards.size(); i += 3)
			{
				std::vector<int> list = { var.cards[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i; j < 3; j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
			}
		}
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SANDAIYI(int point)//跟牌：三条1
{
	std::vector<int> cardlist;

	std::vector<int> cardlist_1 = Get_PlayHand_SANTIAO(point);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards= m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards,cardlist_1);
	Init_Card_Type(cards);

	std::vector<int> cardlist_2=Get_Playhand_DANPAI(0);
	if (cardlist_2.size() == 0)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SANDAIYIDUI(int point)//跟牌：三条1对
{
	std::vector<int> cardlist;
	std::vector<int> cardlist_1 = Get_PlayHand_SANTIAO(point);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);
	Init_Card_Type(cards);

	std::vector<int> cardlist_2 = Get_PlayHand_DUIPAI(0);
	if (cardlist_2.size() == 0)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SANSHUN(int point, int count)//跟牌：三顺
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SANZHANGPAISHUNZI && var.cards.size()==count && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int j = 0; j < var.cards.size(); j++)
			{
				cardlist.push_back(var.cards[j]);
			}
			return cardlist;
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SANZHANGPAISHUNZI && var.cards.size() > count  && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int i = count-1; i < var.cards.size(); i += 3)
			{
				std::vector<int> list = { var.cards[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i-count+1; j <=i; j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
			}
		}
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SANDAIYISHUNZI(int point, int count)//跟牌：三带一 顺子
{
	std::vector<int> cardlist;
	std::vector<int>&& cardlist_1 = Get_PlayHand_SANSHUN(point,count/4*3);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	Init_Card_Type(cards);
	std::vector<int>&& cardlist_2 = Get_Playhand_DANPAI_1(count/4);

	if (cardlist_2.size() !=count/4 )
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SANDAIYIDUISHUNZI(int point, int count)//跟牌：三带一对 顺子
{
	std::vector<int> cardlist;
	std::vector<int>&& cardlist_1 = Get_PlayHand_SANSHUN(point, count / 5 * 3);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}


	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	Init_Card_Type(cards);
	std::vector<int>&& cardlist_2 = Get_Playhand_DUIPAI_1(count/5*2);
	if (cardlist_2.size() != count/5*2)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SITIAO(int point)//跟牌：四条
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SIZHANGPAI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (int j = 0; j < var.cards.size(); j++)
				{
					cardlist.push_back(var.cards[j]);
				}
				return cardlist;
			}
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SIZHANGPAISHUNZI)
		{
			if (m_cardmanager->getPoint(var.type, var.cards) > point)
			{
				for (int i = 0; i < var.cards.size(); i += 4)
				{
					std::vector<int> list = { var.cards[i] };
					if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
					{
						for (int j = i; j < 4; j++)
						{
							cardlist.push_back(var.cards[j]);
						}
						return cardlist;
					}
				}
			}
		}
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIYI(int point)//跟牌：四条1
{
	std::vector<int> cardlist;
	std::vector<int> cardlist_1 =Get_PlayHand_SITIAO(point);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);
	Init_Card_Type(cards);

	std::vector<int> cardlist_2 = Get_Playhand_DANPAI(0);
	if (cardlist_2.size() == 0)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIYIDUI(int point)//跟牌：四条1对
{
	std::vector<int> cardlist;
	std::vector<int> cardlist_1 = Get_PlayHand_SITIAO(point);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);
	Init_Card_Type(cards);

	std::vector<int> cardlist_2 = Get_PlayHand_DUIPAI(0);
	if (cardlist_2.size() == 0)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIER(int point)//跟牌：四条2
{
	std::vector<int> cardlist;
	std::vector<int> cardlist_1 = Get_PlayHand_SITIAO(point);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	std::vector<int> cardlist_2;
	int index = 0;
	while (index<2)
	{
		Init_Card_Type(cards);
		std::vector<int>&& list_danpai = Get_Playhand_DANPAI(0);
		if (list_danpai.size() == 1)
		{
			index++;
		}
		else
		{
			break;
		}
		for (auto& var : list_danpai)
		{
			cardlist_2.push_back(var);
		}
		playhand(cards, list_danpai);
	}

	if (cardlist_2.size() <2)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIERDUI(int point)//跟牌：四条2对
{
	std::vector<int> cardlist;
	std::vector<int> cardlist_1 = Get_PlayHand_SITIAO(point);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	std::vector<int> cardlist_2;
	int index = 0;
	while (index<2)
	{
		Init_Card_Type(cards);
		std::vector<int>&& list_danpai = Get_PlayHand_DUIPAI(0);
		if (list_danpai.size() == 2)
		{
			index++;
		}
		else
		{
			break;
		}
		for (auto& var : list_danpai)
		{
			cardlist_2.push_back(var);
		}
		playhand(cards, list_danpai);
	}

	if (cardlist_2.size() <4)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SITIAOSHUNZI(int point, int count)//跟牌：四条 顺子
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SIZHANGPAISHUNZI && var.cards.size()==count && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int j = 0; j < var.cards.size(); j++)
			{
				cardlist.push_back(var.cards[j]);
			}
			return cardlist;
		}
	}

	if (orcamp)
	{
		return cardlist;
	}

	for (auto& var : mCardTypeList)
	{
		if (var.type == cardType::SANZHANGPAISHUNZI && var.cards.size() > count && m_cardmanager->getPoint(var.type, var.cards) > point)
		{
			for (int i = count-1; i < var.cards.size(); i += 4)
			{
				std::vector<int> list = { var.cards[i] };
				if (m_cardmanager->getPoint(cardType::DANZHANGPAI, list) > point)
				{
					for (int j = i-count+1; j <=i; j++)
					{
						cardlist.push_back(var.cards[j]);
					}
					return cardlist;
				}
			}
		}
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIYISHUNZI(int point, int count)//跟牌：四条1 顺子
{
	std::vector<int> cardlist;
	std::vector<int>&& cardlist_1 = Get_PlayHand_SITIAOSHUNZI(point,count/5*4);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	Init_Card_Type(cards);
	std::vector<int>&& cardlist_2 = Get_Playhand_DANPAI_1(count / 5 );

	if (cardlist_2.size() !=count/5)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIYIDUISHUNZI(int point, int count)//跟牌：四条1对 顺子
{
	std::vector<int> cardlist;
	std::vector<int>&& cardlist_1 = Get_PlayHand_SITIAOSHUNZI(point, count / 6 * 4);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	Init_Card_Type(cards);
	std::vector<int>&& cardlist_2 = Get_Playhand_DUIPAI_1(count / 6 * 2);

	if (cardlist_2.size() !=count / 6*2)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIERSHUNZI(int point, int count)//跟牌：四条2 顺子
{
	std::vector<int> cardlist;
	std::vector<int>&& cardlist_1 = Get_PlayHand_SITIAOSHUNZI(point, count / 6 * 4);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	Init_Card_Type(cards);
	std::vector<int>&& cardlist_2 = Get_Playhand_DANPAI_1(count / 6 * 2);

	if (cardlist_2.size() !=count / 6 * 2)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}
std::vector<int> logic_playhandAI::Get_PlayHand_SIDAIERDUISHUNZI(int point, int count)//跟牌：四条2对 顺子
{
	std::vector<int> cardlist;
	std::vector<int>&& cardlist_1 = Get_PlayHand_SITIAOSHUNZI(point, count / 8 * 4);
	if (cardlist_1.size() == 0)
	{
		return cardlist;
	}

	std::vector<int> cards = m_cardmanager->get_cards_info(currentwhoplayHandId);
	playhand(cards, cardlist_1);

	Init_Card_Type(cards);
	std::vector<int>&& cardlist_2 = Get_Playhand_DUIPAI_1(count / 8 * 4);

	if (cardlist_2.size() !=count / 8 * 4)
	{
		return cardlist;
	}

	for (auto& var : cardlist_1)
	{
		cardlist.push_back(var);
	}

	for (auto& var : cardlist_2)
	{
		cardlist.push_back(var);
	}
	return cardlist;
}

std::vector<int> logic_playhandAI::Get_PlayHand_SHUANGFEI()//跟牌：双飞
{
	std::vector<int> cardlist;
	for (auto& var : mCardTypeList)
	{
		if (var.type==cardType::SHUANGFEI)
		{
			for (auto& var1 : var.cards)
			{
				cardlist.push_back(var1);
			}
			return cardlist;
		}
	}
	return cardlist;
}

void logic_playhandAI::Init_Card_Type(int deskId)
{
	std::vector<int>& sort_cards_1 =m_cardmanager->get_cards_info(deskId);
	Init_Card_Type(sort_cards_1);
}

void logic_playhandAI::Init_Card_Type(std::vector<int>& cards)
{
	mCardTypeList.clear();
	std::vector<int>& sort_cards_1 = cards;
	std::sort(sort_cards_1.begin(), sort_cards_1.end(), [](int x, int y) { return x < y; });

	for (int i = 0; i < sort_cards_1.size();)
	{
		cardTypeInfo mInfo;
		if (IS_SHUANGFEI(sort_cards_1, i))
		{
			mInfo.type = cardType::SHUANGFEI;
			mInfo.cards.push_back(sort_cards_1[i]);
			mInfo.cards.push_back(sort_cards_1[i + 1]);
			i += 2;
		}
		else
		{
			int count1 = IS_SIZHANGPAISHUNZI(sort_cards_1, i);
			int count2 = -1;
			if (count1 <= 0)
			{
				count2 = IS_SANZHANGPAISHUNZI(sort_cards_1, i);
			}
			int count3 = -1;
			if (count2 <= 0)
			{
				count3 = IS_ERZHANGPAISHUNZI(sort_cards_1, i);
			}
			int count4 = -1;
			if (count3 <= 0)
			{
				count4 = IS_DANZHANGPAISHUNZI(sort_cards_1, i);
			}

			if (count1 > 0)
			{
				mInfo.type = cardType::SIZHANGPAISHUNZI;
				for (int j = 0; j < count1; j++)
				{
					mInfo.cards.push_back(sort_cards_1[i + j]);
				}
				i += count1;
			}
			else if (count2>0)
			{
				mInfo.type = cardType::SANZHANGPAISHUNZI;
				for (int j = 0; j < count2; j++)
				{
					mInfo.cards.push_back(sort_cards_1[i + j]);
				}
				i += count2;
			}
			else if (count3 > 0)
			{
				mInfo.type = cardType::ERZHANGPAISHUNZI;
				for (int j = 0; j < count3; j++)
				{
					mInfo.cards.push_back(sort_cards_1[i + j]);
				}
				i += count3;
			}
			else if (count4 > 0)
			{
				mInfo.type = cardType::DANPAISHUNZI;
				for (int j = 0; j < count4; j++)
				{
					mInfo.cards.push_back(sort_cards_1[i + j]);
				}
				i += count4;
			}
			else
			{
				int count = IS_JIZHANGPAI(sort_cards_1, i);
				if (count == 4)
				{
					mInfo.type = cardType::SIZHANGPAI;
					mInfo.cards.push_back(sort_cards_1[i]);
					mInfo.cards.push_back(sort_cards_1[i + 1]);
					mInfo.cards.push_back(sort_cards_1[i + 2]);
					mInfo.cards.push_back(sort_cards_1[i + 3]);
				}
				else if (count == 3)
				{
					mInfo.type = cardType::SANZHANGPAI;
					mInfo.cards.push_back(sort_cards_1[i]);
					mInfo.cards.push_back(sort_cards_1[i + 1]);
					mInfo.cards.push_back(sort_cards_1[i + 2]);
				}
				else if (count == 2)
				{
					mInfo.type = cardType::ERZHANGPAI;
					mInfo.cards.push_back(sort_cards_1[i]);
					mInfo.cards.push_back(sort_cards_1[i + 1]);
				}
				else if (count == 1)
				{
					mInfo.type = cardType::DANZHANGPAI;
					mInfo.cards.push_back(sort_cards_1[i]);
				}
				i += count;
			}
		}
		if (orrenege==true && (mInfo.cards[0] / 100 >= 15 || mInfo.type==cardType::SIZHANGPAISHUNZI || mInfo.type==cardType::SIZHANGPAI))
		{
			
		}
		else
		{
			mCardTypeList.push_back(mInfo);
		}
	}

	if (orrenege == true)
	{
		std::vector<int> cardList;
		for (auto& var : mCardTypeList)
		{
			std::for_each(var.cards.begin(), var.cards.end(), [&](int x) {cardList.push_back(x); });
		}

		if (m_cardmanager->GetPlayHandType(cardList) != cardType::NONE || cardList.size()==0)
		{
			orrenege = false;
			Init_Card_Type(sort_cards_1);
		}
	}
}

void logic_playhandAI::Init_Card_Type_1(int deskId)
{
	Init_Card_Type(deskId);
}

std::vector<int> logic_playhandAI::get_Card_Type_List(std::vector<int>& sort_cards_2)
{
	cardType type = m_cardmanager->GetPlayHandType(sort_cards_2);
	int point = m_cardmanager->getPoint(type, sort_cards_2);
	int count = sort_cards_2.size();

	if (type == cardType::SHUANGFEI)
	{

	}
	else if (type == cardType::DANZHANGPAI)
	{
		std::vector<int>&& cardList=Get_Playhand_DANPAI(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::ERZHANGPAI)
	{
		std::vector<int>&& cardList=Get_PlayHand_DUIPAI(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SANZHANGPAI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SANTIAO(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIZHANGPAI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SITIAO(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SANDAIYI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SANDAIYI(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SANDAIYIDUI)
	{
		std::vector<int>&& cardList=Get_PlayHand_SANDAIYIDUI(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIYI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIYI(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIYIDUI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIYIDUI(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIER)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIER(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIERDUI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIERDUI(point);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::DANPAISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_DANPAISHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::ERZHANGPAISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_DUIPAISHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SANZHANGPAISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SANSHUN(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SANDAIYISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SANDAIYISHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SANDAIYIDUISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SANDAIYIDUISHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIZHANGPAISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SITIAOSHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIYISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIYISHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIYIDUISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIYIDUISHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIERSHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIERSHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}
	else if (type == cardType::SIDAIERDUISHUNZI)
	{
		std::vector<int>&& cardList = Get_PlayHand_SIDAIERDUISHUNZI(point,count);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}

	if (type != cardType::SIZHANGPAI && type != cardType::SHUANGFEI)
	{
		std::vector<int>&& cardList=Get_PlayHand_SITIAO(0);
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}

	if (type != cardType::SHUANGFEI)
	{
		std::vector<int>&& cardList=Get_PlayHand_SHUANGFEI();
		if (cardList.size() > 0)
		{
			return cardList;
		}
	}

	std::vector<int> cardList;
	return cardList;
}

std::vector<int> logic_playhandAI::get_Card_Type_List_1(std::vector<cardType>& exceptTypeList,bool orReverse)
{
	std::vector<cardTypeInfo> mTemp_CardTypeList;
	if (orReverse == false)
	{
		mTemp_CardTypeList = mCardTypeList;
	}
	else
	{
		for (int i = mCardTypeList.size() - 1; i >= 0; i--)
		{
			mTemp_CardTypeList.push_back(mCardTypeList[i]);
		}
	}

	for (auto& var : mTemp_CardTypeList)
	{
		bool orContinue=true;
		for (auto& i : exceptTypeList)
		{
			if (var.type == i)
			{
				orContinue = false;
				break;
			}
		}
		if (!orContinue)
		{
			continue;
		}

		if (var.type == cardType::DANZHANGPAI)
		{
			int santiao_count = 0;
			for (auto& var : mCardTypeList)
			{
				if (var.type == cardType::SANZHANGPAI)
				{
					santiao_count++;
				}
				else if (var.type == cardType::SANZHANGPAISHUNZI)
				{
					santiao_count += var.cards.size() / 3;
				}
			}

			int dandui_count = 0;
			for (auto& var : mCardTypeList)
			{
				if (var.type == cardType::DANZHANGPAI)
				{
					dandui_count++;
				}
				else if (var.type == cardType::ERZHANGPAI)
				{
					dandui_count++;
				}
			}

			if (santiao_count <= dandui_count - 2)
			{
				return var.cards;
			}
			else
			{
				std::vector<int>&& cardList = Get_PlayHand_SANTIAO(0);
				std::for_each(var.cards.begin(), var.cards.end(), [&](int x) {cardList.push_back(x); });
				return cardList;
			}
		}
		else if (var.type == cardType::DANPAISHUNZI)
		{
			return var.cards;
		}
		else if (var.type == cardType::ERZHANGPAI)
		{
			int santiao_count = 0;
			for (auto& var : mCardTypeList)
			{
				if (var.type == cardType::SANZHANGPAI)
				{
					santiao_count++;
				}
				else if (var.type == cardType::SANZHANGPAISHUNZI)
				{
					santiao_count += var.cards.size() / 3;
				}
			}

			int dandui_count = 0;
			for (auto& var : mCardTypeList)
			{
				if (var.type == cardType::DANZHANGPAI)
				{
					dandui_count++;
				}
				else if (var.type == cardType::ERZHANGPAI)
				{
					dandui_count++;
				}
			}

			if (santiao_count <= dandui_count - 2)
			{
				return var.cards;
			}
			else
			{
				std::vector<int>&& cardList = Get_PlayHand_SANTIAO(0);
				std::for_each(var.cards.begin(), var.cards.end(), [&](int x) {cardList.push_back(x); });
				return cardList;
			}
		}
		else if (var.type == cardType::ERZHANGPAISHUNZI)
		{
			return var.cards;
		}
		else if (var.type == cardType::SANZHANGPAI)
		{
			std::vector<int> cardList=var.cards;

			std::vector<int> list_1 = {};
			std::vector<int> list_2 = {};

			for (auto& var : mCardTypeList)
			{
				if (var.type == cardType::DANZHANGPAI)
				{
					for (auto& var1 : var.cards)
					{
						list_1.push_back(var1);
					}
					break;
				}
			}

			for (auto& var : mCardTypeList)
			{
				if (var.type == cardType::ERZHANGPAI)
				{
					for (auto& var1 : var.cards)
					{
						list_2.push_back(var1);
					}
					break;
				}
			}

			if (list_1.size() > 0 && list_2.size() > 0)
			{
				if (list_1[0] / 100 > list_2[0] / 100)
				{
					std::for_each(list_2.begin(), list_2.end(), [&](int x) {cardList.push_back(x); });
				}
				else
				{
					std::for_each(list_1.begin(), list_1.end(), [&](int x) {cardList.push_back(x); });
				}
			}
			else if (list_1.size() > 0)
			{
				std::for_each(list_1.begin(), list_1.end(), [&](int x) {cardList.push_back(x); });
			}
			else if (list_2.size() > 0)
			{
				std::for_each(list_2.begin(), list_2.end(), [&](int x) {cardList.push_back(x); });
			}
			return  cardList;
		}
		else if (var.type == cardType::SANZHANGPAISHUNZI)
		{
			 std::vector<int>&& list = Get_PlayHand_SANDAIYISHUNZI(0, var.cards.size());
			if (list.size() > 0)
			{
				return list;
			}
			std::vector<int>&& list_1 = Get_PlayHand_SANDAIYIDUISHUNZI(0, var.cards.size());
			if (list_1.size() > 0)
			{
				return list_1;
			}
			std::vector<int>&& list_2 = Get_PlayHand_SANSHUN(0, var.cards.size());
			if (list_2.size() > 0)
			{
				return list_2;
			}
		}
		else if (var.type == cardType::SIZHANGPAISHUNZI || var.type == cardType::SIZHANGPAI || var.type == cardType::SHUANGFEI)
		{
			continue;
		}
		else
		{
			assert(false);
		}
	}

	std::vector<int> list_2 = {};
	return list_2;
}


std::vector<int> logic_playhandAI::get_Card_Type_List_1(int deskId)
{
	int enemy_count = get_enemy_min_cards(deskId);
	int my_count = m_cardmanager->get_cards_info(deskId).size();
	int friend_count = get_friend_cards(deskId);

	if (NextOrCamp(deskId))
	{
		if (friend_count == 1)
		{
			std::vector<int>&& list = Get_Playhand_DANPAI(0);
			if (list.size()>0 && list[0] / 100 < 15)
			{
				return list;
			}
		}
		else if (friend_count == 2)
		{
			std::vector<int>&& list = Get_PlayHand_DUIPAI(0);
			if (list.size()>0 && list[0] / 100 < 15)
			{
				return list;
			}
		}
		else if (enemy_count == 1)
		{
			std::vector<cardType> exceptList = { cardType::DANZHANGPAI };
			std::vector<int>&& cardList = get_Card_Type_List_1(exceptList);
			if (cardList.size() > 0)
			{
				return cardList;
			}

			exceptList = {};
			std::vector<int>&& cardList_1 = get_Card_Type_List_1(exceptList, true);
			if (cardList_1.size() > 0)
			{
				return cardList_1;
			}
		}
		else if (enemy_count == 2)
		{
			std::vector<cardType> exceptList = { cardType::DANZHANGPAI, cardType::ERZHANGPAI };
			std::vector<int>&& cardList = get_Card_Type_List_1(exceptList);
			if (cardList.size() > 0)
			{
				return cardList;
			}

			exceptList = {};
			std::vector<int>&& cardList_1 = get_Card_Type_List_1(exceptList, true);
			if (cardList_1.size() > 0)
			{
				return cardList_1;
			}
		}
	}
	else
	{
		if (enemy_count == 1)
		{
			std::vector<cardType> exceptList = { cardType::DANZHANGPAI };
			std::vector<int>&& cardList = get_Card_Type_List_1(exceptList);
			if (cardList.size() > 0)
			{
				return cardList;
			}
			
			exceptList = {};
			std::vector<int>&& cardList_1 = get_Card_Type_List_1(exceptList,true);
			if (cardList_1.size() > 0)
			{
				return cardList_1;
			}
		}
		else if (enemy_count == 2)
		{
			std::vector<cardType> exceptList = { cardType::DANZHANGPAI, cardType::ERZHANGPAI };
			std::vector<int>&& cardList = get_Card_Type_List_1(exceptList);
			if (cardList.size() > 0)
			{
				return cardList;
			}

			exceptList = {};
			std::vector<int>&& cardList_1 = get_Card_Type_List_1(exceptList, true);
			if (cardList_1.size() > 0)
			{
				return cardList_1;
			}
		}
		else if (friend_count == 1)
		{
			std::vector<int>&& list = Get_Playhand_DANPAI(0);
			if (list.size()>0 && list[0] / 100 < 15)
			{
				return list;
			}
		}
		else if (friend_count == 2)
		{
			std::vector<int>&& list = Get_PlayHand_DUIPAI(0);
			if (list.size()>0 && list[0] / 100 < 15)
			{
				return list;
			}
		}
	}

	std::vector<cardType> exceptList = { };
	std::vector<int>&& cardList = get_Card_Type_List_1(exceptList);
	if (cardList.size() > 0)
	{
		return cardList;
	}

	return mCardTypeList[0].cards;
}

//是否同一个阵营
bool logic_playhandAI::orCamp(int deskId, int lastdeskId)
{
	if (deskId != m_cardmanager->landlord_id && lastdeskId !=m_cardmanager->landlord_id)
	{
		return true;
	}
	else
	{
		return false;
	}
}

//下家是否同一个阵营
bool logic_playhandAI::NextOrCamp(int deskId)
{
	if (m_cardmanager->landlord_id != deskId)
	{
		int tem_deskId = deskId + 1;
		if (tem_deskId > 3)
		{
			tem_deskId = 1;
		}
		if (tem_deskId == m_cardmanager->landlord_id)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	else
	{
		return false;
	}
}

//是否藏牌
bool logic_playhandAI::orRenege(int deskId, int lastdeskId)
{
	if (orCamp(deskId, lastdeskId))
	{
		return	true;
	}
	else
	{
		if (currentContinueNotPlayHandCountMap.find(deskId)!=currentContinueNotPlayHandCountMap.end() && currentContinueNotPlayHandCountMap[deskId]>=2)
		{
			return false;
		}
		int count_1 = m_cardmanager->get_cards_info(deskId).size();
		int count_2 = m_cardmanager->get_cards_info(lastdeskId).size();
		if (count_1 - count_2 >= 8 || count_2 <= 3)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}

//敌方最小牌
int logic_playhandAI::get_enemy_min_cards(int deskId)
{
	if (deskId != m_cardmanager->get_Landlord())
	{
		int count = m_cardmanager->get_cards_info(m_cardmanager->get_Landlord()).size();
		return count;
	}
	else
	{
		int min_count = 18;
		for (auto var : m_cardmanager->get_cards_info())
		{
			if (var.first != m_cardmanager->get_Landlord())
			{
				if (var.second.size()<min_count)
				{
					min_count = var.second.size();
				}
			}
		}
		return min_count;
	}
}

//友方牌数量
int logic_playhandAI::get_friend_cards(int deskId)
{
	if (deskId != m_cardmanager->get_Landlord())
	{
		for (auto& var : m_cardmanager->get_cards_info())
		{
			if (var.first != deskId && var.first != m_cardmanager->get_Landlord())
			{
				return var.second.size();
			}
		}
	}
	return -1;
}

void logic_playhandAI::InitAI()
{
	for (auto& var : currentContinueNotPlayHandCountMap)
	{
		var.second = 0;
	}
}


game_landlord_rank_protocol::card_Info logic_playhandAI::get_rob_playhand(logic_player* player)
{
	game_landlord_rank_protocol::card_Info result_cards;
	result_cards.set_deskid(player->get_deskId());

	int realLandlord = player->get_table()->get_realLandlord_Id();
	int deskId = player->get_deskId();
	currentwhoplayHandId = deskId;
	std::vector<int>& lastplayhand = player->get_table()->get_lastplayhand();
	int lastplayhand_Id = player->get_table()->get_lastplayhand_Id();
	if (lastplayhand_Id != 0 && lastplayhand_Id != deskId)//跟牌
	{
		if (orCamp(deskId, lastplayhand_Id))
		{
			orcamp = true;
		}
		else
		{
			orcamp = false;
		}

		if (orRenege(deskId, lastplayhand_Id))
		{
			orrenege= true;
		}
		else
		{
			orrenege= false;
		}

		Init_Card_Type(deskId);
		std::vector<int> sort_cards_2 = lastplayhand;
		std::sort(sort_cards_2.begin(), sort_cards_2.end(), [](int x, int y) { return x < y; });

		std::vector<int>&& list = get_Card_Type_List(sort_cards_2);
		if (list.size() == 0)
		{
			if (currentContinueNotPlayHandCountMap.find(deskId)==currentContinueNotPlayHandCountMap.end())
			{
				currentContinueNotPlayHandCountMap[deskId] = 1;
			}
			else
			{
				currentContinueNotPlayHandCountMap[deskId]++;
			}
		}
		else
		{
			currentContinueNotPlayHandCountMap[deskId] = 0;
		}


		if (list.size()>0 && !m_cardmanager->check_cards(deskId, list))
		{
			SLOG_CRITICAL << "AI 跟牌 错误" << std::endl;
		}
		
		for (auto var : list)
		{
			result_cards.add_cards(var);
		}
		
	}
	else
	{
		orcamp = false;
		orrenege= false;
		Init_Card_Type(deskId);
		std::vector<int>&& list = get_Card_Type_List_1(deskId);

		if (list.size()>0 && !m_cardmanager->check_cards(deskId, list))
		{
			SLOG_CRITICAL << "AI 主动 出牌错误" << std::endl;
		}
		
		for (auto var : list)
		{
			result_cards.add_cards(var);
		}
		
	}

	return result_cards;
}
