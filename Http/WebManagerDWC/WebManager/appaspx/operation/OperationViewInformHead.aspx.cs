using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationViewInformHead : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "举报时间", "举报者ID", "被举报玩家ID", "查看头像", "头像", "选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_playerList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                genTable(m_result, user);
            }
            else
            {
                m_playerList = Request["playerList"];
            }
        }

        protected void onDelPlayer(object sender, EventArgs e)
        {
            ParamInformHead param = new ParamInformHead();
            param.m_playerList = m_playerList;
            param.m_opType = 1;
            GMUser user = (GMUser)Session["user"];
            user.doQuery(param, QueryType.queryTypeInformHead);
            genTable(m_result, user);
        }

        protected void genTable(Table table, GMUser user)
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

            ParamInformHead param = new ParamInformHead();
            OpRes res = user.doQuery(param, QueryType.queryTypeInformHead);
            List<ResultnformHeadItem> qresult
                = (List<ResultnformHeadItem>)user.getQueryResult(QueryType.queryTypeInformHead);
            
            URLParam uparam = new URLParam();
            uparam.m_text = "查看";
            uparam.m_url = @"/appaspx/operation/OperationFreezeHead.aspx";
            uparam.m_key = "playerId";
            for (i = 0; i < qresult.Count; i++)
            {
                ResultnformHeadItem item = qresult[i];
                m_content[0] = item.m_time;
                m_content[1] = item.m_informerId.ToString();
                m_content[2] = item.m_dstPlayerId.ToString();

                uparam.m_value = item.m_dstPlayerId.ToString();
                m_content[3] = Tool.genHyperlink(uparam);
                
                m_content[5] = Tool.getCheckBoxHtml("playerList", item.m_dstPlayerId.ToString(), false);

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    if (j == 4)
                    {
                        Image img = new Image();
                        img.Width = 72;
                        img.Height = 72;
                        img.ImageUrl = item.m_headURL;
                        td.Controls.Add(img);
                    }
                    else
                    {
                        td.Text = m_content[j];
                    }
                }
            }
        }
    }
}