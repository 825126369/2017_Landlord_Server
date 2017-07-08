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
using System.IO;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using System.Security.Cryptography;

namespace HttpRecharge
{
    public partial class miaozm_pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //NameValueCollection req = Request.Form; //获取post类型的参数
            NameValueCollection req = Request.QueryString; //获取get类型的参数
            //Stream s = Request.GetBufferedInputStream();

            if (req.Count <= 0)
            {
                Response.Write("req.Count <= 0");
                Response.Flush();
                return;
            }
            //string dataStr = Encoding.UTF8.GetString(buffer);

            //Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(dataStr);
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            data["log_time"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("third_log", data);

            try
            {
                if (data["sign"].ToString() != getSignForAnyValid(Request))
                {
                    Response.Write("errorSign");
                    return;
                }

                var findacc = MongodbPayment.Instance.ExecuteGetBykey("ex_orderinfo", "UseKey", data["cp_order_no"]);
                if (findacc != null)
                {
                    data["OrderID"] = data["cp_order_no"];
                    data["PlayerId"] = Convert.ToInt32(findacc["PlayerId"].ToString());
                    data["PayCode"] = data["goods_id"];
                    data["Account"] = data["u_id"];
                    //MongodbPayment.Instance.ExecuteRemoveBykey("ex_orderinfo", "UseKey", data["cp_order_no"]);
                }
                
                try
                {
                    data["RMB"] = (int)Convert.ToDouble(data["goods_price"]);
                }
                catch (Exception)
                {
                    data["RMB"] = (int)(Convert.ToInt32(data["goods_price"]));
                }

                if (Convert.ToInt32(data["RMB"]) <= 0)
                {
                    Response.Write("error");
                    return;
                }


                if (MongodbPayment.Instance.KeyExistsBykey("miaozm_pay", "cp_order_no", data["cp_order_no"]))
                {
                    Response.Write("success");
                    return;
                }
                else
                {
                    data["Process"] = false;

                    if (MongodbPayment.Instance.ExecuteInsert("miaozm_pay", data))
                    {
                        save_payinfo("lobby", Convert.ToInt32(data["RMB"]));
                    }
                }
            }
            catch (Exception ex)
            {
                string result = ex.ToString();
                CLOG.Info(result);
                Response.Write("error");
                return;
            }
            Response.Write("success");
        }

        ////获得third支付通知 sign,将该函数返回的值与third传过来的sign进行比较验证
        //public String getSignForAnyValid(Dictionary<string, object> request)
        //{
        //   // NameValueCollection requestParams = request;//获得所有的参数名

        //    Dictionary<string, object> ps = new Dictionary<string, object>();
        //    string appStr = "5875578dc41fd14de";
            
        //    ps = request;
        //    StringBuilder paramValues = new StringBuilder();

        //    paramValues.AppendFormat("{0}+", appStr);
        //    paramValues.AppendFormat("{0}={1}&", "c_id", ps["c_id"]);
        //    paramValues.AppendFormat("{0}={1}&", "cp_order_no", ps["cp_order_no"]);
        //    paramValues.AppendFormat("{0}={1}&", "g_id", ps["g_id"]);
        //    paramValues.AppendFormat("{0}={1}&", "a_id", ps["a_id"]);
        //    paramValues.AppendFormat("{0}={1}&", "u_id", ps["u_id"]);
        //    paramValues.AppendFormat("{0}={1}", "goods_id", ps["goods_id"]);
        //    paramValues.AppendFormat("{0}={1}", "goods_name", ps["goods_name"]);
        //    paramValues.AppendFormat("{0}={1}", "goods_body", ps["goods_body"]);
        //    paramValues.AppendFormat("{0}={1}", "goods_num", ps["goods_num"]);
        //    paramValues.AppendFormat("{0}={1}", "goods_price", ps["goods_price"]);
        //    paramValues.AppendFormat("{0}={1}", "goods_amount", ps["goods_amount"]);

        //    string signkey = paramValues.ToString();

        //    String md5Values = Helper.getMD5(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(signkey)));
        //    return md5Values;
        //}
        //获得anysdk支付通知 sign,将该函数返回的值与any传过来的sign进行比较验证
        public String getSignForAnyValid(HttpRequest request)
        {
            NameValueCollection requestParams = request.QueryString;//获得所有的参数名
            List<String> ps = new List<String>();
            foreach (string key in requestParams)
            {
                ps.Add(key);
            }

            sortParamNames(ps);// 将参数名从小到大排序，结果如：adfd,bcdr,bff,zx

            String paramValues = "5875578dc41fd14de";
            foreach (string param in ps)
            {//拼接参数值
                if (param == "sign")
                {
                    continue;
                }
                String paramValue = requestParams[param];
                if (paramValue != null)
                {
                    paramValues += param.ToString();
                    paramValues += "=";
                    paramValues += paramValue;
                    if (param == "u_id")
                    {
                        break;
                    }
                    paramValues += "&";
                }
            }
            String md5Values = Helper.getMD5(paramValues);
            return md5Values;
        }
        //将参数名从小到大排序，结果如：adfd,bcdr,bff,zx
        public static void sortParamNames(List<String> paramNames)
        {
            paramNames.Sort((String str1, String str2) => { return str1.CompareTo(str2); });
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