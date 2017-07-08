using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

// 动态操作
public class DyOpBase
{
    // GM账号
    protected GMUser m_gmUser;

    protected Dictionary<string, object> m_retData = new Dictionary<string, object>();

    // 返回json串格式
    public virtual string doDyop(object param)
    {
        return "";
    }

    // 作查询, 返回json串格式
    public virtual string doQuery(object param) { return ""; }

    // 创建GM账号，返回true创建成功
    public bool createGMUser(ParamBase param)
    {
        m_gmUser = new GMUser(param);
        if (!m_gmUser.isLogin)
        {
            m_retData.Add("result", RetCode.RET_GM_LOGIN_FAILED);
        }
        return m_gmUser.isLogin;
    }
}

public class ParamBase
{
    // 操作账号
    public string m_gmAccount;

    // 操作密码，传过来时，需要以MD5加密
    public string m_gmPwd;

    public string m_playerAcc = "";

    // 签名
    public string m_sign;

    protected int m_fieldIndex;

    public int fieldIndex
    {
        get { return m_fieldIndex; }
        set { m_fieldIndex = value; }
    }

    public virtual bool isParamValid()
    {
        if (string.IsNullOrEmpty(m_gmAccount))
            return false;

        if (string.IsNullOrEmpty(m_gmPwd))
            return false;

        if (!playerAccIsValid())
            return false;

        if (string.IsNullOrEmpty(m_sign))
            return false;

        return true;
    }

    // 检测签名
    public virtual bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_playerAcc + user.m_devSecretKey);
        return m_sign == sign;
    }

    protected virtual bool playerAccIsValid()
    {
        if (string.IsNullOrEmpty(m_playerAcc))
            return false;

        return true;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamCreatePlayer : ParamBase
{
    public string m_pwd = "";
    // 洗码比
    public string m_washRatioStr;

    public double m_washRatio;

    // 别名
    public string m_aliasName;

    public ParamCreatePlayer()
    {
        m_fieldIndex = 0;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_pwd))
            return false;

        if (!string.IsNullOrEmpty(m_washRatioStr))
        {
            if (!double.TryParse(m_washRatioStr, out m_washRatio))
            {
                return false;
            }
        }
        if (string.IsNullOrEmpty(m_aliasName))
        {
            m_aliasName = "";
        }

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_playerAcc + m_pwd + user.m_devSecretKey);
        return m_sign == sign;
    }
}

// 创建玩家
public class DyOpCreatePlayer : DyOpBase
{
    const string AES_KEY = "&@*(#kas9081fajk";

    public override string doDyop(object param)
    {
        ParamCreatePlayer p = (ParamCreatePlayer)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        // 只有API号才可以创建玩家
        if (m_gmUser.m_accType != AccType.ACC_API) 
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        // 洗码比的判断
        if (p.m_washRatio < 0 || p.m_washRatio > m_gmUser.m_washRatio)
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }

        // 玩家账号由API方拼接，这里进行判定
        // 账号里面需要有前缀
        if (p.m_playerAcc.IndexOf(m_gmUser.m_postfix) != 0)
        {
            m_retData.Add("result", RetCode.RET_ACC_PWD_FORMAT_ERROR);
            return Helper.genJsonStr(m_retData);
        }
        //string tmpAcc = p.m_playerAcc.Remove(0, m_gmUser.m_postfix.Length);

