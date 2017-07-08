using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;
using Common;
using YSDK;

namespace HttpRecharge
{
    public partial class ysdk_pay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, object> saveData = new Dictionary<string, object>();
                int amount = 0;
                try
                {
                    amount = (int)Convert.ToDouble(Request.Params["amount"]);
                }
                catch (Exception)
                {
                    amount = Convert.ToInt32(Request.Params["amount"]);
                }
                saveData["RMB"] = amount;
                if (amount == 0)
                {
                    Response.Write(Helper.buildLuaReturn(-2, "param error"));
                    return;
                }
                int pay_amount = amount * 10;
                saveData["OrderID"] = Request.Params["orderid"];
                saveData["Account"] = Request.Params["acount"];
                saveData["PayCode"] = Request.Params["paycode"];
                saveData["Process"] = false;
                saveData["IsPay"] = false;
                saveData["PayTime"] = DateTime.Now;
                saveData["PlayerId"] = Convert.ToInt32(Request.Params["playerid"]);
                saveData["Channel"] = Request.Params["channel"];

                string[] strs = Request.Params["ysdkdata"].ToString().Trim().Split(':');
                if (strs.Length < 5)
                {
                    Response.Write(Helper.buildLuaReturn(-2, "param error"));
                    return;
                }

                Dictionary<string, object> payData = MongodbPayment.Instance.ExecuteGetBykey("ysdk_log", "OrderID", saveData["OrderID"]);
                if (payData == null)
                {
                    MongodbPayment.Instance.ExecuteInsert("ysdk_log", saveData);
                }
                else
                {
                    bool isPay = (bool)payData["IsPay"];
                    if (isPay == true)
                    {
                        Response.Write(Helper.buildLuaReturn(-1, "is pay"));
                        return;
                    }
                }

                string platform = strs[0];
                string openid = strs[1];
                string openkey = strs[2];
                string pf = strs[3];
                string pfkey = strs[4];

                string mode = "GET";
                YSDKHelper ysdkHelper = new YSDKHelper();
                ysdkHelper.initKeyValue(openid, openkey, pf, pfkey);
                ysdkHelper.addKeyValue("userip", Helper.GetWebClientIp());
                ysdkHelper.addKeyValue("amt", pay_amount.ToString());
                ysdkHelper.addKeyValue("billno", saveData["OrderID"].ToString());

                string url = ysdkHelper.buildURL(YSDKMethod.Pay);
                Uri uri = new Uri(url);
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                request.Method = mode;
                request.Headers.Add("cookie", ysdkHelper.buildCookie(platform, YSDKMethod.Pay));

                HttpWebResponse responser = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(responser.GetResponseStream(), Encoding.UTF8);
                string msg = reader.ReadToEnd();

                PayResult result = JsonHelper.ParseFromStr<PayResult>(msg);
                if (result.ret == 0)
                {
                    saveData["IsPay"] = true;

                    MongodbPayment.Instance.ExecuteUpdate("ysdk_log", "OrderID", saveData["OrderID"], saveData);
                    MongodbPayment.Instance.ExecuteInsert("ysdk_pay", saveData);

                    Dictionary<string, object> savelog = new Dictionary<string, object>();
                    savelog["acc"] = saveData["Account"];
                    savelog["real_acc"] = saveData["Account"];
                    savelog["acc_dev"] = Request.Params["device"];
                    savelog["time"] = DateTime.Now;
                    savelog["channel"] = saveData["Channel"].ToString();
                    savelog["rmb"] = saveData["RMB"];
                    MongodbPayment.Instance.ExecuteInsert("PayLog", savelog);

                    Response.Write(Helper.buildLuaReturn(0, ""));
                }
                else
                {
                    Response.Write(Helper.buildLuaReturn(result.ret, result.msg));
                }
            }
            catch (Exception error)
            {
                Response.Write(Helper.buildLuaReturn(-1000, error.Message));
            }
        }
    }
}