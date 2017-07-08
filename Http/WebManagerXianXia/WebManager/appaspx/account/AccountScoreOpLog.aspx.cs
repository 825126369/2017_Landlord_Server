using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 上分下分记录
    public partial class AccountScoreOpLog : System.Web.UI.Page
    {
        ViewScoreOpRecord m_view = new ViewScoreOpRecord();
        private PageGmScoreLog m_gen = new PageGmScoreLog(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                m_orderResult.Items.Add(new ListItem("全部", "-1"));
                m_orderResult.Items.Add(new ListItem("成功", PlayerReqOrderState.STATE_FINISH.ToString()));
                m_orderResult.Items.Add(new ListItem("失败", PlayerReqOrderState.STATE_FAILED.ToString()));

                if (m_gen.parse(Request))
                {
                    m_time.Text = m_gen.m_time;
                    m_opAcc.Text = m_gen.m_opAcc;
                    m_dstAcc.Text = m_gen.m_dstAcc;
                    
                    for (int i = 0; i < m_orderResult.Items.Count; i++)
                    {
                        if (m_orderResult.Items[i].Value == m_gen.m_orderState.ToString())
                        {
                            m_orderResult.SelectedIndex = i;
                            break;
                        }
                    }
                    onQueryRecord(null, null);
                }
            }
        }

        protected void onQueryRecord(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamScoreOpRecord param = getParam();
            
            m_time.Text = searchDateSpan.getDateTimeSpanLeft() + " - " + searchDateSpan.getDateTimeSpanRight();

            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";

            OpRes res = user.doQuery(param, QueryType.queryTypeQueryScoreOpRecord);
            m_view.genTable(m_result, res, user);

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/account/AccountScoreOpLog.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }

        // 删除记录
        protected void onDelRecord(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamDelDataScoreLog param = new ParamDelDataScoreLog();
            param.m_tableName = TableName.GM_SCORE;
            param.m_timeRange = m_time.Text;
            param.m_opAcc = m_opAcc.Text;
            param.m_playerAcc = m_dstAcc.Text;

            OpRes res = user.doDyop(param, DyOpType.opTypeDelData);
            setOpRes(res);
        }

        protected void onExport(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamScoreOpRecord param = getParam();
            ExportMgr mgr = user.getSys<ExportMgr>(SysType.sysTypeExport);
            OpRes res = mgr.doExport(param, ExportType.exportTypeScoreOpLog, user);
            setOpRes(res);
        }

        ParamScoreOpRecord getParam()
        {
            ParamScoreOpRecord param = new ParamScoreOpRecord();
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_curPage = m_gen.curPage;
            param.m_time = m_time.Text;
            param.m_opAcc = m_opAcc.Text;
            param.m_dstAcc = m_dstAcc.Text;
            param.m_orderState = Convert.ToInt32(m_orderResult.SelectedValue);
            return param;
        }

        void setOpRes(OpRes res)
        {
            m_result.Rows.Clear();
            TableRow tr = new TableRow();
            m_result.Rows.Add(tr);
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = OpResMgr.getInstance().getResultString(res);
        }
    }
}