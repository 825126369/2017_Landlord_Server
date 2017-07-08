using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace HttpThindPart
{
    public partial class CheckUsername : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string username = Request.Params["username"];
            // string sign = Request.Params["sign"];

            if (string.IsNullOrEmpty(username))
            {
                ReturnMsg(99999);
                return;
            }

            string table = "AccountTable";
            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey(table, "acc", username, new string[] { "" });
            if (data != null)
            {
                
                ReturnMsg(10000);
            }
            else
            {
                ReturnMsg(10001);
            }
        }

        void ReturnMsg(int bret = 10000)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;
            data["msg"] = ConfigurationManager.AppSettings["return_" + bret.ToString()];

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(jsonstr);

        }
    }
}