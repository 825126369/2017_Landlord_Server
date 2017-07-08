<?php
    class CC
    {
        //windows下的PHP，只需要到php.ini中把extension=php_openssl.dll前面的 ; 删掉，就可以请求https连接
        //public static $SERVER_DOMAIN = "https://localhost:9439";
        
        public static $SERVER_DOMAIN = "http://localhost:9439";
        
        // 创建玩家
        public static $URL_CREATE_PLAYER = "/appaspx/CreatePlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&playerPwd=%s&sign=%s";
        
        // 创建玩家，含洗码比，别名
        public static $URL_CREATE_PLAYER1 = "/appaspx/CreatePlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&playerPwd=%s&washRatio=%d&aliasName=%s&sign=%s";
        
        // 玩家存款
        public static $URL_SAVE_MONEY = "/appaspx/PlayerSaveMoney.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&score=%d&sign=%s&userOrderId=%s&apiCallBack=%s";
        
        // 玩家取款
        public static $URL_DRAW_MONEY = "/appaspx/PlayerDrawMoney.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&score=%d&sign=%s&userOrderId=%s&apiCallBack=%s";
        
        // 玩家存提款记录
        public static $URL_PLAYER_TRADING_RECORD = "/appaspx/QueryPlayerTradingRecord.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&curPage=%d&countEachPage=%d&startTime=%s&endTime=%s&opType=%d&sign=%s";
        
        // 查询玩家信息
        public static $URL_QUERY_PLAYER_INFO = "/appaspx/QueryPlayerInfo.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";
        
        // 查询玩家是否在线
        public static $URL_QUERY_PLAYER_ONLINE = "/appaspx/QueryPlayerOnline.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";
        
        // 登出玩家
        public static $URL_LOGOUT_PLAYER = "/appaspx/LogoutPlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&forbidTime=%d&sign=%s";
        
        // 登录失败次数清0
        public static $URL_CLEAR_LOGIN_FAILED = "/appaspx/ClearLoginFailed.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";
        
        // 解锁玩家
        public static $URL_UNLOCK_PLAYER = "/appaspx/UnlockPlayer.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";
        
        // 更新玩家信息
        public static $URL_UPDATE_PLAYER_INFO = "/appaspx/UpdatePlayerInfo.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&sign=%s";
        
        // 查询玩家在游戏内的货币变化记录
        public static $URL_PLAYER_MONEY_CHANGE = "/appaspx/QueryPlayerMoneyChange.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&curPage=%d&countEachPage=%d&startTime=%s&endTime=%s&sign=%s";
        
        // 查询玩家输赢统计记录
        public static $URL_PLAYER_WIN_LOSE = "/appaspx/QueryPlayerWinLose.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&curPage=%d&countEachPage=%d&startTime=%s&endTime=%s&sign=%s";
        
        // 查询订单信息
        public static $URL_QUERY_ORDER_INFO = "/appaspx/QueryOrderInfo.aspx?gmAcc=%s&gmPwd=%s&orderId=%s&sign=%s";
        
        // 修改玩家密码
        public static $URL_UPDATE_PLAYER_PWD = "/appaspx/UpdatePlayerPwd.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&oldPwd=%s&newPwd=%s&sign=%s";
        
        // API下的所有玩家的输赢总和
        public static $URL_QUERY_WIN_LOSE_SUM = "/appaspx/QueryWinLoseSum.aspx?gmAcc=%s&gmPwd=%s&startTime=%s&endTime=%s&sign=%s";
        
        // 玩家相关操作
        public static $URL_PLAYER_OP = "/appaspx/PlayerOp.aspx?gmAcc=%s&gmPwd=%s&playerAcc=%s&op=%s&sign=%s";
    }
    
    class Tool
    {
        public static function md5($str)
        {
            $data = md5($str);
            $data = strtoupper($data);
            return $data;
        }
        
        public static function get($urlStr)
        {
            $fp = fopen($urlStr, 'r');
            $result = "";
            stream_get_meta_data($fp);
            while(!feof($fp))
            {
                $result .= fgets($fp, 1024);
            }
            return $result;
        }
        
        public static function genGuid() 
        {
            $charid = strtoupper(md5(uniqid(mt_rand(), true)));
            $hyphen = chr(45);// "-"
            $uuid = ""// "{"
            .substr($charid, 0, 8).$hyphen
            .substr($charid, 8, 4).$hyphen
            .substr($charid,12, 4).$hyphen
            .substr($charid,16, 4).$hyphen
            .substr($charid,20,12)
            ;// "}"
            
            $uuid = str_replace("-", "", $uuid);
            return $uuid;
        }
    }
?>