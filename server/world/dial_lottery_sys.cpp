#include "stdafx.h"
#include "dial_lottery_sys.h"

void DialLotterySys::init_sys_object()
{
	std::vector<int> plist;
	boost::unordered_map<int, M_DialLotteryCFGData>& data = M_DialLotteryCFG::GetSingleton()->GetMapData();
	for(auto it = data.begin(); it != data.end(); ++it)
	{
		plist.push_back(it->second.mProbability);
		m_items.push_back(it->second);
	}

	m_prob.initByProbabilityList(plist);
}

bool DialLotterySys::doLottery(int& num, GOLD_TYPE& coin, int& rtype)
{
	int index = m_prob.getRandRange();
	if(index < 0 || index >= (int)m_items.size())
	{
		SLOG_ERROR << boost::format("DialLotterySys::doLottery, Ë÷Òý[%1%]³¬³ö·¶Î§") % index;
		return false;
	}

	num = m_items[index].mID;
	coin = m_items[index].mRewardCoin;
	rtype = m_items[index].mRewardType;
	return true;
}

