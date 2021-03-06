﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;

namespace NaNiT
{
    class ServiceWork
    {
        static object locker = new object();

        public static void DownlAndCheck(string urlAndJ, string name)
        {
            string slash = urlAndJ.Substring(urlAndJ.Length - 2, 1);
            string numb = null;
            string url = null;
            if (slash == @"/")
            {
                url = urlAndJ.Substring(0, urlAndJ.Length - 1);
                numb = urlAndJ.Substring(urlAndJ.Length - 1, 1);
            }
            else
            {
                url = urlAndJ.Substring(0, urlAndJ.Length - 2);
                numb = urlAndJ.Substring(urlAndJ.Length - 2, 2);
            }
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
                gl_i_adrUpdNum = Convert.ToInt32(numb);
                gl_s_updVerAvi = Mass[1].Substring(0, 5);
                Mass = null;
                ServiceInit();
            }
        //MessageBox.Show("Адрес " + tempUri + ", Номер " + numb + ", Файл " + newName); // Какие обработаны данные
        Finish:
            Thread.Sleep(8000);
            File.Delete(newName);
            gl_i_updateIn++;
        }

        public static void CheckUpdServer()
        {
            if (gl_i_updateIn < gl_i_itemsInList)
                goto End;
            gl_i_updateIn = 0;
            gl_i_adrUpdNum = -1;
            for (int j = 0; j < gl_i_itemsInList; j++)
            {
                if (Functions.UrlCorrect(gl_sMas_pathUpdate[j]) == null)
                {
                    gl_i_updateIn++;
                    continue;
                }
                string normalAdress = Functions.UrlCorrect(gl_sMas_pathUpdate[j]) + "/nanit/";
                string fileNameVer = "version.txt";
                string AdressToTranslate = normalAdress + j.ToString();
                Thread dnl = new Thread(delegate () { DownlAndCheck(AdressToTranslate, fileNameVer); });
                //MessageBox.Show("Адрес " + normalAdress + ", Файл " + fileNameVer + ", Номер " + j.ToString()); // Какие переданы данные
                if (dnl.Name == null)
                    dnl.Name = "Загрузка файла " + j;
                dnl.Start();
            }
        End:
            Thread.Sleep(100);
        }

