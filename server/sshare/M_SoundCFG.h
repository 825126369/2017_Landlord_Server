#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_SoundCFGData
{
	//ÉùÒôID
	int mSoundID;
	//ÉùÒôÃû×Ö
	std::string mSoundName;
	//ÉùÒôÂ·¾¶
	std::string mSoundPath;
	//ÉùÒôÊ±¼ä
	int mSoundTime;
};

class M_SoundCFG
{
public:
private:
	static std::auto_ptr<M_SoundCFG> msSingleton;
public:
	int GetCount();
	const M_SoundCFGData* GetData(int SoundID);
	boost::unordered_map<int, M_SoundCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_SoundCFG* GetSingleton();
private:
	boost::unordered_map<int, M_SoundCFGData> mMapData;
};
