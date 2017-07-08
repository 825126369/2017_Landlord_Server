using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using log4net;
using System.IO;

public class LogMgr
{
    public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

    public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

    public static void init()
    {
        FileInfo finfo = new FileInfo("..\\data\\log4net.config");
        log4net.Config.XmlConfigurator.Configure(finfo);
    }

    public static void info(string format, params object[] args)
    {
        if (loginfo.IsInfoEnabled)
        {
            loginfo.InfoFormat(format, args);
        }
    }

    public static void error(string format, params object[] args)
    {
        if (logerror.IsErrorEnabled)
        {
            logerror.ErrorFormat(format, args);
        }
    }
}

