#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct SensitiveWordCFGData
{
	//ÐòºÅ
	int mID;
	//×Ö·û
	std::string mSensitiveWord;
};

class SensitiveWordCFG
{
public:
private:
	static std::auto_ptr<SensitiveWordCFG> msSingleton;
public:
	int GetCount();
	const SensitiveWordCFGData* GetData(int ID);
	boost::unordered_map<int, SensitiveWordCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static SensitiveWordCFG* GetSingleton();
private:
	boost::unordered_map<int, SensitiveWordCFGData> mMapData;
};
