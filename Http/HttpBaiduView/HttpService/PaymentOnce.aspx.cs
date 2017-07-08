using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace PaymentCheck
{
    public partial class PaymentOnce : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //account
            //orderid
            //platform
            string platform = Request.Params["platform"];
            string orderid = Request.Params["orderid"];
            string account = Request.Params["account"];
            if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(account)
                || string.IsNullOrEmpty(orderid))
            {
                ReturnMsg("data error");//data error
                return;
            }

            string table = ConfigurationManager.AppSettings["pay_"+platform];
            if (string.IsNullOrEmpty(table))
            {
                ReturnMsg("error platform");//platform error
                return;
            }

            string splayerid = Request.Params["playerid"];
            if (string.IsNullOrEmpty(splayerid))
                splayerid = "0";

            int playerid = 0;

            try
            {
                playerid = Convert.ToInt32(splayerid);
            }
            catch (Exception)
            {

            }

            if (splayerid == "0")
            {
                splayerid = account;
            }

            List<IMongoQuery> lmq = new List<IMongoQuery>();
            //百度视频PC支付没有playerid
            //if (platform == "baiduview")
            //{
                lmq.Add(Query.EQ("Account", account));
            //}
            //else
            //{
            //    if (playerid > 0)
            //        lmq.Add(Query.EQ("PlayerId", playerid));
            //    else
            //        lmq.Add(Query.EQ("Account", account));
            //}
            lmq.Add(Query.EQ("OrderID", orderid));
            lmq.Add(Query.EQ("Process", false));

            var one = MongodbPayment.Instance.ExecuteGetByQuery(table, Query.And(lmq), new string[] { "PayCode", "RMB", "Custom" });
            if (one != null)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["Process"] = true;
                data["UpdateTime"] = DateTime.Now;
                if (playerid > 0)
                    data["PlayerId"] = playerid;
                string err = MongodbPayment.Instance.ExecuteUpdate(table, "OrderID", orderid, data);

                string rmb = one["RMB"].ToString();

                string custom = "0";
                if (one.ContainsKey("Custom"))
                    custom = one["Custom"].ToString();

                if (err == string.Empty)
                {
                    ReturnMsg(one["PayCode"].ToString() + " " + rmb + " " + orderid + " " + custom, platform, true);
                    ExceptionCheckInfo.doSaveCheckInfo(Request, "recharge");

                    TalkingGame tg = new TalkingGame();
                    tg.adddata(splayerid, orderid, one["RMB"].ToString(), one["PayCode"].ToString());
                    tg.PostToTG();                   
                }
                else
                    ReturnMsg(err);
            }
            else
            {
                ReturnMsg("can't find orderid");//需要返回payid
            }         
        }        


        void ReturnMsg(string info, string plm = "", bool bret = false)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;
            
            if (bret)
                data["platform"] = plm;

            if (bret)
                data["data"] = info;
            else
                data["error"] = info;

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsonstr)));
        }
    }
}