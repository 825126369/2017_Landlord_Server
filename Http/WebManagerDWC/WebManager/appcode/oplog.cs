using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Reflection;

struct LogType
{
    // 发邮件
    public const int LOG_TYPE_SEND_MAIL = 0;

    // 封IP
    public const int LOG_TYPE_BLOCK_IP = 1;

    // 封账号
    public const int LOG_TYPE_BLOCK_ACC = 2;

    // 重置密码
    public const int LOG_TYPE_RESET_PWD = 3;

    // 后台充值
    public const int LOG_TYPE_GM_RECHARGE = 4;

    // 停封玩家ID
    public const int LOG_TYPE_BLOCK_ID = 5;

    // 修改捕鱼盈利率
    public const int LOG_TYPE_MODIFY_FISHLORD_GAIN_RATE = 6;

    // 重置捕鱼盈利率
    public const int LOG_TYPE_RESET_FISHLORD_GAIN_RATE = 7;

    // 删除自定义头像
    public const int LOG_TYPE_DEL_CUSTOM_HEAD = 8;

    // 增加牛牛牌型
    public const int LOG_TYPE_COWS_ADD_CARDS_TYPE = 9;

    // 祝福与诅咒
    public const int LOG_TYPE_WISH_CURSE = 10;
}

//////////////////////////////////////////////////////////////////////////

// 各类操作参数的基础
[Serializable]
class OpParam
{
    public OpParam() { }
    // 取得描述串
    public virtual string getDescription(OpInfo info, string str) { return ""; }
    // 取得存储数据库的串
    public virtual string getString() { return ""; }
}

//////////////////////////////////////////////////////////////////////////

// 发邮件
[Serializable]
class LogSendMail : OpParam
{
    public string m_title = "";
    public string m_sender = "";
    public string m_content = "";
    public string m_playerList = "";
    public string m_itemList = "";
    public int m_validDay;

