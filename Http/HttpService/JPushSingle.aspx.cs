using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cn.jpush.api;
using cn.jpush.api.push;
using cn.jpush.api.report;
using cn.jpush.api.common;
using cn.jpush.api.util;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using cn.jpush.api.common.resp;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace AccountCheck
{
    // 单点极光推送
    public partial class JPushSingle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            // 平台
            string plat = Request.QueryString["plat"];
            if (plat == null)
            {
                Response.Write("platError");
                Response.Flush();
                return;
            }

            // 账号
            string acc = Request.QueryString["acc"];
            if (acc == null)
            {
                Response.Write("accError");
                Response.Flush();
                return;
            }

            string msg = Request.QueryString["msg"];
            if (msg == null)
            {
                Response.Write("msgError");
                Response.Flush();
                return;
            }

            Dictionary<string, object> appData = MongodbAccount.Instance.ExecuteGetBykey("jpushAppInfoList", "plat", plat);
            if (appData == null)
            {
                Response.Write("appError");
                Response.Flush();
                return;
            }

            IMongoQuery imq1 = Query.EQ("plat", BsonValue.Create(plat));
            IMongoQuery imq2 = Query.EQ("acc", BsonValue.Create(acc));
            IMongoQuery imq = Query.And(imq1, imq2);
            Dictionary<string, object> alias = MongodbAccount.Instance.ExecuteGetByQuery("jpushAlias", imq);
            if (alias == null)
            {
                Response.Write("aliasError");
                Response.Flush();
                return;
            }

            try
            {
                string appKey = Convert.ToString(appData["appKey"]);
                string apisec = Convert.ToString(appData["apiSecret"]);
                JPushClient client = new JPushClient(appKey, apisec);
                PushPayload payload = PushObject_all_alias_alert(msg, Convert.ToString(alias["alias"]));
                client.SendPush(payload);
            }
            catch (APIRequestException ex)
            {
            }
            Response.Write(""); 
            Response.Flush();
        }

        public static PushPayload PushObject_all_alias_alert(string msg, string alias)
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.all();
            pushPayload.audience = Audience.s_alias(alias);
            pushPayload.notification = new Notification().setAlert(msg);
            return pushPayload;
        }
    }
}