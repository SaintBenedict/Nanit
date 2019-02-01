using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading;
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
            int k = 0;
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp in scServices)
            {

                if (scTemp.ServiceName == "Nanit Updater")
                {
                    ServiceController sc = new ServiceController("Nanit Updater");
                    k = 1;
                    LabelServiceInstall.ForeColor = System.Drawing.Color.Green;
                    string path = Path.GetPathRoot(Environment.SystemDirectory);
                    string targetPath = path + @"Windows\services";
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(targetPath + @"\nanit-svc" + "_" + Globals.nanitSvcVer + @".exe");
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
                        ButServiceInstall.Enabled = true;
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
            if (k == 0)
                Globals.nanitSvcVer = "0";
            switch (Globals.serviceStatus)
            {
                case 0:
                    ButServiceDel.Visible = false;
                    ButServiceChange.Visible = true;
                    break;
                case 1:
                    ButServiceDel.Visible = true;
                    ButServiceChange.Visible = false;
                    break;
                case 2:
                    ButServiceDel.Visible = false;
                    ButServiceChange.Visible = true;
                    break;
                case 3:
                    ButServiceDel.Visible = false;
                    ButServiceChange.Visible = true;
                    break;
            }
            CheckUpdServer();
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey updateKey = regNanit.CreateSubKey("Update");
            updateKey.SetValue("nanitSvcVer", Globals.nanitSvcVer);
            regNanit.Close();
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
            { result = false; }
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
                        Globals.updVerAvi = Mass[1].Substring(0, 5);
                        if (Globals.nanitSvcVer == Globals.updVerAvi)
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
                                groupBox2.Text = "Служба Windows (для обновлений) " + Globals.updVerAvi;
                            }
                            else
                            {
                                Globals.serviceStatus = 3;
                                ButServiceInstall.Text = "Обновить";
                                ButServiceInstall.Enabled = true;
                                groupBox2.Text = "Обновить службу обновлений до версии" + Globals.updVerAvi;
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
            }
        }

        private void ButServiceInstall_Click(object sender, EventArgs e)
        {
            string remoteUri = Globals.pathUpdate[Globals.adrUpdNum] + "/nanit/";
            string fileName = "nanit-svc.exe", myStringWebResource = null;
            string fileName2 = "nanit-svc" + "_" + Globals.updVerAvi + @".exe";
            string fileName3 = "nanit-svc" + "_" + Globals.nanitSvcVer + @".exe";
            WebClient myWebClient = new WebClient();
            myStringWebResource = remoteUri + fileName;
            string path = Path.GetPathRoot(Environment.SystemDirectory);
            string sourcePath = Application.StartupPath;
            string targetPath = path + @"Windows\services";
            string downlFile = Path.Combine(sourcePath, fileName);
            string sourceFile = Path.Combine(sourcePath, fileName2);
            string targetFile = Path.Combine(targetPath, fileName2);
            string oldFile = Path.Combine(targetPath, fileName3);
            Directory.CreateDirectory(targetPath);
            string InstSvc = path + @"Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe ";

            switch (Globals.serviceStatus)
            {
                case 0:
                    int install = 1;
                    myWebClient.DownloadFile(myStringWebResource, fileName);
                    myWebClient.Dispose();
                    File.Delete(sourceFile);
                    File.Move(downlFile, sourceFile);
                    File.Copy(sourceFile, targetFile, true);
                    Globals.nanitSvcVer = Globals.updVerAvi;
                    File.Delete(sourceFile);
                    File.Delete(downlFile);
                    Process.Start("cmd.exe", "/C " + InstSvc + targetPath + @"\" + fileName2);
                    ServiceController[] scServices;
                    // Кусок необходимый для проверки корректной установки, чтобы выйти из цикла 
                    // и не прибегать к функции thread.sleep потому как на разных компах скорость разная будет
                    while (install > 0)
                    {
                        Thread.Sleep(100);
                        scServices = ServiceController.GetServices();
                        foreach (ServiceController scTemp in scServices)
                        {
                            install++;
                            if (scTemp.ServiceName == "Nanit Updater")
                            {
                                install = 0;
                            }
                        }
                    }
                    //Конец куска
                    ServiceInit();
                    break;

                case 2:
                    int run = 1;
                    scServices = ServiceController.GetServices();
                    foreach (ServiceController scTemp in scServices)
                    {
                        if (scTemp.ServiceName == "Nanit Updater")
                        {
                            ServiceController sc = new ServiceController("Nanit Updater");
                            sc.Start();
                            // Кусок необходимый для проверки корректного запуска
                            while (run > 0)
                            {
                                Thread.Sleep(100);
                                run++;
                                if (sc.Status == ServiceControllerStatus.Running)
                                {
                                    run = 0;
                                }
                            }
                            sc.Dispose();
                            //Конец куска
                            ServiceInit();
                        }
                    }
                    break;

                case 3:
                    UpdateService();
                    break;
            }
        }

        private void ButServiceDel_Click(object sender, EventArgs e)
        {
            DeleteService();
        }

        public void DeleteService()
        {
            string remoteUri = Globals.pathUpdate[Globals.adrUpdNum] + "/nanit/";
            string fileName = "nanit-svc.exe", myStringWebResource = null;
            string fileName2 = "nanit-svc" + "_" + Globals.updVerAvi + @".exe";
            string fileName3 = "nanit-svc" + "_" + Globals.nanitSvcVer + @".exe";
            string fileName4 = "nanit-svc" + "_" + Globals.nanitSvcVer + @".InstallLog";
            myStringWebResource = remoteUri + fileName;
            string path = Path.GetPathRoot(Environment.SystemDirectory);
            string targetPath = path + @"Windows\services";
            string targetFile = Path.Combine(targetPath, fileName);
            string destFile = Path.Combine(targetPath, fileName2);
            string oldFile = Path.Combine(targetPath, fileName3);
            string logFile = Path.Combine(targetPath, fileName4);
            string InstSvc = path + @"Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe ";
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp in scServices)
            {

                if (scTemp.ServiceName == "Nanit Updater")
                {
                    ServiceController sc = new ServiceController("Nanit Updater");
                    if (sc.Status == ServiceControllerStatus.Running)
                        sc.Stop();
                    Thread.Sleep(100);
                // Кусок необходимый для проверки корректной остановки
                Running:
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        Thread.Sleep(100);
                        goto Running;
                    }
                    sc.Dispose();
                }
                //Конец куска
                ServiceInit();
                goto Deleting;
            }
            goto End;
        Deleting:
            Process.Start("cmd.exe", "/C " + InstSvc + @"-u " + targetPath + @"\" + fileName3);
        // Кусок необходимый для проверки корректного удаления
        Testc:
            Thread.Sleep(100);
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp5 in scServices)
            {
                if (scTemp5.ServiceName == "Nanit Updater")
                {
                    goto Testc;
                }
            }
            //Конец куска
            ServiceInit();
        End:
            if (Globals.updVerAvi == "1.0.0")
                Globals.nanitSvcVer = "0";
            ServiceInit();
            Thread.Sleep(600);
        DelFuckDel:
            try
            {
                File.Delete(oldFile);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(100);
                goto DelFuckDel;
            }
            File.Delete(logFile);
        }

        public void UpdateService()
        {
            DeleteService();
            string remoteUri = Globals.pathUpdate[Globals.adrUpdNum] + " / nanit/";
            string fileName = "nanit-svc.exe", myStringWebResource = null;
            string fileName2 = "nanit-svc" + "_" + Globals.updVerAvi + @".exe";
            string fileName3 = "nanit-svc" + "_" + Globals.nanitSvcVer + @".exe";
            WebClient myWebClient = new WebClient();
            myStringWebResource = remoteUri + fileName;
            string path = Path.GetPathRoot(Environment.SystemDirectory);
            string targetPath = path + @"Windows\services";
            string sourcePath = Application.StartupPath;
            string downlFile = Path.Combine(sourcePath, fileName);
            string sourceFile = Path.Combine(sourcePath, fileName2);
            string targetFile = Path.Combine(targetPath, fileName2);
            string oldFile = Path.Combine(targetPath, fileName3);
            Directory.CreateDirectory(targetPath);
            string InstSvc = path + @"Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe ";
            myWebClient.DownloadFile(myStringWebResource, fileName);
            myWebClient.Dispose();
            File.Delete(sourceFile);
            File.Move(downlFile, sourceFile);
            File.Copy(sourceFile, targetFile, true);
            File.Delete(sourceFile);
            File.Delete(downlFile);
            Process.Start("cmd.exe", "/C " + InstSvc + targetPath + @"\" + fileName2);
            // Кусок необходимый для проверки корректной повторной установки
            int update2 = 1;
            ServiceController[] scServices;
            while (update2 > 0)
            {
                Thread.Sleep(100);
                scServices = ServiceController.GetServices();
                foreach (ServiceController scTemp3 in scServices)
                {
                    update2++;
                    if (scTemp3.ServiceName == "Nanit Updater")
                    {
                        update2 = 0;
                    }
                }
            }
            //Конец куска
            Globals.nanitSvcVer = Globals.updVerAvi;
            ServiceInit();
            ServiceController sc = new ServiceController("Nanit Updater");
            sc.Start();
            // Кусок необходимый для проверки корректного запуска
            int run2 = 1;
            while (run2 > 0)
            {
                Thread.Sleep(100);
                run2++;
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    run2 = 0;
                }
            }
            sc.Dispose();
            //Конец куска
            ServiceInit();
        }
    }
}