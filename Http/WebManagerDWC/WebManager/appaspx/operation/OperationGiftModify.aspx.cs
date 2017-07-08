using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationGiftModify : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "礼包ID", "内容", "截止日期" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);

            if (!IsPostBack)
            {
                m_queryWay.Items.Add("已过期");
                m_queryWay.Items.Add("未过期");
            }
            else
            {
                if (m_modifyInfo.Value != "")
                {
                    GMUser user = (GMUser)Session["user"];
                    DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);

                    ParamGift param = new ParamGift();
                    param.m_isAdd = false;
                    param.m_itemList = m_modifyInfo.Value;
                    OpRes res = mgr.doDyop(param, DyOpType.opTypeGift, user);
                    if (param.m_result != "")
                    {
                        m_res.InnerHtml = string.Format("礼包[{0}]更新失败!", param.m_result);
                    }
                    else
                    {
                        m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
                    }
                    genTable(m_result, user);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            genTable(m_result, user);
        }

        private void genTable(Table table, GMUser user)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            ParamQuery param = new ParamQuery();
            param.m_curPage = 1;
            param.m_countEachPage = 1000;
            param.m_way = (QueryWay)m_queryWay.SelectedIndex;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeGift, user);
            List<GiftItem> qresult = (List<GiftItem>)mgr.getQueryResult(QueryType.queryTypeGift);

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            m_clientInfo.Value = "";
            m_modifyInfo.Value = "";

            if (qresult.Count > 0)
            {
                m_clientInfo.Value += qresult.Count;
            }

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                m_content[0] = qresult[i].m_giftId.ToString();
                //m_content[1] = Tool.getTextBoxHtml("Content" + i, qresult[i].getSrcGiftList());
                m_content[2] = Tool.getTextBoxHtml("DeadTime" + i, qresult[i].m_deadTime.ToString("yyyy/MM/dd"));
                m_clientInfo.Value += "," + m_content[0];

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    if (j == 1)
                    {
                        td.Width = Unit.Pixel(500);
                        td.Controls.Add(createTxt("Content" + i, qresult[i].getSrcGiftList(), td.Width));
                    }
                    else
                    {
                        td.Text = m_content[j];
                    }
                    tr.Cells.Add(td);
                }
            }
        }

        private TextBox createTxt(string id, string content, Unit width)
        {
            TextBox txt = new TextBox();
            txt.ID = id;
            txt.Text = content;
            txt.Width = width;
            return txt;
        }
    }
}
