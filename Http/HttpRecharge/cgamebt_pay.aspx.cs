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
    public partial class cgamebt_pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.QueryString;

            if (req.Count <= 0)
            {
                Response.Write("fail");
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            data["PayTime"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("cgamebt_log", data);

            try
            {
                string from = data["from"].ToString();
                if (!from.Equals("sdk"))
                {
                    Response.Write("fail");
                    return;
                }

                if (data["sign"].ToString() != getSignForAnyValid(Request))
                {
                    Response.Write("fail");
                    return;
                }
                string openSource = "lobby";
                string ext1 = data["ext1"].ToString();
                if (string.IsNullOrEmpty(ext1))
                {
                    Response.Write("fail");
                    return;
                }

                string[] extdata = ext1.Split(':');
                if (extdata.Length < 4)
                {
                    Response.Write("fail");
                    return;
                }
                data["Account"] = extdata[0];
                data["PlayerId"] = Convert.ToInt32(extdata[1]);
                data["PayCode"] = extdata[2];
                openSource = extdata[3];

                try
                {
                    data["RMB"] = (int)(Convert.ToDouble(data["money"]) / 100);
                }
                catch (Exception)
                {
                    data["RMB"] = (int)(Convert.ToInt32(data["money"]) / 100);
                }

                if (Convert.ToInt32(data["RMB"]) <= 0)
                {
                    Response.Write("fail");
                    return;
                }

                if (MongodbPayment.Instance.KeyExistsBykey("cgamebt_pay", "orderid", data["orderid"]))
                {
                    Response.Write("success");
                    return;
                }
                else
                {
                    data["OrderID"] = data["outorderid"].ToString();
                    data["Process"] = false;

                    if (MongodbPayment.Instance.ExecuteInsert("cgamebt_pay", data))
                    {
                        save_payinfo("lobby", Convert.ToInt32(data["RMB"]));
                    }
                }
            }
            catch (Exception ex)
            {
                string result = ex.ToString();
                CLOG.Info(result);
                Response.Write("fail");
                return;
            }
            Response.Write("success");
        }

        //获得cgamebt支付通知 sign,将该函数返回的值与cgamebt传过来的sign进行比较验证
        public String getSignForAnyValid(HttpRequest request)
        {
            NameValueCollection requestParams = request.QueryString;//获得所有的参数名
            List<String> ps = new List<String>();
            foreach (string key in requestParams)
            {
                ps.Add(key);
            }

            sortParamNames(ps);// 将参数名从小到大排序，结果如：adfd,bcdr,bff,zx

            StringBuilder paramValues = new StringBuilder();
            foreach (string param in ps)
            {//拼接参数值
                if (param == "sign")
                {
                    continue;
                }
                String paramValue = requestParams[param];
                if (paramValue != null)
                {
                    paramValues.AppendFormat("{0}={1}&", param, paramValue);
                }
            }
            string appStr = "06a7144742be9c37b3697e552d9dd7fe";
            paramValues.Append(appStr);
            String md5Values = MD5Encode(paramValues.ToString());
            return md5Values;
        }

        //将参数名从小到大排序，结果如：adfd,bcdr,bff,zx
        public static void sortParamNames(List<String> paramNames)
        {
            paramNames.Sort((String str1, String str2) => { return str1.CompareTo(str2); });
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
    }
}