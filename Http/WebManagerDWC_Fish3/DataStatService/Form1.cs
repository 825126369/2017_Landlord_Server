using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataStatService
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

            CheckForIllegalCrossThreadCalls = false;

            m_lblCurState.ForeColor = Color.Green;

            XmlConfig cfg = ResMgr.getInstance().getRes("dbserver.xml");
            this.Text = "数据统计--" + cfg.getString("title", "");
            m_notify.Text = this.Text;

            StatService sys =
                ServiceMgr.getInstance().getSys<StatService>(ServiceType.serviceTypeStat);
            if (sys != null)
            {
                sys.setForm(this);
            }
        }

        public void setDbIP(string ip)
        {
            m_lblDbIp.Text = ip;
        }

        public void setState(int state)
        {
            if (state == 0)
            {
                m_lblCurState.Text = "忙碌";
                m_lblCurState.ForeColor = Color.Red;
            }
            else
            {
                m_lblCurState.Text = "空闲";
                m_lblCurState.ForeColor = Color.Green;
            }
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
