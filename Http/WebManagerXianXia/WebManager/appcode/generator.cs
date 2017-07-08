using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

// 生成查询语句
public class CommonSearchCmdBase
{
    public const int SEARCH_TYPE_GM = 0;            // GM账号表
    public const int SEARCH_TYPE_PLAYER = 1;        // 玩家账号表
    public const int SEARCH_TYPE_PLAYER_SCORE = 2;  // 玩家上分下分表 player_score
    public const int SEARCH_TYPE_OTHER = 3;         // 其他

    // GM账号查询语句的生成
    private const string GM_SEARCH_CMD = "select acc,accType,owner,money,moneyType,createTime,state,generalAgency,gmRight,devSecretKey,aliasName,lastLoginIP,agentRatio,washRatio,depth " +
                                        " from {0} where {1} order by createTime desc ";

    private const string PLAYER_SEARCH_CMD = "select acc,creator,money,moneyType,createTime,state,aliasName,lastLoginDate,enable,playerWashRatio,moneyOnline " +
                                                " from {0} where {1} ";

    private int m_searchType;

    public CommonSearchCmdBase()
    {
        m_searchType = SEARCH_TYPE_OTHER;
    }

    public CommonSearchCmdBase(int searchType)
    {
        m_searchType = searchType;
    }

    public OpRes genSearchSql(ParamMemberInfo param, GMUser user, ref string sqlCMD)
    {
        bool timeCanIgnore = timeCanEmpty();
        if (!timeCanIgnore) // 时间条件是必须的
        {
            if (string.IsNullOrEmpty(param.m_time))
                return OpRes.op_res_time_format_error;
        }

        DateTime mint = DateTime.Now, maxt = mint;
        bool useTime = false;

        if (!string.IsNullOrEmpty(param.m_time))
        {
            useTime = Tool.splitTimeStr(param.m_time, ref mint, ref maxt);
            if (!useTime)
                return OpRes.op_res_time_format_error;
        }

        sqlCMD = genSearchSqlCmd(param, mint, maxt, useTime, user);
        return OpRes.opres_success;
    }

    // 生成查询指令
    private string genSearchSqlCmd(ParamMemberInfo p, DateTime mint, DateTime maxt, bool useTime, GMUser user)
    {
        string cond = "";

        if (!string.IsNullOrEmpty(p.m_acc)) // 搜索具体的账号
        {
            cond = genAccSearchCond(user, p);
            useTime = isUseTimeSearchAcc();
        }
        else
        {
            if (isSearchGM())
            {
                cond = getCondGm(p, user);
            }
            else if (isSearchPlayer())
            {
                cond = getCondPlayer(p, user);
            }
            else if (isSearchPlayerScore())
            {
                cond = getCondPlayerScore(p, user);
            }
            else
            {
                cond = getCondition(p, user);
            }
        }

        if (useTime)
        {
            if (isSearchPlayerScore())
            {
                cond += string.Format(" and opTime >='{0}' and opTime < '{1}' ", mint.ToString(ConstDef.DATE_TIME24),
                    maxt.ToString(ConstDef.DATE_TIME24));
            }
            else
            {
              //  cond += string.Format(" and createTime >='{0}' and createTime < '{1}' ", mint, maxt);
                cond = getTimeCondition(mint, maxt, cond);
            }
        }

        string cmd;
        if (isSearchGM())
        {
            cmd = string.Format(GM_SEARCH_CMD, TableName.GM_ACCOUNT, cond);
        }
        else if (isSearchPlayer())
        {
            cmd = string.Format(PLAYER_SEARCH_CMD, TableName.PLAYER_ACCOUNT_XIANXIA, cond);
        }
        else if(isSearchPlayerScore())
        {
            cmd = cond;
        }
        else
        {
            cmd = getResultSql(cond, p);
        }
        p.m_resultCond = cond;
        return cmd;
    }

    // 生成查询具体账号的条件
    protected virtual string genAccSearchCond(GMUser user, ParamMemberInfo p)
    {
        string cond = "";

        if (isSearchGM())
        {
            if (p.m_acc == user.m_user ||
                user.m_accType == AccType.ACC_SUPER_ADMIN) // 是自己
            {
                cond = string.Format(" acc='{0}' ", p.m_acc);
            }
            else
            {
                cond = string.Format(" acc='{0}' and createCode like '{1}%' ", p.m_acc, ItemHelp.getCreateCodeSpecial(user));
            }
        }
        else if (isSearchPlayer())
        {
            cond = string.Format(" acc='{0}' and createCode like '{1}%' ", p.m_acc, ItemHelp.getCreateCodeSpecial(user));
        }
        else
        {
            cond = string.Format(" opDst='{0}' and opSrcCreateCode like '{1}%' ", p.m_acc, ItemHelp.getCreateCodeSpecial(user));
        }

        return cond;
    }

