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
            CLOG.Info("服务器列表。。。： ServerList22222222222222");

            //评测服
            string test = Request.Params["test"];
            if (!string.IsNullOrEmpty(test))
            {
                return_testxml();
                return;
            }

            //测试服
            string channel = Request.Params["channel"];
            string ver = Request.Params["ver"];
            if (!string.IsNullOrEmpty(channel) && !string.IsNullOrEmpty(ver))
            {
                //List<IMongoQuery> imq = new List<IMongoQuery>();
                //imq.Add(Query.EQ("channel", BsonValue.Create(channel)));
                //imq.Add(Query.EQ("isTest", BsonValue.Create(1)));
                //if (MongodbConfig.Instance.ExecuteGetCount("TestServers", Query.And(imq)) > 0)

                if (ConverVer(ver) >= load_ver() && MongodbConfig.Instance.KeyExistsBykey("TestServers", "channel", channel))
                {
                    return_testxml();
                    return;
                }
            }

            string serlist = "";

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]))
                serlist = (string)Cache.Get("allserver");

            if (string.IsNullOrEmpty(serlist))
            {
                var ret = MongodbConfig.Instance.ExecuteGetAll("ServerList");
                if (ret != null)
                {
                    XmlDocument docReturn = new XmlDocument();
                    XmlElement configReturn = docReturn.CreateElement("ServerList");
                    var it = ret.GetEnumerator();
                    while (it.MoveNext())
                    {
                        if (it.Current.ContainsKey("test"))
                        {
                            if (Convert.ToInt32(it.Current["test"]) == 1)
                                continue;
                        }

                        var it2 = it.Current.GetEnumerator();
                        XmlElement serverNode = docReturn.CreateElement("server");
                        while (it2.MoveNext())
                        {
                            if (it2.Current.Key == "_id")
                                continue;

                            XmlAttribute attrVer = docReturn.CreateAttribute(it2.Current.Key);
                            attrVer.Value = it2.Current.Value.ToString();
                            serverNode.Attributes.Append(attrVer);
                        }
                        configReturn.AppendChild(serverNode);
                    }
                    docReturn.AppendChild(configReturn);

                    serlist = encryptUrl(docReturn.InnerXml);
                    Cache.Insert("allserver", serlist);
                }
                else
                {
                    return_nullxml();
                    return;
                }
            }

            Response.Write(serlist);
        }

        void return_nullxml()
        {
            //创建空文件列表
            string nullstr = (string)Cache.Get("nullserver");
            if (string.IsNullOrEmpty(nullstr))
            {
                XmlDocument docReturn = new XmlDocument();
                XmlElement configReturn = docReturn.CreateElement("ServerList");
                docReturn.AppendChild(configReturn);
                nullstr = encryptUrl(docReturn.InnerXml);
                Cache.Insert("nullserver", nullstr);
            }

            Response.Write(nullstr);
        }

        void return_testxml()
        {
            var data = MongodbConfig.Instance.ExecuteGetBykey("ServerList", "test", 1);

            if (data != null)
            {
                XmlDocument docReturn = new XmlDocument();
                XmlElement configReturn = docReturn.CreateElement("ServerList");

                XmlAttribute attrtest = docReturn.CreateAttribute("isTest");
                attrtest.Value = "true";
                configReturn.Attributes.Append(attrtest);

                XmlElement serverNode = docReturn.CreateElement("server");
                var it2 = data.GetEnumerator();
                while (it2.MoveNext())
                {
                    if (it2.Current.Key == "_id")
                        continue;

                    XmlAttribute attrVer = docReturn.CreateAttribute(it2.Current.Key);
                    attrVer.Value = it2.Current.Value.ToString();
                    serverNode.Attributes.Append(attrVer);
                }
                configReturn.AppendChild(serverNode);


                docReturn.AppendChild(configReturn);
                Response.Write(encryptUrl(docReturn.InnerXml));
            }
            else
            {
                return_nullxml();
            }
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