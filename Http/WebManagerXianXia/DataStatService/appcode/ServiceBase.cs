using System.Collections.Generic;

public enum ServiceType
{
    serviceTypeWinLose,
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
        addSys(new StatWinLoseService());
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
//////////////////////////////////////////////////////////////////////////
public enum SysType
{
    sysTypeWinLoseStat,
}

public class SysBase
{
    public virtual void init() { }

    public virtual void exit() { }

    public virtual void update(double delta) { }
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





