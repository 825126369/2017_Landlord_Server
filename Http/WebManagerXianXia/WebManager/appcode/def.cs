using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.Configuration;

public enum RechargeType
{
    // 充人民币
    rechargeRMB,

    // 删除自定义头像
    delIconCustom,
};

// 统计
delegate void statData(List<Dictionary<string, object>> dataList);

public struct DefCC
{
    public const string ASPX_GM_INFO = "/appaspx/account/AccountGmInfo.aspx?acc={0}";

    public const string ASPX_GM_INFO_NO_PARAM = "/appaspx/account/AccountGmInfo.aspx";

    public const string ASPX_GM_INFO_VIEW_TREE_NO_PARAM = "/appaspx/account/GmInfoViewTree.aspx";

    public const string ASPX_EMPTY = "/appaspx/Empty.aspx";

   // public const string ASPX_EMPTY_FOR_COMMON_SCORE = "/appaspx/account/sub/AccountEmptyForCommonScore.aspx";
   // public const string ASPX_EMPTY_FOR_COMMON_REPORT = "/appaspx/account/sub/AccountEmptyForCommonReport.aspx";
   // public const string ASPX_EMPTY_FOR_COMMON_ACCMGR = "/appaspx/account/sub/AccountEmptyForCommonAccMgr.aspx";

    public const string ASPX_SUB_PLAYER = "/appaspx/account/sub/AccountSubPlayer.aspx";

    public const string ASPX_SUB_SCORE_PLAYER = "/appaspx/account/AccountScorePlayer.aspx";

    public const string ASPX_SUB_Agency = "/appaspx/account/sub/AccountAgencySub.aspx";

    public const string ASPX_AGENCY_RIGHT = "/appaspx/account/sub/AccountAgencyRight.aspx";

    public const string ASPX_PLAYER_OP = "/appaspx/account/AccountPlayerOp.aspx";

    public const string ASPX_SCORE_GM = "/appaspx/account/AccountScore.aspx";

    public const string ASPX_MODIFY_LOGIN_PWD = "/appaspx/account/AccountModifyLoginPwd.aspx?acc={0}";

    public const string ASPX_MODIFY_HOME = "/appaspx/account/AccountModifyHome.aspx?acc={0}&home={1}";

    public const string ASPX_MODIFY_ALIASNAME = "/appaspx/account/sub/AccountSubModifyAliasName.aspx?acc={0}";

    // 输赢统计页面
    public const string ASPX_WIN_LOSE = "/appaspx/account/report/AccountWinLoseReport.aspx";

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
    // 黑红梅方详情
    public const string ASPX_GAME_DETAIL_SHCD = "/appaspx/stat/gamedetail/GameDetailShcd.aspx?index={0}";
    // 五龙详情
    public const string ASPX_GAME_DETAIL_DRAGON = "/appaspx/stat/gamedetail/GameDetailDragon.aspx?index={0}";

    //public const string ASPX_LOGIN_STEP1 = "/appaspx/Login.aspx";
    // 步骤1
    public const string ASPX_LOGIN_STEP1 = "/appaspx/LoginAccount.aspx";
    // 步骤2
    public const string ASPX_LOGIN_STEP2 = "/appaspx/LoginPwd.aspx";
    // 步骤3
    public const string ASPX_LOGIN_STEP3 = "/appaspx/LoginCode.aspx";

    //public const string ASPX_LOGIN_ENTER = "/appaspx/account/AccountCreateSwitch.aspx";
    public const string ASPX_LOGIN_ENTER = "/appaspx/account/AccountSelfInfo.aspx";

    public const string ASPX_API_APPROVE = "/appaspx/account/AccountApiApprove.aspx";

    public const string ASPX_PLAYER_ORDER = "/appaspx/account/AccountPlayerOrder.aspx";

    // 解锁玩家-返回 URL
    public static string URL_UNLOCK_PLAYER_RET = WebConfigurationManager.AppSettings["unLockPlayerRet"];

    // 清理登录失败次数
    public static string URL_CLEAR_FAILED_LOGIN = WebConfigurationManager.AppSettings["clearFailedLoginCount"];

    // 货币单位
    public static int MONEY_BASE = Convert.ToInt32(WebConfigurationManager.AppSettings["moneyBase"]);

    public static string HTTP_MONITOR = Convert.ToString(WebConfigurationManager.AppSettings["httpMonitor"]);

    // 验证key
    public const string KEY_VERIFICATION = "keyver";

    // 验证码随机序列
    public const string CODE_SERIAL = "0,1,2,3,4,5,6,7,8,9";

    // 扑克牌型
    public static string[] s_poker = { "diamond", "club", "spade", "heart" };

    public static string[] s_pokerCows = { "diamond", "club", "heart", "spade" };
    public static string[] s_pokerColorCows = { "方块", "梅花", "红桃", "黑桃" };

    // 黑红梅方的扑克牌
    public static string[] s_pokerShcd = { "spade", "heart", "club", "diamond", "joker" };

