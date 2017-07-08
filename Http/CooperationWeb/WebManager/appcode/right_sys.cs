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
class RightSys : SysBase
{
    // 权限串
    Dictionary<string, string> m_rightMap = new Dictionary<string, string>();

    // 从数据库中读出的权限串，人员类型->权限串
    Dictionary<string, string> m_rightCheck = new Dictionary<string, string>();

    public RightSys()
    {
        m_sysType = SysType.sysTypeRight;
    }

    public static RightSys getInstance()
    {
        return SysMgr.getGlobalSys<RightSys>(SysType.sysTypeRight);
    }

    // 判断account是否具有权限right
    private RightResCode hasRight(string right, HttpSessionState session)
    {
        if (session["user"] == null)
        {
            return RightResCode.right_not_login;
        }

        GMUser user = (GMUser)session["user"];
        if (!user.isLogin)
            return RightResCode.right_not_login;

        if (right != "") // 为空的，任何账号可以查看，不为空的只有admin可以查看
        {
            if (user.m_type != "admin")
                return RightResCode.right_no_right;
        }
        return RightResCode.right_success;
    }

    // 对当前要进行的操作进行检验
    public bool opCheck(string right, HttpSessionState session, HttpResponse response)
    {
        RightResCode code = hasRight(right, session);
        if (code == RightResCode.right_success)
            return true;
        if (code == RightResCode.right_not_login)
        {
            response.Redirect("~/appaspx/UserLogin.aspx");
        }
        if (code == RightResCode.right_no_right)
        {
            response.Redirect("~/appaspx/Error.aspx");
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

    public override void initSys()
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
                  //  LOGW.Info("读right.xml时，发生了错误，出现了重复的权限 {0}", right);
                }
                else
                {
                    m_rightMap.Add(right, fmt);
                }
            }
        }
        catch (System.Exception ex)
        {
        }
    }
}




