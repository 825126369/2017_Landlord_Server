using System;
using System.Web.SessionState;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Xml;

/*
    all:拥有所有权限
*/

// 权限的检测结果
enum RightResCode
{
    right_success,              // 成功
    right_not_login,            // 未登陆
    right_no_right,             // 没有权限操作
}

// 权限管理
class RightMgr
{
    private static RightMgr s_mgr = null;
    // 权限串
    Dictionary<string, string> m_rightMap = new Dictionary<string, string>();

    // 从数据库中读出的权限串，人员类型->权限串
    Dictionary<string, string> m_rightCheck = new Dictionary<string, string>();

    public static RightMgr getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new RightMgr();
            s_mgr.init();
        }
        return s_mgr;
    }

    // 判断account是否具有权限right
    public RightResCode hasRight(string right, HttpSessionState session)
    {
        if (session["user"] == null)
        {
            return RightResCode.right_not_login;
        }

        GMUser account = (GMUser)session["user"];
        if(!m_rightCheck.ContainsKey(account.m_type))
            return RightResCode.right_no_right;

        string r = m_rightCheck[account.m_type];

        // 具有所有权限，直接返回true
        if (r.IndexOf("all") >= 0)
            return RightResCode.right_success;
        if (r.IndexOf(right) >= 0)
            return RightResCode.right_success;

        return RightResCode.right_no_right;
    }

    // 对当前要进行的操作进行检验
    public bool opCheck(string right, HttpSessionState session, HttpResponse response)
    {
        RightResCode code = hasRight(right, session);
        if (code == RightResCode.right_success)
            return true;
        if (code == RightResCode.right_not_login)
        {
            response.Redirect("~/Account/Login.aspx");
        }
        if (code == RightResCode.right_no_right)
        {
            response.Redirect("~/appaspx/Error.aspx?right=" + right);
        }
        return false;
    }

    // 返回权限名称
    public string getRrightName(string right)
    {
        if (m_rightMap.ContainsKey(right))
            return m_rightMap[right];
        return "";
    }

    // 获取权限列表
    public Dictionary<string, string> getRightMap()
    {
        return m_rightMap;
    }

    // 从权限表获取所有权限
    public List<Dictionary<string, object>> getAllRight()
    {
        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.RIGHT, 0, DbName.DB_ACCOUNT, null);
        return data;
    }

    // 从权限表获取所有权限
    public string getRight(string type)
    {
        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.RIGHT, 0, DbName.DB_ACCOUNT, null);
        foreach (Dictionary<string, object> dt in data)
        {
            if (Convert.ToString(dt["type"]) == type)
            {
                string str = Convert.ToString(dt["right"]);
                return str;
            }
        }
        return "";
    }

    // 修改人员权限
    public bool modifyRight(string type, string right)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["type"] = type;
        data["right"] = right;
        m_rightCheck[type] = right;
        return DBMgr.getInstance().save(TableName.RIGHT, data, "type", type, 0, DbName.DB_ACCOUNT);
    }

    private void init()
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpRuntime.BinDirectory + "..\\" + "data\\right.xml");

            XmlNode node = doc.SelectSingleNode("/configuration");

            for (node = node.FirstChild; node != null; node = node.NextSibling)
            {
                string right = node.Attributes["right"].Value;
                string fmt = node.Attributes["fmt"].Value;
                if (m_rightMap.ContainsKey(right))
                {
                    LOGW.Info("读right.xml时，发生了错误，出现了重复的权限 {0}", right);
                }
                else
                {
                    m_rightMap.Add(right, fmt);
                }
            }
        }
        catch (System.Exception ex)
        {
            LOGW.Info(ex.Message);
            LOGW.Info(ex.StackTrace);
        }
        finally
        {
            modifyRight("admin", "all");  // 管理员类型
        }

        List<Dictionary<string, object>> allR = getAllRight();
        foreach (var d in allR)
        {
            string st = Convert.ToString(d["type"]);
            string rt = Convert.ToString(d["right"]);
            m_rightCheck[st] = rt;
        }
    }
}




