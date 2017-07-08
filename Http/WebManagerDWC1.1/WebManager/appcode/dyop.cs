using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Web.Configuration;
using System.Text;

public enum DyOpType
{
    // 发邮件
    opTypeSendMail,

    //修改密码
    opTypeModifyPwd,

    // 停封账号
    opTypeBlockAcc,

    // 停封玩家ID
    opTypeBlockId,

    // 停封IP
    opTypeBlockIP,

    // 后台充值
    opTypeRecharge,

    // 推送添加APP应用信息
    opTypePushApp,

    // 绑定手机
    opTypeBindPhone,

    // 礼包生成
    opTypeGift,

    // 礼包码生成
    opTypeGiftCode,

    // 兑换
    opTypeExchange,

    // 通告
    opTypeNotify,

    // 运营维护
    opTypeMaintenance,

    // 捕鱼参数调整
    opTypeFishlordParamAdjust,

    // 鳄鱼大亨参数调整
    opTypeCrocodileParamAdjust,

    // 骰宝参数调整
    opTypeDiceParamAdjust,

    // 清空鱼统计表
    opTypeClearFishTable,

    // 重新加载表格
    opTypeReLoadTable,

    // 客服信息
    opTypeServiceInfo,

    // 冻结头像
    opTypeFreezeHead,
}

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

    public override void initSys()
    {
        m_items.Add(DyOpType.opTypeSendMail, new DyOpSendMail());
        m_items.Add(DyOpType.opTypeModifyPwd, new DyOpModifyPwd());
        m_items.Add(DyOpType.opTypeBlockAcc, new DyOpBlockAccount());
        m_items.Add(DyOpType.opTypeBlockId, new DyOpBlockId());
        m_items.Add(DyOpType.opTypeBlockIP, new DyOpBlockIP());

        m_items.Add(DyOpType.opTypeRecharge, new DyOpRecharge());
        m_items.Add(DyOpType.opTypePushApp, new DyOpJPushAddApp());
        m_items.Add(DyOpType.opTypeBindPhone, new DyOpBindPhone());
        m_items.Add(DyOpType.opTypeGift, new DyOpGift());
        m_items.Add(DyOpType.opTypeGiftCode, new DyOpGiftCode());

        m_items.Add(DyOpType.opTypeExchange, new DyOpExchange());
        m_items.Add(DyOpType.opTypeNotify, new DyOpNotify());
        m_items.Add(DyOpType.opTypeMaintenance, new DyOpMaintenance());
        m_items.Add(DyOpType.opTypeFishlordParamAdjust, new DyOpFishlordParamAdjust());
        m_items.Add(DyOpType.opTypeCrocodileParamAdjust, new DyOpCrocodileParamAdjust());

        m_items.Add(DyOpType.opTypeClearFishTable, new DyOpClearFishTable());
        m_items.Add(DyOpType.opTypeReLoadTable, new DyOpReLoadTable());
        m_items.Add(DyOpType.opTypeDiceParamAdjust, new DyOpDiceParamAdjust());
        m_items.Add(DyOpType.opTypeServiceInfo, new DyOpServiceInfo());
        m_items.Add(DyOpType.opTypeFreezeHead, new DyOpFreezeHead());
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
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_SEND_MAIL, new LogSendMail(p.m_title, p.m_sender, p.m_content, m_successPlayer, p.m_itemList, days), user);
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
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_SEND_MAIL, new LogSendMail(p.m_title, p.m_sender, p.m_content, "", p.m_itemList, days), user);
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

        OpRightInfo info = ResMgr.getInstance().getOpRightInfo(user.m_type);
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

public class DyOpModifyPwd : DyOpBase
{
  //  static string[] m_fields = { "phone", "acc" };