    private bool isSearchPlayer()
    {
        return m_searchType == SEARCH_TYPE_PLAYER;
    }

    private bool isSearchGM()
    {
        return m_searchType == SEARCH_TYPE_GM;
    }

    private bool isSearchPlayerScore()
    {
        return m_searchType == SEARCH_TYPE_PLAYER_SCORE;
    }

    private string getCondGm(ParamMemberInfo p, GMUser user)
    {
        string cond = "";
        string parentAcc = "";
        int depth = 0;

        if (p.isSearchAll()) // 搜索所有的
        {
            string createCode = "";
            if (string.IsNullOrEmpty(p.m_creator))
            {
                createCode = ItemHelp.getCreateCodeSpecial(user);
                parentAcc = getAccountSpecial(user);
            }
            else
            {
                createCode = ItemHelp.getCreateCode(p.m_creator, user);
                parentAcc = p.m_creator;
            }

            if (p.m_subAcc == 2) // 只搜索子账号
            {
                cond = string.Format(" createCode like '{0}%' and  depth={1} ",
                    createCode, user.m_depth);
            }
            else
            {
                cond = string.Format(" createCode like '{0}%' and  depth>{1} ", createCode, user.m_depth);
            }
           /* if (p.m_subAcc == 1) // 不包括子账号
            {
                cond = string.Format(" createCode like '{0}%' and  acc<>'{1}' and accType<>{2} ",
                    createCode, parentAcc, AccType.ACC_AGENCY_SUB);
            }
            else if (p.m_subAcc == 2) // 只搜索子账号
            {
                cond = string.Format(" createCode like '{0}%' and  acc<>'{1}' and accType={2} ",
                    createCode, parentAcc, AccType.ACC_AGENCY_SUB);
            }
            else
            {
                cond = string.Format(" createCode like '{0}%' and  acc<>'{1}' ", createCode, parentAcc);
            }*/
        }
        else // 直接下级
        {
            if (string.IsNullOrEmpty(p.m_creator))
            {
                cond = string.Format(" owner='{0}' ", getAccountSpecial(user));
                depth = user.m_depth;
            }
            else
            {
                cond = string.Format(" owner='{0}' and createCode like '{1}%' ", p.m_creator,
                    ItemHelp.getCreateCodeSpecial(user));

                depth = p.m_creatorDepth;
            }

            if (p.m_subAcc == 2) // 只搜索子账号
            {
               // cond += string.Format("  and accType={0} ", AccType.ACC_AGENCY_SUB);
                cond += string.Format("  and depth={0} ", depth);
            }
            else
            {
               // cond += string.Format("  and depth>{0} ", depth);
                cond += string.Format("  and accType<>{0} and accType<>{1} and accType<>{2} ",
                    AccType.ACC_AGENCY_SUB,
                    AccType.ACC_API_ADMIN,
                    AccType.ACC_SUPER_ADMIN_SUB);
            }
            /*if (p.m_subAcc == 1) // 不包括子账号
            {
                cond += string.Format("  and accType<>{0} ", AccType.ACC_AGENCY_SUB);
            }
            else if (p.m_subAcc == 2) // 只搜索子账号
            {
                cond += string.Format("  and accType={0} ", AccType.ACC_AGENCY_SUB);
            }*/
        }

        return cond;
    }

    private string getCondPlayer(ParamMemberInfo p, GMUser user)
    {
        string cond;
        if (p.isSearchAll()) // 搜索所有的
        {
            string createCode = "";
            if (string.IsNullOrEmpty(p.m_creator))
            {
                createCode = ItemHelp.getCreateCodeSpecial(user);
            }
            else
            {
                createCode = ItemHelp.getCreateCode(p.m_creator, user);
            }

            cond = string.Format(" createCode like '{0}%'  ", createCode);
        }
        else // 直接下级
        {
            if (string.IsNullOrEmpty(p.m_creator))
            {
                cond = string.Format(" creator='{0}' ", getAccountSpecial(user));
            }
            else
            {
                cond = string.Format(" creator='{0}' and createCode like '{1}%'  ", p.m_creator,
                    ItemHelp.getCreateCodeSpecial(user));
            }
        }
        return cond;
    }

