#include "stdafx.h"
#include "game_sys_mgr.h"


game_sys_mgr::game_sys_mgr()
{
};
game_sys_mgr::~game_sys_mgr()
{
	m_syslist.clear();
};

void game_sys_mgr::regedit_sys(game_sys_base::SysBasePtr sysbase)
{
	if(sysbase == nullptr)
	{
		SLOG_ERROR << boost::format("regedit_sys sysbase is nullprt");
		return;
	}

	auto it = m_syslist.find(sysbase->get_sys_type());
	if(it != m_syslist.end())
	{
		SLOG_ERROR << boost::format("regedit_sys repeat: %1%")%sysbase->get_sys_type();
		return;
	}

	sysbase->attach(this);
	m_syslist.insert(std::make_pair(sysbase->get_sys_type(), sysbase));
}

void game_sys_mgr::init_sys_object()//这个首先被执行
{
	for (auto it = m_syslist.begin(); it != m_syslist.end(); ++it)
	{
		it->second->init_sys_object();
	}
}

void game_sys_mgr::sys_time_update()
{
	for (auto it = m_syslist.begin();it != m_syslist.end(); ++it)
	{
		it->second->sys_time_update();
	}
};

void game_sys_mgr::sys_load()
{
	for (auto it = m_syslist.begin();it != m_syslist.end(); ++it)
	{
		if(!it->second->sys_load())
		{
			SLOG_ERROR << boost::format("sys_load type: %1%")%it->first;
		}
	}
}
void  game_sys_mgr::sys_init()
{
	for (auto it = m_syslist.begin();it != m_syslist.end(); ++it)
	{
		it->second->sys_init();
	}
}

void game_sys_mgr::sys_update(double delta)//系统更新
{
	for (auto it = m_syslist.begin();it != m_syslist.end(); ++it)
	{
		it->second->sys_update(delta);
	}
}

// 退出系统
void game_sys_mgr::sys_exit()
{
	for (auto it = m_syslist.begin();
		it != m_syslist.end(); ++it)
	{
		it->second->sys_exit();
	}
}

