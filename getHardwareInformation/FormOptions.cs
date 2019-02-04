using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;


namespace NaNiT
{
    public partial class FormOptions : Form
    {
        static object locker = new object();
        static bool workerByte = true;

        public FormOptions()
        {
            InitializeComponent();
            Globals.form2 = this;
            Globals.isOptOpen = true;
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
            button2.Visible = false;
            progressBar1.Visible = false;
            progressBar1.Step = 1;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();
            if (Globals.DEBUGMODE == true)
            {
                button1.Visible = true;
                button2.Visible = true;
                //progressBar1.Visible = true;
            }
            ServiceInit();
            ServiceWork.ServiceInit();
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
            Globals.md5PortIp = Functions.MD5Code(Globals.servPort + Globals.servIP);
            Globals.md5Clients = Functions.MD5Code(Globals.RoleSecurity.ToString().ToLower() + Globals.RoleMessager.ToString().ToLower() + Globals.RoleOperate.ToString().ToLower() + Globals.RoleAdmin.ToString().ToLower() + Globals.RoleAgent.ToString().ToLower());

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
            string tryPass = Functions.MD5Code(BoxPassOld.Text);
            if (tryPass == Globals.optionsPasswordReg)
            {
                if (BoxPassNew.Text == BoxPassNew2.Text)
                {
                    Globals.optionsPasswordReg = Functions.MD5Code(BoxPassNew.Text);
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
            Thread InstallService = new Thread(new ThreadStart(ServiceWork.InstallService));
            InstallService.Name = "Install or Update on click";
            InstallService.Start();
        }


        private void ButServiceDel_Click(object sender, EventArgs e)
        {
            Thread DeleteService = new Thread(new ThreadStart(ServiceWork.DeleteService));
            DeleteService.Name = "Delete on click";
            DeleteService.Start();
        }

        public void ServiceInit()
        {
            lock (locker)
            {
                switch (Globals.serviceStatus)
                //0 не установлена и не запущена. 1 установлена и запущена. 2 установлена не запущена. 3 обновление запущена 4 обновление не запущена
                {
                    case 0:
                        Globals.form2.ButServiceDel.Visible = false;
                        Globals.form2.ButServiceChange.Visible = true;
                        Globals.form2.ButServiceInstall.Text = "Установить";
                        Globals.form2.LabelServiceStart.Text = "Не запущена";
                        Globals.form2.LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                        Globals.form2.LabelServiceInstall.Text = "Не установлена";
                        Globals.form2.LabelServiceInstall.ForeColor = System.Drawing.Color.Red;
                        if (Globals.adrUpdNum == -1)
                        {
                            Globals.form2.groupBox2.Text = "Служба Windows (для обновлений)";
                            Globals.form2.ButServiceInstall.Enabled = false;
                        }
                        else
                        {
                            Globals.form2.groupBox2.Text = "Установить службу обновлений версии " + Globals.updVerAvi;
                            Globals.form2.ButServiceInstall.Enabled = true;
                        }
                        break;
                    case 1:
                        Globals.form2.ButServiceDel.Visible = true;
                        Globals.form2.ButServiceChange.Visible = false;
                        Globals.form2.ButServiceInstall.Text = "ОК";
                        Globals.form2.ButServiceInstall.Enabled = false;
                        Globals.form2.LabelServiceStart.Text = "Запущена";
                        Globals.form2.LabelServiceStart.ForeColor = System.Drawing.Color.Green;
                        Globals.form2.LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                        Globals.form2.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (Globals.adrUpdNum == -1)
                            Globals.form2.groupBox2.Text = "Служба Windows (для обновлений)";
                        else
                            Globals.form2.groupBox2.Text = "Доступная версия службы обновлений " + Globals.nanitSvcVer;
                        break;
                    case 2:
                        Globals.form2.ButServiceDel.Visible = false;
                        Globals.form2.ButServiceChange.Visible = true;
                        Globals.form2.ButServiceInstall.Text = "Запустить";
                        Globals.form2.ButServiceInstall.Enabled = true;
                        Globals.form2.LabelServiceStart.Text = "Не запущена";
                        Globals.form2.LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                        Globals.form2.LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                        Globals.form2.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (Globals.adrUpdNum == -1)
                            Globals.form2.groupBox2.Text = "Служба Windows (для обновлений)";
                        else
                            Globals.form2.groupBox2.Text = "Доступная версия службы обновлений " + Globals.nanitSvcVer;
                        break;
                    case 3:
                        Globals.form2.ButServiceDel.Visible = false;
                        Globals.form2.ButServiceChange.Visible = true;
                        Globals.form2.ButServiceInstall.Text = "Обновить";
                        Globals.form2.LabelServiceStart.Text = "Запущена";
                        Globals.form2.LabelServiceStart.ForeColor = System.Drawing.Color.Green;
                        Globals.form2.LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                        Globals.form2.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (Globals.adrUpdNum == -1)
                        {
                            Globals.form2.groupBox2.Text = "Служба Windows (для обновлений)";
                            Globals.form2.ButServiceInstall.Enabled = false;
                        }
                        else
                        {
                            Globals.form2.groupBox2.Text = "Обновить службу обновлений до версии " + Globals.updVerAvi;
                            Globals.form2.ButServiceInstall.Enabled = true;
                        }
                        break;
                    case 4:
                        Globals.form2.ButServiceDel.Visible = false;
                        Globals.form2.ButServiceChange.Visible = true;
                        Globals.form2.LabelServiceStart.Text = "Не запущена";
                        Globals.form2.LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                        Globals.form2.LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                        Globals.form2.LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                        if (Globals.adrUpdNum == -1)
                        {
                            Globals.form2.ButServiceInstall.Text = "Запустить";
                            Globals.form2.groupBox2.Text = "Служба Windows (для обновлений)";
                            Globals.form2.ButServiceInstall.Enabled = true;
                        }
                        else
                        {
                            Globals.form2.ButServiceInstall.Text = "Обновить";
                            Globals.form2.groupBox2.Text = "Обновить службу обновлений до версии " + Globals.updVerAvi;
                            Globals.form2.ButServiceInstall.Enabled = true;
                        }
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)                  // Кнопка для тестирования всякой фигни
        {
            /*Process cmdInstall = new Process();
            cmdInstall.StartInfo.FileName = "cmd.exe";
            cmdInstall.StartInfo.Arguments = "/C " + @"ping google.com";
            cmdInstall.Start();
            cmdInstall.WaitForExit();*/
            ServiceWork.CheckUpdServer();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //MessageBox.Show(e.ProgressPercentage.ToString());
            progressBar1.Value = (int)((progressBar1.Maximum / 100.0) * e.ProgressPercentage);
            ServiceInit();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Globals.isOptOpen)
            {
                /*for (int i = 0; i <= 20; i++) // Проверка работы воркера
                {
                    backgroundWorker1.ReportProgress(i * 100 / 20);
                    Thread.Sleep(200);
                }*/
                if (workerByte)
                {
                    if (!Globals.work)
                    {
                        backgroundWorker1.ReportProgress(30);
                        workerByte = Functions.Revers(workerByte);
                    }
                    else
                        Thread.Sleep(200);
                }
                else
                {
                    if (Globals.work)
                    {
                        backgroundWorker1.ReportProgress(60);
                        workerByte = Functions.Revers(workerByte);
                    }
                    else
                        Thread.Sleep(200);
                }
            }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Canceled!");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
            else
            {
                if (Globals.isOptOpen)
                    MessageBox.Show("Что-то пошло не так. Я закончил работать");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread DeleteService = new Thread(new ThreadStart(ServiceWork.DeleteService));
            DeleteService.Name = "Delete on click";
            DeleteService.Start();
        }
    }
}