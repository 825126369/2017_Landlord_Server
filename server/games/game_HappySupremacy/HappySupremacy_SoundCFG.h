#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct HappySupremacy_SoundCFGData
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

class HappySupremacy_SoundCFG
{
public:
private:
	static std::auto_ptr<HappySupremacy_SoundCFG> msSingleton;
public:
	int GetCount();
	const HappySupremacy_SoundCFGData* GetData(int SoundID);
	boost::unordered_map<int, HappySupremacy_SoundCFGData>& GetMapData();
	void Reload();
	void Load();
	static HappySupremacy_SoundCFG* GetSingleton();
private:
	boost::unordered_map<int, HappySupremacy_SoundCFGData> mMapData;
};
