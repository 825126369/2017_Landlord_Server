namespace DataStatService
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.m_notifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_notify = new System.Windows.Forms.NotifyIcon(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.m_lblCurState = new System.Windows.Forms.Label();
            this.m_lblDbIp = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.m_lblMySql = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_notifyMenu
            // 
            this.m_notifyMenu.Name = "m_notifyMenu";
            this.m_notifyMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // m_notify
            // 
            this.m_notify.Icon = ((System.Drawing.Icon)(resources.GetObject("m_notify.Icon")));
            this.m_notify.Text = "电玩城国外版本 输赢统计";
            this.m_notify.Visible = true;
            this.m_notify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.onNotifyMouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(69, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "当前状态:";
            // 
            // m_lblCurState
            // 
            this.m_lblCurState.AutoSize = true;
            this.m_lblCurState.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_lblCurState.Location = new System.Drawing.Point(175, 93);
            this.m_lblCurState.Name = "m_lblCurState";
            this.m_lblCurState.Size = new System.Drawing.Size(47, 19);
            this.m_lblCurState.TabIndex = 2;
            this.m_lblCurState.Text = "空闲";
            // 
            // m_lblDbIp
            // 
            this.m_lblDbIp.AutoSize = true;
            this.m_lblDbIp.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_lblDbIp.Location = new System.Drawing.Point(182, 19);
            this.m_lblDbIp.Name = "m_lblDbIp";
            this.m_lblDbIp.Size = new System.Drawing.Size(199, 19);
            this.m_lblDbIp.TabIndex = 4;
            this.m_lblDbIp.Text = "192.168.1.205:27027";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(0, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(184, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "连接mongodb服务器:";
            // 
            // m_lblMySql
            // 
            this.m_lblMySql.AutoSize = true;
            this.m_lblMySql.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_lblMySql.Location = new System.Drawing.Point(184, 53);
            this.m_lblMySql.Name = "m_lblMySql";
            this.m_lblMySql.Size = new System.Drawing.Size(139, 19);
            this.m_lblMySql.TabIndex = 6;
            this.m_lblMySql.Text = "192.168.1.205";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(20, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 19);
            this.label4.TabIndex = 5;
            this.label4.Text = "连接MySQL服务器:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 125);
            this.Controls.Add(this.m_lblMySql);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_lblDbIp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_lblCurState);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "电玩城国外版本 输赢统计";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip m_notifyMenu;
        private System.Windows.Forms.NotifyIcon m_notify;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label m_lblCurState;
        private System.Windows.Forms.Label m_lblDbIp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label m_lblMySql;
        private System.Windows.Forms.Label label4;
    }
}

