#pragma once
#include "game_def.h"
#include "quest_def.h"
#include "game_sys_def.h"
#include <enable_object_pool.h>

//////////////////////////////////////////////////////////////////////////
class game_quest_mgr
	:public game_object
	,public enable_obj_pool<game_quest_mgr>
	,public game_sys_base
{
public:
	game_quest_mgr();
	~game_quest_mgr();

	MAKE_SYS_TYPE(e_gst_quest);
	MAKE_GET_OWNER(game_player);

	virtual bool sys_load();	
	virtual void sys_init();//新玩家需要初始化
	virtual void on_attach();
	virtual void sys_time_update();
	// 每帧更新
	virtual void sys_update(double delta);
	// 系统退出
	virtual void sys_exit();

	void change_quest(uint16_t questtype, uint32_t count = 1, uint32_t param=0);	
	void insert_quest(uint16_t questtype, uint32_t count = 1, uint32_t param=0);

	GMapObjPtr get_map(int qtype);
	bool check_quest(int qtype, uint32_t questid);
	void receive_quest(int qtype, uint32_t questid, std::vector<stItem>& items);	


	void remove_quest(uint16_t questtype);
	void add_quest(uint16_t questtype, uint32_t count = 0, uint32_t param=0);
	//////////////////////////////////////////////////////////////////////////

public:
	virtual void init_game_object();//注册属性
	virtual bool store_game_object(bool to_all = false);

	
	GMapFieldPtr					Quests;
	GMapFieldPtr					Activitys;
	Tfield<bool>::TFieldPtr			NeedReflush;	// 是否需要刷新

private:
	void init_quest();
	double m_checktime;

	double m_checkupdate;
};

