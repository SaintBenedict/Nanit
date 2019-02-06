using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

/*Форматировать фрагмент кода - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + F.
Форматировать весь код - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + D*/

namespace NaNiT
{
    static class Globals
    {
        public static bool DEBUGMODE = true;
        public static string appVersion = "1.5.0"; // ЕТО НЕ НАСТОЯЩИЕ ЦИФРЫ, НЕ ЕШЬ ПОДУМОЙ
        ///
        ///
        public static FormLogin form1 = null;
        public static FormOptions form2 = null;
        public static FormUpdater form3 = null;
        public static FormSoft form4 = null;
        ///
        ///
        public static string OSdate = Functions.GetOSDate();
        public static string nanitSvcVer = "0", updVerAvi = "1.0.0", version = Application.ProductVersion, nameFile;
        public static string optionsPasswordDefault = "478632", servIP = "127.0.0.1", myHostName;
        public static int servPort = 51782;
        public static string md5PortIp, md5Clients, optionsPasswordReg;
        public static bool RoleSecurity = false, RoleMessager = false, RoleOperate = false, RoleAdmin = false, RoleAgent = true;
        public static bool isAboutLoaded = false, isUpdOpen = false, isOptOpen = false, isSoftOpen = false;
        public static byte serviceStatus = 0; 
        public static int adrUpdNum = -1, itemsInList = 0, updateIn = 11, TimConnLock = 0;
        public static bool ServiceInitLock = false, InstallLock = false, UpdateLock = false, work = true;
        public static string[,] programs = null;
        public static string[] pathUpdate = new string[11];
        public static bool serverIsConnected = false;
        public static string userName, serverStatus;
    }

    class Functions
    {
        public static bool Revers(bool first)
        {
            if (first == true)
                return false;
            else
                return true;
        }

