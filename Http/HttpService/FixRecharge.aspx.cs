using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace AccountCheck
{
    public partial class FixRecharge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //account
            //orderid
            //platform
            string platform = Request.Params["platform"];
            string orderid = Request.Params["orderid"];
            if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(orderid))
            {               
                return;
            }

            string table = ConfigurationManager.AppSettings["pay_" + platform];
            if (string.IsNullOrEmpty(table))
            {             
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Process"] = false;
            string err = MongodbPayment.Instance.ExecuteUpdate(table, "OrderID", orderid, data);
            if (!string.IsNullOrEmpty(err))
                Response.Write(err);            
        }
    }
}