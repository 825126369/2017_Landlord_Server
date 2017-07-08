using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HttpRecharge
{
    public partial class baiduview_pay : System.Web.UI.Page
    {
        static string appkey = "80E44EE5-AA9E-4B68-83F0-57F7B30B4F89";

        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.QueryString;
            if (req.Count <= 0)
            {
                Response.Write("req.Count <= 0");
                Response.Flush();
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }
            data["LogTime"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("baiduview_log", data);

            string urlsign = string.Format("orderid={0}&account={1}&paymoney={2}&givegem={3}&appkey={4}", 
                data["orderid"].ToString(), data["account"].ToString(), data["paymoney"].ToString(), data["givegem"].ToString(), appkey);
            urlsign = MD5Encode(urlsign);

            if (urlsign != data["sign"].ToString())
            {
                Response.Write("sign error");
                Response.Flush();
                return;
            }
            string orderID = "baiduview_" + data["orderid"].ToString();
            string account = data["account"].ToString();

            int payMoney = 0;
            try
            {
                payMoney = (int)Convert.ToDouble(data["paymoney"]);
            }
            catch (Exception)
            {
                payMoney = Convert.ToInt32(data["paymoney"]);
            }
            string giveGem = data["givegem"].ToString();


            if (!MongodbPayment.Instance.KeyExistsBykey("baiduview_pay", "OrderID", orderID) && payMoney > 0)
            {
                Dictionary<string, object> savedata = new Dictionary<string, object>();
                savedata["OrderID"] = orderID;
                savedata["Account"] = account;
                savedata["RMB"] = payMoney;
                savedata["PayCode"] = giveGem;
                savedata["Custom"] = 2;//钻石
                savedata["PayTime"] = DateTime.Now;
                savedata["PayPlatform"] = "baiduview";
                savedata["Process"] = false;

                if (MongodbPayment.Instance.ExecuteInsert("baiduview_pay", savedata))
                {
                    string shoppage = "lobby";
                    if (data.ContainsKey("shoppage"))
                    {
                        shoppage = data["shoppage"].ToString();
                        if (string.IsNullOrEmpty(shoppage))
                        {
                            shoppage = "lobby";
                        }
                    }
                    save_payinfo(shoppage, payMoney);
                    notify_monitor(account, orderID);
                }
            }

            Response.Write("success");
            Response.Flush();
        }

        //MD5编码
        public static String MD5Encode(String sourceStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] src = Encoding.UTF8.GetBytes(sourceStr);
            byte[] res = md5.ComputeHash(src, 0, src.Length);
            return BitConverter.ToString(res).ToLower().Replace("-", "");
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

        void notify_monitor(string account, string orderid)
        {
            string server_api = "http://" + ConfigurationManager.AppSettings["server_api"].ToString() + "/cmd=2&account=" + account + "&orderid=" + orderid;

            try
            {
                var ret = HttpPost.Post(new Uri(server_api));
                if (ret != null)
                {
                    string retstr = Encoding.UTF8.GetString(ret);
                    return;
                }
            }
            catch (Exception)
            {
                //投递失败 玩家下次登录可以检测充值
            }           
        }
    }
}