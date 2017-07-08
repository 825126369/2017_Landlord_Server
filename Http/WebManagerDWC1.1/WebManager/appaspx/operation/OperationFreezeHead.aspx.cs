using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationFreezeHead : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
        }

        protected void onFreeze(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamFreezeHeadInfo p = new ParamFreezeHeadInfo();
            p.m_playerId = m_playerId.Text;
            p.m_freezeDays = m_freezeDays.Text;
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeFreezeHead, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onViewHead(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);

            OpRes res = mgr.doQuery(m_playerId.Text, QueryType.queryTypePlayerHead, user);
            if (res != OpRes.opres_success)
            {
                m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            }
            else
            {
                m_res.InnerHtml = "";
            }
            string url = (string)mgr.getQueryResult(QueryType.queryTypePlayerHead);
            m_headImg.ImageUrl = url;
        }
    }
}