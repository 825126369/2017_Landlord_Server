using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class GmTypeEdit : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.GM_TYPE_EDIT, context.Session, context.Response);
            ParamGmTypeEdit param = new ParamGmTypeEdit();
            param.m_op = Convert.ToInt32(context.Request.Form["op"]);
            param.m_param = context.Request.Form["param"];
            param.m_newValue = context.Request.Form["newValue"];

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeGmTypeEdit);

            string str = "";
            switch (param.m_op)
            {
                case DefCC.OP_ADD:
                    {
                        Dictionary<string, object> retData = new Dictionary<string, object>();
                        retData.Add("result", (int)res);
                        retData.Add("acc", param.m_param);
                        retData.Add("id", param.m_newValue);
                        str = ItemHelp.genJsonStr(retData);
                    }
                    break;
                case DefCC.OP_MODIFY:
                    {
                        Dictionary<string, object> retData = new Dictionary<string, object>();
                        retData.Add("result", (int)res);
                        retData.Add("id", param.m_param);
                        retData.Add("newValue", param.m_newValue);
                        str = ItemHelp.genJsonStr(retData);
                    }
                    break;
                case DefCC.OP_REMOVE:
                    {
                        Dictionary<string, object> retData = new Dictionary<string, object>();
                        retData.Add("result", (int)res);
                        retData.Add("id", param.m_param);
                        str = ItemHelp.genJsonStr(retData);
                    }
                    break;
                case DefCC.OP_VIEW:
                    {
                        List<Dictionary<string, object>> gmList = (List<Dictionary<string, object>>)user.getDyopResult(DyOpType.opTypeGmTypeEdit);
                        Dictionary<string, object> retData = new Dictionary<string, object>();
                        retData.Add("gmList", gmList);
                        
                        str = ItemHelp.genJsonStr(retData);
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