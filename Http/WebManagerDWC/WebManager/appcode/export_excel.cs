using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.Web.Configuration;

public enum ExportType
{
    // 充值记录的导出
    exportTypeRecharge,

    // 玩家金币，钻石的变化情况
    exportTypeMoney,

    // 成就导出
    exportTypeAchievement,

    // 导出金币预警
    exportTypeMoneyWarn,
    // 充值用户
    exportTypeRechargePlayer,
}

// 导出Excel管理
public class ExportMgr : SysBase
{
    private Dictionary<ExportType, ExportBase> m_items = new Dictionary<ExportType, ExportBase>();

    public ExportMgr()
    {
        m_sysType = SysType.sysTypeExport;
    }

    public OpRes doExport(object param, ExportType exportName, GMUser user)
    {
        if (!m_items.ContainsKey(exportName))
        {
            LOGW.Info("不存在名称为[{0}]的导出", exportName);
            return OpRes.op_res_failed;
        }
        return m_items[exportName].doExport(param, user);
    }

    public override void initSys()
    {
        m_items.Add(ExportType.exportTypeRecharge, new ExportRecharge());
        m_items.Add(ExportType.exportTypeMoney, new ExportMoney());
        m_items.Add(ExportType.exportTypeMoneyWarn, new ExportMoneyWarn());
        m_items.Add(ExportType.exportTypeRechargePlayer, new ExportRechagePlayer());
    }
}

///////////////////////////////////////////////////////////////////////////////

public class ExportBase
{
    protected QueryCondition m_cond = new QueryCondition();

    public virtual OpRes doExport(object param, GMUser user) { return OpRes.op_res_failed; }
}

///////////////////////////////////////////////////////////////////////////////

// 导出玩家的充值记录
public class ExportRecharge : ExportBase
{
    public override OpRes doExport(object param, GMUser user)
    {
        m_cond.startExport();
        QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
        OpRes res = mgr.makeQuery(param, QueryType.queryTypeRecharge, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        ExportParam ep = new ExportParam();
        ep.m_account = user.m_user;
        ep.m_dbServerIP = WebConfigurationManager.AppSettings["payment"];
        ep.m_tableName = "recharge";
        ep.m_condition = m_cond.getCond();
        // 当前用户操作的DB服务器
        ep.m_condition.Add("userOpDbIp", user.m_dbIP);
        return RemoteMgr.getInstance().reqExportExcel(ep);
    }
}

///////////////////////////////////////////////////////////////////////////////

// 导出金币变化记录
public class ExportMoney : ExportBase
{
    public override OpRes doExport(object param, GMUser user)
    {
        m_cond.startExport();
        QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
        OpRes res = mgr.makeQuery(param, QueryType.queryTypeMoney, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        ExportParam ep = new ExportParam();
        ep.m_account = user.m_user;
        ep.m_dbServerIP = user.m_dbIP;
        ep.m_tableName = TableName.PUMP_PLAYER_MONEY;
        ep.m_condition = m_cond.getCond();
        return RemoteMgr.getInstance().reqExportExcel(ep);
    }
}

///////////////////////////////////////////////////////////////////////////////

// 金币预警导出
public class ExportMoneyWarn : ExportBase
{
    public override OpRes doExport(object param, GMUser user)
    {
        ParamQuery p = (ParamQuery)param;

        ExportParam ep = new ExportParam();
        ep.m_account = user.m_user;
        // 这个服务器上的成就
        ep.m_dbServerIP = user.m_dbIP;
        ep.m_tableName = TableName.PLAYER_INFO;
        ep.m_condition = new Dictionary<string, object>();
        ep.m_condition.Add("sel", (int)p.m_way);
        ep.m_condition.Add("count", (int)p.m_countEachPage);
        return RemoteMgr.getInstance().reqExportExcel(ep);
    }
}

///////////////////////////////////////////////////////////////////////////////
// 充值用户统计
public class ExportRechagePlayer : ExportBase
{
    public override OpRes doExport(object param, GMUser user)
    {
        m_cond.startExport();
        StatMgr mgr = user.getSys<StatMgr>(SysType.sysTypeStat);
        OpRes res = mgr.makeQuery(param, StatType.statTypeRechargePlayer, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        ExportParam ep = new ExportParam();
        ep.m_account = user.m_user;
        ep.m_dbServerIP = user.m_dbIP;
        ep.m_tableName = TableName.PUMP_RECHARGE_PLAYER;
        ep.m_condition = m_cond.getCond();
        return RemoteMgr.getInstance().reqExportExcel(ep);
    }
}

