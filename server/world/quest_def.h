#pragma once
#include <boost/cstdint.hpp>

struct QuestCFGData;
struct quest_info
{
	quest_info()
	{
		memset(this, 0, sizeof(quest_info));
	}

	uint32_t QuestID;
	uint32_t Count;
	bool Received;
	const QuestCFGData* QuestData;
};


struct stItem
{
	stItem(){};
	stItem(int id, int count)
	{
		m_itemId = id;
		m_count = count;
	};


	int m_itemId;
	int m_count;
};
