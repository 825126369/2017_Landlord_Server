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
    public partial class ex_orderinfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            NameValueCollection req = Request.Form;

            if (req.Count <= 0)
            {
                Response.Write("req.Count <= 0");
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            data["RequestTime"] = DateTime.Now;
            if (!data.ContainsKey("UseKey") || string.IsNullOrEmpty(data["UseKey"].ToString()))
            {
                Response.Write("findn't UseKey");
                return;
            }

            try
            {
                if (MongodbPayment.Instance.KeyExistsBykey("ex_orderinfo", "UseKey", data["UseKey"].ToString()))
                {
                    Response.Write("has UseKey");
                    return;
                }

                MongodbPayment.Instance.ExecuteInsert("ex_orderinfo", data);         
            }
            catch (Exception ex)
            {
                CLOG.Info(ex.ToString());
                Response.Write(ex.ToString());
                return;
            }

            Response.Write("ok");           
        }
    }
}