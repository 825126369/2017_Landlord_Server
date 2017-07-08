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

namespace AccountCheck
{
    public partial class th_pay_check : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //playerid
            string account = Request.Params["account"];
            if (string.IsNullOrEmpty(account))
            {
                ReturnMsg("param error");//data error
                return;
            }
            
            List<IMongoQuery> lmq = new List<IMongoQuery>();
            lmq.Add(Query.EQ("userid", account));
            lmq.Add(Query.EQ("Process", false));

            var list = MongodbPayment.Instance.ExecuteGetListByQuery("th_pay", Query.And(lmq), new string[] { "orderid", "currencytype", "currencycount", "vipexp" });
            if (list.Count > 0)
            {
                string ret = string.Empty;
                foreach (var it in list)
                {
                    ret += (" " + it["orderid"].ToString() + "_" + it["currencytype"].ToString() + "_" + it["currencycount"].ToString() + "_" + it["vipexp"].ToString());                   
                }

                ReturnMsg(ret.Trim(), true);
            }
            else
            {
                ReturnMsg("order empty");
            }
        }


        void ReturnMsg(string info, bool bret = false)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;

            if (bret)
                data["data"] = info;
            else
                data["error"] = info;

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsonstr)));
        }
    }
}