        public static int ChangeMesIn(int first)
        {
            if (first == 0)
            {
                return 1;
            }
            else
            {
                if (first == 1)
                {
                    return 2;
                }
                else
                {
                    if (first == 2)
                    {
                        return 3;
                    }
                    else
                    {
                        return 2;
                    }
                }
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

        public static void FirstRunOptionsLoad()
        {
            Program.notifyIcon = new NotifyIcon();
            Program.notifyIcon.Icon = Properties.Resources.net2;
            Program.notifyIcon.Visible = true;
            Program.notifyIcon.ContextMenuStrip = new ContextMenus().Create();
            Program.notifyIcon.Text = "Сетевой агент НИИ Телевидения";

            RegCheck();                       ///Проверка наличия настроек в реестре

            Thread t = Thread.CurrentThread;
            t.Name = "Main Program";
            
            Globals.myHostName = Dns.GetHostName();

            Globals.form4 = new FormSoft();
            Globals.userName = Globals.myHostName + Globals.OSdate;
        }

        public static string UrlCorrect(string url)
        {
            if (url == null)
                return null;
            string result = null;
            string urlTemp = null;
            int len = url.Length;
            if (len <= 4)
                return null;
            if (url.Substring(len - 1, 1) == @"/")
            {
                url = url.Substring(0, url.Length - 1);
                len = len - 1;
            }
            if (url.Substring(len - 6, 6) == @"/nanit")
            {
                url = url.Substring(0, url.Length - 6);
                len = len - 6;
            }
            if (len > 4)
            {
                string urlStart = url.Substring(0, 4);
                switch (urlStart)
                {
                    case "http":
                        if (url.Substring(0, 7) == @"http://")
                            result = url;
                        else
                        {
                            if (url.Substring(0, 8) == @"https://")
                                result = url;
                            else
                                result = null;
                        }
                        break;
                    case "file":
                        if (url.Substring(0, 8) == @"file:///")
                            result = url;
                        else
                            result = null;
                        break;
                    case "www.":
                        urlTemp = url.Substring(4, len - 4);
                        result = urlTemp.Insert(0, @"http://");
                        break;
                    default:
                        IPAddress address;
                        if (IPAddress.TryParse(url, out address))
                            result = url.Insert(0, @"file:///\\");
                        else
                        {
                            urlTemp = url.Substring(2, len - 2);
                            if (url.Substring(0, 2) == @"\\")
                                if (IPAddress.TryParse(urlTemp, out address))
                                    result = urlTemp.Insert(0, @"file:///\\");
                                else
                                    result = null;
                            else
                            {
                                if (url.Contains(@"."))
                                    result = url.Insert(0, @"http://");
                                else
                                    result = null;
                            }
                        }
                        break;
                }
            }
            else
                result = null;

            return result;
        }

        public static void RegCheck()
        {
            Globals.md5Clients = MD5Code(Globals.OSdate + Globals.RoleSecurity.ToString().ToLower() + Globals.RoleMessager.ToString().ToLower() + Globals.RoleOperate.ToString().ToLower() + Globals.RoleAdmin.ToString().ToLower() + Globals.RoleAgent.ToString().ToLower());
            Globals.md5PortIp = MD5Code(Globals.servPort + Globals.servIP + Globals.OSdate);
            Globals.optionsPasswordReg = MD5Code(Globals.optionsPasswordDefault + Globals.OSdate);
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey updateKey = regNanit.CreateSubKey("Update");

            Globals.servIP = CheckRegString("ip_server", regNanit, Globals.servIP);
            Globals.servPort = Convert.ToInt32(CheckRegString("port_server", regNanit, Globals.servPort.ToString()));
            Globals.md5PortIp = CheckRegString("validate_ip_port", regNanit, Globals.md5PortIp);
            Globals.md5Clients = CheckRegString("validate_clients", regNanit, Globals.md5Clients);
            Globals.RoleSecurity = CheckRegBool("RoleSecurity", regNanit, Globals.RoleSecurity);
            Globals.RoleMessager = CheckRegBool("RoleMessager", regNanit, Globals.RoleMessager);
            Globals.RoleOperate = CheckRegBool("RoleOperate", regNanit, Globals.RoleOperate);
            Globals.RoleAdmin = CheckRegBool("RoleAdmin", regNanit, Globals.RoleAdmin);
            Globals.RoleAgent = CheckRegBool("RoleAgent", regNanit, Globals.RoleAgent);
            Globals.optionsPasswordReg = CheckRegString("password", regNanit, Globals.optionsPasswordReg);
            Globals.nanitSvcVer = CheckRegString("nanitSvcVer", updateKey, Globals.nanitSvcVer);
            Globals.itemsInList = 0;
            for (int j = 0; j < 11; j++)
            {
                Globals.pathUpdate[j] = CheckRegString("path_update_" + j.ToString(), updateKey, "NULL");
                if (Globals.pathUpdate[j] != "NULL")
                {
                    Globals.pathUpdate[j] = UrlCorrect(Globals.pathUpdate[j]);
                    Globals.itemsInList++;
                }
                else
                    Globals.pathUpdate[j] = null;
            }
            Globals.updateIn = Globals.itemsInList;

            string CheckRegString(string toRegName, RegistryKey toDo, string variant)
            {
                string result = variant;
                if (toDo.GetValue(toRegName) != null)
                    result = toDo.GetValue(toRegName).ToString();
                else
                    toDo.SetValue(toRegName, variant);
                return result;
            }
            bool CheckRegBool(string toRegName, RegistryKey toDo, bool variant)
            {
                bool result = variant;
                if (toDo.GetValue(toRegName) != null)
                    result = toDo.GetValue(toRegName).Equals("true");
                else
                    toDo.SetValue(toRegName, variant.ToString().ToLower());
                return result;
            }

            regNanit.Close();
            updateKey.Close();

            if (Globals.md5PortIp != MD5Code(Globals.servPort + Globals.servIP + Globals.OSdate))
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
            if (Globals.md5Clients != MD5Code(Globals.OSdate + Globals.RoleSecurity.ToString().ToLower() + Globals.RoleMessager.ToString().ToLower() + Globals.RoleOperate.ToString().ToLower() + Globals.RoleAdmin.ToString().ToLower() + Globals.RoleAgent.ToString().ToLower()))
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
        }

        public static string GetOSDate()
        {
            string result = null;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + "Win32_OperatingSystem");
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["InstallDate"] != null)
                        result = obj["InstallDate"].ToString().Trim();
                }
            }
            return result.Substring(0, 14);
        }

        public static void RefreshOpions()
        {
            if (Globals.isOptOpen)
                Globals.work = Revers(Globals.work);
        }

        public static void AutoSelfInstall(bool check)
        {
            if (check)
            {
                string path = Path.GetPathRoot(Environment.SystemDirectory);
                string targetPath = path + @"Windows\services";
                string targetFileName = "nanit_" + Globals.version + ".exe";
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
                    Program.notifyIcon.Dispose();
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
        }
    }
}
