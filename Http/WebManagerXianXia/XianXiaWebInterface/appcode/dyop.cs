using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;
using System.Web;
using System.IO;

// 动态操作
public class DyOpBase
{
    public virtual string doDyop(object param)
    {
        return "";
    }

    public string returnMsg(int retCode, Dictionary<string, object> data = null)
    {
        if (data == null)
        {
            data = new Dictionary<string, object>();
        }
        data["result"] = retCode;
        string jsondata = JsonHelper.ConvertToStr(data);
        return Convert.ToBase64String(Encoding.Default.GetBytes(jsondata));
    }
}

//////////////////////////////////////////////////////////////////////////
// 提交订单
public class ParamCommitOrder
{
    public string m_playerAcc;      // 玩家账号
    public string m_moneyStr;       // 下单金额 元
    public string m_orderTypeStr;   // 订单类型
    public string m_sign;

    public int m_money;             // 下单金额 元
    public int m_orderType;

    public int isParamValid()
    {
        if (string.IsNullOrEmpty(m_playerAcc))
            return RetCode.RET_PARAM_NOT_VALID;

        if (string.IsNullOrEmpty(m_moneyStr))
            return RetCode.RET_PARAM_NOT_VALID;

        if (string.IsNullOrEmpty(m_orderTypeStr))
            return RetCode.RET_PARAM_NOT_VALID;

        if (string.IsNullOrEmpty(m_sign))
            return RetCode.RET_PARAM_NOT_VALID;

        if (!int.TryParse(m_moneyStr, out m_money))
            return RetCode.RET_PARAM_NOT_VALID;

        if(m_money <= 0)
            return RetCode.RET_PARAM_NOT_VALID;

        if (!int.TryParse(m_orderTypeStr, out m_orderType))
        {
            return RetCode.RET_PARAM_NOT_VALID;
        }

        if (m_orderType != ScropOpType.ADD_SCORE && m_orderType != ScropOpType.EXTRACT_SCORE)
        {
            return RetCode.RET_PARAM_NOT_VALID;
        }

        string sign = Tool.getMD5Hash(m_playerAcc + m_moneyStr + m_orderTypeStr + DyOpPlayerCommitOrder.AES_KEY);
        if(sign != m_sign)
            return RetCode.RET_SIGN_ERROR;

        return 0;
    }
}

// 提交订单
public class DyOpPlayerCommitOrder : DyOpBase
{
    public const string AES_KEY = "&@*(#kas250917fajk!)";

    public static string SQL_CMD = "select acc,creator,createCode from {0} where acc='{1}' ";

    public static string SQL_CMD_OP_ACC = "select owner,forwardOrder from {0} where acc='{1}' ";