        if (!Regex.IsMatch(p.m_playerAcc, Exp.ACCOUNT_PLAYER))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }

        if (!Regex.IsMatch(p.m_pwd, Exp.ACCOUNT_PLAYER_PWD))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }

        string error = "";
        bool res = createAccToServer(p.m_playerAcc, p.m_pwd, ref error);
        int retCode = RetCode.RET_OP_FAILED;

        if (res || error == "-12")
        {
            retCode = createAcc(p.m_playerAcc, m_gmUser, p);
        }
        else
        {
            /*if (error == "-12") // 该账号已存在
            {
                m_retData.Add("result", RetCode.RET_ACCOUNT_HAS_EXISTS);
            }*/
            if (error == "-11") // 数据库出错了
            {
                retCode = RetCode.RET_DB_ERROR;
                //m_retData.Add("result", RetCode.RET_DB_ERROR);
            }
            else if (error == "-14" || error == "-20") // 账号密码格式不对
            {
                retCode = RetCode.RET_ACC_PWD_FORMAT_ERROR;
                //m_retData.Add("result", RetCode.RET_ACC_PWD_FORMAT_ERROR);
            }
        }

        //if (res)
        {
            if (retCode == RetCode.RET_SUCCESS)
            {
                m_retData.Add("result", RetCode.RET_SUCCESS);
                m_retData.Add("playerAcc", p.m_playerAcc);
            }
            else
            {
                m_retData.Add("result", retCode);
            }
        }
       
        return Helper.genJsonStr(m_retData);
    }

    // 返回0成功，其他出错
    private int createAcc(string accName, GMUser user, ParamCreatePlayer param)
    {
        bool exists = user.sqlDb.keyStrExists(TableName.PLAYER_ACCOUNT_XIANXIA, "acc", accName, MySqlDbName.DB_XIANXIA);
        if (exists)
            return RetCode.RET_ACCOUNT_HAS_EXISTS;

        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("acc", accName, FieldType.TypeString);
        gen.addField("creator", user.m_acc, FieldType.TypeString);
        gen.addField("money", 0, FieldType.TypeNumber);
        gen.addField("moneyType", user.m_moneyType, FieldType.TypeNumber);
        gen.addField("state", PlayerState.STATE_IDLE, FieldType.TypeNumber);
        gen.addField("createTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        gen.addField("createCode", user.m_createCode, FieldType.TypeString);
        gen.addField("aliasName", param.m_aliasName, FieldType.TypeString);
        gen.addField("playerWashRatio", param.m_washRatio, FieldType.TypeNumber);
       
        string sqlCmd = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA);
        int count = user.sqlDb.executeOp(sqlCmd, MySqlDbName.DB_XIANXIA);
        return count > 0 ? 0 : RetCode.RET_DB_ERROR;
    }

    private bool createAccToServer(string accName, string pwd, ref string error)
    {
        RSAHelper rsa = new RSAHelper();
        rsa.init();
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["n1"] = accName;
        string md5Pwd = Tool.getMD5Hash(pwd);
        data["n2"] = AESHelper.AESEncrypt(md5Pwd, AES_KEY);
        data["n3"] = rsa.getModulus();

        string jsonstr = JsonHelper.ConvertToStr(data);
        string md5 = AESHelper.MD5Encrypt(jsonstr + AES_KEY);
        string urlstr = Convert.ToBase64String(Encoding.Default.GetBytes(jsonstr));
            
        string fmt = WebConfigurationManager.AppSettings["createAccount"];
        string aspx = string.Format(fmt, urlstr, md5);
        var ret = HttpPost.Get(new Uri(aspx));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            string oriStr = Encoding.Default.GetString(Convert.FromBase64String(retStr));
            Dictionary<string, object> retData =
                JsonHelper.ParseFromStr<Dictionary<string, object>>(oriStr);
            if (retData.ContainsKey("result"))
            {
                if (retData.ContainsKey("error"))
                {
                    error = Convert.ToString(retData["error"]);
                }
                return Convert.ToBoolean(retData["result"]);
            }
        }
        return false;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamScore : ParamBase
{
    // 0上分 1下分
    public int m_op;
    public string m_score;

    // 用户自定义订单ID[0-200]，字母数字组合
    public string m_userOrderId;

    // api回调页面
    public string m_apiCallBack;

    public ParamScore() 
    {
        m_fieldIndex = 1;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_score))
            return false;

        if (string.IsNullOrEmpty(m_userOrderId))
        {
            return false;
            //m_userOrderId = "";
        }
        else
        {
            if (!Regex.IsMatch(m_userOrderId, Exp.USER_ORDER_ID))
            {
                return false;
            }
        }
        if (string.IsNullOrEmpty(m_apiCallBack))
        {
            m_apiCallBack = "";
        }
        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_playerAcc + m_score + m_userOrderId + user.m_devSecretKey);
        return m_sign == sign;
    }
}

// 上分，下分
public class DyOpScore : DyOpBase
{
    public override string doDyop(object param)
    {
        ParamScore p = (ParamScore)param;
        long oriScore = 0;
        if (!long.TryParse(p.m_score, out oriScore))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }

