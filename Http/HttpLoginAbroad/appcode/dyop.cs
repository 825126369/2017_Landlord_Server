using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;
using System.Web;
using System.IO;

// 动态操作
public class DyOpBase
{
    public virtual string doDyop(object param)
    {
        return "";
    }

    public string returnMsg(string info, bool ret = false)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["result"] = ret;
        if (ret)
            data["data"] = info;
        else
            data["error"] = info;

        string jsondata = JsonHelper.ConvertToStr(data);
        return Convert.ToBase64String(Encoding.Default.GetBytes(jsondata));
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamPlayerSelfRegAcc
{
    public string m_strData;
    public string m_sign;
    public string m_ip;

    private Dictionary<string, object> m_data;
    // gm ID号
    public long m_gmId;

    public int isParamValid()
    {
        if (string.IsNullOrEmpty(m_strData) ||
            string.IsNullOrEmpty(m_sign))
            return HttpRetCode.RET_PARAM_NOT_VALID;

        string strdata = Encoding.Default.GetString(Convert.FromBase64String(m_strData));
        string sign = AESHelper.MD5Encrypt(strdata + DyOpPlayerSelfRegAcc.AES_KEY);

        if (sign != m_sign)
            return HttpRetCode.RET_SIGN_ERROR;

        m_data = JsonHelper.ParseFromStr<Dictionary<string, object>>(strdata);
        if (m_data == null ||
            m_data.Count != 4)
            return HttpRetCode.RET_JSON_ERROR;

        if (!m_data.ContainsKey("n1")) // 账号
            return HttpRetCode.RET_LACK_PARAM;

        if (!m_data.ContainsKey("n2")) // 密码
            return HttpRetCode.RET_LACK_PARAM;

        if (!m_data.ContainsKey("n3")) // rsa modules
            return HttpRetCode.RET_LACK_PARAM;

        if (!m_data.ContainsKey("n5")) // 代理账号
            return HttpRetCode.RET_LACK_PARAM;

        string id = Convert.ToString(m_data["n5"]);
        if(!long.TryParse(id, out m_gmId))
            return HttpRetCode.RET_AGENT_ACC_ERROR;

        return 0;
    }

    public Dictionary<string, object> getData() { return m_data; }
}

// 玩家自注账号
public class DyOpPlayerSelfRegAcc : DyOpBase
{
    public const string AES_KEY = "&@*(#kas9081fajk";

    public static string SQL_CMD = "select acc,accType,createCode from {0} where gmId={1} ";

    public override string doDyop(object param)
    {
        ParamPlayerSelfRegAcc p = (ParamPlayerSelfRegAcc)param;
        int code = p.isParamValid();
        if (code != 0)
            return returnMsg(code.ToString());

        Dictionary<string, object> data = p.getData();

        string acc = Convert.ToString(data["n1"]);
        if (!Regex.IsMatch(acc, Exp.ACCOUNT_PLAYER))
        {
            return returnMsg(HttpRetCode.RET_ACC_ERROR.ToString());
        }

        string pwd = AESHelper.AESDecrypt(data["n2"].ToString(), AES_KEY);
        if (pwd.Length != 32)
        {
            return returnMsg(HttpRetCode.RET_PWD_ERROR.ToString());
        }

        MySqlDbServer sqlDb = new MySqlDbServer(CC.MYSQL_IP);

        string cmd = string.Format(SQL_CMD, TableName.GM_ACCOUNT, p.m_gmId - ConstDef.GM_ID_OFFSET);
        Dictionary<string, object> agentData = sqlDb.queryOne(cmd, MySqlDbName.DB_XIANXIA);
        if (agentData == null)
        {
            return returnMsg(HttpRetCode.RET_AGENT_ACC_ERROR.ToString());
        }

        int accType = Convert.ToInt32(agentData["accType"]);
        if (accType != AccType.ACC_AGENCY) // 玩家账号只能以代理号作为创建者
        {
            return returnMsg(HttpRetCode.RET_AGENT_ACC_ERROR.ToString());
        }

        string creator = Convert.ToString(agentData["acc"]);
        string createCode = Convert.ToString(agentData["createCode"]);
        code = createAccToMySql(acc, creator, createCode, sqlDb);
        if (code == HttpRetCode.RET_ACC_EXISTS || code == 0)
        {
            RSAHelper rsa = new RSAHelper();
            rsa.setModulus(Convert.ToString(data["n3"]));
            string clientKey = "";
            code = createAccToMongo(acc, pwd, p.m_ip, sqlDb, ref clientKey);

            if (code == 0)
            {
                return returnMsg(rsa.RSAEncryptStr(clientKey), true);
            }
        }

        return returnMsg(code.ToString());
    }

