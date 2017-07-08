using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;

// 查看管理账号信息
public class ViewGmAccountInfo
{
    private static string[] s_head = new string[] { "账号", "别名", "账号类型", "直属下线", "会员", "子账号",
                                                     "分数","注册日期", "创建者", "所属总代理", "权限","状态",""/*12*/,""/*, "上次登录IP"*/ };

    private string[] m_content = new string[s_head.Length];

    public static string CONDITION_GM_CHILD = " owner='{0}' and accType<>{1} ";
    public static string CONDITION_GM_SUB = " owner='{0}' and accType={1} ";
    public static string CONDITION_PLAYER_CHILD = " creator='{0}' ";

    public void genTable(Table table, OpRes res, GMUser user, EventHandler handler, ParamMemberInfo param)
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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
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

        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            tr = new TableRow();
//             if ((i & 1) == 0)
//             {
//                 tr.CssClass = "alt";
//             }
            table.Rows.Add(tr);

            uParam.m_className = "cLevelLink";
            uParam.m_value = item.m_acc;
            uParam.m_text = item.m_acc;
            uParam.m_url = DefCC.ASPX_GM_INFO_NO_PARAM;

            HtmlGenericControl alink = new HtmlGenericControl();
            alink.TagName = "a";
            alink.Attributes.Add("id", "aname_" + item.m_acc);
            alink.Attributes.Add("acc", item.m_acc);
            alink.Attributes.Add("class", "btn btn-primary");
            alink.InnerText = "修改";

            m_content[0] = Tool.genHyperlink(uParam);
            m_content[1] = item.m_aliasName + "<br/>" + ItemHelp.genHTML(alink); // 别名
            m_content[2] = StrName.s_accountType[item.m_accType];

            if (item.m_accType == AccType.ACC_AGENCY_SUB || item.m_accType == AccType.ACC_API)
            {
                m_content[3] = "";
            }
            else
            {
                uParam.clearExParam();
                long count = user.sqlDb.getRecordCount(TableName.GM_ACCOUNT,
                                                        string.Format(CONDITION_GM_CHILD, item.m_acc, AccType.ACC_AGENCY_SUB),
                                                        user.getMySqlServerID(),
                                                        MySqlDbName.DB_XIANXIA);

                uParam.m_className = "cSubLevelNum";
                uParam.m_url = DefCC.ASPX_GM_INFO_VIEW_TREE_NO_PARAM;
                uParam.m_text = count.ToString(); //"查看直属下线";
                m_content[3] = Tool.genHyperlink(uParam);
            }

           // user.getOpLevelMgr().addSub(param.getRootUser(user), item.m_acc, uParam);
            addSub(user, item.m_acc, param);

            if (item.m_accType == AccType.ACC_GENERAL_AGENCY || item.m_accType == AccType.ACC_AGENCY_SUB)
            {
                m_content[4] = "";
            }
            else
            {
                long count = user.sqlDb.getRecordCount(TableName.PLAYER_ACCOUNT_XIANXIA,
                                                        string.Format(CONDITION_PLAYER_CHILD, item.m_acc),
                                                        user.getMySqlServerID(),
                                                        MySqlDbName.DB_XIANXIA);
                uParam.m_className = "cSubLevelNum";
                uParam.m_text = count.ToString();// "查看会员";
                uParam.m_url = DefCC.ASPX_SUB_PLAYER;
                m_content[4] = Tool.genHyperlink(uParam); // 会员
            }

            long count1 = user.sqlDb.getRecordCount(TableName.GM_ACCOUNT,
                                                    string.Format(CONDITION_GM_SUB, item.m_acc, AccType.ACC_AGENCY_SUB),
                                                    user.getMySqlServerID(),
                                                    MySqlDbName.DB_XIANXIA);
            uParam.m_className = "cSubLevelNum";
            uParam.m_text = count1.ToString();//"查看子账号";
            uParam.m_url = DefCC.ASPX_SUB_Agency;
            uParam.clearExParam(); uParam.addExParam("depth", item.m_depth);
            m_content[5] = Tool.genHyperlink(uParam); // 子账号

