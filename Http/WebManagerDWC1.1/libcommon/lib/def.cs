using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

public struct TableName
{
    // 玩家信息表
    public const string PLAYER_INFO = "player_info";

    // 邮件表
    public const string PLAYER_MAIL = "playerMail";

    // 邮件检测表
    public const string CHECK_MAIL = "checkMail";

    // 玩家账号表
    public const string PLAYER_ACCOUNT = "AccountTable";

    // GM账号表
    public const string GM_ACCOUNT = "GmAccount";

    // 计数表
    public const string COUNT_TABLE = "OpLogCurID_DWC";

    // GM操作日志表
    public const string OPLOG = "OpLogDWC";

    // 权限表
    public const string RIGHT = "rightDWC";

    // 玩家登陆历史
    public const string PLAYER_LOGIN = "LoginLog";

    // 封IP表
    public const string BLOCK_IP = "blockIP";

    // 后台充值
    public const string GM_RECHARGE = "gmRecharge";

    // 极光应用
    public const string JPUSH_APP = "jpushAppInfoList";

    // 玩家金币，钻石变化总表
    public const string PUMP_PLAYER_MONEY = "pumpPlayerMoney";

    // 每日任务表
    public const string PUMP_DAILY_TASK = "pumpDailyTask";

    // 成就
    public const string PUMP_TASK = "pumpTask";

    // 抽奖
    public const string PUMP_LOTTERY = "pumpLottery";

    // 邮件
    public const string PUMP_MAIL = "pumpMail";

    // 签到
    public const string PUMP_PLAYER_SIGN = "pumpPlayerSign";

    // 商店购买
    public const string PUMP_SHOP_BUY = "pumpShopBuy";

    // 活跃次数
    public const string PUMP_ACTIVE_COUNT = "pumpActiveCount";

    // 活跃人数
    public const string PUMP_ACTIVE_PERSON = "pumpActivePerson";

    // 通用数据统计
    public const string PUMP_GENERAL_STAT = "pumpGeneralStat";

    // 赠送礼物
    public const string PUMP_SEND_GIFT = "pumpSendGift";

    // 相框统计
    public const string PUMP_PHOTO_FRAME = "pumpPhotoFrame";

    // 总的消耗统计
    public const string PUMP_TOTAL_CONSUME = "pumpTotalConsume";

    // 捕鱼每天的收益情况
    public const string PUMP_FISHLORD_EVERY_DAY = "fishlordEveryDay";

    // 鳄鱼每天的收益情况
    public const string PUMP_CROCODILE_EVERY_DAY = "CrocodileEveryday";

    // 骰宝每天的收益情况
    public const string PUMP_DICE_EVERY_DAY = "DiceEveryday";

    // 鳄鱼下注及获奖次数
    public const string PUMP_CROCODILE_BET = "CrocodileBetInfo";

    // 骰宝数据下注及获奖情况
    public const string PUMP_DICE = "dice_table";

    // 鱼的统计
    public const string PUMP_ALL_FISH = "AllFishLog";

    // 重置时旧的盈利率
    public const string PUMP_OLD_EARNINGS_RATE = "pumpOldEarningsRate";

    // 经典捕鱼阶段表
    public const string PUMP_FISH_TABLE_LOG = "FishTableLog";

    // 礼包
    public const string GIFT = "gift";

    // 礼包码表
    public const string GIFT_CODE = "giftCode";

    // 兑换表
    public const string EXCHANGE = "exchange";

    // 运营公告表
    public const string OPERATION_NOTIFY = "optionNotify";

    // 捕鱼房间
    public const string FISHLORD_ROOM = "fishlord_room";

    // 鳄鱼房间
    public const string CROCODILE_ROOM = "crocodile_room";

    // 骰宝房间
    public const string DICE_ROOM = "dice_room";

    // 重新加载鱼表
    public const string RELOAD_FISHCFG = "fishlord_cfg";

    // 客服信息表
    public const string SERVICE_INFO = "serviceInfo";

    public const string COMMON_CONFIG = "common_config";

    // 游戏充值信息
    public const string GAME_RECHARGE_INFO = "pay_infos";
}

public enum ItemType
{
    e_itd_material = 1,		    //材料

    e_itd_heroChip = 2,		    //碎片//英雄卡牌

    e_itd_equip = 3,	        //装备

    e_itd_consumables = 4,	    //消耗品

    e_itd_gold = 5,			    //金币

