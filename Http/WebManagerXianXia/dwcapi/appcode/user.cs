using System.Web.Configuration;
using System.Collections.Generic;
using System;

public class GMUser
{
    public string m_acc;

    // 账号类型
    public int m_accType;
    public int m_moneyType;
    public long m_money;

    // 所属
    public string m_owner;
    public string m_createCode;
    public int m_depth = 0;

    // 洗码比
    public double m_washRatio;

    // 后缀
    public string m_postfix = "";

    // 开发密钥
    public string m_devSecretKey;

    public bool m_isLogin = false;

    private MySqlDbServer m_sqlDb;

    public MySqlDbServer sqlDb
    {
        get { return m_sqlDb; }
    }

    // 是否登录了
    public bool isLogin { get { return m_isLogin; } }

    public GMUser(ParamBase param)
    {
        string sqlServer = WebConfigurationManager.AppSettings["mysql"];
        m_sqlDb = new MySqlDbServer(sqlServer);

        string sqlCmd = string.Format(CONST.SQL_QUERY_GM_USER,
                                      TableName.GM_ACCOUNT,
                                      param.m_gmAccount,
                                      param.m_gmPwd,
                                      CONST.FIELD_SET[param.fieldIndex]);

        Dictionary<string, object> r = m_sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
        if (r != null)
        {
            do
            {
                m_acc = Convert.ToString(r["acc"]);
                string dbPwd = Convert.ToString(r["pwd"]);
                if (m_acc != param.m_gmAccount)
                {
                    m_isLogin = false;
                    break;
                }
                if (dbPwd != param.m_gmPwd)
                {
                    m_isLogin = false;
                    break;
                }
                m_accType = Convert.ToInt32(r["accType"]);
                if (m_accType != AccType.ACC_API)
                {
                    m_isLogin = false;
                    break;
                }
                m_devSecretKey = Convert.ToString(r["devSecretKey"]);
                readFieldValue(r, param.fieldIndex);
                m_isLogin = true;
            } while (false);
        }
    }

    private void readFieldValue(Dictionary<string, object> r, int fieldIndex)
    {
        if (r == null || string.IsNullOrEmpty(CONST.FIELD_SET[fieldIndex]))
            return;

        string[] arr = Tool.split(CONST.FIELD_SET[fieldIndex], ',', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == "moneyType")
            {
                m_moneyType = Convert.ToInt32(r["moneyType"]);
            }
            else if (arr[i] == "owner")
            {
                m_owner = Convert.ToString(r["owner"]);
            }
            else if (arr[i] == "createCode")
            {
                m_createCode = Convert.ToString(r["createCode"]);
            }
            else if (arr[i] == "money")
            {
                m_money = Convert.ToInt64(r["money"]);
            }
            else if (arr[i] == "postfix")
            {
                m_postfix = Convert.ToString(r["postfix"]);
            }
            else if (arr[i] == "depth")
            {
                m_depth = Convert.ToInt32(r["depth"]);
            }
            else if (arr[i] == "washRatio")
            {
                m_washRatio = Convert.ToDouble(r["washRatio"]);
            }
        }
    }
}

//////////////////////////////////////////////////////////////////

// 一个玩家
public class Player : CCPlayer
{
    public Player() { }

    public Player(string acc, GMUser user)
    {
        string sqlCmd = string.Format(SQL_QUERY,
                        TableName.PLAYER_ACCOUNT_XIANXIA, acc);

        Dictionary<string, object> r = user.sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
        init(acc, r);
    }
}

// 玩家详细
public class PlayerDetail : Player
{
    public static string SQL_QUERY1 = "SELECT acc,creator,money,state,enable,createCode,moneyOnline,createTime FROM {0} where acc='{1}' ";

    public DateTime m_createTime;
    public int m_moneyType;
    public string m_acc = "";

    public PlayerDetail(string acc, GMUser user)
    {
        string sqlCmd = string.Format(SQL_QUERY1, TableName.PLAYER_ACCOUNT_XIANXIA, acc);

        Dictionary<string, object> r = user.sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
        if (r != null)
        {
            do
            {
                string dbAcc = Convert.ToString(r["acc"]);
                if (dbAcc != acc)
                {
                    m_isExists = false;
                    break;
                }
                init(acc, r);
                m_createTime = Convert.ToDateTime(r["createTime"]);
                m_acc = acc;
                m_isExists = true;
            } while (false);
        }
    }
}
