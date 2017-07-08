using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

public struct TableName
{
    // 玩家信息表
    public const string PLAYER_INFO = "player_info";

    // 玩家在各个游戏内的投注，盈利情况
    public const string LOG_PLAYER_INFO = "logPlayerInfo";

    // 输赢统计表
    public const string PLAYER_WIN_LOSE = "player_win_lose";

    // 供统计输赢总额的表，玩家登录登出表
    public const string PLAYER_FOR_WIN_LOSE = "playerForWinLoseStat";

    // 邮件表
    public const string PLAYER_MAIL = "playerMail";

    // 邮件检测表
    public const string CHECK_MAIL = "checkMail";

    // 玩家账号表
    public const string PLAYER_ACCOUNT = "AccountTable";

    // GM账号的后缀表
    //public const string GM_ACCOUNT_POSTFIX = "account_postfix";

    // GM账号表
    public const string GM_ACCOUNT = "gm_account";

    // 线下玩家账号表
    public const string PLAYER_ACCOUNT_XIANXIA = "player_account";
    
    // 创建GM账号所需要的数据
    public const string CREATE_CONFIG = "count_config";

    // GM上分，下分表
    public const string GM_SCORE = "log_score";

    // 后台GM登录日志
    public const string GM_LOGIN_LOG = "log_login";

    // 进出游戏的表
    public const string PLAYER_GAME_SCORE = "player_game_score";

    // API号审批表
    public const string API_APPROVE = "api_approve";

    // 等待处理的玩家订单
    public const string PLAYER_ORDER_WAIT = "player_order_wait";

    // 已完成的玩家订单
    public const string PLAYER_ORDER_FINISH = "player_order_finish";

    // 计数表
    public const string COUNT_TABLE = "OpLogCurID_DWC";

    // GM操作日志表
    public const string OPLOG = "log_gm_op";

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
    public const string PUMP_PLAYER_MONEY = "logPlayerInfo";
    // 牛牛生局的牌型表
    public const string PUMP_COWS_CARD = "logCowsInfo";

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

    // 捕鱼每天的收益情况
    public const string PUMP_FISHLORD_EVERY_DAY = "fishlordEveryDay";

    // 鳄鱼每天的收益情况
    public const string PUMP_CROCODILE_EVERY_DAY = "CrocodileEveryday";

    // 骰宝每天的收益情况
    public const string PUMP_DICE_EVERY_DAY = "DiceEveryday";

    // 百家乐每天的收益情况
    public const string PUMP_BACCARAT_EVERY_DAY = "BaccaratEveryday";

    // 鳄鱼公园每天的收益情况
    public const string PUMP_FISHPARK_EVERY_DAY = "fishParkEveryDay";

    // 百家乐玩家上庄情况查询
    public const string PUMP_PLAYER_BANKER = "pumpBaccaratPlayerBanker";
    
    // 五龙每天的收益情况
    public const string PUMP_DRAGON_EVERY_DAY = "DragonEveryday";

    // 黑红梅方每天的收益情况
    public const string PUMP_SHCD_EVERY_DAY = "ShcdCardsEveryday";

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

    // 五龙游戏模式下的盈利率
    public const string DRAGON_TABLE = "dragons_table";

    // 牛牛的牌型表
    public const string COWS_CARDS = "cows_cards";

    // 黑红梅方的结果控制
    public const string SHCD_RESULT = "shcdcards_gm_cards";

    // 重新加载鱼表
    public const string RELOAD_FISHCFG = "fishlord_cfg";
    public const string RELOAD_FISHPARK_CFG = "fishpark_cfg";

    // 百家乐历史记录
    public const string HISTORY_BACCARAT = "HistoryBaccarat";
    // 骰宝历史记录
    public const string HISTORY_DICE = "HistoryDice";
    // 牛牛历史记录
    public const string HISTORY_COWS = "logCowsInfo";
    // 鳄鱼大亨历史记录
    public const string HISTORY_CROCODILE = "HistoryCrocodile";
    // 黑红梅方历史记录
    public const string HISTORY_SHCD = "pumpShcdCardsLog";

    // 客服信息表
    public const string SERVICE_INFO = "serviceInfo";

    public const string COMMON_CONFIG = "common_config";

    // 游戏充值信息
    public const string GAME_RECHARGE_INFO = "pay_infos";

    // 测试游戏表
    public const string TEST_SERVER = "TestServers";

    // 踢出玩家
    public const string KICK_PLAYER = "KickPlayer";

