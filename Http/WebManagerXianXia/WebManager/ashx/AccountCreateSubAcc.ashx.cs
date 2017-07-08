using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// AccountCreateSubAcc 的摘要说明
    /// </summary>
    public class AccountCreateSubAcc : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);
            GMUser user = (GMUser)context.Session["user"];
            ParamCreateGmAccount param = new ParamCreateGmAccount();
            param.m_accType = AccType.ACC_SUPER_ADMIN_SUB;
            param.m_pwd1 = context.Request.Form["pwd"];
            param.m_pwd2 = param.m_pwd1;
            param.m_aliasName = context.Request.Form["name"];

            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpCreateGmAccount);
            string ret = "";
            if (res == OpRes.opres_success)
            {
                ret = OpResMgr.getInstance().getResultString(OpRes.op_res_account_info, param.m_resultAcc, param.m_validatedCode);
            }
            else
            {
                ret = OpResMgr.getInstance().getResultString(res);
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(ret);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}