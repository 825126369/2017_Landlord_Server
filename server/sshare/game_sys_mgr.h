#pragma once
#include <game_sys_base.h>
#include <enable_hashmap.h>
#include <com_log.h>

class game_sys_mgr:public game_sys_handler
{
public:
	game_sys_mgr();
	virtual ~game_sys_mgr();

	template<class T>
	boost::shared_ptr<T> get_sys()
	{
		auto it = m_syslist.find(T::static_type());
		if(it != m_syslist.end())
		{			
			return CONVERT_POINT(T, it->second);
		}

		SLOG_ERROR << boost::format("find_sys is nullprt: %1%")%T::static_type();
		return nullptr;
	}

	void regedit_sys(game_sys_base::SysBasePtr sysbase);
	void init_sys_object();
	void sys_time_update();
	void sys_load();
	void sys_init();
	void sys_update(double delta);
	void sys_exit();
private:
	ENABLE_MAP<uint16_t, game_sys_base::SysBasePtr> m_syslist;
};
