#pragma once
#include "game_sys_def.h"

class game_player;

// 签到结果
struct stStarResult
{
	stStarResult()
	{
		clear();
	}

	int award;	//奖池
	int itemtype;	//物品类型
	int itemcount;	//物品数量

	void clear()
	{
		award = 0;
		itemtype = 0;
		itemcount = 0;
	}
};

// 转盘抽奖管理
class StarLotteryMgr : public enable_obj_pool<StarLotteryMgr>, public game_sys_base
{
public:
	StarLotteryMgr();

	~StarLotteryMgr();

	MAKE_SYS_TYPE(e_gst_star_lottery);

	MAKE_GET_OWNER(game_player);

	virtual void init_sys_object();

	bool Lottery(stStarResult& result);
public:	
	Tfield<int32_t>::TFieldPtr CurStar;		//当前星星
	Tfield<int32_t>::TFieldPtr CurAward;	//当前奖池
	Tfield<int32_t>::TFieldPtr TotalChip;	//累计获取碎片
	Tfield<int32_t>::TFieldPtr CurCount;		//当前次数

	void _lottery(int lvl, stStarResult& result);

	void _sendLotteryNotice(int itemType, int count);
};


