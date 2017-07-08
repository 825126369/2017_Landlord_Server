#pragma once
#include "game_sys_def.h"

class GameItem;
class BagMap;

// 背包管理
class BagMgr : public game_sys_base, public game_object, public enable_obj_pool<BagMgr>
{
public:
	MAKE_SYS_TYPE(e_gst_bag);

	MAKE_GET_OWNER(game_player);

	virtual void init_game_object();

	virtual void init_sys_object();

	// 从db加载数据
	virtual bool sys_load();

	/*
			向背包添加道具
			id		道具ID
			count	数量，<0表示删除若干道具
	*/
	bool addItem(int id, int count);

	// 背包中移除个道具
	bool removeItem(int id, int count);

	// 将道具id全部移除
	bool removeItem(int id);

	// 背包是否为空
	bool empty();

	// 返回某个道具
	GameItem* getItem(int id);

	// 获取物品数量
	int getItemCount(int id);

	// 返回道具map
	BagMap* getBagMap();

	/*
			添加一些永久活动
	*/
	void doActivity();

	void doActivity(int rechargeVal);
public:
	// 玩家id
	Tfield<int32_t>::TFieldPtr m_playerId;			

	// 道具列表
	GMapFieldPtr m_items;
	boost::shared_ptr<BagMap> m_itemsPtr;
};
