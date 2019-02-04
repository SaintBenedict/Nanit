using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NaNiT
{
    class Functions
    {
        public static bool Revers(bool first)
        {
            if (first == true)
                return false;
            else
                return true;
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
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey updateKey = regNanit.CreateSubKey("Update");
            if (regNanit.GetValue("install") == null)
            {
                regNanit.SetValue("install", "1");
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
                for (byte j = 0; j < 11; j++)
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
                Globals.optionsPasswordReg = regNanit.GetValue("password").ToString();
                Globals.nanitSvcVer = CheckRegString("nanitSvcVer", updateKey, Globals.nanitSvcVer);
                Globals.itemsInList = 0;
                for (byte j = 0; j < 11; j++)
                {
                    if (updateKey.GetValue("path_update_" + j.ToString()).ToString() != "NULL")
                    {
                        Globals.pathUpdate[j] = updateKey.GetValue("path_update_" + j.ToString()).ToString();
                        Globals.pathUpdate[j] = UrlCorrect(Globals.pathUpdate[j]);
                        Globals.itemsInList++;
                    }
                    else
                        Globals.pathUpdate[j] = null;
                }
                Globals.updateIn = Globals.itemsInList;

                string CheckRegString(string toRegName, RegistryKey toDo, string variant)
                {
                    if (toDo.GetValue(toRegName) != null)
                        variant = toDo.GetValue(toRegName).ToString();
                    else
                        toDo.SetValue(toRegName, variant);
                    return variant;
                }

                regNanit.Close();
                updateKey.Close();

                if (Globals.md5PortIp != MD5Code(Globals.servPort + Globals.servIP))
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
                if (Globals.md5Clients != Functions.MD5Code(Globals.RoleSecurity.ToString().ToLower() + Globals.RoleMessager.ToString().ToLower() + Globals.RoleOperate.ToString().ToLower() + Globals.RoleAdmin.ToString().ToLower() + Globals.RoleAgent.ToString().ToLower()))
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
        }

        public static void AutoSelfInstall()
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