        if (oriScore <= 0)
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }

        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }

        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        if (orderIdExists(p))
        {
            m_retData.Add("result", RetCode.RET_ORDER_EXISTS);
            return Helper.genJsonStr(m_retData);
        }

        long score = Helper.saveMoneyValue(oriScore);

        int code = canDoScoreToPlayer(p.m_playerAcc, score, ScropOpType.isAddScore(p.m_op));
         // 在线，并且是下分，直接返回失败
        if (code == RetCode.RET_PLAYER_ONLINE && !ScropOpType.isAddScore(p.m_op))
        {
            m_retData.Add("result", RetCode.RET_PLAYER_ONLINE);
        }
        else if (code == RetCode.RET_PLAYER_ONLINE && ScropOpType.isAddScore(p.m_op)) // 玩家在线时提交上下分订单
        {
            //if (ScropOpType.isAddScore(p.m_op)) // 上分操作,先扣GM的钱，若订单最终处理失败，会返还GM这部分钱
            {
                decScore(score);
            }

            OrderGenerator or = new OrderGenerator();
            Dictionary<string, object> orData = or.genOrder(p.m_gmAccount, p.m_playerAcc, score, p.m_op,
                AccType.ACC_PLAYER,
                OrderGenerator.ORDER_FROM_API,
                "",
                p.m_userOrderId, p.m_apiCallBack);

            bool res = MongodbPlayer.Instance.ExecuteInsert(TableName.PLAYER_ORDER_REQ, orData);
            if (res)
            {
                m_retData.Add("orderId", Convert.ToString(orData["orderId"]));
                m_retData.Add("result", RetCode.RET_HAS_SUBMIT_ORDER);
            }
            else
            {
                m_retData.Add("result", RetCode.RET_DB_ERROR);
            }
        }
        else if (code == RetCode.RET_SUCCESS) // 玩家离线，可直接修改数据库上下分
        {
            bool res = doScorePlayer(p, score);
            if (res)
            {
                m_retData.Add("result", RetCode.RET_SUCCESS);
                m_retData.Add("playerAcc", p.m_playerAcc);
                m_retData.Add("score", oriScore);

                /*string cmd = string.Format(CONST.SQL_ORDER_ID, TableName.GM_SCORE, m_gmUser.m_acc, p.m_playerAcc, p.m_op);

                Dictionary<string, object> order = m_gmUser.sqlDb.queryOne(cmd, MySqlDbName.DB_XIANXIA);
                if (order != null)
                {
                    int orderId = Convert.ToInt32(order["opId"]);
                    m_retData.Add("orderId", orderId);
                }*/

                OrderInfo oinfo = writeOfflineOrderToMySql(p, score);
                m_retData.Add("orderId", oinfo.m_orderId);
                m_retData.Add("userOrderId", p.m_userOrderId);
            }
        }
        else
        {
            m_retData.Add("result", code);
        }
        return Helper.genJsonStr(m_retData);
    }

    private bool doScorePlayer(ParamScore p, long score)
    {
        bool res = false;
        if (p.m_op == 0) // 加分
        {
            if (m_gmUser.m_money < score)
            {
                m_retData.Add("result", RetCode.RET_MONEY_NOT_ENOUGH);
                return false;
            }

            res = addScorePlayer(p.m_playerAcc, score);
            if (res)
            {
                decScore(score);
            }
            else
            {
                m_retData.Add("result", RetCode.RET_DB_ERROR);
                return false;
            }
        }
        else
        {
            res = decScorePlayer(p.m_playerAcc, score);
            if (res)
            {
                addScore(score);
            }
            else
            {
                m_retData.Add("result", RetCode.RET_DB_ERROR);
                return false;
            }
        }

        /*if (res)
        {
            long remainMoney = Helper.getRemainMoney(p.m_playerAcc, m_gmUser);
            SqlInsertGenerator gen = new SqlInsertGenerator();
            gen.addField("opTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
            gen.addField("opSrc", m_gmUser.m_acc, FieldType.TypeString);
            gen.addField("opDst", p.m_playerAcc, FieldType.TypeString);
            gen.addField("opType", p.m_op, FieldType.TypeNumber);
            gen.addField("opScore", score, FieldType.TypeNumber);
            gen.addField("moneyType", m_gmUser.m_moneyType, FieldType.TypeNumber);
            gen.addField("opSrcDepth", m_gmUser.m_depth, FieldType.TypeNumber);
            gen.addField("opSrcCreateCode", m_gmUser.m_createCode, FieldType.TypeString);
            gen.addField("opDstType", AccType.ACC_PLAYER, FieldType.TypeNumber);
            gen.addField("opDstRemainMoney", remainMoney, FieldType.TypeNumber);
            gen.addField("userOrderId", p.m_userOrderId, FieldType.TypeString);

            string cmd = gen.getResultSql(TableName.GM_SCORE);
            m_gmUser.sqlDb.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        }*/
        return res;
    }

    private bool addScore(long score)
    {
        string cmd = string.Format(SqlStrCMD.SQL_ADD_SCORE_TO_MGR_DIRECT,
                                    TableName.GM_ACCOUNT,
                                    score,
                                    m_gmUser.m_acc);

        int count = m_gmUser.sqlDb.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            m_gmUser.m_money += score;
        }
        return count > 0;
    }

    // score传正数
    private bool decScore(long score)
    {
        string cmd = string.Format(SqlStrCMD.SQL_DEC_SCORE_TO_MGR_DIRECT,
                                    TableName.GM_ACCOUNT,
                                    score,
                                    m_gmUser.m_acc,
                                    score);
        int count = m_gmUser.sqlDb.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            m_gmUser.m_money -= score;
        }
        return count > 0;
    }

    private bool addScorePlayer(string acc, long score)
    {
        string cmd = string.Format(SqlStrCMD.SQL_ADD_SCORE_TO_PLAYER,
                                    TableName.PLAYER_ACCOUNT_XIANXIA,
                                    score,
                                    acc,
                                    PlayerState.STATE_IDLE,
                                    m_gmUser.m_acc);

        int count = m_gmUser.sqlDb.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        return count > 0;
    }

    // score传正数
    private bool decScorePlayer(string acc, long score)
    {
        string cmd = string.Format(SqlStrCMD.SQL_DEC_SCORE_TO_PLAYER,
                                      TableName.PLAYER_ACCOUNT_XIANXIA,
                                      score,
                                      acc,
                                      PlayerState.STATE_IDLE,
                                      score,
                                      m_gmUser.m_acc);
        int count = m_gmUser.sqlDb.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        return count > 0;
    }

    // 是否可以对玩家账号进行上分下分操作
    private int canDoScoreToPlayer(string acc, long score, bool isAddScore)
    {
        Player player = new Player(acc, m_gmUser);
        if (!player.m_isExists)
            return RetCode.RET_NO_PLAYER;

        if (player.isAccStop())
            return RetCode.RET_ACC_BLOCKED;

        if (!player.isOwner(m_gmUser.m_acc))
        {
            return RetCode.RET_NO_RIGHT;
        }

        if (isAddScore) // 给玩家上分时，判断GM余额是否足够
        {
            if (m_gmUser.m_money < score)
            {
                return RetCode.RET_MONEY_NOT_ENOUGH;
            }
        }
        else
        {
            if (!player.isMoneyEnough(score)) // 下分时，判定玩家余额是否充足
                return RetCode.RET_MONEY_NOT_ENOUGH;
        }

        Dictionary<string, object> data = Helper.getPlayerPropertyByAcc(acc, new string[] { "SyncLock" });
        if (data != null)
        {
            if (data.ContainsKey("SyncLock"))
            {
                int state = Convert.ToInt32(data["SyncLock"]);
                if (state == 2)
                {
                    return RetCode.RET_PLYAER_LOCKED;
                }
            }
        }

        if (player.isInGame())
            return RetCode.RET_PLAYER_ONLINE;

        return RetCode.RET_SUCCESS;
    }

    private OrderInfo writeOfflineOrderToMySql(ParamScore p, long score)
    {
        long remainMoney = Helper.getRemainMoney(p.m_playerAcc, m_gmUser);

        OrderInfo oinfo =
                    OrderGenerator.genOfflineSuccessOrder(p.m_gmAccount, p.m_playerAcc, score,
                    p.m_op, AccType.ACC_PLAYER, remainMoney, OrderGenerator.ORDER_FROM_API, p.m_userOrderId);
        // 生成上下分记录
        string cmd = OrderGenerator.genSqlForLogScore(oinfo, m_gmUser.m_createCode, m_gmUser.m_money);
        m_gmUser.sqlDb.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        return oinfo;
    }

    // 订单id是否处理过
    private bool orderIdExists(ParamScore p)
    {
        List<IMongoQuery> queryList = new List<IMongoQuery>();
        queryList.Add(Query.EQ("gmAcc", BsonValue.Create(p.m_gmAccount)));
        queryList.Add(Query.EQ("apiOrderId", BsonValue.Create(p.m_userOrderId)));
        IMongoQuery imq = Query.And(queryList);
        bool res = MongodbPlayer.Instance.KeyExistsByQuery(TableName.PLAYER_ORDER_REQ, imq);
        if (res)
        {
            return true;
        }

        string cond = string.Format(" userOrderId='{0}' and opSrc='{1}' ", p.m_userOrderId, p.m_gmAccount);
        res = m_gmUser.sqlDb.keyExists(TableName.GM_SCORE, cond, MySqlDbName.DB_XIANXIA);
        return res;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamOnline : ParamBase
{
    public ParamOnline()
    {
        m_fieldIndex = 2;
    }
}

// 玩家是否在线
public class DyOpOnline : DyOpBase
{
    public override string doDyop(object param)
    {
        ParamOnline p = (ParamOnline)param;

        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }

        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }
        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        m_retData.Add("result", RetCode.RET_SUCCESS);
        m_retData.Add("playerAcc", p.m_playerAcc);
        m_retData.Add("online", player.isInGame());
        return Helper.genJsonStr(m_retData);
    }
}

