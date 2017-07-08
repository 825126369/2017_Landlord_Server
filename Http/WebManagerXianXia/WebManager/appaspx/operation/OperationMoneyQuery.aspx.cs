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
        private static string[] s_head = new string[] { "时间", "玩家ID", "玩家账号", "玩家昵称",  
                                                        "初始值", "结束值", "差值(结束值-初始值)", "投注", "返还","盈利", "获胜盈利", "所在游戏", "备注", "_id", "原因" };
        private string[] m_content = new string[s_head.Length];
        private PageGenMoney m_gen = new PageGenMoney(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            if (!IsPostBack)
            {
                m_queryWay.Items.Add("通过账号查询");
                m_queryWay.Items.Add("通过玩家id查询");

                XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");
                if (xml != null)
                {
                    m_filter.Items.Add(new ListItem("全部", "0"));
                    m_filter.Items.Add(new ListItem(xml.getString(36.ToString(), ""), "36"));
                    m_filter.Items.Add(new ListItem(xml.getString(21.ToString(), ""), "21"));
                    /*int i = 1;
                    for (; i < (int)PropertyReasonType.type_max; i++)
                    {
                        m_filter.Items.Add(xml.getString(i.ToString(), ""));
                    }*/
                }

               // m_property.Items.Add("全部");
               //m_property.Items.Add("金币");
               // m_property.Items.Add("礼券");

                m_whichGame.Items.Add(new ListItem("全部", ((int)GameId.gameMax).ToString()));

                for (int i = 0; i < StrName.s_gameList.Count; i++)
                {
                    GameInfo info = StrName.s_gameList[i];
                    m_whichGame.Items.Add(new ListItem(info.m_gameName, info.m_gameId.ToString()));
                }

               // m_whichGame.SelectedIndex = m_whichGame.Items.Count - 1;

                if (m_gen.parse(Request))
                {
                    m_param.Text = m_gen.m_param;
                    m_queryWay.SelectedIndex = m_gen.m_way;
                    __time__.Text = m_gen.m_time;
                    m_filter.SelectedIndex = m_gen.m_filter;
                   // m_property.SelectedIndex = m_gen.m_property;
                   // m_range.Text = m_gen.m_range;
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

            param.m_gameId = m_whichGame.SelectedIndex;
            param.m_filter = m_filter.SelectedIndex;
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
            //param.m_time = __time__.Text;
            param.m_time = searchDateSpan.getDateTimeSpanLeft() + " - " + searchDateSpan.getDateTimeSpanRight();
            // 过滤
            param.m_filter = Convert.ToInt32(m_filter.SelectedValue);
          //  param.m_property = m_property.SelectedIndex;
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
//                 if ((i & 1) == 0)
//                 {
//                     tr.CssClass = "alt";
//                 }
                table.Rows.Add(tr);

                m_content[0] = qresult[i].m_genTime;
                m_content[1] = qresult[i].m_playerId.ToString();
                m_content[2] = qresult[i].m_acc;
                m_content[3] = qresult[i].m_nickName;
                m_content[4] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(qresult[i].m_startValue));
                m_content[5] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(qresult[i].m_endValue));
                m_content[6] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(qresult[i].m_deltaValue));

                m_content[7] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(qresult[i].m_outlay));
                m_content[8] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(qresult[i].m_income));
                m_content[9] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(qresult[i].m_income - qresult[i].m_outlay));
                
                m_content[10] = ItemHelp.toStrByComma(ItemHelp.showMoneyValue(qresult[i].m_playerWinBet));

                m_content[11] = qresult[i].getGameName();
                m_content[12] = qresult[i].getExParam(i);
                m_content[13] = qresult[i].m_id;

                if (qresult[i].m_actionType > 0)
                {
                    m_content[14] = qresult[i].getActionName();
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
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/operation/OperationMoneyQuery.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}