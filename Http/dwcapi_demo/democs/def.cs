using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public struct CC
{
  //  public static string SERVER_DOMAIN = "https://103.17.95.100:444";
    public static string SERVER_DOMAIN = "http://localhost:9439";

    // 创建玩家
    public static string URL_CREATE_PLAYER = "/appaspx/CreatePlayer.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&playerPwd={3}&sign={4}";

    // 创建玩家，含洗码比，别名
    public static string URL_CREATE_PLAYER1 = "/appaspx/CreatePlayer.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&playerPwd={3}&washRatio={4}&aliasName={5}&sign={6}";

    // 玩家存款
    public static string URL_SAVE_MONEY = "/appaspx/PlayerSaveMoney.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&score={3}&sign={4}&userOrderId={5}&apiCallBack={6}";

    // 玩家取款
    public static string URL_DRAW_MONEY = "/appaspx/PlayerDrawMoney.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&score={3}&sign={4}&userOrderId={5}&apiCallBack={6}";

    // 玩家存提款记录
    public static string URL_PLAYER_TRADING_RECORD = "/appaspx/QueryPlayerTradingRecord.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&curPage={3}&countEachPage={4}&startTime={5}&endTime={6}&opType={7}&sign={8}";

    // 查询玩家信息
    public static string URL_QUERY_PLAYER_INFO = "/appaspx/QueryPlayerInfo.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&sign={3}";

    // 查询玩家是否在线
    public static string URL_QUERY_PLAYER_ONLINE = "/appaspx/QueryPlayerOnline.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&sign={3}";

    // 登出玩家
    public static string URL_LOGOUT_PLAYER = "/appaspx/LogoutPlayer.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&forbidTime={3}&sign={4}";

    // 登录失败次数清0
    public static string URL_CLEAR_LOGIN_FAILED = "/appaspx/ClearLoginFailed.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&sign={3}";

    // 解锁玩家
    public static string URL_UNLOCK_PLAYER = "/appaspx/UnlockPlayer.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&sign={3}";

    // 更新玩家信息
    public static string URL_UPDATE_PLAYER_INFO = "/appaspx/UpdatePlayerInfo.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&sign={3}";

    // 查询玩家在游戏内的货币变化记录
    public static string URL_PLAYER_MONEY_CHANGE = "/appaspx/QueryPlayerMoneyChange.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&curPage={3}&countEachPage={4}&startTime={5}&endTime={6}&sign={7}";

    // 查询玩家输赢统计记录
    public static string URL_PLAYER_WIN_LOSE = "/appaspx/QueryPlayerWinLose.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&curPage={3}&countEachPage={4}&startTime={5}&endTime={6}&sign={7}";

    // 查询订单信息
    public static string URL_QUERY_ORDER_INFO = "/appaspx/QueryOrderInfo.aspx?gmAcc={0}&gmPwd={1}&orderId={2}&sign={3}";

    // 修改玩家密码
    public static string URL_UPDATE_PLAYER_PWD = "/appaspx/UpdatePlayerPwd.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&oldPwd={3}&newPwd={4}&sign={5}";

    // API下的所有玩家的输赢总和
    public static string URL_QUERY_WIN_LOSE_SUM = "/appaspx/QueryWinLoseSum.aspx?gmAcc={0}&gmPwd={1}&startTime={2}&endTime={3}&sign={4}";

    // 玩家相关操作
    public static string URL_PLAYER_OP = "/appaspx/PlayerOp.aspx?gmAcc={0}&gmPwd={1}&playerAcc={2}&op={3}&sign={4}";
}

public class Tool
{
    public static string md5(string str)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] res = md5.ComputeHash(Encoding.Default.GetBytes(str), 0, str.Length);
        return BitConverter.ToString(res).Replace("-", "");
    }

    public static string get(string urlStr)
    {
        Uri uri = new Uri(urlStr);
        byte[] bytes = null;
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
          //  HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
          //  X509Certificate cerCaiShang = new X509Certificate("..\\dwcwebapi.pfx", "123456");
         //   httpRequest.ClientCertificates.Add(cerCaiShang);

            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();

            List<byte> lb = new List<byte>();
            MemoryStream ms = new MemoryStream();
            dataStream.CopyTo(ms);
            bytes = ms.ToArray();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message + uri);
            return "";
        }
        return Encoding.UTF8.GetString(bytes);
    }

    // 回调方法
    public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;

        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;
        return false;
    }
}