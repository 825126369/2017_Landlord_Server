package demo;

public class OpMgr 
{
	private String m_gmAcc;
    private String m_gmPwd;
    private String m_devKey;

    public OpMgr(String gmAcc, String gmPwd, String devKey)
    {
        m_gmAcc = gmAcc;
        m_gmPwd = Tool.md5(gmPwd);
        m_devKey = devKey;
    }

    // 创建玩家
    public String createPlayer(String playerAcc, String playerPwd)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + playerPwd + m_devKey);
        String url = String.format(CC.URL_CREATE_PLAYER, m_gmAcc, m_gmPwd, playerAcc, playerPwd, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 创建玩家1，含洗码比，别名
    public String createPlayer(String playerAcc, String playerPwd, float washRatio, String aliasName)
    {
    	String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + playerPwd + m_devKey);
    	String url = String.format(CC.URL_CREATE_PLAYER1, m_gmAcc, m_gmPwd, playerAcc, playerPwd, washRatio, aliasName, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }
    
    // 玩家存款
    public String playerSaveMoney(String playerAcc, int money, String userOrderId, String callBackURL)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + money + userOrderId + m_devKey);
        String url = String.format(CC.URL_SAVE_MONEY, m_gmAcc, m_gmPwd, playerAcc, money, sign, userOrderId, callBackURL);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 玩家取款
    public String playerDrawMoney(String playerAcc, int money, String userOrderId, String callBackURL)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + money + userOrderId + m_devKey);
        String url = String.format(CC.URL_DRAW_MONEY, m_gmAcc, m_gmPwd, playerAcc, money, sign, userOrderId, callBackURL);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家存取款记录
    public String queryPlayerTradingRecord(String playerAcc, 
                                           int opType, 
                                           int curPage, 
                                           int countEachPage,
                                           String startTime,
                                           String endTime)
    {
    	String start = "";
    	String end = "";
    	try 
    	{
    		start = java.net.URLEncoder.encode(startTime,"UTF-8");
    		end = java.net.URLEncoder.encode(endTime,"UTF-8");
		}
    	catch (Exception e) {
		}
    	
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        String url = String.format(CC.URL_PLAYER_TRADING_RECORD, 
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    curPage,
                                    countEachPage,
                                    start, 
                                    end,
                                    opType,
                                    sign);
        
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家信息
    public String queryPlayerInfo(String playerAcc)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        String url = String.format(CC.URL_QUERY_PLAYER_INFO, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家是否在线
    public String queryPlayerOnline(String playerAcc)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        String url = String.format(CC.URL_QUERY_PLAYER_ONLINE, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 登出玩家
    public String logoutPlayer(String playerAcc, int forbidTime)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + forbidTime + m_devKey);
        String url = String.format(CC.URL_LOGOUT_PLAYER, m_gmAcc, m_gmPwd, playerAcc, forbidTime, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 清理登录失败次数
    public String clearLoginFailed(String playerAcc)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        String url = String.format(CC.URL_CLEAR_LOGIN_FAILED, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 解锁玩家
    public String unlockPlayer(String playerAcc)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        String url = String.format(CC.URL_UNLOCK_PLAYER, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 更新玩家信息
    public String updatePlayer(String playerAcc)
    {
        String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
        String url = String.format(CC.URL_UNLOCK_PLAYER, m_gmAcc, m_gmPwd, playerAcc, sign);
        return Tool.getHttps(CC.URL_UPDATE_PLAYER_INFO + url);
    }
    
    // 查询玩家在游戏内的货币变化记录
    public String queryPlayerMoneyChange(String playerAcc,
                                            int curPage,
                                            int countEachPage,
                                            String startTime,
                                            String endTime)
    {
    	String start = "";
    	String end = "";
    	try 
    	{
    		start = java.net.URLEncoder.encode(startTime,"UTF-8");
    		end = java.net.URLEncoder.encode(endTime,"UTF-8");
		}
    	catch (Exception e) {
		}
    	
    	String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
    	String url = String.format(CC.URL_PLAYER_MONEY_CHANGE,
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    curPage,
                                    countEachPage,
                                    start,
                                    end,
                                    sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 查询玩家输赢统计记录
    public String queryPlayerWinLose(String playerAcc,
                                    int curPage,
                                    int countEachPage,
                                    String startTime,
                                    String endTime)
    {
    	String start = "";
    	String end = "";
    	try 
    	{
    		start = java.net.URLEncoder.encode(startTime,"UTF-8");
    		end = java.net.URLEncoder.encode(endTime,"UTF-8");
		}
    	catch (Exception e) {
		}
    	String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + m_devKey);
    	String url = String.format(CC.URL_PLAYER_WIN_LOSE,
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    curPage,
                                    countEachPage,
                                    start,
                                    end,
                                    sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }

    // 查询订单信息
    public String queryOrderInfo(String orderId)
    {
    	String sign = Tool.md5(m_gmAcc + m_gmPwd + orderId + m_devKey);
    	String url = String.format(CC.URL_QUERY_ORDER_INFO,
                                    m_gmAcc,
                                    m_gmPwd,
                                    orderId,
                                    sign);

        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }
    
    // 修改玩家密码
    public String updatePlayerPwd(String playerAcc, String oldPwd, String newPwd)
    {
    	String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + oldPwd + newPwd + m_devKey);
    	String url = String.format(CC.URL_UPDATE_PLAYER_PWD,
                                    m_gmAcc,
                                    m_gmPwd,
                                    playerAcc,
                                    oldPwd,
                                    newPwd,
                                    sign);

        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }
    
    // API下的所有玩家的输赢总和
    public String queryWinLoseSum(String startTime, String endTime)
    {
    	String start = "";
    	String end = "";
    	try 
    	{
    		start = java.net.URLEncoder.encode(startTime,"UTF-8");
    		end = java.net.URLEncoder.encode(endTime,"UTF-8");
		}
    	catch (Exception e) {
		}
    	
    	String sign = Tool.md5(m_gmAcc + m_gmPwd + m_devKey);
    	String url = String.format(CC.URL_QUERY_WIN_LOSE_SUM,
                                    m_gmAcc,
                                    m_gmPwd,
                                    start,
                                    end,
                                    sign);

        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }
    
    // 玩家操作
    public String playerOp(String playerAcc, int op)
    {
    	String sign = Tool.md5(m_gmAcc + m_gmPwd + playerAcc + op + m_devKey);
    	String url = String.format(CC.URL_PLAYER_OP, m_gmAcc, m_gmPwd, playerAcc, op, sign);
        return Tool.getHttps(CC.SERVER_DOMAIN + url);
    }
}
