using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Web.Configuration;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;
using System.Web;

// 动态操作
class DyOpMgr : SysBase
{
    private Dictionary<DyOpType, DyOpBase> m_items = new Dictionary<DyOpType, DyOpBase>();

    public DyOpMgr()
    {
        m_sysType = SysType.sysTypeDyOp;
    }

    public OpRes doDyop(object param, DyOpType type, GMUser user)
    {
        if (!m_items.ContainsKey(type))
        {
            LOGW.Info("DyOpMgr.doDyop不存在操作类型[{0}]", type);
            return OpRes.op_res_failed;
        }
        return m_items[type].doDyop(param, user);
    }

    public DyOpBase getDyOp(DyOpType type)
    {
        if (!m_items.ContainsKey(type))
        {
            LOGW.Info("DyOpMgr.getDyOp不存在操作类型[{0}]", type);
            return null;
        }
        return m_items[type];
    }

    public object getResult(DyOpType type)
    {
        if (!m_items.ContainsKey(type))
        {
            LOGW.Info("DyOpMgr.getDyOp不存在操作类型[{0}]", type);
            return null;
        }
        return m_items[type].getResult();
    }

    public override void initSys()
    {
        //m_items.Add(DyOpType.opTypeSendMail, new DyOpSendMail());
        m_items.Add(DyOpType.opTypeModifyPwd, new DyOpModifyPwd());
        m_items.Add(DyOpType.opTypeBlockAcc, new DyOpBlockAccount());
        //m_items.Add(DyOpType.opTypeBlockId, new DyOpBlockId());
        //m_items.Add(DyOpType.opTypeBlockIP, new DyOpBlockIP());

        //m_items.Add(DyOpType.opTypeRecharge, new DyOpRecharge());
        //m_items.Add(DyOpType.opTypePushApp, new DyOpJPushAddApp());
        //m_items.Add(DyOpType.opTypeBindPhone, new DyOpBindPhone());
        //m_items.Add(DyOpType.opTypeGift, new DyOpGift());
        //m_items.Add(DyOpType.opTypeGiftCode, new DyOpGiftCode());

        //m_items.Add(DyOpType.opTypeExchange, new DyOpExchange());
        //m_items.Add(DyOpType.opTypeNotify, new DyOpNotify());
        //m_items.Add(DyOpType.opTypeMaintenance, new DyOpMaintenance());
        m_items.Add(DyOpType.opTypeFishlordParamAdjust, new DyOpFishlordParamAdjust());
        m_items.Add(DyOpType.opTypeFishParkParamAdjust, new DyOpFishParkParamAdjust());

        m_items.Add(DyOpType.opTypeCrocodileParamAdjust, new DyOpCrocodileParamAdjust());

        m_items.Add(DyOpType.opTypeClearFishTable, new DyOpClearFishTable());
        m_items.Add(DyOpType.opTypeReLoadTable, new DyOpReLoadTable());
        m_items.Add(DyOpType.opTypeDiceParamAdjust, new DyOpDiceParamAdjust());
        m_items.Add(DyOpType.opTypeServiceInfo, new DyOpServiceInfo());
        m_items.Add(DyOpType.opTypeFreezeHead, new DyOpFreezeHead());

        //m_items.Add(DyOpType.opTypeEditChannel, new DyOpAddChannel());
        m_items.Add(DyOpType.opTypeBaccaratParamAdjust, new DyOpBaccaratParamAdjust());
        //m_items.Add(DyOpType.opTypeSpeaker, new DyOpSpeaker());
        m_items.Add(DyOpType.opTypeCowsParamAdjust, new DyOpCowsParamAdjust());
        m_items.Add(DyOpType.opTypeSetCowsCard, new DyOpAddCowsCardType());
        m_items.Add(DyOpType.opTypeDyOpGameResult, new DyOpGameResult());
        
        m_items.Add(DyOpType.opTypeDyOpCreateGmAccount, new DyOpCreateGmAccount());
        m_items.Add(DyOpType.opTypeDyOpCreatePlayer, new DyOpCreatePlayer());
        m_items.Add(DyOpType.opTypeDyOpScore, new DyOpScore());
        m_items.Add(DyOpType.opTypeModifyLoginPwd, new DyOpModifyLoginPwd());

        m_items.Add(DyOpType.opTypeKickPlayer, new DyOpKickPlayer());
        m_items.Add(DyOpType.opTypeUnlockPlayer, new DyOpUnLockPlayer());
        m_items.Add(DyOpType.opTypeClearLoginFailed, new DyOpClearLoginFailedCount());

        m_items.Add(DyOpType.opTypeSetPlayerSpecialFlag, new DyOpSetPlayerSpecialFlag());
        
        m_items.Add(DyOpType.opTypeDyOpModiyGmRight, new DyOpModifyGmRight());
        m_items.Add(DyOpType.opTypeDyOpStartStopGmAcc, new DyOpStartStopGmAcc());

        m_items.Add(DyOpType.opTypeDragonParamAdjust, new DyOpDragonParamAdjust());

        m_items.Add(DyOpType.opTypeDyOpApiApprove, new DyOpApiApprove());
        m_items.Add(DyOpType.opTypeDyOpPlayerOrder, new DyOpPlayerOrder());
        m_items.Add(DyOpType.opTypeWishCurse, new DyOpAddWishCurse());
        m_items.Add(DyOpType.opTypeDyOpModifyGmProperty, new DyOpModifyGmProperty());
        m_items.Add(DyOpType.opTypeDelData, new DyOpDelData());

        m_items.Add(DyOpType.opTypeDelAccount, new DyOpDelAccount());
        m_items.Add(DyOpType.opTypeGameParamAdjust, new DyOpGameParamAdjust());

        m_items.Add(DyOpType.opTypeOpenGame, new DyOpOpenGame());
        m_items.Add(DyOpType.opTypeModifyMaxBetLimit, new DyOpModifyMaxBetLimit());

        m_items.Add(DyOpType.opTypeModifyAPISetLimit, new DyOpModifyAPISetLimit());
        m_items.Add(DyOpType.opTypePlayerOp, new DyOpPlayerOp());
    }
}

//////////////////////////////////////////////////////////////////////////

// GM的动态操作
public class DyOpBase
{
    public virtual OpRes doDyop(object param, GMUser user)
    {
        return OpRes.op_res_failed;
    }

    public virtual object getResult() { return null; }
}

//////////////////////////////////////////////////////////////////////////

public class ParamSendMail
{
    public string m_title = "";
    public string m_sender = "";
    public string m_content = "";
    public string m_toPlayer = "";
    public string m_itemList = "";
    public string m_validDay = "";
    public int m_target;
    public bool m_isCheck = false;

    // 条件，下线时间
    public string m_condLogoutTime = "";
    // 条件，vip等级区间
    public string m_condVipLevel = "";

    public string m_comment = "";
    public string m_result = "";
}

public class ParamCheckMail : ParamSendMail
{
    public string m_id = "";
}

public class DyOpSendMail : DyOpBase
{
    private string m_successPlayer = "";

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamSendMail p = (ParamSendMail)param;

        int days = 7;
        List<int> playerList = new List<int>();
        List<ParamItem> tmpItem = new List<ParamItem>();
        OpRes code = checkValid(p, user, ref days, tmpItem, playerList);
        if (code != OpRes.opres_success)
            return code;

        code = sendRewardCheck(user, tmpItem);
        if (code != OpRes.opres_success)
            return code;

