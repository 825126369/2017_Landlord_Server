using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Text;

namespace SearchAccount
{
    // 发送验证码
    public partial class check_code : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            string result = "";
            if (Request.RequestType == "GET" || Request.RequestType == "POST")
            {
                string phone = Request.QueryString["phone"];
                if (phone == null)
                {
                    result = "phoneError";     
                    Response.Write(result);
                    Response.Flush();
                    return;
                }
                string code = Request.QueryString["code"];
                if (code == null)
                {
                    result = "codeError";     
                    Response.Write(result);
                    Response.Flush();
                    return;
                }
                string type = Request.QueryString["type"];
                if (type == null)
                {
                    result = "typeError";
                    Response.Write(result);
                    Response.Flush();
                    return;
                }
               
                SetUPInfo info = new SetUPInfo();

                string fmt = "";
                if (type == "0")
                {
                    fmt = WebConfigurationManager.AppSettings["bind_info"];
                }
                else if(type == "1")
                {
                    fmt = WebConfigurationManager.AppSettings["safeBoxInfo"];
                }
                else if (type == "2")
                {
                    fmt = WebConfigurationManager.AppSettings["relive_info"];
                }
                string retstr = PhoneCom.sendCheckInfo(phone, code, fmt);
                Response.Write(retstr);
            }
            else
            {
                Response.Write(result);
            }
            Response.Flush();
        }
    }
}
