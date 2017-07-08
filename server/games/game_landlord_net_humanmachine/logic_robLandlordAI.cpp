#include "stdafx.h"
#include "logic_robLandlordAI.h"
#include <enable_random.h>


logic_robLandlordAI::logic_robLandlordAI(logic_core* core)
{
	m_cardmanager = core;
}

bool logic_robLandlordAI::orRobLandlord(int deskId)
{
	int dapai_count = 0;
	for(auto& var : m_cardmanager->get_cards_info(deskId))
	{
		if (var / 100 >= 15)
		{
			dapai_count++;
		}
	}

	if (dapai_count >= 3)
	{
		int endIndex = 1;
		if (dapai_count == 3)
		{
			endIndex = 2;
		}
		else if (dapai_count == 4)
		{
			endIndex = 3;
		}
		else if (dapai_count == 5)
		{
			endIndex = 4;
		}
		else if (dapai_count == 6)
		{
			endIndex = 5;
		}
		int or_Rob = global_random::instance().rand_int(1, endIndex);
		if (or_Rob != endIndex)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	else
	{
		int endIndex = 2;
		if (dapai_count == 0)
		{
			endIndex = 7;
		}
		else if (dapai_count == 1)
		{
			endIndex = 6;
		}
		else if (dapai_count == 2)
		{
			endIndex = 5;
		}

		int or_Rob = global_random::instance().rand_int(1, endIndex);
		if (or_Rob <=2)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}