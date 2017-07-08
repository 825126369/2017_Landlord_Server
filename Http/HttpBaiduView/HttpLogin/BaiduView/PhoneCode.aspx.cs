using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Common;

namespace HttpLogin.thirdsdk
{
    public partial class PhoneCode : System.Web.UI.Page
    {
        Random m_rd = new Random();
        string AES_KEY = "@@baiduviewkey@@";

        protected void Page_Load(object sender, EventArgs e)
        {
            string phoneNum = Request.QueryString["phonenum"];
            if (string.IsNullOrEmpty(phoneNum))
            {
                Response.Write(Helper.buildLuaReturn(-2, "err_not_phone"));//号码错误
                return;
            }
            phoneNum = Encoding.Default.GetString(Convert.FromBase64String(phoneNum));
            phoneNum = AESHelper.AESDecrypt(phoneNum, AES_KEY);//aes解密
            if (phoneNum.Length != 11 || !Regex.IsMatch(phoneNum, @"^\d{11}$"))
            {
                Response.Write(Helper.buildLuaReturn(-2, "err_not_phone"));//号码错误
                return;
            }

            int sendCount = 0;                        
            string[] field = { "lastSendTime", "sendCount" };
            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey("BaiduPhoneCode", "phoneNum", phoneNum, field);
            if (data != null)
            {
                DateTime last = DateTime.MinValue;
                if (data.ContainsKey("lastSendTime"))
                {
                    last = Convert.ToDateTime(data["lastSendTime"]).ToLocalTime();
                }
                DateTime now = DateTime.Now;
                TimeSpan span = now - last;
                int interval = Convert.ToInt32(ConfigurationManager.AppSettings["send_interval"]);
                if (span.TotalSeconds < interval)                   
                {
                    Response.Write(Helper.buildLuaReturn(-3, "err_timecd"));//验证码cd时间
                    return;
                }

                if (data.ContainsKey("SendCount"))
                {
                    sendCount = Convert.ToInt32(data["SendCount"]);
                }

                if (last.DayOfYear != now.DayOfYear)
                    sendCount = 0;

                int limitCount = Convert.ToInt32(ConfigurationManager.AppSettings["send_count"]);
                if (sendCount >= limitCount)
                {
                    Response.Write(Helper.buildLuaReturn(-4, "err_maxcount"));//当日次数已满
                    return;
                }
            }

            string pwdcode = m_rd.Next(100000, 999999).ToString();

            Dictionary<string, object> savedata = new Dictionary<string,object>();
            savedata["sendCount"] = ++sendCount;
            savedata["lastSendTime"] = DateTime.Now;
            savedata["phoneCode"] = pwdcode;
            if(data == null)
            {
                savedata["phoneNum"] = phoneNum;
                MongodbAccount.Instance.ExecuteInsert("BaiduPhoneCode", savedata);
            }
            else
            {
                MongodbAccount.Instance.ExecuteUpdate("BaiduPhoneCode", "phoneNum", phoneNum, savedata);
            }

            string ret = sendMsgToPhone(phoneNum, pwdcode);
            Response.Write(Helper.buildLuaReturn(0, ret));
        }

        string sendMsgToPhone(string phone, string pwdcode)
        {
            return PhoneCom.sendPhoneCode(phone, pwdcode);
        }
    }
}