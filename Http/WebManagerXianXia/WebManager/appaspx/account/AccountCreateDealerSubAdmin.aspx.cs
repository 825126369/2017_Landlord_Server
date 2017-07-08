using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 总代理或下级代理创建下一层级的账号
    public partial class AccountCreateDealerSubAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                string isCreateSub = Request.QueryString["isCreateSub"];
                if (!string.IsNullOrEmpty(isCreateSub)) // 仅创建子账号，其它页面转到这里
                {
                    m_type.Items.Add(new ListItem("子账号", AccType.ACC_AGENCY_SUB.ToString()));
                }
                else if (user.m_accType == AccType.ACC_GENERAL_AGENCY)
                {
                    m_type.Items.Add(new ListItem("下级代理", AccType.ACC_AGENCY.ToString()));
                    m_type.Items.Add(new ListItem("API账号", AccType.ACC_API.ToString()));
                    m_type.Items.Add(new ListItem("子账号", AccType.ACC_AGENCY_SUB.ToString()));
                }
                else if (user.m_accType == AccType.ACC_AGENCY)
                {
                    if (RightMap.hasRight(RIGHT.CREATE_AGENCY, user.m_right))
                    {
                        m_type.Items.Add(new ListItem("下级代理", AccType.ACC_AGENCY.ToString()));
                    }
                    else
                    {
                       // m_type.Items.Add(new ListItem("###", AccType.ACC_AGENCY.ToString()));
                    }

                    if (RightMap.hasRight(RIGHT.CREATE_API, user.m_right))
                    {
                        m_type.Items.Add(new ListItem("API账号", AccType.ACC_API.ToString()));
                    }
                    else
                    {
                      //  m_type.Items.Add(new ListItem("###", AccType.ACC_API.ToString()));
                    }
                    m_type.Items.Add(new ListItem("子账号", AccType.ACC_AGENCY_SUB.ToString()));
                }
                else
                {
                    Server.Transfer(DefCC.ASPX_EMPTY);
                }

                m_right.Items.Add(new ListItem("能够创建下级代理", RIGHT.CREATE_AGENCY.ToString()));
                m_right.Items.Add(new ListItem("能够创建API账号", RIGHT.CREATE_API.ToString()));

              //  m_prefix.Text = "";
                //m_prefix1.Text = user.m_generalAgency;

                m_agentRatio.Text = (user.m_agentRatio * 100).ToString();
                m_washRatio.Text = (user.m_washRatio * 100).ToString();

                RangeValidator2.MaximumValue = (user.m_agentRatio * 100).ToString();
                RangeValidator1.MaximumValue = (user.m_washRatio * 100).ToString();
            }
        }

        protected void onCreateAccount(object sender, EventArgs e)
        {
            ParamCreateGmAccount param = new ParamCreateGmAccount();
            param.m_accType = Convert.ToInt32(m_type.SelectedValue);
            param.m_pwd1 = m_pwd1.Text;
            param.m_pwd2 = m_pwd2.Text;
            param.m_agentRatio = m_agentRatio.Text;
            param.m_washRatio = m_washRatio.Text;

            if (param.m_accType == AccType.ACC_AGENCY) // 下级代理
            {
                param.m_accName = m_accName.Text;

                string r = "";
                for (int i = 0; i < m_right.Items.Count; i++)
                {
                    if (m_right.Items[i].Selected)
                    {
                        r += m_right.Items[i].Value;// RightMap.getRightName(Convert.ToInt32(m_right.Items[i].Value));
                        r += ",";
                    }
                }
                param.m_right = r;
            }
            else if (param.m_accType == AccType.ACC_API) // API账号
            {
                param.m_accName = m_accName1.Text;
                param.m_apiPrefix = m_apiPrefix.Text;
            }
            else // 子账号
            {
                param.m_accName = m_accName.Text;
            }
            param.m_aliasName = m_aliasName.Text;

            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeDyOpCreateGmAccount, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);

            if (res == OpRes.opres_success)
            {
                m_res.InnerHtml += "," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_account_info, param.m_resultAcc, param.m_validatedCode);
            }
        }
    }
}