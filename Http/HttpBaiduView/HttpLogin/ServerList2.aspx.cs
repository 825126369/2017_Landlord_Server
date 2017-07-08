using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Configuration;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Text;

namespace HttpLogin
{
    public partial class ServerList2 : System.Web.UI.Page
    {
        const string AES_KEY = "849*@#fwoa&%1k6d";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
            {
                //return_nullxml();
                return;
            }
            //评测服
            string test = Request.Params["test"];

            //测试服
            string channel = Request.Params["channel"];
            string ver = Request.Params["ver"];
            int istest = 0;
            if (!string.IsNullOrEmpty(channel) && !string.IsNullOrEmpty(ver))
            {
                if (ConverVer(ver) >= load_ver() && MongodbConfig.Instance.KeyExistsBykey("TestServers", "channel", channel))
                {
                    istest = 1;
                }
            }
            if (!string.IsNullOrEmpty(test))
            {
                istest = 1;
            }
            if (channel == null)
            {
                channel = "default";
            }

            return_xml(channel, istest);
        }

        string build_xml(string channel, int istest)
        {
            string returnStr = null;
            List<IMongoQuery> lmq = new List<IMongoQuery>();
            lmq.Add(Query.EQ("spread_id", channel));
            lmq.Add(Query.EQ("test", istest));

            var data = MongodbConfig.Instance.ExecuteGetListByQuery("ServerList", Query.And(lmq));
            if (data != null && data.Count > 0)
            {
                XmlDocument docReturn = new XmlDocument();
                XmlElement configReturn = docReturn.CreateElement("ServerList");
                var it = data.GetEnumerator();
                while (it.MoveNext())
                {
                    var it2 = it.Current.GetEnumerator();
                    XmlElement serverNode = docReturn.CreateElement("server");
                    while (it2.MoveNext())
                    {
                        if (it2.Current.Key == "index" || it2.Current.Key == "name" || it2.Current.Key == "ip")
                        {
                            XmlAttribute attrVer = docReturn.CreateAttribute(it2.Current.Key);
                            attrVer.Value = it2.Current.Value.ToString();
                            serverNode.Attributes.Append(attrVer);
                        }
                    }
                    configReturn.AppendChild(serverNode);
                }
                docReturn.AppendChild(configReturn);

                returnStr = encryptUrl(docReturn.InnerXml);
            }
            return returnStr;
        }

        string build_nullxml()
        {
            XmlDocument docReturn = new XmlDocument();
            XmlElement configReturn = docReturn.CreateElement("ServerList");
            docReturn.AppendChild(configReturn);
            string nullstr = encryptUrl(docReturn.InnerXml);
            return nullstr;
        }

        void return_xml(string channel, int istest = 0)
        {
            string key = string.Format("{0}_{1}", channel, istest);

            string serverInfo = null;
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]))
                serverInfo = (string)Cache.Get(key);

            if (serverInfo == null)
            {
                serverInfo = build_xml(channel, istest);
            }
            if (serverInfo == null && channel != "default")
            {
                serverInfo = build_xml("default", istest);
            }
            if (serverInfo == null)
            {
                serverInfo = build_nullxml();
            }
            Cache.Insert(key, serverInfo);
            Response.Write(serverInfo);
        }

        int load_ver()
        {
            object over = Cache.Get("clientver");
            if (over != null)
                return Convert.ToInt32(over);
            try
            {
                XmlDocument docServer = new XmlDocument();
                string path = HttpContext.Current.Server.MapPath("./");
                docServer.Load(path + "\\download\\config.xml");
                XmlNode configServer = docServer.SelectSingleNode("Config");
                string cver = configServer.Attributes["new"].Value;

                int iver = ConverVer(cver);
                Cache["clientver"] = iver;
                return iver;
            }
            catch (Exception)
            {

            }
            return 0;
        }

        int ConverVer(string strVer)
        {
            try
            {
                var vars = strVer.Split('.');
                if (vars.Length == 3)
                {
                    return Convert.ToInt32(vars[0]) * 1000000 +
                        Convert.ToInt32(vars[1]) * 10000 +
                        Convert.ToInt32(vars[2]);
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return 0;
        }

        string encryptUrl(string src)
        {
            string dec = AESHelper.AESEncrypt(src, AES_KEY);
            return Convert.ToBase64String(Encoding.Default.GetBytes(dec));
        }
    }
}