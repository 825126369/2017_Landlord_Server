using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace PaymentCheck
{
    public partial class anysdk_google : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string result = "ok";
            NameValueCollection req = Request.Form;

            if (req.Count <= 0)
            {
                Response.Write("req.Count <= 0");
                Response.Flush();
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string key in req)
            {
                data[key] = req[key];
            }

            data["PayTime"] = DateTime.Now;
            MongodbPayment.Instance.ExecuteInsert("anysdk_log", data);

            try
            {
                if (data["sign"].ToString() == getSignForAnyValid(Request) && data["pay_status"].ToString() == "1")
                {
                    int payCode = check_paycode(data["product_id"].ToString());
                    if (payCode < 0)
                    {
                        data["amount"] = "0.0";//异常订单不算充值
                    }

                    try
                    {
                        data["RMB"] = Convert.ToDouble(data["amount"]);
                    }
                    catch (Exception)
                    {
                        data["RMB"] = (double)Convert.ToInt32(data["amount"]);
                    }
                    if (!MongodbPayment.Instance.KeyExistsBykey("anysdk_pay", "OrderID", data["order_id"]) && Convert.ToDouble(data["RMB"]) > 0)
                    {
                        data["OrderID"] = data["order_id"].ToString();
                        data["Account"] = data["channel_number"].ToString() + "_" + data["user_id"].ToString();
                        data["PayCode"] = payCode.ToString();
                        data["Process"] = false;
                        data["PlayerId"] = Convert.ToInt32(data["game_user_id"]);
                        data["ServerId"] = Convert.ToInt32(data["server_id"]);

                        if (data.ContainsKey("private_data"))
                        {
                            string[] strs = data["private_data"].ToString().Trim().Split('#');
                            if (strs.Length > 1)
                            {
                                data["shoppage"] = strs[0];

                                //爱贝渠道特殊处理
                                if (data["channel_number"].ToString() == "800053")
                                {
                                    data["Account"] = strs[1];
                                }
                                //真实账号KEY
                                data["real_Account"] = strs[1];
                                //设备号
                                if (strs.Length > 2)
                                    data["acc_dev"] = strs[2];
                            }
                            else
                            {
                                data["shoppage"] = data["private_data"];
                            }
                        }
                        if (MongodbPayment.Instance.ExecuteInsert("anysdk_pay", data))
                        {
                            string shoppage = "lobby";
                            if (data.ContainsKey("shoppage"))
                            {
                                shoppage = data["shoppage"].ToString();
                                if (string.IsNullOrEmpty(shoppage))
                                {
                                    shoppage = "lobby";
                                }
                            }

                            save_payinfo(shoppage, Convert.ToDouble(data["RMB"]));
                        }

                        Dictionary<string, object> savelog = new Dictionary<string, object>();
                        savelog["acc"] = data["Account"];
                        if (data.ContainsKey("real_Account"))
                        {
                            savelog["real_acc"] = data["real_Account"];
                        }
                        //记录设备号
                        if (data.ContainsKey("acc_dev"))
                        {
                            savelog["acc_dev"] = data["acc_dev"];
                        }
                        savelog["time"] = DateTime.Now;
                        savelog["channel"] = data["channel_number"].ToString();
                        savelog["rmb"] = data["RMB"];
                        MongodbPayment.Instance.ExecuteInsert("PayLog", savelog);
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                CLOG.Info(ex.ToString());
            }

            Response.Write(result);
        }

        void save_payinfo(string info, double amount)
        {
            if (string.IsNullOrEmpty(info) || amount <= 0)
                return;

            DateTime dt = DateTime.Now.Date;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[info] = amount;
            data["total_rmb"] = amount;

            MongodbPayment.Instance.ExecuteIncByQuery("pay_infos", Query.EQ("date", BsonValue.Create(dt)), data);
        }


        //获得anysdk支付通知 sign,将该函数返回的值与any传过来的sign进行比较验证
        public String getSignForAnyValid(HttpRequest request)
        {
            NameValueCollection requestParams = request.Form;//获得所有的参数名
            List<String> ps = new List<String>();
            foreach (string key in requestParams)
            {
                ps.Add(key);
            }

            sortParamNames(ps);// 将参数名从小到大排序，结果如：adfd,bcdr,bff,zx

            String paramValues = "";
            foreach (string param in ps)
            {//拼接参数值
                if (param == "sign")
                {
                    continue;
                }
                String paramValue = requestParams[param];
                if (paramValue != null)
                {
                    paramValues += paramValue;
                }
            }
            String md5Values = MD5Encode(paramValues);
            md5Values = MD5Encode(md5Values.ToLower() + ConfigurationManager.AppSettings["anysdk_key"].ToString()).ToLower();
            return md5Values;
        }


        //MD5编码
        public static String MD5Encode(String sourceStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] src = Encoding.UTF8.GetBytes(sourceStr);
            byte[] res = md5.ComputeHash(src, 0, src.Length);
            return BitConverter.ToString(res).ToLower().Replace("-", "");
        }
        //将参数名从小到大排序，结果如：adfd,bcdr,bff,zx
        public static void sortParamNames(List<String> paramNames)
        {
            paramNames.Sort((String str1, String str2) => { return str1.CompareTo(str2); });
        }

        public int check_paycode(string productid)
        {
            int returnid = -1;
            string isThPay = ConfigurationManager.AppSettings["th_pay"];
            if (string.IsNullOrEmpty(isThPay))
            {
                isThPay = "false";
            }
            if (isThPay.Equals("true"))
            {
                Dictionary<string, int> payCodes = GetTHPayCode();
                if (payCodes.ContainsKey(productid))
                {
                    returnid = payCodes[productid];
                }
                else
                {
                    returnid = -1;
                }
            }
            else
            {
                string pid = productid;
                int index = pid.LastIndexOf('_');
                if (index > 0)
                    pid = pid.Remove(0, index + 1);

                try
                {
                    returnid = Convert.ToInt32(pid);
                }
                catch (Exception ed)
                {
                    returnid = -1;
                }
            }
            return returnid;
        }

        protected Dictionary<string, int> GetTHPayCode()
        {
            Dictionary<string, int> ResList = (Dictionary<string, int>)Cache.Get("THPayCode");

            if (ResList == null)
            {
                try
                {
                    XmlDocument docServer = new XmlDocument();
                    string path = HttpContext.Current.Server.MapPath("./");
                    docServer.Load(path + "M_RechangeCFG.xml");

                    XmlNode configServer = docServer.SelectSingleNode("Root");
                    ResList = new Dictionary<string, int>();
                    foreach (XmlNode node in configServer.ChildNodes)
                    {
                        if (node.Name == "Data")
                        {
                            int paycode = Convert.ToInt32(node.Attributes["ID"].Value);
                            string appstoreid = node.Attributes["AppStoreID"].Value;
                            if (!string.IsNullOrEmpty(appstoreid))
                            {
                                ResList.Add(appstoreid, paycode);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CLOG.Info(ex.Message);
                }
                Cache["THPayCode"] = ResList;
            }
            return ResList;
        }
    }
}