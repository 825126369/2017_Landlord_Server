using System.Web.Configuration;
using System.Collections.Generic;
using System.Web;

public enum SysType
{
    sysTypeQuery,
    sysTypeStat,
    sysTypeDyOp,
    sysTypeExport,

    // 账号系统
    sysTypeAccount,

    // 权限系统
    sysTypeRight,

    // 渠道编辑
    sysTypeChannel,
}

public class SysBase
{
    protected SysType m_sysType;

    public virtual void initSys() { }

    public virtual void exitSys() { }

    public SysType getSysType() { return m_sysType; }
}

public class SysMgr
{
    protected Dictionary<SysType, SysBase> m_sys = new Dictionary<SysType, SysBase>();

    public void addSys(SysBase sys)
    {
        if (sys == null)
            return;

        m_sys.Add(sys.getSysType(), sys);
    }

    public T getSys<T>(SysType sysType) where T : SysBase 
    {
        if (m_sys.ContainsKey(sysType))
        {
            return (T)m_sys[sysType];
        }
        return default(T);
    }

    public void initSys()
    {
        foreach (var p in m_sys.Values)
        {
            p.initSys();
        }
    }

    public void exit()
    {
        foreach (var p in m_sys.Values)
        {
            p.exitSys();
        }
    }

    public static T getGlobalSys<T>(SysType sysType) where T : SysBase
    {
        SysMgr mgr = (SysMgr)HttpContext.Current.Application["sys"];
        return mgr.getSys<T>(sysType);
    }
}





