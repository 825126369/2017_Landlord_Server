using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HttpLogin
{
    public class BuildAccount
    {
        static private Object thisLock = new Object();

        static public string getAutoAccount(string acc_table)
        {
            string acc = null;
            lock (thisLock)
            {
                string autoName = acc_table + "autoInc";
                var data = MongodbAccount.Instance.ExecuteGetBykey("config", "type", autoName);
                if (data == null)
                {
                    data = new Dictionary<string, object>();
                    data["type"] = autoName;
                    data["value"] = 100000;
                    MongodbAccount.Instance.ExecuteInsert("config", data);
                }

                Int32 acc_inc = Convert.ToInt32(data["value"]);
                int j = 0;
                do
                {
                    if (j++ > 100)
                        break;

                    acc = "play" + acc_inc.ToString();
                    acc_inc++;
                }
                while (MongodbAccount.Instance.KeyExistsBykey(acc_table, "acc", acc));

                data["value"] = acc_inc;
                MongodbAccount.Instance.ExecuteUpdate("config", "type", autoName, data);
            }
            return acc;
        }

        static public string getVisitorAccount(string acc_table)
        {
            string acc = null;
            lock (thisLock)
            {
                string autoName = acc_table + "visitorInc";
                var data = MongodbAccount.Instance.ExecuteGetBykey("config", "type", autoName);
                if (data == null)
                {
                    data = new Dictionary<string, object>();
                    data["type"] = autoName;
                    data["value"] = 100000;
                    MongodbAccount.Instance.ExecuteInsert("config", data);
                }

                Int32 acc_inc = Convert.ToInt32(data["value"]);
                int j = 0;
                do
                {
                    if (j++ > 100)
                        break;

                    acc = "游客" + acc_inc.ToString();
                    acc_inc++;
                }
                while (MongodbAccount.Instance.KeyExistsBykey(acc_table, "acc", acc));

                data["value"] = acc_inc;
                MongodbAccount.Instance.ExecuteUpdate("config", "type", autoName, data);
            }
            return acc;
        }

        static public string getAutoPassword(int length)
        {
            string password = "";
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                int c = rd.Next(0, 61);
                if (c <= 9)
                {
                    c += 48;
                }
                else if (c <= 35)
                {
                    c += 55;
                }
                else
                {
                    c += 61;
                }
                password += (char)c;
            }
            return password;
        }

        static public string buildLuaReturn(int code, string msg)
        {
            return string.Format("local ret = {{code = {0}, msg=\"{1}\"}}; return ret;", code, msg);
        }

        //static public bool regeditAccount(string acc, string pwd, string randkey, string platform)
        //{
        //    Dictionary<string, object> updata = new Dictionary<string, object>();
        //    updata["acc"] = acc;
        //    updata["pwd"] = pwd;
        //    DateTime now = DateTime.Now;
        //    updata["randkey"] = randkey;
        //    updata["lasttime"] = now.Ticks;
        //    updata["regedittime"] = now;
        //    updata["regeditip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
        //    updata["updatepwd"] = false;
        //    //updata["platform"] = Platform;


        //    string strerr = MongodbAccount.Instance.ExecuteStoreBykey(table, "acc", sacc, updata);
        //    if (strerr != "")
        //    {
        //        ReturnMsg("-11");//server error
        //    }
        //    else
        //    {
        //        RSAHelper rsa = new RSAHelper();
        //        rsa.setModulus(data["n3"].ToString());

        //        string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
        //        ReturnMsg(rsa.RSAEncryptStr(clientkey), true);//login success

        //        Dictionary<string, object> savelog = new Dictionary<string, object>();
        //        savelog["acc"] = sacc;
        //        savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
        //        savelog["time"] = now;
        //        MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
        //    }
        //}
    }
}