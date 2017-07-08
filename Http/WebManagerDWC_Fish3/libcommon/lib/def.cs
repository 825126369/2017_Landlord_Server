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

    // 任务表
    public const string PLAYER_QUEST = "player_quest";

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
    // 牛牛生局的牌型表
    public const string PUMP_COWS_CARD = "logCowsInfo";

    // 玩家金币，钻石变化详细表
    public const string PUMP_PLAYER_MONEY_DETAIL = "logPlayerInfo";

    // 每日任务表
    public const string PUMP_DAILY_TASK = "pumpDailyTask";

    // 成就
    public const string PUMP_TASK = "pumpTask";

    // 邮件
    public const string PUMP_MAIL = "pumpMail";

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

    // 金币增长排行
    public const string PUMP_COIN_GROWTH = "pumpCoinGrowth";

    // 金币增长历史排行
    public const string PUMP_COIN_GROWTH_HISTORY = "pumpCoinGrowthHistory";

    // 经典捕鱼消耗表
    public const string FISH_CONSUME = "pumpFishConsume";
    // 锁定、急速、散射的消耗
    public const string FISH_CONSUME_ITEM = "pumpFishItemConsume";

    // 捕鱼每天的收益情况
    public const string PUMP_FISHLORD_EVERY_DAY = "fishlordEveryDay";

    // 鳄鱼每天的收益情况
    public const string PUMP_CROCODILE_EVERY_DAY = "CrocodileEveryday";

    // 骰宝每天的收益情况
    public const string PUMP_DICE_EVERY_DAY = "DiceEveryday";

    // 百家乐每天的收益情况
    public const string PUMP_BACCARAT_EVERY_DAY = "BaccaratEveryday";

    // 五龙每天的收益情况
    public const string PUMP_DRAGON_EVERY_DAY = "DragonEveryday";

    // 鳄鱼公园每天的收益情况
    public const string PUMP_FISHPARK_EVERY_DAY = "fishParkEveryDay";

    // 黑红梅方每天的收益情况
    public const string PUMP_SHCD_EVERY_DAY = "ShcdCardsEveryday";

    // 套牛每天的收益情况
    public const string PUMP_CALFROPING_EVERY_DAY = "ropingEveryDay";

    // 百家乐玩家上庄情况查询
    public const string PUMP_PLAYER_BANKER = "pumpBaccaratPlayerBanker";

    // 牛牛每天的收益情况
    public const string PUMP_COWS_EVERY_DAY = "CowsEveryday";

    // 牛牛玩家上庄情况查询
    public const string PUMP_PLAYER_BANKER_COWS = "pumpCowsPlayerBanker";

    // 鳄鱼下注及获奖次数
    public const string PUMP_CROCODILE_BET = "CrocodileBetInfo";

    // 骰宝数据下注及获奖情况
    public const string PUMP_DICE = "dice_table";

    // 鱼的统计
    public const string PUMP_ALL_FISH = "AllFishLog";
    // 鳄鱼公园鱼的统计
    public const string PUMP_ALL_FISH_PARK = "AllFishParkLog";

    // 重置时旧的盈利率
    public const string PUMP_OLD_EARNINGS_RATE = "pumpOldEarningsRate";

    // 经典捕鱼阶段表
    public const string PUMP_FISH_TABLE_LOG = "FishTableLog";
    // 鳄鱼公园阶段表
    public const string PUMP_FISH_PARK_TABLE_LOG = "FishParkTableLog";

    // 最高在线玩家
    public const string PUMP_MAXONLINE_PLAYER = "pumpMaxOnlinePlayer";

    // 每日0点所有玩家的金币总和统计表
    public const string PUMP_PLAYER_TOTAL_MONEY = "pumpPlayerTotalMoney";

    // 兑换统计
    public const string PUMP_EXCHANGE = "pumpExchange";

    // 付费点统计
    public const string PUMP_RECHARGE = "pumpRecharge";

    // 星星抽奖
    public const string PUMP_STAR_LOTTERY = "pumpStarLottery";

    // 充值用户统计
    public const string PUMP_RECHARGE_PLAYER = "pumpRechargePlayer";

    // 玩家一天内各个游戏内，游戏时间累计
    public const string PUMP_GAME_TIME_FOR_PLAYER = "pumpGameTimeForPlayer";

    // 新增玩家发炮次数,捕鱼等级
    public const string PUMP_NEW_PLAYER_FIRECOUNT_FISHLEVEL = "pumpNewPlayerFireCountFishLevel";

    // 礼包
    public const string GIFT = "gift";

    // 礼包码表
    public const string GIFT_CODE = "giftCode";

    // 兑换表
    public const string EXCHANGE = "exchange";

    // 运营公告表
    public const string OPERATION_NOTIFY = "optionNotify";

    // 通告消息
    public const string OPERATION_SPEAKER = "operationSpeaker";

    // 捕鱼房间
    public const string FISHLORD_ROOM = "fishlord_room";
    // 鳄鱼公园房间
    public const string FISHPARK_ROOM = "fishpark_room";

    // 捕鱼桌子
    public const string FISHLORD_ROOM_DESK = "fishlord_table";
    // 鳄鱼公园桌子
    public const string FISHPARK_ROOM_DESK = "fishpark_table";

    // 鳄鱼房间
    public const string CROCODILE_ROOM = "crocodile_room";

    // 骰宝房间
    public const string DICE_ROOM = "dice_room";

    // 百家乐房间
    public const string BACCARAT_ROOM = "baccarat_room";

    // 牛牛房间
    public const string COWS_ROOM = "cows_room";

    // 五龙房间
    public const string DRAGON_ROOM = "dragons_room";

    // 黑红梅方房间
    public const string SHCDCARDS_ROOM = "shcdcards_room";

    // 套牛房间
    public const string CALF_ROPING_ROOM = "calfRoping_lobby";
    // 套牛 牛的分类 统计
    public const string CALF_ROPING_LOG = "ropingLog";
    // 套牛关卡统计
    public const string CALF_ROPING_PASS_LOG = "ropingPassLog";

    // 五龙游戏模式下的盈利率
    public const string DRAGON_TABLE = "dragons_table";

    // 牛牛的牌型表
    public const string COWS_CARDS = "cows_cards";

    // 黑红梅方的结果控制
    public const string SHCD_RESULT = "shcdcards_gm_cards";

    // 重新加载鱼表
    public const string RELOAD_FISHCFG = "fishlord_cfg";
    public const string RELOAD_FISHPARK_CFG = "fishpark_cfg";

    // 客服信息表
    public const string SERVICE_INFO = "serviceInfo";

    public const string COMMON_CONFIG = "common_config";

    // 游戏充值信息
    public const string GAME_RECHARGE_INFO = "pay_infos";

    // 测试游戏表
    public const string TEST_SERVER = "TestServers";

    // 经典捕鱼玩家表
    public const string FISHLORD_PLAYER = "fishlord_player";
    // 鳄鱼公园玩家表
    public const string FISHPARK_PLAYER = "fishpark_player";

    // 头像举报
    public const string INFORM_HEAD = "informHead";

    // 踢出玩家
    public const string KICK_PLAYER = "KickPlayer";

    public const string DAY_ACTIVATION = "day_activation";
    // 渠道数据统计日
    public const string CHANNEL_STAT_DAY = "channelStatDay";
    // 渠道相关的统计数据
    public const string CHANNEL_TD = "channelTalkingData";
    // 渠道相关的充值统计
    public const string CHANNEL_TD_PAY = "channelTalkingDataPay";

    // 玩家拥有的总金币统计日
    public const string TOTAL_MONEY_STAT_DAY = "totalMoneyStatDay";

    // VIP流失
    public const string RLOSE = "vipLose";

    // 大奖赛周冠军
    public const string MATCH_GRAND_PRIX_WEEK_CHAMPION = "fishlord_match_champion";
    // boss记录
    public const string PUMP_BOSSINFO = "logBossInfo";

    public const string MATCH_GRAND_PRIX_DAY = "fishlord_match_day";

    // 安全账号列表
    public const string MATCH_GRAND_SAFE_ACCOUNT = "fishlord_match_safe_account";

    // 玩家龙珠统计
    public const string STAT_PLAYER_DRAGON = "statPlayerDragonBall";
    // 每日龙珠总计
    public const string STAT_DRAGON_DAILY = "statDragonBallDaily";

    // 玩家付费监控
    public const string PUMP_RECHARGE_FIRST = "pumpRechargeFirst";
    // 玩家累计的游戏时间
    public const string STAT_PLAYER_GAME_TIME = "statPlayerGameTime";

    // 玩家在线时间段
    public const string PUMP_PLAYER_ONLINE_TIME = "pumpPlayerOnlineTime";

    // 收支总计
    public const string STAT_INCOME_EXPENSES = "statIncomeExpenses";
    // 每天收支的总数据库结余
    public const string STAT_INCOME_EXPENSES_REMAIN = "statIncomeExpensesRemain";

    // 收支总计 新
    public const string STAT_INCOME_EXPENSES_NEW = "statIncomeExpensesNew";

    // 每小时收入统计
    public const string STAT_RECHARGE_HOUR = "statRechargeHour";
    // 每小时在线人数
    public const string STAT_ONLINE_HOUR = "statOnlinePlayerNumHour";

    // 活跃行为--用户喜好 在线时间
    public const string STAT_GAME_TIME_FOR_PLAYER_FAVOR_RESULT = "statGameTimeForPlayerFavorResult";
    // 时长分布
    public const string STAT_GAME_TIME_FOR_DISTRIBUTION_RESULT = "statGameTimeForDistributionResult";
    // 首付游戏时长分布
    public const string STAT_FIRST_RECHARGE_GAME_TIME_DISTRIBUTION_RESULT = "statFirstRechargeGameTimeDistributionResult";
    // 首次购买计费点分布
    public const string STAT_FIRST_RECHARGE_POINT_DISTRIBUTION_RESULT = "statFirstRechargePointDistributionResult";
    // 玩家下注情况统计
    public const string STAT_PLAYER_GAME_BET_RESULT = "statPlayerGameBetResult";
    // 当日新增用户金币下注分布
    public const string STAT_NEW_PLAYER_OUTLAY_DISTRIBUTION = "statNewPlayerOutlayDistributionResult";

    public const string STAT_NEW_PLAYER_ENTER_ROOM = "pumpNewPlayerGame";
    // 新增玩家发炮次数分布
    public const string STAT_NEW_PLAYER_FIRECOUNT_DISTRIBUTION = "statNewPlayerFireCountDistributionResult";
    // 新增玩家发捕鱼等级分布
    public const string STAT_NEW_PLAYER_FISHLEVEL_DISTRIBUTION = "statNewPlayerFishLevelDistributionResult";

    //////////////////////////////////////////////////////////////////////////
    // GM账号类型分组
    public const string GM_TYPE = "gmTypeGroup";
}

