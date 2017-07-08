namespace CheckBalance
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPlayerId = new System.Windows.Forms.TextBox();
            this.txtGameTime = new System.Windows.Forms.TextBox();
            this.m_gameList = new System.Windows.Forms.ComboBox();
            this.m_progress = new System.Windows.Forms.ProgressBar();
            this.m_pText = new System.Windows.Forms.Label();
            this.btnCheck = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.m_moneyType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(73, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "玩家ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(49, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "游戏时间：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(49, 170);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "所在游戏：";
            // 
            // txtPlayerId
            // 
            this.txtPlayerId.Location = new System.Drawing.Point(141, 40);
            this.txtPlayerId.Name = "txtPlayerId";
            this.txtPlayerId.Size = new System.Drawing.Size(592, 26);
            this.txtPlayerId.TabIndex = 3;
            // 
            // txtGameTime
            // 
            this.txtGameTime.Location = new System.Drawing.Point(141, 99);
            this.txtGameTime.Name = "txtGameTime";
            this.txtGameTime.Size = new System.Drawing.Size(592, 26);
            this.txtGameTime.TabIndex = 4;
            // 
            // m_gameList
            // 
            this.m_gameList.FormattingEnabled = true;
            this.m_gameList.Location = new System.Drawing.Point(141, 170);
            this.m_gameList.Name = "m_gameList";
            this.m_gameList.Size = new System.Drawing.Size(592, 28);
            this.m_gameList.TabIndex = 5;
            // 
            // m_progress
            // 
            this.m_progress.Location = new System.Drawing.Point(34, 470);
            this.m_progress.Name = "m_progress";
            this.m_progress.Size = new System.Drawing.Size(698, 23);
            this.m_progress.TabIndex = 6;
            // 
            // m_pText
            // 
            this.m_pText.AutoSize = true;
            this.m_pText.Location = new System.Drawing.Point(331, 436);
            this.m_pText.Name = "m_pText";
            this.m_pText.Size = new System.Drawing.Size(50, 20);
            this.m_pText.TabIndex = 7;
            this.m_pText.Text = "label4";
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(246, 315);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(239, 80);
            this.btnCheck.TabIndex = 8;
            this.btnCheck.Text = "开始检测";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(50, 237);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "货币属性:";
            // 
            // m_moneyType
            // 
            this.m_moneyType.FormattingEnabled = true;
            this.m_moneyType.Location = new System.Drawing.Point(141, 235);
            this.m_moneyType.Name = "m_moneyType";
            this.m_moneyType.Size = new System.Drawing.Size(592, 28);
            this.m_moneyType.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 507);
            this.Controls.Add(this.m_moneyType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.m_pText);
            this.Controls.Add(this.m_progress);
            this.Controls.Add(this.m_gameList);
            this.Controls.Add(this.txtGameTime);
            this.Controls.Add(this.txtPlayerId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "检测玩家金币变化收支";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPlayerId;
        private System.Windows.Forms.TextBox txtGameTime;
        private System.Windows.Forms.ComboBox m_gameList;
        private System.Windows.Forms.ProgressBar m_progress;
        private System.Windows.Forms.Label m_pText;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox m_moneyType;
    }
}

