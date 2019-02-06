namespace NaNiT
{
    partial class FormUpdater
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
                Globals.isUpdOpen = false;
            }
            base.Dispose(disposing);
            Globals.isUpdOpen = false;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.ChList = new System.Windows.Forms.CheckedListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ButChg = new System.Windows.Forms.Button();
            this.ButDel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(322, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "Добавление или изменение адресов, к которым\rпрограмма будет пытаться обращаться з" +
    "а новыми версиями";
            // 
            // ChList
            // 
            this.ChList.CheckOnClick = true;
            this.ChList.FormattingEnabled = true;
            this.ChList.Location = new System.Drawing.Point(15, 52);
            this.ChList.Name = "ChList";
            this.ChList.Size = new System.Drawing.Size(270, 169);
            this.ChList.TabIndex = 1;
            this.ChList.SelectedIndexChanged += new System.EventHandler(this.ChList_ItemCheck);

            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 230);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(235, 20);
            this.textBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(253, 227);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 24);
            this.button1.TabIndex = 3;
            this.button1.Text = "Сохранить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ButChg
            // 
            this.ButChg.Location = new System.Drawing.Point(294, 52);
            this.ButChg.Name = "ButChg";
            this.ButChg.Size = new System.Drawing.Size(40, 40);
            this.ButChg.TabIndex = 4;
            this.ButChg.Text = "Chg";
            this.ButChg.UseVisualStyleBackColor = true;
            this.ButChg.Click += new System.EventHandler(this.ButChg_Click);
            // 
            // ButDel
            // 
            this.ButDel.Location = new System.Drawing.Point(294, 98);
            this.ButDel.Name = "ButDel";
            this.ButDel.Size = new System.Drawing.Size(40, 40);
            this.ButDel.TabIndex = 5;
            this.ButDel.Text = "Del";
            this.ButDel.UseVisualStyleBackColor = true;
            this.ButDel.Click += new System.EventHandler(this.ButDel_Click);
            // 
            // FormUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 258);
            this.Controls.Add(this.ButDel);
            this.Controls.Add(this.ButChg);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ChList);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUpdater";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Адреса для получения обновлений";
            this.Closed += new System.EventHandler(this.FormUpdater_Close);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Deactivate += new System.EventHandler(this.FormUpdater_Deactivate);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox ChList;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button ButChg;
        private System.Windows.Forms.Button ButDel;
    }
}