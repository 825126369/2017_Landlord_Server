using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;

namespace HttpLogin
{
    public partial class SwitchLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string platform = Request.Params["platform"];
            if (string.IsNullOrEmpty(platform))
            {
                ReturnMsg("-1");//data error
                return;
            }

            string url = ConfigurationManager.AppSettings[platform];
            if (string.IsNullOrEmpty(url))
            {
                ReturnMsg("-1");//data error
                return;
            }

            string res = LoginCheck.checkIP(Request);
            if (res != "")
            {
                ReturnMsg(res);
                return;
            }
            Server.Transfer("Platform/"+url, true);
        }


        void ReturnMsg(string info, bool ret = false)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = ret;
            if (ret)
                data["data"] = info;
            else
                data["error"] = info;

            string jsondata = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsondata)));
        }
    }
}