    // 骰宝结果描述串
    public static string[] s_diceStr = { "大", "小", "豹子" };

    public static string[] s_isBanker = { "是否上庄:是", "是否上庄:否" };

    // 扑克牌面值
    public static string[] s_pokerNum = { "", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    // 百家乐的结果
    public static string[] s_baccaratResult = { "和", "闲", "闲对", "庄对", "庄" };
}

//////////////////////////////////////////////////////////////////////////

public enum DyOpType
{
    // 发邮件
    opTypeSendMail,

    // 修改密码
    opTypeModifyPwd,

    // 停封账号
    opTypeBlockAcc,

    // 停封玩家ID
    opTypeBlockId,

    // 停封IP
    opTypeBlockIP,

    // 后台充值
    opTypeRecharge,

    // 推送添加APP应用信息
    opTypePushApp,

    // 绑定手机
    opTypeBindPhone,

    // 礼包生成
    opTypeGift,

    // 礼包码生成
    opTypeGiftCode,

    // 兑换
    opTypeExchange,

    // 通告
    opTypeNotify,

    // 运营维护
    opTypeMaintenance,

    // 捕鱼参数调整
    opTypeFishlordParamAdjust,
    // 鳄鱼公园参数调整
    opTypeFishParkParamAdjust,

    // 鳄鱼大亨参数调整
    opTypeCrocodileParamAdjust,

    // 骰宝参数调整
    opTypeDiceParamAdjust,

    // 百家乐参数调整
    opTypeBaccaratParamAdjust,

    // 牛牛参数调整
    opTypeCowsParamAdjust,

    // 五龙参数调整
    opTypeDragonParamAdjust,

    // 清空鱼统计表
    opTypeClearFishTable,

    // 重新加载表格
    opTypeReLoadTable,

    // 客服信息
    opTypeServiceInfo,

    // 冻结头像
    opTypeFreezeHead,

    // 渠道编辑
    opTypeEditChannel,

    // 通告消息
    opTypeSpeaker,

    // 设置牛牛牌型
    opTypeSetCowsCard,
    // 祝福诅咒
    opTypeWishCurse,

    // 游戏结果控制
    opTypeDyOpGameResult,

    // 游戏参数调整
    opTypeGameParamAdjust,

    // 修改游戏最大下注限制
    opTypeModifyMaxBetLimit,
    //////////////////////////////////////////////////////////////////////////
    // 创建管理员账号
    opTypeDyOpCreateGmAccount,

    // 创建线下玩家
    opTypeDyOpCreatePlayer,

    // 上分，下分
    opTypeDyOpScore,

    // 修改登录后台的密码
    opTypeModifyLoginPwd,

    // 踢玩家
    opTypeKickPlayer,
    // 解锁玩家
    opTypeUnlockPlayer,
    // 清理登录失败次数
    opTypeClearLoginFailed,
    // 给玩家打特殊标记，该玩家是否记录盈利率
    opTypeSetPlayerSpecialFlag,
    // 修改权限
    opTypeDyOpModiyGmRight,
    // 启用停用GM账号
    opTypeDyOpStartStopGmAcc,

    // API审批账号
    opTypeDyOpApiApprove,
    // 玩家订单处理
    opTypeDyOpPlayerOrder,

    // 修改GM属性
    opTypeDyOpModifyGmProperty,

    // 删操作日志
    opTypeDelData,

    // 删除GM账号，玩家账号
    opTypeDelAccount,

    // 游戏开关
    opTypeOpenGame,

    // API可设置下注最大上限
    opTypeModifyAPISetLimit,

    opTypePlayerOp,
}

public enum QueryType
{
    // GM账号
    queryTypeGmAccount,

    // 金币，钻石变化
    queryTypeMoney,

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

    // 游戏历史记录
    queryTypeGameHistory,

    // 查询玩家会员
    queryTypePlayerMember,

    // 级联账号
    queryTypeGmAccountCascade,

    queryTypeQueryGmAccountDetail,

    // 上分/下分记录
    queryTypeQueryScoreOpRecord,

    // api号审批查询
    queryTypeQueryApiApprove,

    // 查询玩家订单
    queryTypeQueryPlayerOrder,

    // 上下分实时订单，记录已处理过的在线订单。对于离线，可以查看上下分记录
    queryTypeQueryRealTimeOrder,
}

public enum StatType
{
    // 充值统计
    statTypeRecharge,

    // 相同订单号的统计
    statTypeSameOrderId,

    // 活跃次数
    statTypeActiveCount,

    // 活跃人数
    statTypeActivePerson,

    // vip等级分布
    statTypeVipLevel,

    // 售货亭统计
    statTypeSeller,

    // 售货亭管理员统计
    statTypeSellerAdmin,

    // 玩家的数据统计
    statTypePlayer,

    // 逐级统计
    statTypeSellerStep,

    // 输赢统计
    statTypeWinLose,
}



