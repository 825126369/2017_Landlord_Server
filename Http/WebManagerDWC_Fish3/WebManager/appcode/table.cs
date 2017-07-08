using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

// 捕鱼盈利率调整
public class TableStatFishlordControl
{
    private static string[] s_head = new string[] { "房间", "期望盈利率", "系统总收入", "系统总支出", "盈亏情况",
        "实际盈利率", "当前人数", "废弹", "导弹产出","选择" };
    private string[] m_content = new string[s_head.Length];

    private static string[] s_head1 = new string[] { "房间", "BOSS收入", "BOSS支出", "池子收入", "池子支出", "实际池子盈利率" };

    public TableStatFishlordControl()
    {
    }

    public OpRes onModifyExpRate(GMUser user, string expRate, string roomList, DyOpType dyType)
    {
        ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
        p.m_isReset = false;
        p.m_expRate = expRate;
        p.m_roomList = roomList;
        p.m_rightId = RightDef.FISH_PARAM_CONTROL;
        OpRes res = user.doDyop(p, dyType/*DyOpType.opTypeFishlordParamAdjust*/);
       // genExpRateTable(m_expRateTable, user);

        return res;
    }

    public OpRes onReset(GMUser user, string roomList, DyOpType dyType)
    {
        ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
        p.m_isReset = true;
        p.m_roomList = roomList;
        p.m_rightId = RightDef.FISH_PARAM_CONTROL;
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

        long totalIncome = 0;
        long totalOutlay = 0;
        long totalAbandonedbullets = 0;
        long totalMissileCount = 0;

        OpRes res = user.doQuery(null, qType/*QueryType.queryTypeFishlordParam*/);
        Dictionary<int, ResultFishlordExpRate> qresult
            = (Dictionary<int, ResultFishlordExpRate>)user.getQueryResult(qType/*QueryType.queryTypeFishlordParam*/);

        for (i = 1; i <= StrName.s_roomName.Length; i++)
        {
            m_content[0] = StrName.s_roomName[i - 1];
            if (qresult.ContainsKey(i))
            {
                ResultFishlordExpRate r = qresult[i];
                m_content[1] = r.m_expRate.ToString();
                m_content[2] = r.m_totalIncome.ToString();
                m_content[3] = r.m_totalOutlay.ToString();
                m_content[4] = (r.m_totalIncome - r.m_totalOutlay).ToString();
                m_content[5] = r.getFactExpRate();
                m_content[6] = r.m_curPlayerCount.ToString();
                m_content[7] = r.m_abandonedbullets.ToString();
                m_content[8] = r.m_missileCount.ToString();

                totalIncome += r.m_totalIncome;
                totalOutlay += r.m_totalOutlay;
                totalAbandonedbullets += r.m_abandonedbullets;
                totalMissileCount += r.m_missileCount;
            }
            else
            {
                m_content[1] = "0.05";
                m_content[2] = "0";
                m_content[3] = "0";
                m_content[4] = "0";
                m_content[5] = "0";
                m_content[6] = "0";
                m_content[7] = "0";
                m_content[8] = "0";
            }
            m_content[9] = Tool.getCheckBoxHtml("roomList", i.ToString(), false);

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

        addStatFoot(table, totalIncome, totalOutlay, totalAbandonedbullets, totalMissileCount);
    }

    public void getBossTable(Table table, GMUser user, QueryType qType, string midT, string highT)
    {
        table.GridLines = GridLines.Both;
        TableRow tr = new TableRow();
        table.Rows.Add(tr);

        int i = 0;
        for (; i < s_head1.Length; i++)
        {
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head1[i];
        }

        ParamFishlordBoss param = new ParamFishlordBoss();
        param.m_midTime = midT;
        param.m_highTime = highT;
        OpRes res = user.doQuery(param, qType);
        List<ResultFishlordExpRate> qresult
            = (List<ResultFishlordExpRate>)user.getQueryResult(param, qType/*QueryType.queryTypeFishlordParam*/);

        for (i = 0; i < qresult.Count; i++)
        {
            ResultFishlordExpRate item = qresult[i];
            m_content[0] = StrName.s_roomName[item.m_roomId - 1];
            m_content[1] = item.m_robotIncome.ToString();
            m_content[2] = item.m_robotOutlay.ToString();

            m_content[3] = item.m_totalIncome.ToString();
            m_content[4] = item.m_totalOutlay.ToString();
            m_content[5] = item.getFactExpRate();

            tr = new TableRow();
            table.Rows.Add(tr);
            for (int j = 0; j < s_head1.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }

    // 增加统计页脚
    protected void addStatFoot(Table table, long totalIncome, long totalOutlay, long totalAbandonedbullets,
        long totalMissileCount)
    {
        TableRow tr = new TableRow();
        table.Rows.Add(tr);
        m_content[0] = "总计";
        m_content[1] = "";
        // 总收入
        m_content[2] = totalIncome.ToString();
        // 总支出
        m_content[3] = totalOutlay.ToString();
        // 总盈亏
        m_content[4] = (totalIncome - totalOutlay).ToString();
        m_content[5] = "";
        m_content[6] = "";
        m_content[7] = totalAbandonedbullets.ToString();
        m_content[8] = totalMissileCount.ToString();
        m_content[9] = "";

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
    private static string[] s_head = new string[] { "桌子ID", "系统总收入", "系统总支出", "盈亏情况", "实际盈利率", "废弹" };
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
            m_content[1] = info.m_totalIncome.ToString();
            m_content[2] = info.m_totalOutlay.ToString();
            m_content[3] = info.getDelta().ToString();
            m_content[4] = info.getFactExpRate();
            m_content[5] = info.m_abandonedbullets.ToString();

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
            m_content[3] = qresult[i].m_income.ToString();
            m_content[4] = qresult[i].m_outlay.ToString();
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
            m_content[5] = data.m_outlay.ToString();
            m_content[6] = data.m_income.ToString();
            m_content[7] = data.getOutlay_Income();
            if (data.m_roomId > 0)
            {
                m_content[8] = StrName.s_fishRoomName[data.m_roomId - 1];
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
// Td活跃数据
public class TableTdActivation
{
    private static string[] s_head = new string[] {"日期", "渠道", "注册", "设备激活", "活跃", "收入", "付费人数", "付费次数", "公众号收入",
        "次日留存率", "3日留存率", "7日留存率" ,"30日留存率", "ARPU", "ARPPU", "付费率","新增用户付费", "新增用户付费率",
    "次日设备留存率"/*15*/, "3日设备留存率", "7日设备留存率" ,"30日设备留存率", "平均每个设备生成账号"};
    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0, f = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<ResultActivationItem> qresult = (List<ResultActivationItem>)user.getQueryResult(QueryType.queryTypeTdActivation);
        foreach (var data in qresult)
        {
            f = 0;
            m_content[f++] = data.m_genTime.ToShortDateString();
            TdChannelInfo info = TdChannel.getInstance().getValue(data.m_channel);
            if (info != null)
            {
                m_content[f++] = info.m_channelName;
            }
            else
            {
                m_content[f++] = data.m_channel;
            }

            m_content[f++] = data.m_regeditCount.ToString();
            m_content[f++] = data.m_deviceActivationCount.ToString();
            m_content[f++] = data.m_activeCount.ToString();
            m_content[f++] = data.m_totalIncome.ToString();
            m_content[f++] = data.m_rechargePersonNum.ToString();
            m_content[f++] = data.m_rechargeCount.ToString();
            m_content[f++] = data.m_wchatPublicNumIncome.ToString();

            m_content[f++] = data.get2DayRemain();
            m_content[f++] = data.get3DayRemain();
            m_content[f++] = data.get7DayRemain();
            m_content[f++] = data.get30DayRemain();
            m_content[f++] = data.getARPU();
            m_content[f++] = data.getARPPU();
            m_content[f++] = data.getRechargeRate();

            m_content[f++] = data.m_newAccIncome > -1 ? data.m_newAccIncome.ToString() : "";
            m_content[f++] = data.getNewAccRechargeRate();

            m_content[f++] = data.get2DayDevRemain();
            m_content[f++] = data.get3DayDevRemain();
            m_content[f++] = data.get7DayDevRemain();
            m_content[f++] = data.get30DayDevRemain();
            m_content[f++] = data.getAccNumberPerDev();

            tr = new TableRow();
            table.Rows.Add(tr);
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            i++;

            for (int j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 最高在线玩家个数
public class TableMaxOnline
{
    private static string[] s_head = new string[] { "日期", "最高在线时间点", "在线人数" };
    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<ResultMaxOnlineItem> qresult = (List<ResultMaxOnlineItem>)user.getQueryResult(QueryType.queryTypeMaxOnline);
        foreach (var data in qresult)
        {
            m_content[0] = data.m_date;
            m_content[1] = data.m_timePoint;
            m_content[2] = data.m_playerNum.ToString();

            tr = new TableRow();
            table.Rows.Add(tr);
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            i++;

            for (int j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家金币总和
public class TablePlayerTotalMoney
{
    private static string[] s_head = new string[] { "日期", "总金币", "保险箱内总金币" };
    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<ResultTotalPlayerMoneyItem> qresult = (List<ResultTotalPlayerMoneyItem>)user.getQueryResult(QueryType.queryTypeTotalPlayerMoney);
        foreach (var data in qresult)
        {
            m_content[0] = data.m_date;
            m_content[1] = string.Format("{0:N0}", data.m_money);
            if (data.m_safeBox > -1)
            {
                m_content[2] = string.Format("{0:N0}", data.m_safeBox);
            }
            else
            {
                m_content[2] = "";
            }

            tr = new TableRow();
            table.Rows.Add(tr);
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            i++;

            for (int j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 平均价值
public class TableTdLTVBase
{
    protected static string[] s_head = new string[] {"渠道", "日期", "注册", "1日价值", "3日价值", "7日价值", "14日价值", "30日价值", 
        "60日价值", "90日价值"};
    protected string[] m_content = new string[s_head.Length];

    public static TableTdLTVBase create(int type)
    {
        switch (type)
        {
            case 0:
                return new TableTdLTVFull();
                break;
            default:
                return new TableTdLTVChannel();
                break;
        }
        return null;
    }

    public virtual void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        fillData(user, table);
    }

    public virtual void fillData(GMUser user, Table table) { }
    public virtual OpRes query(GMUser user, ParamQuery param) { return OpRes.op_res_failed; }
}

public class TableTdLTVChannel : TableTdLTVBase
{
    public override void fillData(GMUser user, Table table)
    {
        TableRow tr = null;
        TableCell td = null;
        int i = 0;

        List<ResultLTVItem> qresult = (List<ResultLTVItem>)user.getQueryResult(QueryType.queryTypeLTV);
        foreach (var data in qresult)
        {
            TdChannelInfo info = TdChannel.getInstance().getValue(data.m_channel);
            if (info != null)
            {
                m_content[0] = info.m_channelName;
            }
            else
            {
                m_content[0] = data.m_channel;
            }

            m_content[1] = data.m_genTime;
            m_content[2] = data.m_regeditCount.ToString();
            m_content[3] = data.get1DayAveRecharge();
            m_content[4] = data.get3DayAveRecharge();
            m_content[5] = data.get7DayAveRecharge();
            m_content[6] = data.get14DayAveRecharge();
            m_content[7] = data.get30DayAveRecharge();
            m_content[8] = data.get60DayAveRecharge();
            m_content[9] = data.get90DayAveRecharge();
            
            tr = new TableRow();
            table.Rows.Add(tr);
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            i++;

            for (int j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }

    public override OpRes query(GMUser user, ParamQuery param) 
    {
        OpRes res = user.doQuery(param, QueryType.queryTypeLTV);
        return res;
    }
}

// 全体平均
public class TableTdLTVFull : TableTdLTVBase
{
    public override void fillData(GMUser user, Table table)
    {
        TableRow tr = null;
        TableCell td = null;
        int i = 0;

        List<ResultLTVItem> qresult = (List<ResultLTVItem>)user.getStatResult(StatType.statTypeLTV);
        foreach (var data in qresult)
        {
            m_content[0] = "";
            m_content[1] = data.m_genTime;
            m_content[2] = data.m_regeditCount.ToString();
            m_content[3] = data.get1DayAveRecharge();
            m_content[4] = data.get3DayAveRecharge();
            m_content[5] = data.get7DayAveRecharge();
            m_content[6] = data.get14DayAveRecharge();
            m_content[7] = data.get30DayAveRecharge();
            m_content[8] = data.get60DayAveRecharge();
            m_content[9] = data.get90DayAveRecharge();

            tr = new TableRow();
            table.Rows.Add(tr);
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            i++;

            for (int j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }

    public override OpRes query(GMUser user, ParamQuery param)
    {
        OpRes res = user.doStat(param, StatType.statTypeLTV);
        return res;
    }
}

//////////////////////////////////////////////////////////////////////////
// 捕鱼3种道具 锁定、急速、散射
public class TableFishConsumeItem
{
    private static string[] s_head = new string[] { "日期", "房间", "道具", "钻石购买使用", "游戏赠送使用" };
    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        ResultConsumeItem qresult = (ResultConsumeItem)user.getQueryResult(null, QueryType.queryTypeFishConsume);
        var timeList = qresult.timeList;
        foreach (var t in timeList)
        {
            for (i = 1; i < 5; i++)
            {
                RooomItemConsume ric = qresult.getRooomItemConsume(t, i);
                if (ric == null)
                    continue;

                for (int j = 1; j <= 3; j++)
                {
                    CFishItem fi = ric.getIem(j);
                    if (fi != null)
                    {
                        tr = new TableRow();
                        table.Rows.Add(tr);

                        m_content[0] = t.ToShortDateString();
                        m_content[1] = StrName.s_roomName[i - 1];
                        m_content[2] = fi.getItemName();
                        m_content[3] = fi.m_buyCount.ToString();
                        m_content[4] = fi.m_useCount.ToString();

                        for (int k = 0; k < s_head.Length; k++)
                        {
                            td = new TableCell();
                            tr.Cells.Add(td);
                            td.Text = m_content[k];
                        }
                    }
                }
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 大R流失
public class TableRLose
{
    private static string[] s_head = new string[] { "玩家id", "昵称", "VIP等级", "金币", "钻石", "龙珠", "最后登录时间" };

    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<RLoseItem> qresult = (List<RLoseItem>)user.getQueryResult(QueryType.queryTypeRLose);
        for (; i < qresult.Count; i++)
        {
            tr = new TableRow();
            table.Rows.Add(tr);

            RLoseItem item = qresult[i];
            m_content[0] = item.m_playerId.ToString();
            m_content[1] = item.m_nickName;
            m_content[2] = item.m_vipLevel.ToString();
            m_content[3] = item.m_gold.ToString();
            m_content[4] = item.m_gem.ToString();
            m_content[5] = item.m_dragonBall.ToString();
            m_content[6] = item.m_lastLoginTime;

            for (int k = 0; k < s_head.Length; k++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[k];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 充值用户统计
public class TableRechargePlayer
{
    private static string[] s_head = new string[] { "玩家id", "渠道","充值次数", "充值金额", 
        "注册时间", "上线次数", "剩余金币","最后上线时间", "曾经最大金币",
        StrName.s_gameName[(int)GameId.fishlord],StrName.s_gameName[(int)GameId.shcd],
        StrName.s_gameName[(int)GameId.cows], StrName.s_gameName[(int)GameId.dragon],
        StrName.s_gameName[(int)GameId.crocodile],StrName.s_gameName[(int)GameId.baccarat],
        StrName.s_gameName[(int)GameId.dice]};

    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0, f = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }
        
        i = 0;
        List<RechargePlayerItem> qresult = (List<RechargePlayerItem>)user.getStatResult(StatType.statTypeRechargePlayer);
        for (; i < qresult.Count; i++)
        {
            tr = new TableRow();
            table.Rows.Add(tr);

            f = 0;
            RechargePlayerItem item = qresult[i];
            m_content[f++] = item.m_playerId.ToString();
            m_content[f++] = item.getChannelName();
            m_content[f++] = item.m_rechargeCount.ToString();
            m_content[f++] = item.m_rechargeMoney.ToString();
            m_content[f++] = item.m_regTime.ToString(); // 注册时间
            m_content[f++] = item.m_loginCount.ToString();// 上线次数
            m_content[f++] = item.m_remainGold.ToString(); // 剩余金币
            m_content[f++] = item.m_lastLoginTime.ToString(); // 最后上线时间
            m_content[f++] = item.m_mostGold.ToString(); // 曾经最大金币

            m_content[f++] = item.getEnterCount((int)GameId.fishlord).ToString();
            m_content[f++] = item.getEnterCount((int)GameId.shcd).ToString();
            m_content[f++] = item.getEnterCount((int)GameId.cows).ToString();
            m_content[f++] = item.getEnterCount((int)GameId.dragon).ToString();
            m_content[f++] = item.getEnterCount((int)GameId.crocodile).ToString();
            m_content[f++] = item.getEnterCount((int)GameId.baccarat).ToString();
            m_content[f++] = item.getEnterCount((int)GameId.dice).ToString();

            for (int k = 0; k < s_head.Length; k++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[k];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家龙珠监控
public class TablePlayerDragonBall
{
    private static string[] s_head = new string[] { "玩家ID", "注册时间", 
       "打到龙珠",  "送出龙珠", "收取龙珠", "龙珠兑换","初始龙珠", "龙珠结余",
       "金币充值获得", "金币其余获得", "金币消耗","初始金币", "结余金币",
       "钻石充值获得", "钻石其余获得", "钻石消耗", "初始钻石", "结余钻石",
       "总充值", "时间段内充值"
        };

    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0, f = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<StatPlayerDragonBallItem> qresult = (List<StatPlayerDragonBallItem>)user.getStatResult(StatType.statTypePlayerDragonBall);
        for (; i < qresult.Count; i++)
        {
            tr = new TableRow();
            table.Rows.Add(tr);

            f = 0;
            StatPlayerDragonBallItem item = qresult[i];
            m_content[f++] = item.m_playerId.ToString();
            m_content[f++] = item.m_regTime.ToString();
            m_content[f++] = item.m_dbgain.ToString();
            m_content[f++] = item.m_dbsend.ToString(); 
            m_content[f++] = item.m_dbaccept.ToString();
            m_content[f++] = item.m_dbexchange.ToString(); 
            m_content[f++] = item.m_dbStart.ToString(); 
            m_content[f++] = item.m_dbRemain.ToString();

            m_content[f++] = item.m_goldByRecharge.ToString();
            m_content[f++] = item.m_goldByOther.ToString();
            m_content[f++] = item.m_goldConsume.ToString();
            m_content[f++] = item.m_goldStart.ToString();
            m_content[f++] = item.m_goldRemain.ToString();

            m_content[f++] = item.m_gemByRecharge.ToString();
            m_content[f++] = item.m_gemByOther.ToString();
            m_content[f++] = item.m_gemConsume.ToString();
            m_content[f++] = item.m_gemStart.ToString();
            m_content[f++] = item.m_gemRemain.ToString();

            m_content[f++] = item.m_rechargeFromReg.ToString();
            m_content[f++] = item.m_todayRecharge.ToString();

            for (int k = 0; k < s_head.Length; k++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[k];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 每日龙珠
public class TableDragonBallDaily
{
    private static string[] s_head = new string[] { "日期", "每日充值", 
       "龙珠产出",  "龙珠消耗", "龙珠结余", "盈利折算RMB"};

    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0, f = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<DragonBallDailyItem> qresult = (List<DragonBallDailyItem>)user.getQueryResult(QueryType.queryTypeDragonBallDaily);
        for (; i < qresult.Count; i++)
        {
            tr = new TableRow();
            table.Rows.Add(tr);

            f = 0;
            DragonBallDailyItem item = qresult[i];
            m_content[f++] = item.m_time.ToShortDateString();
            m_content[f++] = item.m_todayRecharge.ToString();
            m_content[f++] = item.m_dragonBallGen.ToString();
            m_content[f++] = item.m_dragonBallConsume.ToString();
            m_content[f++] = item.m_dragonBallRemain.ToString();
            m_content[f++] = item.m_rmb.ToString();

            for (int k = 0; k < s_head.Length; k++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[k];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 充值玩家监控
public class TableRechargePlayerMonitor
{
    private static string[] s_head = new string[] { "玩家ID", "当前炮数", 
       "累计付费",  "累计游戏时间", 
       "初次付费日期", "初次付费游戏时间","初次付费项目","初次付费时拥有金币","初次付费炮数",
       "再次付费日期", "再次付费游戏时间","再次付费项目","再次付费时拥有金币","再次付费炮数",
       "注册时间", "龙珠结余", "龙珠累计获得", "龙珠累计转出"
        };

    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0, f = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<RechargePlayerMonitorItem> qresult = (List<RechargePlayerMonitorItem>)user.getQueryResult(QueryType.queryTypeRechargePlayerMonitor);
        for (; i < qresult.Count; i++)
        {
            tr = new TableRow();
            table.Rows.Add(tr);

            f = 0;
            RechargePlayerMonitorItem item = qresult[i];
            m_content[f++] = item.m_playerId.ToString();
            m_content[f++] = item.getOpenRate(item.m_curFishLevel);
            m_content[f++] = item.m_totalRecharge.ToString();
            m_content[f++] = item.getGameTime(item.m_totalGameTime);

            m_content[f++] = item.m_firstRechargeTime.ToString();
            m_content[f++] = item.getGameTime(item.m_firstRechargeGameTime);
            m_content[f++] = item.getRechargePoint(item.m_firstRechargePoint);
            m_content[f++] = item.m_firstRechargeGold.ToString();
            m_content[f++] = item.getOpenRate(item.m_firstRechargeFishLevel);

            m_content[f++] = item.m_secondRechargeTime.ToString();
            m_content[f++] = item.getGameTime(item.m_secondRechargeGameTime);
            m_content[f++] = item.getRechargePoint(item.m_secondRechargePoint);
            m_content[f++] = item.m_secondRechargeGold.ToString();
            m_content[f++] = item.getOpenRate(item.m_secondRechargeFishLevel);

            m_content[f++] = item.m_regTime.ToString();
            m_content[f++] = item.m_remainDragon.ToString();
            m_content[f++] = item.m_gainDragon.ToString();
            m_content[f++] = item.m_sendDragon.ToString();

            for (int k = 0; k < s_head.Length; k++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[k];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家收支统计
public class TablePlayerIncomeExpenses
{
    private static string[] s_head = new string[] { "日期", "活跃人数", "",
       "初始值",  "免费获得总计", "免费获得人均", "充值获得总计","充值获得人均", "消耗总计","消耗人均",
       "结余总计","结余人均", "数据库结余"
        };
    private static string[] s_head1 = { "金币", "钻石", "龙珠", "话费券" };

    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res, ParamIncomeExpenses param)
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

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<StatIncomeExpensesItem> qresult = (List<StatIncomeExpensesItem>)user.getStatResult(StatType.statTypePlayerIncomeExpenses);
        for (; i < qresult.Count; i++)
        {
            StatIncomeExpensesItem item = qresult[i];
            genDataCol(item, table, param);
        }
    }

    private void genDataCol(StatIncomeExpensesItem item, Table table, ParamIncomeExpenses param)
    {
        int curRow = table.Rows.Count;
        int rowCount = s_head1.Length;
        if (param.m_property > -1)
        {
            rowCount = 1;
        }

        // 每块数据生成足够行
        for (int i = 0; i < rowCount; i++)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
        }

        // 日期列
        TableCell td = new TableCell();
        table.Rows[curRow].Cells.Add(td);
        td.RowSpan = rowCount;
        td.Text = item.m_genTime.ToShortDateString();

        td = new TableCell();
        table.Rows[curRow].Cells.Add(td);
        td.RowSpan = rowCount;
        td.Text = item.m_playerCount.ToString();

        if (rowCount > 1)
        {
            for (int i = 0; i < s_head1.Length; i++)
            {
                fillRow(item, i, i, table.Rows[curRow + i]);
            }
        }
        else
        {
            fillRow(item, 0, param.m_property, table.Rows[curRow]);
        }
    }

    private void fillRow(StatIncomeExpensesItem item, int curRow, int property, TableRow tr)
    {
        switch (property)
        {
            case 0:  // 金币
                {
                    genCell(tr, "金币");
                    genCell(tr, item.m_goldStart.ToString());       // 初始值
                    genCell(tr, item.m_goldFreeGain.ToString());    // 免费获得总计
                    genCell(tr, ItemHelp.getRate(item.m_goldFreeGain, item.m_playerCount, 1));    // 免费获得人均

                    genCell(tr, item.m_goldRechargeGain.ToString()); // 充值获得总计
                    genCell(tr, ItemHelp.getRate(item.m_goldRechargeGain, item.m_playerCount, 1)); // 充值获得人均

                    genCell(tr, item.m_goldConsume.ToString()); // 消耗总计
                    genCell(tr, ItemHelp.getRate(item.m_goldConsume, item.m_playerCount, 1)); // 消耗人均

                    genCell(tr, item.m_goldRemain.ToString()); // 结余总计
                    genCell(tr, ItemHelp.getRate(item.m_goldRemain, item.m_playerCount, 1)); // 结余人均

                    genCell(tr, item.getDataBaseRemain(item.m_dataBaseGoldRemain));
                }
                break;
            case 1:  // 钻石
                {
                    genCell(tr, "钻石");
                    genCell(tr, item.m_gemStart.ToString());       // 初始值
                    genCell(tr, item.m_gemFreeGain.ToString());    // 免费获得总计
                    genCell(tr, ItemHelp.getRate(item.m_gemFreeGain, item.m_playerCount, 1));    // 免费获得人均

                    genCell(tr, item.m_gemRechargeGain.ToString());
                    genCell(tr, ItemHelp.getRate(item.m_gemRechargeGain, item.m_playerCount, 1));

                    genCell(tr, item.m_gemConsume.ToString());
                    genCell(tr, ItemHelp.getRate(item.m_gemConsume, item.m_playerCount, 1));

                    genCell(tr, item.m_gemRemain.ToString());
                    genCell(tr, ItemHelp.getRate(item.m_gemRemain, item.m_playerCount, 1));

                    genCell(tr, item.getDataBaseRemain(item.m_dataBaseGemRemain));
                }
                break;
            case 2: // 龙珠
                {
                    genCell(tr, "龙珠");
                    genCell(tr, item.m_dbStart.ToString());       // 初始值
                    genCell(tr, item.m_dbFreeGain.ToString());    // 免费获得总计
                    genCell(tr, ItemHelp.getRate(item.m_dbFreeGain, item.m_playerCount, 1));    // 免费获得人均

                    genCell(tr, "");
                    genCell(tr, "");

                    genCell(tr, item.m_dbConsume.ToString());
                    genCell(tr, ItemHelp.getRate(item.m_dbConsume, item.m_playerCount, 1));

                    genCell(tr, item.m_dbRemain.ToString());
                    genCell(tr, ItemHelp.getRate(item.m_dbRemain, item.m_playerCount, 1));

                    genCell(tr, item.getDataBaseRemain(item.m_dataBaseDbRemain));
                }
                break;
            case 3: // 话费券
                {
                    genCell(tr, "话费券");
                    genCell(tr, item.m_chipStart.ToString());       // 初始值
                    genCell(tr, item.m_chipFreeGain.ToString());    // 免费获得总计
                    genCell(tr, ItemHelp.getRate(item.m_chipFreeGain, item.m_playerCount, 1));    // 免费获得人均

                    genCell(tr, "");
                    genCell(tr, "");

                    genCell(tr, item.m_chipConsume.ToString());
                    genCell(tr, ItemHelp.getRate(item.m_chipConsume, item.m_playerCount, 1));

                    genCell(tr, item.m_chipRemain.ToString());
                    genCell(tr, ItemHelp.getRate(item.m_chipRemain, item.m_playerCount, 1));

                    genCell(tr, item.getDataBaseRemain(item.m_dataBaseChipRemain));
                }
                break;
        }
    }

    private TableCell genCell(TableRow tr, string text)
    {
        TableCell td = new TableCell();
        tr.Cells.Add(td);
        td.Text = text;
        return td;
    }
}

//////////////////////////////////////////////////////////////////////////
// 游戏台账
public class TableGameStandingBook
{
    private static string[] s_head = new string[] { "日期", "前日库存", "今日系统金币减少",
       "今日系统金币增加",  "今日实际库存", "今日记录差值比"};

    private static string[] s_head1 = { "金币", "钻石", "龙珠", "话费券" };

    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res, ParamIncomeExpenses param)
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

        int i = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        List<StatIncomeExpensesItem> qresult = (List<StatIncomeExpensesItem>)user.getStatResult(StatType.statTypePlayerIncomeExpenses);
        for (; i < qresult.Count; i++)
        {
            StatIncomeExpensesItem item = qresult[i];
            //genDataCol(item, table, param);

            tr = new TableRow();
            table.Rows.Add(tr);
            fillRow(item, 0, param.m_property, tr);
        }
    }

    private void genDataCol(StatIncomeExpensesItem item, Table table, ParamIncomeExpenses param)
    {
        int curRow = table.Rows.Count;
        int rowCount = s_head1.Length;
        if (param.m_property > -1)
        {
            rowCount = 1;
        }

        // 每块数据生成足够行
        for (int i = 0; i < rowCount; i++)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
        }

        // 日期列
        TableCell td = new TableCell();
        table.Rows[curRow].Cells.Add(td);
        td.RowSpan = rowCount;
        td.Text = item.m_genTime.ToShortDateString();

        td = new TableCell();
        table.Rows[curRow].Cells.Add(td);
        td.RowSpan = rowCount;
        td.Text = item.m_playerCount.ToString();

        if (rowCount > 1)
        {
            for (int i = 0; i < s_head1.Length; i++)
            {
                fillRow(item, i, i, table.Rows[curRow + i]);
            }
        }
        else
        {
            fillRow(item, 0, param.m_property, table.Rows[curRow]);
        }
    }

    private void fillRow(StatIncomeExpensesItem item, int curRow, int property, TableRow tr)
    {
        genCell(tr, item.m_genTime.ToShortDateString());
        switch (property)
        {
            case 1:  // 金币
                {
                   // genCell(tr, "金币");
                    genCell(tr, item.m_goldStart.ToString("N0"));       // 初始值
                    genCell(tr, (item.m_goldFreeGain + item.m_goldRechargeGain).ToString("N0"));
                    genCell(tr, item.m_goldConsume.ToString("N0")); // 消耗总计
                    genCell(tr, item.m_goldRemain.ToString("N0")); // 结余总计
                    genCell(tr,
                        relativeErrorBeyond(item.m_goldStart + item.m_goldFreeGain + item.m_goldRechargeGain - item.m_goldConsume,
                        item.m_goldRemain));
                }
                break;
            case 2:  // 钻石
                {
                   // genCell(tr, "钻石");
                    genCell(tr, item.m_gemStart.ToString("N0"));       // 初始值
                    genCell(tr, (item.m_gemFreeGain + item.m_gemRechargeGain).ToString("N0"));
                    genCell(tr, item.m_gemConsume.ToString("N0"));
                    genCell(tr, item.m_gemRemain.ToString("N0"));

                    genCell(tr,
                        relativeErrorBeyond(item.m_gemStart + item.m_gemFreeGain + item.m_gemRechargeGain - item.m_gemConsume,
                        item.m_gemRemain));
                }
                break;
            case 14: // 龙珠
                {
                   // genCell(tr, "龙珠");
                    genCell(tr, item.m_dbStart.ToString("N0"));       // 初始值
                    genCell(tr, item.m_dbFreeGain.ToString("N0"));    // 免费获得总计
                    genCell(tr, item.m_dbConsume.ToString("N0"));
                    genCell(tr, item.m_dbRemain.ToString("N0"));

                    genCell(tr,
                        relativeErrorBeyond(item.m_dbStart + item.m_dbFreeGain - item.m_dbConsume,
                        item.m_dbRemain));
                }
                break;
            case 11: // 话费券
                {
                  //  genCell(tr, "话费券");
                    genCell(tr, item.m_chipStart.ToString("N0"));       // 初始值
                    genCell(tr, item.m_chipFreeGain.ToString("N0"));    // 免费获得总计
                    genCell(tr, item.m_chipConsume.ToString("N0"));
                    genCell(tr, item.m_chipRemain.ToString("N0"));

                    genCell(tr,
                        relativeErrorBeyond(item.m_chipStart + item.m_chipFreeGain - item.m_chipConsume,
                        item.m_chipRemain));
                }
                break;
        }
    }

    private TableCell genCell(TableRow tr, string text)
    {
        TableCell td = new TableCell();
        tr.Cells.Add(td);
        td.Text = text;
        return td;
    }

    string relativeErrorBeyond(double cur, double accuracy)
    {
        if (accuracy == 0.0)
            return "";

        double delta = Math.Abs(cur - accuracy);
        double e = delta / accuracy;
        return (Math.Round(e * 100, 2)).ToString() + "%";
    }
}

//////////////////////////////////////////////////////////////////////////
// 每小时付费
public class TableTdRechargePerHour
{
    private static string[] s_head = new string[] {"日期", 
        "0点累计","1点累计", "2点累计", "3点累计", "4点累计", "5点累计", "6点累计", "7点累计", 
        "8点累计","9点累计", "10点累计", "11点累计", "12点累计", "13点累计", "14点累计", "15点累计", 
        "16点累计","17点累计", "18点累计", "19点累计", "20点累计", "21点累计", "22点累计", "23点累计"};
    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0, f = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        DataEachDay qresult = (DataEachDay)user.getQueryResult(QueryType.queryTypeRechargePerHour);
        var allData = qresult.getData();
        foreach (var data in allData)
        {
            f = 0;
            m_content[f++] = data.m_time.ToShortDateString();

            for (int k = 0; k < data.getCount(); k++)
            {
                m_content[f++] = data.getData(k).ToString();
            }

            tr = new TableRow();
            table.Rows.Add(tr);
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            i++;

            for (int j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 每小时的实时在线
public class TableTdOnlinePlayerNumPerHour
{
    private static string[] s_head = new string[] {"日期", 
        "最高在线","最高在线时间点", "最低在线", "最低在线时间点", "平均在线"};
    private string[] m_content = new string[s_head.Length];

    public void genTable(GMUser user, Table table, OpRes res)
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

        int i = 0, f = 0;
        for (; i < s_head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = s_head[i];
        }

        i = 0;
        DataEachDay qresult = (DataEachDay)user.getQueryResult(QueryType.queryTypeOnlinePlayerNumPerHour);
        var allData = qresult.getData();
        foreach (var data in allData)
        {
            f = 0;
            m_content[f++] = data.m_time.ToShortDateString();
            m_content[f++] = data.getData(data.m_maxIndex).ToString();
            m_content[f++] = data.m_maxIndex.ToString() + "点";
            m_content[f++] = data.getData(data.m_minIndex).ToString();
            m_content[f++] = data.m_minIndex.ToString() + "点";
            m_content[f++] = qresult.average(data);

            tr = new TableRow();
            table.Rows.Add(tr);
            if ((i & 1) == 0)
            {
                tr.CssClass = "alt";
            }
            i++;

            for (int j = 0; j < s_head.Length; j++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
public class TableHVBase
{
    protected string[] m_headH;
    protected string[] m_headV;

    public TableCell genCell(TableRow tr, string text)
    {
        TableCell td = new TableCell();
        tr.Cells.Add(td);
        td.Text = text;
        return td;
    }

    public void genTable(GMUser user, Table table, OpRes res, object param)
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

        int i = 0;
        for (; i < m_headH.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = m_headH[i];
        }

        fillTableData(user, table, param);
    }

    public void fillHeadV(Table table, int startRow)
    {
        for (int i = 0; i < m_headV.Length; i++)
        {
            genCell(table.Rows[startRow + i], m_headV[i]);
        }
    }

    public virtual void fillTableData(GMUser user, Table table, object param) { }
}

// 游戏金币流动统计总计
public class TableStatServerEarningsTotal : TableHVBase
{
    // 横向标题
    private static string[] s_head = new string[] { "日期", "", "捕鱼", "鳄鱼大亨", "牛牛", "五龙", "黑红梅方" };
    private static string[] s_head1 = new string[] { "系统总收入", "系统总支出", "盈利值", "盈利率", "活跃人数" };

    public TableStatServerEarningsTotal()
    {
        m_headH = s_head;
        m_headV = s_head1;
    }

    public override void fillTableData(GMUser user, Table table, object param)
    {
        int[] ids = { (int)GameId.fishlord, (int)GameId.crocodile, (int)GameId.cows, (int)GameId.dragon, (int)GameId.shcd };
        ResultServerEarningsTotal qresult = (ResultServerEarningsTotal)user.getQueryResult(null, QueryType.queryTypeServerEarnings);
        PlayerTypeData<EarningItem> total = qresult.sum(ids);
        genDataCol(total, table, "总计", ids);

        var arr = qresult.getAllDescByTime();
        foreach (var item in arr)
        {
            genDataCol(item.Value, table, item.Key.ToShortDateString(), ids);
        }
    }

    private void genDataCol(PlayerTypeData<EarningItem> item, Table table, string time, int[] ids)
    {
        int curRow = table.Rows.Count;
        int rowCount = m_headV.Length;
        
        // 每块数据生成足够行
        for (int i = 0; i < rowCount; i++)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
        }

        // 日期列
        TableCell td = new TableCell();
        table.Rows[curRow].Cells.Add(td);
        td.RowSpan = rowCount;
        td.Text = time;

        fillHeadV(table, curRow);

        for (int i = 0; i < ids.Length; i++)
        {
            var dataItem = item.getData(ids[i]);
            fillColDataBlock(dataItem, table, curRow);
        }
    }

    private void fillColDataBlock(EarningItem item, Table table, int startRow)
    {
        int i = 0;
        if (item == null)
        {
            genCell(table.Rows[startRow + i], "");
            i++;
            genCell(table.Rows[startRow + i], "");
            i++;
            genCell(table.Rows[startRow + i], "");
            i++;
            genCell(table.Rows[startRow + i], "");
            i++;
            genCell(table.Rows[startRow + i], "");
        }
        else
        {
            genCell(table.Rows[startRow + i], item.getRoomIncome(4).ToString());
            i++;
            genCell(table.Rows[startRow + i], item.getRoomOutlay(4).ToString());
            i++;
            genCell(table.Rows[startRow + i], item.getDelta(4).ToString());
            i++;
            genCell(table.Rows[startRow + i], item.getFactExpRate(4).ToString());
            i++;
            genCell(table.Rows[startRow + i], item.getRoomIncome(0).ToString());
        }
    }
}

//////////////////////////////////////////////////////////////////////////
public class TableHeadInfo
{
    public string m_title;
    public int m_rowSpan;
    public int m_colSpan;
}

public class TableHHBase
{
    protected string[] m_headH1;
    protected string[] m_headH2;

    public TableCell genCell(TableRow tr, string text)
    {
        TableCell td = new TableCell();
        tr.Cells.Add(td);
        td.Text = text;
        return td;
    }

    public void genTable(GMUser user, Table table, OpRes res, object param)
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

        genHead(m_headH1, tr);
        tr = new TableRow();
        table.Rows.Add(tr);
        genHead(m_headH2, tr);

        fillTableData(user, table, param);
    }

    List<TableHeadInfo> getHeadList(string[] head)
    {
        List<TableHeadInfo> res = new List<TableHeadInfo>();
        for (int i = 0; i < head.Length; i++)
        {
            string[] tmp = Tool.split(head[i], '#', StringSplitOptions.RemoveEmptyEntries);
            TableHeadInfo ti = new TableHeadInfo();
            ti.m_title = tmp[0];
            ti.m_rowSpan = Convert.ToInt32(tmp[1]);
            ti.m_colSpan = Convert.ToInt32(tmp[2]);
            res.Add(ti);
        }
        return res;
    }

    void genHead(string[] head, TableRow tr)
    {
        List<TableHeadInfo> infoList = getHeadList(head);
        TableCell td = null;
        int i = 0;
        for (; i < infoList.Count; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = infoList[i].m_title;
            td.ColumnSpan = infoList[i].m_colSpan;
            td.RowSpan = infoList[i].m_rowSpan;
        }
    }
    public virtual void fillTableData(GMUser user, Table table, object param) { }
}

// 用户下注情况查询
public class TablePlayerGameBet : TableHHBase
{
    private static string[] s_head = new string[] { "日期#2#1", "玩家ID#2#1", "携带量#1#3", "下注量#1#3", "赢钱数#1#3", "输钱数#1#3",
        "当日流水#2#1", "输赢详情#2#1" };
    private static string[] s_head1 = new string[] { 
        "平均#1#1", "最大#1#1", "最小#1#1", 
        "平均#1#1", "最大#1#1", "最小#1#1",
        "平均#1#1", "最大#1#1", "最小#1#1",
        "平均#1#1", "最大#1#1", "最小#1#1",};

    public TablePlayerGameBet()
    {
        m_headH1 = s_head;
        m_headH2 = s_head1;
    }

    public override void fillTableData(GMUser user, Table table, object param)
    {
        List<ResultItemPlayerGameBet> qresult =
            (List<ResultItemPlayerGameBet>)user.getQueryResult(QueryType.queryTypePlayerGameBet);
        for (int i = 0; i < qresult.Count; i++)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            genDataRow(qresult[i], tr);
        }
    }

    void genDataRow(ResultItemPlayerGameBet data, TableRow tr)
    {
        genCell(tr, data.m_time.ToShortDateString());
        genCell(tr, data.getPlayerId());
        genCell(tr, data.getAve(ResultItemPlayerGameBet.CARRY));
        genCell(tr, data.getMax(ResultItemPlayerGameBet.CARRY));
        genCell(tr, data.getMin(ResultItemPlayerGameBet.CARRY));

        genCell(tr, data.getAve(ResultItemPlayerGameBet.OUTLAY));
        genCell(tr, data.getMax(ResultItemPlayerGameBet.OUTLAY));
        genCell(tr, data.getMin(ResultItemPlayerGameBet.OUTLAY));

        genCell(tr, data.getAve(ResultItemPlayerGameBet.WIN));
        genCell(tr, data.getMax(ResultItemPlayerGameBet.WIN));
        genCell(tr, data.getMin(ResultItemPlayerGameBet.WIN));

        genCell(tr, data.getAve(ResultItemPlayerGameBet.LOSE));
        genCell(tr, data.getMax(ResultItemPlayerGameBet.LOSE));
        genCell(tr, data.getMin(ResultItemPlayerGameBet.LOSE));

        genCell(tr, data.getRw());

        if (data.m_playerId > 0)
        {
            genCell(tr, genLink(data));
        }
        else
        {
            genCell(tr, "");
        }
    }

    string genLink(ResultItemPlayerGameBet data)
    {
        URLParam uParam = new URLParam();
        uParam.m_text = "详情";
        uParam.m_key = "param";
        uParam.m_value = data.m_playerId.ToString();
        uParam.m_url = "/appaspx/operation/OperationMoneyQueryDetail.aspx";
        uParam.m_target = "_blank";
        uParam.addExParam("time", data.m_time.ToShortDateString());
        uParam.addExParam("property", 1);
        uParam.addExParam("filter", (int)PropertyReasonType.type_reason_single_round_balance);
        uParam.addExParam("gameId", data.m_gameId);
        return Tool.genHyperlink(uParam);
    }
}

//////////////////////////////////////////////////////////////////////////
public class TableNormalBase
{
    protected string[] m_headH;

    public TableCell genCell(TableRow tr, string text)
    {
        TableCell td = new TableCell();
        tr.Cells.Add(td);
        td.Text = text;
        return td;
    }

    public void genTable(GMUser user, Table table, OpRes res, object param)
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

        genHead(m_headH, tr);
        fillTableData(user, table, param);
    }

    void genHead(string[] head, TableRow tr)
    {
        TableCell td = null;
        int i = 0;
        for (; i < head.Length; i++)
        {
            td = new TableCell();
            tr.Cells.Add(td);
            td.Text = head[i];
        }
    }
    public virtual void fillTableData(GMUser user, Table table, object param) { }
}

public class TableRank : TableNormalBase
{
    private static string[] s_head = new string[] { "玩家id", "排行", "增长值", "累计充值", "最后登录" };

    public TableRank()
    {
        m_headH = s_head;
    }

    public override void fillTableData(GMUser user, Table table, object param)
    {
        var rankList = (List<ResultRankItem>)param;
        if (rankList == null)
            return;
        for (int i = 0; i < rankList.Count; i++)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            genDataRow(rankList[i], tr, i + 1);
        }
    }

    void genDataRow(ResultRankItem data, TableRow tr, int rank)
    {
        genCell(tr, data.m_playerId.ToString());
        genCell(tr, rank.ToString());
        genCell(tr, data.m_value.ToString());
        genCell(tr, data.m_rechargeTotal.ToString());
        genCell(tr, data.m_lastLogin.ToString());
    }
}



































