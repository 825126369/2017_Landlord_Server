using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HttpLogin
{
    public partial class ErrorReport : System.Web.UI.Page
    {
        JavaScriptSerializer JsonHelper = new JavaScriptSerializer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string strver = Request.Params["ver"];
            string strgame = Request.Params["game"];
            string strerr = Server.UrlDecode(Request.Params["error"]);

            if (string.IsNullOrEmpty(strver) || string.IsNullOrEmpty(strgame) || string.IsNullOrEmpty(strerr))
            {                    
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["ver"] = strver;
            data["game"] = strgame;
            data["error"] = strerr;
            data["time"] = DateTime.Now;

            MongodbConfig.Instance.ExecuteInsert("Errors", data);
            
        }
    }
}