using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckBalance
{
    public partial class Form1 : Form
    {
        static int[] s_itemId = { 1, 2, 11, 14 };

        public Form1()
        {
            InitializeComponent();
            initForm();

            CheckForIllegalCrossThreadCalls = false;
        }

        void initForm()
        {
            for (int i = 0; i < StrName.s_onlineGameIdList.Length; i++)
            {
                int gameId = StrName.s_onlineGameIdList[i];
                m_gameList.Items.Add(StrName.s_gameName[gameId]);
            }
            m_gameList.SelectedIndex = 0;

            m_moneyType.Items.Add("金币");
            m_moneyType.Items.Add("钻石");
            m_moneyType.Items.Add("话费碎片");
            m_moneyType.Items.Add("龙珠");
            m_moneyType.SelectedIndex = 0;
        }

        // 开始检测收支平衡
        private void btnCheck_Click(object sender, EventArgs e)
        {
            StatService stat = ServiceMgr.getInstance().getSys<StatService>(ServiceType.serviceTypeStat);
            if (stat.isBusy())
            {
                MessageBox.Show("忙碌中，请稍候");
                return;
            }

            int playerId = 0;
            if (!int.TryParse(txtPlayerId.Text, out playerId))
            {
                MessageBox.Show("玩家ID非法");
                return;
            }

            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(txtGameTime.Text, ref mint, ref maxt);
            if (!res || mint.AddDays(1) != maxt)
            {
                MessageBox.Show("游戏时间非法");
                return;
            }

            ParamCheck param = new ParamCheck();
            param.m_gameId = StrName.s_onlineGameIdList[m_gameList.SelectedIndex];
            param.m_playerId = playerId;
            param.m_startTime = mint;
            param.m_endTime = maxt;
            param.m_itemId = s_itemId[m_moneyType.SelectedIndex];
            param.m_from = this;
            stat.startCheck(param);
        }

        public void begin(int totalCount)
        {
            m_pText.Text = "0%";
            m_progress.Maximum = totalCount;
            m_progress.Value = 0;
        }
        public void finishOne()
        {
            m_progress.Value++;
            double f = (double)m_progress.Value * 100 / m_progress.Maximum;
            m_pText.Text = string.Format("{0:N2}%", f);
        }
        public void done()
        {
            m_progress.Value = m_progress.Maximum;
            m_pText.Text = "100%";
        }
    }
}
