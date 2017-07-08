using System;
using System.Web;
using System.Collections.Generic;

namespace _360Game
{
    public class IISCheckHandler : IHttpHandler
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此处理程序 
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        /// 

        class CheckResult
        {
            public int errno;
            public string errmsg;
            public CheckData[] data;
        }

        class CheckData
        {
            public string uid;
            public string nickname;
            public string sex;
            public string last_login;
            public int loginlong;
            public int group;
            public string occupation;
            public int level;
            public int is_valid;
            public int exp;
            public long createtime;
        }


        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        static string[] checkOrder = { "uid", "platform", "gkey", "skey", "time" };
        static string checkKey = "TijLKiOeqKGraEOopW8rdSjuimLnldg4";
        public void ProcessRequest(HttpContext context)
        {
            HandlerHelper.log(context, "check_user", context.Request.QueryString["uid"]);
            Dictionary<string, string> data = HandlerHelper.convertParams(context.Request.QueryString);
            bool result = HandlerHelper.checkSign(data, checkOrder, checkKey);
            if (!result)
            {
                WriteResult(context , -2, null);
                return;
            }
            string acc = string.Format("360_{0}", HandlerHelper.UrlDecode(data["uid"]));

            Dictionary<string, object> playerData = MongodbPlayer.Instance.ExecuteGetBykey("player_info", "account", acc);
            if (playerData == null)
            {
                WriteResult(context, -1, null);
            }
            else
            {
                CheckData checkData = new CheckData();
                checkData.uid = data["uid"];
                checkData.nickname = playerData["nickname"].ToString();
                string sex = playerData["sex"].ToString();
                checkData.sex = sex == "1"?"m":"f";
                checkData.last_login = playerData["logout_time"].ToString();
                checkData.loginlong = 1;
                checkData.group = 1;
                checkData.occupation = "";
                checkData.level = 1;
                checkData.is_valid = 1;
                checkData.exp = 1;
                checkData.createtime = 1;
                try
                {
                    DateTime dt = Convert.ToDateTime(playerData["create_time"].ToString());
                    checkData.createtime = dt.Ticks;
                }
                catch
                {

                }
                WriteResult(context, 0, checkData);
            }            
        }

        void WriteResult(HttpContext context, int errno, CheckData checkData)
        {
            CheckResult result = new CheckResult();
            if (errno == 0)
            {
                result.errno = errno;
                result.errmsg = "查询成功";
                result.data = new CheckData[1];
                result.data[0] = checkData;
            }
            else if (errno == -1)
            {
                result.errno = errno;
                result.errmsg = "未创建角色";
            }
            else if (errno == -2)
            {
                result.errno = errno;
                result.errmsg = "参数错误";
            }

            string str = JsonHelper.ConvertToStr(result);
            context.Response.Write(str);
        }

        #endregion
    }
}
