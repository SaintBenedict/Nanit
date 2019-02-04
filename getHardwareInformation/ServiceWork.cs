﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NaNiT
{
    class ServiceWork
    {
        static object locker = new object();

        public static void DownlAndCheck(string url, string name, sbyte numb)
        {
            string newName = numb + name;
            Uri tempUri = new Uri(url + name);
            try
            {
                WebClient ClientDownl = new WebClient();
                ClientDownl.DownloadFile(tempUri, newName);
                ClientDownl.Dispose();
            }
            catch (WebException)
            { goto Finish; }
            Thread.Sleep(100);
            string[] Mass = File.ReadAllLines(newName, Encoding.Default);
            if (Mass.Length == 0)
                goto Finish;
            if (Mass[0] == "version-nanit-service")
            {
                Globals.adrUpdNum = numb;
                Globals.updVerAvi = Mass[1].Substring(0, 5);
                Mass = null;
            }
        Finish:
            Thread.Sleep(8000);
            File.Delete(newName);
            Globals.updateIn++;
        }

        public static void CheckUpdServer()
        {
            if (Globals.updateIn < Globals.itemsInList)
                goto End;
            Globals.updateIn = 0;
            Globals.adrUpdNum = -1;
            for (sbyte j = 0; j < Globals.itemsInList; j++)
            {
                if (Functions.UrlCorrect(Globals.pathUpdate[j]) == null)
                {
                    Globals.updateIn++;
                    continue;
                }
                string normalAdress = Functions.UrlCorrect(Globals.pathUpdate[j]) + "/nanit/";
                string fileNameVer = "version.txt";
                Thread dnl = new Thread(delegate () { DownlAndCheck(normalAdress, fileNameVer, j); });
                dnl.Name = "Загрузка файла " + j;
                dnl.Start();
            }
        End:
            Thread.Sleep(100);
        }

        public static void ServiceInit()
        {
            lock (locker)
            {
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
                        if (Globals.nanitSvcVer == "0")
                            GoToHackMyself(targetPath);
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

                if (Globals.work == 30)
                    Globals.work = 40;
                else
                    Globals.work = 30;
                MessageBox.Show("НУ ПИЗДЕЦ " + Globals.work.ToString());
                //Thread.Sleep(2000);
            }
        }

        public static void InstallService()
        {
            lock (locker)
            {
                ServiceInit();
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
        }

        public static void DeleteService()
        {
            lock (locker)
            {
                ServiceInit();
                string fileName2 = "nanit-svc" + "_" + Globals.updVerAvi + @".exe";
                string fileName3 = "nanit-svc" + "_" + Globals.nanitSvcVer + @".exe";
                string fileName4 = "nanit-svc" + "_" + Globals.nanitSvcVer + @".InstallLog";
                string path = Path.GetPathRoot(Environment.SystemDirectory);
                string targetPath = path + @"Windows\services";
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
            End:
                if (Globals.updVerAvi == "1.0.0")
                    Globals.nanitSvcVer = "0";
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
                ServiceInit();
            }
        }

        public static void UpdateService()
        {
            lock (locker)
            {
                DeleteService();
                string remoteUri = Globals.pathUpdate[Globals.adrUpdNum] + "/nanit/";
                string fileName = "nanit-svc.exe", myStringWebResourceU = null;
                string fileName2 = "nanit-svc" + "_" + Globals.updVerAvi + @".exe";
                string fileName3 = "nanit-svc" + "_" + Globals.nanitSvcVer + @".exe";
                WebClient myWebClient = new WebClient();
                myStringWebResourceU = remoteUri + fileName;
                string path = Path.GetPathRoot(Environment.SystemDirectory);
                string targetPath = path + @"Windows\services";
                string sourcePath = Application.StartupPath;
                string downlFile = Path.Combine(sourcePath, fileName);
                string sourceFile = Path.Combine(sourcePath, fileName2);
                string targetFile = Path.Combine(targetPath, fileName2);
                string oldFile = Path.Combine(targetPath, fileName3);
                Directory.CreateDirectory(targetPath);
                string InstSvc = path + @"Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe ";
                myWebClient.DownloadFile(myStringWebResourceU, fileName);
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
        }

        public static void GoToHackMyself(string targetPath)
        {
            string[] tempDir2 = Directory.GetFiles(targetPath, "nanit-svc*");
            foreach (string notEvenPretty in tempDir2)
            {
                string myPrettyFile = Path.GetFileName(notEvenPretty);
                int notEvenPrettyLong = myPrettyFile.Length;
                if (notEvenPrettyLong > 20)
                    continue;
                int PrettyLong = myPrettyFile.Length - 14;
                string myPrettyVersion = myPrettyFile.Substring(10, PrettyLong);
                Globals.nanitSvcVer = myPrettyVersion;
            }
            tempDir2 = null;
        }
    }
}