    // 经典捕鱼玩家表
    public const string FISHLORD_PLAYER = "fishlord_player";
    // 鳄鱼公园玩家表
    public const string FISHPARK_PLAYER = "fishpark_player";

    // 玩家请求的待处理订单，存于mongodb
    public const string PLAYER_ORDER_REQ = "playerOrderReq";
    // 玩家在线上下分 已完成订单
    public const string PLAYER_ORDER_COMPLETE = "order_complete";
    // api订单回调缓存
    public const string API_ORDER_CALL = "apiOrderCall";

    // API设置的最大下注限制
    public const string API_MAX_BET_LIMIT = "apiMaxBetLimit";

    public const string API_MAX_BET_SETTING_LIMIT = "apiMaxBetSettingLimit";

    // 玩家拥有的总金币统计日
    public const string TOTAL_MONEY_STAT_DAY = "statTimeByDay";
}

public class GameInfo
{
    public string m_gameName;
    public int m_gameId;

    public GameInfo(string name, int id)
    {
        m_gameName = name;
        m_gameId = id;
    }
}

// 币种信息
public class MoneyInfo
{
    public string m_moneyCode;
    public string m_moneyDesc;

    public MoneyInfo(string code, string desc)
    {
        m_moneyCode = code;
        m_moneyDesc = desc;
    }
}

public static class StrName
{
    public static string[] s_rechargeType = { "人民币" };

    public static string[] s_statLobbyName = { "全部", "赠送礼物", "小喇叭", "vip等级分布情况", "上传头像",
                                               "昵称修改", "签名修改", "性别修改", "头像框购买", "在线奖励",
                                             "救济金","保险箱存入","保险箱取出"};

    public static string[] s_gameName = { "大厅", "经典捕鱼", "鳄鱼大亨", "欢乐骰宝", "万人牛牛", "百家乐", "五龙", "套牛", "抓姓姓", "捕鱼", "黑红梅方", "", "", "试玩场" };

    public static string[] s_gameName1 = { "系统", s_gameName[1], s_gameName[2], s_gameName[3], s_gameName[4], s_gameName[5], s_gameName[6], s_gameName[7], s_gameName[8], s_gameName[9] };

    public static string[] s_roomName = { "初级场", "中级场", "高级场", "VIP专场" };

    public static string[] s_dragonRoomName = { "初级场", "高级场", "大师场" };

    public static string[] s_stageName = { "大天堂", "中天堂", "小天堂", "正常", "小地狱", "中地狱", "大地狱" };

    private static Dictionary<string, string> s_gameName2 = new Dictionary<string, string>();

    public static string[] s_cowsArea = { "东", "南", "西", "北" };

    public static string[] s_shcdArea = { "黑桃", "红心", "梅花", "方块", "大小王" };
    public static string[] s_dragonArea = { "最终倍率", "福袋倍率", "开花倍率" };

   // public static string[] s_moneyType = { "CNY", "USD" };

    public static string[] s_logFrom = { "", "后台操作", "API订单", "玩家订单" };

    public static string[] s_accountType = { "超级管理员", 
                                             "总代理", "代理", "API号",
                                             "代理子账号", "会员", "API管理员",
                                           "管理员子账号"};

    public static string s_rightDesc = "创建下级代理:{0}<br/>创建API号:{1}";

    public static string[] s_stateName = { "离线", "在线", "已停封" };

    public static string[] s_gmStateName = { "正常", "停用" };

    public static string[] s_scoreOpName = { "存入", "提出" };

    public static string[] s_playerOrderIdName = { "充值", "提款" };

    public static string[] s_playerOrderState = { "完成", "待处理", "已取消", "玩家在线 已提交" };

    public static string[] s_wishCurse = { "祝福", "诅咒" };

    public static string[] s_realTimeOrderState = { "成功", "失败" };

    private static Dictionary<int, string> s_realTimeOrderFailReason = new Dictionary<int, string>();

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

    public static List<GameInfo> s_gameList = new List<GameInfo>();

    static StrName()
    {
        addGame((int)GameId.crocodile);
        addGame((int)GameId.dice);
        addGame((int)GameId.cows);
        addGame((int)GameId.baccarat);
        addGame((int)GameId.dragon);
        addGame((int)GameId.fishpark);
        addGame((int)GameId.shcd);
       // initMoneyInfo();
        initRealTimeOrderFailReason();
    }

    static void addGame(int id)
    {
        GameInfo info = new GameInfo(s_gameName[id], id);
        s_gameList.Add(info);
    }

    public static List<MoneyInfo> s_moneyInfo = new List<MoneyInfo>();

