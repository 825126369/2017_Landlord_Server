using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

public enum RechargeType
{
    // 充人民币
    rechargeRMB,

    // 删除自定义头像
    delIconCustom,

    gold,   // 金币
    gem,    // 钻石
    vipExp,  // VIP经验
    dragonBall, // 龙珠
};


public struct DefCC
{
    // 详情查看页
    public const string ASPX_GAME_DETAIL = "/appaspx/stat/gamedetail/GameDetailViewer.aspx";
    // 百家乐详情
    public const string ASPX_GAME_DETAIL_BACCARAT = "/appaspx/stat/gamedetail/GameDetailBaccarat.aspx?index={0}";
    // 牛牛详情
    public const string ASPX_GAME_DETAIL_COWS = "/appaspx/stat/gamedetail/GameDetailCows.aspx?index={0}";
    // 鳄鱼大亨详情
    public const string ASPX_GAME_DETAIL_CROCODILE = "/appaspx/stat/gamedetail/GameDetailCrocodile.aspx?index={0}";
    // 骰宝详情
    public const string ASPX_GAME_DETAIL_DICE = "/appaspx/stat/gamedetail/GameDetailDice.aspx?index={0}";
    // 鳄鱼公园详情
    public const string ASPX_GAME_DETAIL_FISH_PARK = "/appaspx/stat/gamedetail/GameDetailFishPark.aspx?index={0}";
    // 五龙详情
    public const string ASPX_GAME_DETAIL_DRAGON = "/appaspx/stat/gamedetail/GameDetailDragon.aspx?index={0}";

    // 扑克牌型
    public static string[] s_poker = { "diamond", "club", "spade", "heart" };

    public static string[] s_pokerCows = { "diamond", "club", "heart", "spade" };
    public static string[] s_pokerColorCows = { "方块", "梅花", "红桃", "黑桃" };

    // 骰宝结果描述串
    public static string[] s_diceStr = { "大", "小", "豹子" };

    public static string[] s_isBanker = { "是否上庄:是", "是否上庄:否" };

    // 扑克牌面值
    public static string[] s_pokerNum = { "", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    // 百家乐的结果
    public static string[] s_baccaratResult = { "和", "闲", "闲对", "庄对", "庄" };

    public const int OP_ADD = 0;      // 添加
    public const int OP_REMOVE = 1;   // 移除
    public const int OP_MODIFY = 2;   // 修改
    public const int OP_VIEW = 3;     // 查看
}

public enum QueryType
{
    // GM账号
    queryTypeGmAccount,

    // 金币，钻石变化
    queryTypeMoney,
    // 详细
    queryTypeMoneyDetail,

    // 客服信息查询
    queryTypeServiceInfo,

    // 邮件
    queryTypeMail,

    // 充值记录
    queryTypeRecharge,

    // 账号查询
    queryTypeAccount,

    // 登陆历史
    queryTypeLoginHistory,

    // 礼包查询
    queryTypeGift,

    // 礼包码查询
    queryTypeGiftCode,

    // 兑换
    queryTypeExchange,

    // 大厅通用数据
    queryTypeLobby,

    // 服务器收益
    queryTypeServerEarnings,

    // 捕鱼独立数据
    queryTypeIndependentFishlord,

    // 鳄鱼独立数据
    queryTypeIndependentCrocodile,

    // 骰宝独立数据
    queryTypeIndependentDice,

    // 牛牛独立数据
    queryTypeIndependentCows,

    // 骰宝盈利率
    queryTypeDiceEarnings,

    // 百家乐盈利率
    queryTypeBaccaratEarnings,

    // 百家乐上庄情况
    queryTypeBaccaratPlayerBanker,

    // 牛牛上庄情况
    queryTypeCowsPlayerBanker,

    // 当前公告
    queryTypeCurNotice,

    // 捕鱼参数查询
    queryTypeFishlordParam,
    // 鳄鱼公园参数查询
    queryTypeFishParkParam,

    // 捕鱼桌子参数查询
    queryTypeFishlordDeskParam,
    // 鳄鱼公园桌子参数查询
    queryTypeFishParkDeskParam,

    // 鳄鱼大亨参数查询
    queryTypeCrocodileParam,

    // 牛牛参数查询
    queryTypeQueryCowsParam,

    // 五龙参数查询，每个房间的系统总收入，总支出，盈利率..
    queryTypeDragonParam,

    // 五龙各游戏模式下的参数查询
    queryTypeDragonGameModeEarning,

    // 黑红梅方参数查询
    queryTypeShcdParam,
    // 黑红梅方独立数据
    queryTypeIndependentShcd,

    // 查询套牛游戏相关
    queryTypeGameCalfRoping,

    // 鱼的情况统计
    queryTypeFishStat,
    // 鳄鱼公园鱼的情况统计
    queryTypeFishParkStat,

    // 货币最多的玩家
    queryTypeMoneyAtMost,

    // 旧的盈利率
    queryTypeOldEaringsRate,

    // 经典捕鱼阶段分析
    queryTypeFishlordStage,
    // 鳄鱼公园阶段分析
    queryTypeFishParkStage,

    // 当前在线
    queryTypeOnlinePlayerCount,

    // 操作日志
    queryTypeOpLog,

    // 查询玩家头像
    queryTypePlayerHead,

    // 消耗总计
    queryTypeTotalConsume,

    // 各游戏收入
    queryTypeGameRecharge,

    // 金币增长排行
    queryTypeCoinGrowthRank,

    // 流失查询
    queryTypeAccountCoinLessValue,

    // 捕鱼消耗
    queryTypeFishConsume,

    // 牛牛牌型查询
    queryTypeCowsCardsType,

    // 游戏结果控制查询
    queryTypeGameResultControl,

    // 头像举报
    queryTypeInformHead,

    // 查询td活跃
    queryTypeTdActivation,
    // LTV价值
    queryTypeLTV,

    // 查询最高在线
    queryTypeMaxOnline,

    // 玩家金币总和
    queryTypeTotalPlayerMoney,

    // 大奖赛相关查询
    queryTypeGrandPrix,

    // boss统计
    queryTypeFishBoss,

    // 兑换统计
    queryTypeExchangeStat,

    // 付费点
    queryTypeRechargePointStat,

    // 星星抽奖
    queryTypeStarLottery,

    queryTypeRLose,
    // 每日龙珠
    queryTypeDragonBallDaily,
    // 玩家充值监控
    queryTypeRechargePlayerMonitor,

    // 每小时付费
    queryTypeRechargePerHour,
    // 每小时在线人数
    queryTypeOnlinePlayerNumPerHour,
    // 平均游戏时长分布
    queryTypeGameTimeDistribution,
    // 用户喜好-平均在线时间
    queryTypeGameTimePlayerFavor,
    // 首付游戏时长分布
    queryTypeFirstRechargeGameTimeDistribution,
    // 首次购买计费点分布
    queryTypeFirstRechargePointDistribution,
    // 用户下注情况
    queryTypePlayerGameBet,
    // 查询玩家收支统计
    queryTypePlayerIncomeExpenses,

    // 新增用户分析
    queryTypeNewPlayer,
}

