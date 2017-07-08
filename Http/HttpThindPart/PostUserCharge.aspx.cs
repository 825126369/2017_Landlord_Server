using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Collections.Specialized;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace HttpThindPart
{
    public class ParamSendMail
    {
        public string m_title = "";
        public string m_sender = "";
        public string m_content = "";
        public int m_playerId;
        // public string m_itemList = "";
    }

    public partial class PostUserCharge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.Form;

            if (req.Count <= 0)
            {
                ReturnMsg(99999);
                return;
            }

            Dictionary<string, object> param = new Dictionary<string, object>();
            foreach (string key in req)
            {
                param[key] = req[key];
            }
            param["time"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("wechat_log", param);

            try
            {
                string playerId = param["id"].ToString();
                string orderid = param["orderid"].ToString();
                string chargeid = param["chargeid"].ToString();
                string amount = param["amount"].ToString();
                string sign = param["sign"].ToString();

                if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(orderid) || string.IsNullOrEmpty(chargeid) || string.IsNullOrEmpty(amount))
                {
                    ReturnMsg(99999);
                    return;
                }

                int iPlayerid;
                bool flag = int.TryParse(playerId, out iPlayerid);
                if (flag == false)
                {
                    ReturnMsg(99999);
                    return;
                }

                int iAmount = 0;
                flag = int.TryParse(amount, out iAmount);
                if (flag == false)
                {
                    ReturnMsg(99999);
                    return;
                }
                iAmount = iAmount / 100;

                if (sign != getSignForAnyValid(Request))
                {
                    // Response.Write(getSignForAnyValid(Request));
                    ReturnMsg(10099);
                    return;
                }

                Dictionary<string, object> data = new Dictionary<string, object>();
                data["amount"] = iAmount.ToString();
                try
                {
                    data["RMB"] = (int)Convert.ToDouble(data["amount"]);
                }
                catch (Exception)
                {
                    data["RMB"] = Convert.ToInt32(data["amount"]);
                }

                if (!MongodbPayment.Instance.KeyExistsBykey("wechat_pay", "OrderID", orderid) && Convert.ToInt32(data["RMB"]) > 0)
                {
                    DateTime now = DateTime.Now;
                    string playerTable = "player_info";
                    Dictionary<string, object> PlayerInfo = MongodbPlayer.Instance.ExecuteGetBykey(playerTable, "player_id", iPlayerid, new string[] { "platform", "ChannelID", "account" });
                    if (PlayerInfo != null && PlayerInfo.Count > 0)
                    {
                        data["Account"] = PlayerInfo["account"].ToString();
                        data["ChannelId"] = PlayerInfo["ChannelID"].ToString();
                    }
                    else
                    {
                        ReturnMsg(10004);
                        return;
                    }
                    data["OrderID"] = orderid;
                    data["PayCode"] = chargeid;
                    data["Process"] = false;
                    data["PlayerId"] = Convert.ToInt32(playerId);
                    data["ServerId"] = 0;
                    data["ChannelId"] = PlayerInfo["ChannelID"].ToString();
                    data["PayTime"] = now;

                    if (MongodbPayment.Instance.ExecuteInsert("wechat_pay", data))
                    {
                        string shoppage = "wechat";

                        save_payinfo(shoppage, Convert.ToInt32(data["RMB"]));
                    }

                    Dictionary<string, object> savelog = new Dictionary<string, object>();
                    savelog["acc"] = data["Account"];
                    savelog["real_acc"] = data["Account"];
                    //记录设备号

                    savelog["time"] = now;
                    savelog["channel"] = Convert.ToInt32(data["ChannelId"]);
                    savelog["rmb"] = data["RMB"];
                    savelog["pay_type"] = 1;   //微信支付

                    // 写pay log
                    MongodbPayment.Instance.ExecuteInsert("PayLog", savelog);

                    // 通知游戏服务器
                    /*
                    string url = "orderid=" + data["OrderID"].ToString() + "&playerid=" + data["PlayerId"].ToString() + "&paycode=" +
                        data["PayCode"].ToString() + "&channelid=" + data["ChannelId"].ToString();
                    string server_api = "http://" + ConfigurationManager.AppSettings["server_api"].ToString() + "/cmd=1&" + url;

                    var ret = HttpPost.Post(new Uri(server_api));
                    if (ret != null)
                    {
                        string retstr = Encoding.UTF8.GetString(ret);
                        if (retstr == "ok")
                        {
                            ReturnMsg(10000);
                            return;
                        }
                    }
                     * */

                    // 插入数据到 Gmrecharge 表中
                    Dictionary<string, object> dataGmRecharge = new Dictionary<string, object>();
                    dataGmRecharge["playerId"] = Convert.ToInt32(playerId);
                    dataGmRecharge["rtype"] = 0;
                    dataGmRecharge["param"] = Convert.ToInt32(chargeid);
                    MongodbPlayer.Instance.ExecuteInsert("gmRecharge", dataGmRecharge);

                    // 发邮件
                    ParamSendMail paramMail = new ParamSendMail();
                    paramMail.m_title = ConfigurationManager.AppSettings["mail_title"].ToString();
                    paramMail.m_sender = ConfigurationManager.AppSettings["mail_sender"].ToString();
                    paramMail.m_content = ConfigurationManager.AppSettings["mail_content"].ToString();
                    paramMail.m_playerId = Convert.ToInt32(playerId);


                    bool res = sendMail(paramMail);
                    ReturnMsg(10000);
                }
                else
                {
                    ReturnMsg(10003);
                }
            }
            catch (Exception ex)
            {
                ReturnMsg(99999);
                CLOG.Info(ex.ToString());
                return;
            }

        }

        public bool sendMail(object param)
        {

            ParamSendMail p = (ParamSendMail)param;

            int days = 7;

            return specialSend(p, days, null, p.m_playerId);

        }

        private bool specialSend(ParamSendMail p, int days, BsonDocument mailItem, int playerId)
        {
            bool res = false;
            DateTime now = DateTime.Now;
            DateTime nt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("title", p.m_title);
            data.Add("sender", p.m_sender);
            data.Add("content", p.m_content);

            data.Add("time", nt);
            data.Add("deadTime", nt.AddDays(days));
            data.Add("isReceive", false);
            data.Add("playerId", playerId);

            // 标识是系统发送的邮件
            data.Add("senderId", 0);

            if (mailItem != null)
            {
                data.Add("gifts", mailItem);
            }

            res = MongodbPlayer.Instance.ExecuteInsert("playerMail", data);

            return res;
        }

        /// ///////////////////////////////////////////////////////////////////////////////////////
        void ReturnMsg(int bret = 10000)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;
            data["msg"] = ConfigurationManager.AppSettings["return_" + bret.ToString()];

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(jsonstr);
        }

        void save_payinfo(string info, int amount)
        {
            if (string.IsNullOrEmpty(info) || amount <= 0)
                return;

            DateTime dt = DateTime.Now.Date;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[info] = (long)amount;
            data["total_rmb"] = (long)amount;

            MongodbPayment.Instance.ExecuteIncByQuery("pay_infos", Query.EQ("date", BsonValue.Create(dt)), data);
        }


        //获得anysdk支付通知 sign,将该函数返回的值与any传过来的sign进行比较验证
        public String getSignForAnyValid(HttpRequest request)
        {
            NameValueCollection requestParams = request.Form;//获得所有的参数名
            List<String> ps = new List<String>();
            foreach (string key in requestParams)
            {
                ps.Add(key);
            }

            sortParamNames(ps);// 将参数名从小到大排序，结果如：adfd,bcdr,bff,zx

            String paramValues = "";
            foreach (string param in ps)
            {//拼接参数值
                if (param == "sign" || param == "time")
                {
                    continue;
                }
                String paramValue = requestParams[param];
                if (paramValue != null)
                {
                    paramValues += paramValue;
                }
            }
            String md5Values = MD5Encode(ConfigurationManager.AppSettings["wechat_key"].ToString() + paramValues).ToLower();
            return md5Values;
        }


        //MD5编码
        public static String MD5Encode(String sourceStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] src = Encoding.UTF8.GetBytes(sourceStr);
            byte[] res = md5.ComputeHash(src, 0, src.Length);
            return BitConverter.ToString(res).ToLower().Replace("-", "");
        }

        //将参数名从小到大排序，结果如：adfd,bcdr,bff,zx
        public static void sortParamNames(List<String> paramNames)
        {
            paramNames.Sort((String str1, String str2) => { return str1.CompareTo(str2); });
        }
    }
}