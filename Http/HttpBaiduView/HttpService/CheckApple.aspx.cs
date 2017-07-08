using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Configuration;
using System.Text;
using System.IO;

namespace AccountCheck
{
    public partial class CheckApple : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "POST")
                return;

            string sacc = Request.Params["account"];   

            if (string.IsNullOrEmpty(sacc))
            {
                ReturnMsg("data error");
                return;
            }

            string table = ConfigurationManager.AppSettings["pay_apple"];
            if (string.IsNullOrEmpty(table))
            {
                ReturnMsg("error platform");//platform error
                return;
            }

            StreamReader StreamReader = new StreamReader(Request.InputStream, Encoding.UTF8);
            string order64 = StreamReader.ReadToEnd();

             //string order64 = Request.Params["order64"];

            try
            {
                string postDataStr = "{\"receipt-data\":\"" + order64 + "\"}";
                MemoryStream stream = new MemoryStream();
                byte[] data = Encoding.UTF8.GetBytes(postDataStr);
                stream.Write(data, 0, data.Length);
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["appleurl"]);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = stream.Length;
                Stream requestStream = request.GetRequestStream();
                stream.Position = 0L;
                stream.CopyTo(requestStream);
                stream.Close();
                requestStream.Close();

                //return

                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())                
                {
                    StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8);
                    string retString = myStreamReader.ReadToEnd();
                    Dictionary<string, object> ret = JsonHelper.ParseFromStr<Dictionary<string, object>>(retString);

                    if (ret["status"].ToString() == "0")
                    {
                        Dictionary<string, object> ret2 = JsonHelper.ParseFromStr<Dictionary<string, object>>(ret["receipt"].ToString());
                        List<Dictionary<string, object>> ret3 = JsonHelper.ParseFromStr<List<Dictionary<string, object>>>(ret2["in_app"].ToString());

                        string retstr = "";
                        foreach (var dic in ret3)
                        {
                            string orderid = dic["transaction_id"].ToString();
                            string payid = dic["product_id"].ToString();
                            int index = payid.LastIndexOf('_');
                            payid = payid.Substring(index+1);

                            if (!MongodbPayment.Instance.KeyExistsBykey(table, "OrderID", orderid))
                            {
                                retstr += payid + " ";                            
                                dic["time"] = DateTime.Now;
                                dic["account"] = sacc;
                                
                                MongodbPayment.Instance.ExecuteStoreBykey(table, "OrderID", orderid, dic);
                            }
                        }

                        if (retstr != "")
                        {
                            ReturnMsg(retstr.Trim(), true);
                            return;
                        }
                    }
                }
                ReturnMsg("order error");
            }
            catch(Exception ex)
            {
                ReturnMsg(ex.ToString());
            }
        }


        void ReturnMsg(string info, bool bret = false)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;
            if (bret)
                data["data"] = info;
            else
                data["error"] = info;

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsonstr)));            
        }
    }
    
}