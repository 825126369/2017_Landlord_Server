package demo;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;
import java.security.MessageDigest;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import javax.net.ssl.*;

public final class CC 
{
    public static String SERVER_DOMAIN = "http://localhost:9439";

	// 这个域名需要更改
	//public static String SERVER_DOMAIN = "https://127.0.0.1";
	 
    // 创建玩家
    public static String URL_CREATE_PLAYER = "/appaspx/CreatePlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&playerPwd=%s&sign=%s";

    // 创建玩家，含洗码比，别名
    public static String URL_CREATE_PLAYER1 = "/appaspx/CreatePlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&playerPwd=%s&washRatio=%f&aliasName=%s&sign=%s";

    // 玩家存款
    public static String URL_SAVE_MONEY = "/appaspx/PlayerSaveMoney.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&score=%d&sign=%s&userOrderId=%s&apiCallBack=%s";

    // 玩家取款
    public static String URL_DRAW_MONEY = "/appaspx/PlayerDrawMoney.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&score=%d&sign=%s&userOrderId=%s&apiCallBack=%s";

    // 玩家存提款记录
    public static String URL_PLAYER_TRADING_RECORD = "/appaspx/QueryPlayerTradingRecord.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&curPage=%d&countEachPage=%d&startTime=%s&endTime=%s&opType=%d&sign=%s";

    // 查询玩家信息
    public static String URL_QUERY_PLAYER_INFO = "/appaspx/QueryPlayerInfo.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";

    // 查询玩家是否在线
    public static String URL_QUERY_PLAYER_ONLINE = "/appaspx/QueryPlayerOnline.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";

    // 登出玩家
    public static String URL_LOGOUT_PLAYER = "/appaspx/LogoutPlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&forbidTime=%d&sign=%s";

    // 登录失败次数清0
    public static String URL_CLEAR_LOGIN_FAILED = "/appaspx/ClearLoginFailed.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";

    // 解锁玩家
    public static String URL_UNLOCK_PLAYER = "/appaspx/UnlockPlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";

    // 更新玩家信息
    public static String URL_UPDATE_PLAYER_INFO = "/appaspx/UpdatePlayerInfo.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";
    
    // 查询玩家在游戏内的货币变化记录
    public static String URL_PLAYER_MONEY_CHANGE = "/appaspx/QueryPlayerMoneyChange.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&curPage=%d&countEachPage=%d&startTime=%s&endTime=%s&sign=%s";

    // 查询玩家输赢统计记录
    public static String URL_PLAYER_WIN_LOSE = "/appaspx/QueryPlayerWinLose.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&curPage=%d&countEachPage=%d&startTime=%s&endTime=%s&sign=%s";

    // 查询订单信息
    public static String URL_QUERY_ORDER_INFO = "/appaspx/QueryOrderInfo.aspx?gmAcc=%s&gmPwd=%s&orderId=%s&sign=%s";
    
    // 修改玩家密码
    public static String URL_UPDATE_PLAYER_PWD = "/appaspx/UpdatePlayerPwd.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&oldPwd=%s&newPwd=%s&sign=%s";
    
    // API下的所有玩家的输赢总和
    public static String URL_QUERY_WIN_LOSE_SUM = "/appaspx/QueryWinLoseSum.aspx?gmAcc=%s&gmPwd=%s&startTime=%s&endTime=%s&sign=%s";
    
    // 玩家相关操作
    public static String URL_PLAYER_OP = "/appaspx/PlayerOp.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&op=%s&sign=%s";
}

class Tool
{
	static char hexDigits[]={'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
	
    public static String md5(String s)
    {
        try 
        {
            byte[] btInput = s.getBytes();
            MessageDigest mdInst = MessageDigest.getInstance("MD5");
            mdInst.update(btInput);
            byte[] md = mdInst.digest();
            int j = md.length;
            char str[] = new char[j * 2];
            int k = 0;
            for (int i = 0; i < j; i++) 
            {
                byte byte0 = md[i];
                str[k++] = hexDigits[byte0 >>> 4 & 0xf];
                str[k++] = hexDigits[byte0 & 0xf];
            }
            return new String(str);
        } 
        catch (Exception e) 
        {
        }
        return "";
    }

    public static String getHttp(String requestUrl)
    {
    	String ret = "";
    	try 
    	{
    		URL pt = new URL(requestUrl);
    		
        	URLConnection conn = pt.openConnection();
        	
        	BufferedReader in = new BufferedReader(new InputStreamReader(
        			conn.getInputStream()));
        	
        	String inputLine;
        	
        	while ((inputLine = in.readLine()) != null)
        	{
        		ret += inputLine;
        	}
        	
        	in.close();
		} 
    	catch (Exception e) 
		{
    		System.out.println(e.toString());
		}
    	return ret;
    }
    
    public static String getHttps(String requestUrl)
    {
    	String ret = "";
    	try 
    	{
    		SSLContext sc = SSLContext.getInstance("SSL");
    		sc.init(null, new TrustManager[] { new TrustAnyTrustManager() },
                    new java.security.SecureRandom());
            
    		URL pt = new URL(requestUrl);
    		
    		HttpsURLConnection conn = (HttpsURLConnection) pt.openConnection();
            conn.setSSLSocketFactory(sc.getSocketFactory());
            conn.setHostnameVerifier(new TrustAnyHostnameVerifier());
            conn.connect();
        	
        	BufferedReader in = new BufferedReader(new InputStreamReader(
        			conn.getInputStream()));
        	
        	String inputLine;
        	
        	while ((inputLine = in.readLine()) != null)
        	{
        		ret += inputLine;
        	}
        	
        	in.close();
		} 
    	catch (Exception e) 
		{
    		System.out.println(e.toString());
		}
    	return ret;
    }
}

class TrustAnyTrustManager implements X509TrustManager
{
    public void checkClientTrusted(X509Certificate[] chain, String authType)
            throws CertificateException 
    {
    }

    public void checkServerTrusted(X509Certificate[] chain, String authType)
            throws CertificateException
    {
    }

    public X509Certificate[] getAcceptedIssuers()
    {
        return new X509Certificate[] {};
    }
}

class TrustAnyHostnameVerifier implements HostnameVerifier 
{
    public boolean verify(String hostname, SSLSession session)
    {
        return true;
    }
}




























