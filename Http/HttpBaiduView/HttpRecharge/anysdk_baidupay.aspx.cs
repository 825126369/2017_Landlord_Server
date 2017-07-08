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
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace PaymentCheck
{
    public partial class anysdk_baidupay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string result = "ok";
            NameValueCollection req = Request.Form;

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

            data["PayTime"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("anysdk_log", data);

            try
            {
                if (data["sign"].ToString() == getSignForAnyValid(Request) && data["pay_status"].ToString() == "1")
                {
                    string payCode = data["product_id"].ToString();
                    int payMoney = 0;
                    try
                    {
                        payMoney = (int)Convert.ToDouble(data["amount"]);
                    }
                    catch (Exception)
                    {
                        payMoney = Convert.ToInt32(data["amount"]);
                    }
                    string orderID = "anysdk_" + data["order_id"].ToString();
                    string shoppage = "lobby";
                    string account = "";
                    if (data.ContainsKey("private_data"))
                    {
                        string[] strs = data["private_data"].ToString().Trim().Split('#');
                        if (strs.Length >= 2)
                        {
                            shoppage = strs[0];
                            account = strs[1];
                        }
                        else
                        {
                            Response.Write("private_data error");
                            Response.Flush();
                            return;
                        }
                    }
                    else
                    {
                        Response.Write("private_data error");
                        Response.Flush();
                        return;
                    }

                    if (!MongodbPayment.Instance.KeyExistsBykey("baiduview_pay", "OrderID", orderID) && payMoney > 0)
                    {
                        Dictionary<string, object> savedata = new Dictionary<string, object>();

                        savedata["OrderID"] = orderID;
                        savedata["Account"] = account;
                        savedata["RMB"] = payMoney;
                        savedata["PayCode"] = payCode;
                        savedata["Custom"] = 0;
                        savedata["PayTime"] = DateTime.Now;
                        savedata["PayPlatform"] = "anysdk";
                        savedata["ChannelID"] = data["channel_number"].ToString(); ;
                        savedata["Process"] = false;
                        savedata["PlayerId"] = Convert.ToInt32(data["game_user_id"]);

                        if (MongodbPayment.Instance.ExecuteInsert("baiduview_pay", savedata))
                        {
                            save_payinfo(shoppage, payMoney);
                        }

                        Dictionary<string, object> savelog = new Dictionary<string, object>();
                        savelog["acc"] = account;
                        //记录设备号
                        if (data.ContainsKey("acc_dev"))
                        {
                            savelog["acc_dev"] = data["acc_dev"];
                        }
                        savelog["time"] = DateTime.Now;
                        savelog["channel"] = savedata["ChannelID"].ToString();
                        savelog["rmb"] = savedata["RMB"];
                        MongodbPayment.Instance.ExecuteInsert("PayLog", savelog);
                    }                    
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                CLOG.Info(ex.ToString());
            }

            Response.Write(result);
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
                if (param == "sign")
                {
                    continue;
                }
                String paramValue = requestParams[param];
                if (paramValue != null)
                {
                    paramValues += paramValue;
                }
            }
            String md5Values = MD5Encode(paramValues);
            md5Values = MD5Encode(md5Values.ToLower() + ConfigurationManager.AppSettings["anysdk_key"].ToString()).ToLower();
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