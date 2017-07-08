<?php
    include_once 'def.php';

    class OpMgr
    {
        private $m_gmAcc;
        private $m_gmPwd;
        private $m_devKey;
        
        public function __construct($gmAcc, $gmPwd, $devKey)
        {
            $this->m_gmAcc = $gmAcc;
            $this->m_gmPwd = Tool::md5($gmPwd);
            $this->m_devKey = $devKey;
        }
        
        // 创建玩家
        public function createPlayer($playerAcc, $playerPwd)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $playerPwd . $this->m_devKey);
            $url = sprintf(CC::$URL_CREATE_PLAYER, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $playerPwd, $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 玩家存款  userOrderId 自定义订单ID
        // callBackURL 回调页面
        public function playerSaveMoney($playerAcc, $money, $userOrderId, $callBackURL)
        {
            if ($callBackURL != "")
            {
                $callBackURL = urlencode($callBackURL);
            }
            
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $money . $userOrderId . $this->m_devKey);
            $url = sprintf(CC::$URL_SAVE_MONEY, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $money, $sign, $userOrderId, $callBackURL);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 玩家取款 userOrderId 自定义订单ID
        // callBackURL 回调页面
        public function playerDrawMoney($playerAcc, $money, $userOrderId, $callBackURL)
        {
            if ($callBackURL != "")
            {
                $callBackURL = urlencode($callBackURL);
            }
        
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $money . $userOrderId . $this->m_devKey);
            $url = sprintf(CC::$URL_DRAW_MONEY, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $money, $sign, $userOrderId, $callBackURL);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 查询玩家存取款记录
        public function queryPlayerTradingRecord($playerAcc,
                                                $opType,
                                                $curPage,
                                                $countEachPage,
                                                $startTime,
                                                $endTime)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $this->m_devKey);
            $startTime = urlencode($startTime);
            $endTime = urlencode($endTime);
            
            $url = sprintf(CC::$URL_PLAYER_TRADING_RECORD,
                            $this->m_gmAcc,
                            $this->m_gmPwd,
                            $playerAcc,
                            $curPage,
                            $countEachPage,
                            $startTime,
                            $endTime,
                            $opType,
                            $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 查询玩家信息
        public function queryPlayerInfo($playerAcc)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $this->m_devKey);
            $url = sprintf(CC::$URL_QUERY_PLAYER_INFO, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 查询玩家是否在线
        public function queryPlayerOnline($playerAcc)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $this->m_devKey);
            $url = sprintf(CC::$URL_QUERY_PLAYER_ONLINE, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 登出玩家
        public function logoutPlayer($playerAcc, $forbidTime)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $forbidTime . $this->m_devKey);
            $url = sprintf(CC::$URL_LOGOUT_PLAYER, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $forbidTime, $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 清理登录失败次数
        public function clearLoginFailed($playerAcc)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $this->m_devKey);
            $url = sprintf(CC::$URL_CLEAR_LOGIN_FAILED, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 解锁玩家
        public function unlockPlayer($playerAcc)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $this->m_devKey);
            $url = sprintf(CC::$URL_UNLOCK_PLAYER, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 查询玩家在游戏内的货币变化记录
        public function queryPlayerMoneyChange($playerAcc,
                                                $curPage,
                                                $countEachPage,
                                                $startTime,
                                                $endTime)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $this->m_devKey);
            $startTime = urlencode($startTime);
            $endTime = urlencode($endTime);
            $url = sprintf(CC::$URL_PLAYER_MONEY_CHANGE,
                            $this->m_gmAcc,
                            $this->m_gmPwd,
                            $playerAcc,
                            $curPage,
                            $countEachPage,
                            $startTime,
                            $endTime,
                            $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 查询玩家输赢统计记录
        public function queryPlayerWinLose($playerAcc,
                                            $curPage,
                                            $countEachPage,
                                            $startTime,
                                            $endTime)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $this->m_devKey);
            $startTime = urlencode($startTime);
            $endTime = urlencode($endTime);
            $url = sprintf(CC::$URL_PLAYER_WIN_LOSE,
                            $this->m_gmAcc,
                            $this->m_gmPwd,
                            $playerAcc,
                            $curPage,
                            $countEachPage,
                            $startTime,
                            $endTime,
                            $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 查询订单信息
        public function queryOrderInfo($orderId)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $orderId . $this->m_devKey);
            $url = sprintf(CC::$URL_QUERY_ORDER_INFO,
                            $this->m_gmAcc,
                            $this->m_gmPwd,
                            $orderId,
                            $sign);
        
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 修改玩家密码
        public function updatePlayerPwd($playerAcc, $oldPwd, $newPwd)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $oldPwd . $newPwd . $this->m_devKey);
            $url = sprintf(CC::$URL_UPDATE_PLAYER_PWD,
                            $this->m_gmAcc,
                            $this->m_gmPwd,
                            $playerAcc,
                            $oldPwd,
                            $newPwd,
                            $sign);
        
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // API下的所有玩家的输赢总和
        public function queryWinLoseSum($startTime, $endTime)
        {
            $startTime = urlencode($startTime);
            $endTime = urlencode($endTime);
            
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $this->m_devKey);
            $url = sprintf(CC::$URL_QUERY_WIN_LOSE_SUM,
                            $this->m_gmAcc,
                            $this->m_gmPwd,
                            $startTime,
                            $endTime,
                            $sign);
        
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
        
        // 玩家操作
        public function playerOp($playerAcc, $op)
        {
            $sign = Tool::md5($this->m_gmAcc . $this->m_gmPwd . $playerAcc . $op . $this->m_devKey);
            $url = sprintf(CC::$URL_PLAYER_OP, $this->m_gmAcc, $this->m_gmPwd, $playerAcc, $op, $sign);
            return Tool::get(CC::$SERVER_DOMAIN . $url);
        }
    }
?>


















