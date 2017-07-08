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
    "次日设备留存率"/*15*/, "3日设备留存率", "7日设备留存率" ,"30日设备留存率", "平均每个设备生成账号",
    "次日留存(付费)", "3日留存(付费)", "7日留存(付费)" ,"30日留存(付费)",};
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

            m_content[f++] = data.get2DayRemain(true);
            m_content[f++] = data.get3DayRemain(true);
            m_content[f++] = data.get7DayRemain(true);
            m_content[f++] = data.get30DayRemain(true);

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







































