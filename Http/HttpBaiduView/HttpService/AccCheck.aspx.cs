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

namespace HttpLogin.Default
{
    public partial class AccCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string sacc = Request.Params["acc"];
            string platform = Request.Params["platform"];

            if (string.IsNullOrEmpty(sacc) || string.IsNullOrEmpty(platform))
            {
                ReturnMsg("data error");  
                return;
            }

            string table = ConfigurationManager.AppSettings["acc_"+platform];
            if (string.IsNullOrEmpty(table))
            {
                ReturnMsg("error platform");//platform error
                return;
            }

            //List<IMongoQuery> imqs = new List<IMongoQuery>();
            //imqs.Add(Query.EQ("acc", sacc));

            string acckey = ConfigurationManager.AppSettings["acckey_" + platform];
            if (string.IsNullOrEmpty(acckey))
            {
                acckey = "acc";
            }

            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey(table, acckey, sacc, new string[] { "randkey", "lasttime" });
            if (data != null && data.Count>=2)
            {
                string jsonstr = data["randkey"].ToString() + "_" + data["lasttime"].ToString();     //JsonHelper.ConvertToStr(data);
                ReturnMsg(jsonstr.Trim(), true);
                ExceptionCheckInfo.doSaveCheckInfo(Request, "login");
            }
            else
            {
                ReturnMsg("db error");             
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