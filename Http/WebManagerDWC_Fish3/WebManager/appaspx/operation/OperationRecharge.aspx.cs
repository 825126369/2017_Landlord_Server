using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationRecharge : RefreshPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_BG_RECHARGE, Session, Response);
            if (!IsPostBack)
            {
                List<RechargeCFGData> allData = RechargeCFG.getInstance().getRechargeList();
                foreach (var data in allData)
                {
                    m_rechargeRMB.Items.Add(new ListItem(data.m_price.ToString() + HttpUtility.HtmlDecode("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;") + data.m_name,
                        data.m_payId.ToString()));
                }
            }
        }

        protected void onRecharge(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;
            
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);

            ParamRecharge param = new ParamRecharge();
            param.m_rtype = (int)RechargeType.rechargeRMB;
            param.m_playerId = m_playerId.Text;
            param.m_comment = m_comment.Text;

            ListItem selItem = m_rechargeRMB.Items[m_rechargeRMB.SelectedIndex];
            param.m_param = selItem.Value;
            OpRes res = mgr.doDyop(param, DyOpType.opTypeRecharge, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}