//////////////////////////////////////////////////////////////////////////

// 踢玩家下线
public class ParamKickPlayer : ParamBase
{
    public string m_timeStr;

    public int m_time;  // 单位秒，多长时间以内不能重新登录

    public ParamKickPlayer()
    {
        m_fieldIndex = 2;    
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_timeStr))
            return false;

        if (!int.TryParse(m_timeStr, out m_time))
        {
            return false;
        }

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_playerAcc + m_timeStr + user.m_devSecretKey);
        return m_sign == sign;
    }
}

// 踢玩家
public class DyOpKickPlayer : DyOpBase
{
    public override string doDyop(object param)
    {
        ParamKickPlayer p = (ParamKickPlayer)param;

        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }

        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }
        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }
        /*if (!player.isInGame()) // 玩家不在游戏内
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
            return Helper.genJsonStr(m_retData);
        }*/

       /* string url = string.Format(CONST.URL_KICK_PLAYER, p.m_playerAcc, p.m_time);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            Dictionary<string, object> retData =
                JsonHelper.ParseFromStr<Dictionary<string, object>>(retStr);
            if (retData.ContainsKey("result"))
            {
                m_retData.Add("result", retData["result"]);
            }
        }*/
        kickPlayer(p);
        return Helper.genJsonStr(m_retData);
    }

    void kickPlayer(ParamKickPlayer p)
    {
        var ret = MongodbPlayer.Instance.ExecuteGetBykey(TableName.PLAYER_INFO, "account", p.m_playerAcc, new string[] { "SyncLock" });
        if (ret == null)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return;
        }

        int state = Convert.ToInt32(ret["SyncLock"]);
        if (state != 1)
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
            return;
        }

        int nt = p.m_time;
        if (nt < 480) nt = 480;

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["time"] = nt;

        string err = MongodbPlayer.Instance.ExecuteStoreBykey(TableName.KICK_PLAYER, "key", p.m_playerAcc, data);
        if (string.IsNullOrEmpty(err))
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
        }
        else
        {
            m_retData.Add("result", RetCode.RET_DB_ERROR);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 解锁玩家
public class ParamUnLockPlayer : ParamBase
{
    public ParamUnLockPlayer()
    {
        m_fieldIndex = 2;  
    }
}

// 解锁玩家
public class DyOpUnLockPlayer : DyOpBase
{
    public override string doDyop(object param)
    {
        ParamUnLockPlayer p = (ParamUnLockPlayer)param;

        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }

        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }
        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }
        if (player.isInGame()) // 玩家在游戏内
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
            return Helper.genJsonStr(m_retData);
        }
        unlock(p.m_playerAcc);

       /* string url = string.Format(CONST.URL_UNLOCK_PLAYER, p.m_playerAcc);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            Dictionary<string, object> retData =
                JsonHelper.ParseFromStr<Dictionary<string, object>>(retStr);
            if (retData.ContainsKey("result"))
            {
                int retCode = Convert.ToInt32(retData["result"]);

                m_retData.Add("result", retCode);

                if (retCode == RetCode.RET_SUCCESS && 
                    retData.ContainsKey("info"))
                {
                    string retURL = string.Format(CONST.URL_UNLOCK_PLAYER_RET, p.m_playerAcc, retData["info"]);
                    HttpPost.Get(new Uri(retURL));
                }
            }
        }*/

        return Helper.genJsonStr(m_retData);
    }

    void unlock(string acc)
    {
        var ret = MongodbPlayer.Instance.ExecuteGetBykey(TableName.PLAYER_INFO, "account", acc, new string[] { "SyncLock", "gold" });
        if (ret == null)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return;
        }

        if (!ret.ContainsKey("SyncLock"))
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
            return;
        }
        int state = Convert.ToInt32(ret["SyncLock"]);
        if (state == 1 || state == 0) // 正常情况
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
            return;
        }

        string retURL = string.Format(CONST.URL_UNLOCK_PLAYER_RET, acc, ret["gold"]);
        byte[] byarr = HttpPost.Get(new Uri(retURL));
        string retStr = Encoding.UTF8.GetString(byarr);
        if (retStr == "ok")
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["SyncLock"] = (sbyte)0;
            data["gold"] = 0;

            string err = MongodbPlayer.Instance.ExecuteUpdate(TableName.PLAYER_INFO, "account", acc, data);
            if (string.IsNullOrEmpty(err))
            {
                m_retData.Add("result", RetCode.RET_SUCCESS);
            }
            else
            {
                m_retData.Add("result", RetCode.RET_DB_ERROR);
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 更新玩家信息
public class ParamUpdatePlayerInfo : ParamBase
{
    SqlUpdateGenerator m_gen = new SqlUpdateGenerator();

    public ParamUpdatePlayerInfo()
    {
        m_fieldIndex = 2;  
    }

    public void addField(HttpRequest Request)
    {
        foreach(var key in Request.QueryString.AllKeys)
        {
            if (DbFieldSet.getInstance().hasField(key))
            {
                m_gen.addField(key, Request.QueryString[key], DbFieldSet.getInstance().getDbField(key).m_fieldType);
            }
        }
    }

    public string genUpdateSql()
    {
        string sql = m_gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
                                        string.Format(" acc='{0}' ", m_playerAcc));
        return sql;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (m_gen.count() == 0) // 没有可以更新的信息
            return false;

        return true;
    }
}

