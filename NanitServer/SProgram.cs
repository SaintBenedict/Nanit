using Microsoft.Win32;
using System;
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
        public static int servPort = 51782;
        public static FormSOptions form1 = null;
        public static bool isAboutLoaded = false;
        public static bool isOptOpen = false;
        public static bool isOptOpenStatic = true;
        public static int MessageIn = 0;
        public static int MessageInOld = 0;
        public static string MessageText = "";
        public static string ClientId = "";
    }
    class SProgram
    {
        public static NotifyIcon notifyIcon = null;
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.net2;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new SContextMenus().Create();
            notifyIcon.Text = "Сетевой сервер НИИ Телевидения";


            SFunctions.RegCheck();                       ///Проверка наличия настроек в реестре

            Thread t = Thread.CurrentThread;
            t.Name = "Main Program";

            Globals.form1 = new FormSOptions();
            Application.Run();
            
        }
        
        public static void CheckServiceUpdate(object obj)
        {
            Thread t2 = Thread.CurrentThread;
            t2.Name = "Ебучий таймер";
        }
    }

    
}