using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fish
{
    public partial class FishBoss : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "日期", "BOSS出现次数", "BOSS死亡次数", "金币消耗", "放出龙珠",
            "使用锁定", "使用急速", "使用散射", "使用激光", "攻击次数", "攻击人次", "BOSS放出金币","龙珠金币折合总计" ,"每BOSS系统盈利"};
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.FISH_BOSS_CONSUME, Session, Response);

            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    m_room.Items.Add(new ListItem(StrName.s_roomName[i], (i + 1).ToString()));
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamFishBoss param = new ParamFishBoss();
            param.m_roomId = Convert.ToInt32(m_room.SelectedValue);
            param.time = m_time.Text;

            OpRes res = user.doQuery(param, QueryType.queryTypeFishBoss);
            genTable(m_result, res, user);
        }

        private void genTable(Table table, OpRes res, GMUser user)
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

            int i = 0, k = 0;

            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ResultFishBoss> qresult =
                (List<ResultFishBoss>)user.getQueryResult(QueryType.queryTypeFishBoss);

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);
                ResultFishBoss item = qresult[i];

                int n = 0;
                m_content[n++] = item.m_date;
                m_content[n++] = item.m_bossCount.ToString();
                m_content[n++] = item.m_bossDieCount.ToString();
                m_content[n++] = item.m_consumeGold.ToString();
                m_content[n++] = item.m_dragonBall.ToString();
                m_content[n++] = item.m_lock.ToString();
                m_content[n++] = item.m_rapid.ToString();
                m_content[n++] = item.m_scattering.ToString();
                m_content[n++] = item.m_laser.ToString();

                m_content[n++] = item.m_bossHitCount.ToString();
                m_content[n++] = item.m_bossPersonTime.ToString();
                m_content[n++] = item.m_bossReleaseGold.ToString();

                m_content[n++] = item.getBossZheKouTotal().ToString();
                m_content[n++] = item.getEarnEachBoss();

                for (k = 0; k < s_head.Length; k++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[k];
                }
            }
        }
    }
}