using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Text;

namespace WebManager.appaspx.operation
{
    public partial class OperationMaintenanceNotice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                ParamMaintenance p = new ParamMaintenance();
                p.m_opType = 0;
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                OpRes res = mgr.doDyop(p, DyOpType.opTypeMaintenance, user);
                if (res == OpRes.opres_success)
                {
                    DyOpMaintenance dm = (DyOpMaintenance)mgr.getDyOp(DyOpType.opTypeMaintenance);
                    ResultMaintenance result = dm.getResult();
                    switch (result.m_curState)
                    {
                        case 0:
                            {
                                m_curState.InnerText = "运行中";
                            }
                            break;
                        case 1:
                            {
                                m_curState.InnerText = "维护中";
                            }
                            break;
                    }
                    m_info.Text = result.m_info;
                }
                else
                {
                    m_curState.InnerText = "未知，出错";
                }
            }
        }

        protected void onOk(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMaintenance p = new ParamMaintenance();
            p.m_opType = 1;
            p.m_content = m_info.Text;
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeMaintenance, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onCancel(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMaintenance p = new ParamMaintenance();
            p.m_opType = 2;
            p.m_content = m_info.Text;
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeMaintenance, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}