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
            RightMgr.getInstance().opCheck("addAccount", context.Session, context.Response);

            string retStr = "";
            int op = Convert.ToInt32(context.Request.Form["op"]);
            switch (op)
            {
                case 1:
                    {
                        string acc = context.Request.Form["acc"];
                        string viewChannel = context.Request.Form["viewChannel"];
                        bool res = AccountMgr.getInstance().updateViewChannel(acc, viewChannel);
                        retStr = OpResMgr.getInstance().getResultString(res ? OpRes.opres_success : OpRes.op_res_failed);
                    }
                    break;
                case 2:
                    {
                        string acc = context.Request.Form["acc"];
                        string gmType = context.Request.Form["type"];
                        bool res = AccountMgr.getInstance().updateGmType(acc, gmType);
                        retStr = OpResMgr.getInstance().getResultString(res ? OpRes.opres_success : OpRes.op_res_failed);
                    }
                    break;
                case 3:
                    {
                        string acc = context.Request.Form["acc"];
                        OpRes res = AccountMgr.getInstance().delAccount(acc, null);
                        retStr = OpResMgr.getInstance().getResultString(res);
                    }
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(retStr);
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