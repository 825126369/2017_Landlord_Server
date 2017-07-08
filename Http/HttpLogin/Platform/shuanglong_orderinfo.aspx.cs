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

namespace HttpLogin.Platform
{
    public partial class shuanglong_orderinfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            NameValueCollection req = Request.Form;

            if (req.Count <= 0)
            {
                ReturnLuaMsg("req.Count <= 0", false);
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            data["RequestTime"] = DateTime.Now;
            //if (!data.ContainsKey("UseKey") || string.IsNullOrEmpty(data["UseKey"].ToString()))
            //{
            //    Response.Write("findn't UseKey");
            //    return;
            //}
            
            DateTime curTime = DateTime.Now;
            string playerID = Convert.ToString(data["PlayerId"]);
            string orderID = string.Format("{0:D2}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6}",
                curTime.Year, curTime.Month, curTime.Day, curTime.Hour, curTime.Minute, curTime.Second, playerID);

            data["OrderID"] = orderID;
            try
            {
                if (MongodbPayment.Instance.KeyExistsBykey("shuanglong_orderinfo", "OrderID", orderID))
                {
                    ReturnLuaMsg("has OrderID", false);
                    return;
                }
                MongodbPayment.Instance.ExecuteInsert("shuanglong_orderinfo", data); 
            }
            catch (Exception ex)
            {
                CLOG.Info(ex.ToString());
                ReturnLuaMsg(ex.ToString(), false);
                return;
            }
            ReturnLuaMsg(orderID, true);
        }

        void ReturnLuaMsg(string info, bool ret = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ret = {};");
            sb.AppendFormat("ret.result = {0};", ret.ToString().ToLower());
            sb.AppendFormat("ret.data = \"{0}\";", info);
            sb.Append("return ret;");
            Response.Write(sb.ToString());
        }
    }
}