            m_content[6] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_money));
            m_content[7] = item.m_createTime; // 注册日期
            m_content[8] = item.m_owner;      // 创建者
            m_content[9] = item.m_generalAgency; // 所属总代理


            HtmlGenericControl aright = new HtmlGenericControl();
            aright.TagName = "a";
            aright.Attributes.Add("id", "aright_" + item.m_acc);
            aright.Attributes.Add("acc", item.m_acc);
            aright.InnerText = "分配";
            m_content[10] = ""; // ItemHelp.genHTML(aright);// 权限

            //m_content[13] = item.m_lastLoginIP; // 上次登录IP

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);

                if (j == 11) // 状态
                {
                    addStateLabel(td, item);
                }
                else if (j == 12) // 设置账号
                {
                    addButton(td, item, 0);
                }
                else if (j == 13)
                {
                    addButton(td, item, 1);
                }
                else
                {
                    td.Text = m_content[j];
                }
            }
        }
    }

    private void addStateLabel(TableCell td, MemberInfo item)
    {
        Label L = new Label();
        L.ID = "LabelState" + item.m_acc;
        L.Text = StrName.s_gmStateName[item.m_state];
        td.Controls.Add(L);
    }

    private void addSub(GMUser user, string acc, ParamMemberInfo param)
    {
        URLParam uparam = new URLParam();
        uparam.m_key = "acc";
        uparam.m_value = acc;
        uparam.m_url = DefCC.ASPX_GM_INFO_VIEW_TREE_NO_PARAM;
        user.getOpLevelMgr().addSub(param.getRootUser(user), acc, uparam);
    }

    public static void addButton(TableCell td, MemberInfo item, int info)
    {
        switch (info)
        {
            case 0:
                {
                    HtmlInputButton btn = new HtmlInputButton();
                    btn.Attributes.Add("value", "启用账号");
                    btn.Attributes.Add("op", "0");
                    btn.Attributes.Add("acc", item.m_acc);
                    btn.Attributes.Add("id", "astate_" + item.m_acc);
                    btn.Attributes.Add("class", "btn btn-success");
                    td.Controls.Add(btn);
                }
                break;
            case 1:
                {
                    HtmlInputButton btn = new HtmlInputButton();
                    btn.Attributes.Add("value", "停用账号");
                    btn.Attributes.Add("op", "1");
                    btn.Attributes.Add("acc", item.m_acc);
                    btn.Attributes.Add("id", "astate_" + item.m_acc);
                    btn.Attributes.Add("class", "btn btn-danger");
                    td.Controls.Add(btn);
                }
                break;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 查看管理代理子账号信息，仅显示子账号的情况
public class ViewGmSubAgencyAccountInfo
{
    private static string[] s_head = new string[] { "账号", "别名", "账号类型", "注册日期", 
        "创建者", "所属总代理", "状态","设置账号"/*,"上次登录IP"*/ };

    private string[] m_content = new string[s_head.Length];

    public void genTable(Table table, OpRes res, GMUser user, EventHandler handler)
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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
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

        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            tr = new TableRow();
            table.Rows.Add(tr);

            uParam.m_value = item.m_acc;
            uParam.m_text = item.m_acc;
            uParam.m_url = DefCC.ASPX_GM_INFO_NO_PARAM;

            m_content[0] = Tool.genHyperlink(uParam);
            m_content[1] = item.m_aliasName; // 别名
            m_content[2] = StrName.s_accountType[item.m_accType];
            m_content[3] = item.m_createTime; // 注册日期
            m_content[4] = item.m_owner;      // 创建者
            m_content[5] = item.m_generalAgency; // 所属总代理
            m_content[6] = StrName.s_gmStateName[item.m_state]; // 状态

           // m_content[8] = item.m_lastLoginIP;  // 上次登录IP

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                if (j == 7) // 设置账号
                {
                    ItemHelp.addStopStartListener(td, handler, item);
                }
                else
                {
                    td.Text = m_content[j];
                }
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 给玩家上分下分表
public class ViewPlayerScoreInfo
{
    private static string[] s_head = new string[] { "会员账号", "余额", "分数", "","","" };

    private string[] m_content = new string[s_head.Length];

    Table m_result;
    GMUser m_user;
    bool m_isRefreshed;

    public ViewPlayerScoreInfo(bool IsRefreshed)
    {
        m_isRefreshed = IsRefreshed;
    }

    public void genTable(Table table, OpRes res, GMUser user)
    {
        m_result = table;
        m_user = user;

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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypePlayerMember);
        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            tr = new TableRow();
//             if ((i & 1) == 0)
//             {
//                 tr.CssClass = "alt";
//             }
            table.Rows.Add(tr);

            m_content[0] = item.m_acc;
            m_content[1] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_money));

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);

                if (1 == j) {
                    td.ID = "RemainMoney_" + item.m_acc;
                }
                
                if (j == 2) // 生成分数文本框
                {
                    createScoreTextBox("txt" + item.m_acc, td);
                }
                else if (j == 3) // 上分下分
                {
                    addScoreButton1(td, item, 0);
                }
                else if (j == 4)
                {
                    addScoreButton1(td, item, 1);
                }
                else if (j == 5)
                {
                    addScoreButton1(td, item, 2);
                }
                else
                {
                    td.Text = m_content[j];
                }
            }
        }
    }

    public void genTable(Table table, OpRes res, GMUser user, WebManager.appaspx.account.AccountScorePlayer asp,
       ParamMemberInfo param)
    {
        m_result = table;
        m_user = user;

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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypePlayerMember);
        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            tr = new TableRow();
            table.Rows.Add(tr);

            m_content[0] = item.m_acc;
            m_content[1] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_money));

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                if (1 == j)
                {
                    td.ID = "RemainMoney_" + item.m_acc;
                }
                
                if (j == 2) // 生成分数文本框
                {
                    createScoreTextBox("txt" + item.m_acc, td);
                }
                else if (j == 3) // 上分下分
                {
                    addScoreButton1(td, item, 0);
                }
                else if (j == 4)
                {
                    addScoreButton1(td, item, 1);
                }
                else if (j == 5)
                {
                    addScoreButton1(td, item, 2);
                }
                else
                {
                    td.Text = m_content[j];
                }
            }
        }

        string page_html = "", foot_html = "";
        asp.getGen().genPage(param, @"/appaspx/account/AccountScorePlayer.aspx", ref page_html, ref foot_html, user);
        asp.getPage().InnerHtml = page_html;
        asp.getFoot().InnerHtml = foot_html;
    }

    public static TextBox createScoreTextBox(string ID, TableCell td)
    {
        TextBox txt = new TextBox();
        txt.Attributes.Add("value", "");
        txt.ID = ID;
        td.Controls.Add(txt);
        return txt;
    }

    public static void addScoreButton1(TableCell td, MemberInfo item, int info)
    {
        switch (info)
        {
            case 0: // 上分
                {
                    HtmlInputButton btn1 = new HtmlInputButton();
                    btn1.Attributes.Add("value", "上分");
                    btn1.Attributes.Add("op", "add");
                    btn1.Attributes.Add("acc", item.m_acc);
                    btn1.Attributes.Add("id", item.m_acc + "add");
                    btn1.Attributes.Add("class", "btn btn-primary");
                    btn1.Attributes.Add("targettype", item.m_accType.ToString());
                    td.Controls.Add(btn1);
                }
                break;
            case 1: // 下分
                {
                    HtmlInputButton btn2 = new HtmlInputButton();
                    btn2.Attributes.Add("value", "下分");
                    btn2.Attributes.Add("op", "des");
                    btn2.Attributes.Add("acc", item.m_acc);
                    btn2.Attributes.Add("id", item.m_acc + "des");
                    btn2.Attributes.Add("class", "btn btn-success");
                    btn2.Attributes.Add("targettype", item.m_accType.ToString());
                    td.Controls.Add(btn2);
                }
                break;
            case 2: // 提示
                {
                    Label L = new Label();
                    L.ID = "Label" + item.m_acc;
                    td.Controls.Add(L);
                }
                break;
        }
    }

    public static void addScoreButton(TableCell td, EventHandler handler, MemberInfo item)
    {
        Button btn1 = new Button();
        btn1.Text = "上分";
        btn1.CommandName = "add";
        btn1.CommandArgument = item.m_acc;
        btn1.ID = item.m_acc + "add";
        btn1.Click += handler;
        btn1.OnClientClick = string.Format("return confirmScore(true, 'player', '{0}')", item.m_acc);
        td.Controls.Add(btn1);

        Button btn2 = new Button();
        btn2.Text = "下分";
        btn2.CommandName = "des";
        btn2.CommandArgument = item.m_acc;
        btn2.ID = item.m_acc + "des";
        btn2.Click += handler;
        btn2.OnClientClick = string.Format("return confirmScore(false, 'player', '{0}')", item.m_acc);
        td.Controls.Add(btn2);
        Label L = new Label();
        L.ID = "Label" + item.m_acc;
        td.Controls.Add(L);
    }

    protected void onScoreOp(object sender, EventArgs e)
    {
        if (m_isRefreshed)
            return;

        ScoreOp op = new ScoreOp(m_user, false, m_result);
        op.onScoreOp(sender);
    }
}

//////////////////////////////////////////////////////////////////////////
// API管理员上分下分
public class ViewAPIAdminScoreInfo
{
    private static string[] s_head = new string[] { "API管理员", "余额", "分数", "操作" };

    private string[] m_content = new string[s_head.Length];

    Table m_result;
    GMUser m_user;
    bool m_isRefreshed;

    public ViewAPIAdminScoreInfo(bool IsRefreshed)
    {
        m_isRefreshed = IsRefreshed;
    }

