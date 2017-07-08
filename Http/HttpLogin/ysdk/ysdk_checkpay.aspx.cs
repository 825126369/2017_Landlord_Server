using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;
using YSDK;
using Common;

namespace HttpRecharge
{
    public partial class ysdk_checkpay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string[] strs = Request.Params["ysdkdata"].ToString().Trim().Split(':');
                if (strs.Length < 5)
                {
                    Response.Write(Helper.buildLuaReturn(-2, "param error"));
                    return;
                }

                string platform = strs[0];
                string openid = strs[1];
                string openkey = strs[2];
                string pf = strs[3];
                string pfkey = strs[4];

                int pay_amount = Convert.ToInt32(Request.Params["amount"])*10;

                string mode = "GET";
                YSDKHelper ysdkHelper = new YSDKHelper();
                ysdkHelper.initKeyValue(openid, openkey, pf, pfkey);
                ysdkHelper.addKeyValue("userip", Helper.GetWebClientIp());

                string url = ysdkHelper.buildURL(YSDKMethod.Get_Balance);
                Uri uri = new Uri(url);
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                request.Method = mode;
                request.Headers.Add("cookie", ysdkHelper.buildCookie(platform, YSDKMethod.Get_Balance));

                HttpWebResponse responser = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(responser.GetResponseStream(), Encoding.UTF8);
                string msg = reader.ReadToEnd();

                BalanceResult result = JsonHelper.ParseFromStr<BalanceResult>(msg);
                if (result.ret == 0)
                {
                    if (result.balance >= pay_amount)
                    {
                        Response.Write(Helper.buildLuaReturn(0, ""));
                    }
                    else
                    {
                        Response.Write(Helper.buildLuaReturn(-1, "need recharge"));
                    }
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