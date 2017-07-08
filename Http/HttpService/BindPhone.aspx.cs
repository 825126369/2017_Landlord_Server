using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace AccountCheck
{
    public partial class BindPhone : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string sacc = Request.Params["acc"];
            string phone = Request.Params["phone"];
            string platform = Request.Params["platform"];

            if (string.IsNullOrEmpty(sacc) || phone!=null || string.IsNullOrEmpty(platform))            
                return;

            string acckey = ConfigurationManager.AppSettings["acckey_default"];
            if (string.IsNullOrEmpty(acckey))
            {
                acckey = "acc";
            }

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                return;
            }

            if (MongodbAccount.Instance.KeyExistsBykey(table, acckey, sacc))
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["bindPhone"] = phone;
                MongodbAccount.Instance.ExecuteUpdate(table, acckey, sacc, data);
            }         
        }
    }
}