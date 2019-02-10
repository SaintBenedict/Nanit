using static NaNiT.GlobalVariable;

namespace NaNiT
{
    partial class FormOptions
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                gl_b_isAboutLoaded = false;
                gl_b_isOptOpen = false;
            }
            base.Dispose(disposing);
            gl_b_isAboutLoaded = false;
            gl_b_isOptOpen = false;
        }

        #region Windows Form Designer generated code

        public void InitializeComponent()
        {
            this.ButOptSave = new System.Windows.Forms.Button();
            this.ButOptClose = new System.Windows.Forms.Button();
            this.LabelIpServ = new System.Windows.Forms.Label();
            this.ControlBoxIpServ = new System.Windows.Forms.TextBox();
            this.BoxPassOld = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BoxPassNew = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BoxPassNew2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ButChangePass = new System.Windows.Forms.Button();
            this.LabelPortServ = new System.Windows.Forms.Label();
            this.ControlBoxPortServ = new System.Windows.Forms.TextBox();
            this.CheckRoleAgent = new System.Windows.Forms.CheckBox();
            this.CheckRoleAdmin = new System.Windows.Forms.CheckBox();
            this.CheckRoleOperate = new System.Windows.Forms.CheckBox();
            this.CheckRoleMessager = new System.Windows.Forms.CheckBox();
            this.CheckRoleSecurity = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ButServiceDel = new System.Windows.Forms.Button();
            this.ButServiceChange = new System.Windows.Forms.Button();
            this.ButServiceInstall = new System.Windows.Forms.Button();
            this.LabelServiceStart = new System.Windows.Forms.Label();
            this.LabelServiceInstall = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButOptSave
            // 
            this.ButOptSave.Location = new System.Drawing.Point(347, 150);
            this.ButOptSave.Name = "ButOptSave";
            this.ButOptSave.Size = new System.Drawing.Size(126, 32);
            this.ButOptSave.TabIndex = 1;
            this.ButOptSave.Text = "Сохранить";
            this.ButOptSave.UseVisualStyleBackColor = true;
            this.ButOptSave.Click += new System.EventHandler(this.ButOptSave_Click);
            // 
            // ButOptClose
            // 
            this.ButOptClose.Location = new System.Drawing.Point(347, 188);
            this.ButOptClose.Name = "ButOptClose";
            this.ButOptClose.Size = new System.Drawing.Size(126, 32);
            this.ButOptClose.TabIndex = 2;
            this.ButOptClose.Text = "Закрыть";
            this.ButOptClose.UseVisualStyleBackColor = true;
            this.ButOptClose.Click += new System.EventHandler(this.ButOptClose_Click);
            // 
            // LabelIpServ
            // 
            this.LabelIpServ.AutoSize = true;
            this.LabelIpServ.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelIpServ.Location = new System.Drawing.Point(183, 12);
            this.LabelIpServ.Name = "LabelIpServ";
            this.LabelIpServ.Size = new System.Drawing.Size(140, 20);
            this.LabelIpServ.TabIndex = 3;
            this.LabelIpServ.Text = "IP адрес сервера";
            // 
            // ControlBoxIpServ
            // 
            this.ControlBoxIpServ.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ControlBoxIpServ.Location = new System.Drawing.Point(337, 12);
            this.ControlBoxIpServ.Name = "ControlBoxIpServ";
            this.ControlBoxIpServ.Size = new System.Drawing.Size(136, 24);
            this.ControlBoxIpServ.TabIndex = 4;
            this.ControlBoxIpServ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BoxPassOld
            // 
            this.BoxPassOld.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BoxPassOld.Location = new System.Drawing.Point(6, 40);
            this.BoxPassOld.Name = "BoxPassOld";
            this.BoxPassOld.PasswordChar = '*';
            this.BoxPassOld.Size = new System.Drawing.Size(136, 24);
            this.BoxPassOld.TabIndex = 6;
            this.BoxPassOld.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Текущий";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Новый";
            // 
            // BoxPassNew
            // 
            this.BoxPassNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BoxPassNew.Location = new System.Drawing.Point(6, 80);
            this.BoxPassNew.Name = "BoxPassNew";
            this.BoxPassNew.PasswordChar = '*';
            this.BoxPassNew.Size = new System.Drawing.Size(136, 24);
            this.BoxPassNew.TabIndex = 8;
            this.BoxPassNew.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Повторите новый";
            // 
            // BoxPassNew2
            // 
            this.BoxPassNew2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BoxPassNew2.Location = new System.Drawing.Point(6, 120);
            this.BoxPassNew2.Name = "BoxPassNew2";
            this.BoxPassNew2.PasswordChar = '*';
            this.BoxPassNew2.Size = new System.Drawing.Size(136, 24);
            this.BoxPassNew2.TabIndex = 10;
            this.BoxPassNew2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ButChangePass);
            this.groupBox1.Controls.Add(this.BoxPassNew2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.BoxPassNew);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.BoxPassOld);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(148, 218);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Смена пароля";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // ButChangePass
            // 
            this.ButChangePass.Location = new System.Drawing.Point(47, 161);
            this.ButChangePass.Name = "ButChangePass";
            this.ButChangePass.Size = new System.Drawing.Size(54, 38);
            this.ButChangePass.TabIndex = 12;
            this.ButChangePass.Text = "OK";
            this.ButChangePass.UseVisualStyleBackColor = true;
            this.ButChangePass.Click += new System.EventHandler(this.ButChangePass_Click);
            // 
            // LabelPortServ
            // 
            this.LabelPortServ.AutoSize = true;
            this.LabelPortServ.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelPortServ.Location = new System.Drawing.Point(183, 50);
            this.LabelPortServ.Name = "LabelPortServ";
            this.LabelPortServ.Size = new System.Drawing.Size(148, 20);
            this.LabelPortServ.TabIndex = 13;
            this.LabelPortServ.Text = "TCP Порт сервера";
            // 
            // ControlBoxPortServ
            // 
            this.ControlBoxPortServ.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ControlBoxPortServ.Location = new System.Drawing.Point(337, 50);
            this.ControlBoxPortServ.Name = "ControlBoxPortServ";
            this.ControlBoxPortServ.Size = new System.Drawing.Size(136, 24);
            this.ControlBoxPortServ.TabIndex = 14;
            this.ControlBoxPortServ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CheckRoleAgent
            // 
            this.CheckRoleAgent.AutoSize = true;
            this.CheckRoleAgent.Checked = true;
            this.CheckRoleAgent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckRoleAgent.Enabled = false;
            this.CheckRoleAgent.Location = new System.Drawing.Point(347, 95);
            this.CheckRoleAgent.Name = "CheckRoleAgent";
            this.CheckRoleAgent.Size = new System.Drawing.Size(99, 17);
            this.CheckRoleAgent.TabIndex = 15;
            this.CheckRoleAgent.Text = "Сетевой агент";
            this.CheckRoleAgent.UseVisualStyleBackColor = true;
            // 
            // CheckRoleAdmin
            // 
            this.CheckRoleAdmin.AutoSize = true;
            this.CheckRoleAdmin.Location = new System.Drawing.Point(170, 95);
            this.CheckRoleAdmin.Name = "CheckRoleAdmin";
            this.CheckRoleAdmin.Size = new System.Drawing.Size(149, 17);
            this.CheckRoleAdmin.TabIndex = 16;
            this.CheckRoleAdmin.Text = "Клиент администратора";
            this.CheckRoleAdmin.UseVisualStyleBackColor = true;
            // 
            // CheckRoleOperate
            // 
            this.CheckRoleOperate.AutoSize = true;
            this.CheckRoleOperate.Location = new System.Drawing.Point(170, 130);
            this.CheckRoleOperate.Name = "CheckRoleOperate";
            this.CheckRoleOperate.Size = new System.Drawing.Size(118, 17);
            this.CheckRoleOperate.TabIndex = 17;
            this.CheckRoleOperate.Text = "Клиент оператора";
            this.CheckRoleOperate.UseVisualStyleBackColor = true;
            // 
            // CheckRoleMessager
            // 
            this.CheckRoleMessager.AutoSize = true;
            this.CheckRoleMessager.Location = new System.Drawing.Point(170, 165);
            this.CheckRoleMessager.Name = "CheckRoleMessager";
            this.CheckRoleMessager.Size = new System.Drawing.Size(137, 17);
            this.CheckRoleMessager.TabIndex = 18;
            this.CheckRoleMessager.Text = "Рассылка сообщений";
            this.CheckRoleMessager.UseVisualStyleBackColor = true;
            // 
            // CheckRoleSecurity
            // 
            this.CheckRoleSecurity.AutoSize = true;
            this.CheckRoleSecurity.Location = new System.Drawing.Point(170, 200);
            this.CheckRoleSecurity.Name = "CheckRoleSecurity";
            this.CheckRoleSecurity.Size = new System.Drawing.Size(166, 17);
            this.CheckRoleSecurity.TabIndex = 19;
            this.CheckRoleSecurity.Text = "Повышенная безопасность";
            this.CheckRoleSecurity.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ButServiceDel);
            this.groupBox2.Controls.Add(this.ButServiceChange);
            this.groupBox2.Controls.Add(this.ButServiceInstall);
            this.groupBox2.Controls.Add(this.LabelServiceStart);
            this.groupBox2.Controls.Add(this.LabelServiceInstall);
            this.groupBox2.Location = new System.Drawing.Point(12, 230);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(460, 50);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Служба Windows (для обновлений)";
            // 
            // ButServiceDel
            // 
            this.ButServiceDel.Location = new System.Drawing.Point(125, 18);
            this.ButServiceDel.Name = "ButServiceDel";
            this.ButServiceDel.Size = new System.Drawing.Size(25, 26);
            this.ButServiceDel.TabIndex = 23;
            this.ButServiceDel.Text = "X";
            this.ButServiceDel.UseVisualStyleBackColor = true;
            this.ButServiceDel.Click += new System.EventHandler(this.ButServiceDel_Click);
            // 
            // ButServiceChange
            // 
            this.ButServiceChange.Location = new System.Drawing.Point(95, 18);
            this.ButServiceChange.Name = "ButServiceChange";
            this.ButServiceChange.Size = new System.Drawing.Size(25, 26);
            this.ButServiceChange.TabIndex = 22;
            this.ButServiceChange.Text = "...";
            this.ButServiceChange.UseVisualStyleBackColor = true;
            this.ButServiceChange.Click += new System.EventHandler(this.ButServiceChange_Click);
            // 
            // ButServiceInstall
            // 
            this.ButServiceInstall.Location = new System.Drawing.Point(7, 18);
            this.ButServiceInstall.Name = "ButServiceInstall";
            this.ButServiceInstall.Size = new System.Drawing.Size(83, 26);
            this.ButServiceInstall.TabIndex = 21;
            this.ButServiceInstall.Text = "Установить";
            this.ButServiceInstall.UseVisualStyleBackColor = true;
            this.ButServiceInstall.Click += new System.EventHandler(this.ButServiceInstall_Click);
            // 
            // LabelServiceStart
            // 
            this.LabelServiceStart.AutoSize = true;
            this.LabelServiceStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelServiceStart.ForeColor = System.Drawing.Color.Green;
            this.LabelServiceStart.Location = new System.Drawing.Point(350, 21);
            this.LabelServiceStart.Name = "LabelServiceStart";
            this.LabelServiceStart.Size = new System.Drawing.Size(22, 13);
            this.LabelServiceStart.TabIndex = 1;
            this.LabelServiceStart.Text = "***";
            // 
            // LabelServiceInstall
            // 
            this.LabelServiceInstall.AutoSize = true;
            this.LabelServiceInstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelServiceInstall.ForeColor = System.Drawing.Color.Red;
            this.LabelServiceInstall.Location = new System.Drawing.Point(172, 21);
            this.LabelServiceInstall.Name = "LabelServiceInstall";
            this.LabelServiceInstall.Size = new System.Drawing.Size(22, 13);
            this.LabelServiceInstall.TabIndex = 0;
            this.LabelServiceInstall.Text = "***";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(443, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 26);
            this.button1.TabIndex = 21;
            this.button1.Text = "ref";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(347, 80);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(125, 10);
            this.progressBar1.TabIndex = 22;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(407, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 26);
            this.button2.TabIndex = 23;
            this.button2.Text = "X";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 289);
            this.ControlBox = false;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CheckRoleSecurity);
            this.Controls.Add(this.CheckRoleMessager);
            this.Controls.Add(this.CheckRoleOperate);
            this.Controls.Add(this.CheckRoleAdmin);
            this.Controls.Add(this.CheckRoleAgent);
            this.Controls.Add(this.ControlBoxPortServ);
            this.Controls.Add(this.LabelPortServ);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ControlBoxIpServ);
            this.Controls.Add(this.LabelIpServ);
            this.Controls.Add(this.ButOptClose);
            this.Controls.Add(this.ButOptSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки программы";
            this.Closed += new System.EventHandler(this.FormOptions_Close);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Deactivate += new System.EventHandler(this.FormOptions_Deactivate);

        }

        #endregion

        private System.Windows.Forms.Button ButOptSave;
        private System.Windows.Forms.Button ButOptClose;
        private System.Windows.Forms.Label LabelIpServ;
        private System.Windows.Forms.TextBox ControlBoxIpServ;
        private System.Windows.Forms.TextBox BoxPassOld;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox BoxPassNew;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox BoxPassNew2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ButChangePass;
        private System.Windows.Forms.Label LabelPortServ;
        private System.Windows.Forms.TextBox ControlBoxPortServ;
        private System.Windows.Forms.CheckBox CheckRoleAgent;
        private System.Windows.Forms.CheckBox CheckRoleAdmin;
        private System.Windows.Forms.CheckBox CheckRoleOperate;
        private System.Windows.Forms.CheckBox CheckRoleMessager;
        private System.Windows.Forms.CheckBox CheckRoleSecurity;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label LabelServiceStart;
        public System.Windows.Forms.Label LabelServiceInstall;
        public System.Windows.Forms.Button ButServiceChange;
        public System.Windows.Forms.Button ButServiceInstall;
        public System.Windows.Forms.Button ButServiceDel;
        private System.Windows.Forms.Button button1;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button2;
    }
}