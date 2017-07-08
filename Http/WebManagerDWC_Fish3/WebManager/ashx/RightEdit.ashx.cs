using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class RightEdit : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.GM_TYPE_EDIT, context.Session, context.Response);
            int op = Convert.ToInt32(context.Request.Form["op"]);
            string str = "";
            switch (op)
            {
                case 0:
                    {
                        str = RightMgr.getInstance().getRightJson();
                    }
                    break;
                case 1:
                    {
                        string gmType = context.Request.Form["gmType"];
                        string rstr = context.Request.Form["rightStr"];
                        string rid = context.Request.Form["rightId"];
                        OpRes res = RightMgr.getInstance().modifyGmTypeRight(gmType, rid, rstr);
                        string resstr = OpResMgr.getInstance().getResultString(res);

                        Dictionary<string, object> ret = new Dictionary<string, object>();
                        ret.Add("gmType", gmType);
                        ret.Add("rstr", rstr);
                        ret.Add("rid", rid);
                        ret.Add("result", (int)res);
                        ret.Add("resultStr", resstr);
                        str = ItemHelp.genJsonStr(ret);
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