using System;
using System.Windows.Forms;

namespace NaNiT
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void ButPassEntCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            Globals.isAboutLoaded = false;
        }

        private void ButPassEntOk_Click(object sender, EventArgs e)
        {
            if (Functions.MD5Code(BoxPass.Text) == Globals.optionsPasswordReg)
            {
                Globals.form2 = new FormOptions();
                Globals.form2.Text = (@"N.A.N.I.T (((ver." + Globals.version + ")))");
                Globals.form2.Show();
                Globals.isOptOpen = true;
                this.Close();
            }
            else
            {
                this.Close();
                Globals.isAboutLoaded = false;
            }
        }
    }
}
