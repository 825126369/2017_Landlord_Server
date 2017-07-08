#pragma once

#include <enable_singleton.h>
#include <db_query.h>
#include "game_def.h"

//日志表
enum e_db_log_table
{
	// 活跃次数
	e_dlt_pump_active_count,

	// 活跃人数
	e_dlt_pump_active_person,

	// 金币，礼券变化
	e_dlt_pump_player_money,

	// 赠送礼物
	e_dlt_pump_send_gift,

	// 相框统计
	e_dlt_pump_photo_frame,

	// 通用统计
	e_dlt_pump_general_stat,

	// 个人的赠送礼物日志
	e_dlt_pump_personal_send_gift,

	// 消耗总的统计
	e_dlt_pump_total_consume,

	// 玩家金币增长
	e_dlt_pump_coin_growth,

	//玩家金币增长历史
	e_dlt_pump_coin_growth_history,
	
	//world金币记录
	e_dlt_player_gold_log,

	// 最大同时在线玩家表
	e_dlt_max_online_player,

	// 玩家登陆记录
	e_dlt_player_login,

	// 转账记录
	e_dlt_pump_send_gold,

	e_dlt_max,
};

enum gold_log_type
{
	gold_log_type_unknown = 100,
	gold_log_type_save,
	gold_log_type_load,
	gold_log_type_gift,
	gold_log_type_item,
	gold_log_type_other,
};


//////////////////////////////////////////////////////////////////////////
//日志数据库
class db_log : public db_queue
	, public enable_singleton<db_log>
{
public:
	db_log();
	virtual ~db_log();
	virtual void init_index();

	virtual const std::string& get_tablename(uint16_t table_type);

	void player_gold_log(int32_t player_id, GOLD_TYPE old_value, GOLD_TYPE new_value, int32_t reason);

	void player_login_log(int32_t player_id, const std::string& ip, int port, const std::string& mcode,const std::string& mtype);
};
