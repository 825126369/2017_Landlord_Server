using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceMail : RefreshPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("service", Session, Response);

            if (!IsPostBack)
            {
                m_target.Items.Add("给指定玩家");
                m_target.Items.Add("全服");
            }
        }

        protected void onSendMail(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;
            
            GMUser user = (GMUser)Session["user"];
            ParamSendMail param = new ParamSendMail();
            param.m_title = m_title.Text;
            param.m_sender = m_sender.Text;
            param.m_content = m_content.Text;
            param.m_validDay = m_validDay.Text;
            param.m_toPlayer = m_toPlayer.Text;
            param.m_itemList = m_itemList.Text;
            param.m_target = m_target.SelectedIndex;
            param.m_isCheck = m_chk.Checked;
            param.m_condLogoutTime = m_logOutTime.Text;
            param.m_condVipLevel = m_vipLevel.Text;
            param.m_comment = m_comment.Text;

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeSendMail, user);
            if (res == OpRes.op_res_item_not_exist)
            {
                m_res.InnerHtml = string.Format("道具[{0}]不存在，请检测!", param.m_result);
            }
            else if (res == OpRes.op_res_player_not_exist)
            {
                m_res.InnerHtml = string.Format("玩家[{0}]不存在，请检测!", param.m_result);
            }
            else
            {
                m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            }
        }
    }
}
