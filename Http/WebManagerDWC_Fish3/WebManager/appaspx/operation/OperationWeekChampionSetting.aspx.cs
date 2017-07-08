using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationWeekChampionSetting : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "玩家ID", "玩家昵称" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_WEEK_CHAMPION_SETTING, Session, Response);
            if (!IsPostBack)
            {
                onQuery();
            }
        }

        protected void onQuery()
        {
            GMUser user = (GMUser)Session["user"];
            ParamGrandPrixWeekChampion param = new ParamGrandPrixWeekChampion();
            param.m_op = ParamGrandPrixWeekChampion.CUR_SAFE_ACCOUNT;
            user.doDyop(param, DyOpType.opTypeWeekChampionControl);
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            DyOpWeekChampionControl dy = (DyOpWeekChampionControl)mgr.getDyOp(DyOpType.opTypeWeekChampionControl);
            genTable(m_result, OpRes.opres_success, dy);
        }

        private void genTable(Table table, OpRes res, DyOpWeekChampionControl mgr)
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

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ResultSafeAccItem> qresult = (List<ResultSafeAccItem>)mgr.getResult();

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                m_content[0] = qresult[i].m_playerId.ToString();
                m_content[1] = qresult[i].m_nickName;
                tr.ID = qresult[i].m_playerId.ToString();
                tr.ClientIDMode = ClientIDMode.Static;

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}