    // player_info中的
    static string[] s_fields1 = { "account", "platform" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamModifyPwd p = (ParamModifyPwd)param;

        int playerId = 0;

        if (!int.TryParse(p.m_playerId, out playerId))
            return OpRes.op_res_param_not_valid;

        if (string.IsNullOrEmpty(p.m_newPwd))
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
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    /*public override OpRes doDyop(object param, GMUser user)
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
}

public class ResultBlock
{
    public string m_param = "";
    public string m_time = "";
}

// 停封账号
public class DyOpBlockAccount : DyOpBase
{
    static string[] s_fields = { "acc", "blockTime" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamBlock p = (ParamBlock)param;
        bool res = false;
        int accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (accServerId == -1)
            return OpRes.op_res_failed;

        if (p.m_isBlock)
        {
            IMongoQuery imq = Query.EQ("acc", BsonValue.Create(p.m_param));

            long count = DBMgr.getInstance().getRecordCount(TableName.PLAYER_ACCOUNT, imq, accServerId, DbName.DB_ACCOUNT);
            if (count == 0)
                return OpRes.op_res_not_found_data;

            Dictionary<string, object> data = new Dictionary<string, object>();
            // 账号
            data["acc"] = p.m_param;
            data["blockTime"] = DateTime.Now;
            data["block"] = true;
            res = DBMgr.getInstance().update(TableName.PLAYER_ACCOUNT, data, "acc", p.m_param, accServerId, DbName.DB_ACCOUNT);
            if (res)
            {
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_ACC, new LogBlockAcc(p.m_param, p.m_isBlock), user);
            }
        }
        else
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["block"] = false;

            string[] str = Tool.split(p.m_param, ',');
            for (int i = 0; i < str.Length; i++)
            {
                res = DBMgr.getInstance().update(TableName.PLAYER_ACCOUNT, data, "acc", str[i], accServerId, DbName.DB_ACCOUNT);
            }
            if (res)
            {
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_ACC, new LogBlockAcc(p.m_param, p.m_isBlock), user);
            }
        }

        return res ? OpRes.opres_success : OpRes.op_res_failed;
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

        int playerId = 0;
        if (!int.TryParse(p.m_param, out playerId))
        {
            return OpRes.op_res_param_not_valid;
        }

        if (p.m_isBlock)
        {
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
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_ID, new LogBlockId(p.m_param, p.m_isBlock), user);
            }
        }
        else
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["delete"] = false;

            string[] str = Tool.split(p.m_param, ',');
            for (int i = 0; i < str.Length; i++)
            {
                if (!int.TryParse(p.m_param, out playerId))
                {
                    continue;
                }

                res = DBMgr.getInstance().update(TableName.PLAYER_INFO, data, "player_id", playerId, user.getDbServerID(), DbName.DB_PLAYER);
            }
            if (res)
            {
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_ID, new LogBlockId(p.m_param, p.m_isBlock), user);
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
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_IP, new LogBlockIP(ip, p.m_isBlock), user);
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
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_BLOCK_IP, new LogBlockIP(p.m_param, p.m_isBlock), user);
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
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_GM_RECHARGE, new LogGmRecharge(playerId, p.m_rtype, rParam), user);
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

            giftId = CountMgr.getInstance().getCurId(CountMgr.GIFT_KEY);
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

        DateTime now = DateTime.Now;
        DateTime nt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("genTime", nt);
        data.Add("title", p.m_title);
        data.Add("content", p.m_content);
        data.Add("deadTime", nt.AddDays(day));
        data.Add("noticeId", Guid.NewGuid().ToString());
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
}

// 经典捕鱼参数调整
public class DyOpFishlordParamAdjust : DyOpBase
{
    private static string[] s_fileds = new string[] { "TotalIncome", "TotalOutlay" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFishlordParamAdjust p = (ParamFishlordParamAdjust)param;

        if(string.IsNullOrEmpty(p.m_roomList))
            return OpRes.op_res_param_not_valid;

        if (p.m_isReset)
        {
            return resetExp(user, p);
        }
        return modifyExp(user, p);
    }

    private OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
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
            data.Add("EarningsRate", expRate);
            int roomId = Convert.ToInt32(rooms[i]);
            DBMgr.getInstance().update(TableName.FISHLORD_ROOM, data, "room_id", roomId,
                user.getDbServerID(), DbName.DB_GAME);
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_MODIFY_FISHLORD_GAIN_RATE,
            new LogModifyFishlordRoomExpRate(p.m_roomList, expRate, (int)GameId.fishlord), user);

        return OpRes.opres_success;
    }

    private OpRes resetExp(GMUser user, ParamFishlordParamAdjust p)
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
            DBMgr.getInstance().update(TableName.FISHLORD_ROOM, data, "room_id", roomId,
                user.getDbServerID(), DbName.DB_GAME);
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_RESET_FISHLORD_GAIN_RATE,
            new LogResetFishlordRoomExpRate(p.m_roomList, (int)GameId.fishlord), user);

        return OpRes.opres_success;
    }

    // 增加旧的盈利率
    private OpRes addOldEarningsRate(GMUser user, int roomId, DateTime now)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.FISHLORD_ROOM, "room_id", roomId,
             s_fileds, user.getDbServerID(), DbName.DB_GAME);

        if (data == null)
            return OpRes.opres_success;

        long income = 0, outlay = 0;
        if (data.ContainsKey("TotalIncome"))
        {
            income = Convert.ToInt64(data["TotalIncome"]);
        }
        if (data.ContainsKey("TotalOutlay"))
        {
            outlay = Convert.ToInt64(data["TotalOutlay"]);
        }

        Dictionary<string, object> old = new Dictionary<string, object>();
        old.Add("gameId", (int)GameId.fishlord);
        old.Add("roomId", roomId);
        old.Add("time", now);
        old.Add("income", income);
        old.Add("outlay", outlay);
        DBMgr.getInstance().insertData(TableName.PUMP_OLD_EARNINGS_RATE, old, user.getDbServerID(), DbName.DB_PUMP);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 鳄鱼大亨参数调整
public class DyOpCrocodileParamAdjust : DyOpBase
{
    private static string[] s_fileds = new string[] { "room_income", "room_outcome" };

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

    private OpRes modifyExp(GMUser user, ParamFishlordParamAdjust p)
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

    private OpRes resetExp(GMUser user, ParamFishlordParamAdjust p)
    {
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
            DBMgr.getInstance().update(TableName.CROCODILE_ROOM, data, "room_id", roomId,
                user.getDbServerID(), DbName.DB_GAME);
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_RESET_FISHLORD_GAIN_RATE,
            new LogResetFishlordRoomExpRate(p.m_roomList, (int)GameId.crocodile), user);

        return OpRes.opres_success;
    }

    // 增加旧的盈利率
    private OpRes addOldEarningsRate(GMUser user, int roomId, DateTime now)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.CROCODILE_ROOM, "room_id", roomId,
             s_fileds, user.getDbServerID(), DbName.DB_GAME);

        if (data == null)
            return OpRes.opres_success;

        long income = 0, outlay = 0;
        if (data.ContainsKey("room_income"))
        {
            income = Convert.ToInt64(data["room_income"]);
        }
        if (data.ContainsKey("room_outcome"))
        {
            outlay = Convert.ToInt64(data["room_outcome"]);
        }

        Dictionary<string, object> old = new Dictionary<string, object>();
        old.Add("gameId", (int)GameId.crocodile);
        old.Add("roomId", roomId);
        old.Add("time", now);
        old.Add("income", income);
        old.Add("outlay", outlay);
        DBMgr.getInstance().insertData(TableName.PUMP_OLD_EARNINGS_RATE, old, user.getDbServerID(), DbName.DB_PUMP);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 骰宝参数调整
public class DyOpDiceParamAdjust : DyOpBase
{
    private static string[] s_fileds = new string[] { "room_income", "room_outcome" };

    public override OpRes doDyop(object param, GMUser user)
    {
        ParamFishlordParamAdjust p = (ParamFishlordParamAdjust)param;

        if (string.IsNullOrEmpty(p.m_roomList))
            return OpRes.op_res_param_not_valid;

        return resetExp(user, p);
    }

    private OpRes resetExp(GMUser user, ParamFishlordParamAdjust p)
    {
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
            DBMgr.getInstance().update(TableName.DICE_ROOM, data, "room_id", roomId,
                user.getDbServerID(), DbName.DB_GAME);
        }

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_RESET_FISHLORD_GAIN_RATE,
            new LogResetFishlordRoomExpRate(p.m_roomList, (int)GameId.dice), user);

        return OpRes.opres_success;
    }

    // 增加旧的盈利率
    private OpRes addOldEarningsRate(GMUser user, int roomId, DateTime now)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.DICE_ROOM, "room_id", roomId,
             s_fileds, user.getDbServerID(), DbName.DB_GAME);

        if (data == null)
            return OpRes.opres_success;

        long income = 0, outlay = 0;
        if (data.ContainsKey("room_income"))
        {
            income = Convert.ToInt64(data["room_income"]);
        }
        if (data.ContainsKey("room_outcome"))
        {
            outlay = Convert.ToInt64(data["room_outcome"]);
        }

        Dictionary<string, object> old = new Dictionary<string, object>();
        old.Add("gameId", (int)GameId.dice);
        old.Add("roomId", roomId);
        old.Add("time", now);
        old.Add("income", income);
        old.Add("outlay", outlay);
        DBMgr.getInstance().insertData(TableName.PUMP_OLD_EARNINGS_RATE, old, user.getDbServerID(), DbName.DB_PUMP);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 清空鱼统计表
public class DyOpClearFishTable : DyOpBase
{
    public override OpRes doDyop(object param, GMUser user)
    {
        DBMgr.getInstance().clearTable(TableName.PUMP_ALL_FISH, user.getDbServerID(), DbName.DB_PUMP);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

enum ReloadTable
{
    // 鱼表
    fish,    
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
            case (int)ReloadTable.fish: // 加载鱼表
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data.Add("key", "reload_cfg");
                    res = DBMgr.getInstance().insertData(TableName.RELOAD_FISHCFG, data, user.getDbServerID(), DbName.DB_GAME);
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

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("playerId", playerId);
        data.Add("rtype", (int)RechargeType.delIconCustom);
        data.Add("paramTime", DateTime.Now.AddDays(days));
        res = DBMgr.getInstance().insertData(TableName.GM_RECHARGE, data, user.getDbServerID(), DbName.DB_PLAYER);

        if (res)
        {
            OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_DEL_CUSTOM_HEAD, new LogFreezeHead(playerId), user);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }
}
