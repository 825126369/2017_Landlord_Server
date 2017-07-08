using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
        订单管理
        将处理完毕的订单移入mysql，并根据情况返还gm金币
 */
public class OrderMgr
{
    public const string SQL_SEARCH_GM = "select createCode, depth,devSecretKey,money from {0} where acc='{1}' ";

    public const string SQL_UPDATE_ONLINE_MONEY = "UPDATE {0} set moneyOnline={1} where acc='{2}' ";

    private CMySqlDbServer m_sqlDb;

    public void init()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string mysql = xml.getString("mysql", "");
        string connectStr = xml.getString("connectStr", "");
        m_sqlDb = new CMySqlDbServer(mysql, connectStr);
    }

    // 处理玩家不在线时的订单请求。可能从后台请求时，该玩家在线，但订单到服务器时，该玩家却离线了
    public void processOfflineOrder(OrderInfo info)
    {
        Player p = new Player(m_sqlDb);
        info.m_orderState = p.process(info);

        // 需要重新处理的订单
        if (info.m_orderState == PlayerReqOrderState.STATE_WAIT)
        {
            reWait(info);
        }
        else
        {
            completeOrder(info);
        }
    }

    // 完成一个订单
    public int completeOrder(OrderInfo info)
    {
        string cmd = string.Format(SQL_SEARCH_GM, TableName.GM_ACCOUNT, info.m_gmAcc);
        Dictionary<string, object> data = m_sqlDb.queryOne(cmd, CMySqlDbName.DB_XIANXIA);
        if (data == null)
            return RetCode.RET_NO_SUP_ACC;

        string createCode = Convert.ToString(data["createCode"]);
        int depth = Convert.ToInt32(data["depth"]);
        long remainMoney = Convert.ToInt64(data["money"]);
        bool res = addOrderToMySql(info, createCode, remainMoney);
        if (!res)
            return RetCode.RET_DB_ERROR;

        if (info.m_isApi) // 若是API发出的，需要回调
        {
            string devKey = Convert.ToString(data["devSecretKey"]);
            ApiCallBackService sys = ServiceMgr.getInstance().getSys<ApiCallBackService>(ServiceType.serviceTypeApiCallBack);
            sys.addApiOrder(info, devKey);
        }

        forPlayerOrder(info);

        // 插入mysql成功，从mongodb里面删除
        MongodbPlayer.Instance.ExecuteRemoveBykey(TableName.PLAYER_ORDER_REQ, "key", info.m_key);
        returnMoneyToGM(info);

        // 修改player_account的money临时字段，可以在后台随时查看分数
        if (info.m_orderState == PlayerReqOrderState.STATE_FINISH)
        {
          //  scoreLog(info, depth, createCode); // 上下分成功后，才会写日志

            string upSql = string.Format(SQL_UPDATE_ONLINE_MONEY,
                                        TableName.PLAYER_ACCOUNT_XIANXIA,
                                        info.m_playerRemainMoney,
                                        info.m_playerAcc);
            m_sqlDb.executeOp(upSql, CMySqlDbName.DB_XIANXIA);
        }

        return RetCode.RET_SUCCESS;
    }

    private bool addOrderToMySql(OrderInfo info, string createCode, long opRemainMoney)
    {
        //string sqlCmd = OrderGenerator.genCmdOrderToMySql(info, createCode);
        // 生成上下分记录
        string sqlCmd = OrderGenerator.genSqlForLogScore(info, createCode, opRemainMoney);
        int count = m_sqlDb.executeOp(sqlCmd, CMySqlDbName.DB_XIANXIA);
        return count > 0;
    }

    /*
     *      gm给玩家上分，先扣gm的钱，若订单处理失败，需要把钱返还gm
     *      gm给玩家下分，先处理订单，若订单处理成功，则需要给gm钱
     */
    private void returnMoneyToGM(OrderInfo info)
    {
        if (info.m_gmAcc == "admin") // 操作者是管理员，不用返还
            return;

        long retMoney = 0;
        switch (info.m_orderState)
        {
            case PlayerReqOrderState.STATE_FINISH: // 处理成功了，若是下分，需要返还
                {
                    if (!ScropOpType.isAddScore(info.m_orderType))
                    {
                        retMoney = info.m_money;
                    }
                }
                break;
            case PlayerReqOrderState.STATE_FAILED: // 处理失败了，若是上分，需要返还
                {
                    if (ScropOpType.isAddScore(info.m_orderType))
                    {
                        retMoney = info.m_money;
                    }
                }
                break;
        }

        if (retMoney > 0)
        {
            SqlUpdateGenerator up = new SqlUpdateGenerator();
            up.addField("money",
                        string.Format("money+{0}", retMoney),
                        FieldType.TypeNumber);

            // 可以通知到web后台
            string cmd = up.getResultSql(TableName.GM_ACCOUNT, string.Format("acc='{0}'", info.m_gmAcc));
            m_sqlDb.executeOp(cmd, CMySqlDbName.DB_XIANXIA);
        }
    }

    // 增加一条上下分日志
    private void scoreLog(OrderInfo info, int depth, string createCode)
    {
        string cmd = string.Format(SqlStrCMD.SQL_CMD_PLAYER_SCORE,
                                    TableName.GM_SCORE,
                                    DateTime.Now.ToString(ConstDef.DATE_TIME24),
                                    info.m_gmAcc,
                                    info.m_playerAcc,
                                    info.m_orderType,
                                    info.m_money,
                                    0,
                                    depth,
                                    createCode,
                                    AccType.ACC_PLAYER,
                                    info.m_playerRemainMoney);

        m_sqlDb.executeOp(cmd, CMySqlDbName.DB_XIANXIA);
    }

    // 订单重新等待处理
    public void reWait(OrderInfo info)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("orderState", PlayerReqOrderState.STATE_WAIT);
        data.Add("tryCount", 0);
        data.Add("lastProcessTime", DateTime.Now);
        MongodbPlayer.Instance.ExecuteUpdate(TableName.PLAYER_ORDER_REQ, "key", info.m_key, data);
    }

    // 对玩家自己在游戏内提交的订单状态进行修改
    void forPlayerOrder(OrderInfo info)
    {
        if (info.m_orderFrom != OrderGenerator.ORDER_FROM_PLAYER_ORDER)
            return;

        if (info.m_orderState == PlayerReqOrderState.STATE_FAILED) // 订单处理失败
        {
            // 订单处理失败，还原成等待状态
            string upcmd = CPlayerOrder.genUpdateSql(info.m_orderId, OrderState.STATE_WAIT);
            m_sqlDb.executeOp(upcmd, CMySqlDbName.DB_XIANXIA);
        }
        else
        {
            string selcmd = string.Format(" SELECT * from {0} where {1}", TableName.PLAYER_ORDER_WAIT,
                                          string.Format("orderId= '{0}' ", info.m_orderId));
            Dictionary<string, object> data = m_sqlDb.queryOne(selcmd, CMySqlDbName.DB_XIANXIA);
            ResultPlayerOrderItem item = CPlayerOrder.toOrder(data);
            if (item == null)
                return;

            string delcmd = CPlayerOrder.genRemoveSql(info.m_orderId);
            string inscmd = CPlayerOrder.genInsertSql(item, info.m_gmAcc, OrderState.STATE_FINISH);
            m_sqlDb.executeOp(delcmd, CMySqlDbName.DB_XIANXIA);
            m_sqlDb.executeOp(inscmd, CMySqlDbName.DB_XIANXIA);
        }
    }
}