    public void genTable(Table table, OpRes res, GMUser user)
    {
        m_result = table;
        m_user = user;

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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_acc;
            m_content[1] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_money));

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                if (j == 2) // 生成分数文本框
                {
                    ViewPlayerScoreInfo.createScoreTextBox("txt" + item.m_acc, td);
                }
                else if (j == 3) // 上分下分
                {
                    ViewPlayerScoreInfo.addScoreButton(td, new EventHandler(onScoreOp), item);
                }
                else
                {
                    td.Text = m_content[j];
                }
            }
        }
    }

    protected void onScoreOp(object sender, EventArgs e)
    {
        if (m_isRefreshed)
            return;

        ScoreOp op = new ScoreOp(m_user, true, m_result);
        op.onScoreOp(sender);
    }
}

//////////////////////////////////////////////////////////////////////////
// 逐级上分下分，可以越级，列出明细
public class ViewScoreStepByStep
{
    private static string[] s_head = new string[] { "账号", "别名", "账号类型", "直属下线", "会员", 
                                                     "账户余额", "状态","分数", ""/*8*/,"","" };

    private string[] m_content = new string[s_head.Length];

    public void genTable(Table table, OpRes res, GMUser user, EventHandler handler, ParamMemberInfo param)
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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
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

        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            tr = new TableRow();
            table.Rows.Add(tr);

            uParam.m_value = item.m_acc;

           // user.getOpLevelMgr().addSub(param.getRootUser(user), item.m_acc);
            addSubLevel(user, item.m_acc, param);

            uParam.m_className = "cLevelLink";
            uParam.m_text = item.m_acc;
            uParam.m_url = DefCC.ASPX_GM_INFO_NO_PARAM;

            m_content[0] = Tool.genHyperlink(uParam);
            m_content[1] = item.m_aliasName; // 别名
            m_content[2] = StrName.s_accountType[item.m_accType];

            if (item.m_accType == AccType.ACC_API)
            {
                m_content[3] = "";
            }
            else
            {
                long count = user.sqlDb.getRecordCount(TableName.GM_ACCOUNT,
                                                        string.Format(ViewGmAccountInfo.CONDITION_GM_CHILD, item.m_acc, AccType.ACC_AGENCY_SUB),
                                                        user.getMySqlServerID(),
                                                        MySqlDbName.DB_XIANXIA);
                uParam.m_className = "cSubLevelNum";
                uParam.m_text = count.ToString(); // "查看直属下线";
                uParam.m_url = DefCC.ASPX_SCORE_GM;
                m_content[3] = Tool.genHyperlink(uParam);
            }

            if (item.m_accType == AccType.ACC_GENERAL_AGENCY || item.m_accType == AccType.ACC_AGENCY_SUB)
            {
                m_content[4] = "";
            }
            else
            {
                long count = user.sqlDb.getRecordCount(TableName.PLAYER_ACCOUNT_XIANXIA,
                                                       string.Format(ViewGmAccountInfo.CONDITION_PLAYER_CHILD, item.m_acc),
                                                       user.getMySqlServerID(),
                                                       MySqlDbName.DB_XIANXIA);

                uParam.m_className = "cSubLevelNum";
                uParam.m_text = count.ToString();// "查看会员";
                uParam.m_url = DefCC.ASPX_SUB_SCORE_PLAYER;
                m_content[4] = Tool.genHyperlink(uParam); // 会员
            }
            m_content[5] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_money));

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                if (5 == j)
                {
                    td.ID = "RemainMoney_" + item.m_acc;
                }
                if (j == 6) // 状态
                {
                    addStateLabel(td, item);
                }
                else if (j == 7) // 分数
                {
                    ViewPlayerScoreInfo.createScoreTextBox("txt" + item.m_acc, td);
                }
                else if (j == 8) // 操作
                {
                    ViewPlayerScoreInfo.addScoreButton1(td, item, 0);
                }
                else if (j == 9) // 操作
                {
                    ViewPlayerScoreInfo.addScoreButton1(td, item, 1);
                }
                else if (j == 10) // 操作
                {
                    ViewPlayerScoreInfo.addScoreButton1(td, item, 2);
                }
                else
                {
                    td.Text = m_content[j];
                }
            }
        }
    }

    private void addStateLabel(TableCell td, MemberInfo item)
    {
        Label L = new Label();
        L.ID = "LabelState" + item.m_acc;
        L.Text = StrName.s_gmStateName[item.m_state];
        td.Controls.Add(L);
    }

    private void addSubLevel(GMUser user, string acc, ParamMemberInfo param)
    {
        URLParam uparam = new URLParam();
        uparam.m_key = "acc";
        uparam.m_value = acc;
        uparam.m_url = DefCC.ASPX_SCORE_GM;
        user.getOpLevelMgr().addSub(param.getRootUser(user), acc, uparam);
    }
}

//////////////////////////////////////////////////////////////////////////
// 上分下分的操作
class ScoreOp 
{
    // 操作者
    GMUser m_user;

    // 是否给GM管理员上下分
    bool m_istoGm;        

    Table m_result;

    public ScoreOp(GMUser user, bool istoGm, Table table)
    {
        m_user = user;
        m_istoGm = istoGm;
        m_result = table;
    }

    public void onScoreOp(object sender)
    {
        if (!(sender is Button))
            return;

        try
        {
            Button btn = (Button)sender;
            TextBox txt = (TextBox)m_result.FindControl("txt" + btn.CommandArgument);
            if (btn.CommandName == "add") // 上分
            {
                onAddScore(btn.CommandArgument, txt.Text);
            }
            else // 下分
            {
                onDecScore(btn.CommandArgument, txt.Text);
            }
        }
        catch (System.Exception ex)
        {
        }
    }

    private void onAddScore(string acc, string score)
    {
        ParamScore param = new ParamScore();
        param.m_op = 0;
        param.m_toAcc = acc;
        param.m_score = score;
        if (m_istoGm)
        {
            param.scoreToMgr();
        }
        else
        {
            param.scoreToPlayer();
        }

        OpRes res = m_user.doDyop(param, DyOpType.opTypeDyOpScore);
        setOpRes(res, param);
    }

    private void onDecScore(string acc, string score)
    {
        ParamScore param = new ParamScore();
        param.m_op = 1;
        param.m_toAcc = acc;
        param.m_score = score;
        if (m_istoGm)
        {
            param.scoreToMgr();
        }
        else
        {
            param.scoreToPlayer();
        }

        OpRes res = m_user.doDyop(param, DyOpType.opTypeDyOpScore);
        setOpRes(res, param);
    }

