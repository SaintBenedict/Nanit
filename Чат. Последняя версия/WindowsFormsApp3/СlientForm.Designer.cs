namespace ChatClient
{
    partial class СlientForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.textSend = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttoncon = new System.Windows.Forms.Button();
            this.buttondis = new System.Windows.Forms.Button();
            this.textChat = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textSend
            // 
            this.textSend.Location = new System.Drawing.Point(12, 418);
            this.textSend.Name = "textSend";
            this.textSend.Size = new System.Drawing.Size(605, 20);
            this.textSend.TabIndex = 0;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(647, 311);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(141, 127);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttoncon
            // 
            this.buttoncon.Location = new System.Drawing.Point(664, 34);
            this.buttoncon.Name = "buttoncon";
            this.buttoncon.Size = new System.Drawing.Size(101, 33);
            this.buttoncon.TabIndex = 2;
            this.buttoncon.Text = "Подключение";
            this.buttoncon.UseVisualStyleBackColor = true;
            this.buttoncon.Click += new System.EventHandler(this.buttoncon_Click);
            // 
            // buttondis
            // 
            this.buttondis.Location = new System.Drawing.Point(664, 92);
            this.buttondis.Name = "buttondis";
            this.buttondis.Size = new System.Drawing.Size(101, 32);
            this.buttondis.TabIndex = 3;
            this.buttondis.Text = "Отключение";
            this.buttondis.UseVisualStyleBackColor = true;
            this.buttondis.Click += new System.EventHandler(this.buttondis_Click);
            // 
            // textChat
            // 
            this.textChat.Location = new System.Drawing.Point(12, 12);
            this.textChat.Multiline = true;
            this.textChat.Name = "textChat";
            this.textChat.Size = new System.Drawing.Size(605, 367);
            this.textChat.TabIndex = 4;
            // 
            // СlientForm
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textChat);
            this.Controls.Add(this.buttondis);
            this.Controls.Add(this.buttoncon);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textSend);
            this.Name = "СlientForm";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textSend;
        public System.Windows.Forms.Button buttonSend;
        public System.Windows.Forms.Button buttoncon;
        public System.Windows.Forms.Button buttondis;
        public System.Windows.Forms.TextBox textChat;
    }
}

