using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;


namespace NaNiT
{
    public partial class FormOptions : Form
    {
        public FormOptions()
        {
            InitializeComponent();
            ControlBoxIpServ.Text = Globals.servIP;
            ControlBoxPortServ.Text = Globals.servPort;
            CheckRoleAdmin.Checked = Globals.RoleAdmin;
            CheckRoleOperate.Checked = Globals.RoleOperate;
            CheckRoleMessager.Checked = Globals.RoleMessager;
            CheckRoleSecurity.Checked = Globals.RoleSecurity;
            CheckRoleAgent.Checked = Globals.RoleAgent;
            LabelServiceInstall.Text = "Загрузка...";
            LabelServiceInstall.ForeColor = System.Drawing.Color.Black;
            LabelServiceStart.Text = "Загрузка...";
            LabelServiceStart.ForeColor = System.Drawing.Color.Black;
            ButServiceInstall.Enabled = false;
            button1.Visible = false;
            if (Globals.DEBUGMODE == true)
                button1.Visible = true;
            RefreshPliz();
            Program.ServiceInit();
        }

        public void FormOptions_Close(object sender, EventArgs e)
        {
            Globals.isOptOpen = false;
            Globals.isAboutLoaded = false;
        }

        private void ButOptSave_Click(object sender, EventArgs e)
        {
            Globals.servIP = ControlBoxIpServ.Text;
            Globals.servPort = ControlBoxPortServ.Text;
            Globals.RoleAdmin = CheckRoleAdmin.Checked;
            Globals.RoleOperate = CheckRoleOperate.Checked;
            Globals.RoleMessager = CheckRoleMessager.Checked;
            Globals.RoleSecurity = CheckRoleSecurity.Checked;
            Globals.RoleAgent = CheckRoleAgent.Checked;
            Globals.md5PortIp = Program.MD5Code(Globals.servPort + Globals.servIP);
            Globals.md5Clients = Program.MD5Code(Globals.RoleSecurity.ToString().ToLower() + Globals.RoleMessager.ToString().ToLower() + Globals.RoleOperate.ToString().ToLower() + Globals.RoleAdmin.ToString().ToLower() + Globals.RoleAgent.ToString().ToLower());

            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            regNanit.SetValue("ip_server", Globals.servIP);
            regNanit.SetValue("port_server", Globals.servPort);
            regNanit.SetValue("validate_ip_port", Globals.md5PortIp);
            regNanit.SetValue("RoleSecurity", Globals.RoleSecurity.ToString().ToLower());
            regNanit.SetValue("RoleMessager", Globals.RoleMessager.ToString().ToLower());
            regNanit.SetValue("RoleOperate", Globals.RoleOperate.ToString().ToLower());
            regNanit.SetValue("RoleAdmin", Globals.RoleAdmin.ToString().ToLower());
            regNanit.SetValue("RoleAgent", Globals.RoleAgent.ToString().ToLower());
            regNanit.SetValue("validate_clients", Globals.md5Clients);
            regNanit.Close();

            this.Close();
            Globals.isOptOpen = false;
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
                    Globals.isOptOpen = false;
                }
            }
            else
            {
                this.Close();
                Globals.isAboutLoaded = false;
                Globals.isOptOpen = false;
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

        private void ButServiceChange_Click(object sender, EventArgs e)
        {
            if (!Globals.isUpdOpen)
            {
                Globals.form3 = new FormUpdater();
                Globals.form3.Show();
                Globals.isUpdOpen = true;
                ButServiceChange.Enabled = false;
            }
        }

        private void ButServiceInstall_Click(object sender, EventArgs e)
        {
            Program.InstallService();
        }


        private void ButServiceDel_Click(object sender, EventArgs e)
        {
            Program.DeleteService();
        }

        public void RefreshPliz()
        {
            ServiceInit(Globals.serviceStatus);
        }

        public void ServiceInit(byte status)
        {
            switch (status)
            //0 не установлена и не запущена. 1 установлена и запущена. 2 установлена не запущена. 3 обновление запущена 4 обновление не запущена
            {
                case 0:
                    ButServiceDel.Visible = false;
                    ButServiceChange.Visible = true;
                    ButServiceInstall.Text = "Установить";
                    LabelServiceStart.Text = "Не запущена";
                    LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                    LabelServiceInstall.Text = "Не установлена";
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Red;
                    if (Globals.adrUpdNum == -1)
                    {
                        groupBox2.Text = "Служба Windows (для обновлений)";
                        ButServiceInstall.Enabled = false;
                    }
                    else
                    {
                        groupBox2.Text = "Установить службу обновлений версии " + Globals.updVerAvi;
                        ButServiceInstall.Enabled = true;
                    }
                    break;
                case 1:
                    ButServiceDel.Visible = true;
                    ButServiceChange.Visible = false;
                    ButServiceInstall.Text = "ОК";
                    ButServiceInstall.Enabled = false;
                    LabelServiceStart.Text = "Запущена";
                    LabelServiceStart.ForeColor = System.Drawing.Color.Green;
                    LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                    if (Globals.adrUpdNum == -1)
                        groupBox2.Text = "Служба Windows (для обновлений)";
                    else
                        groupBox2.Text = "Доступная версия службы обновлений " + Globals.nanitSvcVer;
                    break;
                case 2:
                    ButServiceDel.Visible = false;
                    ButServiceChange.Visible = true;
                    ButServiceInstall.Text = "Запустить";
                    ButServiceInstall.Enabled = true;
                    LabelServiceStart.Text = "Не запущена";
                    LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                    LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                    if (Globals.adrUpdNum == -1)
                        groupBox2.Text = "Служба Windows (для обновлений)";
                    else
                        groupBox2.Text = "Доступная версия службы обновлений " + Globals.nanitSvcVer;
                    break;
                case 3:
                    ButServiceDel.Visible = false;
                    ButServiceChange.Visible = true;
                    ButServiceInstall.Text = "Обновить";
                    LabelServiceStart.Text = "Запущена";
                    LabelServiceStart.ForeColor = System.Drawing.Color.Green;
                    LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                    if (Globals.adrUpdNum == -1)
                    {
                        groupBox2.Text = "Служба Windows (для обновлений)";
                        ButServiceInstall.Enabled = false;
                    }
                    else
                    {
                        groupBox2.Text = "Обновить службу обновлений до версии " + Globals.updVerAvi;
                        ButServiceInstall.Enabled = true;
                    }
                    break;
                case 4:
                    ButServiceDel.Visible = false;
                    ButServiceChange.Visible = true;
                    LabelServiceStart.Text = "Не запущена";
                    LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                    LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                    if (Globals.adrUpdNum == -1)
                    {
                        ButServiceInstall.Text = "Запустить";
                        groupBox2.Text = "Служба Windows (для обновлений)";
                        ButServiceInstall.Enabled = true;
                    }
                    else
                    {
                        ButServiceInstall.Text = "Обновить";
                        groupBox2.Text = "Обновить службу обновлений до версии " + Globals.updVerAvi;
                        ButServiceInstall.Enabled = true;
                    }
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process cmdInstall = new Process();
            cmdInstall.StartInfo.FileName = "cmd.exe";
            cmdInstall.StartInfo.Arguments = "/C " + @"ping google.com";
            cmdInstall.Start();
            cmdInstall.WaitForExit();
        }
    }
}