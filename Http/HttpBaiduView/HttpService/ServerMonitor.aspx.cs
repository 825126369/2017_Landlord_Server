using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace SearchAccount
{
    // 服务器监控短信
    public partial class ServerMonitor : System.Web.UI.Page
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

                string idList = Request.QueryString["serverId"];
                if (idList == null)
                {
                    result = "idError";      
                    Response.Write(result);
                    Response.Flush();
                    return;
                }

                string msgInfo = Request.QueryString["msgInfo"];
                if (msgInfo == null)
                {
                    result = "msgError";      
                    Response.Write(result);
                    Response.Flush();
                    return;
                }
                string retstr = PhoneCom.sendMonitorInfo(phone, idList, idList);
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