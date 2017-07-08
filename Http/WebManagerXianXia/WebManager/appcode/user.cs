using System.Web.Configuration;
using System.Collections.Generic;
using System.Web.SessionState;
using System.Web;

// 登陆GM用户相关
public class GMUser : SysMgr
{
    // 登陆帐号名
    public string m_user = "";
    // 帐号权限
    public string m_right = "";
    // 登陆密码
    public string m_pwd = "";
    // 四位固定验证码
    public string m_verCode = "";

    public int m_depth = 0;

    public string m_createCode = "";
    public int m_childCount;

    // 账号类型
    public int m_accType;
    public int m_moneyType;
    public long m_money;

    // 所属
    public string m_owner = "";
    
    // 总代理名称
    public string m_generalAgency = "";
    // 当前账号的后缀
    public string m_postfix = "";

    // 代理占成
    public double m_agentRatio;
    // 洗码比
    public double m_washRatio;

    public int m_autoForward;

    // 登陆的帐号IP
    public string m_ip = "";
    // 要操作的数据库IP
    public string m_dbIP="";
    // GM上挂接的DB，每个GM可以操作的db不同
    private int m_dbId = 0;

    // 之前的URL
    private string m_preURL = "";
    private long m_totalRecord = 0;

    private MySqlDb m_sqlDb;

    // 是否切换了服务器
    private bool m_isSwitchDbServer = false;

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

    public bool isSwitchDbServer
    {
        get { return m_isSwitchDbServer; }
    }

    public MySqlDb sqlDb
    {
        get { return m_sqlDb; }
    }

    public GMUser() { }

    public GMUser(AccountInfo info)
    {
        m_user = info.m_user;
        m_right = info.m_right;
        m_pwd = info.m_pwd;
        m_ip = info.m_ip;
        m_accType = info.m_accType;
        m_moneyType = info.m_moneyType;
        m_owner = info.m_owner;
        m_generalAgency = info.m_generalAgency;
        m_postfix = info.m_postfix;
        m_money = info.m_money;
        m_depth = info.m_depth;
        m_createCode = info.m_createCode;
        m_childCount = info.m_childCount;
        m_agentRatio = info.m_agentRatio;
        m_washRatio = info.m_washRatio;
        m_verCode = info.m_verCode;
        m_autoForward = info.m_autoForward;
    }

    // 返回挂接的操作DB
    public int getDbServerID()
    {
        return m_dbId;
    }

    public int getMySqlServerID()
    {
        return 0;
    }

    // 初始化
    public void init()
    {
        m_isSwitchDbServer = false;

        m_sqlDb = new MySqlDb();

        addSys(new QueryMgr());
        addSys(new DyOpMgr());
        addSys(new StatMgr());
        addSys(new ExportMgr());
        addSys(new OpLevelMgr());

        string conn = WebConfigurationManager.AppSettings["connectStrMongo"];
        changeGameDb(conn);

        initSys();
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

        m_isSwitchDbServer = true;
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

    // 返回查询结果
    public object getQueryResult(object param, QueryType queryType)
    {
        QueryMgr mgr = getSys<QueryMgr>(SysType.sysTypeQuery);
        return mgr.getQueryResult(param, queryType, this);
    }

    public OpRes doStat(object param, StatType statName)
    {
        StatMgr mgr = getSys<StatMgr>(SysType.sysTypeStat);
        return mgr.doStat(param, statName, this);
    }

    public object getStatResult(StatType statName)
    {
        StatMgr mgr = getSys<StatMgr>(SysType.sysTypeStat);
        return mgr.getStatResult(statName);
    }

    public OpRes doDyop(object param, DyOpType type)
    {
        DyOpMgr mgr = getSys<DyOpMgr>(SysType.sysTypeDyOp);
        return mgr.doDyop(param, type, this);
    }

    // 操作层级
    public OpLevelMgr getOpLevelMgr()
    {
        OpLevelMgr mgr = getSys<OpLevelMgr>(SysType.sysTypeOpLevel);
        return mgr;
    }

    public bool isAPIAcc()
    {
        return m_accType == AccType.ACC_API;
    }

    public bool isAPIAdminAcc()
    {
        return m_accType == AccType.ACC_API_ADMIN;
    }

    // 是子账号
    public bool isSubAcc()
    {
        return m_accType == AccType.ACC_AGENCY_SUB;
    }

    // 是总代理
    public bool isGeneralAgency()
    {
        return m_accType == AccType.ACC_GENERAL_AGENCY;
    }

    public bool isAgency()
    {
        return m_accType == AccType.ACC_AGENCY;
    }

    public bool isAdmin()
    {
        return m_accType == AccType.ACC_SUPER_ADMIN;
    }

    public bool isAdminSub()
    {
        return m_accType == AccType.ACC_SUPER_ADMIN_SUB;
    }
}

// 用户验证
public class UserVerification
{
    // 账号
    public string m_acc;
    // 从数据库读出的密码
    public string m_pwd;
    // 明码
    public string m_pwd1;
    // 从数据库读出的固定验证码
    public string m_dbValidatedCode;

    // 登录已完成步骤
    private int m_loginHasFinishStep = 0;
    // 页面生成的验证码
    public string m_validatedCode;

    public int loginHasFinishStep
    {
        get { return m_loginHasFinishStep; }
        set { m_loginHasFinishStep = value; }
    }

    // 登录成功后清空
    public void clear()
    {
        m_acc = null;
        m_pwd = m_pwd1 = null;
        m_dbValidatedCode = null;
        m_validatedCode = null;
    }

    public void reset()
    {
        loginHasFinishStep = 0;
        m_dbValidatedCode = "";
    }

    public static void checkLogin(HttpSessionState session, HttpResponse response, int step)
    {
        UserVerification verInfo = (UserVerification)session[DefCC.KEY_VERIFICATION];
        if (verInfo == null)
        {
            response.Redirect(DefCC.ASPX_LOGIN_STEP1);
        }

        if (step - verInfo.loginHasFinishStep != 1)
        {
            response.Redirect(DefCC.ASPX_LOGIN_STEP1);
        }
    }
}


