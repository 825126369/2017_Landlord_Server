using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountApiLogViewer : System.Web.UI.Page
    {
        private static int[] s_ids = { LogType.LOG_TYPE_BLOCK_ACC, LogType.LOG_TYPE_RESET_PWD,
                                       LogType.LOG_TYPE_KICK_PLAYER, LogType.LOG_TYPE_CREATE_PLAYER };

        private CLogViewer m_view;

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            GMUser user = (GMUser)Session["user"];
            if (!user.isAPIAcc())
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }

            m_view = new CLogViewer(LogTable, m_page, m_foot, opType);
            if (!IsPostBack)
            {
                int count = OpLogMgr.getInstance().getOpInfoCount();
                opType.Items.Add(new ListItem("全部", "0"));
                for (int i = 0; i < s_ids.Length; i++)
                {
                    OpInfo info = OpLogMgr.getInstance().getOpInfo(s_ids[i]);
                    if (info != null)
                    {
                        opType.Items.Add(new ListItem(info.m_opName, info.m_opType.ToString()));
                    }
                }

                PageViewLog gen = m_view.getPageViewFoot();
                if (gen.parse(Request))
                {
                    opType.SelectedIndex = gen.m_opType;
                    m_time.Text = gen.m_time;
                    onSearchLog(null,null);
                }
            }
        }

        protected void onSearchLog(object sender, EventArgs e)
        {
            PageViewLog gen = m_view.getPageViewFoot();
            ParamQueryOpLog param = new ParamQueryOpLog();
            param.m_logType = int.Parse(opType.SelectedValue);
            param.m_time = m_time.Text;
            param.m_curPage = gen.curPage;
            param.m_countEachPage = gen.rowEachPage;

            GMUser user = (GMUser)Session["user"];
            m_view.genTable(user, param, @"/appaspx/account/AccountApiLogViewer.aspx");
        }
    }
}