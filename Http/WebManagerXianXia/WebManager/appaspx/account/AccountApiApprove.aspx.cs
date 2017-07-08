using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountApiApprove : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RIGHT.APPROVE_API, Session, Response);

            /*string index = Request.QueryString["index"];
            string op = Request.QueryString["op"];
            if (!string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(op))
            {
                doOp(index, op);
            }*/
            queryRecord();
        }

        protected void queryRecord()
        {
            GMUser user = (GMUser)Session["user"];
            OpRes res = user.doQuery(null, QueryType.queryTypeQueryApiApprove);

            ViewApiApprove view = new ViewApiApprove();
            view.genTable(m_result, res, user);
        }

        /*void doOp(string index, string opStr)
        {
            int idx = 0, op = 0;
            if(!int.TryParse(index, out idx))
            {
                return;
            }
            if (!int.TryParse(opStr, out op))
            {
                return;
            }

            GMUser user = (GMUser)Session["user"];
            ParamApiApprove param = new ParamApiApprove();
            param.m_index = idx;
            param.m_isPass = (op == 1);
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpApiApprove);
            if (res == OpRes.opres_success)
            {
                m_res.InnerHtml += "审批成功," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_account_info, param.m_resultAcc, param.m_validatedCode);
            }
        }*/
    }
}