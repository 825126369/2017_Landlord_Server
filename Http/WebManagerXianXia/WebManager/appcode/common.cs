using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Net;
using System.Collections.Specialized;
using MongoDB.Bson;
using MongoDB.Driver;

// 分页浏览的生成器
public class PageBrowseGenerator
{
    private StringBuilder m_textBuilder = new StringBuilder();
    // 同时显示的最多页链接个数
    private int m_maxShowPage = 10;

    public PageBrowseGenerator()
    {
    }

    public void setTotalMaxPage(int p)
    {
        m_maxShowPage = p;
    }

    // 构造页链接
    // curpage:当前是第几页 
    // row_each_page:每页多少条记录
    // href:链接的是哪个页面
    // total_page:返回的是总页数
    // user具体user，返回页链接时，需要针对某个用户
    // url_param额外url参数，这里面不能含page关键字
    public string genPageFoot(long curpage,
                                int row_each_page,
                                string href,
                                ref long total_page,
                                GMUser user,
                                Dictionary<string, object> url_param = null)
    {
        if (url_param != null && url_param.ContainsKey("page"))
        {
            LOGW.Info("PageBrowseGenerator.genPageFoot含page关键字");
            return "";
        }
        long page_count = getPageCount(row_each_page, user);
        total_page = page_count;
        if (page_count <= 1)
            return "";

        long count = page_count >= m_maxShowPage ? m_maxShowPage : page_count;
        m_textBuilder.Remove(0, m_textBuilder.Length);
        m_textBuilder.Append(genPage(curpage, page_count, true, 1, href, "首页", url_param));
        m_textBuilder.Append(genPage(curpage, page_count, true, curpage - 1, href, "上一页", url_param));
        m_textBuilder.Append("  ");
        m_textBuilder.Append(genBody(curpage, href, count, page_count, url_param));
        m_textBuilder.Append(genPage(curpage, page_count, false, curpage + 1, href, "下一页", url_param));
        m_textBuilder.Append(genPage(curpage, page_count, false, page_count, href, "尾页", url_param));
        return m_textBuilder.ToString();
    }

    // 取得共多少页
    private long getPageCount(int row_each_page, GMUser user)
    {
        return (long)Math.Ceiling((double)user.totalRecord / row_each_page);
    }

    // page_count总页数, count有多少链接可以点选
    private string genBody(long curpage, string href, long count, long page_count, Dictionary<string, object> url_param)
    {
        // 向前
        long pre = count >> 1;
        // 向后
        long next = count - pre - 1;

        long start = curpage - pre >= 1 ? curpage - pre : 1;
        long end = start + count;

        if (end > page_count)
        {
            end = page_count + 1;
            start = end - count;
        }
        URLParam[] plist = new URLParam[count];

        long j = 0;
        for (long i = start; i < end; i++, j++)
        {
            plist[j] = new URLParam();
            if (i == curpage)
            {
                plist[j].m_url = "";
                plist[j].m_text = i.ToString();
            }
            else
            {
                plist[j].m_url = href;
                plist[j].m_text = i.ToString();
                plist[j].m_key = "page";
                plist[j].m_value = i.ToString();
                plist[j].m_exUrlParam = url_param;
            }
        }
        string str = Tool.genHyperlink(plist);
        return str;
    }

    // 产生第指定文本的页
    private string genPage(long curpage, 
                           long page_count, 
                           bool left,
                           long special_page,
                           string href,
                           string text,
                           Dictionary<string, object> url_param)
    {
        if (left)
        {
            if (curpage <= 1)
                return "";
        }
        else
        {
            if (curpage >= page_count)
                return "";
        }
        URLParam[] p = new URLParam[1];
        p[0] = new URLParam();
        p[0].m_url = href;
        p[0].m_text = text;
        p[0].m_key = "page";
        p[0].m_value = special_page.ToString();
        p[0].m_exUrlParam = url_param;
        return Tool.genHyperlink(p);
    }
}

public class ItemHelp
{
    public static string getRewardList(List<ParamItem> rewardList)
    {
        string result = "";
        string name = "";

        for (int i = 0; i < rewardList.Count; i++)
        {
            ItemCFGData data = ItemCFG.getInstance().getValue(rewardList[i].m_itemId);
            if (data != null)
            {
                name = data.m_itemName;
            }
            else
            {
                name = "";
            }
            result += string.Format("id : {0}, name:{1}, count : {2}", rewardList[i].m_itemId, name, rewardList[i].m_itemCount);
            result += "<br />";
        }

        return result;
    }

