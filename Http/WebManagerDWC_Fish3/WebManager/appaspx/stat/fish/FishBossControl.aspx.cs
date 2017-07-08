using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fish
{
    public partial class FishBossControl : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "房间", "最大BOSS数量", "BOSS出现概率(万分值)", "选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_roomList = "";

        TableStatFishlordControl m_common = new TableStatFishlordControl();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.FISH_BOSS_CONTROL, Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                genTable(m_room, user);
            }
            else
            {
                m_roomList = Request["roomList"];
                if (m_roomList == null)
                {
                    m_roomList = "";
                }
            }
        }

        protected void onModify(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamFishBossControl param = new ParamFishBossControl();
            param.m_op = ParamFishBossControl.MODIFY_ROOM_BOSS;
            param.m_roomList = m_roomList;
            bool valid = true;
            if (!int.TryParse(m_maxCount.Text, out param.m_maxBossCount))
            {
                valid = false;
            }
            if (!int.TryParse(m_rand.Text, out param.m_createBossRand))
            {
                valid = false;
            }
            OpRes res = OpRes.op_res_failed;
            if (valid)
            {
                res = user.doDyop(param, DyOpType.opTypeFishBoss);
            }
            else
            {
                res = OpRes.op_res_reward_beyond_limit;
            }
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);
            genTable(m_room, user);
        }

        protected void genTable(Table table, GMUser user)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            DyOpFishBoss dy = (DyOpFishBoss)mgr.getDyOp(DyOpType.opTypeFishBoss);
            ParamFishBossControl param = new ParamFishBossControl();
            param.m_op = ParamFishBossControl.VIEW_ROOM_BOSS;
            dy.doDyop(param, user);
            List<ResultBossItem> qresult = (List<ResultBossItem>)dy.getResult();

            for (i = 0; i < qresult.Count; i++)
            {
                ResultBossItem item = qresult[i];
                m_content[0] = StrName.s_fishRoomName[item.m_roomId - 1];
                m_content[1] = item.m_maxBossCount.ToString();
                m_content[2] = item.m_createBossRand.ToString();
                m_content[3] = Tool.getCheckBoxHtml("roomList", item.m_roomId.ToString(), false);

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