package demo;

import java.util.UUID;

public class Test 
{
	public static void main(String[] args) 
	{
		// 假定申请的GM后台账号
		String gmAcc = "ybiaoCNY123z";
		// 假定申请的GM后台账号密码
		String gmPwd = "123456";
		// 假定申请的GM后台接入密钥
		String devKey = "fa4c1529f25e426ea489a88637178d36";

        OpMgr mgr = new OpMgr(gmAcc, gmPwd, devKey);
  
        String retStr = "";
        
        String playerAcc = "123zyb003";
       
        retStr = mgr.createPlayer(playerAcc, "abc123");
        System.out.println("创建玩家结果----------------------------");
        System.out.println(retStr);
        
        retStr = mgr.playerSaveMoney(playerAcc, 500, UUID.randomUUID().toString().replace("-", ""), "");
        System.out.println("玩家存款结果----------------------------");
        System.out.println(retStr);
        
        retStr = mgr.playerDrawMoney(playerAcc, 500, UUID.randomUUID().toString().replace("-", ""), "");
        System.out.println("玩家提款结果----------------------------");
        System.out.println(retStr);

        retStr = mgr.queryPlayerTradingRecord(playerAcc, 0, 0, 0, "2016-03-01 00:00:00", "2016-03-31 23:59:59");
        System.out.println("玩家存取款记录1----------------------------");
        System.out.println(retStr);

        retStr = mgr.queryPlayerTradingRecord(playerAcc, 2, 1, 9, "2016-03-01 00:00:00", "2016-03-31 23:59:59");
        System.out.println("玩家存取款记录2----------------------------");
        System.out.println(retStr);

        retStr = mgr.queryPlayerInfo(playerAcc);
        System.out.println("查询玩家信息结果----------------------------");
        System.out.println(retStr);

        retStr = mgr.queryPlayerOnline(playerAcc);
        System.out.println("查询玩家是否在线----------------------------");
        System.out.println(retStr);

        retStr = mgr.logoutPlayer(playerAcc, 600);
        System.out.println("登出玩家----------------------------");
        System.out.println(retStr);

        retStr = mgr.clearLoginFailed(playerAcc);
        System.out.println("清理玩家登录失败次数结果---------------------------");
        System.out.println(retStr);

        retStr = mgr.unlockPlayer(playerAcc);
        System.out.println("解锁玩家结果---------------------------");
        System.out.println(retStr);
        
        queryMoneyChange(mgr, playerAcc);
        queryWinLose(mgr, playerAcc);
        
        updatePwd(mgr, playerAcc);
        
        queryWinLoseSum(mgr);
        
        playerOp(mgr, playerAcc);
	}

	static void queryMoneyChange(OpMgr mgr, String playerAcc)
    {
		String retStr = mgr.queryPlayerMoneyChange(playerAcc, 0, 0, "2016-04-01 00:00:00", "2016-04-27 23:59:59");
		System.out.println("玩家货币变化结果1----------------------------");
		System.out.println(retStr);

        retStr = mgr.queryPlayerMoneyChange(playerAcc, 1, 100, "2016-04-01 00:00:00", "2016-04-27 23:59:59");
        System.out.println("玩家货币变化结果2----------------------------");
        System.out.println(retStr);
    }

    static void queryWinLose(OpMgr mgr, String playerAcc)
    {
    	String retStr = mgr.queryPlayerWinLose(playerAcc, 0, 0, "2016-04-01", "2016-04-27");
    	System.out.println("玩家输赢结果1----------------------------");
    	System.out.println(retStr);

        retStr = mgr.queryPlayerWinLose(playerAcc, 1, 100, "2016-04-01", "2016-04-27");
        System.out.println("玩家输赢结果2----------------------------");
        System.out.println(retStr);
    }
    
    static void updatePwd(OpMgr mgr, String playerAcc)
    {
    	String retStr = mgr.updatePlayerPwd(playerAcc, "123456", "654321");
    	System.out.println("修改密码结果1----------------------------");
    	System.out.println(retStr);

        retStr = mgr.updatePlayerPwd(playerAcc, "654321", "123456");
        System.out.println("修改密码结果2----------------------------");
        System.out.println(retStr);
    }
    
    static void queryWinLoseSum(OpMgr mgr)
    {
    	String retStr = mgr.queryWinLoseSum("2016-05-01", "2016-05-08");
    	System.out.println("输赢总和1----------------------------");
    	System.out.println(retStr);

        retStr = mgr.queryWinLoseSum("2016-05-08", "2016-06-08");
        System.out.println("输赢总和2----------------------------");
        System.out.println(retStr);
    }
    
    static void playerOp(OpMgr mgr, String playerAcc)
    {
    	String retStr = mgr.playerOp(playerAcc, 0);
    	System.out.println("停封结果----------------------------");
    	System.out.println(retStr);

        retStr = mgr.playerOp(playerAcc, 1);
        System.out.println("解封结果----------------------------");
        System.out.println(retStr);
    }
}
