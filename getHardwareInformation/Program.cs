using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

/*Форматировать фрагмент кода - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + F.
Форматировать весь код - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + D*/

namespace NaNiT
{
    static class Globals
    {
        public static bool DEBUGMODE = true;
        public static string appVersion = "1.3.4"; // ЕТО НЕ НАСТОЯЩИЕ ЦИФРЫ, НЕ ЕШЬ ПОДУМОЙ
        public static string nanitSvcVer = "0";
        public static string version = Application.ProductVersion; /// Изменять в AssemblyInfo.cs версию, чтобы была такой же как ^^ app.Version
        public static string[] pathUpdate = new string[11];
        public static string nameFile = "";
        public static string optionsPasswordDefault = "478632";
        public static string optionsPasswordReg = "";
        public static string servIP = "127.0.0.1";
        public static string servPort = "51782";
        public static string md5PortIp = Program.MD5Code(servPort + servIP);
        public static FormLogin form1 = null;
        public static FormOptions form2 = null;
        public static FormUpdater form3 = null;
        public static bool RoleSecurity = false;
        public static bool RoleMessager = false;
        public static bool RoleOperate = false;
        public static bool RoleAdmin = false;
        public static bool RoleAgent = true;
        public static string md5Clients = Program.MD5Code(RoleSecurity.ToString().ToLower() + RoleMessager.ToString().ToLower() + RoleOperate.ToString().ToLower() + RoleAdmin.ToString().ToLower() + RoleAgent.ToString().ToLower());
        public static bool isAboutLoaded = false;
        public static bool isUpdOpen = false;
        public static string exMessage = null;
        public static byte serviceStatus = 0; // Проверка службы обновлений. 0 не установлена и не запущена. 1 установлена и запущена. 2 установлена не запущена. 3 обновление.
        public static sbyte adrUpdNum = -1;
        public static string updVerAvi = "1.0.0"; // Стринг для версии файла доступного для обновления
    }
    class Program
    {
        public static NotifyIcon notifyIcon = null;
        public static string[,,] dataResult = new string[50, 10, 2];

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.net2;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new ContextMenus().Create();
            notifyIcon.Text = "Сетевой агент НИИ Телевидения";
            Globals.pathUpdate[0] = @"http://mail.niitv.ru";
            Globals.appVersion = Globals.version;

