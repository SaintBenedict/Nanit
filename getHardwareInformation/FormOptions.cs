using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.ServiceProcess;
using System.Net;
using System.IO;


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

            ServiceInit();
            CheckUpdServer();
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

        public void ServiceInit()
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp in scServices)
            {

                if (scTemp.ServiceName == "Nanit Updater")
                {
                    ServiceController sc = new ServiceController("Nanit Updater");
                    
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(Environment.SystemDirectory + @"\nanit-svc.exe");
                    string str = myFileVersionInfo.FileVersion.Substring(0, 5);
                    Globals.nanitSvcVer = str;
                    groupBox2.Text = "Версия службы обновлений " + Globals.nanitSvcVer;
                    LabelServiceInstall.Text = "Установлена (" + Globals.nanitSvcVer + ")";
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        LabelServiceStart.Text = "Запущена";
                        LabelServiceStart.ForeColor = System.Drawing.Color.Green;
                        Globals.serviceStatus = 1;
                        ButServiceInstall.Text = "Обновить";
                        ButServiceInstall.Enabled = false;
                    }
                    else
                    {
                        LabelServiceStart.Text = "Не запущена";
                        LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                        Globals.serviceStatus = 2;
                        ButServiceInstall.Text = "Запустить";
                        ButServiceInstall.Enabled = false;
                    }
                }
                else
                {
                    LabelServiceInstall.Text = "Не установлена";
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Red;
                    LabelServiceStart.Text = "Не запущена";
                    LabelServiceStart.ForeColor = System.Drawing.Color.Red;
                    Globals.serviceStatus = 0;
                    ButServiceInstall.Text = "Установить";
                    groupBox2.Text = "Служба Windows (для обновлений)";
                }
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

        public static bool FileExists(string url)
        {
            bool result = true;
            try
            {
                string remoteUri = url;
                string fileName = "version.txt", myStringWebResource = null;
                WebClient myWebClient = new WebClient();
                myStringWebResource = remoteUri + fileName;
                myWebClient.DownloadFile(myStringWebResource, fileName);
            }
            catch (WebException)
            { result = false;}
            return result;
        }

        public void CheckUpdServer()
        {
            for (int j = 0; j < 11; j++)
            {
                groupBox2.Text = "Служба Windows (для обновлений)";
                Globals.adrUpdNum = -1;
                if (Globals.pathUpdate[j] == null)
                    continue;
                if (Globals.pathUpdate[j].Length < 10)
                    continue;
                if (FileExists(Globals.pathUpdate[j] + "/nanit/") == true)
                {
                    string[] Mass = File.ReadAllLines(@"version.txt", System.Text.Encoding.Default);
                    if (Mass[0] == "version-nanit-service")
                    {
                        ButServiceInstall.Enabled = true;
                        Globals.adrUpdNum = j;
                        string str = Mass[1].Substring(0, 5);
                        if (Globals.nanitSvcVer == str)
                        {
                            if (Globals.serviceStatus == 1)
                            {
                                ButServiceInstall.Text = "ОК";
                                ButServiceInstall.Enabled = false;
                            }
                            if (Globals.serviceStatus == 2)
                            {
                                ButServiceInstall.Text = "Запустить";
                                ButServiceInstall.Enabled = true;
                            }
                        }
                        else
                        {
                            if (Globals.serviceStatus == 0)
                            {
                                ButServiceInstall.Text = "Установить";
                                ButServiceInstall.Enabled = true;
                                groupBox2.Text = "Служба Windows (для обновлений) " + str;
                            }
                            else
                            {
                                Globals.serviceStatus = 3;
                                ButServiceInstall.Text = "Обновить";
                                ButServiceInstall.Enabled = true;
                                groupBox2.Text = "Обновить службу обновлений до версии" + str;
                            }
                        }
                        j = 11;
                    }
                    else
                    {
                        ButServiceInstall.Enabled = false;
                        Globals.adrUpdNum = -1;
                        groupBox2.Text = "Служба Windows (для обновлений)";
                        continue;
                    }
                }
                else
                {
                        ButServiceInstall.Enabled = false;
                        groupBox2.Text = "Служба Windows (для обновлений)";
                }
            }
                
        }

        private void ButServiceInstall_Click(object sender, EventArgs e)
        {
            string remoteUri = Globals.pathUpdate[Globals.adrUpdNum] + "/nanit/";
            string fileName = "nanit-svc.exe", myStringWebResource = null;
            WebClient myWebClient = new WebClient();
            myStringWebResource = remoteUri + fileName;
            string path = Path.GetPathRoot(Environment.SystemDirectory);
            string sourcePath = Application.StartupPath;
            string targetPath = path + @"Windows\services";
            string sourceFile = Path.Combine(sourcePath, fileName);
            string destFile = Path.Combine(targetPath, fileName);
            string InstSvc = path + @"Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe ";

            switch (Globals.serviceStatus)
            {
                case 0:
                    myWebClient.DownloadFile(myStringWebResource, fileName);
                    File.Copy(sourceFile, destFile, true);
                    File.Delete(sourceFile);
                    Process.Start("cmd.exe", "/C " + InstSvc + targetPath + @"\nanit-svc.exe");
                    Thread.Sleep(2000);
                    CheckUpdServer();
                    ServiceInit();
                    break;

                case 2:
                    ServiceController[] scServices;
                    scServices = ServiceController.GetServices();
                    foreach (ServiceController scTemp in scServices)
                    {

                        if (scTemp.ServiceName == "Nanit Updater")
                        {
                            ServiceController sc = new ServiceController("Nanit Updater");
                            sc.Start();
                            Thread.Sleep(2000);
                            CheckUpdServer();
                            ServiceInit();
                        }
                    }
                    break;

                case 3:
                    scServices = ServiceController.GetServices();
                    foreach (ServiceController scTemp in scServices)
                    {

                        if (scTemp.ServiceName == "Nanit Updater")
                        {
                            ServiceController sc = new ServiceController("Nanit Updater");
                            if (sc.Status == ServiceControllerStatus.Running)
                                sc.Stop();
                            Process.Start("cmd.exe", "/C " + InstSvc + @"-u " + targetPath + @"\nanit-svc.exe");
                            myWebClient.DownloadFile(myStringWebResource, fileName);
                            Globals.serviceStatus = 0;
                            Thread.Sleep(2000);
                            CheckUpdServer();
                            ServiceInit();
                        }
                    }
                    break;
            }
        }
    }
}
