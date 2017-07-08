#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_ChipCFGData
{
	//key
	int mID;
	//对应金额
	int mCount;
	//对应图片
	std::string mIcon;
	//是否选中
	bool mIsCheck;
};

class M_ChipCFG
{
public:
private:
	static std::auto_ptr<M_ChipCFG> msSingleton;
public:
	int GetCount();
	const M_ChipCFGData* GetData(int ID);
	boost::unordered_map<int, M_ChipCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_ChipCFG* GetSingleton();
private:
	boost::unordered_map<int, M_ChipCFGData> mMapData;
};
