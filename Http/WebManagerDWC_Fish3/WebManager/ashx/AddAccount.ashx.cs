using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class AddAccount : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.GM_TYPE_EDIT, context.Session, context.Response);
            int op = Convert.ToInt32(context.Request.Form["op"]);
            string acc = context.Request.Form["acc"];
            string gmType = context.Request.Form["gmType"];

            string str = "";
            switch (op)
            {
                case 0:
                    {
                        OpRes res = AccountMgr.getInstance().updateAccountType(acc, gmType);
                        str = OpResMgr.getInstance().getResultString(res);
                    }
                    break;
                case 1:
                    {
                        OpRes res = AccountMgr.getInstance().delAccount(acc, null);
                        str = OpResMgr.getInstance().getResultString(res);
                    }
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(str);
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