    static void initMoneyInfo()
    {
        addMoney("CNY", "人民币CNY");
        addMoney("USD", "美元USD");
        addMoney("HKD", "港币HKD");
        addMoney("KRW", "韩元KRW");
        addMoney("MYR", "马来西亚币MYR");

        addMoney("SGD", "新加坡币SGD");
        addMoney("JPY", "日元JPY");
        addMoney("THB", "泰铢THB");
        addMoney("BTC", "比特币BTC");

        addMoney("IDR", "印尼盾IDR");
        addMoney("VND", "越南盾VND");
        addMoney("EUR", "欧元EUR");
        addMoney("AUD", "澳元AUD");
        addMoney("GBP", "英镑GBP");

        addMoney("CHF", "瑞士元CHF");
        addMoney("MXP", "墨西哥比索MXP");
        addMoney("CAD", "加拿大元CAD");
        addMoney("RUB", "俄罗斯卢布RUB");
        addMoney("INR", "印度卢比INR");

        addMoney("RON", "罗马尼亚币RON");
        addMoney("DKK", "丹麦克朗DKK");
        addMoney("NOK", "挪威克朗NOK");
    }

    static void addMoney(string code, string desc)
    {
        MoneyInfo info = new MoneyInfo(code, desc);
        s_moneyInfo.Add(info);
    }

    public static string getMoneyCode(int moneyType)
    {
        return "";

        if (moneyType < 0 || moneyType >= s_moneyInfo.Count)
            return "";

        return s_moneyInfo[moneyType].m_moneyCode;
    }

    public static string getMoneyDesc(int moneyType)
    {
        if (moneyType < 0 || moneyType >= s_moneyInfo.Count)
            return "";

        return s_moneyInfo[moneyType].m_moneyDesc;
    }

    static void initRealTimeOrderFailReason()
    {
        s_realTimeOrderFailReason.Add(RetCode.RET_MONEY_NOT_ENOUGH, "余额不足");
        s_realTimeOrderFailReason.Add(RetCode.RET_PLAYER_NOT_IN_LOBBY, "玩家不在大厅");
        s_realTimeOrderFailReason.Add(RetCode.RET_PLAYER_OFFLINE, "玩家离线");
        s_realTimeOrderFailReason.Add(RetCode.RET_NO_PLAYER, "玩家不存在");
        s_realTimeOrderFailReason.Add(RetCode.RET_DB_ERROR, "db错误");
        s_realTimeOrderFailReason.Add(RetCode.RET_PLYAER_LOCKED, "玩家被锁定");
    }

    public static string getRealTimeOrderFailReason(int reason)
    {
        if (s_realTimeOrderFailReason.ContainsKey(reason))
            return s_realTimeOrderFailReason[reason];

        return reason.ToString();
    }
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

   //五龙翻倍游戏消耗
	type_reason_double_game = 36,

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

    cows,         // 万人牛牛
    
    baccarat,     // 百家乐

    dragon,       // 五龙

    calf_roping,   // 套牛
    prize_claw,    // 抓姓姓

    fishpark,     // 鳄鱼公园
    shcd,         // 黑红梅方
    fish_practice = 13,         // 捕鱼试玩场
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
};

public struct PlayerState
{
    // 空闲，正常
    public const int STATE_IDLE = 0;

    // 游戏中
    public const int STATE_GAME = 1;

    // 账号被停封
    public const int STATE_BLOCK = 2;
}

public struct GmState
{
    // 正常
    public const int STATE_NORMAL = 0;

    // 停用
    public const int STATE_BLOCK = 1;
}

public struct OrderState
{
    // 已完成
    public const int STATE_FINISH = 0;

    // 等待处理
    public const int STATE_WAIT = 1;

    // 已取消
    public const int STATE_CANCEL = 2;

    // 玩家在线，订单已提交处理
    public const int STATE_HAS_SUB = 3;
}

public struct AccType
{
    // 超级管理员
    public const int ACC_SUPER_ADMIN = 0;

    // 总代理
    public const int ACC_GENERAL_AGENCY = 1;

    // 代理
    public const int ACC_AGENCY = 2;

    // API号
    public const int ACC_API = 3;

    // 代理子账号
    public const int ACC_AGENCY_SUB = 4;

    // 玩家
    public const int ACC_PLAYER = 5;

    // API管理员
    public const int ACC_API_ADMIN = 6;

    // 管理员子账号
    public const int ACC_SUPER_ADMIN_SUB = 7;
}

