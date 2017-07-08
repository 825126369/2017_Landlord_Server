#pragma once
#include <enable_singleton.h>
#include <game_sys_mgr.h>

class global_sys_mgr:
	public enable_singleton<global_sys_mgr>
	,public game_sys_mgr
{
public:
	global_sys_mgr();
	virtual ~global_sys_mgr();

private:
	void init_sys();
};

#define GLOBAL_SYS(sys_type) global_sys_mgr::instance().get_sys<sys_type>()