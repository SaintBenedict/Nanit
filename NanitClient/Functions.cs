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
using static NaNiT.GlobalVariable;
using static NaNiT.GlobalFunctions;

/*Форматировать фрагмент кода - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + F.
Форматировать весь код - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + D*/

namespace NaNiT
{


    class Functions
    {
        public static void FirstRunOptionsLoad()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Установка трей иконки
            Program.notifyIcon = new NotifyIcon();
            Program.notifyIcon.Icon = Resources.net2;
            Program.notifyIcon.Visible = true;
            Program.notifyIcon.ContextMenuStrip = new ContextMenus().Create();
            Program.notifyIcon.Text = "Сетевой агент НИИ Телевидения";

            // Проверка наличия настроек в реестре
            RegCheck();

            // Узнаём имя треда, для удобства дебага
            Thread MyName = Thread.CurrentThread;
            if (MyName.Name == null)
                MyName.Name = "Main Program";


            gl_s_myHostName = Dns.GetHostName();

            gl_f_soft = new FormSoft();
            gl_s_userName = gl_s_myHostName + gl_s_OSdate;
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
            gl_s_md5Clients = MD5Code(gl_s_OSdate + gl_b_roleSecurity.ToString().ToLower() + gl_b_roleMessager.ToString().ToLower() + gl_b_roleOperate.ToString().ToLower() + gl_b_roleAdmin.ToString().ToLower() + gl_b_roleAgent.ToString().ToLower());
            gl_s_md5PortIp = MD5Code(gl_i_servPort + gl_s_servIP + gl_s_OSdate);
            gl_s_optionsPasswordReg = MD5Code(gl_s_optionsPasswordDefault + gl_s_OSdate);

            RegistryWork mainReg = new RegistryWork();
            RegistryWork updateReg = new RegistryWork("Update");
            gl_s_servIP = mainReg.ReadString("ip_server", gl_s_servIP);
            gl_i_servPort = mainReg.ReadInt("port_server", gl_i_servPort);
            gl_s_md5PortIp = mainReg.ReadString("validate_ip_port", gl_s_md5PortIp);
            gl_s_md5Clients = mainReg.ReadString("validate_clients", gl_s_md5Clients);
            gl_b_roleSecurity = mainReg.ReadBool("RoleSecurity", gl_b_roleSecurity);
            gl_b_roleMessager = mainReg.ReadBool("RoleMessager", gl_b_roleMessager);
            gl_b_roleOperate = mainReg.ReadBool("RoleOperate", gl_b_roleOperate);
            gl_b_roleAdmin = mainReg.ReadBool("RoleAdmin", gl_b_roleAdmin);
            gl_b_roleAgent = mainReg.ReadBool("RoleAgent", gl_b_roleAgent);
            gl_s_optionsPasswordReg = mainReg.ReadString("password", gl_s_optionsPasswordReg);
            gl_s_nanitSvcVer = updateReg.ReadString("nanitSvcVer", gl_s_nanitSvcVer);
            gl_i_itemsInList = 0;
            for (int j = 0; j < 11; j++)
            {
                gl_sMas_pathUpdate[j] = updateReg.ReadString("path_update_" + j.ToString(), "NULL");
                if (gl_sMas_pathUpdate[j] != "NULL")
                {
                    gl_sMas_pathUpdate[j] = UrlCorrect(gl_sMas_pathUpdate[j]);
                    gl_i_itemsInList++;
                }
                else
                    gl_sMas_pathUpdate[j] = null;
            }
            gl_i_updateIn = gl_i_itemsInList;

            mainReg.Exit();
            updateReg.Exit();
            

            if (gl_s_md5PortIp != MD5Code(gl_i_servPort + gl_s_servIP + gl_s_OSdate))
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
            if (gl_s_md5Clients != MD5Code(gl_s_OSdate + gl_b_roleSecurity.ToString().ToLower() + gl_b_roleMessager.ToString().ToLower() + gl_b_roleOperate.ToString().ToLower() + gl_b_roleAdmin.ToString().ToLower() + gl_b_roleAgent.ToString().ToLower()))
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

        

        public static void RefreshOpions()
        {
            if (gl_b_isOptOpen)
                gl_b_workLock = Revers(gl_b_workLock);
        }

        public static void AutoSelfInstall(bool check)
        {
            if (check)
            {
                string path = Path.GetPathRoot(Environment.SystemDirectory);
                string targetPath = path + @"Windows\services";
                string targetFileName = "nanit_" + gl_s_version + ".exe";
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
