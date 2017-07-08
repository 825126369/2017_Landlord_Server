using System;
using System.Web.Configuration;
using System.Collections.Generic;

// 登陆GM用户相关
public class GMUser : SysMgr
{
    // 登陆帐号名
    public string m_user = "";

    // 帐号权限
    public string m_right = "";

    // 登陆密码
    public string m_pwd = "";

    public string m_type = "";
    
    // 可查看的渠道
    public string m_viewChannel = "";

    // 登陆的帐号IP
    public string m_ip = "";
    // 要操作的数据库IP
    public string m_dbIP="";
    // GM上挂接的DB，每个GM可以操作的db不同
    private int m_dbId = 0;
    // 之前的URL
    private string m_preURL = "";
    private long m_totalRecord = 0;

    // 是否登录了
    private bool m_isLogin = false;

    public string preURL
    {
        get { return m_preURL; }
        set
        {
            if (value != "/appaspx/SelectMachine.aspx")
            {
                m_preURL = value;
            }
        }
    }

    // 用户浏览的总记录个数
    public long totalRecord
    {
        get { return m_totalRecord; }
        set { m_totalRecord = value; }
    }

    public GMUser() { }

    public bool isLogin { get { return m_isLogin; } }

    // 返回挂接的操作DB
    public int getDbServerID()
    {
        return m_dbId;
    }

    // 初始化
    public void init()
    {
        m_dbIP = WebConfigurationManager.AppSettings["account"];
        m_dbId = DBMgr.getInstance().getDbId(m_dbIP);

        addSys(new QueryMgr());
        initSys();
    }

    public void exitLogin()
    {
        exit();
        m_sys.Clear();
    }

    public bool doLogin(string accLogin, string pwdLogin)
    {
        if (m_isLogin)
            return true;

        Dictionary<string, object> ret =
            DBMgr.getInstance().getTableData(TableName.GM_ACCOUNT, "user", accLogin, 0, DbName.DB_ACCOUNT);
        if (ret == null)
            return false;

        string user = Convert.ToString(ret["user"]);
        string tpwd = Convert.ToString(ret["password"]);
        string p = Tool.getMD5Hash(pwdLogin);

        if (user == accLogin && tpwd == p)
        {
            init();

            m_user = user;
            m_type = Convert.ToString(ret["type"]);
            m_viewChannel = Convert.ToString(ret["viewChannel"]);
            m_isLogin = true;
            return true;
        }

        return false;
    }

    // 更换要操作的游戏数据库
    public bool changeGameDb(string pools)
    {
        int id = DBMgr.getInstance().getDbId(pools);
        if (id == -1)
        {
            return false;
        }
        m_dbId = id;
        m_dbIP = pools;
        return true;
    }

    // 查询
    public OpRes doQuery(object param, QueryType queryType)
    {
        QueryMgr mgr = getSys<QueryMgr>(SysType.sysTypeQuery);
        OpRes res = mgr.doQuery(param, queryType, this);
        return res;
    }

    // 返回查询结果
    public object getQueryResult(QueryType queryType)
    {
        QueryMgr mgr = getSys<QueryMgr>(SysType.sysTypeQuery);
        return mgr.getQueryResult(queryType);
    }

    // 可否查看渠道
    public bool canViewChannel(string channelId)
    {
        if (m_viewChannel == "all")
            return true;

        int index = m_viewChannel.IndexOf(channelId);
        return index >= 0;
    }

    // 返回可查看的渠道列表
    public List<string> getViewChannelList()
    {
        List<string> channelList = new List<string>();
        if (m_viewChannel == "all")
        {
            List<ChannelInfo> cList = Channel.getInstance().m_cList;
            foreach (var info in cList)
            {
                channelList.Add(info.channelNo);
            }
        }
        else
        {
            string[] arr = Tool.split(m_viewChannel, ',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in arr)
            {
                channelList.Add(str);
            }
        }

        return channelList;
    }
}