            ///Проверка наличия настроек в реестре
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey updateKey = regNanit.CreateSubKey("Update");
            if (regNanit.GetValue("install") == null)
            {
                regNanit.SetValue("install", "1");
                updateKey.SetValue("install", Globals.appVersion);
                regNanit.SetValue("ip_server", Globals.servIP);
                regNanit.SetValue("port_server", Globals.servPort);
                regNanit.SetValue("validate_ip_port", Globals.md5PortIp);
                regNanit.SetValue("password", MD5Code(Globals.optionsPasswordDefault));
                Globals.optionsPasswordReg = MD5Code(Globals.optionsPasswordDefault);
                regNanit.SetValue("RoleSecurity", Globals.RoleSecurity.ToString().ToLower());
                regNanit.SetValue("RoleMessager", Globals.RoleMessager.ToString().ToLower());
                regNanit.SetValue("RoleOperate", Globals.RoleOperate.ToString().ToLower());
                regNanit.SetValue("RoleAdmin", Globals.RoleAdmin.ToString().ToLower());
                regNanit.SetValue("RoleAgent", Globals.RoleAgent.ToString().ToLower());
                regNanit.SetValue("validate_clients", Globals.md5Clients);
                updateKey.SetValue("nanitSvcVer", Globals.nanitSvcVer);
                updateKey.SetValue("path_update_0", Globals.pathUpdate[0]);
                for (byte j = 1; j < 11; j++)
                {
                    updateKey.SetValue("path_update_" + j.ToString(), "NULL");
                }
                regNanit.Close();
                updateKey.Close();
            }
            else
            {
                Globals.servIP = regNanit.GetValue("ip_server").ToString();
                Globals.servPort = regNanit.GetValue("port_server").ToString();
                Globals.md5PortIp = regNanit.GetValue("validate_ip_port").ToString();
                Globals.md5Clients = regNanit.GetValue("validate_clients").ToString();
                Globals.RoleSecurity = regNanit.GetValue("RoleSecurity").Equals("true");
                Globals.RoleMessager = regNanit.GetValue("RoleMessager").Equals("true");
                Globals.RoleOperate = regNanit.GetValue("RoleOperate").Equals("true");
                Globals.RoleAdmin = regNanit.GetValue("RoleAdmin").Equals("true");
                Globals.RoleAgent = regNanit.GetValue("RoleAgent").Equals("true");
                if (updateKey.GetValue("nanitSvcVer") != null)
                    Globals.nanitSvcVer = updateKey.GetValue("nanitSvcVer").ToString();
                else
                    updateKey.SetValue("nanitSvcVer", Globals.nanitSvcVer);
                updateKey.SetValue("path_update_0", Globals.pathUpdate[0]);
                for (byte j = 1; j < 11; j++)
                {
                    if (updateKey.GetValue("path_update_" + j.ToString()).ToString() != "NULL")
                        Globals.pathUpdate[j] = updateKey.GetValue("path_update_" + j.ToString()).ToString();
                    else
                        Globals.pathUpdate[j] = null;
                }

                if (Globals.md5PortIp != Program.MD5Code(Globals.servPort + Globals.servIP))
                {
                    const string message = "Указаны неверные настройки. Отправлено сообщение администратору.";
                    const string caption = "";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    if (result == DialogResult.OK)
                    {
                        Program.notifyIcon.Dispose();
                        Application.Exit();
                        Process.GetCurrentProcess().Kill();
                    }
                }
                if (Globals.md5Clients != Program.MD5Code(Globals.RoleSecurity.ToString().ToLower() + Globals.RoleMessager.ToString().ToLower() + Globals.RoleOperate.ToString().ToLower() + Globals.RoleAdmin.ToString().ToLower() + Globals.RoleAgent.ToString().ToLower()))
                {
                    const string message = "Указаны неверные политики. Отправлено сообщение администратору.";
                    const string caption = "";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    if (result == DialogResult.OK)
                    {
                        Program.notifyIcon.Dispose();
                        Application.Exit();
                        Process.GetCurrentProcess().Kill();
                    }
                }

                Globals.optionsPasswordReg = regNanit.GetValue("password").ToString();

                regNanit.Close();
                updateKey.Close();
            }
            ///InfoGet(); /* Кусок кода для версии со сбором данных и не более того */

            if (Globals.DEBUGMODE == false)
            {
                string path = Path.GetPathRoot(Environment.SystemDirectory);
                string targetPath = path + @"Windows\services";
                string targetFileName = "nanit_" + Globals.appVersion + ".exe";
                string sourceFile = Application.ExecutablePath;
                string myName = Path.GetFileName(sourceFile);
                string targetFile = Path.Combine(targetPath, targetFileName);
                Process currentProcess = Process.GetCurrentProcess();
                string[] dirs2 = Directory.GetFiles(targetPath, "nanit_*");
                byte fuck = 0;
                if (sourceFile != targetFile)
                {
                    foreach (string dir in dirs2)
                    {
                        string tempDir = dir.Substring(0, dir.Length - 4);
                        string processToKill = Path.GetFileName(tempDir);
                        Process[] AllNanit = Process.GetProcessesByName(processToKill);
                        foreach (Process tempProc in AllNanit)
                        {
                            if (tempProc.Id != currentProcess.Id)
                                tempProc.Kill();
                        }
                        AllNanit = null;
                        fuck = 0;
                    DelThisPlz:
                        try
                        {
                            File.Delete(dir);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            if (fuck == 100)
                            {
                                //MessageBox.Show("FUCK");
                                continue;
                            }
                            fuck++;
                            goto DelThisPlz;
                        }
                    }
                    dirs2 = null;
                    File.Copy(sourceFile, targetFile, true);
                    Process.Start(targetFile);
                    notifyIcon.Dispose();
                    Application.Exit();
                    currentProcess.Kill();
                }
                else
                {
                    foreach (string dir in dirs2)
                    {
                        string tempDir = dir.Substring(0, dir.Length - 4);
                        string processToKill = Path.GetFileName(tempDir);
                        Process[] AllNanit = Process.GetProcessesByName(processToKill);
                        foreach (Process tempProc in AllNanit)
                        {
                            if (tempProc.Id != currentProcess.Id)
                                tempProc.Kill();
                        }
                        AllNanit = null;
                        fuck = 0;
                        if (dir != sourceFile)
                        {
                        DelThisPlz2:
                            try
                            {
                                File.Delete(dir);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                if (fuck == 100)
                                {
                                    //MessageBox.Show("FUCK");
                                    continue;
                                }
                                fuck++;
                                goto DelThisPlz2;
                            }
                        }
                    }
                    dirs2 = null;
                }
            }
            TimerCallback tm1 = new TimerCallback(CheckServiceUpdate);
            Timer timer1 = new Timer(tm1, 0, 10000, 20000);
            Application.Run();
        }

