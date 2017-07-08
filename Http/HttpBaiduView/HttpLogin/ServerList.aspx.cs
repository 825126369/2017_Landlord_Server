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

namespace HttpLogin
{
    public partial class ServerList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
            {
                //return_nullxml();
                return;
            }

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

                if (ConverVer(ver)>= load_ver() && MongodbConfig.Instance.KeyExistsBykey("TestServers", "channel", channel))
                {
                    return_testxml();
                    return;
                }
            }

            XmlDocument docReturn = null;

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]))
                docReturn = (XmlDocument)Cache.Get("allserver");

            if (docReturn == null)
            {
                var ret = MongodbConfig.Instance.ExecuteGetAll("ServerList");
                if (ret != null)
                {
                    docReturn = new XmlDocument();
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

                    Cache.Insert("allserver", docReturn);
                }
                else
                {
                    return_nullxml();
                    return;
                }
            }
           
            Response.Write(Server.UrlEncode(docReturn.InnerXml));  
        }

        void return_nullxml()
        {
            //创建空文件列表
            XmlDocument docReturn = (XmlDocument)Cache.Get("nullserver");
            if (docReturn == null)
            {
                docReturn = new XmlDocument();
                XmlElement configReturn = docReturn.CreateElement("ServerList");
                docReturn.AppendChild(configReturn);
                Cache.Insert("nullserver", docReturn);
            }

            Response.Write(Server.UrlEncode(docReturn.InnerXml));
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
                Response.Write(Server.UrlEncode(docReturn.InnerXml));
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
    }
}