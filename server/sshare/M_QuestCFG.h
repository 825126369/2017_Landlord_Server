#pragma once
#include <boost/unordered_map.hpp>
#include <vector>
struct M_QuestCFGData
{
	//任务ID
	int mID;
	//名字
	std::string mName;
	//描述
	std::string mDesc;
	//任务ICON1
	std::string mIcon1;
	//任务ICON2
	std::string mIcon2;
	//边框
	std::string mFrame;
	//初始化
	bool mDefault;
	//分类
	int mClass;
	//类型
	int mType;
	//完成条件类型
	int mCompleteType;
	//完成计数
	int mCompleteCount;
	//特殊参数
	int mCompleteParam;
	//后续任务id
	int mNextQuestID;
	//是否保留计数
	bool mIsSaveCount;
	//奖励物品ID
	std::vector<int> mAwardItemIDs;
	//奖励物品数量
	std::vector<int> mAwardItemCounts;
	//计数是否设置(或者累加)
	bool mIsSet;
	//是否有前往
	int mGoTo;
};

class M_QuestCFG
{
public:
private:
	static std::auto_ptr<M_QuestCFG> msSingleton;
public:
	int GetCount();
	const M_QuestCFGData* GetData(int ID);
	boost::unordered_map<int, M_QuestCFGData>& GetMapData();
	void Reload();
	void Reload(const std::string& path);
	void Load(const std::string& path);
	void Load();
	static M_QuestCFG* GetSingleton();
private:
	boost::unordered_map<int, M_QuestCFGData> mMapData;
};
