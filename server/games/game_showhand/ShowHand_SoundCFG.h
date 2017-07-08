#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct ShowHand_SoundCFGData
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

class ShowHand_SoundCFG
{
public:
private:
	static std::auto_ptr<ShowHand_SoundCFG> msSingleton;
public:
	int GetCount();
	const ShowHand_SoundCFGData* GetData(int SoundID);
	boost::unordered_map<int, ShowHand_SoundCFGData>& GetMapData();
	void Reload();
	void Load();
	static ShowHand_SoundCFG* GetSingleton();
private:
	boost::unordered_map<int, ShowHand_SoundCFGData> mMapData;
};