    // Dictionary 道具，数量
    public static string getRewardList(Dictionary<int, int> rewardList)
    {
        string result = "";
        string name = "";

        foreach(var item in rewardList)
        {
            ItemCFGData data = ItemCFG.getInstance().getValue(item.Key);
            if (data != null)
            {
                name = data.m_itemName;
            }
            else
            {
                name = "";
            }
            result += string.Format("id : {0}, name:{1}, count : {2}", item.Key, name, item.Value);
            result += "<br />";
        }

        return result;
    }

    // 生成道具列表的数组
    public static BsonDocument genItemBsonArray(List<ParamItem> itemList)
    {
        if (itemList == null)
            return null;

        Dictionary<string, object> dd = new Dictionary<string, object>();
        for (int i = 0; i < itemList.Count; i++)
        {
            Dictionary<string, object> tmpd = new Dictionary<string, object>();
            tmpd.Add("itemId", itemList[i].m_itemId);
            tmpd.Add("itemCount", itemList[i].m_itemCount);
            dd.Add(i.ToString(), tmpd.ToBsonDocument());
        }
        return dd.ToBsonDocument();
    }

    // 对gameList集合取反
    public static string getReverseGameList(string gameList)
    {
        string[] arr = Tool.split(gameList, ',', StringSplitOptions.RemoveEmptyEntries);
        HashSet<string> s1 = new HashSet<string>(arr);
        HashSet<string> s2 = new HashSet<string>();

        for (int i = 0; i < StrName.s_gameList.Count; i++)
        {
            s2.Add(StrName.s_gameList[i].m_gameId.ToString());
        }

        string game = "";
        s2.ExceptWith(s1);
        foreach (var s in s2)
        {
            game += s + ',';
        }

        if (game != "")
        {
            game = game.Remove(game.Length - 1);
        }
        
        return game;
    }

    // 返回盈利率
    public static string getRate(long income, long outlay)
    {
        if (outlay == 0)
            return "1";

        double factGain = (double)income / outlay;
        return Math.Round(factGain, 3).ToString();
    }

    // 返回实际盈利率
    public static string getFactExpRate(long income, long outlay)
    {
        if (income == 0 && outlay == 0)
            return "0";
        if (income == 0)
            return "-∞";

        double factGain = (double)(income - outlay) / income;
        return Math.Round(factGain, 3).ToString();
    }

    public static string getCowsCardTypeName(int cardType)
    {
        XmlConfig cfg = ResMgr.getInstance().getRes("cows_card.xml");
        return cfg.getString(cardType.ToString(), "未知");
    }

    // gm账号的级联查询填充
    public static void fillDropDownList(DropDownList dList,
                                          int searchAccType,
                                          string owner,
                                          GMUser user)
    {
        ParamGmAccountCascade param = new ParamGmAccountCascade();
        param.m_searchAccType = searchAccType;
        param.m_owner = owner;
        user.doQuery(param, QueryType.queryTypeGmAccountCascade);

        List<MemberInfo> data = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccountCascade);

