using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace HttpLogin
{
    public partial class ActivationInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string strimei = Request.Params["imei"];
            string strchannel = Request.Params["channel"];
            if (string.IsNullOrEmpty(strimei) || string.IsNullOrEmpty(strchannel))
            {
                return;
            }

            //渠道每日激活
            if (MongodbAccount.Instance.KeyExistsBykey("link_phone", "phone", strimei))
                return;

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["phone"] = strimei;
            MongodbAccount.Instance.ExecuteInsert("link_phone", data);
            MongodbAccount.Instance.ExecuteIncBykey("day_activation", "date", DateTime.Now.Date, strchannel, 0);
        }
    }
}