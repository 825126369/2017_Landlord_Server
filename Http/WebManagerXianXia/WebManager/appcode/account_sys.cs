using System;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

public class AccountInfo
{
    public string m_user = "";
    public string m_right = "";
    public string m_pwd = "";
    public string m_ip = "";  // 帐号IP
    public int m_accType;
    public int m_moneyType;
    
    public string m_owner = "";
    public string m_generalAgency = "";
    public string m_postfix = "";

    public long m_money;
    public int m_depth;

    public string m_createCode = "";
    public int m_childCount;

    // 代理占成
    public double m_agentRatio;
    // 洗码比
    public double m_washRatio;

    public string m_verCode = "";

    public int m_autoForward;
}

public enum enumLoginResult
{
    result_success,      // 成功
    result_has_login,    // 已登陆
    result_error,        // 账号或密码错误
}

public class LoginResult
{
    public AccountInfo m_info;
    public enumLoginResult m_code = enumLoginResult.result_error;

    public bool isSuccess()
    {
        return m_code == enumLoginResult.result_success;
    }
}

public class AccountType
{
    public AccountType(string type, string name)
    {
        m_type = type;
        m_name = name;
    }

    public string m_type;
    public string m_name;
}

public class AccountSys : SysBase
{
    public const string LOGIN_SQL = " SELECT acc,pwd,state,accType,moneyType,owner,generalAgency,postfix,money,gmRight," +
                                    " depth,createCode,agentRatio,washRatio,validatedCode,childNodeNumber,forwardOrder " +
                                    " FROM {0} where acc='{1}' and pwd='{2}' ";

    private static AccountSys s_mgr = null;
    // 存储登陆用户表，用户名与会话映射
    private Dictionary<string, HttpSessionState> m_user = new Dictionary<string, HttpSessionState>();
    // 账号类型列表
    private List<AccountType> m_accountType = new List<AccountType>();
    private object m_lockObj = new object();

    public AccountSys()
    {
        m_sysType = SysType.sysTypeAccount;
    }

