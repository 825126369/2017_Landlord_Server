using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace WebManager.appaspx.account
{
    // 输赢报表
    public partial class AccountWinLoseReport : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "", "级别", "账号", "总押注", "总返还", "总盈利(系统)", "洗码量" ,
            "洗码比", "洗码佣金", "总金额", "代理占成", "代理交公司", "交公司投注", "交公司洗码量", "公司获利比"};

        private static string[] s_head1 = new string[] { "", "级别", "账号", "总押注", "总返还", "总盈利(系统)", "洗码量",
                        "会员洗码比", "会员洗码佣金", "会员总金额"};

        private string[] m_content = new string[s_head.Length];
        private SumItem m_sum = new SumItem();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                string acc = Request.QueryString["acc"];
                string detail = Request.QueryString["detail"];
                string time = Request.QueryString["time"];
                if (!string.IsNullOrEmpty(acc) &&
                    !string.IsNullOrEmpty(detail) &&
                    !string.IsNullOrEmpty(time))
                {
                    try
                    {
                        m_time.Text = time;
                        GMUser user = (GMUser)Session["user"];
                        onQuery(acc, Convert.ToInt32(detail), user);
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            m_time.Text = searchDateSpan.getDateTimeSpanLeft() + " - " + searchDateSpan.getDateTimeSpanRight();

            GMUser user = (GMUser)Session["user"];
            URLParam uparam = new URLParam();
            uparam.m_key = "acc";
            uparam.m_value = ItemHelp.getAccountSpecial(user);
            
            uparam.addExParam("time", m_time.Text.TrimStart(' ').TrimEnd(' '));
            uparam.addExParam("detail", 0); // 细节显示区是代理
            uparam.m_url = DefCC.ASPX_WIN_LOSE;
            user.getOpLevelMgr().addRootAcc(user.m_user, uparam);

            onQuery("", 0, user);

            m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(user.m_user);
        }

        protected void onQuery(string creator, int detailType, GMUser user)
        {
            m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(creator);

            ParamWinLose param = new ParamWinLose();
            param.m_creator = creator == "" ? ItemHelp.getAccountSpecial(user) : creator;
            param.m_detailType = detailType;
            param.m_time = m_time.Text;
            param.m_playerAcc = m_acc.Text;

            if (creator == "")
            {
                param.m_creatorIsSubAcc = ItemHelp.isSubAcc(user);
            }

            OpRes res = user.doStat(param, StatType.statTypeWinLose);

            if (param.isStatOnePlayer())
            {
                ViewPlayerWinLose v = new ViewPlayerWinLose();
                v.genTable(m_result, res, user);
            }
            else
            {
                genTableSumResult(m_result, res, user, param);
                if (res == OpRes.opres_success)
                {
                    if (param.isDetailSubAgent())
                    {
                        genTableDetailResult(m_detailResult, res, user, param, s_head);
                        m_info.InnerText = string.Format("{0}的直属下线", creator);
                    }
                    else
                    {
                        genTableDetailResult(m_detailResult, res, user, param, s_head1);
                        m_info.InnerText = string.Format("{0}的直属会员", creator);
                    }
                }
            }
        }

        public void genTableSumResult(Table table, OpRes res, GMUser user, ParamWinLose param)
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
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            URLParam uparam = new URLParam();
            uparam.m_url = DefCC.ASPX_WIN_LOSE;
            uparam.m_text = "明细";
            uparam.m_key = "acc"; uparam.m_value = param.m_creator;
            uparam.m_className = "cLevelLink";
            StatResultWinLose qresult = (StatResultWinLose)user.getStatResult(StatType.statTypeWinLose);

            for (i = 0; i < 2; i++)
            {
                tr = new TableRow();
                table.Rows.Add(tr);
                StatResultWinLoseItem item = qresult.getWinLoseItem(i);

                uparam.clearExParam(); uparam.addExParam("detail", i);
                uparam.addExParam("time", param.m_time.TrimStart(' ').TrimEnd(' '));
                m_content[0] = Tool.genHyperlink(uparam);
                m_content[1] = i == 0 ? "代理" : "会员";             // 级别，账号类型
                m_content[2] = item.m_acc;                          // 账号
                m_content[3] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_totalOutlay));      // 总押注
                m_content[4] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_totalIncome));       // 总返还
                m_content[5] = ItemHelp.toStrByComma(item.getWinLoseMoney()); // 总盈利
                m_content[6] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_totalWashCount));    // 洗码量

                m_content[7] = item.getWashRatio().ToString(); // 洗码比
                m_content[8] = ItemHelp.toStrByComma(item.getWashCommission()); // 洗码佣金
                m_content[9] = ItemHelp.toStrByComma(item.getTotalMoney()); // 总金额
                m_content[10] = item.getAgentRatio().ToString(); // 代理占成
                m_content[11] = ItemHelp.toStrByComma(item.getAgentHandInCompany()); // 代理交公司
                m_content[12] = ItemHelp.toStrByComma(item.getOutlayHandInCompany()); // 交公司投注
                m_content[13] = ItemHelp.toStrByComma(item.getWashCountHandInCompany()); // 交公司洗码量
                m_content[14] = item.getCompanyProfitRatioStr(); // 公司获利比

                for (int k = 0; k < s_head.Length; k++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[k];

                    if (k == 14)
                    {
                        setColor(td, td.Text);
                    }
                }

                m_sum.m_item.m_totalIncome += item.m_totalIncome;
                m_sum.m_item.m_totalOutlay += item.m_totalOutlay;
                m_sum.m_item.m_totalWashCount += item.m_totalWashCount;

                m_sum.m_washCommission += item.getWashCommission();
                m_sum.m_totalMoney += item.getTotalMoney();
                m_sum.m_agentHandInCompany += item.getAgentHandInCompany();
                m_sum.m_outlayHandInCompany += item.getOutlayHandInCompany();
                m_sum.m_washCountHandInCompany += item.getWashCountHandInCompany();
            }

            addFootSum(table, true);
        }

        public void genTableDetailResult(Table table, OpRes res, GMUser user, ParamWinLose param,
            string[] head)
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
            // 表头
            for (i = 0; i < head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            URLParam uparam = new URLParam();
            uparam.m_url = DefCC.ASPX_WIN_LOSE;
            uparam.m_text = "明细";
            uparam.m_key = "acc"; 
            uparam.m_className = "cLevelLink";
            StatResultWinLose qresult = (StatResultWinLose)user.getStatResult(StatType.statTypeWinLose);
            m_sum.reset();
            for (i = 0; i < qresult.m_detail.Count; i++)
            {
                tr = new TableRow();
                table.Rows.Add(tr);
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }

                StatResultWinLoseItem item = qresult.m_detail[i];
                uparam.m_value = item.m_acc;
                if (param.isDetailSubAgent())
                {
                    uparam.clearExParam(); uparam.addExParam("detail", param.m_detailType);
                    uparam.addExParam("time", param.m_time.TrimStart(' ').TrimEnd(' '));
                    m_content[0] = Tool.genHyperlink(uparam);
                }
                else
                {
                    m_content[0] = "";
                }

                m_content[1] = StrName.s_accountType[item.m_accType];          // 级别，账号类型
                m_content[2] = item.m_acc;                          // 账号
                m_content[3] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_totalOutlay));       // 总押注
                m_content[4] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_totalIncome));       // 总返还
                m_content[5] = ItemHelp.toStrByComma(item.getWinLoseMoney()); // 总盈利
                m_content[6] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(item.m_totalWashCount));    // 洗码量

                if (param.isDetailSubAgent())
                {
                    m_content[7] = item.getWashRatio().ToString(); // 洗码比
                    m_content[8] = ItemHelp.toStrByComma(item.getWashCommission()); // 洗码佣金
                    m_content[9] = ItemHelp.toStrByComma(item.getTotalMoney()); // 总金额
                    m_content[10] = item.getAgentRatio().ToString(); // 代理占成
                    m_content[11] = ItemHelp.toStrByComma(item.getAgentHandInCompany()); // 代理交公司
                    m_content[12] = ItemHelp.toStrByComma(item.getOutlayHandInCompany()); // 交公司投注
                    m_content[13] = ItemHelp.toStrByComma(item.getWashCountHandInCompany()); // 交公司洗码量
                    m_content[14] = item.getCompanyProfitRatioStr(); // 公司获利比
                }
                else
                {
                    m_content[7] = item.getWashRatio().ToString(); // 洗码比
                    m_content[8] = ItemHelp.toStrByComma(item.getWashCommission()); // 洗码佣金
                    m_content[9] = ItemHelp.toStrByComma(item.getTotalMoney()); // 总金额
                }

                for (int k = 0; k < head.Length; k++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[k];

                    if (k == 14)
                    {
                        setColor(td, td.Text);
                    }
                }

                m_sum.m_item.m_totalIncome += item.m_totalIncome;
                m_sum.m_item.m_totalOutlay += item.m_totalOutlay;
                m_sum.m_item.m_totalWashCount += item.m_totalWashCount;

                m_sum.m_washCommission += item.getWashCommission();
                m_sum.m_totalMoney += item.getTotalMoney();
                m_sum.m_agentHandInCompany += item.getAgentHandInCompany();
                m_sum.m_outlayHandInCompany += item.getOutlayHandInCompany();
                m_sum.m_washCountHandInCompany += item.getWashCountHandInCompany();
            }

            addFootSum(table, param.isDetailSubAgent());
        }

        // 增加页脚总和
        public void addFootSum(Table table, bool isDetailSubAgent)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总和";
            m_content[1] = "";          // 级别，账号类型
            m_content[2] = "";                          // 账号
            m_content[3] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(m_sum.m_item.m_totalOutlay));       // 总押注
            m_content[4] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(m_sum.m_item.m_totalIncome));       // 总返还

            // 总盈利
            m_content[5] =
                ItemHelp.toStrByComma(ItemHelp.showMoneyValue(m_sum.m_item.m_totalOutlay - m_sum.m_item.m_totalIncome));
            m_content[6] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(m_sum.m_item.m_totalWashCount));    // 洗码量
            m_content[7] = ""; // 洗码比

            string[] head = null;
            if (isDetailSubAgent)
            {
                m_content[8] = ItemHelp.toStrByComma(m_sum.m_washCommission); // 洗码佣金
                m_content[9] = ItemHelp.toStrByComma(m_sum.m_totalMoney); // 总金额
                m_content[10] = ""; // 代理占成
                m_content[11] = ItemHelp.toStrByComma(m_sum.m_agentHandInCompany); // 代理交公司
                m_content[12] = ItemHelp.toStrByComma(m_sum.m_outlayHandInCompany); // 交公司投注
                m_content[13] = ItemHelp.toStrByComma(m_sum.m_washCountHandInCompany); // 交公司洗码量
                m_content[14] = m_sum.getCompanyProfitRatioStr(); // 公司获利比

                head = s_head;
            }
            else
            {
                m_content[8] = ItemHelp.toStrByComma(m_sum.m_washCommission); // 洗码佣金
                m_content[9] = ItemHelp.toStrByComma(m_sum.m_totalMoney); // 总金额

                head = s_head1;
            }

            TableCell td = null;
            for (int k = 0; k < head.Length; k++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[k];

                if (k == 14)
                {
                    setColor(td, td.Text);
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
                td.ForeColor = Color.Blue;
            }
        }
    }

    public class SumItem
    {
        public StatResultWinLoseItem m_item = new StatResultWinLoseItem();
        public double m_washCommission;  // 洗码佣金
        public double m_totalMoney;      // 总金额
        public double m_agentHandInCompany; // 代理交公司
        public double m_outlayHandInCompany; // 交公司投注金额
        public double m_washCountHandInCompany; // 交公司洗码量

        // 返回公司获利比
        public double getCompanyProfitRatio()
        {
            return StatResultWinLoseItem.getCompanyProfitRatio(m_outlayHandInCompany, m_agentHandInCompany);
        }

        public string getCompanyProfitRatioStr()
        {
            return StatResultWinLoseItem.getCompanyProfitRatioStr(m_outlayHandInCompany, m_agentHandInCompany);
        }

        public void reset()
        {
            m_item.reset();
            m_washCommission = 0;
            m_totalMoney = 0;
            m_agentHandInCompany = 0;
            m_outlayHandInCompany = 0;
            m_washCountHandInCompany = 0;
        }
    }
}