        if (p.m_isCheck) // 缓存
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("id", Guid.NewGuid().ToString());
            data.Add("title", p.m_title);
            data.Add("sender", p.m_sender);
            data.Add("content", p.m_content);
            data.Add("validDay", p.m_validDay);
            data.Add("toPlayer", p.m_toPlayer);
            data.Add("itemList", p.m_itemList);
            data.Add("target", p.m_target);
            data.Add("time", DateTime.Now);
            data.Add("logOutTime", p.m_condLogoutTime);
            data.Add("vipLevel", p.m_condVipLevel);
            data.Add("comment", p.m_comment);
            bool res = DBMgr.getInstance().insertData(TableName.CHECK_MAIL, data, user.getDbServerID(), DbName.DB_PLAYER);
            return res ? OpRes.opres_success : OpRes.op_res_failed;
        }

        if (p.m_target == 0) // 给指定玩家发送
        {
            BsonDocument mailItem = null;

            if (p.m_itemList != "")
            {
                Dictionary<string, object> dd = new Dictionary<string, object>();
                for (int i = 0; i < tmpItem.Count; i++)
                {
                    Dictionary<string, object> tmpd = new Dictionary<string, object>();
                    tmpd.Add("giftId", tmpItem[i].m_itemId);
                    tmpd.Add("count", tmpItem[i].m_itemCount);
                    tmpd.Add("receive", false);
                    dd.Add(i.ToString(), tmpd.ToBsonDocument());
                }
                mailItem = dd.ToBsonDocument();
            }

            return specialSend(p, user, days, mailItem, playerList);
        }
        
        return fullSend(p, user, days);
    }

    // 返回所有待检测邮件列表
    public void getCheckMailList(GMUser user, List<ParamCheckMail> result)
    {
        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.CHECK_MAIL, user.getDbServerID(), DbName.DB_PLAYER);
        if (data == null || data.Count == 0)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            ParamCheckMail tmp = new ParamCheckMail();
            result.Add(tmp);

            tmp.m_id = Convert.ToString(data[i]["id"]);
            tmp.m_title = Convert.ToString(data[i]["title"]);
            tmp.m_sender = Convert.ToString(data[i]["sender"]);
            tmp.m_content = Convert.ToString(data[i]["content"]);
            tmp.m_validDay = Convert.ToString(data[i]["validDay"]);
            tmp.m_toPlayer = Convert.ToString(data[i]["toPlayer"]);
            tmp.m_itemList = Convert.ToString(data[i]["itemList"]);
            tmp.m_target = Convert.ToInt32(data[i]["target"]);
            tmp.m_result = Convert.ToDateTime(data[i]["time"]).ToLocalTime().ToString();
            tmp.m_condLogoutTime = Convert.ToString(data[i]["logOutTime"]);
            tmp.m_condVipLevel = Convert.ToString(data[i]["vipLevel"]);
            if (data[i].ContainsKey("comment"))
            {
                tmp.m_comment = Convert.ToString(data[i]["comment"]);
            }
        }
    }

    public Dictionary<string, object> getCheckMail(GMUser user, string id)
    {
        return DBMgr.getInstance().getTableData(TableName.CHECK_MAIL, "id", id, user.getDbServerID(), DbName.DB_PLAYER);
    }

    public void removeCheckMail(GMUser user, string id)
    {
        DBMgr.getInstance().remove(TableName.CHECK_MAIL, "id", id, user.getDbServerID(), DbName.DB_PLAYER);
    }

    private OpRes specialSend(ParamSendMail p, GMUser user, int days, BsonDocument mailItem, List<int> playerList)
    {
        bool res = false;
        m_successPlayer = "";
        DateTime now = DateTime.Now;
        DateTime nt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

        List<Dictionary<string, object>> docList = new List<Dictionary<string, object>>();

        for (int i = 0; i < playerList.Count; i++)
        {
            res = DBMgr.getInstance().keyExists(TableName.PLAYER_INFO, "player_id", playerList[i], user.getDbServerID(), DbName.DB_PLAYER);
            if (!res)
            {
                p.m_result += playerList[i] + " ";
                continue;
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("title", p.m_title);
            data.Add("sender", p.m_sender);
            data.Add("content", p.m_content);

            data.Add("time", nt);
            data.Add("deadTime", nt.AddDays(days));
            data.Add("isReceive", false);
            data.Add("playerId", playerList[i]);

            // 标识是系统发送的邮件
            data.Add("senderId", 0);

            if (mailItem != null)
            {
                data.Add("gifts", mailItem);
            }
            m_successPlayer += playerList[i] + " ";
            docList.Add(data);
        }
        res = DBMgr.getInstance().insertData(TableName.PLAYER_MAIL, docList, user.getDbServerID(), DbName.DB_PLAYER);
        if (res)
        {
//             OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_SEND_MAIL, 
//                 new LogSendMail(p.m_title, p.m_sender, p.m_content, m_successPlayer, p.m_itemList, days), 
//                 user,
//                 p.m_comment);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    // 全局发放
    private OpRes fullSend(ParamSendMail p, GMUser user, int days)
    {
        ParamSendMailFullSvr param = new ParamSendMailFullSvr();
        param.m_dbServerIP = user.m_dbIP;
        param.m_title = p.m_title;
        param.m_sender = p.m_sender;
        param.m_content = p.m_content;
        param.m_itemList = p.m_itemList;
        param.m_validDay = days;
        param.m_condition = new Dictionary<string, object>();
        if (p.m_condLogoutTime != "")
        {
            param.m_condition.Add("logOutTime", p.m_condLogoutTime);
        }
        if (p.m_condVipLevel != "")
        {
            param.m_condition.Add("vipLevel", p.m_condVipLevel);
        }

        OpRes res = RemoteMgr.getInstance().reqSendMail(param);
        if (res == OpRes.opres_success)
        {
//             OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_SEND_MAIL, 
//                 new LogSendMail(p.m_title, p.m_sender, p.m_content, "", p.m_itemList, days),
//                 user,
//                 p.m_comment);
        }
        return res;
    }

    // 邮件的合法性检验
    private OpRes checkValid(ParamSendMail p, GMUser user, ref int days, List<ParamItem> itemList, List<int> playerList)
    {
        if (!string.IsNullOrEmpty(p.m_validDay))
        {
            if (!int.TryParse(p.m_validDay, out days))
            {
                return OpRes.op_res_param_not_valid;
            }
        }

        if (p.m_itemList != "")
        {
            if (itemList != null)
            {
                bool res = Tool.parseItemList(p.m_itemList, itemList);
                if (!res)
                {
                    return OpRes.op_res_param_not_valid;
                }

                for (int i = 0; i < itemList.Count; i++)
                {
                    var t = ItemCFG.getInstance().getValue(itemList[i].m_itemId);
                    if (t == null)
                    {
                        p.m_result += itemList[i].m_itemId + " ";
                    }
                }

                if (p.m_result != "")
                    return OpRes.op_res_item_not_exist;
            }
            else
            {
                if (!Tool.isItemListValid(p.m_itemList, true))
                    return OpRes.op_res_param_not_valid;
            }
        }

        if (p.m_target == 0) // 给指定玩家
        {
            bool res = Tool.parseNumList(p.m_toPlayer, playerList);
            if (!res)
                return OpRes.op_res_param_not_valid;

            for (int i = 0; i < playerList.Count; i++)
            {
                res = DBMgr.getInstance().keyExists(TableName.PLAYER_INFO, "player_id", playerList[i], user.getDbServerID(), DbName.DB_PLAYER);
                if (!res)
                {
                    p.m_result += playerList[i] + " ";
                }
            }

            if (p.m_result != "")
                return OpRes.op_res_player_not_exist;

            if (p.m_condVipLevel != "")
                return OpRes.op_res_param_not_valid;

             if (p.m_condLogoutTime != "")
                 return OpRes.op_res_time_format_error;
        }
        else // 全服发放
        {
            if (p.m_condVipLevel != "")
            {
                if (!Tool.isTwoNumValid(p.m_condVipLevel))
                    return OpRes.op_res_param_not_valid;
            }

            if (p.m_condLogoutTime != "")
            {
                DateTime mint = DateTime.Now, maxt = DateTime.Now;
                bool res = Tool.splitTimeStr(p.m_condLogoutTime, ref mint, ref maxt);
                if (!res)
                    return OpRes.op_res_time_format_error;
            }
        }

        return OpRes.opres_success;
    }

    private OpRes sendRewardCheck(GMUser user, List<ParamItem> itemList)
    {
        double val = 0.0;
        foreach (var item in itemList)
        {
            val += transToRMB(item.m_itemId, item.m_itemCount);
        }

        OpRightInfo info = null; // ResMgr.getInstance().getOpRightInfo(user.m_type);
        if (info == null)
            return OpRes.op_res_reward_beyond_limit;

        if (info.m_sendRewardLimit == 0) // 0表示没有限制
            return OpRes.opres_success;

        if(val > info.m_sendRewardLimit)
            return OpRes.op_res_reward_beyond_limit;
        return OpRes.opres_success;
    }

    private double transToRMB(int itemId, int count)
    {
        double val = 0.0;
        int r = 0;
        if (itemId == 1) // 金币
        {
            r = 10000;
        }
        else if (itemId == 2) // 礼券
        {
            r = 10;
        }
        if (r > 0)
        {
            val = (double)count / r;
        }
        return val;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamModifyPwd
{
    public string m_account = "";
    public string m_phone = "";

    // 玩家ID
    public string m_playerId = "";
    public string m_newPwd = "";
    public int m_pwdType;
}

public class DyOpModifyPwd : DyOpPlayerBase
{
  //  static string[] m_fields = { "phone", "acc" };

    // player_info中的
    static string[] s_fields1 = { "account", "platform" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamModifyPwd p = (ParamModifyPwd)param;
        string acc = p.m_account;
        if (!canOpPlayer(acc, user))
            return OpRes.op_res_no_right;

        if (!Regex.IsMatch(p.m_newPwd, Exp.ACCOUNT_PLAYER_PWD))
        {
            return OpRes.op_res_param_not_valid;
        }

       /* int playerId = 0;

        if (!int.TryParse(p.m_playerId, out playerId))
            return OpRes.op_res_param_not_valid;

        if(string.IsNullOrEmpty(p.m_newPwd))
            return OpRes.op_res_param_not_valid;

        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.PLAYER_INFO,
                "player_id", playerId,
                s_fields1, user.getDbServerID(), DbName.DB_PLAYER);
        if (data == null)
            return OpRes.op_res_not_found_data;

        if (p.m_pwdType == 1) // 保险箱密码
        {
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            tmp.Add("safeBoxPwd", Tool.getMD5Hash(p.m_newPwd));
            bool res1 = DBMgr.getInstance().update(TableName.PLAYER_INFO, tmp,
                "player_id", playerId, user.getDbServerID(), DbName.DB_PLAYER);
            return res1 ? OpRes.opres_success : OpRes.op_res_failed; 
        }

        if (!data.ContainsKey("platform"))
        {
            return OpRes.op_res_failed;
        }

        if (!data.ContainsKey("account"))
        {
            return OpRes.op_res_failed;
        }

        string plat = Convert.ToString(data["platform"]);
        if (plat != "default")
            return OpRes.op_res_third_part_platform;

        string acc = Convert.ToString(data["account"]);
        */
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        bool res = DBMgr.getInstance().keyExists(TableName.PLAYER_ACCOUNT, "acc", acc,
            serverId, DbName.DB_ACCOUNT);
        if (!res)
            return OpRes.op_res_not_found_data;

        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("pwd", Tool.getMD5Hash(p.m_newPwd));
        res = DBMgr.getInstance().update(TableName.PLAYER_ACCOUNT, upData, "acc", acc, serverId, DbName.DB_ACCOUNT);
        if (res)
        {
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_RESET_PWD, new LogResetPwd(acc), user);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    /*public override OpRes _doDyop(object param, GMUser user)
    {
        ParamModifyPwd p = (ParamModifyPwd)param;

        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.PLAYER_ACCOUNT, "acc", p.m_account, m_fields, serverId, DbName.DB_ACCOUNT);
        if (data == null)
            return OpRes.op_res_not_found_data;

        if (!data.ContainsKey("phone"))
        {
            return OpRes.op_res_not_bind_phone;
        }

        p.m_phone = Convert.ToString(data["phone"]);
        OpRes code = sendMsgToPhone(p.m_phone);
        if (code == OpRes.opres_success)
        {
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_RESET_PWD, new LogResetPwd(p.m_account, p.m_phone), user);
        }
        return code;
    }*/

    private OpRes sendMsgToPhone(string phone)
    {
        try
        {
            string url = string.Format("{0}?phone={1}&not_often=0", WebConfigurationManager.AppSettings["findAccountWeb"], phone);
            var ret = HttpPost.Post(new Uri(url), null, null);
            if (ret != null)
            {
                string retStr = Encoding.UTF8.GetString(ret);
                if (retStr == "resSuccess")
                {
                    return OpRes.opres_success;
                }
            }
        }
        catch (System.Exception ex)
        {
        }
        return OpRes.op_res_failed;
    }

    // 取得截止时间
    public long calEndTime(DateTime now)
    {
        DateTime t = now.AddDays(1);
        DateTime e = new DateTime(t.Year, t.Month, t.Day, 0, 0, 0);
        return e.Ticks;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamBlock
{
    // 为true表示停封
    public bool m_isBlock;
    public string m_param;
    public string m_comment = "";
}

public class ResultBlock
{
    public string m_param = "";
    public string m_time = "";
}

// 停封账号
public class DyOpBlockAccount : DyOpPlayerBase
{
    static string[] s_fields = { "acc", "blockTime" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamBlock p = (ParamBlock)param;

        if (!canOpPlayer(p.m_param, user))
            return OpRes.op_res_no_right;

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        if (p.m_isBlock)
        {
            // 0禁用
            gen.addField("enable", 0, FieldType.TypeNumber);
        }
        else
        {
            // 1可用
            gen.addField("enable", 1, FieldType.TypeNumber);
        }

        string createCode = ItemHelp.getCreateCodeSpecial(user);
        string condition = string.Format(" acc='{0}' ", p.m_param);
        string sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA, condition);

        int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_ACC,
                new LogBlockAcc(p.m_param, p.m_isBlock), user);
        }
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    // 返回当前停封的所有账号
    public void getAccountList(GMUser user, List<ResultBlock> result)
    {
        int accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (accServerId == -1)
            return;

        IMongoQuery imq = Query.EQ("block", BsonValue.Create(true));
        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.PLAYER_ACCOUNT, accServerId, DbName.DB_ACCOUNT, imq, 0, 0, s_fields);
        for (int i = 0; i < data.Count; i++)
        {
            ResultBlock tmp = new ResultBlock();
            result.Add(tmp);
            tmp.m_param = Convert.ToString(data[i]["acc"]);
            tmp.m_time = Convert.ToDateTime(data[i]["blockTime"]).ToLocalTime().ToString();
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 停封Id
public class DyOpBlockId : DyOpBase
{
    static string[] s_fields = { "player_id", "blockTime" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamBlock p = (ParamBlock)param;
        bool res = false;

        if (p.m_isBlock)
        {
            int playerId = 0;
            if (!int.TryParse(p.m_param, out playerId))
            {
                return OpRes.op_res_param_not_valid;
            }

            IMongoQuery imq = Query.EQ("player_id", BsonValue.Create(playerId));

            long count = DBMgr.getInstance().getRecordCount(TableName.PLAYER_INFO, imq, user.getDbServerID(), DbName.DB_PLAYER);
            if (count == 0)
                return OpRes.op_res_not_found_data;

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["blockTime"] = DateTime.Now;
            data["delete"] = true;
            res = DBMgr.getInstance().update(TableName.PLAYER_INFO, data, imq, user.getDbServerID(), DbName.DB_PLAYER);
            if (res)
            {
//                 OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_ID, 
//                     new LogBlockId(p.m_param, p.m_isBlock), 
//                     user,
//                     p.m_comment);
            }
        }
        else
        {
            int playerId = 0;

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["delete"] = false;

            string[] str = Tool.split(p.m_param, ',');
            for (int i = 0; i < str.Length; i++)
            {
                if (!int.TryParse(str[i], out playerId))
                {
                    continue;
                }

                res = DBMgr.getInstance().update(TableName.PLAYER_INFO, data, "player_id", playerId, user.getDbServerID(), DbName.DB_PLAYER);
            }
            if (res)
            {
              //  OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_ID, new LogBlockId(p.m_param, p.m_isBlock), user);
            }
        }

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    // 返回当前停封的所有玩家ID
    public void getIdList(GMUser user, List<ResultBlock> result)
    {
        IMongoQuery imq = Query.EQ("delete", BsonValue.Create(true));
        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.PLAYER_INFO, user.getDbServerID(), DbName.DB_PLAYER, imq, 0, 0, s_fields);
        if (data == null)
            return;

        for (int i = 0; i < data.Count; i++)
        {
            ResultBlock tmp = new ResultBlock();
            result.Add(tmp);
            tmp.m_param = Convert.ToString(data[i]["player_id"]);
            tmp.m_time = Convert.ToDateTime(data[i]["blockTime"]).ToLocalTime().ToString();
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 停封IP
public class DyOpBlockIP : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamBlock p = (ParamBlock)param;
        bool res = false;
        int accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (accServerId == -1)
            return OpRes.op_res_failed;

        if (p.m_isBlock)
        {
            Match match = Regex.Match(p.m_param, Exp.IP_ADDRESS);
            if (!match.Success)
            {
                match = Regex.Match(p.m_param, Exp.IP_ADDRESS1);
                if (!match.Success)
                    return OpRes.op_res_param_not_valid;
            }

            string ip = match.Groups[1].Value;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["blockTime"] = DateTime.Now;
            data["ip"] = ip;
            res = DBMgr.getInstance().save(TableName.BLOCK_IP, data, "ip", ip, accServerId, DbName.DB_ACCOUNT);
            if (res)
            {
//                 OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_IP,
//                     new LogBlockIP(ip, p.m_isBlock),
//                     user,
//                     p.m_comment);
            }
        }
        else
        {
            string[] str = Tool.split(p.m_param, ',');
            for (int i = 0; i < str.Length; i++)
            {
                res = DBMgr.getInstance().remove(TableName.BLOCK_IP, "ip", str[i], accServerId, DbName.DB_ACCOUNT);
            }
            if (res)
            {
                //OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_IP, new LogBlockIP(p.m_param, p.m_isBlock), user);
            }
        }

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    public void getIPList(GMUser user, List<ResultBlock> result)
    {
        int accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (accServerId == -1)
            return;

        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.BLOCK_IP, accServerId, DbName.DB_ACCOUNT);
        for (int i = 0; i < data.Count; i++)
        {
            ResultBlock tmp = new ResultBlock();
            result.Add(tmp);
            tmp.m_param = Convert.ToString(data[i]["ip"]);
            tmp.m_time = Convert.ToDateTime(data[i]["blockTime"]).ToLocalTime().ToString();
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamRecharge
{
    public int m_rtype;
    public string m_playerId = "";
    public string m_param = "";
    public string m_comment = "";
}

// 后台充值
public class DyOpRecharge : DyOpBase
{
    private Dictionary<string, object> m_data = new Dictionary<string, object>();

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamRecharge p = (ParamRecharge)param;
        bool res = false;
        int playerId = 0, rParam = 1;
        if (!int.TryParse(p.m_playerId, out playerId))
        {
            return OpRes.op_res_param_not_valid;
        }

        res = DBMgr.getInstance().keyExists(TableName.PLAYER_INFO, "player_id", playerId, user.getDbServerID(), DbName.DB_PLAYER);
        if (!res)
            return OpRes.op_res_not_found_data;

        if (p.m_param != "")
        {
            if (!int.TryParse(p.m_param, out rParam))
            {
                return OpRes.op_res_param_not_valid;
            }
        }
        if (rParam <= 0)
            return OpRes.op_res_param_not_valid;

        m_data.Clear();
        m_data.Add("playerId", playerId);
        m_data.Add("rtype", p.m_rtype);
        m_data.Add("param", rParam);
        res = DBMgr.getInstance().insertData(TableName.GM_RECHARGE, m_data, user.getDbServerID(), DbName.DB_PLAYER);

        if (res)
        {
//             OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_GM_RECHARGE,
//                 new LogGmRecharge(playerId, p.m_rtype, rParam), 
//                 user,
//                 p.m_comment);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamJPushAddApp
{
    public bool m_isAdd = true;
    public string m_platName = "";
    public string m_appName = "";
    public string m_appKey = "";
    public string m_apiSecret = "";

    public bool isValid()
    {
        return m_platName != "" && m_appName != "" && m_appKey != "" && m_apiSecret != "";
    }
}

// 增加一个极光应用
public class DyOpJPushAddApp: DyOpBase
{
    private Dictionary<string, object> m_data = new Dictionary<string, object>();

    public override OpRes doDyop(object param, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        ParamJPushAddApp p = (ParamJPushAddApp)param;
        if (p.m_isAdd)
        {
            if (!p.isValid())
                return OpRes.op_res_param_not_valid;

            m_data.Clear();
            m_data.Add("plat", p.m_platName);
            m_data.Add("appName", p.m_appName);
            m_data.Add("appKey", p.m_appKey);
            m_data.Add("apiSecret", p.m_apiSecret);
            bool res = DBMgr.getInstance().save(TableName.JPUSH_APP, m_data, "plat", p.m_platName, serverId, DbName.DB_ACCOUNT);
            return res ? OpRes.opres_success : OpRes.op_res_failed;
        }

        string[] str = Tool.split(p.m_platName, ',');
        for (int i = 0; i < str.Length; i++)
        {
            DBMgr.getInstance().remove(TableName.JPUSH_APP, "plat", str[i], serverId, DbName.DB_ACCOUNT);
        }

        return OpRes.opres_success;
    }

    public void getAppList(GMUser user, List<ParamJPushAddApp> result)
    {
        int accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (accServerId == -1)
            return;

        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.JPUSH_APP, accServerId, DbName.DB_ACCOUNT);
        for (int i = 0; i < data.Count; i++)
        {
            ParamJPushAddApp tmp = new ParamJPushAddApp();
            result.Add(tmp);
            tmp.m_platName = Convert.ToString(data[i]["plat"]);
            tmp.m_appName = Convert.ToString(data[i]["appName"]);
            tmp.m_appKey = Convert.ToString(data[i]["appKey"]);
            tmp.m_apiSecret = Convert.ToString(data[i]["apiSecret"]);
        }
    }
}

// 绑定手机
public class DyOpBindPhone : DyOpBase
{
    static string[] m_fields = { "phone", "acc" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamModifyPwd p = (ParamModifyPwd)param;

        if (string.IsNullOrEmpty(p.m_phone))
            return OpRes.op_res_param_not_valid;

        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.PLAYER_ACCOUNT, "acc", p.m_account, m_fields, serverId, DbName.DB_ACCOUNT);
        if (data == null)
            return OpRes.op_res_not_found_data;

        Dictionary<string, object> data1 = new Dictionary<string, object>();

        if (data.ContainsKey("phone")) // 已绑定
        {
            data1["phone"] = p.m_phone; // 更换手机号
        }
        else
        {
            DateTime now = DateTime.Now;
            data1["phone"] = p.m_phone;
            data1["searchTime"] = now;
            data1["searchCount"] = 0;
            data1["resetTime"] = calEndTime(now);
        }

        bool res = DBMgr.getInstance().update(TableName.PLAYER_ACCOUNT, data1, "acc", p.m_account, serverId, DbName.DB_ACCOUNT);
        if (!res)
            return OpRes.op_res_failed;

        return OpRes.opres_success;
    }

    // 取得截止时间
    public long calEndTime(DateTime now)
    {
        DateTime t = now.AddDays(1);
        DateTime e = new DateTime(t.Year, t.Month, t.Day, 0, 0, 0);
        return e.Ticks;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamGift
{
    // 添加还是修改
    public bool m_isAdd = true;
    // 礼包ID
    public string m_giftId;
    // 礼包道具列表
    public string m_itemList = "";
    // 截止日期
    public string m_deadTime = "";

    public string m_result = "";
}

public class GiftInfo
{
    public int m_giftId;
    public List<ParamItem> m_itemList = new List<ParamItem>();
    public DateTime m_deadTime;
}

// 礼包生成
public class DyOpGift : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;
        
        ParamGift p = (ParamGift)param;
        bool res = false;

        if (p.m_isAdd)
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;

            res = Tool.splitTimeStr(p.m_deadTime, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            if (mint < DateTime.Now)
                return OpRes.op_res_time_format_error;

            mint = mint.AddDays(1);
            mint = mint.AddSeconds(-1);
            List<ParamItem> itemList = new List<ParamItem>();
            res = Tool.parseItemList(p.m_itemList, itemList, false);
            if (!res)
            {
                return OpRes.op_res_param_not_valid;
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                var t = ItemCFG.getInstance().getValue(itemList[i].m_itemId);
                if (t == null)
                {
                    return OpRes.op_res_item_not_exist;
                }
            }

            long giftId = 0;

            giftId = 0;// CountMgr.getInstance().getCurId(CountMgr.GIFT_KEY);
            res = DBMgr.getInstance().keyExists(TableName.GIFT, "giftId", giftId, serverId, DbName.DB_ACCOUNT);
            if (res)
                return OpRes.op_res_data_duplicate;

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("giftId", giftId);
            data.Add("deadTime", mint);
            data.Add("item", ItemHelp.genItemBsonArray(itemList));
            res = DBMgr.getInstance().insertData(TableName.GIFT, data, serverId, DbName.DB_ACCOUNT);
            if(!res)
                return OpRes.op_res_failed;
        }
        else
        {
            bool isAdd = false;
            List<GiftInfo> giftList = new List<GiftInfo>();
            constructGiftList(giftList, p.m_itemList, p);
            
            for (int i = 0; i < giftList.Count; i++)
            {
                res = DBMgr.getInstance().keyExists(TableName.GIFT, "giftId", giftList[i].m_giftId, serverId, DbName.DB_ACCOUNT);
                if (!res)
                {
                    p.m_result += giftList[i].m_giftId + " ";
                    continue;
                }

                List<ParamItem> itemList = giftList[i].m_itemList;
                isAdd = true;

                for (int j = 0; j < itemList.Count; j++)
                {
                    var t = ItemCFG.getInstance().getValue(itemList[j].m_itemId);
                    if (t == null)
                    {
                        p.m_result += giftList[i].m_giftId + " ";
                        isAdd = false;
                        break;
                    }
                }

                if (isAdd)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data.Add("deadTime", giftList[i].m_deadTime);
                    data.Add("item", ItemHelp.genItemBsonArray(itemList));
                    res = DBMgr.getInstance().update(TableName.GIFT, data, "giftId", giftList[i].m_giftId, serverId, DbName.DB_ACCOUNT);
                    if (!res)
                    {
                        p.m_result += giftList[i].m_giftId + " ";
                    }
                }
            }
        }
        
        return OpRes.opres_success;
    }

    private OpRes constructGiftList(List<GiftInfo> giftList, string str, ParamGift pres)
    {
        int giftId = 0;
        bool res = false;
        string[] group = Tool.split(str, '#', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < group.Length; i++)
        {
            string[] et = Tool.split(group[i], '@', StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(et[0], out giftId))
            {
                GiftInfo info = new GiftInfo();
                info.m_giftId = giftId;

                res = Tool.parseItemList(et[1], info.m_itemList, false);
                if (!res)
                {
                    pres.m_result += giftId + " ";
                    continue;
                }

                DateTime mint = DateTime.Now, maxt = DateTime.Now;

                res = Tool.splitTimeStr(et[2], ref mint, ref maxt);
                if (!res)
                {
                    pres.m_result += giftId + " ";
                    continue;
                }

                if (mint < DateTime.Now)
                {
                    pres.m_result += giftId + " ";
                    continue;
                }

                mint = mint.AddDays(1);
                mint = mint.AddSeconds(-1);
                info.m_deadTime = mint;

                giftList.Add(info);
            }
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 礼包码生成
public class DyOpGiftCode : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        string p = (string)param;
        if (p == "")
            return OpRes.op_res_failed;

        string[] arr = Tool.split(p, ';', StringSplitOptions.RemoveEmptyEntries);
        ParamGenGiftCode pcode = new ParamGenGiftCode();
        for (int i = 0; i < arr.Length; i++)
        {
            string[] tmp = Tool.split(arr[i], ',', StringSplitOptions.RemoveEmptyEntries);
            GiftCodeInfo info = new GiftCodeInfo();
            info.m_count = Convert.ToInt32(tmp[2]);
            if (info.m_count > 0)
            {
                pcode.m_codeList.Add(info);
                info.m_giftId = Convert.ToInt64(tmp[0]);

                int platId = Convert.ToInt32(tmp[1]);
                PlatformInfo pinfo = ResMgr.getInstance().getPlatformInfo(platId);
                if (pinfo != null)
                {
                    info.m_plat = pinfo.m_engName;
                }
                else
                {
                    info.m_plat = "default";
                }
            }
        }
        pcode.m_dbServerIP = WebConfigurationManager.AppSettings["account"];

        return RemoteMgr.getInstance().reqGenGiftCode(pcode);
    }
}

//////////////////////////////////////////////////////////////////////////

// 兑换写入
public class DyOpExchange : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        string p = (string)param;
        if (p == "")
            return OpRes.op_res_failed;

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["isReceive"] = true;
        data["giveOutTime"] = DateTime.Now;

        string[] arr = Tool.split(p, ',', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < arr.Length; i++)
        {
            DBMgr.getInstance().update(TableName.EXCHANGE, data, "exchangeId", arr[i], user.getDbServerID(), DbName.DB_PLAYER);
        }
        
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public enum NoticeOpType
{
    add,
    del,
    modify,
}

public class ParamNotify
{
    public string m_title = "";

    public string m_content = "";

    public string m_day = "";

    public NoticeOpType m_opType;

    public string m_id = "";

    public string m_comment = "";

    public string m_order = "";
}

// 通告
public class DyOpNotify : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamNotify p = (ParamNotify)param;

        if (p.m_opType == NoticeOpType.del)
        {
            string[] str = Tool.split(p.m_id, ',');
            for (int i = 0; i < str.Length; i++)
            {
                p.m_id = str[i];
                delNotice(p, user);
            }
            return OpRes.opres_success;
        }

        if (p.m_id != "")
        {
            return updateNotice(p, user);
        }
        return addNotice(p, user);
    }

    private OpRes addNotice(ParamNotify p, GMUser user)
    {
        if (string.IsNullOrEmpty(p.m_title) || string.IsNullOrEmpty(p.m_content))
            return OpRes.op_res_param_not_valid;

        int day = 0;
        if (!int.TryParse(p.m_day, out day))
        {
            return OpRes.op_res_param_not_valid;
        }
        if (day <= 0)
            return OpRes.op_res_param_not_valid;

        int order = 0;
        if(!int.TryParse(p.m_order, out order))
            return OpRes.op_res_param_not_valid;

        DateTime now = DateTime.Now;
        DateTime nt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("genTime", nt);
        data.Add("title", p.m_title);
        data.Add("content", p.m_content);
        data.Add("deadTime", nt.AddDays(day));
        data.Add("noticeId", Guid.NewGuid().ToString());
        data.Add("comment", p.m_comment);
        data.Add("order", order);

        bool res = DBMgr.getInstance().insertData(TableName.OPERATION_NOTIFY, data, user.getDbServerID(), DbName.DB_PLAYER);
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    private OpRes updateNotice(ParamNotify p, GMUser user)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, object> retData = DBMgr.getInstance().getTableData(TableName.OPERATION_NOTIFY,
            "noticeId", p.m_id, new string[] { "genTime" }, user.getDbServerID(), DbName.DB_PLAYER);

        if (retData == null)
            return OpRes.op_res_not_found_data;

        if (!string.IsNullOrEmpty(p.m_day))
        {
            int day = 0;
            if (!int.TryParse(p.m_day, out day))
            {
                return OpRes.op_res_param_not_valid;
            }
            if (day <= 0)
                return OpRes.op_res_param_not_valid;

            DateTime nt = Convert.ToDateTime(retData["genTime"]).ToLocalTime();
            data.Add("deadTime", nt.AddDays(day));
        }

        if (!string.IsNullOrEmpty(p.m_title))
        {
            data.Add("title", p.m_title);
        }
        if (!string.IsNullOrEmpty(p.m_content))
        {
            data.Add("content", p.m_content);
        }
        if (!string.IsNullOrEmpty(p.m_comment))
        {
            data.Add("comment", p.m_comment);
        }
        if (!string.IsNullOrEmpty(p.m_order))
        {
            int order = 0;
            if (!int.TryParse(p.m_order, out order))
                return OpRes.op_res_param_not_valid;

            data.Add("order", order);
        }
        bool res = DBMgr.getInstance().update(TableName.OPERATION_NOTIFY, 
            data, "noticeId", p.m_id, user.getDbServerID(), DbName.DB_PLAYER);

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    private OpRes delNotice(ParamNotify p, GMUser user)
    {
        bool res = DBMgr.getInstance().remove(TableName.OPERATION_NOTIFY, "noticeId", p.m_id, 
            user.getDbServerID(), DbName.DB_PLAYER);

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamSpeaker
{
    // 要显示的消息
    public string m_content = "";

    // 发出去的时间
    public string m_sendTime = "";

    // 重复时间
    public string m_repCount = "";

    // 发送间隔
    public string m_interval = "";
}

// 运营发布的通告消息，会显示到通告栏
public class DyOpSpeaker : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamSpeaker p = (ParamSpeaker)param;
        if (string.IsNullOrEmpty(p.m_content))
            return OpRes.op_res_param_not_valid;

        bool res = false;
        DateTime sendTime = DateTime.MinValue;
        if (!string.IsNullOrEmpty(p.m_sendTime))
        {
            res = Tool.splitTimeStr(p.m_sendTime, ref sendTime, 3);
            if (!res)
                return OpRes.op_res_time_format_error;
        }

        int repCount = 1;
        if (!string.IsNullOrEmpty(p.m_repCount))
        {
            res = int.TryParse(p.m_repCount, out repCount);
            if(!res)
                return OpRes.op_res_param_not_valid;
        }

        int interval = 1;
        if (!string.IsNullOrEmpty(p.m_interval))
        {
            res = int.TryParse(p.m_interval, out interval);
            if (!res)
                return OpRes.op_res_param_not_valid;
        }

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("genTime", DateTime.Now);
        data.Add("content", p.m_content);
        data.Add("sendTime", sendTime);
        data.Add("repCount", repCount);
        data.Add("interval", interval);
        res = DBMgr.getInstance().insertData(TableName.OPERATION_SPEAKER, data, user.getDbServerID(), DbName.DB_PLAYER);
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamMaintenance
{
    // 0取得当前信息, 1确定维护, 2撤消维护
    public int m_opType;

    // 内容
    public string m_content = "";
}

// 当前信息
public class ResultMaintenance
{
    // 0运行中，1维护中, 2未知
    public int m_curState;
    // 当前的维护信息
    public string m_info = "";
}

// 运营维护
public class DyOpMaintenance : DyOpBase
{
    private ResultMaintenance m_result = new ResultMaintenance();

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamMaintenance p = (ParamMaintenance)param;
        if (p.m_opType == 0) // 取得当前信息
        {
            return fetchCurState();
        }
        if (p.m_opType == 1) // 确定维护
        {
            return doMaintenance("false", p.m_content);
        }
        if (p.m_opType == 2) // 撤消维护
        {
            return doMaintenance("true", p.m_content);
        }
        return OpRes.op_res_failed;
    }

    public ResultMaintenance getResult()
    {
        return m_result;
    }

    private OpRes fetchCurState()
    {
        string fmt = WebConfigurationManager.AppSettings["maintenaceWeb"];
        string aspx = string.Format(fmt, "", "");
        var ret = HttpPost.Get(new Uri(aspx));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            Dictionary<string, object> data = parseString(retStr);
            if (data != null)
            {
                m_result.m_info = Convert.ToString(data["info"]);
                string state = Convert.ToString(data["state"]);
                if (state == "true")
                {
                    m_result.m_curState = 0;
                }
                else
                {
                    m_result.m_curState = 1;
                }
                return OpRes.opres_success;
            }
        }
        else
        {
            m_result.m_curState = 2;
        }
        return OpRes.op_res_failed;
    }

    private Dictionary<string, object> parseString(string str)
    {
        byte[] arr = Convert.FromBase64String(str);
        string dst = Encoding.Default.GetString(arr);
        Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(dst);
        return data;
    }

    // 开始维护
    private OpRes doMaintenance(string state, string info)
    {
        string fmt = WebConfigurationManager.AppSettings["maintenaceWeb"];
        string aspx = string.Format(fmt, state, info);
        try
        {
            var ret = HttpPost.Post(new Uri(aspx), null);
            if (ret != null)
            {
                string retStr = Encoding.UTF8.GetString(ret);
                if (retStr == "0")
                    return OpRes.opres_success;
            }
        }
        catch (System.Exception ex)
        {
        }
       
        return OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamFishlordParamAdjust
{
    // 是否重置
    public bool m_isReset;

    // 期望盈利率
    public string m_expRate = "";

    // 房间列表
    public string m_roomList = "";

    public GameId m_gameId;
}

public class DyOpParamAdjust : DyOpBase
{
    protected static string[] s_fields = new string[] { "room_income", "room_outcome", "ExpectEarnRate" };

    // 继承类需要赋值
    protected int m_gameId;

    protected string m_roomTableName = "";

    public DyOpParamAdjust(int gameId, string roomTableName)
    {
        m_gameId = gameId;
        m_roomTableName = roomTableName;
    }

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFishlordParamAdjust p = (ParamFishlordParamAdjust)param;

        if (string.IsNullOrEmpty(p.m_roomList))
            return OpRes.op_res_param_not_valid;

        if (p.m_isReset)
        {
            return resetExp(user, p);
        }
        return modifyExp(user, p);
    }

    protected virtual OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
    {
        return OpRes.op_res_failed;
    }

    protected virtual OpRes resetExp(GMUser user, ParamFishlordParamAdjust p)
    {
        bool res = false;
        DateTime now = DateTime.Now;
        Dictionary<string, object> data = new Dictionary<string, object>();
        string[] rooms = Tool.split(p.m_roomList, ',', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rooms.Length; i++)
        {
            data.Clear();
            data.Add("room_income", -1L);
            data.Add("room_outcome", -1L);
            int roomId = Convert.ToInt32(rooms[i]);
            addOldEarningsRate(user, roomId, now);
            res = DBMgr.getInstance().update(m_roomTableName, data, "room_id", roomId,
                 user.getDbServerID(), DbName.DB_GAME);
            if (!res)
            {
                break;
            }
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_RESET_FISHLORD_GAIN_RATE,
            new LogResetFishlordRoomExpRate(p.m_roomList, m_gameId), user);

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    // 增加旧的盈利率
    protected OpRes addOldEarningsRate(GMUser user, int roomId, DateTime now)
    {
        ResultExpRateParam oldParam = getOldParam(roomId, user);
        if (oldParam == null)
            return OpRes.op_res_failed;

        Dictionary<string, object> old = new Dictionary<string, object>();
        old.Add("gameId", m_gameId);
        old.Add("roomId", roomId);
        old.Add("time", now);
        old.Add("income", oldParam.m_totalIncome);
        old.Add("outlay", oldParam.m_totalOutlay);
        old.Add("expRate", oldParam.m_expRate);
        DBMgr.getInstance().insertData(TableName.PUMP_OLD_EARNINGS_RATE, old, user.getDbServerID(), DbName.DB_PUMP);
        return OpRes.opres_success;
    }

    // 返回旧的参数
    protected virtual ResultExpRateParam getOldParam(int roomId, GMUser user)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(m_roomTableName, "room_id", roomId,
             s_fields, user.getDbServerID(), DbName.DB_GAME);

        if (data == null)
            return null;

        ResultExpRateParam param = new ResultExpRateParam();

        if (data.ContainsKey("room_income"))
        {
            param.m_totalIncome = Convert.ToInt64(data["room_income"]);
        }
        if (data.ContainsKey("room_outcome"))
        {
            param.m_totalOutlay = Convert.ToInt64(data["room_outcome"]);
        }
        if (data.ContainsKey("ExpectEarnRate"))
        {
            param.m_expRate = Convert.ToDouble(data["ExpectEarnRate"]);
        }
        return param;
    }

    // 修改盈利率
    // expRateFieldName 期望盈利率字段名  roomIdFieldName 房间ID字段名称
    protected OpRes modifyExpImp(GMUser user,
                                ParamFishlordParamAdjust p,
                                string expRateFieldName,
                                string roomIdFieldName = "room_id")
    {
        double expRate = 0.0;
        if (!double.TryParse(p.m_expRate, out expRate))
            return OpRes.op_res_param_not_valid;
        if (expRate <= 0.0)
            return OpRes.op_res_param_not_valid;

        Dictionary<string, object> data = new Dictionary<string, object>();
        string[] rooms = Tool.split(p.m_roomList, ',', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rooms.Length; i++)
        {
            data.Clear();
            data.Add(expRateFieldName, expRate);
            int roomId = Convert.ToInt32(rooms[i]);
            bool res = DBMgr.getInstance().update(m_roomTableName, data, roomIdFieldName, roomId,
                 user.getDbServerID(), DbName.DB_GAME);
            if (!res)
                return OpRes.op_res_failed;
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_MODIFY_FISHLORD_GAIN_RATE,
            new LogModifyFishlordRoomExpRate(p.m_roomList, expRate, m_gameId), user);

        return OpRes.opres_success;
    }
}

// 捕鱼参数调整
public class DyOpFishlordParamAdjust : DyOpParamAdjust
{
    private static string[] s_fieldsFish = new string[] { "TotalIncome", "TotalOutlay", "EarningsRate" };

    public DyOpFishlordParamAdjust()
        : base((int)GameId.fishlord, TableName.FISHLORD_ROOM)
    {

    }

    public DyOpFishlordParamAdjust(int gameId, string roomTableName)
        : base(gameId, roomTableName)
    {
    }

    protected override OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
    {
        return modifyExpImp(user, p, "EarningsRate");
    }

    protected override OpRes resetExp(GMUser user, ParamFishlordParamAdjust p)
    {
        DateTime now = DateTime.Now;
        Dictionary<string, object> data = new Dictionary<string, object>();
        string[] rooms = Tool.split(p.m_roomList, ',', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rooms.Length; i++)
        {
            data.Clear();
            data.Add("TotalIncome", -1L);
            data.Add("TotalOutlay", -1L);
            int roomId = Convert.ToInt32(rooms[i]);
            addOldEarningsRate(user, roomId, now);
            DBMgr.getInstance().update(m_roomTableName, data, "room_id", roomId,
                user.getDbServerID(), DbName.DB_GAME);
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_RESET_FISHLORD_GAIN_RATE,
            new LogResetFishlordRoomExpRate(p.m_roomList, m_gameId), user);

        return OpRes.opres_success;
    }

    // 返回旧的参数
    protected override ResultExpRateParam getOldParam(int roomId, GMUser user)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(m_roomTableName, "room_id", roomId,
             s_fieldsFish, user.getDbServerID(), DbName.DB_GAME);

        if (data == null)
            return null;

        ResultExpRateParam param = new ResultExpRateParam();

        if (data.ContainsKey("TotalIncome"))
        {
            param.m_totalIncome = Convert.ToInt64(data["TotalIncome"]);
        }
        if (data.ContainsKey("TotalOutlay"))
        {
            param.m_totalOutlay = Convert.ToInt64(data["TotalOutlay"]);
        }
        if (data.ContainsKey("EarningsRate"))
        {
            param.m_expRate = Convert.ToDouble(data["EarningsRate"]);
        }
        return param;
    }
}

// 鳄鱼公园参数调整
public class DyOpFishParkParamAdjust : DyOpFishlordParamAdjust
{
    public DyOpFishParkParamAdjust()
        : base((int)GameId.fishpark, TableName.FISHPARK_ROOM)
    {

    }
}

//////////////////////////////////////////////////////////////////////////

// 鳄鱼大亨参数调整
public class DyOpCrocodileParamAdjust : DyOpParamAdjust
{
    public DyOpCrocodileParamAdjust()
        : base((int)GameId.crocodile, TableName.CROCODILE_ROOM)
    {
        //m_gameId = (int)GameId.crocodile;
        //m_roomTableName = TableName.CROCODILE_ROOM;
    }

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFishlordParamAdjust p = (ParamFishlordParamAdjust)param;

        if (string.IsNullOrEmpty(p.m_roomList))
            return OpRes.op_res_param_not_valid;

        if (p.m_isReset)
        {
            return resetExp(user, p);
        }
        return modifyExp(user, p);
    }

    protected override OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
    {
        double expRate = 0.0;
        if (!double.TryParse(p.m_expRate, out expRate))
            return OpRes.op_res_param_not_valid;
        if (expRate <= 0.0)
            return OpRes.op_res_param_not_valid;

        Dictionary<string, object> data = new Dictionary<string, object>();
        string[] rooms = Tool.split(p.m_roomList, ',', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rooms.Length; i++)
        {
            data.Clear();
            data.Add("ExpectEarnRate", expRate);
            int roomId = Convert.ToInt32(rooms[i]);
            DBMgr.getInstance().update(TableName.CROCODILE_ROOM, data, "room_id", roomId,
                user.getDbServerID(), DbName.DB_GAME);
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_MODIFY_FISHLORD_GAIN_RATE,
            new LogModifyFishlordRoomExpRate(p.m_roomList, expRate, (int)GameId.crocodile), user);

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 骰宝参数调整
public class DyOpDiceParamAdjust : DyOpParamAdjust
{
    public DyOpDiceParamAdjust()
        : base((int)GameId.dice, TableName.DICE_ROOM)
    {
        // m_gameId = (int)GameId.dice;
        // m_roomTableName = TableName.DICE_ROOM;
    }

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFishlordParamAdjust p = (ParamFishlordParamAdjust)param;

        if (string.IsNullOrEmpty(p.m_roomList))
            return OpRes.op_res_param_not_valid;

        return resetExp(user, p);
    }
}

//////////////////////////////////////////////////////////////////////////

// 百家乐参数调整
public class DyOpBaccaratParamAdjust : DyOpParamAdjust
{
    public DyOpBaccaratParamAdjust()
        : base((int)GameId.baccarat, TableName.BACCARAT_ROOM)
    {
        // m_gameId = (int)GameId.baccarat;
        // m_roomTableName = TableName.BACCARAT_ROOM;
    }

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFishlordParamAdjust p = (ParamFishlordParamAdjust)param;

        if (string.IsNullOrEmpty(p.m_roomList))
            return OpRes.op_res_param_not_valid;

        return resetExp(user, p);
    }
}

//////////////////////////////////////////////////////////////////////////

// 牛牛参数调整
public class DyOpCowsParamAdjust : DyOpParamAdjust
{
    public DyOpCowsParamAdjust()
        : base((int)GameId.cows, TableName.COWS_ROOM)
    {
        //m_gameId = (int)GameId.cows;
        //m_roomTableName = TableName.COWS_ROOM;
    }

    protected override OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
    {
        return modifyExpImp(user, p, "ExpectEarnRate");
    }
}

//////////////////////////////////////////////////////////////////////////

// 五龙参数调整
public class DyOpDragonParamAdjust : DyOpParamAdjust
{
    public DyOpDragonParamAdjust()
        : base((int)GameId.dragon, TableName.DRAGON_ROOM)
    {
    }

    protected override OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
    {
        return modifyExpImp(user, p, "expect_earn_rate");
    }

    // 返回旧的参数
    protected override ResultExpRateParam getOldParam(int roomId, GMUser user)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(m_roomTableName, "room_id", roomId,
             null, user.getDbServerID(), DbName.DB_GAME);

        if (data == null)
            return null;

        ResultExpRateParam param = new ResultExpRateParam();

        if (data.ContainsKey("room_income"))
        {
            param.m_totalIncome = Convert.ToInt64(data["room_income"]);
        }
        if (data.ContainsKey("room_outcome"))
        {
            param.m_totalOutlay = Convert.ToInt64(data["room_outcome"]);
        }
        if (data.ContainsKey("expect_earn_rate"))
        {
            param.m_expRate = Convert.ToDouble(data["expect_earn_rate"]);
        }
        return param;
    }
}

//////////////////////////////////////////////////////////////////////////
// 游戏参数调整，入口
public class DyOpGameParamAdjust : DyOpBase
{
    private Dictionary<GameId, DyOpParamAdjust> m_game = new Dictionary<GameId, DyOpParamAdjust>();

    public DyOpGameParamAdjust()
    {
        m_game.Add(GameId.shcd, new DyOpShcdParamAdjust());
       // m_game.Add(GameId.calf_roping, new DyOpCalfRopingParamAdjust());
    }

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFishlordParamAdjust p = (ParamFishlordParamAdjust)param;
        if (m_game.ContainsKey(p.m_gameId))
        {
            return m_game[p.m_gameId].doDyop(p, user);
        }
        return OpRes.op_res_failed;
    }
}

// 黑红梅方参数调整
public class DyOpShcdParamAdjust : DyOpParamAdjust
{
    public DyOpShcdParamAdjust()
        : base((int)GameId.shcd, TableName.SHCDCARDS_ROOM)
    {
    }

    protected override OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
    {
        return modifyExpImp(user, p, "ExpectEarnRate");
    }
}

//////////////////////////////////////////////////////////////////////////

// 清空鱼统计表
public class DyOpClearFishTable : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        string tableName = (string)param;
        DBMgr.getInstance().clearTable(tableName, user.getDbServerID(), DbName.DB_PUMP);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

enum ReloadTable
{
    // 经典捕鱼鱼表
  //  fish,
    // 鳄鱼公园鱼表
    fishpark_fish,
}

// 重新加载表格
public class DyOpReLoadTable : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        bool res = false;
        int tableIndex = (int)param;
        switch (tableIndex)
        {
           /* case (int)ReloadTable.fish: // 加载鱼表
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data.Add("key", "reload_cfg");
                    res = DBMgr.getInstance().insertData(TableName.RELOAD_FISHCFG, data, user.getDbServerID(), DbName.DB_GAME);
                }
                break;*/
            case (int)ReloadTable.fishpark_fish: // 加载鱼表
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data.Add("key", "reload_cfg");
                    res = DBMgr.getInstance().insertData(TableName.RELOAD_FISHPARK_CFG, data, user.getDbServerID(), DbName.DB_GAME);
                }
                break;
        }

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamServiceInfo
{
    // 平台
    public string m_key = "";
    // 描述
    public string m_desc = "";

    public bool m_isAdd = true;
}

// 增加或修改客服信息
public class DyOpServiceInfo : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        int accServerId = user.getDbServerID();
        if (accServerId == -1)
            return OpRes.op_res_failed;
        
        ParamServiceInfo p = (ParamServiceInfo)param;
        if (p.m_isAdd)
        {
            return addServiceInfo(accServerId, p);
        }
        return delServiceInfo(accServerId, p);
    }

    private OpRes addServiceInfo(int accServerId, ParamServiceInfo p)
    {
        if (p.m_key == "" || p.m_desc == "")
        {
            return OpRes.op_res_param_not_valid;
        }
        Match m = Regex.Match(p.m_desc, Exp.SERVICE_HELP_M);
        if (!m.Success)
        {
            m = Regex.Match(p.m_desc, Exp.SERVICE_HELP1);
            if (!m.Success)
            {
                return OpRes.op_res_param_not_valid;
            }
        }
        Dictionary<string, object> data = new Dictionary<string, object>();
        bool res = false;
        if (data != null)
        {
            data["plat"] = p.m_key;
            data["info"] = p.m_desc;
                
            res = DBMgr.getInstance().save(TableName.SERVICE_INFO, data, "plat", p.m_key, accServerId, DbName.DB_PLAYER);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    private OpRes delServiceInfo(int accServerId, ParamServiceInfo p)
    {
         string[] strs = Tool.split(p.m_key, ',', StringSplitOptions.RemoveEmptyEntries);
         for (int i = 0; i < strs.Length; i++)
         {
             DBMgr.getInstance().remove(TableName.SERVICE_INFO, "plat", strs[i], accServerId, DbName.DB_PLAYER);
         }
         return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamFreezeHeadInfo
{
    // 玩家ID
    public string m_playerId = "";
    // 冻结天数
    public string m_freezeDays = "";
}

// 冻结头像
public class DyOpFreezeHead : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFreezeHeadInfo p = (ParamFreezeHeadInfo)param;
        int playerId = 0, days = 7;
        if (!int.TryParse(p.m_playerId, out playerId))
            return OpRes.op_res_param_not_valid;

        if (!string.IsNullOrEmpty(p.m_freezeDays))
        {
            if (!int.TryParse(p.m_freezeDays, out days))
            {
                return OpRes.op_res_param_not_valid;
            }
            if (days <= 0)
                return OpRes.op_res_param_not_valid; 
        }

        bool res = DBMgr.getInstance().keyExists(TableName.PLAYER_INFO, "player_id", playerId, user.getDbServerID(), DbName.DB_PLAYER);
        if (!res)
        {
            return OpRes.op_res_player_not_exist;
        }

        DateTime deadTime = DateTime.Now.AddDays(days);
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("playerId", playerId);
        data.Add("rtype", (int)RechargeType.delIconCustom);
        data.Add("paramTime", deadTime);
        res = DBMgr.getInstance().insertData(TableName.GM_RECHARGE, data, user.getDbServerID(), DbName.DB_PLAYER);

        if (res)
        {
//             OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_DEL_CUSTOM_HEAD,
//                 new LogFreezeHead(playerId, deadTime), 
//                 user);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamAddChannel
{
    public bool m_isAdd;
    public List<string> m_channels = new List<string>();
}

// 渠道编辑
public class DyOpAddChannel : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        ParamAddChannel p = (ParamAddChannel)param;
        if (p.m_isAdd)
            return addChannel(serverId, p);

        return delChannel(serverId, p);
    }

    public string getTestChannel(GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return "";

        List<Dictionary<string, object>> data =
            DBMgr.getInstance().executeQuery(TableName.TEST_SERVER, serverId, DbName.DB_CONFIG);

        string str = "";
        foreach (var d in data)
        {
           // str += "'" + d["channel"].ToString() + "'" + ",";
            str += d["channel"].ToString() + ",";
        }
        return str;
    }

    private OpRes addChannel(int serverId, ParamAddChannel p)
    {
        List<string> channels = (List<string>)p.m_channels;
        if (channels.Count == 0)
            return OpRes.opres_success;

        string str = "";
        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
        foreach (string c in channels)
        {
            bool res = DBMgr.getInstance().keyExists(TableName.TEST_SERVER, "channel", c, serverId, DbName.DB_CONFIG);
            if (!res)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("channel", c);
                dataList.Add(data);

               // str += "'" + c + "'" + ",";
            }
        }

        if (dataList.Count > 0)
        {
            bool res = DBMgr.getInstance().insertData(TableName.TEST_SERVER, dataList, serverId, DbName.DB_CONFIG);
            /*if (res)
            {
                AccessDb.getAccDb().setConnDb("channel.mdb");

                string sql = string.Format("update  channel set enable=false where channelNo in({0})", str);
                int n = AccessDb.getAccDb().executeOp(sql);
                AccessDb.getAccDb().end();
            }*/
        }

        return OpRes.opres_success;
    }

    private OpRes delChannel(int serverId, ParamAddChannel p)
    {
        List<string> channels = (List<string>)p.m_channels;
        if (channels.Count == 0)
            return OpRes.opres_success;

       // string str = "";
        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
        foreach (string c in channels)
        {
            bool res = DBMgr.getInstance().remove(TableName.TEST_SERVER, "channel", c, serverId, DbName.DB_CONFIG);
            if (res)
            {
            //    str += "'" + c + "'" + ",";
            }
        }

       /* if (str != "")
        {
            AccessDb.getAccDb().setConnDb("channel.mdb");
            string sql = string.Format("update  channel set enable=true where channelNo in({0})", str);
            int n = AccessDb.getAccDb().executeOp(sql);
            AccessDb.getAccDb().end();
        }*/

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamCowsCard
{
    public int m_op = 0;
    public object m_data;
}

public class ParamAddCowsCard
{
    public int m_bankerType;
    public int m_other1Type;
    public int m_other2Type;
    public int m_other3Type;
    public int m_other4Type;
}

// 牛牛牌型设置
public class DyOpAddCowsCardType : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamCowsCard p = (ParamCowsCard)param;
        if (p.m_op == 0)
        {
            ParamAddCowsCard add = (ParamAddCowsCard)p.m_data;
            return addCardType(add, user);
        }

        string key = (string)p.m_data;
        return delCardType(key, user);
    }

    private OpRes addCardType(ParamAddCowsCard param, GMUser user)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("insert_time", DateTime.Now);
        data.Add("banker_cards", param.m_bankerType);
        data.Add("other_cards1", param.m_other1Type);
        data.Add("other_cards2", param.m_other2Type);
        data.Add("other_cards3", param.m_other3Type);
        data.Add("other_cards4", param.m_other4Type);
        data.Add("key", Guid.NewGuid().ToString());

        bool res = DBMgr.getInstance().insertData(TableName.COWS_CARDS,
            data, user.getDbServerID(), DbName.DB_GAME);
        if (res)
        {
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_COWS_ADD_CARDS_TYPE,
                new LogCowsAddCardType(param.m_bankerType, param.m_other1Type, 
                    param.m_other2Type, param.m_other3Type, param.m_other4Type),
                user);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    private OpRes delCardType(string key, GMUser user)
    {
        bool res =
            DBMgr.getInstance().remove(TableName.COWS_CARDS, "key", key, user.getDbServerID(), DbName.DB_GAME);
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamAddWishCurse
{
    public int m_opType; // 0添加, 1去除
    public int m_gameId; // 经典捕鱼or鳄鱼公园
    public int m_wishType;
    public string m_playerId;
    public string m_rate;

    public bool isAdd()
    {
        return m_opType == 0;
    }
}

// 祝福与诅咒
public class DyOpAddWishCurse : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamAddWishCurse p = (ParamAddWishCurse)param;
        int playerId = 0;
        double rate = 0.0;

        if (p.isAdd())
        {
            if (!double.TryParse(p.m_rate, out rate))
                return OpRes.op_res_param_not_valid;

            if (rate <= 0.0)
                return OpRes.op_res_param_not_valid;
        }

        if (!int.TryParse(p.m_playerId, out playerId))
            return OpRes.op_res_param_not_valid;

        if (p.m_wishType == 1) // 诅咒
        {
            rate = -rate;
        }
        return addWishCurse(p, playerId, rate, user);
    }

    private OpRes addWishCurse(ParamAddWishCurse param,
                               int playerId,
                               double rate,
                               GMUser user)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        if (param.isAdd())
        {
            data.Add("FixRate", rate);
        }
        else
        {
            data.Add("FixRate", 0.0);
        }
        bool res = false;
        string tableName = "";
        if (param.m_gameId == (int)GameId.fishlord)
        {
            tableName = TableName.FISHLORD_PLAYER;
        }
        else
        {
            tableName = TableName.FISHPARK_PLAYER;
        }

        res = DBMgr.getInstance().keyExists(tableName, "player_id", playerId, user.getDbServerID(), DbName.DB_GAME);
        if (!res)
            return OpRes.op_res_player_not_exist;

        res = DBMgr.getInstance().update(tableName, data,
             "player_id",
             playerId, user.getDbServerID(), DbName.DB_GAME);

        if (res)
        {
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_WISH_CURSE,
                new LogWishCurse(param.m_gameId, playerId, param.m_wishType, param.m_opType),
                user);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamCreateGmAccount
{
    public int m_accType;
    public string m_accName = "";
    public string m_pwd1;
    public string m_pwd2;
    public int m_moneyType;
    public string m_right = "";
    public string m_aliasName = "";  // 别名
    public string m_agentRatio;     // 代理占成
    public string m_washRatio;      // 洗码比
    public string m_apiPrefix;      // api前缀

    // 结果账号
    public string m_resultAcc = "";
    // 4位固定数字码
    public string m_validatedCode = "";
}

class GmAccountInfo
{
    public string m_accName = "";
    public string m_pwd = "";
    public int m_accType;
    public string m_owner = "";
    public int m_moneyType;
    public string m_devSecretKey = "";

    public string m_generalAgency = "";
    public string m_postfix = "";
    public string m_right = "";
    public string m_aliasName = "";

    // 这个由外部填充
    public string m_validatedCode = "";

    public void fillInfo(GMUser user, string accName, int accType,
        int moneyType, ParamCreateGmAccount p, string postfix = "", string generalAgency = "", string right = "", string devSecretKey = "")
    {
        m_accName = accName;
        m_pwd = p.m_pwd1;
        m_accType = accType;
        m_owner = user.m_user;
        m_moneyType = moneyType;
        m_generalAgency = generalAgency;
        m_postfix = postfix;
        m_right = right;
        m_devSecretKey = devSecretKey;
        m_aliasName = p.m_aliasName;
    }
}

// 创建管理员
public class DyOpCreateGmAccount : DyOpBase
{
    GmAccountInfo m_accInfo = new GmAccountInfo();

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamCreateGmAccount p = (ParamCreateGmAccount)param;
        OpRes res = OpRes.op_res_failed;
        try
        {
            HttpContext.Current.Application.Lock();

            switch (p.m_accType)
            {
                case AccType.ACC_GENERAL_AGENCY: // 创建总代理
                    {
                        res = createGeneralAgency(p, user);
                    }
                    break;
                case AccType.ACC_AGENCY:  // 创建下级代理
                    {
                        res = createAgency(p, user);
                    }
                    break;
                case AccType.ACC_API:  // 创建API账号
                    {
                        res = createAPI(p, user);
                    }
                    break;
                case AccType.ACC_AGENCY_SUB:  // 创建代理子账号
                    {
                        res = createAgencySub(p, user);
                    }
                    break;
                case AccType.ACC_API_ADMIN:
                    {
                        res = createAPIAdmin(p, user);
                    }
                    break;
                case AccType.ACC_SUPER_ADMIN_SUB:
                    {
                        res = createAdminSub(p, user);
                    }
                    break;
            }
        }
        catch (System.Exception ex)
        {
        }
        finally
        {
            HttpContext.Current.Application.UnLock();
        }

        return res;
    }

    // 创建总代理
    private OpRes createGeneralAgency(ParamCreateGmAccount param, GMUser user)
    {
        // 只有超级管理员才可以创建
        if (user.m_accType != AccType.ACC_SUPER_ADMIN) 
            return OpRes.op_res_no_right;

        OpRes res = gmPwdValid(param);
        if (res != OpRes.opres_success)
            return res;

        if (!Regex.IsMatch(param.m_accName, Exp.ACCOUNT_DEALER))
        {
            return OpRes.op_res_param_not_valid;
        }

        // 创建总代理，去除后面的CNY串
        string accName = param.m_accName; // +StrName.getMoneyCode(param.m_moneyType);//StrName.s_moneyType[param.m_moneyType];

        m_accInfo.fillInfo(user, accName, AccType.ACC_GENERAL_AGENCY, param.m_moneyType, param,
            StrName.getMoneyCode(param.m_moneyType), accName);
        //m_accInfo.m_aliasName = param.m_aliasName;

        res = createAcc(user, param);
        if (res == OpRes.opres_success)
        {
            param.m_resultAcc = accName;
            param.m_validatedCode = m_accInfo.m_validatedCode;
        }
        return res;
    }

    // 创建下级代理
    private OpRes createAgency(ParamCreateGmAccount param, GMUser user)
    {
        OpRes res = canCreateAgency(user);
        if (res != OpRes.opres_success)
            return res;

        res = gmPwdValid(param);
        if (res != OpRes.opres_success)
            return res;

        if (!Regex.IsMatch(param.m_accName, Exp.ACCOUNT_AGENCY))
        {
            return OpRes.op_res_param_not_valid;
        }

        /*bool exists = ItemHelp.existPostfix(param.m_accName, user);
        if (exists)
            return OpRes.op_res_account_has_exists;*/

        string accName = param.m_accName; //user.m_generalAgency + param.m_accName;

        m_accInfo.fillInfo(user, accName, param.m_accType, user.m_moneyType, param,
            "", user.m_generalAgency, param.m_right);
        //m_accInfo.m_aliasName = param.m_aliasName;
        res = createAcc(user, param);
        if (res == OpRes.opres_success)
        {
            param.m_resultAcc = accName;
            param.m_validatedCode = m_accInfo.m_validatedCode;
           // savePostfix(user, param.m_accName);
        }
        return res;
    }

    // 创建代理子账号
    private OpRes createAgencySub(ParamCreateGmAccount param, GMUser user)
    {
        OpRes res = OpRes.opres_success;
        if (!(user.isAgency() || user.isGeneralAgency()))
            return OpRes.op_res_no_right;

        res = gmPwdValid(param);
        if (res != OpRes.opres_success)
            return res;

        if (!Regex.IsMatch(param.m_accName, Exp.ACCOUNT_AGENCY))
        {
            return OpRes.op_res_param_not_valid;
        }

        string postfix = "#" + param.m_accName;
        /*bool exists = ItemHelp.existPostfix(postfix, user);
        if (exists)
            return OpRes.op_res_account_has_exists;*/

        string accName = postfix;//user.m_generalAgency + postfix;

        m_accInfo.fillInfo(user, accName, param.m_accType, user.m_moneyType, param,
            "", user.m_generalAgency, param.m_right);
       // m_accInfo.m_aliasName = param.m_aliasName;
        res = createAcc(user, param);
        if (res == OpRes.opres_success)
        {
            param.m_resultAcc = accName;
            param.m_validatedCode = m_accInfo.m_validatedCode;
           // savePostfix(user, param.m_accName);
        }
        return res;
    }

    // 创建API账号
    private OpRes createAPI(ParamCreateGmAccount param, GMUser user)
    {
        OpRes res = canCreateAPI(user);
        if (res != OpRes.opres_success)
            return res;

        res = gmPwdValid(param);
        if (res != OpRes.opres_success)
            return res;

        if (!Regex.IsMatch(param.m_accName, Exp.ACCOUNT_API))
        {
            return OpRes.op_res_param_not_valid;
        }

        // 前缀判别
        if (!Regex.IsMatch(param.m_apiPrefix, Exp.ACCOUNT_API_PREFIX))
        {
            return OpRes.op_res_param_not_valid;
        }

        /*{
            string accName = param.m_accName;

            string key = Guid.NewGuid().ToString().Replace("-", "");
            m_accInfo.fillInfo(user, accName, AccType.ACC_API, user.m_moneyType, param, "", 
                user.m_generalAgency, "", key);
           // m_accInfo.m_aliasName = param.m_aliasName;
            res = createAcc(user, param);
            if (res == OpRes.opres_success)
            {
                param.m_resultAcc = accName;
                param.m_validatedCode = m_accInfo.m_validatedCode;
            }
            return res;
        }*/

        // 有前缀，提交审批
        bool exists = ItemHelp.existPostfix(param.m_apiPrefix, user);
        if (exists)
            return OpRes.op_res_prefix_has_exists;

        exists = ItemHelp.existApiPostfix(param.m_apiPrefix, user);
        if (exists)
            return OpRes.op_res_prefix_has_exists;

        exists = user.sqlDb.keyStrExists(TableName.API_APPROVE, "apiAcc",
                                            param.m_accName,
                                            user.getMySqlServerID(),
                                            MySqlDbName.DB_XIANXIA);
        if (exists)
            return OpRes.op_res_account_has_exists;

        exists = user.sqlDb.keyStrExists(TableName.GM_ACCOUNT, "acc", param.m_accName,
                                        user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (exists)
            return OpRes.op_res_account_has_exists;

        double agentRatio = 0, washRatio = 0;
        if (!ItemHelp.isValidAgentRatio(param, user, ref agentRatio))
            return OpRes.op_res_param_not_valid;

        if (!ItemHelp.isValidWashRatio(param, user, ref washRatio))
            return OpRes.op_res_param_not_valid;

        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("apiAcc", param.m_accName, FieldType.TypeString);
        gen.addField("apiPwd", Tool.getMD5Hash(param.m_pwd1), FieldType.TypeString);
        gen.addField("genTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        gen.addField("apiCreator", user.m_user, FieldType.TypeString);
        gen.addField("apiAliasName", param.m_aliasName, FieldType.TypeString);
        gen.addField("apiAgentRatio", agentRatio, FieldType.TypeNumber);
        gen.addField("apiWashRatio", washRatio, FieldType.TypeNumber);
        gen.addField("apiPrefix", param.m_apiPrefix, FieldType.TypeString);
        gen.addField("apiCreatorCode", user.m_createCode, FieldType.TypeString);

        string sqlCmd = gen.getResultSql(TableName.API_APPROVE);
        int count = user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            return OpRes.op_res_wait_approve;
        }
        return OpRes.op_res_db_failed;
    }

    // 创建API管理员
    private OpRes createAPIAdmin(ParamCreateGmAccount param, GMUser user)
    {
        if(!user.isAPIAcc())
            return OpRes.op_res_failed;

        OpRes res = gmPwdValid(param);
        if (res != OpRes.opres_success)
            return res;

        string accName = user.m_user + "ADMIN" + CountMgr.getInstance().genDyId(CountMgr.API_ADMIN);

        m_accInfo.fillInfo(user, accName, AccType.ACC_API_ADMIN, 0, param, user.m_postfix, user.m_generalAgency, "", "");
        res = createAcc(user, param);

        if (res == OpRes.opres_success)
        {
            param.m_resultAcc = accName;
            param.m_validatedCode = m_accInfo.m_validatedCode;
        }
        return res;
    }

    // 创建admin子账号
    private OpRes createAdminSub(ParamCreateGmAccount param, GMUser user)
    {
        if (!user.isAdmin())
            return OpRes.op_res_failed;

        OpRes res = gmPwdValid(param);
        if (res != OpRes.opres_success)
            return res;

        string accName = user.m_user + CountMgr.getInstance().genDyId(CountMgr.SUPER_ADMIN_SUB);

        m_accInfo.fillInfo(user, accName, AccType.ACC_SUPER_ADMIN_SUB, 0, param, user.m_postfix, user.m_generalAgency, "", "");
        res = createAcc(user, param);

        if (res == OpRes.opres_success)
        {
            param.m_resultAcc = accName;
            param.m_validatedCode = m_accInfo.m_validatedCode;
        }
        return res;
    }

    private OpRes createAcc(GMUser user, ParamCreateGmAccount param)
    {
        if (string.IsNullOrEmpty(m_accInfo.m_aliasName))
            return OpRes.op_res_param_not_valid;

        double agentRatio = 0, washRatio = 0;
        if(!ItemHelp.isValidAgentRatio(param, user, ref agentRatio))
            return OpRes.op_res_param_not_valid;

        if (!ItemHelp.isValidWashRatio(param, user, ref washRatio))
            return OpRes.op_res_param_not_valid;

        bool res = user.sqlDb.keyStrExists(TableName.GM_ACCOUNT, "acc", m_accInfo.m_accName,
                                                user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (res)
            return OpRes.op_res_account_has_exists; // 账号重复

        ValidatedCodeGenerator vg = new ValidatedCodeGenerator();
        vg.CodeSerial = DefCC.CODE_SERIAL;
        m_accInfo.m_validatedCode = vg.CreateVerifyCode(4);

        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("acc", m_accInfo.m_accName, FieldType.TypeString);
        gen.addField("pwd", Tool.getMD5Hash(m_accInfo.m_pwd), FieldType.TypeString);
        gen.addField("accType", m_accInfo.m_accType, FieldType.TypeNumber);
        gen.addField("createTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        gen.addField("owner", m_accInfo.m_owner, FieldType.TypeString);
        gen.addField("generalAgency", m_accInfo.m_generalAgency, FieldType.TypeString);
        gen.addField("postfix", m_accInfo.m_postfix, FieldType.TypeString);
        gen.addField("money", 0, FieldType.TypeNumber);
        gen.addField("moneyType", m_accInfo.m_moneyType, FieldType.TypeNumber);
        gen.addField("devSecretKey", m_accInfo.m_devSecretKey, FieldType.TypeString);
        gen.addField("gmRight", m_accInfo.m_right, FieldType.TypeString);

        if (m_accInfo.m_accType == AccType.ACC_API_ADMIN ||
            m_accInfo.m_accType == AccType.ACC_AGENCY_SUB ||
            m_accInfo.m_accType == AccType.ACC_SUPER_ADMIN_SUB) // 创建的是同级别账号
        {
            gen.addField("depth", user.m_depth, FieldType.TypeNumber);
        }
        else // 创建的是下级账号
        {
            gen.addField("depth", user.m_depth + 1, FieldType.TypeNumber);
        }

        if (m_accInfo.m_accType == AccType.ACC_SUPER_ADMIN_SUB)
        {
            gen.addField("createCode", user.m_createCode, FieldType.TypeString);
        }
        else
        {
            gen.addField("createCode", genCreateCode(user), FieldType.TypeString);
        }
        
        gen.addField("aliasName", m_accInfo.m_aliasName, FieldType.TypeString);
        gen.addField("validatedCode", m_accInfo.m_validatedCode, FieldType.TypeString);
        gen.addField("agentRatio", agentRatio, FieldType.TypeNumber);
        gen.addField("washRatio", washRatio, FieldType.TypeNumber);

        string sqlCmd = gen.getResultSql(TableName.GM_ACCOUNT);
        int count = user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (count > 0)
        {
            user.m_childCount++;
            updateChildNodeNumber(user);
            return OpRes.opres_success;
        }
        return OpRes.op_res_db_failed;
    }

    private string genCreateCode(GMUser user)
    {
        int n = user.m_childCount + 1;
        if (n >= 10)
        {
            return user.m_createCode + string.Format("({0})", n);
        }
        return user.m_createCode + n.ToString();
    }

    /*private void savePostfix(GMUser user, string postfix)
    {
        string cmd = string.Format("INSERT into {0} (postfix) VALUES ('{1}')",
                                    TableName.GM_ACCOUNT_POSTFIX,
                                    postfix);
        user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
    }*/

    private OpRes gmPwdValid(ParamCreateGmAccount param)
    {
        if (string.IsNullOrEmpty(param.m_pwd1) ||
            string.IsNullOrEmpty(param.m_pwd2))
            return OpRes.op_res_param_not_valid;

        if (param.m_pwd1 != param.m_pwd2)
            return OpRes.op_res_param_not_valid;

        if (!Regex.IsMatch(param.m_pwd1, Exp.GM_ACCOUNT_PWD))
        {
            return OpRes.op_res_param_not_valid;
        }

        return OpRes.opres_success;
    }

    // 当前账号是否可以创建代理账号
    private OpRes canCreateAgency(GMUser user)
    {
        if (user.m_accType != AccType.ACC_AGENCY &&
           user.m_accType != AccType.ACC_GENERAL_AGENCY)
            return OpRes.op_res_no_right;

        OpRes res = OpRes.opres_success;

        // 需要检测相关权限
        if (user.m_accType == AccType.ACC_AGENCY)
        {
            if (!RightMap.hasRight(RIGHT.CREATE_AGENCY, user.m_right))
            {
                res = OpRes.op_res_no_right;
            }
        }

        return res;
    }

    // 当前账号是否可以创建API账号
    private OpRes canCreateAPI(GMUser user)
    {
        if (user.m_accType != AccType.ACC_AGENCY &&
           user.m_accType != AccType.ACC_GENERAL_AGENCY)
            return OpRes.op_res_no_right;

        OpRes res = OpRes.opres_success;

        // 需要检测相关权限
        //if (user.m_accType == AccType.ACC_AGENCY)
        {
            if (!RightMap.hasRight(RIGHT.CREATE_API, user.m_right))
            {
                res = OpRes.op_res_no_right;
            }
        }

        return res;
    }

    private void updateChildNodeNumber(GMUser user)
    {
        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("childNodeNumber", user.m_childCount, FieldType.TypeNumber);
        string sqlCmd = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", user.m_user));
        user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
    }
}

//////////////////////////////////////////////////////////////////////////

// 创建玩家
public class DyOpCreatePlayer : DyOpBase
{
    const string AES_KEY = "&@*(#kas9081fajk";

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamCreateGmAccount p = (ParamCreateGmAccount)param;
        if (user.m_accType != AccType.ACC_AGENCY &&
            user.m_accType != AccType.ACC_API &&
            user.m_accType != AccType.ACC_API_ADMIN) // API号或代理号可以创建玩家
            return OpRes.op_res_no_right;

        if (string.IsNullOrEmpty(p.m_pwd1) ||
            string.IsNullOrEmpty(p.m_pwd2))
            return OpRes.op_res_param_not_valid;

        if (p.m_pwd1 != p.m_pwd2)
            return OpRes.op_res_param_not_valid;

        if (!Regex.IsMatch(p.m_accName, Exp.ACCOUNT_PLAYER))
        {
            return OpRes.op_res_param_not_valid;
        }

        if (!Regex.IsMatch(p.m_pwd1, Exp.ACCOUNT_PLAYER_PWD))
        {
            return OpRes.op_res_param_not_valid;
        }

        // 玩家账号名称
        string acc = null;// user.m_postfix + p.m_accName;

        // 是API账号 或 API管理员
        if (user.isAPIAcc() || user.isAPIAdminAcc())
        {
            acc = user.m_postfix + p.m_accName;
        }
        else
        {
            acc = p.m_accName;
        }

        string error = "";
        bool res = createAccToServer(acc, p.m_pwd1, ref error);
        if (res || error == "-12")  // 账号重复了或成功了
        {
            p.m_resultAcc = acc;
            OpRes resCode = createAcc(acc, p, user);

            if (resCode == OpRes.opres_success)
            {
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_CREATE_PLAYER,
                    new LogCreatePlayer(acc), user);
            }
            return resCode;
        }
        
        p.m_resultAcc = error;
        return OpRes.op_res_failed;
    }

    private OpRes createAcc(string accName, ParamCreateGmAccount param, GMUser user)
    {
        double washRatio = 0;
        if (!ItemHelp.isValidWashRatio(param, user, ref washRatio))
            return OpRes.op_res_param_not_valid;

        bool exists = user.sqlDb.keyStrExists(TableName.PLAYER_ACCOUNT_XIANXIA, "acc", accName, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (exists)
            return OpRes.op_res_account_has_exists;

        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("acc", accName, FieldType.TypeString);

        if (user.isAPIAdminAcc()) // API管理员创建的玩家归于owner名下
        {
            gen.addField("creator", user.m_owner, FieldType.TypeString);
            gen.addField("createCode", ItemHelp.getCreateCodeSpecial(user), FieldType.TypeString);
        }
        else
        {
            gen.addField("creator", user.m_user, FieldType.TypeString);
            gen.addField("createCode", user.m_createCode, FieldType.TypeString);
        }
        
        gen.addField("money", 0, FieldType.TypeNumber);
        gen.addField("moneyType", user.m_moneyType, FieldType.TypeNumber);
        gen.addField("state", PlayerState.STATE_IDLE, FieldType.TypeNumber);
        gen.addField("createTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        gen.addField("aliasName", param.m_aliasName, FieldType.TypeString);
        gen.addField("playerWashRatio", washRatio, FieldType.TypeNumber);

        string sqlCmd = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA);
        int count = user.sqlDb.executeOp(sqlCmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
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

public class ParamModifyLoginPwd
{
    // 更改哪个账号的密码，为空表示更改自己的
    public string m_acc;
    public string m_oriPwd;
    public string m_pwd1;
    public string m_pwd2;

    // 0修改登录密码  1修改四位固定验证码
    public int m_op;
}

// 修改登录到后台的密码
public class DyOpModifyLoginPwd : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamModifyLoginPwd p = (ParamModifyLoginPwd)param;
        OpRes res = OpRes.op_res_failed;

        if (p.m_op == 0)
        {
            res = modifyLoginPwd(p, user);
        }
        else
        {
            res = modifyVerCode(p, user);
        }
        return res;
    }

    private OpRes modifyLoginPwd(ParamModifyLoginPwd p, GMUser user)
    {
        OpRes res = pwdValid(p, Exp.GM_ACCOUNT_PWD);
        if (res != OpRes.opres_success)
            return res;

        string newPwd = Tool.getMD5Hash(p.m_pwd1);
        if (string.IsNullOrEmpty(p.m_acc)) // 改自己的密码
        {
            string oriPwd = Tool.getMD5Hash(p.m_oriPwd);
            if (oriPwd != user.m_pwd)
            {
                res = OpRes.op_res_param_not_valid;
            }
            else
            {
                if (newPwd == user.m_pwd)
                    return OpRes.opres_success;

                res = updatePwdDirect(newPwd, user.m_user, user);
                if (res == OpRes.opres_success)
                {
                    user.m_pwd = newPwd;
                }
            }
        }
        else
        {
            res = updatePwd(newPwd, p.m_acc, user.m_user, user);
        }

        return res;
    }

    private OpRes updatePwd(string newPwd, string acc, string owner, GMUser user)
    {
        string cmd = string.Format(SqlStrCMD.SQL_UPDATE_PWD,
                            TableName.GM_ACCOUNT,
                            newPwd,
                            acc,
                            owner);

        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    private OpRes updatePwdDirect(string newPwd, string acc, GMUser user)
    {
        string cmd = string.Format(SqlStrCMD.SQL_UPDATE_PWD_DIRECT,
                            TableName.GM_ACCOUNT,
                            newPwd,
                            acc);

        int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        return count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    // 密码是否合法
    private OpRes pwdValid(ParamModifyLoginPwd param, string pattern)
    {
        if (string.IsNullOrEmpty(param.m_pwd1) ||
            string.IsNullOrEmpty(param.m_pwd2))
            return OpRes.op_res_param_not_valid;

        if (param.m_pwd1 != param.m_pwd2)
            return OpRes.op_res_param_not_valid;

        if (!Regex.IsMatch(param.m_pwd1, pattern))
        {
            return OpRes.op_res_param_not_valid;
        }

        return OpRes.opres_success;
    }

    //////////////////////////////////////////////////////////////////////////
    // 修改四位固定验证码
    private OpRes modifyVerCode(ParamModifyLoginPwd p, GMUser user)
    {
        OpRes res = pwdValid(p, Exp.VER_CODE_FOUR);
        if (res != OpRes.opres_success)
            return res;

        if (string.IsNullOrEmpty(p.m_acc)) // 改自己的密码
        {
            if (!string.IsNullOrEmpty(user.m_verCode))
            {
                if (p.m_oriPwd != user.m_verCode)
                {
                    res = OpRes.op_res_param_not_valid;
                }

                if (p.m_pwd1 == user.m_verCode) // 不用修改
                    return OpRes.opres_success;
            }

            if (res == OpRes.opres_success)
            {
                SqlUpdateGenerator gen = new SqlUpdateGenerator();
                gen.addField("validatedCode", p.m_pwd1, FieldType.TypeString);
                string cmd = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", user.m_user));
                int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
                if (count > 0)
                {
                    user.m_verCode = p.m_pwd1;
                }
                else
                {
                    res = OpRes.op_res_db_failed;
                }
            }
        }
        else
        {
            SqlUpdateGenerator gen = new SqlUpdateGenerator();
            gen.addField("validatedCode", p.m_pwd1, FieldType.TypeString);
            string cmd = gen.getResultSql(TableName.GM_ACCOUNT,
                string.Format(" acc='{0}' and owner='{1}' ", p.m_acc, user.m_user));

            int count = user.sqlDb.executeOp(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            res = count > 0 ? OpRes.opres_success : OpRes.op_res_db_failed;
        }

        return res;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamPlayerOp
{
    public string m_acc;
}

// 操作玩家数据
public class DyOpPlayerBase : DyOpBase
{
    private const string OP_PLAYER = " acc='{0}' and createCode like '{1}%' ";
    
    // 可否操作玩家
    public bool canOpPlayer(ParamPlayerOp param, GMUser user)
    {
        return canOpPlayer(param.m_acc, user);
    }

    // 可否操作玩家
    public bool canOpPlayer(string playerAcc, GMUser user)
    {
        if (string.IsNullOrEmpty(playerAcc))
            return false;

        string ccode = ItemHelp.getCreateCodeSpecial(user);
        string cond = string.Format(OP_PLAYER, playerAcc, ccode);
        return user.sqlDb.keyExists(TableName.PLAYER_ACCOUNT_XIANXIA, cond, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
    }
}

// 踢玩家
public class DyOpKickPlayer : DyOpPlayerBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamPlayerOp p = (ParamPlayerOp)param;
        if (!canOpPlayer(p, user))
            return OpRes.op_res_no_right;

        OpRes res = kick(p.m_acc, 600, user);
        if (res == OpRes.opres_success)
        {
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_KICK_PLAYER, new LogKickPlayer(p.m_acc), user);
        }
        return res;

        /*OpRes res = OpRes.op_res_failed;
        string url = string.Format(DefCC.URL_KICK_PLAYER, p.m_acc, 600);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            Dictionary<string, object> retData =
                JsonHelper.ParseFromStr<Dictionary<string, object>>(retStr);
            if (retData.ContainsKey("result"))
            {
                int code = Convert.ToInt32(retData["result"]);
                if (code == RetCode.RET_SUCCESS)
                {
                    res = OpRes.opres_success;
                }
            }
        }

        return res;*/
    }

    OpRes kick(string acc, int nt, GMUser user)
    {
        var ret = DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "account", acc, new string[] { "SyncLock" },
            user.getDbServerID(), DbName.DB_PLAYER);

        if (ret == null)
        {
            return OpRes.op_res_player_not_exist;
        }

        if (ret.ContainsKey("SyncLock"))
        {
            int state = Convert.ToInt32(ret["SyncLock"]);
            if (state != 1)
            {
                //returnMsg(RetCode.RET_PLAYER_OFFLINE);
                return OpRes.opres_success;
            }
        }

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["time"] = nt;
        data["key"] = acc;

        bool res = DBMgr.getInstance().save(TableName.KICK_PLAYER, data, "key", acc, user.getDbServerID(), DbName.DB_PLAYER);
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

// 解锁玩家
public class DyOpUnLockPlayer : DyOpPlayerBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamPlayerOp p = (ParamPlayerOp)param;
        if (!canOpPlayer(p, user))
            return OpRes.op_res_no_right;

        return unlock(p.m_acc, user);

        /*OpRes res = OpRes.op_res_failed;

        string url = string.Format(DefCC.URL_UNLOCK_PLAYER, p.m_acc);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            Dictionary<string, object> retData =
                JsonHelper.ParseFromStr<Dictionary<string, object>>(retStr);
            if (retData.ContainsKey("result"))
            {
                int retCode = Convert.ToInt32(retData["result"]);

                if (retCode == RetCode.RET_SUCCESS &&
                    retData.ContainsKey("info"))
                {
                    res = OpRes.opres_success;
                    string retURL = string.Format(DefCC.URL_UNLOCK_PLAYER_RET, p.m_acc, retData["info"]);
                    HttpPost.Get(new Uri(retURL));
                }
            }
        }

        return res;*/
    }

    OpRes unlock(string acc, GMUser user)
    {
        var ret = DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "account", acc, new string[] { "SyncLock", "gold" },
            user.getDbServerID(), DbName.DB_PLAYER);

        if (ret == null)
        {
            return OpRes.op_res_player_not_exist;
        }

        if (!ret.ContainsKey("SyncLock"))
        {
            return OpRes.opres_success;
        }
        int state = Convert.ToInt32(ret["SyncLock"]);
        if (state == 1 || state == 0)
        {
            //returnMsg(RetCode.RET_PLAYER_ONLINE);
            return OpRes.opres_success;
        }

        string retURL = string.Format(DefCC.URL_UNLOCK_PLAYER_RET, acc, ret["gold"]);
        byte[] byarr = HttpPost.Get(new Uri(retURL));
        string retStr = Encoding.UTF8.GetString(byarr);
        if (retStr == "ok")
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["SyncLock"] = (sbyte)0;
            data["gold"] = 0;

            bool res = DBMgr.getInstance().update(TableName.PLAYER_INFO, data, "account", acc, user.getDbServerID(), DbName.DB_PLAYER);
            return res ? OpRes.opres_success : OpRes.op_res_failed;
        }
        return OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

// 清理登录失败次数
public class DyOpClearLoginFailedCount : DyOpPlayerBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamPlayerOp p = (ParamPlayerOp)param;
        if (!canOpPlayer(p, user))
            return OpRes.op_res_no_right;

        OpRes res = OpRes.op_res_failed;

        string url = string.Format(DefCC.URL_CLEAR_FAILED_LOGIN, p.m_acc);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            if (retStr == "0")
            {
                res = OpRes.opres_success;
            }    
        }

        return res;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamPlayerSpecialFlag : ParamPlayerOp
{
    public bool m_isAffectEarning;
}

// 给玩家打标记，该玩家是否会影响盈利率
public class DyOpSetPlayerSpecialFlag : DyOpPlayerBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamPlayerSpecialFlag p = (ParamPlayerSpecialFlag)param;
        if (!canOpPlayer(p, user))
            return OpRes.op_res_no_right;

        bool res = DBMgr.getInstance().keyExists(TableName.PLAYER_INFO, "account", p.m_acc,
            user.getDbServerID(), DbName.DB_PLAYER);

        if (!res)
        {
            return OpRes.op_res_player_not_exist;
        }

        Player pr = new Player(p.m_acc, user);
        if (pr.isInGame())
        {
            return OpRes.op_res_player_in_game;
        }

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["NotSaveRate"] = !p.m_isAffectEarning;

        res = DBMgr.getInstance().update(TableName.PLAYER_INFO, data,
            "account", p.m_acc, user.getDbServerID(), DbName.DB_PLAYER);
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamGameResult
{
    public int m_gameId;
    public int m_roomId;
}

public class ParamGameResultCrocodile : ParamGameResult
{
    public int m_result;
}

public class ParamGameResultDice : ParamGameResult
{
    public int m_dice1;
    public int m_dice2;
    public int m_dice3;
}

// 游戏结果控制
public class DyOpGameResult : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        ParamGameResult p = (ParamGameResult)param;
        OpRes res = OpRes.op_res_failed;

        switch (p.m_gameId)
        {
            case (int)GameId.crocodile:
                {
                    res = setResultCrocodile(param, user);
                }
                break;
            case (int)GameId.dice:
                {
                    res = setResultDice(param, user);
                }
                break;
            case (int)GameId.baccarat:
                {
                    res = setResultBaccarat(param, user);
                }
                break;
            case (int)GameId.shcd:
                {
                    res = setResultShcd(param, user);
                }
                break;
        }

        return res;
    }

    OpRes setResultCrocodile(object param, GMUser user)
    {
        ParamGameResultCrocodile p = (ParamGameResultCrocodile)param;
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("GmIndex", p.m_result);
        bool res = DBMgr.getInstance().update(TableName.CROCODILE_ROOM, upData,
            "room_id", p.m_roomId, user.getDbServerID(), DbName.DB_GAME);
        return res ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    OpRes setResultDice(object param, GMUser user)
    {
        ParamGameResultDice p = (ParamGameResultDice)param;
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("GmDice1", p.m_dice1);
        upData.Add("GmDice2", p.m_dice2);
        upData.Add("GmDice3", p.m_dice3);

        bool res = DBMgr.getInstance().update(TableName.DICE_ROOM, upData,
            "room_id", p.m_roomId, user.getDbServerID(), DbName.DB_GAME);
        return res ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    OpRes setResultBaccarat(object param, GMUser user)
    {
        ParamGameResultCrocodile p = (ParamGameResultCrocodile)param;
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("GmIndex", p.m_result);
        bool res = DBMgr.getInstance().update(TableName.BACCARAT_ROOM, upData,
            "room_id", p.m_roomId, user.getDbServerID(), DbName.DB_GAME);
        return res ? OpRes.opres_success : OpRes.op_res_db_failed;
    }

    OpRes setResultShcd(object param, GMUser user)
    {
        ParamGameResultCrocodile p = (ParamGameResultCrocodile)param;
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("next_card_type", p.m_result);
        upData.Add("insert_time", DateTime.Now);
        bool res = DBMgr.getInstance().insertData(TableName.SHCD_RESULT, upData, user.getDbServerID(), DbName.DB_GAME);
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamMaxBetLimit
{
    // 0查询  1修改  2提交到服务器
    public int m_op;
    public int m_gameId;
    public int m_areaId;
    public int m_newValue;
    // 倍率列表
    public string m_rateList = "";

    public bool isModify()
    {
        return m_op == 1;
    }

    public bool isQuery()
    {
        return m_op == 0;
    }
}

public class SettingMaxLimitInfo
{
    // 房间ID->可设置的最大限制
    public Dictionary<int, int> m_limit = new Dictionary<int, int>();

    public int getMaxLimitInfo(int gameId, int roomId, GMUser user)
    {
        if (m_limit.ContainsKey(roomId))
            return m_limit[roomId];

        IMongoQuery imq1 = Query.EQ("creator", user.m_createCode);
        IMongoQuery imq2 = Query.EQ("gameId", gameId);
        IMongoQuery imq = Query.And(imq1, imq2);

        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.API_MAX_BET_SETTING_LIMIT,
            user.getDbServerID(), DbName.DB_GAME, imq);
        if (data != null)
        {
            string key = "room" + roomId.ToString();
            if (data.ContainsKey(key))
            {
                m_limit.Add(roomId, Convert.ToInt32(data[key]));
                return Convert.ToInt32(data[key]);
            }
            else
            {
                m_limit.Add(roomId, 0);
            }
        }
        else
        {
            m_limit.Add(roomId, 0);
        }
        return 0;
    }
}

public class GameSettingInfo
{
    // gameId-->限制
    Dictionary<int, SettingMaxLimitInfo> m_game = new Dictionary<int, SettingMaxLimitInfo>();

    public int getGameMaxLimit(int gameId, int roomId, GMUser user)
    {
        if (m_game.ContainsKey(gameId))
            return m_game[gameId].getMaxLimitInfo(gameId, roomId, user);

        var i = new SettingMaxLimitInfo();
        m_game.Add(gameId, i);
        return i.getMaxLimitInfo(gameId, roomId, user);
    }
}

// 修改游戏最大下注限制
public class DyOpModifyMaxBetLimit : DyOpPlayerBase
{
    private Dictionary<string, object> m_result = null;
    private GameSettingInfo m_settingInfo = new GameSettingInfo();

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamMaxBetLimit p = (ParamMaxBetLimit)param;
        if (!user.isAPIAcc())
            return OpRes.op_res_no_right;

        OpRes res = OpRes.op_res_failed;
        switch (p.m_op)
        {
            case 0:
                {
                    queryBetLimit(p, user);
                    res = OpRes.opres_success;
                }
                break;
            case 1:
                {
                    if (p.m_gameId == (int)GameId.fishpark)
                    {
                        res = modifyFishRateList(p, user);
                    }
                    else
                    {
                        res = modifyLimit(p, user);
                    }
                }
                break;
            case 2:
                {
                    res = flushToGameServer(p, user);
                }
                break;
        }
        return res;
    }

    public override object getResult()
    {
        return m_result;
    }

    void queryBetLimit(ParamMaxBetLimit p, GMUser user)
    {
        IMongoQuery imq1 = Query.EQ("creator", user.m_createCode);
        IMongoQuery imq2 = Query.EQ("gameId", p.m_gameId);
        IMongoQuery imq = Query.And(imq1, imq2);

        m_result = DBMgr.getInstance().getTableData(TableName.API_MAX_BET_LIMIT,
                                        user.getDbServerID(),
                                        DbName.DB_GAME,
                                        imq);

        if (m_result != null)
        {
            m_result.Add("base", DefCC.MONEY_BASE);
        }
    }

    OpRes modifyLimit(ParamMaxBetLimit p, GMUser user)
    {
        int roomId = 1;
       
        long newValue = ItemHelp.saveMoneyValue(p.m_newValue);
        int maxLimit = m_settingInfo.getGameMaxLimit(p.m_gameId, roomId, user); // 读出来的数据未除10
        if (newValue > maxLimit)
            return OpRes.op_res_beyond_range;
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("area" + p.m_areaId.ToString(), newValue);
        IMongoQuery imq1 = Query.EQ("creator", user.m_createCode);
        IMongoQuery imq2 = Query.EQ("gameId", p.m_gameId);
        IMongoQuery imq = Query.And(imq1, imq2);

        bool res = DBMgr.getInstance().update(TableName.API_MAX_BET_LIMIT, upData, imq, user.getDbServerID(),
            DbName.DB_GAME, true);

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    // 刷新到游戏服务器
    public OpRes flushToGameServer(ParamMaxBetLimit p, GMUser user)
    {
        string fmt = string.Format("cmd=2&gameId={0}&creator={1}", p.m_gameId, user.m_createCode);

        string url = string.Format(DefCC.HTTP_MONITOR, fmt);
        var ret = HttpPost.Get(new Uri(url));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            if (retStr == "ok")
            {
                return OpRes.opres_success;
            }
        }
        return OpRes.op_res_failed;
    }

    // 修改捕鱼(鳄鱼公园)的炮台倍率列表
    OpRes modifyFishRateList(ParamMaxBetLimit p, GMUser user)
    {
        List<int> rateList = new List<int>();
        if (!Tool.parseNumListByComma(p.m_rateList, rateList))
        {
            return OpRes.op_res_failed;
        }
        rateList.Sort();

        string str = "";
        foreach (var r in rateList)
        {
            var newValue = ItemHelp.saveMoneyValue(r);
            str = str + newValue + ",";

            int maxLimit = m_settingInfo.getGameMaxLimit(p.m_gameId, p.m_areaId, user); // 读出来的数据未除10
            if (newValue > maxLimit)
                return OpRes.op_res_beyond_range;
        }
        str = str.Remove(str.Length - 1);
        p.m_rateList = string.Join(",", rateList);

        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("area" + p.m_areaId.ToString(), str);
        IMongoQuery imq1 = Query.EQ("creator", user.m_createCode);
        IMongoQuery imq2 = Query.EQ("gameId", p.m_gameId);
        IMongoQuery imq = Query.And(imq1, imq2);

        bool res = DBMgr.getInstance().update(TableName.API_MAX_BET_LIMIT, upData, imq, user.getDbServerID(),
            DbName.DB_GAME, true);

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamAPISetLimit
{
    public int m_op;
    public int m_roomId;
    public string m_apiAcc;
    public int m_gameId;
    public int m_setLimit;
}

public class DyOpModifyAPISetLimit : DyOpPlayerBase
{
    private List<Dictionary<string, object>> m_result = new List<Dictionary<string, object>>();

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamAPISetLimit p = (ParamAPISetLimit)param;
        if (!(user.isAdmin() || user.isAdminSub()))
            return OpRes.op_res_no_right;

        DestGmUser dst = new DestGmUser(p.m_apiAcc, user);
        if (!dst.m_isExists)
            return OpRes.op_res_no_right;

        if (!dst.isAccType(AccType.ACC_API))
            return OpRes.op_res_no_right;

        OpRes res = OpRes.op_res_failed;

        switch (p.m_op)
        {
            case 0: // 修改
                {
                    res = modifySettingLimit(p, dst, user);
                }
                break;
            case 1: // 查询
                {
                    m_result = DBMgr.getInstance().getDataListFromTable(TableName.API_MAX_BET_SETTING_LIMIT,
                         user.getDbServerID(),
                        DbName.DB_GAME,
                        "creator", dst.m_createCode);
                }
                break;
        }
        if (p.m_setLimit < 0)
            return OpRes.op_res_param_not_valid;

        return res;
    }

    public override object getResult()
    {
        return m_result;
    }

    OpRes modifySettingLimit(ParamAPISetLimit p, DestGmUser dst, GMUser user)
    {
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("room" + p.m_roomId.ToString(), ItemHelp.saveMoneyValue(p.m_setLimit));
        IMongoQuery imq1 = Query.EQ("creator", dst.m_createCode);
        IMongoQuery imq2 = Query.EQ("gameId", p.m_gameId);
        IMongoQuery imq = Query.And(imq1, imq2);

        bool res = DBMgr.getInstance().update(TableName.API_MAX_BET_SETTING_LIMIT, upData, imq, user.getDbServerID(),
            DbName.DB_GAME, true);

        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}



