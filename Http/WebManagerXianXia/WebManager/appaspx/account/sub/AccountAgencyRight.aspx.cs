using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account.sub
{
    // 代理账号的权限
    public partial class AccountAgencyRight : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
           
            if (!IsPostBack)
            {
                string acc = Request.QueryString["acc"];
                string right = Request.QueryString["right"];
                if (string.IsNullOrEmpty(acc))
                {
                    return;
                }

                m_right.Items.Add(new ListItem("能够创建下级代理", RIGHT.CREATE_AGENCY.ToString()));
                m_right.Items.Add(new ListItem("能够创建API账号", RIGHT.CREATE_API.ToString()));

                m_acc.Text = acc;
                if (RightMap.hasRight(RIGHT.CREATE_AGENCY, right))
                {
                    m_right.Items[0].Selected = true;
                }

                if (RightMap.hasRight(RIGHT.CREATE_API, right))
                {
                    m_right.Items[1].Selected = true;
                }
            }
        }

        protected void onModifyRight(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamModifyGmRight param = new ParamModifyGmRight();
            param.m_acc = m_acc.Text;

            string r = "";
            for (int i = 0; i < m_right.Items.Count; i++)
            {
                if (m_right.Items[i].Selected)
                {
                    r += m_right.Items[i].Value; // RightMap.getRightName(Convert.ToInt32(m_right.Items[i].Value));
                    r += ",";
                }
            }
            param.m_right = r;

            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpModiyGmRight);
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);
        }
    }
}