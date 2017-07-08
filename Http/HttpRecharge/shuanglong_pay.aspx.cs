using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using Common;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HttpRecharge
{
    public partial class shuanglong_pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.Form;

            if (req.Count <= 0)
            {
                Response.Write("error");
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            data["PayTime"] = DateTime.Now;
            string orderID = Convert.ToString(data["cpOrderId"]);
            var findacc = MongodbPayment.Instance.ExecuteGetBykey("shuanglong_orderinfo", "OrderID", orderID);
            if (findacc != null)
            {
                data["Account"] = findacc["Account"].ToString();
                if (findacc.ContainsKey("Channel"))
                {
                    data["Channel"] = findacc["Channel"].ToString();
                }
                data["PayCode"] = findacc["PayCode"].ToString();
                data["PlayerId"] = Convert.ToInt32(findacc["PlayerId"].ToString());
                //MongodbPayment.Instance.ExecuteRemoveBykey("shuanglong_orderinfo", "OrderID", orderID);
            }
            MongodbPayment.Instance.ExecuteInsert("shuanglong_log", data);

            if (findacc == null)
            {
                CLOG.Info("orderID error");
                Response.Write("orderIdError");
                return;
            }

            //data["cpOrderId"]
            //data["slOrderId"]
            //data["channel"]
            //data["payType"]
            //data["price"]
            //data["status"]
            //data["userId"]
            //data["time"]
            //data["sign"]


            try
            {
                if (data["sign"].ToString() != getSignForAnyValid(Request))
                {
                    CLOG.Info("check sign fail");
                    Response.Write("errorSign");
                    return;
                }
                if (Convert.ToInt32(data["status"]) == 1)   //支付状态：0未支付，1支付成功，2支付失败
                {
                    data["RMB"] = Convert.ToInt32(data["price"]);
                    if (Convert.ToInt32(data["RMB"]) <= 0)
                    {
                        Response.Write("SUCCESS");
                        return;
                    }
                    if (!MongodbPayment.Instance.KeyExistsBykey("shuanglong_pay", "OrderID", data["cpOrderId"]))
                    {
                        data["OrderID"] = data["cpOrderId"].ToString();
                        data["Process"] = false;

                        if (MongodbPayment.Instance.ExecuteInsert("shuanglong_pay", data))
                        {
                            save_payinfo("lobby", Convert.ToInt32(data["RMB"]));

                            Dictionary<string, object> savelog = new Dictionary<string, object>();
                            savelog["acc"] = data["Account"];
                            savelog["real_acc"] = data["Account"];
                            savelog["time"] = DateTime.Now;
                            savelog["channel"] = data["Channel"];
                            savelog["rmb"] = data["RMB"];
                            MongodbPayment.Instance.ExecuteInsert("PayLog", savelog);
                        }
                    }
                }
                else
                {
                    Response.Write("SUCCESS");
                    return;
                }
            }
            catch (Exception ex)
            {
                string result = ex.ToString();
                CLOG.Info(result);
                Response.Write("error");
                return;
            }
            Response.Write("SUCCESS");
        }

        //获得shuanglong支付通知 sign,将该函数返回的值与shuanglong传过来的sign进行比较验证
        public String getSignForAnyValid(HttpRequest request)
        {
            NameValueCollection requestParams = request.Form;//获得所有的参数名

            Dictionary<string, string> ps = new Dictionary<string, string>();
            foreach (string key in requestParams)
            {
                ps.Add(key, requestParams[key]);
            }

            StringBuilder paramValues = new StringBuilder();
            paramValues.Append(ps["cpOrderId"]);
            paramValues.Append(ps["slOrderId"]);
            paramValues.Append(ps["channel"]);
            paramValues.Append(ps["payType"]);
            paramValues.Append(ps["price"]);
            paramValues.Append(ps["status"]);
            paramValues.Append(ps["userId"]);
            paramValues.Append(ps["time"]);

            string appStr = "18fc19f4980448e6b0bce397542b8ca6";
            paramValues.Append(appStr);

            //string signkey = paramValues.ToString().ToLower();
            String md5Values = Helper.getMD5(paramValues.ToString());
            return md5Values;
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
    }
}