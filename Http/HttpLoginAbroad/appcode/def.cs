using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

public struct HttpRetCode
{
    // url缺少参数
    public const int RET_PARAM_NOT_VALID = -1;

    // 签名错误
    public const int RET_SIGN_ERROR = -2;

    // 传入的json串有错
    public const int RET_JSON_ERROR = -3;

    // 发生了异常
    public const int RET_HAPPEN_EXCEPTION = -4;

    // 账号或密码错
    public const int RET_ACC_OR_PWD_ERROR = -10;

    // db出错
    public const int RET_DB_ERROR = -11;

    // 账号已存在
    public const int RET_ACC_EXISTS = -12;

    // 密码格式错误
    public const int RET_PWD_ERROR = -14;

    // 平台错误
    public const int RET_PLATFORM_ERROR = -15;

    // 未修改密码
    public const int RET_NOT_MODIFY_PWD = -16;

    // 账号被停封
    public const int RET_ACC_BLOCK = -17;

    // 账号格式错误
    public const int RET_ACC_ERROR = -20;

    // 账号被冻结
    public const int RET_ACC_FREEZE = -21;

    // 创建玩家时，缺少参数
    public const int RET_LACK_PARAM = -302;

    // 代理号出错
    public const int RET_AGENT_ACC_ERROR = -303;
}

public struct CC
{
    public static string MYSQL_IP = WebConfigurationManager.AppSettings["mysql"];

    // 由API接口来修改密码
    public static string RESET_MODIFY_BY_API = "dwcapi";
}













































