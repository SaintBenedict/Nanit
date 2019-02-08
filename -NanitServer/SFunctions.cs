using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        public static int servPort = 51782, MessageIn = 0, MessageInOld = 0;
        public static FormSOptions form1 = null;
        public static bool isAboutLoaded = false, isOptOpen = false, isOptOpenStatic = true, myMessageNotAwait = false, disconnectInProgress = false;
        public static string MessageText = "";
        public static List<string> AutorisedRegistredClients = new List<string>();
    }

    class SFunctions
    {
        public static void FirstRunOptionsLoad()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Установка трей иконки
            SProgram.notifyIcon = new NotifyIcon();
            SProgram.notifyIcon.Icon = Properties.Resources.net2;
            SProgram.notifyIcon.Visible = true;
            SProgram.notifyIcon.ContextMenuStrip = new SContextMenus().Create();
            SProgram.notifyIcon.Text = "Сетевой сервер НИИ Телевидения";

            // Проверка наличия настроек в реестре
            RegCheck();

            // Узнаём имя треда, для удобства дебага
            Thread MyName = Thread.CurrentThread;
            if (MyName.Name == null)
                MyName.Name = "Main Program";


            Globals.form1 = new FormSOptions();
        }

        public static void Catch()
        {
        }

        public static bool Revers(bool first)
        {
            if (first == true)
                return false;
            else
                return true;
        }

        public static int ChangeMesIn(int first, string message)
        {
            Globals.MessageText = message;
            if (first == 500)
                return 501;
            else
                return 500;
        }

        public static string MD5Code(string getCode)
        {
            string result = null;
            try
            {
                byte[] hash = Encoding.ASCII.GetBytes(getCode);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] hashenc = md5.ComputeHash(hash);
                result = "";
                foreach (var b in hashenc)
                {
                    result += b.ToString("x2");
                }
            }
            catch (Exception ex) { MessageBox.Show("SFunctions(MD5) " + ex.Message); }
            return result;
        }

        public static void RegCheck()
        {
            try
            {
                RegistryKey localMachineKey = Registry.LocalMachine;
                RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
                RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
                RegistryKey servKey = regNanit.CreateSubKey("Update");

                Globals.servPort = Convert.ToInt32(CheckRegString("port_server", servKey, Globals.servPort.ToString()));

                string CheckRegString(string toRegName, RegistryKey toDo, string variant)
                {
                    string result = variant;
                    if (toDo.GetValue(toRegName) != null)
                        result = toDo.GetValue(toRegName).ToString();
                    else
                        toDo.SetValue(toRegName, variant);
                    return result;
                }

                servKey.Close();
                servKey = null;
                regNanit.Close();
                regNanit = null;
                localMachineSoftKey.Close();
                localMachineSoftKey = null;
                localMachineKey.Close();
                localMachineKey = null;
            }
            catch (Exception ex) { MessageBox.Show("SFunctions(RegCh) " + ex.Message); }
        }
    }
}
