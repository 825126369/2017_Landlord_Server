#include "stdafx.h"
#include "probability.h"
#include "enable_random.h"

bool Probability::initByProbabilityList(std::vector<int>& pList)
{
	if(pList.size() == 0)
	{
		return false;
	}

	m_rangeList.clear();
	m_total = pList[0];
	int x = 0;

	RANGE r;

	for (int i = 1; i < pList.size(); i++)
	{
		r.first = x;
		r.second = m_total;
		m_rangeList.push_back(r);

		x = m_total;
		m_total += pList[i];
	}

	r.first = x;
	r.second = m_total;
	m_rangeList.push_back(r);
	return true;
}

int Probability::getRandRange(int randNum)
{
	bool run = true;
	while (run)
	{
		for (int i = 0; i < m_rangeList.size(); i++)
		{
			if (randNum >= m_rangeList[i].first && randNum < m_rangeList[i].second)
				return i;
		}
	}
	return 0;
}

int Probability::getRandRange()
{
	int r = global_random::instance().rand_int(0, m_total - 1);

	return getRandRange(r);
}


