    private int createAccToMySql(string accName, string creator, string createCode, MySqlDbServer sqlDb)
    {
        bool res = sqlDb.keyStrExists(TableName.PLAYER_ACCOUNT_XIANXIA, "acc", accName, MySqlDbName.DB_XIANXIA);
        if (res)
        {
            return HttpRetCode.RET_ACC_EXISTS;
        }

        double washRatio = 0;

        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("acc", accName, FieldType.TypeString);
        gen.addField("creator", creator, FieldType.TypeString);
        gen.addField("money", 0, FieldType.TypeNumber);
        gen.addField("moneyType", 0, FieldType.TypeNumber);
        gen.addField("state", PlayerState.STATE_IDLE, FieldType.TypeNumber);
        gen.addField("createTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        gen.addField("createCode", createCode, FieldType.TypeString);
        gen.addField("aliasName", accName, FieldType.TypeString);
        gen.addField("playerWashRatio", washRatio, FieldType.TypeNumber);

        string sqlCmd = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA);
        int count = sqlDb.executeOp(sqlCmd, MySqlDbName.DB_XIANXIA);
        return count > 0 ? 0 : HttpRetCode.RET_DB_ERROR;
    }

    private int createAccToMongo(string accName, 
                                 string pwd, 
                                 string ip, 
                                 MySqlDbServer sqlDb,
                                 ref string clientkey)
    {
        if (MongodbAccount.Instance.KeyExistsBykey(TableName.PLAYER_ACCOUNT, "acc", accName))
        {
            return HttpRetCode.RET_ACC_EXISTS;
        }

        Random rd = new Random();
        int randkey = rd.Next();
        Dictionary<string, object> updata = new Dictionary<string, object>();
        updata["acc"] = accName;
        updata["pwd"] = pwd;
        DateTime now = DateTime.Now;
        updata["randkey"] = randkey;
        updata["lasttime"] = now.Ticks;
        updata["regedittime"] = now;
        updata["regeditip"] = ip;
        updata["updatepwd"] = false;

        string strerr = MongodbAccount.Instance.ExecuteStoreBykey(TableName.PLAYER_ACCOUNT, "acc", accName, updata);
        if (strerr != "")
        {
            return HttpRetCode.RET_DB_ERROR;
        }

       // RSAHelper rsa = new RSAHelper();
       // rsa.setModulus(rsaModule);

        clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
       // ReturnMsg(rsa.RSAEncryptStr(clientkey), true);//login success

        Dictionary<string, object> savelog = new Dictionary<string, object>();
        savelog["acc"] = accName;
        savelog["ip"] = ip;
        savelog["time"] = now;
        MongodbAccount.Instance.ExecuteInsert(TableName.PLAYER_LOGIN, savelog);

        return 0;
    }
}

//////////////////////////////////////////////////////////////////////////
// 修改玩家密码
public class DyOpModifyPlayerPwd : DyOpBase
{
    public static string SQL_QUERY = " select {0}.acc as playerAcc,accType from {0},{1} WHERE {0}.acc='{2}' and {0}.creator={1}.acc ";

    public bool canModifyPwd(string acc)
    {
        MySqlDbServer sqlDb = new MySqlDbServer(CC.MYSQL_IP);

        string sqlCmd = string.Format(SQL_QUERY,
                                    TableName.PLAYER_ACCOUNT_XIANXIA,
                                    TableName.GM_ACCOUNT, acc);

        Dictionary<string, object> retData = sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
        if (retData == null)
        {
            return false;
        }

        string playerAcc = Convert.ToString(retData["playerAcc"]);
        if (playerAcc != acc)
            return false;

        int atype = Convert.ToInt32(retData["accType"]);
        // api账号不能更改密码
        return atype != AccType.ACC_API;
    }
}















