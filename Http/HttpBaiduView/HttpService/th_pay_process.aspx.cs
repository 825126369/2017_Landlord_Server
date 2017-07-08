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
    public partial class th_pay_process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string account = Request.Params["account"];
            string orderid = Request.Params["orderid"];
            if (string.IsNullOrEmpty(orderid) || string.IsNullOrEmpty(account))
            {
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Process"] = true;
            data["UpdateTime"] = DateTime.Now;
            List<IMongoQuery> lmq = new List<IMongoQuery>();
            lmq.Add(Query.EQ("orderid", orderid));
            lmq.Add(Query.EQ("userid", account));

            MongodbPayment.Instance.ExecuteUpdateByQuery("th_pay", Query.And(lmq), data);
        }
    }
}