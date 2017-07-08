using BeeCloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Configuration;

namespace HttpRecharge
{
    public partial class beecloud_pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BeeCloud.BeeCloud.registerApp(ConfigurationManager.AppSettings["beecloud_appid"].ToString(), ConfigurationManager.AppSettings["beecloud_secret"].ToString());
            int test = 0;
            string req = "";

            Dictionary<string, object> log = new Dictionary<string, object>();
            log["time"] = DateTime.Now;

            byte[] byts = new byte[Request.InputStream.Length];
            Request.InputStream.Read(byts, 0, byts.Length);
            req = System.Text.Encoding.Default.GetString(byts);
            req = Server.UrlDecode(req);
            log["info"] = req;

            var requestData = JsonHelper.ParseFromStr<Dictionary<string, object>>(req);
            string orderid = requestData["transaction_id"].ToString();

            try
            {                
                MongodbPayment.Instance.ExecuteUpdate("beecloud_infos", "orderid", orderid, log);

                string sign = requestData["sign"].ToString();
                long timestamp = long.Parse(requestData["timestamp"].ToString());
                string channelType = requestData["channel_type"].ToString();
                //string transactionType = requestData["transaction_type"].ToString();

                //检查timestamp是否在可信时间段内，阻止重发
                TimeSpan ts = DateTime.Now - BCUtil.GetDateTime(timestamp);

                //验签， 确保来自BeeCloud
                string mySign = BCUtil.GetSign(requestData["timestamp"].ToString());
                if (ts.TotalSeconds < 300 && mySign == sign)
                {
                    // 此处需要验证购买的产品与订单金额是否匹配:
                    // 验证购买的产品与订单金额是否匹配的目的在于防止黑客反编译了iOS或者Android app的代码，
                    // 将本来比如100元的订单金额改成了1分钱，开发者应该识别这种情况，避免误以为用户已经足额支付。
                    // Webhook传入的消息里面应该以某种形式包含此次购买的商品信息，比如title或者optional里面的某个参数说明此次购买的产品是一部iPhone手机，
                    // 开发者需要在客户服务端去查询自己内部的数据库看看iPhone的金额是否与该Webhook的订单金额一致，仅有一致的情况下，才继续走正常的业务逻辑。
                    // 如果发现不一致的情况，排除程序bug外，需要去查明原因，防止不法分子对你的app进行二次打包，对你的客户的利益构成潜在威胁。
                    // 如果发现这样的情况，请及时与我们联系，我们会与客户一起与这些不法分子做斗争。而且即使有这样极端的情况发生，
                    // 只要按照前述要求做了购买的产品与订单金额的匹配性验证，在你的后端服务器不被入侵的前提下，你就不会有任何经济损失。

                    int amont = int.Parse(requestData["transaction_fee"].ToString())/100;
                    if (requestData["transaction_type"].ToString() == "PAY"
                        && amont > 0)
                    {
                        test++;
                        Dictionary<string, object> message_detail = JsonHelper.ParseFromStr<Dictionary<string, object>>(requestData["messageDetail"].ToString());
                        requestData["messageDetail"] = message_detail;

                        if (requestData.ContainsKey("message_detail"))
                            requestData.Remove("message_detail");

                        test++;
                        bool is_scuess = false;
                        switch (channelType)
                        {
                            case "ALI":
                                {
                                    if (message_detail["trade_status"].ToString() == "TRADE_SUCCESS")
                                        is_scuess = true;
                                }
                                break;
                            case "WX":
                                {
                                    if (message_detail["result_code"].ToString() == "SUCCESS")
                                        is_scuess = true;
                                }
                                break;
                            case "UN":
                                {
                                    if (message_detail["respCode"].ToString() == "00")
                                        is_scuess = true;
                                }
                                break;
                        }
                        test++;

                        if (is_scuess && !MongodbPayment.Instance.KeyExistsBykey("beecloud_pay", "OrderID", orderid))
                        {
                            test++;
                            requestData["OrderID"] = orderid;
                            requestData["PayTime"] = DateTime.Now;
                            requestData["Process"] = false;
                            requestData["RMB"] = amont;

                            test++;
                            Dictionary<string, object> optional = JsonHelper.ParseFromStr<Dictionary<string, object>>(requestData["optional"].ToString());
                            requestData["optional"] = optional;

                            requestData["PlayerId"] = Convert.ToInt32(optional["playerid"]);
                            requestData["private_data"] = optional["shoppage"];
                            requestData["shoppage"] = optional["shoppage"];
                            requestData["Account"] = optional["account"];
                            requestData["PayCode"] = optional["paycode"];
                            requestData["channel"] = optional["channel"];

                            test++;
                            if (MongodbPayment.Instance.ExecuteInsert("beecloud_pay", requestData))
                            {
                                test++;
                                save_payinfo(requestData["private_data"].ToString(), amont);
                                save_paychannel(requestData["channel"].ToString(), amont);
                                test++;
                            }
                        }
                    }

                    //当验签成功后务必返回success字样，通知server获取成功。
                    Response.Write("success");
                }
                else
                {
                    Response.Write("fail");
                }
            }
            catch (Exception ex)
            {
                Response.Write(req + test.ToString() + ex.ToString());               
                log["error"] = ex.ToString();              
            }

            log["tag"] = test;
            MongodbPayment.Instance.ExecuteUpdate("beecloud_infos", "orderid", orderid, log);
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

        void save_paychannel(string info, int amount)
        {
            if (string.IsNullOrEmpty(info) || amount <= 0)
                return;

            MongodbPayment.Instance.ExecuteIncBykey("day_channelpay", "date", DateTime.Now.Date, info, 0, amount);
        }
    }
}