        public static void InfoGet()
        {
            DeleteExist(); /// Удаление имеющегося файла информации

            string myHost = Dns.GetHostName();
            string myIP = null;

            OutputSimple("Данные об Автоматизированном Рабочем Месте");

            OutputIniBlock("Данные о сетях");
            OutputSimple("• Имя компьютера: " + myHost);

            for (byte i = 0; i <= Dns.GetHostEntry(myHost).AddressList.Length - 1; i++)
            {
                if (Dns.GetHostEntry(myHost).AddressList[i].IsIPv6LinkLocal == false)
                {
                    myIP = Dns.GetHostEntry(myHost).AddressList[i].ToString();
                    OutputSimple(myIP);
                }
            }

            OutputIniBlock("Операционная система");
            OutputResult("• Операционная система:", GetHardwareInfo("Win32_OperatingSystem", "Caption"), 1);
            OutputResult("Версия:", GetHardwareInfo("Win32_OperatingSystem", "Version"), 2);
            OutputResult("Сервис-пак:", GetHardwareInfo("Win32_OperatingSystem", "ServicePackMajorVersion"), 3);
            OutputResult("Серийный номер:", GetHardwareInfo("Win32_OperatingSystem", "SerialNumber"), 4);
            OutputResult("Дата установки:", GetHardwareInfo("Win32_OperatingSystem", "InstallDate"), 5);
            OutputSimple("");
            OutputResult("Рабочая группа:", GetHardwareInfo("Win32_ComputerSystem", "Domain"), 6);

            OutputIniBlock("Системные устройства");
            OutputResult("• Процессор:", GetHardwareInfo("Win32_Processor", "Name"), 7);
            OutputResult("Производитель:", GetHardwareInfo("Win32_Processor", "Manufacturer"), 8);
            OutputResult("Описание:", GetHardwareInfo("Win32_Processor", "Description"), 9);
            OutputSimple("");
            OutputResult("• Материнская плата:", GetHardwareInfo("Win32_BaseBoard", "Product"), 10);
            OutputResult("Производитель:", GetHardwareInfo("Win32_BaseBoard", "Manufacturer"), 11);
            OutputResult("Описание:", GetHardwareInfo("Win32_BaseBoard", "Description"), 12);
            OutputSimple("");
            OutputResult("• Оперативная память:", GetHardwareInfo("Win32_PhysicalMemory", "Manufacturer"), 13);
            OutputResult("Серийный номер:", GetHardwareInfo("Win32_PhysicalMemory", "SerialNumber"), 14);
            OutputResult("Объём (в байтах):", GetHardwareInfo("Win32_PhysicalMemory", "Capacity"), 15);
            OutputSimple("");
            OutputResult("• Видеокарта:", GetHardwareInfo("Win32_VideoController", "Name"), 16);
            OutputResult("Видеопроцессор:", GetHardwareInfo("Win32_VideoController", "VideoProcessor"), 17);
            OutputResult("Версия драйвера:", GetHardwareInfo("Win32_VideoController", "DriverVersion"), 18);
            OutputResult("Объем памяти (в байтах):", GetHardwareInfo("Win32_VideoController", "AdapterRAM"), 19);
            OutputSimple("");
            OutputResult("• Жесткий диск:", GetHardwareInfo("Win32_DiskDrive", "Caption"), 20);
            OutputResult("Интерфейс:", GetHardwareInfo("Win32_DiskDrive", "InterfaceType"), 21);
            OutputResult("Объем (в байтах):", GetHardwareInfo("Win32_DiskDrive", "Size"), 22);

            OutputIniBlock("Периферийные устройства");
            OutputResult("• Клавиатура:", GetHardwareInfo("Win32_Keyboard", "Description"), 23);
            OutputSimple("");
            OutputResult("• Мышь:", GetHardwareInfo("Win32_PointingDevice", "Description"), 24);
            OutputResult("Производитель:", GetHardwareInfo("Win32_PointingDevice", "Manufacturer"), 25);
            OutputSimple("");
            OutputResult("• Дисковод:", GetHardwareInfo("Win32_CDROMDrive", "Name"), 26);
            OutputResult("Буква привода:", GetHardwareInfo("Win32_CDROMDrive", "Drive"), 27);
            OutputSimple("");
            OutputResult("• Монитор:", GetHardwareInfo("Win32_DesktopMonitor", "DeviceID"), 28);

            Process.Start(Globals.nameFile);

            ///Application.Exit(); /* Кусок кода для версии со сбором данных и не более того */
            ///Process.GetCurrentProcess().Kill(); /*Кусок кода для версии со сбором данных и не более того */


            /*
            ManagementObjectSearcher searcher_soft =
            new ManagementObjectSearcher("root\\CIMV2",
           "SELECT * FROM Win32_Product");

            foreach (ManagementObject queryObj in searcher_soft.Get())
            {
                Console.WriteLine("<soft> Caption: {0} ; InstallDate: {1}</soft>",
                                  queryObj["Caption"], queryObj["InstallDate"]);
            }*/
            ///Console.ReadLine();
        }