    public static AccountSys getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new AccountSys();
        }
        return s_mgr;
    }

    // 登陆处理
    public LoginResult onLogin(string user, string pwd)
    {
        LoginResult result = new LoginResult();

        string sqlCmd = string.Format(LOGIN_SQL, TableName.GM_ACCOUNT, user, Tool.getMD5Hash(pwd));

        MySqlDb sql = new MySqlDb();

        Dictionary<string, object> r = sql.queryOne(sqlCmd, 0, MySqlDbName.DB_XIANXIA);
        if(r != null)
        {
            do 
            {
                AccountInfo info = new AccountInfo();
                info.m_user = Convert.ToString(r["acc"]);
                string dbPwd = Convert.ToString(r["pwd"]);
                if (info.m_user != user)
                {
                    result.m_code = enumLoginResult.result_error;
                    break;
                }
                if (dbPwd != Tool.getMD5Hash(pwd))
                {
                    result.m_code = enumLoginResult.result_error;
                    break;
                }
                int state = Convert.ToInt32(r["state"]);
                if (state == GmState.STATE_BLOCK) // 账号被停封，无法登录
                {
                    result.m_code = enumLoginResult.result_error;
                    break;
                }
                info.m_accType = Convert.ToInt32(r["accType"]);
                info.m_moneyType = Convert.ToInt32(r["moneyType"]);
                info.m_owner = Convert.ToString(r["owner"]);
                info.m_generalAgency = Convert.ToString(r["generalAgency"]);
                info.m_postfix = Convert.ToString(r["postfix"]);
                info.m_money = Convert.ToInt64(r["money"]);
                info.m_right = Convert.ToString(r["gmRight"]);
                info.m_depth = Convert.ToInt32(r["depth"]);
                info.m_createCode = Convert.ToString(r["createCode"]);
                if (!(r["agentRatio"] is DBNull))
                {
                    info.m_agentRatio = Convert.ToDouble(r["agentRatio"]);
                }
                if (!(r["washRatio"] is DBNull))
                {
                    info.m_washRatio = Convert.ToDouble(r["washRatio"]);
                }
                if (!(r["validatedCode"] is DBNull))
                {
                    info.m_verCode = Convert.ToString(r["validatedCode"]);
                }
                info.m_childCount = (int)sql.getRecordCount(TableName.GM_ACCOUNT,
                    string.Format("owner='{0}' ", user), 0, MySqlDbName.DB_XIANXIA);

                int curNum = 0;
                if (!(r["childNodeNumber"] is DBNull))
                {
                    curNum = Convert.ToInt32(r["childNodeNumber"]);
                }
                if (info.m_childCount < curNum)
                {
                    info.m_childCount = curNum;
                }

                if ((r["forwardOrder"] is DBNull))
                {
                    info.m_autoForward = 0;
                }
                else
                {
                    info.m_autoForward = Convert.ToInt32(r["forwardOrder"]);
                }
                info.m_pwd = dbPwd;
                result.m_code = enumLoginResult.result_success;
                result.m_info = info;
            } while (false);
        }

        return result;
    }

    public void onLoginSuccess(HttpSessionState session, LoginResult res, string ip, bool keep)
    {
        if (res.isSuccess())
        {
            res.m_info.m_ip = ip;
            GMUser gmusr = new GMUser(res.m_info);
            gmusr.init();
            // 设置用户信息
            session["user"] = gmusr;
            onUserLogin(gmusr);

            HttpContext.Current.Application.Lock();

            if (m_user.ContainsKey(res.m_info.m_user))
            {
                m_user[res.m_info.m_user].Abandon();
                m_user[res.m_info.m_user]["user"] = null;
                m_user[res.m_info.m_user]["occupy"] = true;
                m_user[res.m_info.m_user] = session;
            }
            else
            {
                m_user[res.m_info.m_user] = session;
            }
            if (keep)
            {
                // 保持一天的登陆状态
                session.Timeout = 24 * 60; 
            }

            HttpContext.Current.Application.UnLock();
        }
    }

    public bool onLoginVerification(string veriStr, HttpSessionState session, string validatedCode = "")
    {
        UserVerification verInfo = (UserVerification)session[DefCC.KEY_VERIFICATION];
        if (verInfo == null)
            return false;

        switch (verInfo.loginHasFinishStep)
        {
            case 0: // 验证账号
                {
                    if(validatedCode.Trim().ToLower() != verInfo.m_validatedCode.ToLower())
                        return false;

                    if (!Regex.IsMatch(veriStr, Exp.CHECK_LOGIN))
                    {
                        return false;
                    }

                    MySqlDb sql = new MySqlDb();

                    string sqlCmd = string.Format("SELECT acc,pwd,validatedCode FROM {0} where acc='{1}' ",
                        TableName.GM_ACCOUNT, veriStr);
                    Dictionary<string, object> r = sql.queryOne(sqlCmd, 0, MySqlDbName.DB_XIANXIA);
                    if (r != null)
                    {
                        verInfo.m_acc = Convert.ToString(r["acc"]);
                        if (verInfo.m_acc == veriStr)
                        {
                            verInfo.m_pwd = Convert.ToString(r["pwd"]);
                            if (r.ContainsKey("validatedCode"))
                            {
                                if (!(r["validatedCode"] is DBNull))
                                {
                                    verInfo.m_dbValidatedCode = Convert.ToString(r["validatedCode"]);
                                }
                            }
                            verInfo.loginHasFinishStep++;
                            return true;
                        }
                    }
                }
                break;
            case 1: // 验证密码
                {
                    if (verInfo.m_pwd == Tool.getMD5Hash(veriStr))
                    {
                        verInfo.m_pwd1 = veriStr;
                        verInfo.loginHasFinishStep++;
                        return true;
                    }
                }
                break;
            case 2: // 验证四位固定code
                {
                    if (string.IsNullOrEmpty(verInfo.m_dbValidatedCode)) // 暂兼容之前的账号
                        return true;

                    if (verInfo.m_dbValidatedCode == veriStr.Trim())
                    {
                        return true;
                    }
                }
                break;
            default:
                {
                }
                break;
        }
        return false;
    }

    // 会话退出
    public void sessionEnd(HttpSessionState session)
    {
        if (session["user"] == null)
            return;

        GMUser info = (GMUser)session["user"];
        lock (m_lockObj)
        {
            if (m_user.ContainsKey(info.m_user))
            {
                m_user.Remove(info.m_user);
                session.Clear();
            }
        }
    }

    // 用户user是否已登陆
    public bool isLogin(string user)
    {
        return m_user.ContainsKey(user);
    }

    public GMUser getUser(string userAcc)
    {
        if (!m_user.ContainsKey(userAcc))
            return null;

        HttpSessionState hs = m_user[userAcc];
        return (GMUser)hs["user"];
    }

    private void onUserLogin(GMUser user)
    {
        /*SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("loginAcc", user.m_user, FieldType.TypeString);
        gen.addField("loginIP", user.m_ip, FieldType.TypeString);
        gen.addField("loginTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        string sql = gen.getResultSql(TableName.GM_LOGIN_LOG);
        user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        // 更新IP
        SqlUpdateGenerator up = new SqlUpdateGenerator();
        up.addField("lastLoginIP", user.m_ip, FieldType.TypeString);
        string cmd = up.getResultSql(TableName.GM_ACCOUNT, string.Format("acc='{0}'", user.m_user));
        user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);*/
    }
}



















