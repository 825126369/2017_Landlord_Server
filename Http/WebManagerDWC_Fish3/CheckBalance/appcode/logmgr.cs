using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

public static class LogMgr
{
    private static log4net.ILog s_log;

    public static void init()
    {
        string cfg = @"..\data\log4net.config";
        FileInfo f = new FileInfo(cfg);
        log4net.Config.XmlConfigurator.ConfigureAndWatch(f);
        s_log = log4net.LogManager.GetLogger("DefLog");
    }

    public static log4net.ILog log { get { return s_log; } }
}



