using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.shcd
{
    public partial class ShcdResult : RefreshPageBase
    {
        private static string[] s_head = new string[] { "增加时间", "结果", "房间" };

        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.SHCD_RESULT_CONTROL, Session, Response);

            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_shcdArea.Length; i++)
                {
                    m_result.Items.Add(new ListItem(StrName.s_shcdArea[i], i.ToString()));
                }
                for (int i = 1; i < StrName.s_shcdRoomName.Length; i++)
                {
                    m_roomList.Items.Add(new ListItem(StrName.s_shcdRoomName[i], i.ToString()));
                }

                GMUser user = (GMUser)Session["user"];
                genTable(m_allResult, user);
            }
        }

        protected void onSetResult(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;

            GMUser user = (GMUser)Session["user"];
            ParamGameResultCrocodile param = new ParamGameResultCrocodile();
            param.m_gameId = (int)GameId.shcd;
            param.m_result = Convert.ToInt32(m_result.SelectedValue);
            param.m_roomId = Convert.ToInt32(m_roomList.SelectedValue);

            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpGameResult);
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);

            genTable(m_allResult, user);
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

            ParamGameResultControl param = new ParamGameResultControl();
            param.m_gameId = GameId.shcd;
            OpRes res = user.doQuery(param, QueryType.queryTypeGameResultControl);
            List<GameResultShcd> qresult
                = (List<GameResultShcd>)user.getQueryResult(param, QueryType.queryTypeGameResultControl);

            for (i = 0; i < qresult.Count; i++)
            {
                GameResultShcd r = qresult[i];
                m_content[0] = r.m_insertTime;
                m_content[1] = StrName.s_shcdArea[r.m_result];
                m_content[2] = StrName.s_shcdRoomName[r.m_roomId];

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}