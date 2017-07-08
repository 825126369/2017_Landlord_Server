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
	e_gst_recharge,			//充值	
	e_gst_chat,				//聊天
	e_gst_mail,             //邮件
	e_gst_dial_lottery,     //转盘抽奖
	e_gst_rank,             //排行
	e_gst_exchange,         //兑换
	e_gst_bag,				//背包
	e_gst_shop,				//商城
	e_gst_friend,			//好友
	e_gst_online_reward,	//在线奖励
	e_gst_safe_deposit_box,	//保险箱
	e_gst_id_generator,	    //id生成器
	e_gst_benefits_sys,		//救济金
	e_gst_pump,				//统计系统
	e_gst_notice,			//公告
	e_gst_mobile_phone_binding, //手机绑定
	e_gst_player_log,           //玩家日志
	e_gst_gm,                   //后台系统
	e_gst_daily_box_lottery,    //每日宝箱抽奖
	e_gst_operation_activity,   //活动
	e_gst_speaker,			    //小喇叭
	e_gst_robots,					//机器人
	e_gst_quest,				//任务成就
	e_gst_star_lottery,			//星星抽奖
	e_gst_order_sys,			//订单系统
};

class game_player;