// 更新玩家信息
public class DyOpUpdatePlayerInfo : DyOpBase
{
    public override string doDyop(object param)
    {
        ParamUpdatePlayerInfo p = (ParamUpdatePlayerInfo)param;

        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }

        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }
        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        string cmd = p.genUpdateSql();
        int count = m_gmUser.sqlDb.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
        }
        else
        {
            m_retData.Add("result", RetCode.RET_DB_ERROR);
        }
        return Helper.genJsonStr(m_retData);
    }
}

//////////////////////////////////////////////////////////////////////////

// 清理登录失败次数
public class DyOpClearLoginFailedCount : DyOpBase
{
    public override string doDyop(object param)
    {
        ParamBase p = (ParamBase)param;
        p.fieldIndex = 2;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }

        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }
        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        string url = string.Format(CONST.URL_CLEAR_FAILED_LOGIN, p.m_playerAcc);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            if (retStr == "0")
            {
                m_retData.Add("result", RetCode.RET_SUCCESS);
            }
            else
            {
                m_retData.Add("result", RetCode.RET_OP_FAILED);
            }
        }
        else
        {
            m_retData.Add("result", RetCode.RET_OP_FAILED);
        }
        return Helper.genJsonStr(m_retData);
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamModifyPlayerPwd : ParamBase
{
    // 老密码
    public string m_oldPwd = "";
    // 新设置的密码
    public string m_newPwd = "";

    public ParamModifyPlayerPwd()
    {
        m_fieldIndex = 2;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        //if (string.IsNullOrEmpty(m_oldPwd))
        //    return false;

        if (string.IsNullOrEmpty(m_newPwd))
            return false;

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_playerAcc + m_oldPwd + m_newPwd + user.m_devSecretKey);
        return m_sign == sign;
    }
}

