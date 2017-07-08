using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace HttpLogin.Default
{
    public partial class PhoneCode : System.Web.UI.Page
    {
        Random m_rd = new Random();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string strphone = Request.Params["phone"];
            if (string.IsNullOrEmpty(strphone))            
                return;

            if (strphone.Length != 11 || !Regex.IsMatch(strphone, @"^\d{11}$"))
            {
                Response.Write("err_not_phone");//号码错误
                return;
            }
                        
            string[] field = { "searchTime", "searchCount" };
            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey("AccountTable", "bindPhone", strphone, field);
            if (data == null)
            {
                Response.Write("err_not_bind");//未绑定  
            }
            else
            {
                DateTime last = DateTime.MinValue; // 账号找回时间
                if (data.ContainsKey("searchTime"))
                {
                    last = Convert.ToDateTime(data["searchTime"]).ToLocalTime();
                }

                DateTime now = DateTime.Now;
                TimeSpan span = now - last;
                int interval = Convert.ToInt32(ConfigurationManager.AppSettings["search_interval"]);
                if (span.TotalSeconds < interval)                   
                {
                    Response.Write("err_timecd");//验证码cd时间
                    return;
                }

                int searchCount = 0;
                if (data.ContainsKey("searchCount"))
                {
                    searchCount = Convert.ToInt32(data["searchCount"]);
                }

                if (last.DayOfWeek != now.DayOfWeek)
                    searchCount = 0;

                int limitCount = Convert.ToInt32(ConfigurationManager.AppSettings["search_count"]);
                if (searchCount >= limitCount)
                {
                    Response.Write("err_maxcount");//当日次数已满
                    return;
                }

                string pwdcode = m_rd.Next(100000, 999999).ToString();

                data["searchCount"] = ++searchCount;
                data["searchTime"] = now;
                data["pwdcode"] = pwdcode;
                MongodbAccount.Instance.ExecuteUpdate("AccountTable", "bindPhone", strphone, data);

                string ret = sendMsgToPhone(strphone, pwdcode);
                Response.Write(ret);
            }      
        }

        string sendMsgToPhone(string phone, string pwdcode)
        {
            return PhoneCom.sendPhoneCode(phone, pwdcode);
        }    
    }
}