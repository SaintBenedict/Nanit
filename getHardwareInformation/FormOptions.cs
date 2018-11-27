using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NaNiT
{
    public partial class FormOptions : Form
    {
        public FormOptions()
        {
            InitializeComponent();
            ControlBoxIpServ.Text = Globals.servIP;
        }

        private void FormOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Globals.isAboutLoaded = false;
            if (e.CloseReason == CloseReason.UserClosing)
                Globals.isAboutLoaded = false;
        }

        private void ButOptSave_Click(object sender, EventArgs e)
        {
            Globals.servIP = ControlBoxIpServ.Text;
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            regNanit.SetValue("ipserver", Globals.servIP);
            regNanit.Close();

            this.Close();
            Globals.isAboutLoaded = false;
        }

        private void ButOptClose_Click(object sender, EventArgs e)
        {
            if (Globals.servIP != ControlBoxIpServ.Text)
            {
                const string message = "Закрыть настройки? Все несохранённые изменения будут потеряны";
                const string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                    Globals.isAboutLoaded = false;
                }
            }
            else
            {
                this.Close();
                Globals.isAboutLoaded = false;
            }
        }

        private void ButChangePass_Click(object sender, EventArgs e)
        {
            string tryPass = Program.MD5Code(BoxPassOld.Text);
            if (tryPass == Globals.optionsPasswordReg)
            {
                if (BoxPassNew.Text == BoxPassNew2.Text)
                {
                    Globals.optionsPasswordReg = Program.MD5Code(BoxPassNew.Text);
                    RegistryKey localMachineKey = Registry.LocalMachine;
                    RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
                    RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
                    regNanit.SetValue("password", Globals.optionsPasswordReg);
                    regNanit.Close();
                    const string message = "Пароль успешно изменён";
                    const string caption = "";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    BoxPassNew.Text = "";
                    BoxPassNew2.Text = "";
                    BoxPassOld.Text = "";
                }
                else
                {
                    const string message = "Введённые пароли не совпадают";
                    const string caption = "";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    BoxPassNew.Text = "";
                    BoxPassNew2.Text = "";
                    BoxPassOld.Text = "";
                }
            }
            else
            {
                const string message = "Текущий пароль введён неверно";
                const string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                BoxPassNew.Text = "";
                BoxPassNew2.Text = "";
                BoxPassOld.Text = "";
            }
        }
    }
}
