#pragma once
#include "game_sys_def.h"

// 星星系统
class StarLotterySys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_star_lottery);

	StarLotterySys();


	virtual void init_sys_object();

	//加载数据
	virtual bool sys_load();

	// 每帧更新
	virtual void sys_update(double delta);

	//获取剩余值
	int get_surplus();
	//更新当前值
	bool update_total(int v);
private:
	int m_total;
	bool m_update;
};