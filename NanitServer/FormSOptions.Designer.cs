namespace NaNiT
{
    partial class FormSOptions
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                Globals.isAboutLoaded = false;
                Globals.isOptOpen = false;
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        public void InitializeComponent()
        {
            this.ButOptSave = new System.Windows.Forms.Button();
            this.ButOptClose = new System.Windows.Forms.Button();
            this.LabelPortServ = new System.Windows.Forms.Label();
            this.ControlBoxPortServ = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.ButStart = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Messaging = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // ButOptSave
            // 
            this.ButOptSave.Location = new System.Drawing.Point(16, 42);
            this.ButOptSave.Name = "ButOptSave";
            this.ButOptSave.Size = new System.Drawing.Size(136, 32);
            this.ButOptSave.TabIndex = 1;
            this.ButOptSave.Text = "Сохранить";
            this.ButOptSave.UseVisualStyleBackColor = true;
            this.ButOptSave.Click += new System.EventHandler(this.ButOptSave_Click);
            // 
            // ButOptClose
            // 
            this.ButOptClose.Location = new System.Drawing.Point(347, 4);
            this.ButOptClose.Name = "ButOptClose";
            this.ButOptClose.Size = new System.Drawing.Size(126, 32);
            this.ButOptClose.TabIndex = 2;
            this.ButOptClose.Text = "Закрыть";
            this.ButOptClose.UseVisualStyleBackColor = true;
            this.ButOptClose.Click += new System.EventHandler(this.ButOptClose_Click);
            // 
            // LabelPortServ
            // 
            this.LabelPortServ.AutoSize = true;
            this.LabelPortServ.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelPortServ.Location = new System.Drawing.Point(12, 9);
            this.LabelPortServ.Name = "LabelPortServ";
            this.LabelPortServ.Size = new System.Drawing.Size(148, 20);
            this.LabelPortServ.TabIndex = 13;
            this.LabelPortServ.Text = "TCP Порт сервера";
            // 
            // ControlBoxPortServ
            // 
            this.ControlBoxPortServ.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ControlBoxPortServ.Location = new System.Drawing.Point(166, 7);
            this.ControlBoxPortServ.Name = "ControlBoxPortServ";
            this.ControlBoxPortServ.Size = new System.Drawing.Size(136, 24);
            this.ControlBoxPortServ.TabIndex = 14;
            this.ControlBoxPortServ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // ButStart
            // 
            this.ButStart.Location = new System.Drawing.Point(166, 42);
            this.ButStart.Name = "ButStart";
            this.ButStart.Size = new System.Drawing.Size(136, 32);
            this.ButStart.TabIndex = 15;
            this.ButStart.Text = "Запустить";
            this.ButStart.UseVisualStyleBackColor = true;
            this.ButStart.Click += new System.EventHandler(this.ButStart_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Messaging});
            this.listView1.Location = new System.Drawing.Point(12, 80);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(461, 196);
            this.listView1.TabIndex = 17;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Messaging
            // 
            this.Messaging.Text = "Сообщения";
            this.Messaging.Width = 429;
            // 
            // FormSOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 289);
            this.ControlBox = false;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.ButStart);
            this.Controls.Add(this.ControlBoxPortServ);
            this.Controls.Add(this.LabelPortServ);
            this.Controls.Add(this.ButOptClose);
            this.Controls.Add(this.ButOptSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки программы";
            this.Closed += new System.EventHandler(this.FormOptions_Close);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButOptSave;
        private System.Windows.Forms.Button ButOptClose;
        private System.Windows.Forms.Label LabelPortServ;
        private System.Windows.Forms.TextBox ControlBoxPortServ;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button ButStart;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader Messaging;
    }
}