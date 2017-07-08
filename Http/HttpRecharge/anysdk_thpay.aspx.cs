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

namespace PaymentCheck
{
    public partial class anysdk_thpay : System.Web.UI.Page
    {
        public string ret_code = null;
        public string price = null;
        public string point = null;
        public string debug_msg = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            string result = "success";
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
                    string pid = data["product_id"].ToString();
                    int index = pid.LastIndexOf('_');
                    if (index>0)
                        pid = pid.Remove(0, index+1);

                    //try
                    //{
                    //    int icode = Convert.ToInt32(pid);
                    //}
                    //catch (Exception ed)
                    //{
                    //    data["amount"] = "0";//异常订单不算充值
                    //    result = ed.ToString();
                    //}
                    
                    try
                    {
                        data["RMB"] = (int)Convert.ToDouble(data["amount"]);
                    }
                    catch (Exception)
                    {
                        data["RMB"] = Convert.ToInt32(data["amount"]);
                    }

                    if (!MongodbPayment.Instance.KeyExistsBykey("anysdk_pay", "OrderID", data["order_id"]) && Convert.ToInt32(data["RMB"]) > 0)
                    {
                        data["OrderID"] = data["order_id"].ToString();
                        data["Account"] = data["channel_number"].ToString() + "_" + data["user_id"].ToString();
                        data["PayCode"] = pid;                        
                        data["Process"] = false;
                        data["PlayerId"] = Convert.ToInt32(data["game_user_id"]);
                        data["ServerId"] = Convert.ToInt32(data["server_id"]);

                        if (data.ContainsKey("private_data"))
                        {
                            string[] strs = data["private_data"].ToString().Trim().Split('#');
                            if (strs.Length > 1)
                            {
                                data["shoppage"] = strs[0];
                                data["Account"] = strs[1];
                            }
                            else
                            {
                                data["shoppage"] = data["private_data"];
                            }
                        }

                        if (MongodbPayment.Instance.ExecuteInsert("anysdk_pay", data))
                        {
                            if (data.ContainsKey("shoppage"))
                                save_payinfo(data["shoppage"].ToString(), Convert.ToInt32(data["RMB"]));
                            else
                                save_payinfo("lobby", Convert.ToInt32(data["RMB"]));
                        }

                        Dictionary<string, object> savelog = new Dictionary<string, object>();
                        savelog["acc"] = data["Account"];
                        savelog["time"] = DateTime.Now;
                        savelog["channel"] = data["channel_number"].ToString();
                        savelog["rmb"] = data["RMB"];
                        MongodbPayment.Instance.ExecuteInsert("PayLog", savelog);
                       
                        //ret_code = data["ret_code"].ToString();
                        price = data["RMB"].ToString();
                        point = data["product_name"].ToString();
                        //debug_msg = data["debug_msg"].ToString();
                        string URL = string.Format("http://103.249.210.94/auth/topup/inapp_purchase/android.php?debug_msg={0}&price={1}&point={2}&ret_code={3}", result, price, point, result);
                        HttpPost.Post(new Uri(URL));
                        
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