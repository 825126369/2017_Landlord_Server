using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExportExcel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            this.ShowInTaskbar = false;
            ToolStripMenuItem v = new ToolStripMenuItem("退出", null, this.onExit);
            m_notifyMenu.Items.Add(v);
            // 关联快捷菜单
            m_notify.ContextMenuStrip = m_notifyMenu;

            this.Text = "电玩城后台服务-" + ResMgr.getInstance().getChannel();
            m_notify.Text = "电玩城后台服务-" + ResMgr.getInstance().getChannel();

            //////////////////////////////////////////////////////////////////////////
           /* ExportParam p = new ExportParam();
            p.m_account = "GM_admin";
            p.m_dbServerIP = "192.168.1.12";
            p.m_tableName = ExportRecharge.TABLE_NAME;
            p.m_condition = new Dictionary<string, object>();
            p.m_condition.Add("userOpDbIp", "192.168.1.205");
            p.m_condition.Add("table", "anysdk_pay");
            p.m_condition.Add("plat", "anysdk");
             
            WorkThreadExport.getInstance().reqExport(p);*/
        }

        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        // 退出应用程序
        private void onExit(object sender, EventArgs e)
        {
            bool busy = ServiceMgr.getInstance().isBusy();
            if (busy)
            {
                MessageBox.Show("当前系统忙碌，请稍候退出！", "确认", MessageBoxButtons.OK);
                return;
            }
            DialogResult result = MessageBox.Show("确定退出?", "确认", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                ServiceMgr.getInstance().exitService();
                Application.Exit();
            }
        }

        // 双击托盘图标
        private void onNotifyMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.TopMost = true;
            }
        }
    }
}
