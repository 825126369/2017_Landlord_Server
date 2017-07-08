using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

public struct TableName
{
    // GM账号表
    public const string GM_ACCOUNT = "CPTGmAccount";

    // 每日设备激活
    public const string DAILY_DEVICE_ACTIVATE = "day_activation";

    // 每日账号创建
    public const string DAILY_ACCOUNT_CREATE = "day_regedit";

    // 每日充值总额
    public const string DAILY_RECHARGE_SUM = "day_channelpay";
}