        private static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + WIN32_Class);
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj[ClassItemField] != null)
                        result.Add(obj[ClassItemField].ToString().Trim());
                    else
                        result.Add("Ошибка при получении параметра (NULL)");
                }
            }
            return result;
        }

        private static void OutputResult(string info, List<string> result, byte pNumber)
        {
            using (StreamWriter file = new StreamWriter(Globals.nameFile, true))
            {
                if (info.Length > 0)
                    file.WriteLine(info);

                if (result.Count > 0)
                {
                    for (byte i = 0; i < result.Count; ++i)
                    {
                        file.WriteLine(result[i]);
                        dataResult[pNumber, i, 0] = result[i];
                        dataResult[pNumber, i, 1] = result.Count.ToString();
                        ///file.WriteLine(" DEBUG *номер запроса* "+ pNumber + " DEBUG *значение* " + dataResult[pNumber, i, 0] + " DEBUG *номер значения в листе* " + (i + 1) + " DEBUG *число значений в листе* " + dataResult[pNumber, i, 1]);
                    }
                }
            }
        }

        private static void OutputSimple(string name)
        {
            using (StreamWriter file = new StreamWriter(Globals.nameFile, true))
            {
                file.WriteLine(name);
            }
        }

        private static void OutputIniBlock(string name)
        {
            using (StreamWriter file = new StreamWriter(Globals.nameFile, true))
            {
                file.WriteLine("");
                file.WriteLine("*****[" + name + "]*****");
                file.WriteLine("");
            }
        }

        private static void DeleteExist()
        {
            string date2 = DateTime.Now.ToString();
            date2 = date2.Replace(":", "-");
            Globals.nameFile = @date2 + @".txt";
            Globals.nameFile = @".\" + Globals.nameFile;
            if (File.Exists(Globals.nameFile))
            {
                File.Delete(Globals.nameFile);
            }
        }

        public static string MD5Code(string getCode)
        {
            byte[] hash = Encoding.ASCII.GetBytes(getCode);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashenc = md5.ComputeHash(hash);
            string result = "";
            foreach (var b in hashenc)
            {
                result += b.ToString("x2");
            }
            return result;
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
                myWebClient.Dispose();
            }
            catch (WebException)
            { result = false; }
            return result;
        }

        public static void ServiceInit()
        {
            CheckUpdServer();
            byte k = 0;
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp in scServices)
            {

                if (scTemp.ServiceName == "Nanit Updater")
                {
                    ServiceController sc = new ServiceController("Nanit Updater");
                    k = 1;
                    string path = Path.GetPathRoot(Environment.SystemDirectory);
                    string targetPath = path + @"Windows\services";
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(targetPath + @"\nanit-svc" + "_" + Globals.nanitSvcVer + @".exe");
                    string str = myFileVersionInfo.FileVersion.Substring(0, 5);
                    Globals.nanitSvcVer = str;
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        Globals.serviceStatus = 1;
                        if (Globals.nanitSvcVer != Globals.updVerAvi)
                            Globals.serviceStatus = 3;
                    }
                    else
                    {
                        Globals.serviceStatus = 2;
                        if (Globals.nanitSvcVer != Globals.updVerAvi)
                            Globals.serviceStatus = 4;
                    }
                    File.Delete(@"InstallUtil.InstallLog");
                }
                else
                {
                    Globals.serviceStatus = 0;
                }
            }
            if (k == 0)
                Globals.nanitSvcVer = "0";
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey updateKey = regNanit.CreateSubKey("Update");
            updateKey.SetValue("nanitSvcVer", Globals.nanitSvcVer);
            regNanit.Close();
            updateKey.Close();
        }

        public static void CheckUpdServer()
        {
            for (sbyte j = 0; j < 11; j++)
            {
                Globals.adrUpdNum = -1;
                if (Globals.pathUpdate[j] == null)
                    continue;
                if (Globals.pathUpdate[j].Length < 10)
                    continue;
                if (FileExists(Globals.pathUpdate[j] + "/nanit/") == true)
                {
                    string[] Mass = File.ReadAllLines(@"version.txt", Encoding.Default);
                    File.Delete(@"version.txt");
                    if (Mass[0] == "version-nanit-service")
                    {
                        Globals.adrUpdNum = j;
                        Globals.updVerAvi = Mass[1].Substring(0, 5);
                        Mass = null;
                        j = 11;
                    }
                    else
                    {
                        Globals.adrUpdNum = -1;
                        continue;
                    }
                }
            }
        }

        public static void InstallService()
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
                    myWebClient.DownloadFile(myStringWebResource, fileName);
                    myWebClient.Dispose();
                    File.Delete(sourceFile);
                    File.Move(downlFile, sourceFile);
                    File.Copy(sourceFile, targetFile, true);
                    Globals.nanitSvcVer = Globals.updVerAvi;
                    File.Delete(sourceFile);
                    File.Delete(downlFile);
                    Process cmdInstall = new Process();
                    cmdInstall.StartInfo.FileName = "cmd.exe";
                    cmdInstall.StartInfo.Arguments = "/C " + InstSvc + targetPath + @"\" + fileName2;
                    cmdInstall.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cmdInstall.Start();
                    cmdInstall.WaitForExit();
                    cmdInstall.Dispose();
                    ServiceController[] scServices;
                    // Кусок необходимый для проверки корректной установки, чтобы выйти из цикла 
                    // и не прибегать к функции thread.sleep потому как на разных компах скорость разная будет
                    byte install = 1;
                    while (install > 0)
                    {
                        scServices = ServiceController.GetServices();
                        foreach (ServiceController scTemp in scServices)
                        {
                            install++;
                            if (scTemp.ServiceName == "Nanit Updater")
                            {
                                install = 0;
                                goto case 2;
                            }
                        }
                    }
                    goto case 2;
                //Конец куска

                case 2:
                    scServices = ServiceController.GetServices();
                    foreach (ServiceController scTemp in scServices)
                    {
                        if (scTemp.ServiceName == "Nanit Updater")
                        {
                            ServiceController sc = new ServiceController("Nanit Updater");
                            sc.Start();
                        // Кусок необходимый для проверки корректного запуска
                        CheckRunning:
                            Thread.Sleep(100);
                            if (sc.Status == ServiceControllerStatus.Running)
                            {
                                sc.Dispose();
                                goto StopStarting;
                            }
                            goto CheckRunning;
                        }
                    }
                StopStarting:
                    Thread.Sleep(200);
                    ServiceInit();
                    break;

                case 3:
                    UpdateService();
                    break;
                case 4:
                    if (Globals.adrUpdNum == -1)
                        goto case 2;
                    else
                        UpdateService();
                    break;
            }
        }

        public static void DeleteService()
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
                    ServiceController sc5 = new ServiceController("Nanit Updater");
                    if (sc5.Status == ServiceControllerStatus.Running)
                        sc5.Stop();
                    Thread.Sleep(100);
                    sc5.Dispose();
                // Кусок необходимый для проверки корректной остановки
                Running:
                    ServiceController sc4 = new ServiceController("Nanit Updater");
                    if (sc4.Status == ServiceControllerStatus.Running)
                    {
                        Thread.Sleep(100);
                        sc4.Dispose();
                        goto Running;
                    }
                    //Конец куска
                    sc5.Dispose();
                    ServiceInit();
                    goto Deleting;
                }
            }
            goto End;
        Deleting:
            Process cmdInstall = new Process();
            cmdInstall.StartInfo.FileName = "cmd.exe";
            cmdInstall.StartInfo.Arguments = "/C " + InstSvc + @"-u " + targetPath + @"\" + fileName3;
            cmdInstall.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmdInstall.Start();
            cmdInstall.WaitForExit();
            cmdInstall.Dispose();
        // Кусок необходимый для проверки корректного удаления
        Testc:
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp5 in scServices)
            {
                if (scTemp5.ServiceName == "Nanit Updater")
                {
                    Thread.Sleep(100);
                    goto Testc;
                }
            }
            //Конец куска
            ServiceInit();
        End:
            if (Globals.updVerAvi == "1.0.0")
                Globals.nanitSvcVer = "0";
            ServiceInit();
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
            File.Delete(@"InstallUtil.InstallLog");
        }

        public static void UpdateService()
        {
            DeleteService();
            string remoteUri = Globals.pathUpdate[Globals.adrUpdNum] + "/nanit/";
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
            Process cmdInstall = new Process();
            cmdInstall.StartInfo.FileName = "cmd.exe";
            cmdInstall.StartInfo.Arguments = "/C " + InstSvc + targetPath + @"\" + fileName2;
            cmdInstall.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmdInstall.Start();
            cmdInstall.WaitForExit();
            cmdInstall.Dispose();
            // Кусок необходимый для проверки корректной повторной установки
            ServiceController[] scServices;
            byte update2 = 1;
            while (update2 > 0)
            {
                scServices = ServiceController.GetServices();
                foreach (ServiceController scTemp3 in scServices)
                {
                    update2++;
                    if (scTemp3.ServiceName == "Nanit Updater")
                    {
                        update2 = 0;
                        goto UpdateEnd;
                    }
                }
            }
        UpdateEnd:
            Globals.nanitSvcVer = Globals.updVerAvi;
            ServiceController sc = new ServiceController("Nanit Updater");
            sc.Start();
        // Кусок необходимый для проверки корректного запуска
        CheckRunning2:
            Thread.Sleep(100);
            if (sc.Status == ServiceControllerStatus.Running)
            {
                sc.Dispose();
                goto StopStarting2;
            }
            goto CheckRunning2;

        StopStarting2:
            Thread.Sleep(200);
            ServiceInit();
        }

        public static void CheckServiceUpdate(object obj)
        {
            ServiceInit();
            if (Globals.adrUpdNum != -1)
                if (Globals.nanitSvcVer != Globals.updVerAvi)
                    UpdateService();
        }
    }
}