namespace NaNiT
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.LabelPassEnter = new System.Windows.Forms.Label();
            this.ButPassEntOk = new System.Windows.Forms.Button();
            this.ButPassEntCancel = new System.Windows.Forms.Button();
            this.BoxPass = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabelPassEnter
            // 
            this.LabelPassEnter.AutoSize = true;
            this.LabelPassEnter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelPassEnter.Location = new System.Drawing.Point(0, 9);
            this.LabelPassEnter.Name = "LabelPassEnter";
            this.LabelPassEnter.Size = new System.Drawing.Size(264, 13);
            this.LabelPassEnter.TabIndex = 0;
            this.LabelPassEnter.Text = "Введите пароль для доступа к настройкам";
            // 
            // ButPassEntOk
            // 
            this.ButPassEntOk.Location = new System.Drawing.Point(12, 68);
            this.ButPassEntOk.Name = "ButPassEntOk";
            this.ButPassEntOk.Size = new System.Drawing.Size(100, 24);
            this.ButPassEntOk.TabIndex = 1;
            this.ButPassEntOk.Text = "OK";
            this.ButPassEntOk.UseVisualStyleBackColor = true;
            this.ButPassEntOk.Click += new System.EventHandler(this.ButPassEntOk_Click);
            // 
            // ButPassEntCancel
            // 
            this.ButPassEntCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButPassEntCancel.Location = new System.Drawing.Point(153, 68);
            this.ButPassEntCancel.Name = "ButPassEntCancel";
            this.ButPassEntCancel.Size = new System.Drawing.Size(100, 24);
            this.ButPassEntCancel.TabIndex = 2;
            this.ButPassEntCancel.Text = "Отмена";
            this.ButPassEntCancel.UseVisualStyleBackColor = true;
            this.ButPassEntCancel.Click += new System.EventHandler(this.ButPassEntCancel_Click);
            // 
            // BoxPass
            // 
            this.BoxPass.Location = new System.Drawing.Point(12, 35);
            this.BoxPass.Name = "BoxPass";
            this.BoxPass.PasswordChar = '*';
            this.BoxPass.Size = new System.Drawing.Size(241, 20);
            this.BoxPass.TabIndex = 1;
            // 
            // FormLogin
            // 
            this.AcceptButton = this.ButPassEntOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButPassEntCancel;
            this.ClientSize = new System.Drawing.Size(265, 104);
            this.ControlBox = false;
            this.Controls.Add(this.BoxPass);
            this.Controls.Add(this.ButPassEntCancel);
            this.Controls.Add(this.ButPassEntOk);
            this.Controls.Add(this.LabelPassEnter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Deactivate += new System.EventHandler(this.ButPassEntCancel_Click);

        }

        #endregion
        private System.Windows.Forms.Label LabelPassEnter;
        private System.Windows.Forms.Button ButPassEntOk;
        private System.Windows.Forms.Button ButPassEntCancel;
        private System.Windows.Forms.TextBox BoxPass;
    }
}