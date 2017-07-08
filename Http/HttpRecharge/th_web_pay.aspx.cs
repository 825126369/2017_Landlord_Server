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

namespace HttpRecharge
{
    public partial class th_web_pay : System.Web.UI.Page
    {
        static string secretKey = "4E18F5EBF3E44FE3BF4C8ADCF0075D91";

        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.QueryString;

            if (req.Count <= 0)
            {
                Response.Write("param error");
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            if (!data.ContainsKey("orderid") || !data.ContainsKey("userid") || !data.ContainsKey("currencytype")
                || !data.ContainsKey("currencycount") || !data.ContainsKey("vipexp") || !data.ContainsKey("sign"))
            {
                Response.Write("param error");
                return;
            }


            string url = "orderid=" + data["orderid"].ToString() + "&userid=" + data["userid"].ToString() + "&currencytype=" +
                data["currencytype"].ToString() + "&currencycount=" + data["currencycount"].ToString() + "&vipexp=" + data["vipexp"].ToString();

            string checkmd5 = url + "&secretKey=" + secretKey;
            checkmd5 = AESHelper.MD5Encrypt(checkmd5);
            if (checkmd5 != data["sign"].ToString())
            {
                Response.Write("check error");
                return;
            }

            if (MongodbPayment.Instance.KeyExistsBykey("th_pay", "orderid", data["orderid"].ToString()))
            {
                Response.Write("orderid exist");                
                return;
            }

            data["PayTime"] = DateTime.Now;
            data["Process"] = false;
            data.Remove("sign");

            if (!MongodbPayment.Instance.ExecuteInsert("th_pay", data))
            {
                Response.Write("db error");
                return;
            }


            string server_api = "http://"+ ConfigurationManager.AppSettings["server_api"].ToString() +"/cmd=1&"+url;

            try
            {
                var ret = HttpPost.Post(new Uri(server_api));
                if (ret != null)
                {
                    string retstr = Encoding.UTF8.GetString(ret);
                    Response.Write(retstr);
                    return;
                }
            }
            catch (Exception)
            {
                //投递失败 玩家下次登录可以检测充值
            }           

            Response.Write("success");
        }
    }
}