#pragma once
#include <enable_smart_ptr.h>
#include <game_sys_base.h>

#include <game_object.h>
#include <game_object_field.h>
#include <game_object_map.h>
#include <game_object_array.h>

enum e_game_sys_type
{
	e_gst_none = 0,
	e_gst_vip,			//vip
	e_gst_sign,			//签到
	e_gst_bag,			//背包
	e_gst_battle,		//战斗
	e_gst_hero,			//英雄
	e_gst_lottery,		//抽奖
	e_gst_mail,			//邮件
	e_gst_quest,		//任务
	e_gst_shop,			//商店
	e_gst_snatch,		//抢矿
	e_gst_arena,		//竞技场
	e_gst_expedition,   //远征

	e_gst_count
};

class game_player;