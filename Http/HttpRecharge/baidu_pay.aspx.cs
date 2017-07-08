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
    public partial class baidu_pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.QueryString;

            if (req.Count <= 0)
            {
                Response.Write("success");
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            data["PayTime"] = DateTime.Now;
            string useKey = "";
            if (data.ContainsKey("cpdefinepart"))
            {
                useKey = data["cpdefinepart"] as string;
            }
            var findacc = MongodbPayment.Instance.ExecuteGetBykey("ex_orderinfo", "UseKey", useKey);
            if (findacc != null)
            {
                data["Account"] = findacc["Account"].ToString();
                data["PayCode"] = findacc["PayCode"].ToString();
                data["PlayerId"] = Convert.ToInt32(findacc["PlayerId"].ToString());
                MongodbPayment.Instance.ExecuteRemoveBykey("ex_orderinfo", "UseKey", useKey);
            }
            MongodbPayment.Instance.ExecuteInsert("baidu_log", data);

            if (findacc == null)
            {
                CLOG.Info("use key error");
                Response.Write("success");
                return;
            }

            try
            {
                if (data["sign"].ToString() != getSignForAnyValid(Request))
                {
                    CLOG.Info("check sign fail");
                    Response.Write("success");
                    return;
                }

                if (data["status"].ToString() == "success")
                {
                    string unitStr = data["unit"] as string;
                    int base_amount = 1;
                    if (unitStr == "fen")
                    {
                        base_amount = 100;
                    }
                    try
                    {
                        data["RMB"] = (int)(Convert.ToDouble(data["amount"]) / base_amount);
                    }
                    catch (Exception)
                    {
                        data["RMB"] = (int)(Convert.ToInt32(data["amount"]) / base_amount);
                    }

                    if (Convert.ToInt32(data["RMB"]) <= 0)
                    {
                        Response.Write("success");
                        return;
                    }
                    
                    if (!MongodbPayment.Instance.KeyExistsBykey("baidu_pay", "OrderID", data["orderid"]))
                    {
                        data["OrderID"] = data["orderid"].ToString();
                        data["Process"] = false;

                        if (MongodbPayment.Instance.ExecuteInsert("baidu_pay", data))
                        {
                            save_payinfo("lobby", Convert.ToInt32(data["RMB"]));
                        }
                    }
                }
                else
                {
                    Response.Write("success");
                    return;
                }
            }
            catch (Exception ex)
            {
                string result = ex.ToString();
                CLOG.Info(result);
            }
            Response.Write("success");
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
            NameValueCollection requestParams = request.QueryString;//获得所有的参数名
            string appidStr = requestParams["appid"] as string;
            string orderidStr = requestParams["orderid"] as string;
            string amountStr = requestParams["amount"] as string;
            string unitStr = requestParams["unit"] as string;
            string statusStr = requestParams["status"] as string;
            string paychannelStr = requestParams["paychannel"] as string;
            string appStr = ConfigurationManager.AppSettings["baidu_key"].ToString();

            string paramValues = appidStr + orderidStr + amountStr + unitStr + statusStr + paychannelStr + appStr;
            String md5Values = MD5Encode(paramValues);
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