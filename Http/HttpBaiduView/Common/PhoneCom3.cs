using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Text;
using System.Security.Cryptography;

public class PhoneCom3 : PhoneBase
{
    private string buildMsgInfo(string phone, string content)
    {
        StringBuilder strBuilder = new StringBuilder();
        content = HttpUtility.UrlEncode(content, Encoding.UTF8);
        string url = WebConfigurationManager.AppSettings["url3"];
        string zh = WebConfigurationManager.AppSettings["account3"];
        string pwd = WebConfigurationManager.AppSettings["pwd3"];
        strBuilder.Clear();
        strBuilder.Append(url);
        strBuilder.Append("/account/");
        strBuilder.Append(zh);
        strBuilder.Append("/pswd/");
        strBuilder.Append(pwd);
        strBuilder.Append("/mobile/");
        strBuilder.Append(phone);
        strBuilder.Append("/msg/");
        strBuilder.Append(content);

        return strBuilder.ToString();
    }

    public override string sendMsgInfo(string phone, string content)
    {
        string url = buildMsgInfo(phone, content);
        var ret = HttpPost.Get(new Uri(url), false);
        if (ret != null)
        {
            string retstr = Encoding.UTF8.GetString(ret);
            string[] ret1 = retstr.Split('\n');
            if (ret1.Length > 0)
            {
                string[] status = ret1[0].Split(',');
                if (status.Length > 1 && status[1] == "0")
                {
                    return "err_success";
                }
            }
            return "err_sendfailed"; // 发送短信失败
        }
        return "err_sendfailed";  // 发送短信失败
    }
}
