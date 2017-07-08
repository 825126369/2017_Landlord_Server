using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountModifyHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                string home = Request["home"];
                string acc = Request["acc"];
                if (!string.IsNullOrEmpty(acc))
                {
                    m_acc.Text = acc;
                }
                if (!string.IsNullOrEmpty(home))
                {
                    m_home.Text = home;
                }
            }
        }

        protected void onModifyHome(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            SqlUpdateGenerator gen = new SqlUpdateGenerator();
            gen.addField("home", m_home.Text, FieldType.TypeString);
            string sql = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", m_acc.Text));

            int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            if (count > 0)
            {
                m_res.InnerText = OpResMgr.getInstance().getResultString(OpRes.opres_success);
            }
            else
            {
                m_res.InnerText = OpResMgr.getInstance().getResultString(OpRes.op_res_db_failed);
            }
        }
    }
}