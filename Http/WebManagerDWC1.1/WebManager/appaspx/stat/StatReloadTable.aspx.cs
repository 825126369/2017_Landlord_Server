using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatReloadTable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                m_table.Items.Add("鱼表");
            }
        }

        protected void onLoad(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(m_table.SelectedIndex, DyOpType.opTypeReLoadTable, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}