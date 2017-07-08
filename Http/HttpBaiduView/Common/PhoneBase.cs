using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Text;
using System.Security.Cryptography;

public class PhoneCom
{
    // 组装验证码接口
    static public string sendCheckInfo(string phone, string code, string fmt)
    {
        string content = string.Format(fmt, code);
        return sendMsgInfo(phone, content);
    }

    // 组装账号找回接口
    static public string sendSearchAccount(string phone, string account)
    {
        string content = string.Format(WebConfigurationManager.AppSettings["content"], account);
        return sendMsgInfo(phone, content);
    }

    static public string sendPhoneCode(string phone, string pwdcode)
    {
        string content = string.Format(WebConfigurationManager.AppSettings["pwdcode"], pwdcode);
        return sendMsgInfo(phone, content);
    }

    // 组装服务器监控短信
    static public string sendMonitorInfo(string phone, string serverIdList, string msgInfo)
    {
        string content = string.Format(WebConfigurationManager.AppSettings[msgInfo], serverIdList); ;
        return sendMsgInfo(phone, content);
    }

    static private string sendMsgInfo(string phone, string content)
    {
        string use = WebConfigurationManager.AppSettings["use"];
        PhoneBase phoneBase = null;
        if (use == "1")
        {
            phoneBase = new PhoneCom1();
        }
        else if (use == "2")
        {
            phoneBase = new PhoneCom2();
        }
        else
        {
            phoneBase = new PhoneCom3();
        }
        return phoneBase.sendMsgInfo(phone, content);
    }
}

public class PhoneBase
{
    public virtual string sendMsgInfo(string phone, string content)
    {
        return "err_sendfailed";  // 发送短信失败
    }
}