        for (int i = 0; i < data.Count; i++)
        {
            MemberInfo info = data[i];
            dList.Items.Add(new ListItem(info.m_acc, info.m_acc));
        }
    }

    public static void fillDropDownList1(DropDownList dList,
                                          int searchAccType,
                                          string owner,
                                          GMUser user)
    {
        string cmd = string.Format("select acc from {0} where owner='{1}' and accType<>{2} ",
            TableName.GM_ACCOUNT,
            owner,
            AccType.ACC_AGENCY_SUB);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
            user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return;

        for (int i = 0; i < dataList.Count; i++)
        {
            string acc = Convert.ToString(dataList[i]["acc"]);
            dList.Items.Add(new ListItem(acc, acc));
        }
    }

    public static void fillDropDownList2(DropDownList dList,
                                          string owner,
                                          GMUser user)
    {
        string cmd = string.Format("select acc from {0} where owner='{1}' ",
            TableName.GM_ACCOUNT,
            owner);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
            user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return;

        for (int i = 0; i < dataList.Count; i++)
        {
            string acc = Convert.ToString(dataList[i]["acc"]);
            dList.Items.Add(new ListItem(acc, acc));
        }
    }

    // 是否存在后缀
    public static bool existPostfix(string posfix, GMUser user)
    {
        return user.sqlDb.keyStrExists(TableName.GM_ACCOUNT, "postfix", posfix, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
    }

    public static bool existApiPostfix(string prefix, GMUser user)
    {
        return user.sqlDb.keyStrExists(TableName.API_APPROVE, "apiPrefix", prefix,
            user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
    }

    public static string getCreateCode(string acc, GMUser user)
    {
        string createCode = "sss";
        string execCmd = string.Format("select createCode from {0} where acc='{1}' and createCode like '{2}%' ", TableName.GM_ACCOUNT, acc, user.m_createCode);
        Dictionary<string, object> ret =
            user.sqlDb.queryOne(execCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (ret != null && ret.ContainsKey("createCode"))
        {
            createCode = Convert.ToString(ret["createCode"]);
        }
        return createCode;
    }

    // 返回user的创建code
    public static string getCreateCodeSpecial(GMUser user)
    {
        if (user.m_accType == AccType.ACC_AGENCY_SUB ||
            user.m_accType == AccType.ACC_API_ADMIN 
            //|| user.m_accType == AccType.ACC_SUPER_ADMIN_SUB
            )
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
    }

    public static string getAccountSpecial(GMUser user)
    {
        if (user.m_accType == AccType.ACC_AGENCY_SUB ||
            user.m_accType == AccType.ACC_API_ADMIN ||
            user.m_accType == AccType.ACC_SUPER_ADMIN_SUB)
        {
            return user.m_owner;
        }

        return user.m_user;
    }

    public static bool isSubAcc(GMUser user)
    {
        if (user.m_accType == AccType.ACC_AGENCY_SUB ||
            user.m_accType == AccType.ACC_API_ADMIN ||
            user.m_accType == AccType.ACC_SUPER_ADMIN_SUB)
        {
            return true;
        }

        return false;
    }

    // 生成创建码
    public static string genCreateCode(int childCount, string parentCreateCode)
    {
        int n = childCount + 1;
        if (n >= 10)
        {
            return parentCreateCode + string.Format("({0})", n);
        }
        return parentCreateCode + n.ToString();
    }

    // 返回账号acc的余额
    public static long getRemainMoney(string acc, bool isPlayer, GMUser user)
    {
        string sql = "";
        SqlSelectGenerator gen = new SqlSelectGenerator();
        gen.addField("money");
        if (isPlayer)
        {
            sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
                                           string.Format("acc='{0}'", acc));
        }
        else
        {
            sql = gen.getResultSql(TableName.GM_ACCOUNT,
                                           string.Format("acc='{0}'", acc));
        }
        Dictionary<string, object> data = user.sqlDb.queryOne(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (data == null)
            return 0;

        return Convert.ToInt64(data["money"]);
    }

    public static void stopStartGmAcc(object sender, GMUser user, Table table)
    {
        if (!(sender is Button))
            return;

        try
        {
            Button btn = (Button)sender;
            ParamStartStopGmAcc param = new ParamStartStopGmAcc();
            param.m_acc = btn.CommandArgument;
            if (btn.CommandName == "start")
            {
                param.m_opType = 0;
            }
            else
            {
                param.m_opType = 1;
            }

            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpStartStopGmAcc);
            Label L = (Label)table.FindControl("Label" + param.m_acc);
            if (L != null)
            {
                L.Text = OpResMgr.getInstance().getResultString(res);
                L.Style.Clear();
                L.Style.Add("color", "red");
                if (res == OpRes.opres_success)
                {
                    Label lblState = (Label)table.FindControl("LabelState" + param.m_acc);
                    if (lblState != null)
                    {
                        lblState.Text = StrName.s_gmStateName[param.m_opType];
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
        }
    }

    public static void addStopStartListener(TableCell td, EventHandler handler, MemberInfo item)
    {
        Button btn1 = new Button();
        btn1.Text = "启用";
        btn1.CommandName = "start";
        btn1.CommandArgument = item.m_acc;
        btn1.ID = item.m_acc + "start";
        btn1.Click += handler;
        td.Controls.Add(btn1);

        Button btn2 = new Button();
        btn2.Text = "停用";
        btn2.CommandName = "stop";
        btn2.CommandArgument = item.m_acc;
        btn2.ID = item.m_acc + "stop";
        btn2.Click += handler;
        td.Controls.Add(btn2);
        Label L = new Label();
        L.ID = "Label" + item.m_acc;
        td.Controls.Add(L);
    }

    // 存入货币到数据库的转换
    public static long saveMoneyValue(long curVal)
    {
        return curVal * DefCC.MONEY_BASE;
    }

    // 显示货币的转换
    public static double showMoneyValue(double curVal)
    {
        return curVal / DefCC.MONEY_BASE;
    }

    // 洗码比是否合法
    public static bool isValidWashRatio(ParamCreateGmAccount param, GMUser user, ref double resWashRatio)
    {
        return isValidRatio(param.m_washRatio, user, user.m_washRatio, ConstDef.MAX_WASH_RATIO, ref resWashRatio);
    }

    // 代理占成是否合法
    public static bool isValidAgentRatio(ParamCreateGmAccount param, GMUser user, ref double resAgentRatio)
    {
        return isValidRatio(param.m_agentRatio, user, user.m_agentRatio, ConstDef.MAX_AGENT_RATIO, ref resAgentRatio);
    }

    // 通过千分位逗号相隔数字
    public static string toStrByComma(double val)
    {
        string str = string.Format("{0:N2}", val);

        for (int i = str.Length - 1; i >= 0; i--)
        {
            if (str[i] != '0')
            {
                if (str[i] == '.')
                {
                    return str.Substring(0, i);
                }
                else
                {
                    return str.Substring(0, i + 1);
                }
            }
        }

        return str;
    }

    // 洗码比/代理占成 是否合法
    private static bool isValidRatio(string inputRatio, 
                                        GMUser user, 
                                        double gmRatio,
                                        double maxRatio,
                                        ref double resRatio)
    {
        if (string.IsNullOrEmpty(inputRatio))
        {
            if (user.isAdmin())
            {
                resRatio = maxRatio;
            }
            else
            {
                resRatio = gmRatio;
            }
            return true;
        }


        if (!double.TryParse(inputRatio, out resRatio))
            return false;

        resRatio /= 100;

        if (resRatio < 0 || resRatio > maxRatio)
                return false;

        if (!user.isAdmin())
        {
            if (resRatio > gmRatio)
                return false;
        }
        
        return true;
    }

    public static string genHTML(System.Web.UI.Control c)
    {
        StringWriter sw = new StringWriter();
        HtmlTextWriter w = new HtmlTextWriter(sw);
        c.RenderControl(w);
        return sw.GetStringBuilder().ToString();
    }
}

//////////////////////////////////////////////////////////////////////////

public class ViewStatSellerStep
{
    private static string[] s_head = new string[] { "账号", "存款", "存款次数", "提款", "提款次数", "(收入)存款 - 提款", "下级" };
    private string[] m_content = new string[s_head.Length];

    public void genTable(Table table, OpRes res, GMUser user)
    {
        TableRow tr = new TableRow();
        table.Rows.Add(tr);
        TableCell td = null;
        if (res != OpRes.opres_success)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = OpResMgr.getInstance().getResultString(res);
            return;
        }

        StatResultSeller qresult = (StatResultSeller)user.getStatResult(StatType.statTypeSellerStep);
        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        URLParam uParam = new URLParam();
        uParam.m_key = "acc";
        uParam.m_className = "cLevelLink";
        uParam.m_url = "/appaspx/account/AccountStatSellerStep.aspx";
        uParam.m_text = "查看下级";
        StatResultSellerItem sum = new StatResultSellerItem();

        for (i = 0; i < qresult.m_items.Count; i++)
        {
            StatResultSellerItem item = qresult.m_items[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            uParam.m_value = item.m_seller;
            uParam.clearExParam(); 

            m_content[0] = item.m_seller;
            m_content[1] = item.m_addScore.ToString();
            m_content[2] = item.m_addScoreCount.ToString();
            m_content[3] = item.m_desScore.ToString();
            m_content[4] = item.m_desScoreCount.ToString();
            m_content[5] = (item.m_addScore - item.m_desScore).ToString();

            sum.m_addScore += item.m_addScore;
            sum.m_desScore += item.m_desScore;
            sum.m_addScoreCount += item.m_addScoreCount;
            sum.m_desScoreCount += item.m_desScoreCount;

            if (item.m_addScore == 0 && item.m_desScore == 0)
            {
                m_content[6] = "";
            }
            else
            {
                uParam.addExParam("time", item.m_time.TrimStart(' ').TrimEnd(' '));
                m_content[6] = Tool.genHyperlink(uParam);
            }
            
            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
        genFoot(table, sum);
    }

    protected void genFoot(Table table, StatResultSellerItem sum)
    {
        m_content[0] = "总计";
        m_content[1] = sum.m_addScore.ToString();
        m_content[2] = sum.m_addScoreCount.ToString();
        m_content[3] = sum.m_desScore.ToString();
        m_content[4] = sum.m_desScoreCount.ToString();
        m_content[5] = (sum.m_addScore - sum.m_desScore).ToString();
        m_content[6] = "";
        TableCell td = null;
        TableRow tr = new TableRow();
        table.Rows.Add(tr);
        for (int j = 0; j < s_head.Length; j++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = m_content[j];
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 生成查询语句
/*public class CommonSearchCmd
{
    public const int SEARCH_TYPE_GM = 0;            // GM账号表
    public const int SEARCH_TYPE_PLAYER = 1;        // 玩家账号表
    public const int SEARCH_TYPE_PLAYER_SCORE = 2;  // 玩家上分下分表 player_score

    // GM账号查询语句的生成
    private const string GM_SEARCH_CMD = "select acc,accType,owner,money,moneyType,createTime,state,generalAgency,gmRight,devSecretKey,aliasName " +
                                        " from {0} where {1} order by createTime desc ";

    private const string PLAYER_SEARCH_CMD = "select acc,creator,money,moneyType,createTime,state,aliasName,lastLoginDate,enable " +
                                                " from {0} where {1} ";

    private int m_searchType;

    public CommonSearchCmd(int searchType)
    {
        m_searchType = searchType;
    }

    public OpRes genSearchSql(ParamMemberInfo param, GMUser user, ref string sqlCMD)
    {
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

            if (!isSearchPlayerScore()) // 上分下分时，需要时间
            {
                useTime = false;
            }
        }
        else
        {
            if (isSearchGM())
            {
                cond = getCondGm(p, user);
            }
            else if(isSearchPlayer())
            {
                cond = getCondPlayer(p, user);
            }
            else if (isSearchPlayerScore())
            {
                cond = getCondPlayerScore(p, user);
            }
        }

        if (useTime)
        {
            if (isSearchPlayerScore())
            {
                cond += string.Format(" and opTime >='{0}' and opTime < '{1}' ", mint, maxt);
            }
            else
            {
                cond += string.Format(" and createTime >='{0}' and createTime < '{1}' ", mint, maxt);
            }
        }

        string cmd;
        if (isSearchGM())
        {
            cmd = string.Format(GM_SEARCH_CMD, TableName.GM_ACCOUNT, cond);
        }
        else if(isSearchPlayer())
        {
            cmd = string.Format(PLAYER_SEARCH_CMD, TableName.PLAYER_ACCOUNT_XIANXIA, cond);
        }
        else
        {
            cmd = cond;
        }
        return cmd;
    }

    // 生成查询具体账号的条件
    private string genAccSearchCond(GMUser user, ParamMemberInfo p)
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
                cond = string.Format(" acc='{0}' and createCode like '{1}%' ", p.m_acc, getCreateCodeSpecial(user));
            }
        }
        else if(isSearchPlayer())
        {
            cond = string.Format(" acc='{0}' and createCode like '{1}%' ", p.m_acc, getCreateCodeSpecial(user));
        }
        else
        {
            cond = string.Format(" opDst='{0}' and opSrcCreateCode like '{1}%' ", p.m_acc, getCreateCodeSpecial(user));
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

        if (p.isSearchAll()) // 搜索所有的
        {
            string createCode = "";
            if (string.IsNullOrEmpty(p.m_creator))
            {
                createCode = getCreateCodeSpecial(user);
                parentAcc = getAccountSpecial(user);
            }
            else
            {
                createCode = ItemHelp.getCreateCode(p.m_creator, user);
                parentAcc = p.m_creator;
            }

            if (p.m_subAcc == 1) // 不包括子账号
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
            }
        }
        else // 直接下级
        {
            if (string.IsNullOrEmpty(p.m_creator))
            {
                cond = string.Format(" owner='{0}' ", getAccountSpecial(user));
            }
            else
            {
                cond = string.Format(" owner='{0}' and createCode like '{1}%' ", p.m_creator,
                    getCreateCodeSpecial(user));
            }

            if (p.m_subAcc == 1) // 不包括子账号
            {
                cond += string.Format("  and accType<>{0} ", AccType.ACC_AGENCY_SUB);
            }
            else if (p.m_subAcc == 2) // 只搜索子账号
            {
                cond += string.Format("  and accType={0} ", AccType.ACC_AGENCY_SUB);
            }
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
                createCode = getCreateCodeSpecial(user);
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
                    getCreateCodeSpecial(user));
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
                createCode = getCreateCodeSpecial(user);
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
                    getCreateCodeSpecial(user));
            }
        }
        return cond;
    }

    // 如果user是代理子账号，则返回他的上一级的code
    // 否则返回身的
    private string getCreateCodeSpecial(GMUser user)
    {
        if (user.m_accType == AccType.ACC_AGENCY_SUB)
        {
            int i = user.m_createCode.Length - 1;
            if (user.m_createCode[user.m_createCode.Length - 1] == ')')
            {
                for (i = user.m_createCode.Length - 2; i >= 0; i--)
                {
                    if(user.m_createCode[i]== '(')
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
    }

    // 如果user是子账号，返回他的创建者的账号
    private string getAccountSpecial(GMUser user)
    {
        if (user.m_accType == AccType.ACC_AGENCY_SUB)
        {
            return user.m_owner;
        }

        return user.m_user;
    }
}*/

//////////////////////////////////////////////////////////////////////////

// 一个玩家
public class Player : CCPlayer
{
    public Player()
    {
    }

    public Player(string acc, GMUser user)
    {
        string sqlCmd = string.Format(SQL_QUERY, TableName.PLAYER_ACCOUNT_XIANXIA, acc);

        Dictionary<string, object> r = user.sqlDb.queryOne(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        init(acc, r);
    }

    // 当前账号是否由user派生出来的
    public bool isDerivedFrom(GMUser user)
    {
        string ccode = ItemHelp.getCreateCodeSpecial(user);
       // int index = m_createCode.IndexOf(user.m_createCode, 0);
        int index = m_createCode.IndexOf(ccode, 0);
        return index == 0;
    }
}

//////////////////////////////////////////////////////////////////////////

// 一个gmuser
public class DestGmUser
{
    static string SQL_QUERY = "SELECT acc,accType,createCode,state,money,owner FROM {0} where acc='{1}' ";

    public bool m_isExists = false;

    public string m_createCode;

    public int m_accType;

    public int m_state;

    public long m_money;

    public string m_owner;

    public bool m_isSelf = false;

    public DestGmUser()
    {
    }

    public DestGmUser(string acc, GMUser user)
    {
        if (acc == user.m_user)
        {
            m_isSelf = true;
            return;
        }

        string sqlCmd = string.Format(SQL_QUERY, TableName.GM_ACCOUNT, acc);

        Dictionary<string, object> r = user.sqlDb.queryOne(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
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

                m_createCode = Convert.ToString(r["createCode"]);
                m_accType = Convert.ToInt32(r["accType"]);
                m_state = Convert.ToInt32(r["state"]);
                m_money = Convert.ToInt64(r["money"]);
                m_owner = Convert.ToString(r["owner"]);
                m_isExists = true;
            } while (false);
        }
    }

    // 当前账号是否由user派生出来的
    public bool isDerivedFrom(GMUser user)
    {
        string ccode = ItemHelp.getCreateCodeSpecial(user);
        int index = m_createCode.IndexOf(ccode, 0);
        return index == 0;
    }

    public bool isAccType(int atype)
    {
        return atype == m_accType;
    }

    // 账号是否停封
    public bool isAccStop()
    {
        return m_state == GmState.STATE_BLOCK;
    }
}



