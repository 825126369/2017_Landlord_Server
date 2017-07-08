#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_RobotNameCFGData
{
	//Ë÷Òý
	int mIndex;
	//êÇ³Æ
	std::string mNickName;
};

class M_RobotNameCFG
{
public:
private:
	static std::auto_ptr<M_RobotNameCFG> msSingleton;
public:
	int GetCount();
	const M_RobotNameCFGData* GetData(int Index);
	boost::unordered_map<int, M_RobotNameCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_RobotNameCFG* GetSingleton();
private:
	boost::unordered_map<int, M_RobotNameCFGData> mMapData;
};