// 修改玩家密码
public class DyOpModifyPlayerPwd : DyOpBase
{
    const string AES_KEY = "959D!@23ia@!#86e";

    public override string doDyop(object param)
    {
        ParamModifyPlayerPwd p = (ParamModifyPlayerPwd)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        if (m_gmUser.m_accType != AccType.ACC_API)
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        if (!Regex.IsMatch(p.m_newPwd, Exp.ACCOUNT_PLAYER_PWD))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }

        string error = modifyPwd(p);
        if (error == "err_success")
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
            m_retData.Add("playerAcc", p.m_playerAcc);
        }
        else if (error == "err_pwd_error") // 原密码错误
        {
            m_retData.Add("result", RetCode.RET_ACC_PWD_FORMAT_ERROR);
            m_retData.Add("playerAcc", p.m_playerAcc);
        }
        else
        {
            m_retData.Add("result", RetCode.RET_OP_FAILED);
            m_retData.Add("playerAcc", p.m_playerAcc);
        }
        return Helper.genJsonStr(m_retData);
    }

    private string modifyPwd(ParamModifyPlayerPwd p)
    {
        RSAHelper rsa = new RSAHelper();
        rsa.init();
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["n1"] = p.m_playerAcc;
        string old = Tool.getMD5Hash(p.m_oldPwd);
        data["n2"] = AESHelper.AESEncrypt(old, AES_KEY);

        string newPwd = Tool.getMD5Hash(p.m_newPwd);
        data["n3"] = AESHelper.AESEncrypt(newPwd, AES_KEY);

        string jsonstr = JsonHelper.ConvertToStr(data);
        string md5 = AESHelper.MD5Encrypt(jsonstr + AES_KEY);
        string urlstr = Convert.ToBase64String(Encoding.Default.GetBytes(jsonstr));

        string fmt = CONST.URL_MODIFY_PLAYER_PWD;
        string aspx = string.Format(fmt, urlstr, md5);
        var ret = HttpPost.Get(new Uri(aspx));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            return retStr;
        }
        return "";
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamPlayerOp : ParamBase
{
    public string m_opStr;
    
    // 0 停封账号 1 解封账号
    public int m_op;

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_opStr))
            return false;

        if (!int.TryParse(m_opStr, out m_op))
            return false;

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_playerAcc + m_opStr + user.m_devSecretKey);
        return m_sign == sign;
    }
}

