namespace NaNiT
{
    partial class FormOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                Globals.isAboutLoaded = false;
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButOptSave = new System.Windows.Forms.Button();
            this.ButOptClose = new System.Windows.Forms.Button();
            this.LabelIPserv = new System.Windows.Forms.Label();
            this.ControlBoxIpServ = new System.Windows.Forms.TextBox();
            this.BoxPassOld = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BoxPassNew = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BoxPassNew2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ButChangePass = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButOptSave
            // 
            this.ButOptSave.Location = new System.Drawing.Point(233, 150);
            this.ButOptSave.Name = "ButOptSave";
            this.ButOptSave.Size = new System.Drawing.Size(159, 32);
            this.ButOptSave.TabIndex = 1;
            this.ButOptSave.Text = "Сохранить";
            this.ButOptSave.UseVisualStyleBackColor = true;
            this.ButOptSave.Click += new System.EventHandler(this.ButOptSave_Click);
            // 
            // ButOptClose
            // 
            this.ButOptClose.Location = new System.Drawing.Point(233, 188);
            this.ButOptClose.Name = "ButOptClose";
            this.ButOptClose.Size = new System.Drawing.Size(159, 32);
            this.ButOptClose.TabIndex = 2;
            this.ButOptClose.Text = "Закрыть";
            this.ButOptClose.UseVisualStyleBackColor = true;
            this.ButOptClose.Click += new System.EventHandler(this.ButOptClose_Click);
            // 
            // LabelIPserv
            // 
            this.LabelIPserv.AutoSize = true;
            this.LabelIPserv.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelIPserv.Location = new System.Drawing.Point(252, 9);
            this.LabelIPserv.Name = "LabelIPserv";
            this.LabelIPserv.Size = new System.Drawing.Size(140, 20);
            this.LabelIPserv.TabIndex = 3;
            this.LabelIPserv.Text = "IP адрес сервера";
            // 
            // ControlBoxIpServ
            // 
            this.ControlBoxIpServ.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ControlBoxIpServ.Location = new System.Drawing.Point(256, 32);
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
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 232);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ControlBoxIpServ);
            this.Controls.Add(this.LabelIPserv);
            this.Controls.Add(this.ButOptClose);
            this.Controls.Add(this.ButOptSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки программы";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButOptSave;
        private System.Windows.Forms.Button ButOptClose;
        private System.Windows.Forms.Label LabelIPserv;
        private System.Windows.Forms.TextBox ControlBoxIpServ;
        private System.Windows.Forms.TextBox BoxPassOld;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox BoxPassNew;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox BoxPassNew2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ButChangePass;
    }
}