    e_itd_gem = 6,			    //钻石

    e_itd_spirit = 7,			//体力	

    e_itd_playerexp = 8,		//玩家经验

    e_itd_heroexp = 9,		    //英雄经验

    e_itd_hero = 10,			//英雄

    e_itd_arena_money = 11,     // 竞技场代币

    e_itd_expedition_money = 12, // 远征代币

    e_itd_guild_money = 13,		 // 公会代币

    e_itd_materialChip = 14,	//材料碎片
}

public struct StrName
{
    // 矿的类型
    public static string[] s_MineName = { "小", "中", "大" };

    // 矿的资源类型
    public static string[] s_resType = { "金币", "药品", "钻石" };

    public static string[] s_rechargeType = { "人民币" };

    public static string[] s_jobName = { "会长", "精英", "普通会员" };

    public static string[] s_statLobbyName = { "全部", "赠送礼物", "小喇叭", "vip等级分布情况", "上传头像",
                                               "昵称修改", "签名修改", "性别修改", "头像框购买", "在线奖励",
                                             "救济金","保险箱存入","保险箱取出"};

    public static string[] s_gameName = { "大厅", "经典捕鱼", "鳄鱼大亨", "欢乐骰宝" };

    public static string[] s_roomName = { "初级场", "中级场", "高级场", "VIP专场" };

    public static string[] s_stageName = { "大天堂", "中天堂", "小天堂", "正常", "小地狱", "中地狱", "大地狱" };
}

public enum PaymentType
{
    e_pt_none = 0,
    e_pt_anysdk,        //anysdk综合
    e_pt_qbao,          //钱宝
    e_pt_max,
}

// 玩家金币，钻石的变化原因
public enum PropertyReasonType
{
    // 每日登录转盘抽奖
	type_reason_dial_lottery = 1,

	// 在线奖励
	type_reason_online_reward = 2,

	// 保险箱存入
	type_reason_deposit_safebox = 3,

	// 保险箱取出
	type_reason_draw_safebox = 4,

	// 赠送礼物
	type_reason_send_gift = 5,

	// 接收礼物
	type_reason_accept_gift = 6,

	// 玩家发小喇叭，全服通告
	type_reason_player_notify = 7,
	
	// 玩家兑换礼物
	type_reason_exchange = 8,

	// 购买商品获得
	type_reason_buy_commodity_gain = 9,

	// 领取救济金
	type_reason_receive_alms = 10,

	// 单局结算
	type_reason_single_round_balance = 11,

	// 购买商品消耗
	type_reason_buy_commodity_expend = 12,
	
	//购买捕鱼等级
	type_reason_buy_fishlevel = 13,

	//购买捕鱼道具
	type_reason_buy_fishitem = 14,

	//捕鱼升级
    type_reason_fish_uplevel = 15,

    // 新手引导
    type_reason_new_guild = 16,

    // 修改头像
	type_reason_update_icon = 17,

	// 充值
	type_reason_recharge = 18,

	// 修改昵称
	type_reason_modify_nickname = 19,

    // 充值赠送
	type_reason_recharge_send = 20,

    // 后台充值
	type_reason_gm_recharge = 21,

	// 后台充值赠送
	type_reason_gm_recharge_send = 22,

    // 月卡每日领取
    type_reason_month_card_daily_recv = 23,

    // 充值礼包
    type_reason_recharge_gift = 24,

    type_max,
};

// 玩家拥有的属性类型
public enum PropertyType
{
    property_type_full,

    // 金币
    property_type_gold,

    // 礼券
    property_type_ticket,
}

public enum DataStatType
{
    // 赠送礼物
    stat_send_gift = 1,

    // 小喇叭
    stat_player_notify,

    // vip等级分布情况
    stat_player_vip_level,

    // 上传头像
    stat_upload_head_icon,

    // 昵称修改
    stat_nickname_modify,

    // 签名修改
    stat_self_signature_modify,

    // 性别修改
    stat_sex_modify,

    // 头像框购买
    stat_photo_frame,

    // 在线奖励
    stat_online_reward,

    // 救济金
    stat_relief,

    // 保险箱存入
    stat_safe_box_deposit,

    // 保险箱取出
    stat_safe_box_draw,

    stat_max,
};

public enum GameId 
{
    lobby = 0,   // 大厅
    fishlord = 1, // 经典捕鱼

    crocodile,    // 鳄鱼大亨

    dice,         // 欢乐骰宝

    gameMax,
}