    private string getCondPlayerScore(ParamMemberInfo p, GMUser user)
    {
        string cond;
        if (p.isSearchAll()) // 搜索所有的
        {
            string createCode = "";
            if (string.IsNullOrEmpty(p.m_creator))
            {
                createCode = ItemHelp.getCreateCodeSpecial(user);
            }
            else
            {
                createCode = ItemHelp.getCreateCode(p.m_creator, user);
            }

            cond = string.Format(" opSrcCreateCode like '{0}%'  ", createCode);
        }
        else // 直接下级
        {
            if (string.IsNullOrEmpty(p.m_creator))
            {
                cond = string.Format(" opSrc='{0}' ", getAccountSpecial(user));
            }
            else
            {
                cond = string.Format(" opSrc='{0}' and opSrcCreateCode like '{1}%' ", p.m_creator,
                    ItemHelp.getCreateCodeSpecial(user));
            }
        }
        return cond;
    }

    // 如果user是代理子账号，则返回他的上一级的code
    // 否则返回身的
   /* protected string getCreateCodeSpecial(GMUser user)
    {
        if (user.m_accType == AccType.ACC_AGENCY_SUB ||
            user.m_accType == AccType.ACC_API_ADMIN)
        {
            int i = user.m_createCode.Length - 1;
            if (user.m_createCode[user.m_createCode.Length - 1] == ')')
            {
                for (i = user.m_createCode.Length - 2; i >= 0; i--)
                {
                    if (user.m_createCode[i] == '(')
                        break;
                }
                return user.m_createCode.Substring(0, i);
            }
            else
            {
                return user.m_createCode.Substring(0, i);
            }
        }

        return user.m_createCode;
    }*/

    // 如果user是子账号，返回他的创建者的账号
    protected string getAccountSpecial(GMUser user)
    {
        if (user.m_accType == AccType.ACC_AGENCY_SUB ||
            user.m_accType == AccType.ACC_API_ADMIN ||
            user.m_accType == AccType.ACC_SUPER_ADMIN_SUB)
        {
            return user.m_owner;
        }

        return user.m_user;
    }

    protected virtual string getCondition(ParamMemberInfo p, GMUser user) { throw new Exception(); }
    
    protected virtual string getTimeCondition(DateTime mint, DateTime maxt, string cond) 
    {
        cond += string.Format(" and createTime >='{0}' and createTime < '{1}' ", mint.ToString(ConstDef.DATE_TIME24),
            maxt.ToString(ConstDef.DATE_TIME24));
        return cond;
    }

    // 搜索具体账号时，是否使用时间
    protected virtual bool isUseTimeSearchAcc()
    {
        bool useTime = true;
        if (!isSearchPlayerScore()) // 上分下分时，需要时间
        {
            useTime = false;
        }
        return useTime;
    }

    // 时间可否为空
    public virtual bool timeCanEmpty() { return true; }

    // 返回最终生成的sql查询串
    protected virtual string getResultSql(string cond, ParamMemberInfo p) { throw new Exception(); }
}

//////////////////////////////////////////////////////////////////////////
// 输赢统计的查询语句生成器
public class CommandWinLoseGenerator : CommonSearchCmdBase
{
    private const string WIN_LOSE_SEARCH_CMD = "select sum(playerOutlay) as playerOutlaySum, " +
                                               "sum(playerIncome) as playerIncomeSum, sum(washCount) as washCountSum " +
                                               "from {0} where {1} ";

    private const string WIN_LOSE_SEARCH_CMD_1 = "select playerOutlay as playerOutlaySum, " +
                                           "playerIncome as playerIncomeSum, washCount as washCountSum, date  " + "from {0} where {1} ";

    protected override string genAccSearchCond(GMUser user, ParamMemberInfo p)
    {
        string cond = "";
        if (p.isPlayerWinLoseList())
        {
            cond = string.Format(" playerAcc='{0}' and playerCreateCode like '{1}%' ",
                   p.m_acc,
                   ItemHelp.getCreateCodeSpecial(user));
        }
        else
        {
            cond = string.Format(" playerAcc='{0}' and playerCreateCode like '{1}%' ",
                    p.m_acc,
                    ItemHelp.getCreateCodeSpecial(user));
        }
        return cond;
    }

