using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace LoginTest
{
    class Program
    {
        const string AES_KEY = "&@*(#kas9081fajk";

        static void Main(string[] args)
        {
            string pattern = @"^[0-9a-zA-z][^_]{6,20}$";

            if (!Regex.IsMatch("1234556547", pattern))
            {
                int i = 0;
            }



            TalkingGame.get_vca("3");
            //TalkingGame tg = new TalkingGame();
            //tg.adddata("test", "PB585615090914221334621",1,10);
            //tg.PostToTG();



            int time1 = Environment.TickCount;
            string result = Encoding.UTF8.GetString(HttpPost.Get(new Uri("http://192.168.1.11:26004/ServerList.aspx"), true));
            int time2 = Environment.TickCount;
            Console.WriteLine("time:"+(time2-time1));
            if (result.StartsWith("error"))
                Console.WriteLine(result);
            else
            {                
                Console.WriteLine(result);
                Console.WriteLine("");
            }

            /////////////////////////////////////////////////////////////

            RSAHelper rsa = new RSAHelper();
            rsa.init();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["n1"] = "test1";
            data["n2"] = AESHelper.AESEncrypt("123456", AES_KEY);
            data["n3"] = rsa.getModulus();
            

            string jsonstr = JsonHelper.ConvertToStr(data);
            string md5 = AESHelper.MD5Encrypt(jsonstr + AES_KEY);

            NameValueCollection nvc = new NameValueCollection();
            string urlstr= Convert.ToBase64String(Encoding.Default.GetBytes(jsonstr));
            nvc.Add("data", urlstr);
            nvc.Add("sign", md5);
            nvc.Add("platform", "default");
            urlstr = string.Format("http://localhost:33820/SwitchLogin.aspx?data={0}&sign={1}&platform={2}", urlstr, md5, "default");

            time1 = Environment.TickCount;
            result = Encoding.Default.GetString(HttpPost.Get(new Uri(urlstr)));
              time2 = Environment.TickCount;
             Console.WriteLine("time:" + (time2 - time1));
            if (result.StartsWith("error"))
                Console.WriteLine(result);
            else
            {
                string jstr = Encoding.Default.GetString(Convert.FromBase64String(result));
                Console.WriteLine("login:" + jstr);
                Console.WriteLine("");
                Dictionary<string, object> retdata = JsonHelper.ParseFromStr<Dictionary<string, object>>(jstr);

                if (!Convert.ToBoolean(retdata["result"]))
                {
                    Console.Read();
                    return;
                }

                string logindata = rsa.RSADecryptStr(retdata["data"].ToString());
                Console.WriteLine("data:" + logindata);
                Console.WriteLine("");

                //////////////////////////////////////////////////////////////              
                bool isok = Convert.ToBoolean(retdata["result"]);
                if (isok)
                {
                    nvc.Clear();
                    nvc.Add("acc","test1");
                    time1 = Environment.TickCount;
                    result = Encoding.Default.GetString(HttpPost.Get(new Uri("http://192.168.1.11:12345/AccCheck.aspx?acc=test1")));
                     time2 = Environment.TickCount;
                    Console.WriteLine("time:" + (time2 - time1));
                    jstr = Encoding.Default.GetString(Convert.FromBase64String(result));
                    Console.WriteLine("check:" + jstr);
                    Dictionary<string, object> retdata2 = JsonHelper.ParseFromStr<Dictionary<string, object>>(jstr);

                    if (!Convert.ToBoolean(retdata2["result"]))
                    {
                        Console.Read();
                        return;
                    }
                    Dictionary<string, object> retdata3 = JsonHelper.ParseFromStr<Dictionary<string, object>>(retdata2["data"].ToString());

                }

            }


            Console.Read();
        }

       
    }
}
