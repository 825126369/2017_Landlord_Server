using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.IO;

namespace HttpLogin
{
    // 维护信息，GM可通过后台修改维护内容
    public partial class MaintenanceInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 获取当前的维护信息
            if (Request.RequestType == "GET")
            {
                Dictionary<string, object> data = getCurInfo();
                returnMsg(data);
            }
            else // 修改信息
            {
                string state = Request.Params["state"];
                string info = Request.Params["info"];
                if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(info))
                {
                    Response.Write("1");  // 失败
                    return;
                }

                string res = modifyInfo(state, info);
                Response.Write(res);  // 成功
            }
        }

        private void returnMsg(Dictionary<string, object> data)
        {
            string jsondata = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsondata)));
        }

        // 返回当前的信息
        private Dictionary<string, object> getCurInfo()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            string dir = HttpContext.Current.Server.MapPath("~/download");
            string file = Path.Combine(dir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            
            XmlNode node = doc.SelectSingleNode("/Config");

            string state = node.Attributes["state"].Value;
            string info = node.Attributes["info"].Value;
            data.Add("state", state);
            data.Add("info", info);
            return data;
        }

        private string modifyInfo(string state, string info)
        {
            try
            {
                string dir = HttpContext.Current.Server.MapPath("~/download");
                string file = Path.Combine(dir, "config.xml");

                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNode node = doc.SelectSingleNode("/Config");
                node.Attributes["state"].Value = state;
                node.Attributes["info"].Value = info;
                doc.Save(file);
            }
            catch (System.Exception)
            {
                return "1";
            }
            return "0";
        }
    }
}