public static class StrName
{
    public static string[] s_rechargeType = { "人民币" };

    public static string[] s_statLobbyName = { "全部", "赠送礼物", "小喇叭", "vip等级分布情况", "上传头像",
                                               "昵称修改", "签名修改", "性别修改", "头像框购买", "在线奖励",
                                             "救济金","保险箱存入","保险箱取出"};

    public static string[] s_gameName = { "大厅", "经典捕鱼", "鳄鱼大亨", "欢乐骰宝", "万人牛牛", "百家乐", "五龙", "套牛", 
                                            "抓姓姓", "鳄鱼公园", "黑红梅方" };

    public static string[] s_gameName1 = { "系统", s_gameName[1], s_gameName[2], s_gameName[3], 
                                             s_gameName[4], s_gameName[5], s_gameName[6],
                                             s_gameName[7], s_gameName[8], s_gameName[9],s_gameName[10] };

    public static string[] s_roomName = { "初级场", "中级场", "高级场", "VIP专场" };

    public static string[] s_shcdRoomName = { "", "金币场", "龙珠场" };

    public static string[] s_fishRoomName = { s_roomName[0], s_roomName[1], s_roomName[2], s_roomName[3], 
                                                "普通赛初级场", "普通赛中级场", "普通赛高级场", "普通赛大师场", "大奖赛" };

