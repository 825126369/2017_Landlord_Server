using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Text;
using System.Security.Cryptography;

// 组装信息内容
public class SetUPInfo
{
    private StringBuilder m_urlBuilder = new StringBuilder();
    private StringBuilder m_textBuilder = new StringBuilder();

    // 组装验证码接口
    public string setUpMsgInfoCheckInfo(string phone, string code, string fmt)
    {
        string use = WebConfigurationManager.AppSettings["use"];
        string content = "";
        if (use == "1")
        {
            content = HttpUtility.UrlDecode(string.Format(fmt, code), Encoding.UTF8);
            return setUpMsgInfo_New(phone, content);
        }

        content = string.Format(fmt, code);
        return setUpMsgInfo(phone, content);
    }

    // 组装账号找回接口
    public string setUpMsgInfoSearchAccount(string phone, string account)
    {
        string use = WebConfigurationManager.AppSettings["use"];
        string content = "";
        if (use == "1")
        {
            content = HttpUtility.UrlDecode(string.Format(WebConfigurationManager.AppSettings["content"], account));
            return setUpMsgInfo_New(phone, content);
        }

        content = string.Format(WebConfigurationManager.AppSettings["content"], account);
        return setUpMsgInfo(phone, content);
    }

    public string setUpMsgInfoPhoneCode(string phone, string pwdcode)
    {
        string use = WebConfigurationManager.AppSettings["use"];
        string content = "";
        if (use == "1")
        {
            content = HttpUtility.UrlDecode(string.Format(WebConfigurationManager.AppSettings["pwdcode"], pwdcode));
            return setUpMsgInfo_New(phone, content);
        }

        content = string.Format(WebConfigurationManager.AppSettings["pwdcode"], pwdcode);
        return setUpMsgInfo(phone, content);
    }

    // 组装服务器监控短信
    public string setUpServerMonitorInfo(string phone, string serverIdList, string msgInfo)
    {
        string use = WebConfigurationManager.AppSettings["use"];
        string content = "";
        if (use == "1")
        {
            content = HttpUtility.UrlDecode(string.Format(WebConfigurationManager.AppSettings[msgInfo], serverIdList));
            return setUpMsgInfo_New(phone, content);
        }

        content = string.Format(WebConfigurationManager.AppSettings[msgInfo], serverIdList);
        return setUpMsgInfo(phone, content);
    }

    // 适配返回给服务端的值
    public string adapterRetValue(string ret)
    {
        string use = WebConfigurationManager.AppSettings["use"];
        if (use == "1")
        {
            if (ret[0] != '0') // 1号接口，返回失败
            {
                return "1";
            }
            return "0"; // 返回0成功
        }

        if (ret[0] == '-') // 2号接口，返回 - 失败
        {
            return "1";
        }
        return "0"; // 返回0成功
    }

    // 组装发送内容
    private string setUpMsgInfo_New(string phone, string content)
    {
        string url = HttpUtility.UrlDecode(WebConfigurationManager.AppSettings["url"], Encoding.UTF8);
        string zh = HttpUtility.UrlDecode(WebConfigurationManager.AppSettings["account"], Encoding.UTF8);
        string pwd = HttpUtility.UrlDecode(WebConfigurationManager.AppSettings["pwd"], Encoding.UTF8);

        m_urlBuilder.Remove(0, m_urlBuilder.Length);
        m_urlBuilder.Append(url);
        m_urlBuilder.Append("?");
        m_urlBuilder.Append("account=");
        m_urlBuilder.Append(zh);
        m_urlBuilder.Append('&');

        m_urlBuilder.Append("password=");
        m_urlBuilder.Append(pwd);
        m_urlBuilder.Append('&');

        m_urlBuilder.Append("content=");
        m_urlBuilder.Append(content);

        m_urlBuilder.Append('&');

        m_urlBuilder.Append("sendtime=");

        m_urlBuilder.Append('&');
        m_urlBuilder.Append("phonelist=");
        m_urlBuilder.Append(phone);

        m_urlBuilder.Append('&');
        m_urlBuilder.Append("taskId=");

        DateTime now = DateTime.Now;
        string ss = genIdentifyingCode(new Random((int)DateTime.Now.Ticks));
        string str = string.Format("{0}_{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}_http_{7}", 
            WebConfigurationManager.AppSettings["account"], now.Year, now.Month, now.Day,
            now.Hour, now.Minute, now.Second, ss);
        m_urlBuilder.Append(str);

        return m_urlBuilder.ToString();
    }

    private string setUpMsgInfo(string phone, string content)
    {
        string url = WebConfigurationManager.AppSettings["url2"];
        string zh = WebConfigurationManager.AppSettings["account2"];
        string pwd = WebConfigurationManager.AppSettings["pwd2"];
        //string id = WebConfigurationManager.AppSettings["sms_type"];

        m_urlBuilder.Remove(0, m_urlBuilder.Length);
        m_urlBuilder.Append(url);
        m_urlBuilder.Append("?");
        m_urlBuilder.Append("Uid=");
        m_urlBuilder.Append(zh);
        m_urlBuilder.Append('&');

        m_urlBuilder.Append("Key=");
        m_urlBuilder.Append(pwd);
        m_urlBuilder.Append('&');

        m_urlBuilder.Append("smsMob=");
        m_urlBuilder.Append(phone);
        m_urlBuilder.Append('&');

        m_urlBuilder.Append("smsText=");
        m_urlBuilder.Append(content);
        return m_urlBuilder.ToString();
    }

    // 生成6位随机数字验证码
    private string genIdentifyingCode(Random rand)
    {
        m_textBuilder.Remove(0, m_textBuilder.Length);
        int i = 0;
        for (i = 0; i < 6; i++)
        {
            m_textBuilder.Append(rand.Next(0, 10));
        }
        return m_textBuilder.ToString();
    }
}

public class CTool
{
    public static string getMD5Hash(String input)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] res = md5.ComputeHash(Encoding.Default.GetBytes(input), 0, input.Length);
        return BitConverter.ToString(res).Replace("-", "");
    }

    // 根据符号ch对串str进行拆分
    public static string[] split(string str, char ch)
    {
        char[] sp = { ch };
        string[] arr = str.Split(sp);
        return arr;
    }
}
