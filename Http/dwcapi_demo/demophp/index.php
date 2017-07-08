<?php
    header("Content-type: text/html; charset=gb2312");

    include_once 'op.php';
   
    function outputInfo($tipInfo, $content)
    {
        echo $tipInfo; 
        echo $content, '<br/>';
    }
    
    function queryMoneyChange($mgr, $playerAcc)
    {
        $retStr = $mgr->queryPlayerMoneyChange($playerAcc, 0, 0, "2016-06-01 00:00:00", "2016-07-27 23:59:59");
        outputInfo("玩家货币变化结果1--------------", $retStr);
        
        $retStr = $mgr->queryPlayerMoneyChange($playerAcc, 1, 100, "2016-06-01 00:00:00", "2016-07-27 23:59:59");
        outputInfo("玩家货币变化结果2--------------", $retStr);
    }
    
    function queryWinLose($mgr, $playerAcc)
    {
        $retStr = $mgr->queryPlayerWinLose($playerAcc, 0, 0, "2016-06-01", "2016-07-27");
        outputInfo("玩家输赢结果1--------------", $retStr);
        
        $retStr = $mgr->queryPlayerWinLose($playerAcc, 1, 100, "2016-06-01", "2016-07-27");
        outputInfo("玩家输赢结果2--------------", $retStr);
    }
    
    function updatePwd($mgr, $playerAcc)
    {
        $retStr = $mgr->updatePlayerPwd($playerAcc, "123456", "654321");
        outputInfo("修改密码结果1----------------------------", $retStr);
    
        $retStr = $mgr->updatePlayerPwd($playerAcc, "654321", "123456");
        outputInfo("修改密码结果2----------------------------", $retStr);
    }
    
    function queryWinLoseSum($mgr)
    {
        $retStr = $mgr->queryWinLoseSum("2016-05-01", "2016-05-08");
        outputInfo("输赢总和1----------------------------", $retStr);
    
        $retStr = $mgr->queryWinLoseSum("2016-05-08", "2016-06-08");
        outputInfo("输赢总和2----------------------------", $retStr);
    }
    
    function playerOp($mgr, $playerAcc)
    {
        $retStr = $mgr->playerOp($playerAcc, 0);
        outputInfo("停封结果----------------------------", $retStr);
    
        $retStr = $mgr->playerOp($playerAcc, 1);
        outputInfo("解封结果----------------------------", $retStr);
    }
    
    // 假定申请的GM后台账号
    $gmAcc = "ybiaoCNY123z";
    // 假定申请的GM后台账号密码
    $gmPwd = "123456";
    // 假定申请的GM后台接入密钥
    $devKey = "fa4c1529f25e426ea489a88637178d36";

    $mgr = new OpMgr($gmAcc, $gmPwd, $devKey);
    
    // 玩家账号构成，前缀+账号名
    $playerAcc = "123zyb003";
    
    $retStr = "";
    
    // 创建玩家
    $retStr = $mgr->createPlayer($playerAcc, "abc123");
    outputInfo("创建玩家结果--------------", $retStr);
    
    $retStr = $mgr->playerSaveMoney($playerAcc, 500, Tool::genGuid(), "");
    outputInfo("玩家存款结果--------------", $retStr);
    
    $retStr = $mgr->playerDrawMoney($playerAcc, 500, Tool::genGuid(), "");
    outputInfo("玩家提款结果--------------", $retStr);
   
    $retStr = $mgr->queryPlayerTradingRecord($playerAcc, 0, 0, 0, "2016-06-01 00:00:00", "2016-07-31 23:59:59");
    outputInfo("玩家存取款结果1--------------", $retStr);
    
    $retStr = $mgr->queryPlayerTradingRecord($playerAcc, 2, 1, 9, "2016-06-01 00:00:00", "2016-07-31 23:59:59");
    outputInfo("玩家存取款结果2--------------", $retStr);
    
    $retStr = $mgr->queryPlayerInfo($playerAcc);
    outputInfo("查询玩家信息结果--------------", $retStr);
    
    $retStr = $mgr->queryPlayerOnline($playerAcc);
    outputInfo("查询玩家是否在线--------------", $retStr);
    
    $retStr = $mgr->logoutPlayer($playerAcc, 600);
    outputInfo("登出玩家--------------", $retStr);
    
    $retStr = $mgr->clearLoginFailed($playerAcc);
    outputInfo("清理玩家登录失败次数结果--------------", $retStr);
    
    $retStr = $mgr->unlockPlayer($playerAcc);
    outputInfo("解锁玩家结果-------------", $retStr);
    
    queryMoneyChange($mgr, $playerAcc);
    
    queryWinLose($mgr, $playerAcc);
    
    updatePwd($mgr, $playerAcc);
    
    queryWinLoseSum($mgr);
    
    playerOp ($mgr, $playerAcc);
?>





























