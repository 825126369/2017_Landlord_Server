using System.Collections.Generic;

public enum ServiceType
{
    serviceTypeStat,
    serviceTypeStat2,
}

public class ServiceBase
{
    protected ServiceType m_sysType;

    public virtual void initService() { }

    public virtual void exitService() { }

    public ServiceType getSysType() { return m_sysType; }
}

public class ServiceMgr
{
    static ServiceMgr s_obj = null;

    protected Dictionary<ServiceType, ServiceBase> m_sys = new Dictionary<ServiceType, ServiceBase>();

    public static ServiceMgr getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new ServiceMgr();
        }
        return s_obj;
    }

    public void init()
    {
        addSys(new StatService());
        initService();
    }

    public void addSys(ServiceBase sys)
    {
        if (sys == null)
            return;

        m_sys.Add(sys.getSysType(), sys);
    }

    public T getSys<T>(ServiceType sysType) where T : ServiceBase 
    {
        if (m_sys.ContainsKey(sysType))
        {
            return (T)m_sys[sysType];
        }
        return default(T);
    }

    public void initService()
    {
        foreach (var p in m_sys.Values)
        {
            p.initService();
        }
    }

    public void exitService()
    {
        foreach (var p in m_sys.Values)
        {
            p.exitService();
        }
    }
}

//////////////////////////////////////////////////////////////////////////
public enum SysType
{
    // 所有玩家金币总量
    systype_stat_player_total_money,

    // 留存统计
    systype_remain_stat,

    // VIP4流失统计
    systype_lose_stat,

    // 玩家龙珠统计
    systype_PlayerDragonBall,
    // 每日龙珠总计
    systype_DragonBallTotal,
    // 在线游戏时间统计
    systype_OnlineGameTime,
    // 玩家总收支表
    systype_PlayerTotalIncomeExpenses,
    // 玩家总收支表 新
    systype_PlayerTotalIncomeExpensesNew,

    // 每小时累计充值
    systype_RechargeHour,
    // 每小时在线人数
    systype_OnlinePlayerNumHour,
    // 用户各游戏在线时长，及平均游戏时长分布
    systype_GameTimeForPlayerActive,
    // 首付行为
    systype_FirstRecharge,
    // 用户下注情况
    systype_PlayerGameBet,
}

public class SysBase
{
    public virtual void init() { }

    public virtual void exit() { }

    public virtual void update(double delta) { }

    private WatchTime m_wt;

    protected void beginStat(string info, params object[] args)
    {
        m_wt = new WatchTime();
        m_wt.start(info, args);
    }

    protected void endStat(string info, params object[] args)
    {
        if (m_wt != null)
        {
            m_wt.end(info, args);
            m_wt = null;
        }
    }
}

public class SysMgr
{
    protected Dictionary<SysType, SysBase> m_sys = new Dictionary<SysType, SysBase>();

    public void addSys(SysBase sys, SysType t)
    {
        if (sys == null)
            return;

        m_sys.Add(t, sys);
    }

    public T getSys<T>(SysType sysType) where T : SysBase
    {
        if (m_sys.ContainsKey(sysType))
        {
            return (T)m_sys[sysType];
        }
        return default(T);
    }

    public void init()
    {
        foreach (var p in m_sys.Values)
        {
            p.init();
        }
    }

    public void exit()
    {
        foreach (var p in m_sys.Values)
        {
            p.exit();
        }
    }

    public void update(double delta)
    {
        foreach (var p in m_sys.Values)
        {
            p.update(delta);
        }
    }
}