    // 返回条件
    protected override string getCondition(ParamMemberInfo p, GMUser user)
    {
        string cond = "";
        if (p.isSearchAll()) // 搜索所有的下级代理下下级代理...所创建的玩家的输赢情况
        {
            string createCode = "";
            string parent = null;
            if (string.IsNullOrEmpty(p.m_creator))
            {
                createCode = ItemHelp.getCreateCodeSpecial(user);
                parent = user.m_user;
            }
            else
            {
                createCode = ItemHelp.getCreateCode(p.m_creator, user);
                parent = p.m_creator;
            }

            if (p.isIncludeDirectlyPlayer())
            {
                cond = string.Format(" playerCreateCode like '{0}%'  ", createCode);
            }
            else
            {
                cond = string.Format(" playerCreateCode like '{0}%' and playerCreator<>'{1}' ", createCode, parent);
            }
        }
        else // 直属会员
        {
            if (string.IsNullOrEmpty(p.m_creator))
            {
                cond = string.Format(" playerCreator='{0}' ", getAccountSpecial(user));
            }
            else
            {
                cond = string.Format(" playerCreator='{0}' and playerCreateCode like '{1}%' ", p.m_creator,
                    ItemHelp.getCreateCodeSpecial(user));
            }
        }
        return cond;
    }

    // 返回加上时间后的条件
    protected override string getTimeCondition(DateTime mint, DateTime maxt, string cond)
    {
        cond += string.Format(" and date >='{0}' and date < '{1}' ", mint.ToString(ConstDef.DATE_TIME24), 
            maxt.ToString(ConstDef.DATE_TIME24));
        return cond;
    }

    protected override bool isUseTimeSearchAcc()
    {
        return true;
    }

    public override bool timeCanEmpty() { return false; }

    // 返回最终的查询语句
    protected override string getResultSql(string cond, ParamMemberInfo p)
    {
        string cmd = "";
        if (p.isPlayerWinLoseList())
        {
            cmd = string.Format(WIN_LOSE_SEARCH_CMD_1, TableName.PLAYER_WIN_LOSE, cond);
        }
        else
        {
            cmd = string.Format(WIN_LOSE_SEARCH_CMD, TableName.PLAYER_WIN_LOSE, cond);
        }

        return cmd;
    }
}

//////////////////////////////////////////////////////////////////////////
public static class ParamMemberInfoStatic
{
    // 搜索全部层次时，不包括直属子玩家
    public static void notIncludeDirectlyPlayer(this ParamMemberInfo info)
    {
        info.m_flag &= ~1;
    }

    // 搜索全部层次时，包括直属子玩家
    public static void includeDirectlyPlayer(this ParamMemberInfo info)
    {
        info.m_flag |= 1;
    }

    public static bool isIncludeDirectlyPlayer(this ParamMemberInfo info)
    {
        int res = (info.m_flag & 1);
        return res > 0;
    }

    //////////////////////////////////////////////////////////////////////////
    public static void cancelPlayerWinLoseList(this ParamMemberInfo info)
    {
        info.m_flag &= ~0x02;
    }

    // 搜索全部层次时，包括直属子玩家
    public static void setPlayerWinLoseList(this ParamMemberInfo info)
    {
        info.m_flag |= 0x02;
    }

    public static bool isPlayerWinLoseList(this ParamMemberInfo info)
    {
        int res = (info.m_flag & 0x02);
        return res > 0;
    }
    //////////////////////////////////////////////////////////////////////////
    // 搜索全部层次时，不包括子账号
    public static void notIncludeSubAcc(this ParamMemberInfo info)
    {
        info.m_subAcc |= 0x01;
    }
    // 搜索全部层次时，不包括子账号
    public static void notIncludeApiAdmin(this ParamMemberInfo info)
    {
        info.m_subAcc |= 0x02;
    }
    public static bool isIncludeSubAcc(this ParamMemberInfo info)
    {
        int res = (info.m_subAcc & 0x01);
        return res > 0;
    }
    public static bool isIncludeApiAdmin(this ParamMemberInfo info)
    {
        int res = (info.m_flag & 0x02);
        return res > 0;
    }
    //////////////////////////////////////////////////////////////////////////
}

//////////////////////////////////////////////////////////////////////////
public class TdButtonGenerator
{
    private TableCell m_td;

    public TableCell td
    {
        set { m_td = value; }
        get { return m_td; }
    }

    public void addButton(string btnText,           // btn按钮文字
                          string cmdName,           // btn 命令名称
                          string cmdArgument,       // btn参数
                          string btnId,             // btn Id号
                          EventHandler svrClick,    // 服务器单击处理
                          string clientClick = "",    // 客户端单击脚本调用
                          string infoId = "")         // 提示信息id号
    {
        Button btn = new Button();
        btn.Text = btnText;
        btn.CommandName = cmdName;
        btn.CommandArgument = cmdArgument;
        btn.ID = btnId;
        btn.Click += svrClick;
        btn.OnClientClick = clientClick;
        m_td.Controls.Add(btn);

        if (!string.IsNullOrEmpty(infoId))
        {
            Label L = new Label();
            L.ID = infoId;
            m_td.Controls.Add(L);
        }
    }
}



