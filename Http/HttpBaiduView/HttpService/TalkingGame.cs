using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Configuration;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;


public class TalkingGame
{
    int get_vca(string paycode)
    {
        try
        {
            XmlDocument xmldoc = new XmlDocument();
            string path = HttpContext.Current.Server.MapPath("./");         
            path+= ConfigurationManager.AppSettings["rechangeCFG"];
            xmldoc.Load(path);

            var node = xmldoc.SelectSingleNode("//Data[@ID='" + paycode + "']");
            if (node != null)
            {
                return Convert.ToInt32(node.Attributes[ConfigurationManager.AppSettings["rechangeItem"]].Value);
            }
        }
        catch(Exception)
        {

        }  
        return 0;
    }

    Random rd = new Random();
    public void adddata(string account, string orderid, string rmb, string paycode)
    {
        if (!Convert.ToBoolean(ConfigurationManager.AppSettings["opentg"]))
            return;

        string postdata = "{" +
                "\"msgID\":\"" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\"" +
                ",\"OS\": \"ios\"" +
                ",\"accountID\":\"" + account + "\"" +
                ",\"orderID\":\"" + orderid + "\"" +
                ",\"currencyAmount\":" + rmb +
                ",\"currencyType\": \"CNY\"" +
                ",\"virtualCurrencyAmount\":" + get_vca(paycode) +
                ",\"status\": \"success\"" +
                "}";

        datas.Add(postdata);
    }

    public void PostToTG()
    {
        if (!Convert.ToBoolean(ConfigurationManager.AppSettings["opentg"]))
            return;

        if (datas.Count <= 0)
            return;

        try
        {       
            string strurl = "http://api.talkinggame.com/api/charge/" + ConfigurationManager.AppSettings["talkinggame"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strurl);
            request.Method = "POST";
            request.ContentType = "application/msgpack";

            byte[] data = getdata();
            request.ContentLength = data.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Flush();
            newStream.Close();
        }
        catch (Exception)
        {

        }           
    }

    byte[] getdata()
    {
        string end = "[";
        bool bf = false;
        foreach (string d in datas)
        {
            if (bf)
                end += ",";
            end += d;
            bf =true;
        }
        end += "]";

        return ZipHelper.Compress(Encoding.UTF8.GetBytes(end));
    }

    List<string> datas = new List<string>();
}



