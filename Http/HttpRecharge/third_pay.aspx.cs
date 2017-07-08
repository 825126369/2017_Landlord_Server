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

namespace HttpRecharge
{
    public partial class third_pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          //  NameValueCollection req = Request.Form;
            Stream s = Request.GetBufferedInputStream();

            byte[] buffer = new byte[s.Length];
            s.Read(buffer, 0, (int)s.Length);
            //string dataStr = Encoding.ASCII.GetString(buffer);
            string dataStr = Encoding.UTF8.GetString(buffer);

            Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(dataStr);

            data["log_time"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("third_log", data);

            try
            {
                if (data["sign"].ToString() != getSignForAnyValid(data))
                {
                    Response.Write("errorSign");
                    return;
                }
                string openSource = "lobby";
                string textInfo = data["attch"].ToString();
                if (string.IsNullOrEmpty(textInfo))
                {
                    Response.Write("error");
                    return;
                }

                string[] extdata = textInfo.Split('-');

                data["OrderID"] = extdata[3];
                data["PlayerId"] = data["playerId"];
                data["PayCode"] = Convert.ToInt32(extdata[1]);
                data["Account"] = extdata[2];

                try
                {
                    data["RMB"] = (int)(Convert.ToDouble(data["price"]));
                }
                catch (Exception)
                {
                    data["RMB"] = (int)(Convert.ToInt32(data["price"]));
                }

                if (Convert.ToInt32(data["RMB"]) <= 0)
                {
                    Response.Write("error");
                    return;
                }
              

                if (MongodbPayment.Instance.KeyExistsBykey("third_pay", "out_trade_no", data["out_trade_no"]))
                {
                    Response.Write("ok");
                    return;
                }
                else
                {
                    data["Process"] = false;

                    if (MongodbPayment.Instance.ExecuteInsert("third_pay", data))
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
            Response.Write("ok");
        }

        //获得third支付通知 sign,将该函数返回的值与third传过来的sign进行比较验证
        public String getSignForAnyValid(Dictionary<string, object> request)
        {
           // NameValueCollection requestParams = request;//获得所有的参数名

            Dictionary<string, object> ps = new Dictionary<string, object>();
            //foreach (string key in requestParams)
            //{
            //    ps.Add(key, requestParams[key]);
            //}
            ps = request;
            StringBuilder paramValues = new StringBuilder();

            if (ps["channel"] == null)
            {
                paramValues.AppendFormat("{0}={1}&", "out_trade_no", ps["out_trade_no"]);
                paramValues.AppendFormat("{0}={1}&", "pay_time", ps["pay_time"]);
                paramValues.AppendFormat("{0}={1}&", "payway", ps["payway"]);
                paramValues.AppendFormat("{0}={1}&", "playerId", ps["playerId"]);
                paramValues.AppendFormat("{0}={1}&", "price", ps["price"]);
                paramValues.AppendFormat("{0}={1}", "text", ps["text"]);

                //string appStr = "9ed55d152f262e1feb19bed06207f3c4";
                // paramValues.Append(appStr);
            }
            else
            {
                paramValues.AppendFormat("{0}={1}&", "attch", ps["attch"]);
                paramValues.AppendFormat("{0}={1}&", "channel", ps["channel"]);
                paramValues.AppendFormat("{0}={1}&", "out_trade_no", ps["out_trade_no"]);
                paramValues.AppendFormat("{0}={1}&", "pay_time", ps["pay_time"]);
                paramValues.AppendFormat("{0}={1}&", "payway", ps["payway"]);
                paramValues.AppendFormat("{0}={1}&", "playerId", ps["playerId"]);
                paramValues.AppendFormat("{0}={1}&", "price", ps["price"]);
                paramValues.AppendFormat("{0}={1}", "text", ps["text"]);
            }
            string signkey = paramValues.ToString();

            String md5Values = Helper.getMD5(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(signkey)));
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