public struct ScropOpType
{
    // 存入
    public const int ADD_SCORE = 0;
    // 提取
    public const int EXTRACT_SCORE = 1;

    public static bool isAddScore(int opType)
    {
        return opType == ADD_SCORE;
    }
}

public struct SqlStrCMD
{
    public const string SQL_COUNT_INT = "SELECT COUNT(*) as cnt FROM {0} WHERE {1}={2}";

    public const string SQL_COUNT_CHAR = "SELECT COUNT(*) as cnt FROM {0} WHERE {1}='{2}'";

    public const string SQL_COUNT_WHERE = "SELECT COUNT(*) as cnt FROM {0} WHERE {1}";

    public const string SQL_COUNT_NO_WHERE = "SELECT COUNT(*) as cnt FROM {0} {1}";

//    public const string SQL_CREATE_GM_ACCOUNT = "INSERT into {0} (acc,pwd,accType,createTime,owner,generalAgency,postfix,money,moneyType,devSecretKey,gmRight,depth,createCode,aliasName) " +
//        " VALUES ('{1}','{2}', {3},'{4}','{5}','{6}','{7}',{8},{9},'{10}','{11}',{12},'{13}','{14}') ";

//     public const string SQL_CREATE_PLAYER_ACCOUNT =
//         "INSERT into {0} (acc,creator,money,moneyType,state,createTime,createCode,aliasName)" +
//         " VALUES ('{1}','{2}',{3},{4},{5},'{6}','{7}','{8}')";

    // 查询创建的玩家会员{0}玩家账号表 {1}gm账号表
    public const string SQL_QUERY_PLAYER_MEMBER = "SELECT " +
                        " {0}.acc as playerAcc,{0}.sellerAdmin,{0}.createTime, {0}.money, {1}.owner as seller, {1}.moneyType  from " +
                        " {0}, {1} " +
                        " where {0}.sellerAdmin={1}.acc {2} " +
                        " LIMIT {3}, {4} ";

    // 按时间统计，存款，提款统计
    public const string STR_FTM1 = "SELECT date_format(opTime,'{0}' ) as dtime, COUNT(*) as cnt,  SUM(opScore) as sum, opType " +
                                    " from {1}" +
                                    " where {2} " +
                                    " GROUP BY dtime, opType ORDER BY opTime";

    // 按售货亭统计
    public const string STR_FTM2 = "SELECT COUNT(*) as cnt,  SUM(opScore) as sum, opType,opSrc " +
                                    " from {0}" +
                                    " where opSrc='{1}' and opTime >= '{2}' and opTime < '{3}' " +
                                    " GROUP BY opType,opSrc";

    // 按售货亭统计
    public const string SQL_STAT_STEP = "SELECT COUNT(*) as cnt,  SUM(opScore) as sum, opType,opSrc " +
                                        " from {0}" +
                                        " where {1} " +
                                        " GROUP BY opType,opSrc";

    // 按售货亭管理统计
    public const string SQL_SELLER_STAT_ADMIN = "SELECT COUNT(*) as cnt,  SUM(opScore) as sum, opType,opDst " +
                                    " from {0}" +
                                    " where opTime >= '{1}' and opTime < '{2}' and " +
                                    " opDst in (SELECT {3}.acc from {3} where {3}.owner='{4}' and {3}.accType={5} ) " +
                                    " GROUP BY opType,opDst";

    // 查询GM账号
    public const string SQL_QUERY_GM_ACCOUNT = " SELECT * from {0}" +
                                               " where {1} " +
                                               " LIMIT {2}, {3} ";

    public const string SQL_CMD_PLAYER_SCORE = "INSERT into {0} (opTime,opSrc,opDst,opType,opScore,moneyType,opSrcDepth,opSrcCreateCode,opDstType,opDstRemainMoney) " +
                                                " VALUES ('{1}','{2}', '{3}',{4},{5},{6},{7},'{8}',{9},{10})";

    // 给玩家上分
    public const string SQL_ADD_SCORE_TO_PLAYER = " UPDATE {0} set money=money+{1} where acc='{2}' and state={3} and creator='{4}' ";
    // 给玩家上分 越级
    public const string SQL_ADD_SCORE_TO_PLAYER_BYPASS = " UPDATE {0} set money=money+{1} where acc='{2}' and state={3} ";

    // 给玩家下分
    public const string SQL_DEC_SCORE_TO_PLAYER = " UPDATE {0} set money=money-{1} where acc='{2}' and state={3} and money>={4} and creator='{5}' ";
    // 给玩家下分 越级
    public const string SQL_DEC_SCORE_TO_PLAYER_BYPASS = " UPDATE {0} set money=money-{1} where acc='{2}' and state={3} and money>={4} ";