    public LogSendMail() { }
    public LogSendMail(string title, string sender, string content, string playerList, string itemList, int days)
    {
        m_title = title;
        m_sender = sender;
        m_content = content;
        m_playerList = playerList;
        m_itemList = itemList;
        m_validDay = days;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogSendMail param = BaseJsonSerializer.deserialize<LogSendMail>(str);
        if (param != null)
        {
            if (param.m_playerList == "")
            {
                return string.Format(info.m_fmt, "全服", param.m_title, param.m_sender, param.m_content, param.m_itemList, param.m_validDay);
            }
            return string.Format(info.m_fmt, param.m_playerList, param.m_title, param.m_sender, param.m_content, param.m_itemList, param.m_validDay);
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 封IP
[Serializable]
class LogBlockIP : OpParam
{
    public string m_ip = "";
    public bool m_isBlock;

    public LogBlockIP() { }
    public LogBlockIP(string ip, bool block)
    {
        m_ip = ip;
        m_isBlock = block;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogBlockIP param = BaseJsonSerializer.deserialize<LogBlockIP>(str);
        if (param != null)
        {
            if (param.m_isBlock)
            {
                return string.Format(info.m_fmt, "封", param.m_ip);
            }
            return string.Format(info.m_fmt, "解封", param.m_ip);
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 封账号
[Serializable]
class LogBlockAcc : OpParam
{
    public string m_acc = "";
    public bool m_isBlock;

    public LogBlockAcc() { }
    public LogBlockAcc(string acc, bool block)
    {
        m_acc = acc;
        m_isBlock = block;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogBlockAcc param = BaseJsonSerializer.deserialize<LogBlockAcc>(str);
        if (param != null)
        {
            if (param.m_isBlock)
            {
                return string.Format(info.m_fmt, "封", param.m_acc);
            }
            return string.Format(info.m_fmt, "解封", param.m_acc);
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 重置密码
[Serializable]
class LogResetPwd : OpParam
{
    public string m_acc = "";
    public string m_phone = "";

    public LogResetPwd() { }
    public LogResetPwd(string acc, string phone)
    {
        m_acc = acc;
        m_phone = phone;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogResetPwd param = BaseJsonSerializer.deserialize<LogResetPwd>(str);
        if (param != null)
        {
            return string.Format(info.m_fmt, param.m_acc, param.m_phone);
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 后台充值
[Serializable]
class LogGmRecharge : OpParam
{
    public int m_playerId;
    public int m_rtype;
    public int m_param;
    
    public LogGmRecharge() { }
    public LogGmRecharge(int playerId, int rtype, int param)
    {
        m_playerId = playerId;
        m_rtype = rtype;
        m_param = param;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogGmRecharge param = BaseJsonSerializer.deserialize<LogGmRecharge>(str);
        if (param != null)
        {
            RechargeCFGData rd = RechargeCFG.getInstance().getValue(param.m_param);
            if (rd != null)
            {
                return string.Format(info.m_fmt, param.m_playerId, StrName.s_rechargeType[param.m_rtype], rd.m_price);
            }
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 封ID
[Serializable]
class LogBlockId : OpParam
{
    public string m_playerId;
    public bool m_isBlock;

    public LogBlockId() { }
    public LogBlockId(string playerId, bool block)
    {
        m_playerId = playerId;
        m_isBlock = block;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogBlockId param = BaseJsonSerializer.deserialize<LogBlockId>(str);
        if (param != null)
        {
            if (param.m_isBlock)
            {
                return string.Format(info.m_fmt, "封", param.m_playerId);
            }
            return string.Format(info.m_fmt, "解封", param.m_playerId);
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 修改了房间的期望盈利率
[Serializable]
class LogModifyFishlordRoomExpRate : OpParam
{
    // 房间列表
    public string m_roomList = "";   
    
    // 盈利率实际值
    public double m_value;

    public int m_gameId;

    public LogModifyFishlordRoomExpRate() { }

    public LogModifyFishlordRoomExpRate(string roomList, double value, int gameId)
    {
        m_roomList = roomList;
        m_value = value;
        m_gameId = gameId;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogModifyFishlordRoomExpRate param = BaseJsonSerializer.deserialize<LogModifyFishlordRoomExpRate>(str);
        if (param.m_roomList == "")
            return "";

        string[] arrs = Tool.split(param.m_roomList, ',', StringSplitOptions.RemoveEmptyEntries);
        string room = "";

        for (int i = 0; i < arrs.Length; i++)
        {
            int roomType = Convert.ToInt32(arrs[i]);
            room += StrName.s_roomName[roomType - 1] + ",";
        }

        return string.Format(info.m_fmt, room, param.m_value, StrName.s_gameName[param.m_gameId]);
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 重置了房间的期望盈利率
[Serializable]
class LogResetFishlordRoomExpRate : OpParam
{
    // 房间列表
    public string m_roomList = "";
    public int m_gameId;

    public LogResetFishlordRoomExpRate() { }

    public LogResetFishlordRoomExpRate(string roomList, int gameId)
    {
        m_roomList = roomList;
        m_gameId = gameId;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogModifyFishlordRoomExpRate param = BaseJsonSerializer.deserialize<LogModifyFishlordRoomExpRate>(str);
        if (param.m_roomList == "")
            return "";

        string[] arrs = Tool.split(param.m_roomList, ',', StringSplitOptions.RemoveEmptyEntries);
        string room = "";

        for (int i = 0; i < arrs.Length; i++)
        {
            int roomType = Convert.ToInt32(arrs[i]);
            room += StrName.s_roomName[roomType - 1] + ",";
        }

        return string.Format(info.m_fmt, room, StrName.s_gameName[param.m_gameId]);
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 冻结头像
[Serializable]
class LogFreezeHead : OpParam
{
    public int m_playerId;
    public DateTime m_deadTime;

    public LogFreezeHead() { }
    public LogFreezeHead(int playerId, DateTime deadTime)
    {
        m_playerId = playerId;
        m_deadTime = deadTime;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogFreezeHead param = BaseJsonSerializer.deserialize<LogFreezeHead>(str);
        if (param != null)
        {
            return string.Format(info.m_fmt, param.m_playerId, param.m_deadTime);
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////

// 增加牛牛牌型
[Serializable]
class LogCowsAddCardType : OpParam
{
    public int m_bankerType;
    public int m_other1Type;
    public int m_other2Type;
    public int m_other3Type;
    public int m_other4Type;

    public LogCowsAddCardType() { }
    public LogCowsAddCardType(int bankerType, 
        int other1Type,int other2Type,int other3Type,int other4Type)
    {
        m_bankerType = bankerType;
        m_other1Type = other1Type;
        m_other2Type = other2Type;
        m_other3Type = other3Type;
        m_other4Type = other4Type;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogCowsAddCardType param = BaseJsonSerializer.deserialize<LogCowsAddCardType>(str);
        if (param != null)
        {
            return string.Format(info.m_fmt, 
                ItemHelp.getCowsCardTypeName(param.m_bankerType),
                ItemHelp.getCowsCardTypeName(param.m_other1Type),
                ItemHelp.getCowsCardTypeName(param.m_other2Type),
                ItemHelp.getCowsCardTypeName(param.m_other3Type),
                ItemHelp.getCowsCardTypeName(param.m_other4Type));
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}

//////////////////////////////////////////////////////////////////////////
// 祝福诅咒
[Serializable]
class LogWishCurse : OpParam
{
    public int m_gameId;     // 游戏ID
    public int m_playerId;   // 玩家ID
    public int m_wishType;   // 祝福诅咒类型
    public int m_opType;     // 操作类型，添加 or 去除
    
    public LogWishCurse() { }
    public LogWishCurse(int gameId, int playerId, int wishType, int opType)
    {
        m_gameId = gameId;
        m_playerId = playerId;
        m_wishType = wishType;
        m_opType = opType;
    }

    public override string getDescription(OpInfo info, string str)
    {
        if (info == null)
            return "引用空";

        LogWishCurse param = BaseJsonSerializer.deserialize<LogWishCurse>(str);
        if (param != null)
        {
            return string.Format(info.m_fmt,
                StrName.s_gameName[param.m_gameId],
                param.m_playerId,
                param.m_opType == 0 ? "添加" : "去除",
                StrName.s_wishCurse[param.m_wishType]);
        }
        return "";
    }

    // 返回要存入数据库的参数串
    public override string getString()
    {
        return BaseJsonSerializer.serialize(this);
    }
}
