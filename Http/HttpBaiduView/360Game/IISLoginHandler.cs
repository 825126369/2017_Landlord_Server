using System;
using System.Web;
using System.Collections.Generic;
namespace _360Game
{
    public class IISLoginHandler : IHttpHandler
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此处理程序 
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }
        static string[] checkOrder = { "uid", "platform", "gkey", "skey", "time", "is_adult", "exts"};
        static string checkKey = "TijLKiOeqKGraEOopW8rdSjuimLnldg4";
        public void ProcessRequest(HttpContext context)
        {
            HandlerHelper.log(context, "login", context.Request.QueryString["uid"]);

            Dictionary<string, string> data = HandlerHelper.convertParams(context.Request.QueryString);
            bool result = HandlerHelper.checkSign(data, checkOrder, checkKey);
            if (!result)
                return;

            Random rd = new Random();
            DateTime now = DateTime.Now;
            int randkey = rd.Next();

            Dictionary<string, object> savedata = new Dictionary<string, object>();
            string acc = string.Format("360_{0}", HandlerHelper.UrlDecode(data["uid"]));
            string remoteIP = HandlerHelper.getRemoteIP(context);
            string channelID = "900002";
            
            savedata["randkey"] = randkey;
            savedata["updatepwd"] = false;
            savedata["lasttime"] = now.Ticks;
            savedata["lastip"] = remoteIP;

            if (MongodbAccount.Instance.KeyExistsBykey("AccountTable", "acc", acc) == false)
            {
                savedata["acc"] = acc;
                savedata["platform"] = "360";
                savedata["channelID"] = channelID;
                savedata["regedittime"] = now;
                savedata["regeditip"] = remoteIP;

                MongodbAccount.Instance.ExecuteStoreBykey("AccountTable", "acc", acc, savedata);

                Dictionary<string, object> registerLog = new Dictionary<string, object>();
                registerLog["acc"] = acc;
                registerLog["acc_real"] = acc;
                registerLog["ip"] = remoteIP;
                registerLog["time"] = now;
                registerLog["channel"] = channelID;

                MongodbAccount.Instance.ExecuteInsert("RegisterLog", registerLog);
            }
            else
            {
                MongodbAccount.Instance.ExecuteUpdate("AccountTable", "acc", acc, savedata);
            }

            Dictionary<string, object> loginLog = new Dictionary<string, object>();
            loginLog["acc"] = acc;
            loginLog["acc_real"] = acc;
            loginLog["ip"] = remoteIP;
            loginLog["time"] = now;
            loginLog["channel"] = channelID;

            MongodbAccount.Instance.ExecuteInsert("LoginLog", loginLog);
        }

        #endregion
    }
}