        public static void ServiceInit()
        {
            if (!gl_b_serviceInitLock)
            {
                byte k = 0;
                gl_b_serviceInitLock = true;
                ServiceController[] scServices;
                scServices = ServiceController.GetServices();
                foreach (ServiceController scTemp in scServices)
                {
                    if (scTemp.ServiceName == "Nanit Updater")
                    {
                        ServiceController sc = new ServiceController("Nanit Updater");
                        k = 1;
                        string pathC = Path.GetPathRoot(Environment.SystemDirectory);
                        string targetPathC = pathC + @"Windows\services";
                        if (gl_s_nanitSvcVer == "0")
                            GoToHackMyself(targetPathC);
                        FileVersionInfo UpdVersionListen = FileVersionInfo.GetVersionInfo(targetPathC + @"\nanit-svc" + "_" + gl_s_nanitSvcVer + @".exe");
                        string str = UpdVersionListen.FileVersion.Substring(0, 5);
                        gl_s_nanitSvcVer = str;
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            gl_by_serviceStatus = 1;                                              // установлена и запущена
                            if (gl_s_nanitSvcVer != gl_s_updVerAvi)
                                gl_by_serviceStatus = 3;                                          // установлена и запущена есть обновление
                        }
                        else
                        {
                            gl_by_serviceStatus = 2;                                              // установлена не запущена
                            if (gl_s_nanitSvcVer != gl_s_updVerAvi)
                                gl_by_serviceStatus = 4;                                          // установлена не запущена есть обновление
                        }
                        File.Delete(@"InstallUtil.InstallLog");
                        goto EndServiceEach;
                    }
                    else
                    {
                        gl_by_serviceStatus = 0;                                                  // не установлена и не запущена
                    }
                }
            EndServiceEach:
                if (k == 0)
                    gl_s_nanitSvcVer = "0";
                RegistryKey localMachineKey = Registry.LocalMachine;
                RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
                RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
                RegistryKey updateKey = regNanit.CreateSubKey("Update");
                updateKey.SetValue("nanitSvcVer", gl_s_nanitSvcVer);
                regNanit.Close();
                updateKey.Close();
                Functions.RefreshOpions();                                                          // Функция обновления интерфейса формы настроек
                gl_b_serviceInitLock = false;
            }
        }

        public static void InstallService()
        {
            if (!gl_b_installLock)
            {
                gl_b_installLock = true;
                ServiceInit();
                string remoteUri = gl_sMas_pathUpdate[gl_i_adrUpdNum] + "/nanit/";
                string fileName = "nanit-svc.exe", installResource = null;
                string fileName2 = "nanit-svc" + "_" + gl_s_updVerAvi + @".exe";
                string fileName3 = "nanit-svc" + "_" + gl_s_nanitSvcVer + @".exe";
                WebClient myWebClient = new WebClient();
                installResource = remoteUri + fileName;
                string path = Path.GetPathRoot(Environment.SystemDirectory);
                string sourcePath = Application.StartupPath;
                string targetPath = path + @"Windows\services";
                string downlFile = Path.Combine(sourcePath, fileName);
                string sourceFile = Path.Combine(sourcePath, fileName2);
                string targetFile = Path.Combine(targetPath, fileName2);
                string oldFile = Path.Combine(targetPath, fileName3);
                Directory.CreateDirectory(targetPath);
                string InstSvc = path + @"Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe ";

                switch (gl_by_serviceStatus)
                {
                    case 0:
                        Functions.RefreshOpions();
                        myWebClient.DownloadFile(installResource, fileName);
                        myWebClient.Dispose();
                        File.Delete(sourceFile);
                        File.Move(downlFile, sourceFile);
                        File.Copy(sourceFile, targetFile, true);
                        gl_s_nanitSvcVer = gl_s_updVerAvi;
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
                        Functions.RefreshOpions();
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
                        Functions.RefreshOpions();
                        gl_b_installLock = false;
                        UpdateService();
                        break;
                    case 4:
                        if (gl_i_adrUpdNum == -1)
                            goto case 2;
                        else
                        {
                            Functions.RefreshOpions();
                            gl_b_installLock = false;
                            UpdateService();
                        }
                        break;
                }
                gl_b_installLock = false;
            }
        }

        public static void DeleteService()
        {
            if (!gl_b_installLock || gl_b_updateLock)
            {
                gl_b_installLock = true;
                gl_b_updateLock = false;
                ServiceInit();
                string fileName2 = "nanit-svc" + "_" + gl_s_updVerAvi + @".exe";
                string fileName3 = "nanit-svc" + "_" + gl_s_nanitSvcVer + @".exe";
                string fileName4 = "nanit-svc" + "_" + gl_s_nanitSvcVer + @".InstallLog";
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
                if (gl_s_updVerAvi == "1.0.0")
                    gl_s_nanitSvcVer = "0";
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
                gl_b_installLock = false;
                ServiceInit();
            }
        }

        public static void UpdateService()
        {
            if (!gl_b_installLock)
            {
                gl_b_installLock = true;
                gl_b_updateLock = true;
                DeleteService();
                Functions.RefreshOpions();
                string remoteUri = gl_sMas_pathUpdate[gl_i_adrUpdNum] + "/nanit/";
                string fileName = "nanit-svc.exe", myStringWebResourceU = null;
                string fileName2 = "nanit-svc" + "_" + gl_s_updVerAvi + @".exe";
                string fileName3 = "nanit-svc" + "_" + gl_s_nanitSvcVer + @".exe";
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
                Functions.RefreshOpions();
                gl_s_nanitSvcVer = gl_s_updVerAvi;
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
                gl_b_installLock = false;
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
                gl_s_nanitSvcVer = myPrettyVersion;
            }
            tempDir2 = null;
        }
    }
}
