#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_GuideCFGData
{
	//引导ID
	int mID;
	//文本内容
	std::string mContent;
	//赠送金币
	int mSendGold;
};

class M_GuideCFG
{
public:
private:
	static std::auto_ptr<M_GuideCFG> msSingleton;
public:
	int GetCount();
	const M_GuideCFGData* GetData(int ID);
	boost::unordered_map<int, M_GuideCFGData>& GetMapData();
	void Reload();
	void Load();
	static M_GuideCFG* GetSingleton();
private:
	boost::unordered_map<int, M_GuideCFGData> mMapData;
};
