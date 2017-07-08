using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Text;
using System.Security.Cryptography;

public class PhoneCom1 : PhoneBase
{
    // 生成6位随机数字验证码
    private string genIdentifyingCode(Random rand)
    {
        StringBuilder txtBuilder = new StringBuilder();
        int i = 0;
        for (i = 0; i < 6; i++)
        {
            txtBuilder.Append(rand.Next(0, 10));
        }
        return txtBuilder.ToString();
    }

    private string buildMsgInfo(string phone, string content)
    {
        StringBuilder strBuilder = new StringBuilder();
        content = HttpUtility.UrlDecode(content);
        string url = HttpUtility.UrlDecode(WebConfigurationManager.AppSettings["url"], Encoding.UTF8);
        string zh = HttpUtility.UrlDecode(WebConfigurationManager.AppSettings["account"], Encoding.UTF8);
        string pwd = HttpUtility.UrlDecode(WebConfigurationManager.AppSettings["pwd"], Encoding.UTF8);

        strBuilder.Append(url);
        strBuilder.Append("?");
        strBuilder.Append("account=");
        strBuilder.Append(zh);
        strBuilder.Append('&');

        strBuilder.Append("password=");
        strBuilder.Append(pwd);
        strBuilder.Append('&');

        strBuilder.Append("content=");
        strBuilder.Append(content);

        strBuilder.Append('&');

        strBuilder.Append("sendtime=");

        strBuilder.Append('&');
        strBuilder.Append("phonelist=");
        strBuilder.Append(phone);

        strBuilder.Append('&');
        strBuilder.Append("taskId=");

        DateTime now = DateTime.Now;
        string ss = genIdentifyingCode(new Random((int)DateTime.Now.Ticks));
        string str = string.Format("{0}_{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}_http_{7}",
            WebConfigurationManager.AppSettings["account"], now.Year, now.Month, now.Day,
            now.Hour, now.Minute, now.Second, ss);
        strBuilder.Append(str);

        return strBuilder.ToString();
    }

    public override string sendMsgInfo(string phone, string content)
    {
        string url = buildMsgInfo(phone, content);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retstr = Encoding.UTF8.GetString(ret);
            if (retstr[0] != '0') 
            {
                return "err_sendfailed";
            }
            return "err_success"; // 发送短信成功
        }
        return "err_sendfailed";  // 发送短信失败
    }
}
