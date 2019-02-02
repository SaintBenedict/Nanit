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
                regNanit.SetValue("password", Functions.MD5Code(Globals.optionsPasswordDefault));
                Globals.optionsPasswordReg = Functions.MD5Code(Globals.optionsPasswordDefault);
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
                Globals.optionsPasswordReg = regNanit.GetValue("password").ToString();
                Globals.nanitSvcVer = CheckRegString("nanitSvcVer", updateKey, Globals.nanitSvcVer);

                updateKey.SetValue("path_update_0", Globals.pathUpdate[0]);
                for (byte j = 1; j < 11; j++)
                {
                    if (updateKey.GetValue("path_update_" + j.ToString()).ToString() != "NULL")
                        Globals.pathUpdate[j] = updateKey.GetValue("path_update_" + j.ToString()).ToString();
                    else
                        Globals.pathUpdate[j] = null;
                }

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

                if (Globals.md5PortIp != Functions.MD5Code(Globals.servPort + Globals.servIP))
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
