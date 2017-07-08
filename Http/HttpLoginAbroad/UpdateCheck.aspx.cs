using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.Caching;

namespace AppUpdate
{
    public partial class UpdateCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();

            

            //if (Request.RequestType == "GET")
            {
                //客户端段出来的版本号1.2.256
                string strClientVersion = Request.Params["ClientVersion"];
                if (string.IsNullOrEmpty(strClientVersion))
                    return;

                string filename = "server";
                string gamename = Request.Params["GameName"];
                string cachekey = strClientVersion;
                if (!string.IsNullOrEmpty(gamename))
                {
                    filename = filename + "_" + gamename;
                    cachekey = cachekey + "_" + gamename;
                }
                filename += ".xml";
                         
                //获取已有对比文件
                XmlDocument docReturn = (XmlDocument)Cache.Get(cachekey);
                if (docReturn != null)
                {
                    Response.Write(docReturn.InnerXml);
                    Response.Flush();
                    return;
                }

                //版本保护
                int nVerClient = ConverVer(strClientVersion);
                string oVer = (string)Cache.Get("ServerVer" + gamename);
                if (oVer != null)
                {                    
                    int nVerServer = ConverVer(oVer);
                    if (nVerServer <= nVerClient)
                    {
                        docReturn = get_nullxml(oVer);
                        Response.Write(docReturn.InnerXml);
                        Response.Flush();
                        return;
                    }
                }
               
                string strVer = "";
                List<ResfileInfo> ResList = GetServerXml(ref strVer, filename, gamename, (oVer == null));

                if (ResList.Count == 0 || strVer == "")
                {
                    docReturn = get_nullxml(strClientVersion);
                    Response.Write(docReturn.InnerXml);
                    Response.Flush();
                    return;
                }
                else
                {
                    docReturn = new XmlDocument();
                    XmlElement configReturn = docReturn.CreateElement("Config");
                    XmlAttribute sVer = docReturn.CreateAttribute("ver");
                    sVer.Value = strVer;
                    configReturn.Attributes.Append(sVer);
                    docReturn.AppendChild(configReturn);

                    foreach(var item in ResList)
                    {
                        if(nVerClient >= item.Index)
                            break;

                        XmlNode nodeNew = docReturn.CreateElement("File");
                        XmlAttribute attrPath = docReturn.CreateAttribute("path");
                        XmlAttribute attrMd5 = docReturn.CreateAttribute("md5");
                        XmlAttribute attrVer = docReturn.CreateAttribute("ver");
                        attrPath.Value = item.Path;
                        attrMd5.Value = item.MD5;
                        attrVer.Value = item.Ver;
                        nodeNew.Attributes.Append(attrPath);
                        nodeNew.Attributes.Append(attrMd5);
                        nodeNew.Attributes.Append(attrVer);
                        configReturn.AppendChild(nodeNew);
                    }

                    Cache[cachekey] = docReturn;
                    Response.Write(docReturn.InnerXml);
                    Response.Flush();
                }
            }
        }

        protected struct ResfileInfo
        {
            public int Index { get; set; }

            public string Path { get; set; }
            public string MD5 { get; set; }
            public string Ver { get; set; }
        }

        protected static int CompareResfileInfo(ResfileInfo x, ResfileInfo y)
        {
            if (x.Index < y.Index)
                return 1;
            else if (x.Index == y.Index)
                return 0;
            else
                return -1;
        }

        protected int ConverVer(string strVer)
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

        protected List<ResfileInfo> GetServerXml(ref string verServer, string filename, string gamename, bool forceLoad = false)
        {
            List<ResfileInfo> ResList = (List<ResfileInfo>)Cache.Get("ResfileInfoList"+gamename);             
               
            if (ResList == null || forceLoad)
            {                
                try
                {
                    XmlDocument docServer = new XmlDocument();
                    string path = HttpContext.Current.Server.MapPath("./");
                    docServer.Load(path + "\\download\\" + filename);

                    XmlNode configServer = docServer.SelectSingleNode("Config");
                    ResList = new List<ResfileInfo>();
                    foreach (XmlNode node in configServer.ChildNodes)
                    {
                        if (node.Name == "File")
                        {
                            ResfileInfo ri = new ResfileInfo();
                            ri.Path = node.Attributes["path"].Value;
                            ri.MD5 = node.Attributes["md5"].Value;
                            ri.Ver = node.Attributes["ver"].Value;
                            ri.Index = ConverVer(ri.Ver);
                            ResList.Add(ri);
                        }
                    }

                    ResList.Sort(CompareResfileInfo);
                    verServer = configServer.Attributes["ver"].Value;                
                }
                catch (Exception)
                {
                    ResList.Clear();
                    verServer = "";
                }   

                if (verServer != "")
                {
                    Cache["ResfileInfoList" + gamename] = ResList;
                    Cache["ServerVer" + gamename] = verServer;
                }
            }
            else
            {
                verServer = (string)Cache.Get("ServerVer" + gamename);
            }

            return ResList;
        }

        protected XmlDocument get_nullxml(string verServer)
        {
            //创建空文件列表
            XmlDocument docReturn = (XmlDocument)Cache.Get(verServer);
            if (docReturn == null)
            {
                docReturn = new XmlDocument();
                XmlElement configReturn = docReturn.CreateElement("Config");
                XmlAttribute attrVer = docReturn.CreateAttribute("ver");
                attrVer.Value = verServer;
                configReturn.Attributes.Append(attrVer);
                docReturn.AppendChild(configReturn);
                Cache.Insert(verServer, docReturn);
            }
           
            return docReturn;
        }
    }    
}