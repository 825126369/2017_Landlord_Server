using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Common;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HttpLogin.thirdsdk
{
    public partial class GetAccountList : System.Web.UI.Page
    {
        Random m_rd = new Random();
        string AES_KEY = "@@baiduviewkey@@";

        struct AccountInfo
        {
            public string account;
            public string password;

            public AccountInfo(string acc, string pwd)
            {
                account = acc;
                password = pwd;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string phoneNum = Request.QueryString["phonenum"];
            if (string.IsNullOrEmpty(phoneNum))
            {
                Response.Write(Helper.buildLuaReturn(-2, "err_not_phone"));//号码错误
                return;
            }
            string phoneCode = Request.QueryString["phonecode"];
            if (string.IsNullOrEmpty(phoneCode))
            {
                Response.Write(Helper.buildLuaReturn(-2, "err_not_phone"));//号码错误
                return;
            }
            phoneNum = Encoding.Default.GetString(Convert.FromBase64String(phoneNum));
            phoneNum = AESHelper.AESDecrypt(phoneNum, AES_KEY);//aes解密
            phoneCode = Encoding.Default.GetString(Convert.FromBase64String(phoneCode));
            phoneCode = AESHelper.AESDecrypt(phoneCode, AES_KEY);//aes解密

            List<IMongoQuery> lmq = new List<IMongoQuery>();
            lmq.Add(Query.EQ("phoneNum", phoneNum));
            lmq.Add(Query.EQ("phoneCode", phoneCode));

            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetByQuery("BaiduPhoneCode", Query.And(lmq), new string[] { "lastSendTime" });
            if (data == null)
            {
                Response.Write(Helper.buildLuaReturn(-2, "err_not_phone"));//号码错误
                return;
            }

            string password = BuildAccount.getAutoPassword(6);
            string passwordMD5 = AESHelper.MD5Encrypt(password);
            updateAccountInfos(phoneNum, passwordMD5);
            //获取帐号信息
            List<AccountInfo> accounts = getAccountInfos(phoneNum);
            //没有帐号
            if (accounts.Count == 0)
            {
                Response.Write(Helper.buildLuaReturn(-2, "err_not_phone"));//号码错误
                return;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("local ret = {{}};");
                sb.Append("ret.code = 0;");
                sb.Append("ret.msg = \"\";");
                sb.Append("ret.data = {{}};");
                for (int i = 0; i < accounts.Count; i++ )
                {
                    string pwd = AESHelper.AESEncrypt(password, AES_KEY);
                    sb.AppendFormat("ret.data[{0}] = {{acc=\"{1}\",pwd=\"{2}\"}};", i + 1, accounts[i].account, pwd);
                }
                sb.Append("return ret;");
                Response.Write(sb.ToString());
            }
        }

        List<AccountInfo> getAccountInfos(string phoneNum)
        {
            var datas = MongodbAccount.Instance.ExecuteGetListBykey("baiduview_login", "bindPhone", phoneNum);
            List<AccountInfo> accDatas = new List<AccountInfo>();
            foreach (var data in datas)
            {
                string account = data["acc"].ToString();
                string pwd = data["pwd"].ToString();

                accDatas.Add(new AccountInfo(account, pwd));
            }
            return accDatas;
        }

        //更新密码
        void updateAccountInfos(string phoneNum, string password)
        {
            Dictionary<string, object> updata = new Dictionary<string, object>();
            updata["pwd"] = password;
            string strerr = MongodbAccount.Instance.ExecuteUpdate("baiduview_login", "bindPhone", phoneNum, updata);
        }
    }
}