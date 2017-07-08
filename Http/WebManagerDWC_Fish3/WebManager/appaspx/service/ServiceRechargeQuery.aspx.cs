using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceRechargeQuery : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "充值时间", "玩家ID", "玩家账号", "客户端类型", "订单ID", "PayCode", "充值金额(元)", "process", "渠道" };
        private static string[] s_head1 = new string[] { "总计充值次数", "充值人数", "金额" };
        private static string[] s_head2 = new string[] { "订单ID", "出现次数" };
        private string[] m_content = new string[s_head.Length];
        private PageGenRecharge m_gen = new PageGenRecharge(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.SVR_RECHARGE_QUERY, Session, Response);
            m_res.InnerHtml = "";
            if (IsPostBack)
            {
                m_gen.m_way = m_queryWay.SelectedIndex;
            }
            else
            {
                m_queryWay.Items.Add("通过玩家id查询");
                m_queryWay.Items.Add("通过账号查询");
                m_queryWay.Items.Add("通过订单号查询");

                var allPlat = ResMgr.getInstance().m_plat;
                foreach (var d in allPlat)
                {
                    if (d.Value.m_engName != "default")
                    {
                        m_platform.Items.Add(new ListItem(d.Value.m_chaName, d.Value.m_engName));
                    }
                }
                m_platform.SelectedIndex = 0;

               /* m_gameServer.Items.Add("全部");
                Dictionary<string, DbServerInfo> db = ResMgr.getInstance().getAllDb();
                foreach (DbServerInfo info in db.Values)
                {
                    m_gameServer.Items.Add(new ListItem(info.m_serverName, info.m_serverId.ToString()));
                }*/

                m_rechargeResult.Items.Add("全部");
                m_rechargeResult.Items.Add("成功");
                m_rechargeResult.Items.Add("失败");

                m_channel.Items.Add(new ListItem("全部", ""));

                Dictionary<string, TdChannelInfo> cd = TdChannel.getInstance().getAllData();
                foreach (var item in cd.Values)
                {
                    m_channel.Items.Add(new ListItem(item.m_channelName, item.m_channelNo));
                }

                if (m_gen.parse(Request))
                {
                    m_queryWay.SelectedIndex = m_gen.m_way;
                    m_param.Text = m_gen.m_param;
                    m_time.Text = m_gen.m_time;

                    for (int i = 0; i < m_platform.Items.Count; i++)
                    {
                        if (m_platform.Items[i].Value == m_gen.m_platKey)
                        {
                            m_platform.Items[i].Selected = true;
                            break;
                        }
                    }

                    m_rechargeResult.SelectedIndex = m_gen.m_result;
                    m_range.Text = m_gen.m_range;

                    for (int i = 0; i < m_channel.Items.Count; i++)
                    {
                        if (m_channel.Items[i].Value == m_gen.m_channelNo)
                        {
                            m_channel.Items[i].Selected = true;
                            break;
                        }
                    }
                  //  m_gameServer.SelectedIndex = m_gen.m_serverIndex;
                    onQueryRecharge(null, null);
                }
            }
        }

        // 开始查询充值记录
        protected void onQueryRecharge(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQueryRecharge param = genParamQueryRecharge();
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeRecharge, user);
            genTable(m_result, res, param, user, mgr);
        }

        // 开始统计充值记录
        protected void onStatRecharge(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQueryRecharge param = genParamQueryRecharge();
            StatMgr mgr = user.getSys<StatMgr>(SysType.sysTypeStat);
            OpRes res = mgr.doStat(param, StatType.statTypeRecharge, user);
            genStatTable(m_result, res, mgr, user);
        }

        protected void onExport(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQueryRecharge param = genParamQueryRecharge();
            ExportMgr mgr = user.getSys<ExportMgr>(SysType.sysTypeExport);
            OpRes res = mgr.doExport(param, ExportType.exportTypeRecharge, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onSameOrder(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQueryRecharge param = genParamQueryRecharge();
            StatMgr mgr = user.getSys<StatMgr>(SysType.sysTypeStat);
            OpRes res = mgr.doStat(param, StatType.statTypeSameOrderId, user);
            genSameOrderTable(m_result, res, mgr, user);
        }

        private ParamQueryRecharge genParamQueryRecharge()
        {
            ParamQueryRecharge param = new ParamQueryRecharge();
            param.m_way = (QueryWay)m_queryWay.SelectedIndex;
            param.m_param = m_param.Text;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_time = m_time.Text;
            param.m_platKey = m_platform.SelectedValue;
            param.m_result = m_rechargeResult.SelectedIndex;
            param.m_range = m_range.Text;
            param.m_channelNo = m_channel.SelectedValue;
           /* param.m_gameServerIndex = m_gameServer.SelectedIndex;

            if (m_gameServer.SelectedIndex == 0)
            {
                param.m_serverId = -1;
            }
            else
            {
                ListItem selItem = m_gameServer.Items[m_gameServer.SelectedIndex];
                param.m_serverId = Convert.ToInt32(selItem.Value);
            }*/

            return param;
        }

        private void genTable(Table table, OpRes res, ParamQueryRecharge query_param, GMUser user, QueryMgr mgr)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            m_result.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            m_result.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            List<RechargeItem> qresult = (List<RechargeItem>)mgr.getQueryResult(QueryType.queryTypeRecharge);
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
                m_result.Rows.Add(tr);

                m_content[0] = qresult[i].m_time;
                m_content[1] = qresult[i].m_playerId.ToString();
                m_content[2] = qresult[i].m_account;
                m_content[3] = qresult[i].m_clientType;
                m_content[4] = qresult[i].m_orderId;
                m_content[5] = qresult[i].getPayName();
                m_content[6] = qresult[i].m_totalPrice.ToString();
                m_content[7] = qresult[i].m_process.ToString();
                m_content[8] = qresult[i].getChannelName();

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(query_param, @"/appaspx/service/ServiceRechargeQuery.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }

        // 生成统计表
        private void genStatTable(Table table, OpRes res, StatMgr mgr, GMUser user)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            m_result.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            m_result.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            int i = 0, j = 0;
            for (i = 0; i < s_head1.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head1[i];
            }

            for (i = 0; i < 1; i++)
            {
                tr = new TableRow();
                m_result.Rows.Add(tr);

                ResultStatRecharge rs = (ResultStatRecharge)mgr.getStatResult(StatType.statTypeRecharge);
                m_content[0] = rs.m_rechargeCount.ToString();
                m_content[1] = rs.m_rechargePersonNum.ToString();
                m_content[2] = rs.m_total.ToString();

                for (j = 0; j < s_head1.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }

        private void genSameOrderTable(Table table, OpRes res, StatMgr mgr, GMUser user)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            m_result.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            m_result.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            int i = 0, j = 0;
            for (i = 0; i < s_head2.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head2[i];
            }

            List<ResultSameOrderIdItem> result = (List<ResultSameOrderIdItem>)mgr.getStatResult(StatType.statTypeSameOrderId);

            for (i = 0; i < result.Count; i++)
            {
                tr = new TableRow();
                m_result.Rows.Add(tr);

                m_content[0] = result[i].m_orderId;
                m_content[1] = result[i].m_count.ToString();

                for (j = 0; j < s_head2.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}