// 玩家相关操作
public class DyOpPlayerOp : DyOpBase
{
    public override string doDyop(object param)
    {
        ParamPlayerOp p = (ParamPlayerOp)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }
        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        int retCode = RetCode.RET_PARAM_NOT_VALID;
        switch (p.m_op)
        {
            case 0:
                {
                    retCode = stopPlayerAcc(p);
                }
                break;
            case 1:
                {
                    retCode = startPlayerAcc(p);
                }
                break;
        }

        m_retData.Add("result", retCode);
        return Helper.genJsonStr(m_retData);
    }

    // 停封玩家账号
    int stopPlayerAcc(ParamPlayerOp p)
    {
        SqlUpdateGenerator gen = new SqlUpdateGenerator();

        gen.addField("enable", 0, FieldType.TypeNumber);
        string sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
             string.Format(" acc='{0}' ", p.m_playerAcc));
        int count = m_gmUser.sqlDb.executeOp(sql, MySqlDbName.DB_XIANXIA);
        return count > 0 ? RetCode.RET_SUCCESS : RetCode.RET_DB_ERROR;
    }

    // 解封玩家账号
    int startPlayerAcc(ParamPlayerOp p)
    {
        SqlUpdateGenerator gen = new SqlUpdateGenerator();

        gen.addField("enable", 1, FieldType.TypeNumber);
        string sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
             string.Format(" acc='{0}' ", p.m_playerAcc));
        int count = m_gmUser.sqlDb.executeOp(sql, MySqlDbName.DB_XIANXIA);
        return count > 0 ? RetCode.RET_SUCCESS : RetCode.RET_DB_ERROR;
    }
}
