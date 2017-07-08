#pragma once
#include "game_sys_def.h"
#include "enable_hashmap.h"
#include "url_param.h"

//class UrlParam;
class game_player;

/*
		玩家在线实时上下分。
		订单由web后台或API接口生成
*/
class OrderSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_order_sys);

	virtual void sys_update(double delta);

	/*
			从url参数中获取订单信息
	*/
	int addOrder(UrlParam& param);

private:


	// 处理订单, 返回失败原因
	void _process(UrlParam& param);


private:
	// 订单列表
	ENABLE_MAP<std::string, UrlParam> m_orderList;
};



#define ORDER_COUNT_EACH_FRAME 20
#define MAX_ORDER_COUNT 1000



