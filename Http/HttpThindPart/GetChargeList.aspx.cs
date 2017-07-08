using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;

namespace HttpThindPart
{
    public partial class GetChargeList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: 验证签名是否正确

            XmlDocument doc = new XmlDocument();
            doc.Load(Request.PhysicalApplicationPath + "App_Data\\M_RechangeCFG.xml");              //加载Xml文件  
            XmlElement rootElem = doc.DocumentElement;   //获取根节点  
            XmlNodeList personNodes = rootElem.GetElementsByTagName("Data");  //获取data子节点集合  
            List<Dictionary<string, object>> jsonData = new List<Dictionary<string, object>>();
            foreach (XmlNode node in personNodes)
            {
                Dictionary<string, object> dicNode = new Dictionary<string, object>();
                int chargeId = int.Parse(((XmlElement)node).GetAttribute("ID"));   //获取name属性值  
                string name = ((XmlElement)node).GetAttribute("Name");
                int amount = int.Parse(((XmlElement)node).GetAttribute("Price")) * 100;
                int coins = int.Parse(((XmlElement)node).GetAttribute("Gold"));
                int diamonds = int.Parse(((XmlElement)node).GetAttribute("GiveTicket"));
                dicNode["chargeId"] = chargeId;
                dicNode["amount"] = amount;
                dicNode["coins"] = coins;
                dicNode["diamonds"] = diamonds;
                dicNode["name"] = name;
                jsonData.Add(dicNode);
            }

            ReturnMsg(jsonData);
        }

        void ReturnMsg(List<Dictionary<string, object>> jsonData)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = 10000;
            data["msg"] = "success";
            data["data"] = jsonData;

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(jsonstr);

        }
    }
}