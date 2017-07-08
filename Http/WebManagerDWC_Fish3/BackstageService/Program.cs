using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace ExportExcel
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            LogMgr.init();

            XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
            int port = xml.getInt("exportPort", 60002);

            TcpServerChannel tcpChannel = new TcpServerChannel(port);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServersEngine), "ServersEngine", WellKnownObjectMode.Singleton);
            ChannelServices.RegisterChannel(tcpChannel, false);

            ServiceMgr.getInstance().init();

            ServersEngine.s_callService = ServiceMgr.getInstance().doService;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
