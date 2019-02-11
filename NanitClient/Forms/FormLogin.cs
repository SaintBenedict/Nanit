using System;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.GlobalFunctions;
using static NaNiT.LocalGlobals;

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
            gl_b_isAboutLoaded = false;
        }

        private void ButPassEntOk_Click(object sender, EventArgs e)
        {
            if (MD5Code(BoxPass.Text + gl_s_OSdate) == gl_s_optionsPasswordReg)
            {
                gl_f_options = new FormOptions();
                gl_f_options.Text = (@"N.A.N.I.T (((ver." + gl_s_version + ")))");
                gl_f_options.Show();
                gl_b_isOptOpen = true;
                this.Close();
            }
            else
            {
                this.Close();
                gl_b_isAboutLoaded = false;
            }
        }
    }
}