    void setOpRes(OpRes res, ParamScore param)
    {
        Label L = (Label)m_result.FindControl("Label" + param.m_toAcc);
        if (L != null)
        {
            L.Text = OpResMgr.getInstance().getResultString(res);
            L.Style.Clear();
            L.Style.Add("color", "red");
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 查看上分下分操作记录
public class ViewScoreOpRecord
{
    private static string[] s_head = new string[] { "编号", "日期", "操作账号","余额","别名","类型", 
        "操作金额", "操作目标账号","目标账号余额",  "订单ID", "API订单", "来源", "处理时间", "处理结果", "备注" };

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

        List<ResultScoreOpRecordItem> qresult = 
            (List<ResultScoreOpRecordItem>)user.getQueryResult(QueryType.queryTypeQueryScoreOpRecord);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            ResultScoreOpRecordItem item = qresult[i];
            tr = new TableRow();
//             if ((i & 1) == 0)
//             {
//                 tr.CssClass = "alt";
//             }
            table.Rows.Add(tr);

            m_content[0] = item.m_id.ToString(); // 编号
            m_content[1] = item.m_opTime; // 操作时间
            m_content[2] = item.m_opAcc;  // 操作账号
            m_content[3] = (item.m_opRemainMoney == -1 ? "" : ItemHelp.showMoneyValue(item.m_opRemainMoney).ToString());

            m_content[4] = item.m_opAccAlias; // 别名
            m_content[5] = StrName.s_scoreOpName[item.m_opType];      // 操作类型
            
            double score = ItemHelp.showMoneyValue(item.m_opScore);
            m_content[6] = ItemHelp.toStrByComma(score);

            m_content[7] = item.m_dstAcc; // 目标账号
            m_content[8] = ItemHelp.showMoneyValue(item.m_dstRemainMoney).ToString(); // 目标账号余额

            m_content[9] = item.m_orderId; // 订单ID
            m_content[10] = item.m_apiOrder; // API订单ID
            m_content[11] = StrName.s_logFrom[item.m_logFrom]; // 订单来源
            m_content[12] = item.m_finishTime;  // 处理时间

            m_content[13] = StrName.s_realTimeOrderState[item.m_opResult];

            if (item.m_opResult == PlayerReqOrderState.STATE_FAILED)
            {
                m_content[14] = StrName.getRealTimeOrderFailReason(item.m_failReason);
            }
            else
            {
                m_content[14] = "";
            }
            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                if (j == 5)
                {
                    if (ScropOpType.isAddScore(item.m_opType))
                    {
                        td.ForeColor = Color.Blue;
                    }
                    else
                    {
                        td.ForeColor = Color.Red;
                    }
                }
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 搜索具体的玩家
public class ViewPlayerInfo
{
    private static string[] s_head = new string[] { "会员账号", "别名", "注册日期", "创建者", "分数", 
       "最后登录", ""/*, "上次登录IP"*/, "影响盈利率", "状态",  "","","",""};
    private string[] m_content = new string[s_head.Length];

    public void genTable(Table table, OpRes res, GMUser user,
        object asp,
       ParamMemberInfo param)
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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypePlayerMember);
        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        int f = 0;
        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            tr = new TableRow();
            table.Rows.Add(tr);
            f = 0;

            m_content[f++] = item.m_acc;
            HtmlGenericControl aname = new HtmlGenericControl();
            aname.TagName = "a";
            aname.Attributes.Add("id", "aname_" + item.m_acc);
            aname.Attributes.Add("acc", item.m_acc);
            aname.Attributes.Add("class", "btn btn-primary");
            aname.InnerText = "修改";

            // 别名
            m_content[f++] = item.m_aliasName + "<br/>" + ItemHelp.genHTML(aname);
            m_content[f++] = item.m_createTime; // 注册日期
            m_content[f++] = item.m_owner;      // 创建者
            // 分数
            m_content[f++] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_money));
               
            m_content[f++] = item.m_lastLoginDate;  // 最后登录时间

            HtmlGenericControl apwd = new HtmlGenericControl();
            apwd.TagName = "a";
            apwd.Attributes.Add("id", "apwd_" + item.m_acc);
            apwd.Attributes.Add("acc", item.m_acc);
            apwd.InnerText = "重置密码";
            m_content[f++] = ItemHelp.genHTML(apwd); // 账号设置

            HtmlGenericControl aAffectRate = new HtmlGenericControl();
            aAffectRate.TagName = "a";
            aAffectRate.Attributes.Add("id", "arate_" + item.m_acc);
            aAffectRate.Attributes.Add("acc", item.m_acc);
            if (item.m_isAffectRate)
            {
                aAffectRate.InnerText = "结束影响";
                aAffectRate.Attributes.Add("param", "false");
                aAffectRate.Attributes.Add("class", "btn btn-success");

                m_content[f++] = "是" + "<br/>" + ItemHelp.genHTML(aAffectRate);
            }
            else
            {
                aAffectRate.InnerText = "开始影响";
                aAffectRate.Attributes.Add("param", "true");
                aAffectRate.Attributes.Add("class", "btn btn-danger");

                m_content[f++] = "否" + "<br/>" + ItemHelp.genHTML(aAffectRate);
            }

            m_content[f++] = StrName.s_stateName[item.m_state];
            // 状态
            HtmlGenericControl aState = new HtmlGenericControl();
            aState.TagName = "a";
            aState.Attributes.Add("id", "astate_" + item.m_acc);
            aState.Attributes.Add("acc", item.m_acc);
            if (item.m_state == PlayerState.STATE_BLOCK)
            {
                aState.InnerText = "解封";
                aState.Attributes.Add("op", "blockAcc");
                aState.Attributes.Add("param", "false");
                aState.Attributes.Add("class", "btn btn-success");

                m_content[f++] = ItemHelp.genHTML(aState);
            }
            else if (item.m_state == PlayerState.STATE_GAME)
            {
                aState.InnerText = "踢出";
                aState.Attributes.Add("op", "kick");
                aState.Attributes.Add("class", "btn btn-danger");
                m_content[f++] = ItemHelp.genHTML(aState);

                HtmlGenericControl aState1 = new HtmlGenericControl();
                aState1.TagName = "a";
                aState1.Attributes.Add("id", "astate_" + item.m_acc);
                aState1.Attributes.Add("acc", item.m_acc);
                aState1.InnerText = "解锁";
                aState1.Attributes.Add("op", "unlock");
                aState1.Attributes.Add("class", "btn btn-success");
                m_content[f++] = ItemHelp.genHTML(aState1);

                HtmlGenericControl aState2 = new HtmlGenericControl();
                aState2.TagName = "a";
                aState2.Attributes.Add("id", "astate_" + item.m_acc);
                aState2.Attributes.Add("acc", item.m_acc);
                aState2.InnerText = "停封";
                aState2.Attributes.Add("op", "blockAcc");
                aState2.Attributes.Add("param", "true");
                aState2.Attributes.Add("class", "btn btn-danger");
                m_content[f++] = ItemHelp.genHTML(aState2);
            }
            else
            {
                aState.InnerText = "停封";
                aState.Attributes.Add("op", "blockAcc");
                aState.Attributes.Add("param", "true");
                aState.Attributes.Add("class", "btn btn-danger");
                m_content[f++] = ItemHelp.genHTML(aState);

                HtmlGenericControl aState1 = new HtmlGenericControl();
                aState1.TagName = "a";
                aState1.Attributes.Add("id", "astate_" + item.m_acc);
                aState1.Attributes.Add("acc", item.m_acc);
                aState1.InnerText = "清理登录失败次数";
                aState1.Attributes.Add("op", "clearFail");
                aState1.Attributes.Add("class", "btn btn-success");

                m_content[f++] = ItemHelp.genHTML(aState1);
            }

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                m_content[j] = "";
            }
        }

        if (asp != null)
        {
            if (asp is WebManager.appaspx.account.AccountSearchPlayer)
            {
                genPaging1((WebManager.appaspx.account.AccountSearchPlayer)asp, param, user);
            }
            else
            {
                genPaging2((WebManager.appaspx.account.sub.AccountSubPlayer)asp, param, user);
            }
        }  
    }

    void genPaging1(WebManager.appaspx.account.AccountSearchPlayer asp, ParamMemberInfo param, GMUser user)
    {
        string page_html = "", foot_html = "";

        asp.getGen().genPage(param, @"/appaspx/account/AccountSearchPlayer.aspx", ref page_html, ref foot_html, user);
        asp.getPage().InnerHtml = page_html;
        asp.getFoot().InnerHtml = foot_html;
    }
    void genPaging2(WebManager.appaspx.account.sub.AccountSubPlayer asp, ParamMemberInfo param, GMUser user)
    {
        string page_html = "", foot_html = "";

        asp.getGen().genPage(param, @"/appaspx/account/sub/AccountSubPlayer.aspx", ref page_html, ref foot_html, user);
        asp.getPage().InnerHtml = page_html;
        asp.getFoot().InnerHtml = foot_html;
    }
}

