using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationGenGiftCode : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "礼包ID", "内容", "截止日期", "平台", "生成个数" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
            GMUser user = (GMUser)Session["user"];
            if (IsPostBack)
            {
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                OpRes res = mgr.doDyop(m_codeInfo.Value, DyOpType.opTypeGiftCode, user);
                if (res == OpRes.opres_success)
                {
                    m_res.InnerHtml = "已提交生成，稍后可以查询";
                }
                else
                {
                    m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
                }
            }
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
            param.m_countEachPage = 100;
            param.m_way = QueryWay.by_way1; // 未过期

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
                m_content[1] = qresult[i].getGiftList();
                m_content[2] = qresult[i].m_deadTime.ToString();
                m_content[4] = Tool.getTextBoxHtml("CodeNum" + i, "0");
                m_clientInfo.Value += "," + m_content[0];

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    if (j == 3)
                    {
                        td.Controls.Add(createPlatList("Plat" + i));
                    }
                    else
                    {
                        td.Text = m_content[j];
                    }
                    tr.Cells.Add(td);
                }
            }
        }

        private DropDownList createPlatList(string id)
        {
            DropDownList d = new DropDownList();
            d.ID = id;
            d.Items.Add("不限平台");
            for (int i = (int)PaymentType.e_pt_none + 1; i < (int)PaymentType.e_pt_max; i++)
            {
                PlatformInfo data = ResMgr.getInstance().getPlatformInfo(i);
                if (data == null)
                {
                    d.Items.Add("###");
                }
                else
                {
                    d.Items.Add(data.m_chaName);
                }
            }
            return d;
        }
    }
}