using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace HttpLogin.BaiduView
{
    public partial class ShowLogin : System.Web.UI.Page
    {
        class IPResult
        {
            public int code = 0;
            public IPResultData data = null;
        }

        class IPResultData
        {
            public string country = string.Empty;
            public string country_id = string.Empty;
            public string area = string.Empty;
            public string area_id = string.Empty;
            public string region = string.Empty;
            public string region_id = string.Empty;
            public string city = string.Empty;
            public string city_id = string.Empty;
            public string county = string.Empty;
            public string county_id = string.Empty;
            public string isp = string.Empty;
            public string isp_id = string.Empty;
            public string ip = string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string remoteIP = Common.Helper.getRemoteIP(Request);
            bool ret = canShow(remoteIP);
            StringBuilder sb = new StringBuilder();
            sb.Append("ret = {};");
            if (ret)
                sb.Append("ret.result = true;");
            else
                sb.Append("ret.result = false;");
            sb.Append("return ret;");
            Response.Write(sb.ToString());
        }

        public bool canShow(string ip)
        {
            string url = "http://ip.taobao.com/service/getIpInfo.php?ip=" + ip;
            var ret = HttpPost.Get(new Uri(url), false);
            if (ret != null)
            {
                string retstr = Encoding.UTF8.GetString(ret);

                try
                {
                    IPResult result = JsonHelper.ParseFromStr<IPResult>(retstr);
                    if (result.code == 0)
                    {
                        if (result.data.city_id == "110100" || result.data.city_id == "310100" || result.data.city_id == "440100" || result.data.city_id == "440300")
                            return false;
                        else
                            return true;
                    }
                }
                catch
                {

                }
            }
            return false;
        }
    }
}