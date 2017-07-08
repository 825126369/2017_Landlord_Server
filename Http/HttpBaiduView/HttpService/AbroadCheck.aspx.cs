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

using System.Net;
using System.IO;

namespace AccountCheck
{
    public partial class AbroadCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string sacc = Request.Params["acc"];

            if (string.IsNullOrEmpty(sacc))
            {
                ReturnMsg("data error");
                return;
            }

            string table = ConfigurationManager.AppSettings["acc_default"];
            if (string.IsNullOrEmpty(table))
            {
                ReturnMsg("error platform");//platform error
                return;
            }

            //List<IMongoQuery> imqs = new List<IMongoQuery>();
            //imqs.Add(Query.EQ("acc", sacc));

            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey(table, "acc", sacc, new string[] { "randkey", "lasttime" });
            if (data != null && data.Count >= 2)
            {
                string jsonstr = data["randkey"].ToString() + "_" + data["lasttime"].ToString();     //JsonHelper.ConvertToStr(data);   
                string err = "";
                AccInfo accInfo = new AccInfo();

                int money = GetAbroadMoney(sacc, ref err, accInfo);
                if (money >= 0)
                {
                    ReturnMsg(jsonstr.Trim(), true, money, accInfo);
                    ExceptionCheckInfo.doSaveCheckInfo(Request, "login");
                }
                else
                {
                    ReturnMsg(err, false);
                }                    
            }
            else
            {
                ReturnMsg("db error");
            }
        }

        void ReturnMsg(string info, bool bret = false, int money = 0, AccInfo accInfo = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;
            if (bret)
            {
                data["data"] = info;
                data["money"] = money;
                data["isApi"] = accInfo.m_isApi ? 1 : 0;
                data["home"] = accInfo.m_home;
                data["creator"] = accInfo.m_creator;
                data["gameClose"] = accInfo.m_gameClose;
            }
            else
                data["error"] = info;

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsonstr)));
        }

        //获取金币
        int GetAbroadMoney(string acc, ref string retstr, AccInfo accInfo)
        {
            int money = -1;
            try
            {
                string strurl = ConfigurationManager.AppSettings["http_abroad"] + "?acc="+acc;
                string retb64 = Encoding.Default.GetString(HttpPost.Get(new Uri(strurl)));
                string retstring = Encoding.Default.GetString(Convert.FromBase64String(retb64));
                Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(retstring);
                if (data["result"].ToString() == "0")
                {
                    money = Convert.ToInt32(data["money"]);
                    accInfo.m_isApi = Convert.ToBoolean(data["isApi"]);
                    accInfo.m_home = Convert.ToString(data["home"]);
                    accInfo.m_creator = Convert.ToString(data["creator"]);
                    accInfo.m_gameClose = Convert.ToString(data["gameClose"]);
                }
                else
                {
                    retstr = "result:" + data["result"].ToString();
                }
            }
            catch (Exception ex)
            {
				CLOG.Info(ex.ToString());                
                money = -1;
                retstr = "GetAbroadMoney_error";
            }

            return money;
        }
    }

    public class AccInfo
    {
        public bool m_isApi = false;
        public string m_home = "";
        public string m_creator = "";
        public string m_gameClose = "";
    }
}