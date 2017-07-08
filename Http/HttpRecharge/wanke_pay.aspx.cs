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
    public partial class wanke_pay : System.Web.UI.Page
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

            data["CreateTime"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("wanke_log", data);

            try
            {
                if (data["sign"].ToString() != getSignForAnyValid(Request))
                {
                    Response.Write("errorSign");
                    return;
                }
                string openSource = "lobby";
                string extInfo = data["attach"].ToString();
                if (string.IsNullOrEmpty(extInfo))
                {
                    Response.Write("error");
                    return;
                }

                string[] extdata = extInfo.Split('-');
                if (extdata.Length < 5)
                {
                    Response.Write("error");
                    return;
                }
                data["OrderID"] = extdata[0];
                data["Account"] = extdata[1];
                data["PlayerId"] = Convert.ToInt32(extdata[2]);
                data["PayCode"] = extdata[3];
                openSource = extdata[4];

                try
                {
                    data["RMB"] = (int)(Convert.ToDouble(data["amount"]));
                }
                catch (Exception)
                {
                    data["RMB"] = (int)(Convert.ToInt32(data["amount"]));
                }

                if (Convert.ToInt32(data["RMB"]) <= 0)
                {
                    Response.Write("error");
                    return;
                }

                if (MongodbPayment.Instance.KeyExistsBykey("wanke_pay", "orderid", data["orderid"]))
                {
                    Response.Write("success");
                    return;
                }
                else
                {
                    data["Process"] = false;

                    if (MongodbPayment.Instance.ExecuteInsert("wanke_pay", data))
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

        //获得wanke支付通知 sign,将该函数返回的值与wanke传过来的sign进行比较验证
        public String getSignForAnyValid(HttpRequest request)
        {
            NameValueCollection requestParams = request.Form;//获得所有的参数名

            Dictionary<string, string> ps = new Dictionary<string, string>();
            foreach (string key in requestParams)
            {
                ps.Add(key, requestParams[key]);
            }

            StringBuilder paramValues = new StringBuilder();
            paramValues.AppendFormat("{0}={1}&", "orderid", ps["orderid"]);
            paramValues.AppendFormat("{0}={1}&", "username", ps["username"]);
            paramValues.AppendFormat("{0}={1}&", "gameid", ps["gameid"]);
            paramValues.AppendFormat("{0}={1}&", "roleid", ps["roleid"]);
            paramValues.AppendFormat("{0}={1}&", "serverid", ps["serverid"]);
            paramValues.AppendFormat("{0}={1}&", "paytype", ps["paytype"]);
            paramValues.AppendFormat("{0}={1}&", "amount", ps["amount"]);
            paramValues.AppendFormat("{0}={1}&", "paytime", ps["paytime"]);
            paramValues.AppendFormat("{0}={1}&", "attach", ps["attach"].ToString());

            string appStr = "appkey=3f2fadb37dd503fe686cdfb33ab8c095";
            paramValues.Append(appStr);

            string signkey = paramValues.ToString().ToLower();
            String md5Values = Helper.getMD5(signkey);
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