//////////////////////////////////////////////////////////////////////////
class Player
{
    public const string SQL = "select money,state from {0} where acc='{1}' ";

    private CMySqlDbServer m_sqlDb;
    private long m_money;
    private int m_state;

    public Player(CMySqlDbServer sqlDb)
    {
        m_sqlDb = sqlDb;
    }

    // 返回true成功
    private bool query(string playerAcc)
    {
        string cmd = string.Format(SQL, TableName.PLAYER_ACCOUNT_XIANXIA, playerAcc);
        Dictionary<string, object> data = m_sqlDb.queryOne(cmd, CMySqlDbName.DB_XIANXIA);
        if (data == null)
            return false;

        m_money = Convert.ToInt64(data["money"]);
        m_state = Convert.ToInt32(data["state"]);
        return true;
    }

    // 返回处理后订单的新状态
    public int process(OrderInfo info)
    {
        if (info.m_money <= 0)
        {
            info.m_failReason = RetCode.RET_MONEY_NOT_VALID;
            return PlayerReqOrderState.STATE_FAILED;
        }

        bool exist = query(info.m_playerAcc);
        if (!exist)
        {
            info.m_failReason = RetCode.RET_NO_PLAYER;
            return PlayerReqOrderState.STATE_FAILED;
        }

        Dictionary<string, object> data =
            MongodbPlayer.Instance.ExecuteGetBykey(TableName.PLAYER_INFO, "account", info.m_playerAcc, new string[] { "SyncLock" });
        if (data == null)
        {
            info.m_failReason = RetCode.RET_NO_PLAYER;
            return PlayerReqOrderState.STATE_FAILED;
        }

        if (data.ContainsKey("SyncLock"))
        {
            int LockS = Convert.ToInt32(data["SyncLock"]);
            if (LockS == 2) // 玩家被锁了
            {
                info.m_failReason = RetCode.RET_PLYAER_LOCKED;
                return PlayerReqOrderState.STATE_FAILED;
            }
        }

        if (m_state == PlayerState.STATE_GAME) // 订单返回到这里，发现玩家又上线了，订单重发。
        {
            info.m_failReason = RetCode.RET_PLYAER_LOCKED;
            return PlayerReqOrderState.STATE_WAIT;
        }

        SqlUpdateGenerator up = new SqlUpdateGenerator();

        switch (info.m_orderType)
        {
            case ScropOpType.ADD_SCORE: // 上分订单
                {
                    up.addField("money",
                                string.Format("money+{0}", info.m_money),
                                FieldType.TypeNumber);
                }
                break;
            case ScropOpType.EXTRACT_SCORE: // 下分订单
                {
                    if (m_money < info.m_money)
                    {
                        info.m_failReason = RetCode.RET_MONEY_NOT_ENOUGH;
                        return PlayerReqOrderState.STATE_FAILED;
                    }

                    up.addField("money",
                               string.Format("money-{0}", info.m_money),
                               FieldType.TypeNumber);
                }
                break;
        }

        // 可以通知到web后台
        string cmd = up.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
            string.Format("acc='{0}' and state={1} ", info.m_playerAcc, PlayerState.STATE_IDLE));

        int count = m_sqlDb.executeOp(cmd, CMySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            info.m_playerRemainMoney = getRemainMoney(info.m_playerAcc);
            return PlayerReqOrderState.STATE_FINISH;
        }

        info.m_failReason = RetCode.RET_DB_ERROR;
        return PlayerReqOrderState.STATE_FAILED;
    }

    // 返回玩家账号余额
    public long getRemainMoney(string acc)
    {
        SqlSelectGenerator gen = new SqlSelectGenerator();
        gen.addField("money");
        string sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
                                    string.Format("acc='{0}'", acc));

        Dictionary<string, object> data = m_sqlDb.queryOne(sql, CMySqlDbName.DB_XIANXIA);
        if (data == null)
            return 0;

        return Convert.ToInt64(data["money"]);
    }
}


