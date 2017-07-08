using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public class OpMgr
{
    private string m_gmAcc;
    private string m_gmPwd;
    private string m_devKey;

    public OpMgr(string gmAcc, string gmPwd, string devKey)
    {
        m_gmAcc = gmAcc;
        m_gmPwd = Tool.md5(gmPwd);
        m_devKey = devKey;
    }

    // 创建玩家
    public string createPlayer(string playerAcc, string playerPwd)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + playerPwd + m_devKey);
        string url = string.Format(CC.URL_CREATE_PLAYER, m_gmAcc, m_gmPwd, playerAcc, playerPwd, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 创建玩家1，含洗码比，别名
    public string createPlayer(string playerAcc, string playerPwd, int washRatio, string aliasName)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + playerPwd + m_devKey);
        string url = string.Format(CC.URL_CREATE_PLAYER1, m_gmAcc, m_gmPwd, playerAcc, playerPwd, washRatio, aliasName, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 玩家存款  userOrderId 自定义订单ID
    // callBackURL 回调页面
    public string playerSaveMoney(string playerAcc, int money, string userOrderId="", string callBackURL = "")
    {
        if (callBackURL != "")
        {
            callBackURL = HttpUtility.UrlEncode(callBackURL);
        }
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + money + userOrderId + m_devKey);
        string url = string.Format(CC.URL_SAVE_MONEY, m_gmAcc, m_gmPwd, playerAcc, money, sign, userOrderId, callBackURL);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 玩家取款 userOrderId 自定义订单ID
    // callBackURL 回调页面
    public string playerDrawMoney(string playerAcc, int money, string userOrderId = "", string callBackURL = "")
    {
        if (callBackURL != "")
        {
            callBackURL = HttpUtility.UrlEncode(callBackURL);
        }

        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + money + userOrderId + m_devKey);
        string url = string.Format(CC.URL_DRAW_MONEY, m_gmAcc, m_gmPwd, playerAcc, money, sign, userOrderId, callBackURL);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家存取款记录
    public string queryPlayerTradingRecord(string playerAcc, 
                                           int opType, 
                                           int curPage, 
                                           int countEachPage,
                                           string startTime,
                                           string endTime)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_PLAYER_TRADING_RECORD, 
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    curPage,
                                    countEachPage,
                                    startTime, 
                                    endTime,
                                    opType,
                                    sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家信息
    public string queryPlayerInfo(string playerAcc)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_QUERY_PLAYER_INFO, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家是否在线
    public string queryPlayerOnline(string playerAcc)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_QUERY_PLAYER_ONLINE, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 登出玩家
    public string logoutPlayer(string playerAcc, int forbidTime)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + forbidTime + m_devKey);
        string url = string.Format(CC.URL_LOGOUT_PLAYER, m_gmAcc, m_gmPwd, playerAcc, forbidTime, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 清理登录失败次数
    public string clearLoginFailed(string playerAcc)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_CLEAR_LOGIN_FAILED, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 解锁玩家
    public string unlockPlayer(string playerAcc)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_UNLOCK_PLAYER, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 更新玩家信息
    public string updatePlayer(string playerAcc, Dictionary<string, object> data)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_UNLOCK_PLAYER, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.get(CC.URL_UPDATE_PLAYER_INFO + url);
    }

    // 查询玩家在游戏内的货币变化记录
    public string queryPlayerMoneyChange(string playerAcc,
                                            int curPage,
                                            int countEachPage,
                                            string startTime,
                                            string endTime)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_PLAYER_MONEY_CHANGE,
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    curPage,
                                    countEachPage,
                                    startTime,
                                    endTime,
                                    sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家输赢统计记录
    public string queryPlayerWinLose(string playerAcc,
                                    int curPage,
                                    int countEachPage,
                                    string startTime,
                                    string endTime)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        string url = string.Format(CC.URL_PLAYER_WIN_LOSE,
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    curPage,
                                    countEachPage,
                                    startTime,
                                    endTime,
                                    sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 查询订单信息
    public string queryOrderInfo(string orderId)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + orderId + m_devKey);
        string url = string.Format(CC.URL_QUERY_ORDER_INFO,
                                    m_gmAcc,
                                    m_gmPwd,
                                    orderId,
                                    sign);

        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 修改玩家密码
    public string updatePlayerPwd(string playerAcc, string oldPwd, string newPwd)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + oldPwd + newPwd + m_devKey);
        string url = string.Format(CC.URL_UPDATE_PLAYER_PWD,
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    oldPwd,
                                    newPwd,
                                    sign);

        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // API下的所有玩家的输赢总和
    public string queryWinLoseSum(string startTime, string endTime)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + m_devKey);
        string url = string.Format(CC.URL_QUERY_WIN_LOSE_SUM,
                                    m_gmAcc,
                                    m_gmPwd,
                                    startTime,
                                    endTime,
                                    sign);

        return Tool.get(CC.SERVER_DOMAIN + url);
    }

    // 玩家操作
    public string playerOp(string playerAcc, int op)
    {
        string sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + op + m_devKey);
        string url = string.Format(CC.URL_PLAYER_OP, m_gmAcc, m_gmPwd, playerAcc, op, sign);
        return Tool.get(CC.SERVER_DOMAIN + url);
    }
}