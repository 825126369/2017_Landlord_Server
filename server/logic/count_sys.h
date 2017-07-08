#pragma once
#include "game_sys_def.h"
#include "enable_hashmap.h"

class db_base;

// 计数系统，为局数
class CountSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_count);

	virtual void init_sys_object();		

	virtual void sys_time_update();

	bool setGameDb(db_base *dbGame);

	int64_t getCurId(const std::string& key);
private:
	bool _checkReflush(time_t lastCheckTime);

	bool _updateCheckTime(const std::string& typeContent);

	void _check(const std::string& typeContent);
private:
	// 所连接的dbgame数据库
	db_base *m_dbGame;
};

