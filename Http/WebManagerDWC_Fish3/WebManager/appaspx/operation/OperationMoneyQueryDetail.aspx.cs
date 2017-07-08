using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationMoneyQueryDetail : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "时间", "玩家ID", "玩家昵称",  "属性", "变化原因",
                                                        "初始值", "结束值", "差值(结束值-初始值)", "投注", "返还","盈利", "获胜盈利", "所在游戏", "备注", "详情" };
        private string[] m_content = new string[s_head.Length];
        private PageGenMoney m_gen = new PageGenMoney(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_PLAYER_MONEY_CHANGE, Session, Response);
            if (!IsPostBack)
            {
                m_queryWay.Items.Add("通过玩家id查询");
                m_queryWay.Items.Add("通过账号查询");

                XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");
                if (xml != null)
                {
                    m_filter.Items.Add("全部");
                    int i = 1;
                    for (; i < (int)PropertyReasonType.type_max; i++)
                    {
                        m_filter.Items.Add(xml.getString(i.ToString(), ""));
                    }
                }

                m_property.Items.Add(new ListItem("全部", "0"));
                m_property.Items.Add(new ListItem("金币", ((int)PropertyType.property_type_gold).ToString()));
                m_property.Items.Add(new ListItem("钻石", ((int)PropertyType.property_type_ticket).ToString()));
                m_property.Items.Add(new ListItem("话费碎片", ((int)PropertyType.property_type_chip).ToString()));
                m_property.Items.Add(new ListItem("龙珠", ((int)PropertyType.property_type_dragon_ball).ToString()));

                m_whichGame.Items.Add(new ListItem("全部", ((int)GameId.gameMax).ToString()));

                for (int i = 0; i < StrName.s_gameName.Length; i++)
                {
                    m_whichGame.Items.Add(new ListItem(StrName.s_gameName[i], i.ToString()));
                }

                if (m_gen.parse(Request))
                {
                    m_param.Text = m_gen.m_param;
                    m_queryWay.SelectedIndex = m_gen.m_way;
                    __time__.Text = m_gen.m_time;
                    m_filter.SelectedIndex = m_gen.m_filter;
                   // m_range.Text = m_gen.m_range;

                    for (int i = 0; i < m_whichGame.Items.Count; i++)
                    {
                        if (m_whichGame.Items[i].Value == m_gen.m_gameId.ToString())
                        {
                            m_whichGame.Items[i].Selected = true;
                            break;
                        }
                    }

                    for (int i = 0; i < m_property.Items.Count; i++)
                    {
                        if (m_property.Items[i].Value == m_gen.m_property.ToString())
                        {
                            m_property.Items[i].Selected = true;
                            break;
                        }
                    }

                    onQuery(null, null);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMoneyQuery param = getParamMoneyQuery();
           
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeMoneyDetail, user);

           // param.m_gameId = m_whichGame.SelectedIndex;
           // param.m_property = m_property.SelectedIndex;
            genTable(m_result, res, param, user, mgr);
        }

        protected void onExport(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMoneyQuery param = getParamMoneyQuery();
            ExportMgr mgr = user.getSys<ExportMgr>(SysType.sysTypeExport);
            OpRes res = mgr.doExport(param, ExportType.exportTypeMoney, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        private ParamMoneyQuery getParamMoneyQuery()
        {
            ParamMoneyQuery param = new ParamMoneyQuery();
            param.m_way = (QueryWay)m_queryWay.SelectedIndex;
            param.m_param = m_param.Text;
            param.m_time = __time__.Text;
            // 过滤
            param.m_filter = m_filter.SelectedIndex;
            param.m_property = Convert.ToInt32(m_property.SelectedValue);
           // param.m_range = m_range.Text;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_gameId = Convert.ToInt32(m_whichGame.SelectedValue);
            return param;
        }

        private void genTable(Table table, OpRes res, ParamQuery param, GMUser user, QueryMgr mgr)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            m_res.InnerHtml = "";
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

            List<MoneyItemDetail> qresult = (List<MoneyItemDetail>)mgr.getQueryResult(QueryType.queryTypeMoneyDetail);
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

                int index = 0;
                m_content[index++] = qresult[i].m_genTime;
                m_content[index++] = qresult[i].m_playerId.ToString();

                m_content[index++] = qresult[i].m_nickName;

                m_content[index++] = qresult[i].getPropertyName();
                m_content[index++] = qresult[i].getActionName();

                m_content[index++] = qresult[i].m_startValue.ToString();
                m_content[index++] = qresult[i].m_endValue.ToString();
                m_content[index++] = qresult[i].m_deltaValue.ToString();

                m_content[index++] = qresult[i].m_outlay.ToString();
                m_content[index++] = qresult[i].m_income.ToString();
                m_content[index++] = (qresult[i].m_income - qresult[i].m_outlay).ToString();

                m_content[index++] = qresult[i].m_playerWinBet.ToString();

                m_content[index++] = qresult[i].getGameName();
                m_content[index++] = qresult[i].m_param;
                m_content[index++] = qresult[i].getExParam(i);

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/operation/OperationMoneyQueryDetail.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}