    public static string[] s_dragonRoomName = { "初级场", "高级场", "大师场" };

    public static string[] s_stageName = { "大天堂", "中天堂", "小天堂", "正常", "小地狱", "中地狱", "大地狱" };

    private static Dictionary<string, string> s_gameName2 = new Dictionary<string, string>();

    public static string[] s_cowsArea = { "东", "南", "西", "北" };

    public static string[] s_shcdArea = { "黑桃", "红心", "梅花", "方块", "大小王" };
    
    public static string[] s_dragonArea = { "最终倍率", "福袋倍率", "开花倍率" };

    public static string[] s_wishCurse = { "祝福", "诅咒" };

    // 当前上线的游戏ID列表
    public static int[] s_onlineGameIdList = {0, (int)GameId.fishlord, (int)GameId.crocodile, (int)GameId.cows, 
                                             (int)GameId.dragon,(int)GameId.shcd};

    public static string getGameName(string key)
    {     
        if (s_gameName2.Count == 0)
        {
            s_gameName2.Add("lobby", s_gameName[0]);
            s_gameName2.Add("fish", s_gameName[1]);
            s_gameName2.Add("crocodile", s_gameName[2]);
            s_gameName2.Add("dice", s_gameName[3]);
            s_gameName2.Add("Cows", s_gameName[4]);
            s_gameName2.Add("baccarat", s_gameName[5]);
        }

        if (s_gameName2.ContainsKey(key))
            return s_gameName2[key];
        return key;
    }
}

