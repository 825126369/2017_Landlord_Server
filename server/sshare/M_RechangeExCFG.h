#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_RechangeExCFGData
{
	//渠道ID
	int mID;
	//渠道对应的paycode(顺序索引对应M_RechangeCFG.xls表充值ID)
	std::vector<int> mPayCodes;
};

class M_RechangeExCFG
{
public:
private:
	static std::auto_ptr<M_RechangeExCFG> msSingleton;
public:
	int GetCount();
	const M_RechangeExCFGData* GetData(int ID);
	boost::unordered_map<int, M_RechangeExCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_RechangeExCFG* GetSingleton();
private:
	boost::unordered_map<int, M_RechangeExCFGData> mMapData;
};