//////////////////////////////////////////////////////////////////////////
// 捕鱼盈利率调整
public class TableStatFishlordControl
{
    private static string[] s_head = new string[] { "房间", "期望盈利率", "系统总收入", "系统总支出", "盈亏情况", "实际盈利率", "当前人数", "选择" };
    private string[] m_content = new string[s_head.Length];

    public TableStatFishlordControl()
    {
    }

    public OpRes onModifyExpRate(GMUser user, string expRate, string roomList, DyOpType dyType)
    {
        ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
        p.m_isReset = false;
        p.m_expRate = expRate;
        p.m_roomList = roomList;

        OpRes res = user.doDyop(p, dyType/*DyOpType.opTypeFishlordParamAdjust*/);
        // genExpRateTable(m_expRateTable, user);

        return res;
    }

    public OpRes onReset(GMUser user, string roomList, DyOpType dyType)
    {
        ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
        p.m_isReset = true;
        p.m_roomList = roomList;

        OpRes res = user.doDyop(p, dyType/*DyOpType.opTypeFishlordParamAdjust*/);
        // genExpRateTable(m_expRateTable, user);

        return res;
    }

    // 期望盈利率表格
    public void genExpRateTable(Table table, GMUser user, QueryType qType)
    {
        table.GridLines = GridLines.Both;
        TableRow tr = new TableRow();
        table.Rows.Add(tr);

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

       // long totalIncome = 0;
       // long totalOutlay = 0;

        OpRes res = user.doQuery(null, qType/*QueryType.queryTypeFishlordParam*/);
        Dictionary<int, ResultFishlordExpRate> qresult
            = (Dictionary<int, ResultFishlordExpRate>)user.getQueryResult(qType/*QueryType.queryTypeFishlordParam*/);

        for (i = 1; i <= 1; i++)
        {
            m_content[0] = StrName.s_roomName[i - 1];
            if (qresult.ContainsKey(i))
            {
                ResultFishlordExpRate r = qresult[i];
                m_content[1] = r.m_expRate.ToString();
                m_content[2] = ItemHelp.showMoneyValue(r.m_totalIncome).ToString();
                m_content[3] = ItemHelp.showMoneyValue(r.m_totalOutlay).ToString();
                m_content[4] = ItemHelp.showMoneyValue(r.m_totalIncome - r.m_totalOutlay).ToString();
                m_content[5] = r.getFactExpRate();
                m_content[6] = r.m_curPlayerCount.ToString();
              //  totalIncome += r.m_totalIncome;
              //  totalOutlay += r.m_totalOutlay;
            }
            else
            {
                m_content[1] = "0.05";
                m_content[2] = "0";
                m_content[3] = "0";
                m_content[4] = "0";
                m_content[5] = "0";
                m_content[6] = "0";
            }
            m_content[7] = Tool.getCheckBoxHtml("roomList", i.ToString(), false);

            tr = new TableRow();
            table.Rows.Add(tr);
            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                if (j == 4)
                {
                    setColor(td, m_content[j]);
                }
            }
        }

