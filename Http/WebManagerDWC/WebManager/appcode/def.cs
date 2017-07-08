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
    // 黑红梅方详情
    public const string ASPX_GAME_DETAIL_SHCD = "/appaspx/stat/gamedetail/GameDetailShcd.aspx?index={0}";

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

    // 黑红梅方的扑克牌
    public static string[] s_pokerShcd = { "spade", "heart", "club", "diamond", "joker" };
}
