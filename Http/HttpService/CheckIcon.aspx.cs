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
    public partial class CheckIcon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string iconstr = Request.Params["iconstr"];
            string playerid = Request.Params["playerid"];
            string platform = Request.Params["platform"];
            string sacc = Request.Params["acc"];
            if (string.IsNullOrEmpty(playerid) || string.IsNullOrEmpty(platform) || 
                string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(sacc))
            {
                ReturnMsg("data error");//data error
                return;
            }

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                ReturnMsg("error platform");//platform error
                return;
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