      //  addStatFoot(table, totalIncome, totalOutlay);
    }

    // 增加统计页脚
    protected void addStatFoot(Table table, long totalIncome, long totalOutlay)
    {
        TableRow tr = new TableRow();
        table.Rows.Add(tr);
        m_content[0] = "总计";
        m_content[1] = "";
        // 总收入
        m_content[2] = ItemHelp.showMoneyValue(totalIncome).ToString();
        // 总支出
        m_content[3] = ItemHelp.showMoneyValue(totalOutlay).ToString();
        // 总盈亏
        m_content[4] = ItemHelp.showMoneyValue(totalIncome - totalOutlay).ToString();
        m_content[5] = "";
        m_content[6] = "";
        m_content[7] = "";

        for (int j = 0; j < s_head.Length; j++)
        {
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = m_content[j];

            if (j == 4)
            {
                setColor(td, m_content[j]);
            }
        }
    }

    protected void setColor(TableCell td, string num)
    {
        if (num[0] == '-')
        {
            td.ForeColor = Color.Red;
        }
        else
        {
            td.ForeColor = Color.Green;
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 捕鱼桌子盈利率查询
public class TableStatFishlordDeskEarningsRate
{
    private static string[] s_head = new string[] { "桌子ID", "系统总收入", "系统总支出", "盈亏情况", "实际盈利率" };
    private string[] m_content = new string[s_head.Length];
    private string m_page = "";
    private string m_foot = "";
    private string m_callURL;

    public TableStatFishlordDeskEarningsRate(string callURL)
    {
        m_callURL = callURL;
    }

    public string getPage() { return m_page; }
    public string getFoot() { return m_foot; }

    public void onQueryDesk(GMUser user,
                            PageGift gen,
                            int roomId,
                            Table table,
                            QueryType qType)
    {
        ParamQueryGift param = new ParamQueryGift();
        param.m_curPage = gen.curPage;
        param.m_countEachPage = gen.rowEachPage;
        param.m_state = roomId;

        user.doQuery(param, qType/*QueryType.queryTypeFishlordDeskParam*/);
        genDeskTable(table, user, param, qType, gen);
    }

    // 桌子的盈利率表格
    protected void genDeskTable(Table table, GMUser user, ParamQueryGift param, QueryType qType, PageGift gen)
    {
        TableRow tr = new TableRow();
        table.Rows.Add(tr);

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        List<ResultFishlordExpRate> qresult
            = (List<ResultFishlordExpRate>)user.getQueryResult(qType/*QueryType.queryTypeFishlordDeskParam*/);

        bool alt = true;
        foreach (var info in qresult)
        {
            tr = new TableRow();
            table.Rows.Add(tr);

            if (alt)
            {
                tr.CssClass = "alt";
            }
            alt = !alt;

            m_content[0] = info.m_roomId.ToString();
            m_content[1] = ItemHelp.showMoneyValue(info.m_totalIncome).ToString();
            m_content[2] = ItemHelp.showMoneyValue(info.m_totalOutlay).ToString();
            m_content[3] = ItemHelp.showMoneyValue(info.getDelta()).ToString();
            m_content[4] = info.getFactExpRate();

            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }

        param.m_state--;
        string page_html = "", foot_html = "";
        gen.genPage(param, m_callURL/*@"/appaspx/stat/StatFishlordDeskEarningsRate.aspx"*/,
            ref page_html, ref foot_html, user);

        m_page = page_html;
        m_foot = foot_html;
    }
}

//////////////////////////////////////////////////////////////////////////
// 捕鱼算法阶段分析
public class TableStatFishlordStage
{
    private static string[] s_head = new string[] { "时间", "房间名称", "阶段", "收入", "支出", "盈利率" };
    private string[] m_content = new string[s_head.Length];

    private string m_page = "";
    private string m_foot = "";
    private string m_callURL;

    public TableStatFishlordStage(string callURL)
    {
        m_callURL = callURL;
    }

    public string getPage() { return m_page; }
    public string getFoot() { return m_foot; }

    public void onQuery(GMUser user,
                           string time,
                           int roomId,
                           PageGift gen,
                           Table table,
                           QueryType qType)
    {
        ParamQueryGift param = new ParamQueryGift();
        param.m_param = time;
        param.m_state = roomId;
        param.m_curPage = gen.curPage;
        param.m_countEachPage = gen.rowEachPage;

        OpRes res = user.doQuery(param, qType/*QueryType.queryTypeFishlordStage*/);
        genTable(table, res, param, user, qType, gen);
    }

    private void genTable(Table table, OpRes res, ParamQueryGift param, GMUser user, QueryType qType, PageGift gen)
    {
        table.GridLines = GridLines.Both;
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

        List<FishlordStageItem> qresult = (List<FishlordStageItem>)user.getQueryResult(qType/*QueryType.queryTypeFishlordStage*/);
        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = qresult[i].m_time;
            m_content[1] = StrName.s_roomName[qresult[i].m_roomId - 1];
            m_content[2] = StrName.s_stageName[qresult[i].m_stage];
            m_content[3] = ItemHelp.showMoneyValue(qresult[i].m_income).ToString();
            m_content[4] = ItemHelp.showMoneyValue(qresult[i].m_outlay).ToString();
            m_content[5] = qresult[i].getFactExpRate();

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }

        string page_html = "", foot_html = "";
        gen.genPage(param, m_callURL, ref page_html, ref foot_html, user);

        m_page = page_html;
        m_foot = foot_html;
    }
}

//////////////////////////////////////////////////////////////////////////
// 鱼的统计
public class TableStatFish
{
    private static string[] s_head = new string[] { "鱼ID", "名称", "击中次数", "死亡次数", "命中率", "支出", "收入", "盈利率", "房间" };
    private string[] m_content = new string[s_head.Length];

    public void onClearFishTable(GMUser user, string tableName, Table table)
    {
        OpRes res = user.doDyop(tableName, DyOpType.opTypeClearFishTable);
        table.Rows.Clear();
    }

    public void onQuery(GMUser user, Table table, int roomIndex, QueryType qtype)
    {
        TableRow tr = new TableRow();
        table.Rows.Add(tr);

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        OpRes res = user.doQuery(roomIndex, qtype/*QueryType.queryTypeFishStat*/);
        List<ResultFish> qresult = (List<ResultFish>)user.getQueryResult(qtype);

        foreach (var data in qresult)
        {
            m_content[0] = data.m_fishId.ToString();
            FishCFGData fishInfo = null;
            if (qtype == QueryType.queryTypeFishStat) // 经典捕鱼
            {
                fishInfo = FishCFG.getInstance().getValue(data.m_fishId);
            }
            else // 鳄鱼公园
            {
                fishInfo = FishParkCFG.getInstance().getValue(data.m_fishId);
            }
            if (fishInfo != null)
            {
                m_content[1] = fishInfo.m_fishName;
            }
            else
            {
                m_content[1] = "";
            }
            m_content[2] = data.m_hitCount.ToString();
            m_content[3] = data.m_dieCount.ToString();
            m_content[4] = data.getHit_Die();
            m_content[5] = ItemHelp.showMoneyValue(data.m_outlay).ToString();
            m_content[6] = ItemHelp.showMoneyValue(data.m_income).ToString();
            m_content[7] = data.getOutlay_Income();
            if (data.m_roomId > 0)
            {
                m_content[8] = StrName.s_roomName[data.m_roomId - 1];
            }
            else
            {
                m_content[8] = "";
            }

            tr = new TableRow();
            table.Rows.Add(tr);
            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// API号审批
public class ViewApiApprove
{
    private static string[] s_head = new string[] { "API账号", "API前缀", "创建者", "申请时间", "", "", "" };

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

        List<ResultAPIItem> qresult =
            (List<ResultAPIItem>)user.getQueryResult(QueryType.queryTypeQueryApiApprove);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            ResultAPIItem item = qresult[i];
            tr = new TableRow();
            table.Rows.Add(tr);

            m_content[0] = item.m_apiAcc;
            m_content[1] = item.m_apiPrefix;
            m_content[2] = item.m_apiCreator;  
            m_content[3] = item.m_genTime;
            m_content[6] = "";

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                if (j == 4) // 同意
                {
                    HtmlInputButton btn = new HtmlInputButton();
                    btn.Attributes.Add("value", "同意");
                    btn.Attributes.Add("op", "pass");
                    btn.Attributes.Add("acc", item.m_apiAcc);
                    btn.Attributes.Add("id", item.m_apiAcc + "_" + "pass");
                    btn.Attributes.Add("class", "btn btn-success");
                    td.Controls.Add(btn);
                }
                else if (j == 5) // 拒绝
                {
                    HtmlInputButton btn = new HtmlInputButton();
                    btn.Attributes.Add("value", "拒绝");
                    btn.Attributes.Add("op", "reject");
                    btn.Attributes.Add("acc", item.m_apiAcc);
                    btn.Attributes.Add("id", item.m_apiAcc + "_" + "reject");
                    btn.Attributes.Add("class", "btn btn-primary");
                    td.Controls.Add(btn);
                }
                else if (j == 6)
                {
                    td.ID = "td_" + item.m_apiAcc;
                    td.ClientIDMode = ClientIDMode.Static;
                }
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家订单
public class ViewPlayerOrder
{
    private static string[] s_head = new string[] { "序号", "订单号", "会员", "所属代理", "实际金额", "下单时间", "订单状态", "订单类型", "", "" };

    private string[] m_content = new string[s_head.Length];

    private GMUser m_user;
    private Table m_result;

    public void genTable(Table table, OpRes res, GMUser user, EventHandler handler)
    {
        m_user = user;
        m_result = table;

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

        List<ResultPlayerOrderItem> qresult =
            (List<ResultPlayerOrderItem>)user.getQueryResult(QueryType.queryTypeQueryPlayerOrder);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        TdButtonGenerator gen = new TdButtonGenerator();

       // URLParam uParam = new URLParam();
       // uParam.m_key = "index";

        for (i = 0; i < qresult.Count; i++)
        {
            ResultPlayerOrderItem item = qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

           /* uParam.m_value = i.ToString();
            uParam.m_text = StrName.s_playerOrderIdName[item.m_orderType];
            uParam.clearExParam();
            uParam.addExParam("op", 0);
            uParam.m_url = DefCC.ASPX_PLAYER_ORDER;*/

            m_content[0] = i.ToString();
            m_content[1] = item.m_orderId;
            m_content[2] = item.m_playerAcc;
            m_content[3] = item.m_playerOwner;
            m_content[4] = item.m_orderMoney.ToString();
            m_content[5] = item.m_orderTime;
            m_content[6] = StrName.s_playerOrderState[item.m_orderState];
            m_content[7] = StrName.s_playerOrderIdName[item.m_orderType];

            if (item.m_orderState == OrderState.STATE_WAIT)
            {
               /* m_content[8] = Tool.genHyperlink(uParam);

                uParam.m_text = "取消订单";
                uParam.clearExParam();
                uParam.addExParam("op", 1);
                m_content[9] = Tool.genHyperlink(uParam);*/
            }
          
            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);

                if (j == 8)
                {
                    if (item.m_orderState != OrderState.STATE_WAIT)
                        continue;

                    gen.td = td;
                    string btnId = "btn" + i.ToString() + j.ToString();
                    string infoId = "Label" + btnId;

                    gen.addButton(StrName.s_playerOrderIdName[item.m_orderType],
                        "exec",
                        item.m_orderId + "$" + item.m_playerAcc,
                        btnId,
                        handler, "",
                        infoId);
                }
                else if (j == 9)
                {
                    if (item.m_orderState != OrderState.STATE_WAIT)
                        continue;

                    gen.td = td;
                    string btnId = "btn" + i.ToString() + j.ToString();
                    string infoId = "Label" + btnId;

                    gen.addButton("取消订单",
                        "cancel",
                        item.m_orderId + "$" + item.m_playerAcc,
                        btnId,
                        handler, "",
                        infoId);
                }
                else
                {
                    td.Text = m_content[j];
                }
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 日志查看
public partial class CLogViewer
{
    private static string[] s_head = new string[] { "ID", "帐号", "帐号IP", "操作类型", "操作时间", "描述", "备注" };
    private string[] m_content = new string[s_head.Length];
    private PageViewLog m_gen = new PageViewLog(50);

    protected HtmlGenericControl m_page;
    protected HtmlGenericControl m_foot;
    protected Table LogTable;
    protected DropDownList opType;

    public PageViewLog getPageViewFoot() { return m_gen; }

    public CLogViewer(Table t, HtmlGenericControl page, HtmlGenericControl foot,DropDownList opt)
    {
        LogTable = t;
        m_page = page;
        m_foot = foot;
        opType = opt;
    }

    private void fillTable(ResultOpLogItem data, bool css)
    {
        TableRow tr = new TableRow();
        if (css)
        {
            tr.CssClass = "alt";
        }

        m_content[0] = data.m_id.ToString();
        m_content[1] = data.m_opAcc;
        m_content[2] = data.m_opAccIP;
        m_content[3] = data.m_opName;
        m_content[4] = data.m_opDateTime;
        m_content[5] = data.m_opDesc;
        m_content[6] = data.m_comment;

        LogTable.Rows.Add(tr);
        int col = s_head.Length;
        for (int i = 0; i < col; i++)
        {
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = m_content[i];
        }
    }

    public void genTable(GMUser user, ParamQueryOpLog param, string callURL)
    {
        LogTable.GridLines = GridLines.Both;
        // 添加标题行
        TableRow tr = new TableRow();
        LogTable.Rows.Add(tr);
        int col = s_head.Length;
        int i = 0;
        for (; i < col; i++)
        {
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
        OpRes res = mgr.doQuery(param, QueryType.queryTypeOpLog, user);
        List<ResultOpLogItem> result = (List<ResultOpLogItem>)mgr.getQueryResult(QueryType.queryTypeOpLog);
        if (result != null)
        {
            bool css = true;

            foreach (ResultOpLogItem data in result)
            {
                fillTable(data, css);
                css = !css;
            }
        }

        m_page.InnerHtml = "";
        m_foot.InnerHtml = "";
        string page_html = "", foot_html = "";
        param.m_logType = opType.SelectedIndex;
        m_gen.genPage(param, callURL, ref page_html, ref foot_html, user);
        m_page.InnerHtml = page_html;
        m_foot.InnerHtml = foot_html;
    }
}

//////////////////////////////////////////////////////////////////////////
// 实时上下分订单记录
public class ViewRealTimeOrder
{
    private static string[] s_head = new string[] { "订单ID", "API订单ID", "下单日期","处理日期","订单类型", 
        "操作金额", "操作账号","玩家账号","订单状态", "备注" };

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

        List<OrderInfoItem> qresult = (List<OrderInfoItem>)user.getQueryResult(QueryType.queryTypeQueryRealTimeOrder);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            OrderInfoItem item = qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_orderId;      // 订单ID
            m_content[1] = item.m_apiOrderId;   // 对应API订单
            m_content[2] = item.m_genTime.ToString(ConstDef.DATE_TIME24);  // 下单日期
            m_content[3] = item.m_finishTime.ToString(ConstDef.DATE_TIME24); // 处理日期
            m_content[4] = StrName.s_scoreOpName[item.m_orderType];      // 订单类型

            double score = ItemHelp.showMoneyValue(item.m_money); // 操作金额
            m_content[5] = ItemHelp.toStrByComma(score);

            m_content[6] = item.m_gmAcc; // 操作账号
            m_content[7] = item.m_playerAcc; // 玩家账号
            m_content[8] = StrName.s_realTimeOrderState[item.m_orderState];

            if (item.m_orderState == PlayerReqOrderState.STATE_FAILED)
            {
                m_content[9] = StrName.getRealTimeOrderFailReason(item.m_failReason);
            }
            else
            {
                m_content[9] = "";
            }

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 游戏历史记录
public class ViewGameHistoryBase
{
    virtual public void genTable(Table table, OpRes res, GMUser user) { }

    public static ViewGameHistoryBase create(int gameId)
    {
        switch (gameId)
        {
            case (int)GameId.crocodile:
                {
                    return new ViewGameHistoryCrocodile();
                }
                break;
            case (int)GameId.dice:
                {
                    return new ViewGameHistoryDice();
                }
                break;
            case (int)GameId.cows:
                {
                    return new ViewGameHistoryCows();
                }
                break;
            case (int)GameId.baccarat:
                {
                    return new ViewGameHistoryBaccarat();
                }
                break;
            case (int)GameId.shcd:
                {
                    return new ViewGameHistoryShcd();
                }
                break;
        }

        return null;
    }
}

//////////////////////////////////////////////////////////////////////////
// 百家乐
public class ViewGameHistoryBaccarat : ViewGameHistoryBase
{
    private static string[] s_head = new string[] { "开牌日期", "游戏名称", "局数", "靴-局数", "结果", "闲家牌", "庄家牌" };

    private string[] m_content = new string[s_head.Length];

    public override void genTable(Table table, OpRes res, GMUser user)
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

        List<ResultGameHistoryItem> qresult =
            (List<ResultGameHistoryItem>)user.getQueryResult(QueryType.queryTypeGameHistory);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            ResultGameHistoryBaccaratIem item = (ResultGameHistoryBaccaratIem)qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_time;
            m_content[1] = StrName.s_gameName[item.m_gameId];
            m_content[2] = item.m_totalBound.ToString();
            m_content[3] = item.getBootBound();
            m_content[4] = item.getResult();
            m_content[5] = item.getXianJiaCard();
            m_content[6] = item.getZhuangJiaCard();

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
//　骰宝
public class ViewGameHistoryDice : ViewGameHistoryBase
{
    private static string[] s_head = new string[] { "开牌日期", "游戏名称", "局数", "开奖结果" };

    private string[] m_content = new string[s_head.Length];

    public override void genTable(Table table, OpRes res, GMUser user)
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

        List<ResultGameHistoryItem> qresult =
            (List<ResultGameHistoryItem>)user.getQueryResult(QueryType.queryTypeGameHistory);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            ResultGameHistoryDicetIem item = (ResultGameHistoryDicetIem)qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_time;
            m_content[1] = StrName.s_gameName[item.m_gameId];
            m_content[2] = item.m_totalBound.ToString();
            m_content[3] = item.getResult();

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
//　牛牛
public class ViewGameHistoryCows : ViewGameHistoryBase
{
    private static string[] s_head = new string[] { "开牌日期", "游戏名称", "局数", "庄家", "东", "南", "西", "北" };

    private string[] m_content = new string[s_head.Length];

    public override void genTable(Table table, OpRes res, GMUser user)
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

        List<ResultGameHistoryItem> qresult =
            (List<ResultGameHistoryItem>)user.getQueryResult(QueryType.queryTypeGameHistory);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            ResultGameHistoryCowstIem item = (ResultGameHistoryCowstIem)qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_time;
            m_content[1] = StrName.s_gameName[item.m_gameId];
            m_content[2] = item.m_totalBound.ToString();
            m_content[3] = item.getBankerCard();
            m_content[4] = item.getEastCard();
            m_content[5] = item.getSouthCard();
            m_content[6] = item.getWestCard();
            m_content[7] = item.getNorthCard();

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
//　鳄鱼大亨
public class ViewGameHistoryCrocodile : ViewGameHistoryBase
{
    private static string[] s_head = new string[] { "开牌日期", "游戏名称", "局数", "开奖结果", "射灯", "人人有奖", "彩金" };

    private string[] m_content = new string[s_head.Length];

    public override void genTable(Table table, OpRes res, GMUser user)
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

        List<ResultGameHistoryItem> qresult =
            (List<ResultGameHistoryItem>)user.getQueryResult(QueryType.queryTypeGameHistory);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            ResultGameHistoryCrocodiletIem item = (ResultGameHistoryCrocodiletIem)qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_time;
            m_content[1] = StrName.s_gameName[item.m_gameId];
            m_content[2] = item.m_totalBound.ToString();
            m_content[3] = item.getResult(); // 开奖结果
            m_content[4] = item.getSpotLight();
            m_content[5] = item.getAllPrizes();
            m_content[6] = item.getHandSel();

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
//　黑红梅方
public class ViewGameHistoryShcd : ViewGameHistoryBase
{
    private static string[] s_head = new string[] { "开奖日期", "游戏名称", "局数", "开奖结果" };

    private string[] m_content = new string[s_head.Length];

    public override void genTable(Table table, OpRes res, GMUser user)
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

        List<ResultGameHistoryItem> qresult =
            (List<ResultGameHistoryItem>)user.getQueryResult(QueryType.queryTypeGameHistory);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.Count; i++)
        {
            ResultGameHistoryShcdItem item = (ResultGameHistoryShcdItem)qresult[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_time;
            m_content[1] = StrName.s_gameName[item.m_gameId];
            m_content[2] = item.m_totalBound.ToString();
            m_content[3] = item.getResult(); // 开奖结果

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 权限分配
public class ViewRightAssign
{
    private static string[] s_head = new string[] { "账号", "权限", "设置" };
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

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        var allR = RightMap.getDispatchRight(AccType.ACC_SUPER_ADMIN_SUB);

        for (i = 0; i < qresult.Count; i++)
        {
            MemberInfo item = qresult[i];
            if (item.m_accType != AccType.ACC_AGENCY && 
                item.m_accType != AccType.ACC_GENERAL_AGENCY &&
                item.m_accType != AccType.ACC_SUPER_ADMIN_SUB)
                continue;

            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            // 账号
            td = new TableCell();
            td.Text = item.m_acc;
            tr.Cells.Add(td);

            // 权限
            td = new TableCell();
            tr.Cells.Add(td);

            foreach (var d in allR)
            {
                HtmlInputCheckBox box = new HtmlInputCheckBox();
                td.Controls.Add(box);
                box.Value = d.Key;
                Label L = new Label();
                L.Text = d.Value.m_rightName;
                td.Controls.Add(L);

                if (RightMap.hasRight(d.Key, item.m_gmRight))
                {
                    box.Checked = true;
                }
                else
                {
                    box.Checked = false;
                }
                if (RightMap.hasInherentRight(item.m_accType, d.Key))
                {
                    box.Attributes["disabled"] = "disabled";
                    box.Checked = true;
                }
            }

            td = new TableCell();
            tr.Cells.Add(td);
            HtmlInputButton btn = new HtmlInputButton();
            btn.Attributes["value"] = "设置";
            td.Controls.Add(btn);
        }
    }
}

//////////////////////////////////////////////////////////////////////////
//　某玩家的输赢报表
public class ViewPlayerWinLose
{
    private static string[] s_head = new string[] { "日期", "玩家账号", "总押注", "总返还", "洗码量" };

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

        StatResultWinLose qresult = (StatResultWinLose)user.getStatResult(StatType.statTypeWinLose);

        int i = 0, j = 0;
        // 表头
        for (i = 0; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        for (i = 0; i < qresult.m_detail.Count; i++)
        {
            StatResultWinLoseItem item = (StatResultWinLoseItem)qresult.m_detail[i];
            tr = new TableRow();
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            table.Rows.Add(tr);

            m_content[0] = item.m_time.ToShortDateString();
            m_content[1] = item.m_acc;
            m_content[2] = item.m_totalOutlay.ToString();
            m_content[3] = item.m_totalIncome.ToString();
            m_content[4] = item.m_totalWashCount.ToString();

            for (j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}
