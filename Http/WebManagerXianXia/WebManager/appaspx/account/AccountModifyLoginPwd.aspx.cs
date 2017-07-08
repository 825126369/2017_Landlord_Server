using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountModifyLoginPwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                init(user);

                string acc = Request.QueryString["acc"];
                if (!string.IsNullOrEmpty(acc))
                {
                    int index = 0;

                    if (acc != user.m_user)
                    {
                        foreach (ListItem item in m_accList.Items)
                        {
                            if (item.Value == acc)
                            {
                                break;
                            }
                            index++;
                        }
                    }
                    if (m_accList.Items.Count == index)
                    {
                        index = 0;
                    }
                    m_accList.SelectedIndex = index;
                }
            }
        }

        protected void onModify(object sender, EventArgs e)
        {
            ParamModifyLoginPwd param = new ParamModifyLoginPwd();
            param.m_acc = m_accList.SelectedValue;
            param.m_oriPwd = m_oriPwd.Text;

            if (opPwd.Checked)
            {
                param.m_pwd1 = m_pwd1.Text;
                param.m_pwd2 = m_pwd2.Text;
                param.m_op = 0;
            }
            else
            {
                param.m_pwd1 = m_verCode1.Text;
                param.m_pwd2 = m_verCode2.Text;
                param.m_op = 1;
            }
            
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeModifyLoginPwd, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        private void init(GMUser user)
        {
            m_accList.Items.Clear();
            m_accList.Items.Add(new ListItem("自己", ""));
            ItemHelp.fillDropDownList2(m_accList, user.m_user, user);
        }
    }
}