// public enum PaymentType
// {
//     e_pt_none = 0,
//     e_pt_anysdk,        //anysdk综合
//     e_pt_qbao,          //钱宝
//     e_pt_baidu,         //百度
//     e_pt_max,
// }

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

    // 每日签到
	type_reason_daily_sign = 25,

	// 每日宝箱抽奖
	type_reason_daily_box_lottery = 26,

	// 谢谢参与兑换
	type_reason_thank_you_exchange = 27,

	// 连续发小喇叭
	type_reason_continuous_send_speaker = 28,

	// 领取邮件
	type_reason_receive_mail = 29,

    // 捕鱼掉落
	type_reason_fishlord_drop = 30,

    // 创建账号
	type_reason_create_account = 31,

    // 领取活动奖励
	type_reason_receive_activity_reward = 32,

    // 百家乐抢庄
	type_reason_rob_banker = 33,
	
	// 百家乐提前下庄
	type_reason_leave_banker = 34,

	// 使用技能
    type_reason_use_skill = 35,

    //五龙翻倍游戏使用钻石
	type_reason_double_game = 36,
	
	//五龙升级 //钻石收入和金币支出
	type_reason_dragons_lv = 37,	

	//星星奖池
	type_reason_star_award = 38,
	//星星抽奖	
	type_reason_star_lottery = 39,

	//新手礼包
	type_reason_new_player = 40,

    //任务
	type_reason_daily_task = 41,
	//成就
	type_reason_achievement = 42,

    // 导弹产出
    type_reason_missile = 43,

    // 充值抽奖
    type_reason_recharge_lottery = 44,

    // 引导充值礼包
	type_reason_recharge_guide_gift = 45,

    //活跃开宝箱
	type_reason_active_box = 46,

	// 玩小游戏兑换而来
	type_reason_play_game = 47,

	// VIP福利
	type_reason_get_vipgold = 48,

	// 比赛门票
	type_reason_match_ticket = 49,
    
    // 后台操作
    type_reason_gm_op = 50,
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

    // 话费碎片
    property_type_chip = 11,

    // 龙珠币
    property_type_dragon_ball = 14,	
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

    cows,         // 万人牛牛
    
    baccarat,     // 百家乐
    
    dragon,       // 五龙

    calf_roping,   // 套牛
    prize_claw,    // 抓姓姓

    fishpark,     // 鳄鱼公园

    shcd,         // 黑红梅方

    gameMax,
}

