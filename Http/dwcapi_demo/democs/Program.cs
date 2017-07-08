using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace democs
{
    class Program
    {
        // 测试前，需要先把 SERVER_DOMAIN.SERVER_DOMAIN  改成正确的IP
        static void Main(string[] args)
        {
            // 假定申请的GM后台账号
            string gmAcc = "ybiaoCNY123z";
            // 假定申请的GM后台账号密码
            string gmPwd = "123456";
            // 假定申请的GM后台接入密钥
            string devKey = "fa4c1529f25e426ea489a88637178d36";

            OpMgr mgr = new OpMgr(gmAcc, gmPwd, devKey);

            // 玩家账号构成，前缀+账号名
            string playerAcc = "123zyb001";
            string retStr = "";

            retStr = mgr.queryOrderInfo("201605181650367701_aaaaCNY123_123test00");
            System.Console.WriteLine(retStr);

            retStr = mgr.createPlayer(playerAcc, "abc123");
            System.Console.WriteLine("创建玩家结果----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.playerSaveMoney(playerAcc, 500, Guid.NewGuid().ToString().Replace("-", ""), "http://localhost:10747/ApiNotify.aspx");
            System.Console.WriteLine("玩家存款结果----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.playerDrawMoney(playerAcc, 500);
            System.Console.WriteLine("玩家提款结果----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerTradingRecord(playerAcc, 0, 0, 0, "2016-03-01 00:00:00", "2016-03-31 23:59:59");
            System.Console.WriteLine("玩家存取款结果1----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerTradingRecord(playerAcc, 2, 1, 9, "2016-03-01 00:00:00", "2016-03-31 23:59:59");
            System.Console.WriteLine("玩家存取款结果2----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerInfo(playerAcc);
            System.Console.WriteLine("查询玩家信息结果----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerOnline(playerAcc);
            System.Console.WriteLine("查询玩家是否在线----------------------------");
            System.Console.WriteLine(retStr);
            
            retStr = mgr.logoutPlayer(playerAcc, 600);
            System.Console.WriteLine("登出玩家----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.clearLoginFailed(playerAcc);
            System.Console.WriteLine("清理玩家登录失败次数结果---------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.unlockPlayer(playerAcc);
            System.Console.WriteLine("解锁玩家结果---------------------------");
            System.Console.WriteLine(retStr);
            
            queryMoneyChange(mgr, playerAcc);

            queryWinLose(mgr, playerAcc);

            updatePwd(mgr, playerAcc);

            queryWinLoseSum(mgr);

            playerOp(mgr, playerAcc);
            System.Console.Read();
        }

        static void queryMoneyChange(OpMgr mgr, string playerAcc)
        {
            string retStr = mgr.queryPlayerMoneyChange(playerAcc, 0, 0, "2016-09-07 00:00:00", "2016-09-07 23:59:59");
            System.Console.WriteLine("玩家货币变化结果1----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerMoneyChange(playerAcc, 1, 100, "2016-09-07 00:00:00", "2016-09-07 23:59:59");
            System.Console.WriteLine("玩家货币变化结果2----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerMoneyChange("", 0, 0, "2016-08-01 00:00:00", "2016-08-31 23:59:59");
            System.Console.WriteLine("玩家货币变化结果3----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerMoneyChange("", 1, 100, "2016-08-01 00:00:00", "2016-08-31 23:59:59");
            System.Console.WriteLine("玩家货币变化结果4----------------------------");
            System.Console.WriteLine(retStr);
        }

        static void queryWinLose(OpMgr mgr, string playerAcc)
        {
            string retStr = mgr.queryPlayerWinLose(playerAcc, 0, 0, "2016-04-01", "2016-04-27");
            System.Console.WriteLine("玩家输赢结果1----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryPlayerWinLose(playerAcc, 1, 100, "2016-04-01", "2016-04-27");
            System.Console.WriteLine("玩家输赢结果2----------------------------");
            System.Console.WriteLine(retStr);
        }

        static void updatePwd(OpMgr mgr, string playerAcc)
        {
            string retStr = mgr.updatePlayerPwd(playerAcc, "", "654321");
            System.Console.WriteLine("修改密码结果1----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.updatePlayerPwd(playerAcc, "", "123456");
            System.Console.WriteLine("修改密码结果2----------------------------");
            System.Console.WriteLine(retStr);
        }

        static void queryWinLoseSum(OpMgr mgr)
        {
            string retStr = mgr.queryWinLoseSum("2016-05-01", "2016-05-08");
            System.Console.WriteLine("输赢总和1----------------------------");
            System.Console.WriteLine(retStr);

            retStr = mgr.queryWinLoseSum("2016-05-08", "2016-06-08");
            System.Console.WriteLine("输赢总和2----------------------------");
            System.Console.WriteLine(retStr);
        }

        static void playerOp(OpMgr mgr, string playerAcc)
        {
            string retStr = mgr.playerOp(playerAcc, 0);
            Console.WriteLine("停封结果----------------------------");
            Console.WriteLine(retStr);

            retStr = mgr.playerOp(playerAcc, 1);
            Console.WriteLine("解封结果----------------------------");
            Console.WriteLine(retStr);
        }
    }
}