    // 给管理上分
    public const string SQL_ADD_SCORE_TO_MGR = " UPDATE {0} set money=money+{1} where acc='{2}' ";

    // 给管理下分
    public const string SQL_DEC_SCORE_TO_MGR = " UPDATE {0} set money=money-{1} where acc='{2}' and money>={3} ";

    // 给管理上分
    public const string SQL_ADD_SCORE_TO_MGR_DIRECT = " UPDATE {0} set money=money+{1} where acc='{2}' ";

    // 给管理下分
    public const string SQL_DEC_SCORE_TO_MGR_DIRECT = " UPDATE {0} set money=money-{1} where acc='{2}' and money>={3} ";

    // 更改密码
    public const string SQL_UPDATE_PWD = " UPDATE {0} set pwd='{1}' where acc='{2}' and owner='{3}' ";

    public const string SQL_UPDATE_PWD_DIRECT = " UPDATE {0} set pwd='{1}' where acc='{2}' ";

    // 上下分记录查询
    public const string SQL_QUERY_SCORE_OP = " SELECT opId,opTime,opSrc,opType,opScore, opDst,{0}.moneyType,aliasName,opDstRemainMoney,opRemainMoney," +
                                         " userOrderId,opResult,failReason,logFrom,finishTime,orderId " +
                                         " from {0},{1} " +
                                           " {2} " +
                                           " order by opTime desc LIMIT {3}, {4} ";
}

public struct RetCode
{
    // 成功
    public const int RET_SUCCESS = 0;

    // 参数非法
    public const int RET_PARAM_NOT_VALID = 1;

    // 操作失败
    public const int RET_OP_FAILED = 2;

    // GM登录失败
    public const int RET_GM_LOGIN_FAILED = 3;

    // 权限不够
    public const int RET_NO_RIGHT = 4;

    // 余额不足
    public const int RET_MONEY_NOT_ENOUGH = 5;

    // 数据库错误
    public const int RET_DB_ERROR = 6;

    // 玩家不在线
    public const int RET_PLAYER_OFFLINE = 7;

    // 玩家在线
    public const int RET_PLAYER_ONLINE = 8;

    // 玩家不存在
    public const int RET_NO_PLAYER = 9;

    // 签名错误
    public const int RET_SIGN_ERROR = 10;

    // 玩家没有被锁定
    public const int RET_PLYAER_NOT_LOCKED = 11;

    // 账号已存在
    public const int RET_ACCOUNT_HAS_EXISTS = 12;

    // 账号或密码格式错误
    public const int RET_ACC_PWD_FORMAT_ERROR = 13;

    // 账号被停封
    public const int RET_ACC_BLOCKED = 14;

    // 找不到上级账号
    public const int RET_NO_SUP_ACC = 15;

    // 金额不合法
    public const int RET_MONEY_NOT_VALID = 16;

    // 玩家不在大厅
    public const int RET_PLAYER_NOT_IN_LOBBY = 17;

    // 玩家被锁定，需要解锁
    public const int RET_PLYAER_LOCKED = 18;

    // 玩家在线，已提交上下分订单请求
    public const int RET_HAS_SUBMIT_ORDER = 19;

    // 订单已存在
    public const int RET_ORDER_EXISTS = 20;
}

//////////////////////////////////////////////////////////////////////////
// 常量定义
public struct ConstDef
{
    // 24小时时间格式
    public static string DATE_TIME24 = "yyyy-MM-dd HH:mm:ss";

    // 洗码比的最大值
    public const double MAX_WASH_RATIO = 0.012;

    // 代理占成的最大值
    public const double MAX_AGENT_RATIO = 1.0;

    // gm ID号的偏移
    public const long GM_ID_OFFSET = 10000;
}

//////////////////////////////////////////////////////////////////////////
// 玩家实时上下分，订单的请求状态
public struct PlayerReqOrderState
{
    // 订单已成功完成
    public const int STATE_FINISH = 0;

    // 订单处理失败
    public const int STATE_FAILED = 1;

    // 等待处理
    public const int STATE_WAIT = 2;

    // 订单处理中
    public const int STATE_PROCESSING = 3;
}

/*
 *     订单处理失败的原因有
 *     
 *     余额不足  玩家不在大厅  玩家不在线  没有该玩家 db出错  金额不合法 玩家被锁定
 *     
 *     具体值参考RetCode中定义的常量
 */
