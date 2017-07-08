#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_StarLotteryCFGData
{
	//索引
	int mIndex;
	//类型
	int mStarLvl;
	//物品类型
	int mItemType;
	//物品数量
	int mItemCount;
	//生成几率(10000)
	int mRate1;
	//生成几率(10000)
	int mRate2;
	//生成几率(10000)
	int mRate3;
	//生成几率(10000)
	int mRate4;
};

class M_StarLotteryCFG
{
public:
private:
	static std::auto_ptr<M_StarLotteryCFG> msSingleton;
public:
	int GetCount();
	const M_StarLotteryCFGData* GetData(int Index);
	boost::unordered_map<int, M_StarLotteryCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_StarLotteryCFG* GetSingleton();
private:
	boost::unordered_map<int, M_StarLotteryCFGData> mMapData;
};