//////////////////////////////////////////////////////////////////////////

// 捕鱼消耗类型
public enum FishLordExpend
{
    fish_buyitem_start,			//购买物品 Fish_ItemCFG
    fish_buyitem_end = fish_buyitem_start + 31,

    fish_useskill_start = 100,	//使用技能 Fish_BuffCFG
    fish_useskill_end = fish_useskill_start + 10,

    fish_turrent_uplevel_start = 150,         // 炮台升级开始
    fish_turrent_uplevel_end = fish_turrent_uplevel_start + 55,

    fish_unlock_level_start = 300,
    fish_unlock_level_end = fish_unlock_level_start + 55,

    // 导弹消耗
    fish_missile = 500,
    fish_missile_end = 500 + 3,
};

public struct PlayerType
{
    public const int TYPE_ACTIVE = 1;          // 活跃用户
    public const int TYPE_RECHARGE = 2;        // 付费用户
    public const int TYPE_NEW = 3;             // 新增用户
}

// 支付类型
public struct PayType
{
    // 使用公众号方式充值
    public const int WeChatPublicNumer = 1;
}

//////////////////////////////////////////////////////////////////////////
public class GameStatData
{
    // 当日注册人数
    public int m_regeditCount;

    // 当日设备激活数量
    public int m_deviceActivationCount;

    // 活跃人数
    public int m_activeCount;

    // 当天总收入
    public int m_totalIncome;

    // 付费人数
    public int m_rechargePersonNum;

    // 付费次数
    public int m_rechargeCount;

    // 当日新增的用户付费总计
    public int m_newAccIncome;
    // 当日新增的用户中，付费用户数量
    public int m_newAccRechargePersonNum;

    // 留存率计算时的总注册人数
    //public int m_2DayRegeditCount;

    // 次日留存人数
    public int m_2DayRemainCount;

    //public int m_3DayRegeditCount;

    // 3日留存人数
    public int m_3DayRemainCount;

    //public int m_7DayRegeditCount;

    // 7日留存人数
    public int m_7DayRemainCount;

    //public int m_30DayRegeditCount;

    // 30日留存人数
    public int m_30DayRemainCount;
    
    // 1日总充值， -1表示还没有数据
    public int m_1DayTotalRecharge = -1;
    // 3日总充值， -1表示还没有数据
    public int m_3DayTotalRecharge = -1;
    // 7日总充值， -1表示还没有数据
    public int m_7DayTotalRecharge = -1;
    // 14日总充值， -1表示还没有数据
    public int m_14DayTotalRecharge = -1;
    // 30日总充值， -1表示还没有数据
    public int m_30DayTotalRecharge = -1;
    // 60日总充值， -1表示还没有数据
    public int m_60DayTotalRecharge = -1;
    // 90日总充值， -1表示还没有数据
    public int m_90DayTotalRecharge = -1;

    //////////////////////////////////////////////////////////////////////////
    // 次日设备留存人数，临时数据
    public int m_2DayDevRemainCount = -1;

    // 3日设备留存人数
    public int m_3DayDevRemainCount = -1;

    // 7日设备留存人数
    public int m_7DayDevRemainCount = -1;

    // 30日设备留存人数
    public int m_30DayDevRemainCount = -1;

    //////////////////////////////////////////////////////////////////////////
}

//////////////////////////////////////////////////////////////////////////

public class ResultRPlayerItem
{
    public int m_playerId;
    public int m_rechargeCount;
    public int m_rechargeMoney;
    public int m_loginCount;
    public Dictionary<int, int> m_games = new Dictionary<int, int>();

    public DateTime m_regTime;
    public DateTime m_lastLoginTime;
    public int m_remainGold;
    public int m_mostGold;

    public void addEnterCount(int gameId, int count)
    {
        m_games.Add(gameId, count);
    }

    public int getEnterCount(int gameId)
    {
        if (m_games.ContainsKey(gameId))
        {
            return m_games[gameId];
        }

        return 0;
    }
}
