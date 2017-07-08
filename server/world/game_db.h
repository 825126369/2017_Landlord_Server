#pragma once

#include <db_base.h>
#include <enable_singleton.h>
#include <db_query.h>

//////////////////////////////////////////////////////////////////////////
static const std::string unknown_table = "DefaultTable";
static const std::string DB_PLAYER_INFO = "player_info";
static const std::string DB_COMMON_CONFIG = "common_config";
//玩家索引
static const std::string DB_PLAYER_INDEX = "player_id";

// 邮件
static const std::string DB_MAIL = "playerMail";

static const std::string DB_FISHLORD = "fishlord_player";

static const std::string DB_CROCODILE = "crocodile_player";

static const std::string DB_DICE = "dice";

// 兑换
static const std::string DB_EXCHANGE = "exchange";

// 玩家背包
static const std::string DB_PLAYER_BAG = "playerBag";

// 玩家喜好
static const std::string DB_PLAYER_FAVOR = "playerFavor";

// 后台充值
static const std::string DB_GM_RECHARGE = "gmRecharge";

//每日充值
static const std::string DB_TODAY_RECHARGE = "todayRecharge";
static const std::string DB_YESTERDAY_RECHARGE = "yesterdayRecharge";

// 玩家连续小喇叭
static const std::string DB_SPEAKER = "playerSpeaker";

static const std::string DB_PLAYER_QUEST = "player_quest";

// 头像举报
static const std::string DB_INFORM_HEAD = "informHead";

static const std::string DB_RECHARGE_HISTORY = "recharge_history";

//玩家数据库
class db_player : public db_base
	, public enable_singleton<db_player>
{
public:
	db_player();
	virtual ~db_player();
	virtual void init_index();
};


//////////////////////////////////////////////////////////////////////////
//日志数据库
/*class db_log : public db_queue
	, public enable_singleton<db_log>
{
public:
	db_log();
	virtual ~db_log();
	virtual void init_index();

	virtual const std::string& get_tablename(uint16_t table_type);

};
*/
//////////////////////////////////////////////////////////////////////////

//游戏数据库
class db_game : public db_base
	, public enable_singleton<db_game>
{
public:
	db_game();
	virtual ~db_game();
	virtual void init_index();
};