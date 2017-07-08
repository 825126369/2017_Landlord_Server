using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceBenefits : System.Web.UI.Page
    {
        // 目标用户索引
        private int m_targetIndex = 0;
        // 发放类型索引
        private int m_typeIndex = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("service", Session, Response);
            if (IsPostBack)
            {
                m_targetIndex = m_target.SelectedIndex;
                m_typeIndex = m_type.SelectedIndex;
            }
            else
            {
                m_target.Items.Add("给指定用户");
                m_target.Items.Add("所有用户");
                m_target.Items.Add("VIP用户");

                m_type.Items.Add("金币");
                m_type.Items.Add("钻石");
                m_type.Items.Add("道具");
                m_type.Items.Add("礼券");

                m_platform.Items.Add("不限平台");
                XmlConfig xml = ResMgr.getInstance().getRes("platform.xml");
                /*for (int i = (int)PaymentType.e_pt_none + 1; i < (int)PaymentType.e_pt_max; i++)
                {
                    List<Dictionary<string, object>> ldata = xml.getTable(i.ToString());
                    if (ldata == null)
                    {
                        m_platform.Items.Add("###");
                    }
                    else
                    {
                        m_platform.Items.Add(Convert.ToString(ldata[0]["cha"]));
                    }
                }*/
            }
        }

        // 发放福利
        protected void onGrant(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
//             ParamGrant param = new ParamGrant();
//             param.m_target = (GrantTarget)m_targetIndex;
//             param.m_grantParam = m_param.Text;
//             param.m_playerId = m_playerId.Text;
//             param.m_level = m_level.Text;
//             param.m_platIndex = m_platform.SelectedIndex;
// 
//             OpRes res = OpRes.op_res_failed;//AddMoneyMgr.getInstance().doGrantBenefit(param, (GrantType)m_typeIndex, user);
//             if (m_targetIndex == (int)GrantTarget.grant_target_someone)
//             {
//                 if (m_typeIndex == (int)GrantType.gran_type_item && param.m_failedPlayer != "")
//                 {
//                     m_res.InnerHtml = string.Format("给玩家[{0}]发放道具[{1}]失败!", param.m_failedPlayer, param.m_failedItem);
//                     return;
//                 }
//                 else if (param.m_failedPlayer != "")
//                 {
//                     m_res.InnerHtml = string.Format("给玩家[{0}]发放福利时失败!", param.m_failedPlayer);
//                     return;
//                 }
//             }
            m_res.InnerHtml = "";// OpResMgr.getInstance().getResultString(res);
        }

        protected void onPaste(object sender, EventArgs e)
        {
//             DyOpCopyPaste paste = (DyOpCopyPaste)VIBMgr.getInstance().getDyOp(DyOpType.op_type_copy_paste);
//             m_playerId.Text  = paste.getContent((GMUser)Session["user"]);
        }
    }
}
