#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ExchangeCFGData
{
	//兑换ID
	int mChangeId;
	//所花费礼券数量
	int mCostTicket;
	//道具ID（与ItemCFG.xld道具ID对应)
	int mItemId;
};

class ExchangeCFG
{
public:
private:
	static std::auto_ptr<ExchangeCFG> msSingleton;
public:
	int GetCount();
	const ExchangeCFGData* GetData(int ChangeId);
	boost::unordered_map<int, ExchangeCFGData>& GetMapData();
	void Reload();
	void Load();
	static ExchangeCFG* GetSingleton();
private:
	boost::unordered_map<int, ExchangeCFGData> mMapData;
};
