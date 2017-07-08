using System;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace _360Game
{
    public class IISExchangeHandler : IHttpHandler
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此处理程序 
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        /// 

        class ExchangeResult
        {
            public int errno;
            public string errmsg;
            public ExchangeData data;
        }

        class ExchangeData
        {
            public string order_id;
            public string uid;
            public string role_id;
            public string role_name;
            public string platfrom;
            public string gkey;
            public string skey;
            public string coins;
            public string moneys;
            public string time;
        }

        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        static string[] checkField = { "uid", "role_id", "role_name", "platform", "gkey", "skey", "order_id", "coins", "moneys", "time" };
        static string[] checkOrder = { "uid", "platform", "gkey", "skey", "time", "order_id", "coins", "moneys"};
        static string checkKey = "DIIdEG2kH3dbfMMcbkLwVzsgP1H1yJD7";


        public void ProcessRequest(HttpContext context)
        {
            HandlerHelper.log(context, "exchange", context.Request.Form["uid"]);
            Dictionary<string, string> data = HandlerHelper.convertParams(context.Request.Form);

            Dictionary<string, object> dataLog = new Dictionary<string, object>();
            foreach(var v in data)
            {
                dataLog[v.Key] = v.Value;
            }
            MongodbPayment.Instance.ExecuteInsert("360pay_log", dataLog);

            bool result = HandlerHelper.checkData(data, checkField);
            if (!result)
            {
                WriteResult(context , -1, null);
                return;
            }

            result = HandlerHelper.checkSign(data, checkOrder, checkKey);
            if (!result)
            {
                WriteResult(context , -2, null);
                return;
            }

            int payMoney = 0;
            try
            {
                payMoney = (int)(Convert.ToDouble(data["moneys"])/100);
            }
            catch (Exception)
            {
                payMoney = (int)(Convert.ToInt32(data["moneys"]) / 100);
            }
            string giveGem = data["coins"].ToString();
            string orderID = data["order_id"].ToString();
            string acc = string.Format("360_{0}", HandlerHelper.UrlDecode(data["uid"]));

            if (!MongodbAccount.Instance.KeyExistsBykey("AccountTable", "acc", acc))
            {
                WriteResult(context, 3, null);
                return;
            }

            if (!MongodbPayment.Instance.KeyExistsBykey("360_pay", "OrderID", orderID))
            {
                Dictionary<string, object> savedata = new Dictionary<string, object>();
                savedata["OrderID"] = orderID;
                savedata["Account"] = acc;
                savedata["RMB"] = payMoney;
                savedata["PayCode"] = giveGem;
                savedata["Custom"] = 2;//钻石
                savedata["PayTime"] = DateTime.Now;
                savedata["PayPlatform"] = "360pay";
                savedata["Process"] = false;

                if (MongodbPayment.Instance.ExecuteInsert("360_pay", savedata))
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
                    notify_monitor(acc, orderID);
                }
                ExchangeData exchangeData = new ExchangeData();
                exchangeData.order_id = data["order_id"];
                exchangeData.uid = data["uid"];
                exchangeData.role_id = data["role_id"];
                exchangeData.role_name = data["role_name"];
                exchangeData.platfrom = data["platform"];
                exchangeData.gkey = data["gkey"];
                exchangeData.skey = data["skey"];
                exchangeData.coins = data["coins"];
                exchangeData.moneys = data["moneys"];
                exchangeData.time = data["time"];

                WriteResult(context, 0, exchangeData);
            }
            else
            {
                WriteResult(context, 1, null);
            }
        }

        void WriteResult(HttpContext context, int errno, ExchangeData checkData)
        {
            ExchangeResult result = new ExchangeResult();
            if (errno == 0)
            {
                result.errno = errno;
                result.errmsg = "充值成功";
                result.data = checkData;
            }
            else if (errno == 1)
            {
                result.errno = errno;
                result.errmsg = "订单重复";
            }
            else if (errno == -1)
            {
                result.errno = errno;
                result.errmsg = "参数不全";
            }
            else if (errno == -2)
            {
                result.errno = errno;
                result.errmsg = "签名错误";
            }
            else if (errno == -3)
            {
                result.errno = errno;
                result.errmsg = "用户不存在";
            }
            else if (errno == -4)
            {
                result.errno = errno;
                result.errmsg = "请求超时";
            }

            string str = JsonHelper.ConvertToStr(result);
            context.Response.Write(str);
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

        #endregion
    }
}
