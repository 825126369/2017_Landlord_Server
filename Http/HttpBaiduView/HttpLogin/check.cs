using System;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.Configuration;

// 登陆时检测，是否封IP或账号
public class LoginCheck
{
    public static string checkIP(HttpRequest request)
    {
        // 封账号
        string remoteIP = Common.Helper.getRemoteIP(request);
        bool res = MongodbAccount.Instance.KeyExistsBykey("blockIP", "ip", remoteIP);
        if (res)
        {
            return "-100"; // 停封IP
        }
        return "";
    }
}

public struct CONST
{
    // 启用登录失败检测
    public static int USE_LOGIN_FAILED_COUNT_CHECK = Convert.ToInt32(WebConfigurationManager.AppSettings["useLoginFailedCountCheck"]);

    // 登录失败允许的最大次数
    public static int LOGIN_FAILED_MAX_COUNT = Convert.ToInt32(WebConfigurationManager.AppSettings["loginFailedMaxCount"]);

    // 登录失败字段
    public static string[] LOGIN_FAILED_FIELD = { "loginFailedDate", "loginFailedCount", "pwd", "updatepwd", "block", "acc_real", "bindPhone"};
}

