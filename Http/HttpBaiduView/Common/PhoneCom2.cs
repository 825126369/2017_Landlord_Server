using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Text;
using System.Security.Cryptography;

public class PhoneCom2 : PhoneBase
{
    private string buildMsgInfo(string phone, string content)
    {
        StringBuilder strBuilder = new StringBuilder();
        string url = WebConfigurationManager.AppSettings["url2"];
        string zh = WebConfigurationManager.AppSettings["account2"];
        string pwd = WebConfigurationManager.AppSettings["pwd2"];
        //string id = WebConfigurationManager.AppSettings["sms_type"];
        strBuilder.Append(url);
        strBuilder.Append("?");
        strBuilder.Append("Uid=");
        strBuilder.Append(zh);
        strBuilder.Append('&');

        strBuilder.Append("Key=");
        strBuilder.Append(pwd);
        strBuilder.Append('&');

        strBuilder.Append("smsMob=");
        strBuilder.Append(phone);
        strBuilder.Append('&');

        strBuilder.Append("smsText=");
        strBuilder.Append(content);
        return strBuilder.ToString();
    }

    public override string sendMsgInfo(string phone, string content)
    {
        string url = buildMsgInfo(phone, content);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retstr = Encoding.UTF8.GetString(ret);
            if (ret[0] == '-') // 2号接口，返回 - 失败
            {
                return "err_sendfailed";
            }
            return "err_success"; // 返回0成功
        }
        return "err_sendfailed";  // 发送短信失败
    }
}