    public override string doDyop(object param)
    {
        ParamCommitOrder p = (ParamCommitOrder)param;
        int code = p.isParamValid();
        if (code != 0)
            return returnMsg(code);

        if (!Regex.IsMatch(p.m_playerAcc, Exp.ACCOUNT_PLAYER))
        {
            return returnMsg(RetCode.RET_ACC_PWD_FORMAT_ERROR);
        }

        MySqlDbServer sqlDb = new MySqlDbServer(CC.MYSQL_IP);

        string cmd = string.Format(SQL_CMD, TableName.PLAYER_ACCOUNT_XIANXIA, p.m_playerAcc);
        Dictionary<string, object> data = sqlDb.queryOne(cmd, MySqlDbName.DB_XIANXIA);
        if (data == null)
        {
            return returnMsg(RetCode.RET_NO_PLAYER);
        }
        string acc = Convert.ToString(data["acc"]);
        if (acc != p.m_playerAcc)
        {
            return returnMsg(RetCode.RET_NO_PLAYER);
        }

        string creatorAcc = Convert.ToString(data["creator"]);

        string opAcc = getOpAcc(creatorAcc, sqlDb);
        if (string.IsNullOrEmpty(opAcc))
        {
            return returnMsg(RetCode.RET_NO_SUP_ACC);
        }

        string createCode = Convert.ToString(data["createCode"]);

        string orderId = OrderGenerator.genOrderId(opAcc, p.m_playerAcc);
        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("orderId", orderId, FieldType.TypeString);
        gen.addField("orderTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        gen.addField("playerAcc", p.m_playerAcc, FieldType.TypeString);
        gen.addField("playerOwner", creatorAcc, FieldType.TypeString);
        gen.addField("curOpAcc", opAcc, FieldType.TypeString);
        gen.addField("orderState", OrderState.STATE_WAIT, FieldType.TypeNumber);
        gen.addField("playerOwnerCreator", createCode, FieldType.TypeString);
        gen.addField("orderMoney", p.m_money, FieldType.TypeNumber);
        gen.addField("orderType", p.m_orderType, FieldType.TypeNumber);

        string sqlCmd = gen.getResultSql(TableName.PLAYER_ORDER_WAIT);
        int count = sqlDb.executeOp(sqlCmd, MySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret.Add("orderId", orderId);
            return returnMsg(RetCode.RET_SUCCESS);
        }

        return returnMsg(RetCode.RET_DB_ERROR);
    }

    // 返回可以处理订单的账号
    public string getOpAcc(string creatorAcc, MySqlDbServer sqlDb)
    {
        string retAcc = "";
        do
        {
            string cmd = string.Format(SQL_CMD_OP_ACC, TableName.GM_ACCOUNT, creatorAcc);
            Dictionary<string, object> data = sqlDb.queryOne(cmd, MySqlDbName.DB_XIANXIA);
            if (data == null)
                break;

            bool isAutoForward = false;
            if (!(data["forwardOrder"] is DBNull))
            {
                isAutoForward = Convert.ToBoolean(data["forwardOrder"]);
            }

            if (!isAutoForward)
            {
                retAcc = creatorAcc;
                break;
            }
            else
            {
                creatorAcc = Convert.ToString(data["owner"]);
            }
        } while (true);

        if (retAcc == "")
        {
            retAcc = "admin";
        }
        return retAcc;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamFetchMoney
{
    public string m_acc;

    public int isParamValid()
    {
        if (string.IsNullOrEmpty(m_acc))
        {
            return RetCode.RET_PARAM_NOT_VALID;
        }

        return 0;
    }
}

// 玩家登录游戏时，从后台取钱
public class DyOpFetchMoney : DyOpBase
{
    // 更新语句
    public static string SQL_UPDATE = " UPDATE {0} set state={1},lastLoginDate='{2}' where acc='{3}' ";

    // 仅更新状态
    public static string SQL_UPDATE_STATE = " UPDATE {0} set state={1} where acc='{2}' ";

    public static string SQL_UPDATE_ONLINE_MONEY = " UPDATE {0} set moneyOnline={1} where acc='{2}' ";

    // 查询语句
    public static string SQL_QUERY = " select {0}.money,enable,accType,home,{0}.createCode,gameClose from {0},{1} WHERE {0}.acc='{2}' and {0}.creator={1}.acc ";

    // 仅查询状态
    public static string SQL_QUERY_STATE = " select state from {0} WHERE acc='{1}' ";

    public override string doDyop(object param)
    {
        ParamFetchMoney p = (ParamFetchMoney)param;
        int code = p.isParamValid();
        if (code != 0)
            return returnMsg(code);

        MySqlDbServer sqlDb = new MySqlDbServer(CC.MYSQL_IP);

        string sqlCmd = string.Format(SQL_QUERY_STATE,
                                      TableName.PLAYER_ACCOUNT_XIANXIA,
                                      p.m_acc);
        Dictionary<string, object> data = sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
        if (data == null)
        {
            return returnMsg(RetCode.RET_NO_PLAYER);
        }

        // 取出原有状态
        int oriState = Convert.ToInt32(data["state"]);

        // sql命令
        string sqlCmd1 = string.Format(SQL_UPDATE,
                       TableName.PLAYER_ACCOUNT_XIANXIA,
                       PlayerState.STATE_GAME,
                       DateTime.Now.ToString(ConstDef.DATE_TIME24),
                       p.m_acc);

        int count = sqlDb.executeOpTran(sqlCmd1, MySqlDbName.DB_XIANXIA);
        if (count <= 0) // 没有成功
        {
            return returnMsg(RetCode.RET_NO_PLAYER);
        }

        string sqlCmd2 = string.Format(SQL_QUERY, TableName.PLAYER_ACCOUNT_XIANXIA, TableName.GM_ACCOUNT, p.m_acc);

        data = sqlDb.queryOne(sqlCmd2, MySqlDbName.DB_XIANXIA);
        if (data != null)
        {
            bool enable = true;
            if (!(data["enable"] is DBNull))
            {
                enable = Convert.ToBoolean(data["enable"]);
            }

            long money = Convert.ToInt64(data["money"]);

            Dictionary<string, object> ret = getRetInfo(p, money, data);
            
            if (enable)
            {
                setOnlineMoney(sqlDb, money, p);
                return returnMsg(RetCode.RET_SUCCESS, ret);
            }
            else // 被封号，将状态置为正常
            {
                if (oriState == PlayerState.STATE_IDLE) // 原有状态为离线，恢复
                {
                    string sqlCmd3 = string.Format(SQL_UPDATE_STATE,
                                                   TableName.PLAYER_ACCOUNT_XIANXIA,
                                                   PlayerState.STATE_IDLE,
                                                   p.m_acc);

                    sqlDb.executeOp(sqlCmd3, MySqlDbName.DB_XIANXIA);
                }
               
                return returnMsg(RetCode.RET_ACC_BLOCKED, ret);
            }
        }

        return returnMsg(RetCode.RET_NO_PLAYER);
    }

    void setOnlineMoney(MySqlDbServer sqlDb, long money, ParamFetchMoney p)
    {
        string sqlCmd = string.Format(SQL_UPDATE_ONLINE_MONEY,
                                        TableName.PLAYER_ACCOUNT_XIANXIA,
                                        money,
                                        p.m_acc);

        sqlDb.executeOp(sqlCmd, MySqlDbName.DB_XIANXIA);
    }

    Dictionary<string, object> getRetInfo(ParamFetchMoney p, long money, Dictionary<string, object> data)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();
        int atype = Convert.ToInt32(data["accType"]);

        ret["acc"] = p.m_acc;
        ret["money"] = money;
        ret["isApi"] = (atype == AccType.ACC_API);
        if (data["home"] is DBNull)
        {
            ret["home"] = "";
        }
        else
        {
            ret["home"] = Convert.ToString(data["home"]);
        }
        ret["creator"] = Convert.ToString(data["createCode"]);

        if (data["gameClose"] is DBNull)
        {
            ret["gameClose"] = "";
        }
        else
        {
            ret["gameClose"] = Convert.ToString(data["gameClose"]);
        }
        return ret;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamSaveMoney
{
    public string m_acc;
    public string m_moneyStr;
    public string m_exit;

    public long m_money;

    // 0退出到登录状态  1退回到大厅
    public int m_exitType = 0; 

    public int isParamValid()
    {
        if (string.IsNullOrEmpty(m_acc))
        {
            return 1;
        }
        if (string.IsNullOrEmpty(m_moneyStr))
        {
            return 1;
        }

        if (!string.IsNullOrEmpty(m_exit))
        {
            m_exitType = Convert.ToInt32(m_exit);
        }

        m_money = Convert.ToInt64(m_moneyStr);

        return 0;
    }
}

// 玩家退出游戏时，游戏服务器同步gold到后台
public class DyOpSaveMoney : DyOpBase
{
    // 更新语句
    public static string SQL_UPDATE = " UPDATE {0} set state={1},money={2} where acc='{3}' ";

    // 查询语句
    public static string SQL_QUERY = " select state,money from {0} WHERE acc='{1}' ";

    // 仅更新玩家临时money字段
    public static string SQL_UPDATE_TEMP_MONEY = " UPDATE {0} set moneyOnline={1} where acc='{2}' ";

    public override string doDyop(object param)
    {
        ParamSaveMoney p = (ParamSaveMoney)param;
        int code = p.isParamValid();
        if (code != 0)
            return "";

        MySqlDbServer sqlDb = new MySqlDbServer(CC.MYSQL_IP);
        if (p.m_exitType == 0)
        {
            return exitGame(sqlDb, p);
        }

        return exitToLobby(sqlDb, p);
    }

    private string exitGame(MySqlDbServer sqlDb, ParamSaveMoney p)
    {
        // sql命令
        string sqlCmd = string.Format(SQL_QUERY,
                                      TableName.PLAYER_ACCOUNT_XIANXIA,
                                      p.m_acc);

        Dictionary<string, object> data = sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
        if (data != null)
        {
            int state = Convert.ToInt32(data["state"]);
            if (state != PlayerState.STATE_GAME)
            {
                return "";
            }

            long oriMoney = Convert.ToInt64(data["money"]);

            sqlCmd = string.Format(SQL_UPDATE,
                                   TableName.PLAYER_ACCOUNT_XIANXIA,
                                   PlayerState.STATE_IDLE,
                                   p.m_money,
                                   p.m_acc);
            int count = sqlDb.executeOp(sqlCmd, MySqlDbName.DB_XIANXIA);

            if (count > 0)
            {
                addGameLog(sqlDb, p.m_acc, oriMoney, p.m_money);
                return "ok";
            }
        }

        return "";
    }

    public string exitToLobby(MySqlDbServer sqlDb, ParamSaveMoney p)
    {
        string sqlCmd = string.Format(SQL_UPDATE_TEMP_MONEY,
                                  TableName.PLAYER_ACCOUNT_XIANXIA,
                                  p.m_money,
                                  p.m_acc);
        int count = sqlDb.executeOp(sqlCmd, MySqlDbName.DB_XIANXIA);
        return count > 0 ? "ok" : "";
    }

    // 添加进出日志
    private void addGameLog(MySqlDbServer db, string acc, long oriMoney, long curMoney)
    {
        string cmd = string.Format(SqlCmdStr.SQL_ADD_GAME_LOG,
                                    TableName.PLAYER_GAME_SCORE,
                                    acc,
                                    oriMoney,
                                    curMoney,
                                    DateTime.Now.ToString(ConstDef.DATE_TIME24));

        db.executeOp(cmd, MySqlDbName.DB_XIANXIA);
    }
}











