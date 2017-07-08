using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationMoneyQuery : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "时间", "玩家ID", "玩家昵称", "属性", "变化原因", "初始值", "结束值", "差值(结束值-初始值)", "所在游戏", "备注" };
        private string[] m_content = new string[s_head.Length];
        private PageGenMoney m_gen = new PageGenMoney(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
            if (!IsPostBack)
            {
                m_queryWay.Items.Add("通过玩家id查询");
                m_queryWay.Items.Add("通过账号查询");
                m_queryWay.Items.Add("通过昵称查询");

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

                for (int i = 0; i < (int)GameId.gameMax; i++)
                {
                    m_whichGame.Items.Add(StrName.s_gameName[i]);
                }
                m_whichGame.Items.Add("全部");

                if (m_gen.parse(Request))
                {
                    m_param.Text = m_gen.m_param;
                    m_queryWay.SelectedIndex = m_gen.m_way;
                    m_time.Text = m_gen.m_time;
                    m_filter.SelectedIndex = m_gen.m_filter;
                    m_property.SelectedIndex = m_gen.m_property;
                    m_range.Text = m_gen.m_range;
                    m_whichGame.SelectedIndex = m_gen.m_gameId;
                    onQuery(null, null);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMoneyQuery param = getParamMoneyQuery();
           
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeMoney, user);

            param.m_property = m_property.SelectedIndex;
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
            param.m_time = m_time.Text;
            // 过滤
            param.m_filter = m_filter.SelectedIndex;
            param.m_property = Convert.ToInt32(m_property.SelectedValue);
            param.m_range = m_range.Text;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_gameId = m_whichGame.SelectedIndex;
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

            List<MoneyItem> qresult = (List<MoneyItem>)mgr.getQueryResult(QueryType.queryTypeMoney);
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

                m_content[0] = qresult[i].m_genTime;
                m_content[1] = qresult[i].m_playerId.ToString();
                m_content[2] = qresult[i].m_nickName;
                m_content[3] = qresult[i].getPropertyName();
                m_content[4] = qresult[i].getActionName();
                m_content[5] = qresult[i].m_startValue.ToString();
                m_content[6] = qresult[i].m_endValue.ToString();
                m_content[7] = (qresult[i].m_deltaValue).ToString();
                m_content[8] = qresult[i].getGameName();
                m_content[9] = qresult[i].m_param;

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/operation/OperationMoneyQuery.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}