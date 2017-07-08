using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Globalization;

public struct CONST
{
    // 踢玩家URL
   // public static string URL_KICK_PLAYER = WebConfigurationManager.AppSettings["kickPlayer"];

    // 解锁玩家-请求URL
  //  public static string URL_UNLOCK_PLAYER = WebConfigurationManager.AppSettings["unLockPlayer"];

    // 解锁玩家-返回 URL
    public static string URL_UNLOCK_PLAYER_RET = WebConfigurationManager.AppSettings["unLockPlayerRet"];

    // 清理登录失败次数
    public static string URL_CLEAR_FAILED_LOGIN = WebConfigurationManager.AppSettings["clearFailedLoginCount"];

    // 修改玩家密码
    public static string URL_MODIFY_PLAYER_PWD = WebConfigurationManager.AppSettings["modifyAccountPwd"];

    // 时间格式
    public static string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

    // 日期部分格式
    public static string DATE_TIME_FORMAT_PART_DAY = "yyyy-MM-dd";

    public static IFormatProvider DATE_PROVIDER = new CultureInfo("zh-cn");

    public const string SQL_UPDATE_PLAYER_INFO = "UPDATE {0} set {1} where acc='{2}' ";

    public const string SQL_QUERY_GM_USER = "SELECT acc,pwd,accType,devSecretKey {3}  " +
                                            " FROM {0} where acc='{1}' and pwd='{2}' ";

    public static string[] FIELD_SET = {
                                           // 创建玩家
                                           ",moneyType,owner,createCode,money,postfix,depth,washRatio",      
   
                                           // 上分下分
                                           ",money,moneyType,depth,createCode",                       

                                           // 查看玩家是否在线，踢玩家,解锁玩家,更新玩家信息,清理登录失败次数,查询玩家详细信息,查询玩家的上分下分情况
                                           "", 
                                           ",createCode",           
                                       };

    public const string SQL_ORDER_ID = " SELECT opId  from {0} where opSrc='{1}' and opDst='{2}' and opType={3} " +
                                       " order by opTime desc LIMIT 0,1 ";

    // 货币单位
    public static int MONEY_BASE = Convert.ToInt32(WebConfigurationManager.AppSettings["moneyBase"]);
}

