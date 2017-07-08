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
    public partial class ServerList3 : System.Web.UI.Page
    {
        const string AES_KEY = "849*@#fwoa&%1k6d";

        protected void Page_Load(object sender, EventArgs e)
        {
            //评测服
            string test = Request.Form["test"];
            string channel = Request.Params["channel"];

            if (!string.IsNullOrEmpty(test) && test == "true" && !string.IsNullOrEmpty(channel))
            {
                if (MongodbConfig.Instance.KeyExistsBykey("TestServers", "channel", channel))
                {
                    return_testxml();
                }
                else
                {
                    return_allxml();
                }
            }
            else
            {
                return_allxml();
            }
            
        }

        string encryptUrl(string src)
        {
            string dec = AESHelper.AESEncrypt(src, AES_KEY);
            return Convert.ToBase64String(Encoding.Default.GetBytes(dec));
        }

        void return_allxml()
        {
            string serlist = null;
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]))
                serlist = (string)Cache.Get("AllServerLuaInfo");

            if (string.IsNullOrEmpty(serlist))
            {
                var ret = MongodbConfig.Instance.ExecuteGetAll("ServerList");
                if (ret != null)
                {
                    StringBuilder sb = new StringBuilder();
                    int i = 1;
                    sb.AppendFormat("serverlist = {{}};");

                    var it = ret.GetEnumerator();
                    while (it.MoveNext())
                    {
                        if (it.Current.ContainsKey("test"))
                        {
                            if (Convert.ToInt32(it.Current["test"]) == 1)
                                continue;
                        }

                        string serverName = null;
                        string serverIP = null;
                        string serverIndex = null;

                        if (it.Current.ContainsKey("name"))
                        {
                            serverName = it.Current["name"].ToString();
                        }
                        if (it.Current.ContainsKey("ip"))
                        {
                            serverIP = it.Current["ip"].ToString();
                        }
                        if (it.Current.ContainsKey("index"))
                        {
                            serverIndex = it.Current["index"].ToString();
                        }
                        if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(serverIP) && !string.IsNullOrEmpty(serverIndex))
                        {
                            sb.AppendFormat("serverlist[{0}] = {{ID={1}, serverName=\"{2}\", serverIP=\"{3}\"}}", i, serverIndex, serverName, serverIP);
                            i++;
                        }
                    }
                    sb.Append("return serverlist;");

                    serlist = encryptUrl(sb.ToString());
                    Cache.Insert("AllServerLuaInfo", serlist);
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
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("serverlist = {{}};");
            sb.Append("return serverlist;");

            string serlist = encryptUrl(sb.ToString());
            Response.Write(serlist);
        }

        void return_testxml()
        {
            string serlist = null;
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]))
                serlist = (string)Cache.Get("TestServerLuaInfo");

            if (string.IsNullOrEmpty(serlist))
            {
                var ret = MongodbConfig.Instance.ExecuteGetAll("ServerList");
                if (ret != null)
                {
                    StringBuilder sb = new StringBuilder();
                    int i = 1;
                    sb.AppendFormat("serverlist = {{}};");

                    var it = ret.GetEnumerator();
                    while (it.MoveNext())
                    {
                        if (it.Current.ContainsKey("test"))
                        {
                            if (Convert.ToInt32(it.Current["test"]) == 1)
                            {
                                string serverName = null;
                                string serverIP = null;
                                string serverIndex = null;

                                if (it.Current.ContainsKey("name"))
                                {
                                    serverName = it.Current["name"].ToString();
                                }
                                if (it.Current.ContainsKey("ip"))
                                {
                                    serverIP = it.Current["ip"].ToString();
                                }
                                if (it.Current.ContainsKey("index"))
                                {
                                    serverIndex = it.Current["index"].ToString();
                                }
                                if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(serverIP) && !string.IsNullOrEmpty(serverIndex))
                                {
                                    sb.AppendFormat("serverlist[{0}] = {{ID={1}, serverName=\"{2}\", serverIP=\"{3}\"}}", i, serverIndex, serverName, serverIP);
                                    i++;
                                }
                            }
                        }
                    }
                    sb.Append("return serverlist;");

                    serlist = encryptUrl(sb.ToString());
                    Cache.Insert("TestServerLuaInfo", serlist);
                }
                else
                {
                    return_nullxml();
                    return;
                }
            }

            Response.Write(serlist);
        }
    }
}